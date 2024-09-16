// <copyright file="ICommunicator.cs" company="LeonardoTassinari">
// Copyright (c) LeonardoTassinari. All rights reserved.
// </copyright>

namespace VolumeKsharp.Communicator;

using AppearanceCommands;

/// <summary>
/// Interface of a Communicator, the api to exchange commands with the device.
/// </summary>
public interface ICommunicator
{
    /// <summary>
    /// Gets or sets selected Port.
    /// </summary>
    string Port { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the connection is running.
    /// </summary>
    bool Running { get; set; }

    /// <summary>
    /// Method to start the serial communications.
    /// </summary>
    void Start();

    /// <summary>
    /// Method to get currently connected serial ports.
    /// </summary>
    /// <returns>Array of string of the names of the connected ports.</returns>
    string[] GetPorts();

    /// <summary>
    /// Method to add a command to send to the knob.
    /// </summary>
    /// <param name="command">The command to send to the knob.</param>
    void AddCommand(IAppearanceCommand command);

    /// <summary>
    /// Method to stop the serial communication.
    /// </summary>
    /// <returns>If it stopped correctly.</returns>
    bool Stop();
}