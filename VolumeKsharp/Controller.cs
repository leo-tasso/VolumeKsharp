// <copyright file="Controller.cs" company="LeonardoTassinari">
// Copyright (c) LeonardoTassinari. All rights reserved.
// </copyright>

namespace VolumeKsharp;

using System.Collections.Generic;
using System.Threading;
using VolumeKsharp.Communicator;
using VolumeKsharp.Light;
using VolumeKsharp.Mode;

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
        this.Communicator = new SerialCom(this);
        this.Communicator.Start();
        this.LightRgbwEffect = new LightRgbwEffect(this);
        new Thread(this.Update).Start();
    }

    /// <summary>
    /// Gets or sets the controller light.
    /// </summary>
    public ILightRgbwEffect LightRgbwEffect { get; set; }

    /// <summary>
    /// Gets the serialCommunication class of this Controller.
    /// </summary>
    public ICommunicator Communicator { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the state of the controller thread.
    /// </summary>
    public bool Continue { get; set; }

    private Mode.Mode? LastMode { get; set; }

    /// <summary>
    /// Method used externally to add commands.
    /// </summary>
    /// <param name="command"> The command to be added.</param>
    public void AddInputCommand(InputCommands command)
    {
        InputCommandsQueue.Enqueue(command);
    }

    private void AddMode(Mode.Mode newMode)
    {
        this.LastMode?.StackMode(newMode);
        this.LastMode = newMode;
    }

    private void Update()
    {
        this.AddMode(new MqttLight(this));
        this.AddMode(new VolumeMode(this));
        while (this.Continue)
        {
            if (InputCommandsQueue.Count > 0)
            {
                this.LastMode?.IncomingCommands(InputCommandsQueue.Dequeue());
            }

            this.LastMode?.Compute();
            Thread.Sleep(10);
        }
    }
}