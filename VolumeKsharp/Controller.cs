// <copyright file="Controller.cs" company="LeonardoTassinari">
// Copyright (c) LeonardoTassinari. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace VolumeKsharp;

using System.Collections.Generic;
using System.Threading;

/// <summary>
/// The controller class.
/// </summary>
public class Controller
{
    private static readonly Queue<InputCommands> InputCommandsQueue = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Controller"/> class.
    /// </summary>
    public Controller()
    {
        this.Continue = true;
        this.Serialcom = new SerialCom(this);
        this.Serialcom.Start();
        this.Light = new();
        this.RgbwLightMqttClient = new RgbwLightMqttClient("192.168.1.26", 1883, "volumeK", "homeassistant/light/volumeK", this.Light);
        new Thread(this.Update).Start();
    }

    /// <summary>
    /// Gets or sets the mqtt api.
    /// </summary>
    public RgbwLightMqttClient RgbwLightMqttClient { get; set; }

    /// <summary>
    /// Gets or sets the controller light.
    /// </summary>
    public Light Light { get; set; }

    /// <summary>
    /// Gets the serialCommunication class of this Controller.
    /// </summary>
    public SerialCom Serialcom { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the state of the controller thread.
    /// </summary>
    public bool Continue { get; set; }

    private IMode? Mode { get; set; }

    /// <summary>
    /// Method used externally to add commands.
    /// </summary>
    /// <param name="command"> The command to be added.</param>
    public void AddInputCommand(InputCommands command)
    {
        InputCommandsQueue.Enqueue(command);
    }

    private void Update()
    {
        this.Mode = new VolumeMode(this);
        while (this.Continue)
        {
            if (InputCommandsQueue.Count > 0)
            {
                this.Mode.IncomingCommands(InputCommandsQueue.Dequeue());
            }

            this.Mode.Compute();
            Thread.Sleep(10);
        }
    }
}