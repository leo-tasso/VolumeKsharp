// <copyright file="RgbwLightMqttClient.cs" company="LeonardoTassinari">
// Copyright (c) LeonardoTassinari. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace VolumeKsharp;

using System;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using Newtonsoft.Json.Linq;

/// <summary>
/// Class to connect to a mqtt broker to manage the light.
/// </summary>
public class RgbwLightMqttClient
{
    private readonly RgbwLight rgbwLight;
    private readonly IManagedMqttClient mqttClient;
    private readonly string clientId;
    private readonly string baseTopic;

    /// <summary>
    /// Initializes a new instance of the <see cref="RgbwLightMqttClient"/> class.
    /// </summary>
    /// <param name="brokerIpAddress">The Ip of the Broker.</param>
    /// <param name="brokerPort">The Port of the Broker.</param>
    /// <param name="clientId">Your client Id.</param>
    /// <param name="baseTopic">The base topic.</param>
    /// <param name="rgbwLight">The target Light.</param>
    public RgbwLightMqttClient(
        string brokerIpAddress,
        int brokerPort,
        string clientId,
        string baseTopic,
        RgbwLight rgbwLight)
    {
        this.clientId = clientId;
        this.baseTopic = baseTopic;
        this.rgbwLight = rgbwLight;
        var options = new ManagedMqttClientOptionsBuilder()
            .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
            .WithClientOptions(new MqttClientOptionsBuilder()
                .WithTcpServer(brokerIpAddress, brokerPort)
                .WithClientId(clientId)
                .Build())
            .Build();

        this.mqttClient = new MqttFactory().CreateManagedMqttClient();
        this.mqttClient.StartAsync(options);
        this.mqttClient.SubscribeAsync($"{this.baseTopic}/set");
        this.mqttClient.ConnectedAsync += _ => this.OnMqttConnected();
        this.mqttClient.DisconnectedAsync += _ => this.OnMqttDisconnected();
        this.mqttClient.ApplicationMessageReceivedAsync += (s) =>
        {
            this.ProcessCommand(Encoding.UTF8.GetString(s.ApplicationMessage.PayloadSegment));
            return null;
        };
    }

    /// <summary>
    /// Method to publish a message with the updated state.
    /// </summary>
    /// <param name="light">The relative light.</param>
    public async void UpdateState(RgbwLight light)
    {
        string stateTopic = $"homeassistant/light/{this.clientId}/state";
        string statePayload = string.Format(
            @"{{
    ""state"": ""{0}"",
    ""color_mode"": ""rgbw"",
    ""brightness"":{1},
    ""color"":{2}
}}",
            light.State ? "ON" : "OFF",
            light.Brightness,
            $"{{\"r\":{light.R},\"g\":{light.G},\"b\":{light.B},\"w\":{light.W}}}");
        await this.mqttClient.EnqueueAsync(new MqttApplicationMessageBuilder()
            .WithTopic(stateTopic)
            .WithPayload(statePayload)
            .WithRetainFlag()
            .Build());
    }

    private async Task OnMqttConnected()
    {
        await this.SubscribeToTopics();
        await this.PublishAutodiscovery();
    }

    private Task OnMqttDisconnected()
    {
        return Task.CompletedTask;
    }

    private async Task SubscribeToTopics()
    {
        string commandTopic = $"{this.baseTopic}/command";
        await this.mqttClient.SubscribeAsync(commandTopic);
    }

    private async Task PublishAutodiscovery()
    {
        string discoveryTopic = $"homeassistant/light/{this.clientId}/config";
        string discoveryPayload = this.GenerateAutodiscoveryPayload();
        await this.mqttClient.EnqueueAsync(new MqttApplicationMessageBuilder()
            .WithTopic(discoveryTopic)
            .WithPayload(discoveryPayload)
            .WithRetainFlag()
            .Build());
    }

    private string GenerateAutodiscoveryPayload()
    {
        string payload = $@"{{
    ""schema"":""json"",
    ""name"":""{this.clientId}"",
    ""unique_id"":""{this.clientId}"",
    ""command_topic"":""{$"{this.baseTopic}/set"}"",
    ""state_topic"":""{$"{this.baseTopic}/state"}"",
    ""optimistic"": false,
    ""brightness"":true,
    ""color_mode"":true,
    ""supported_color_modes"":[""rgbw""]
}}";
        return payload;
    }

    private void ProcessCommand(string command)
    {
        // Process the received command
        // Parse the command payload (in JSON format) and perform the necessary actions
        // Example:
        // Parse the command payload (in JSON format)
        var payloadObject = JObject.Parse(command);
        string state = payloadObject.Value<string>("state") ?? string.Empty;
        var brightness = payloadObject.Value<int?>("brightness");
        var colorObject = payloadObject.Value<JObject?>("color");
        int? red = null;
        int? green = null;
        int? blue = null;
        int? white = null;
        if (colorObject != null)
        {
            red = colorObject.Value<int>("r");
            green = colorObject.Value<int>("g");
            blue = colorObject.Value<int>("b");
            white = colorObject.Value<int>("w");
        }

        this.rgbwLight.State = state.Equals("ON");

        if (brightness != null)
        {
            this.rgbwLight.Brightness = (int)brightness;
        }

        if (red != null)
        {
            this.rgbwLight.R = (int)red;
        }

        if (green != null)
        {
            this.rgbwLight.G = (int)green;
        }

        if (blue != null)
        {
            this.rgbwLight.B = (int)blue;
        }

        if (white != null)
        {
            this.rgbwLight.W = (int)white;
        }
    }
}