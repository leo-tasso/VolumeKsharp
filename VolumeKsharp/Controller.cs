// <copyright file="Controller.cs" company="LeonardoTassinari">
// Copyright (c) LeonardoTassinari. All rights reserved.
// </copyright>

namespace VolumeKsharp;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Communicator;
using Light;
using Mode;

/// <summary>
/// The controller class.
/// </summary>
public class Controller
{
    private static readonly Queue<InputCommands> InputCommandsQueue = new();
    private readonly TrayIconMenu trayIcon;
    private readonly Thread updater;

    /// <summary>
    /// Initializes a new instance of the <see cref="Controller"/> class.
    /// </summary>
    public Controller()
    {
        this.Running = true;
        this.Communicator = new SerialCom(this);
        this.Communicator.Start();
        this.LightRgbwEffect = new LightRgbwEffect(this);
        this.trayIcon = new TrayIconMenu();
        this.trayIcon.ContextMenuThread(this);
        this.updater = new Thread(this.Update);
        this.updater.Start();
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
    private bool Running { get; set; }

    private Mode.Mode? LastMode { get; set; }

    /// <summary>
    /// Method used externally to add commands.
    /// </summary>
    /// <param name="command"> The command to be added.</param>
    public void AddInputCommand(InputCommands command)
    {
        InputCommandsQueue.Enqueue(command);
    }

    /// <summary>
    /// Method to stop all the threads of the controller and close the tray icon.
    /// </summary>
    public void Stop()
    {
        this.Running = false;
        this.Communicator.Stop();
        ActivePrograms.GetInstance().Stop();
        this.trayIcon.Stop();
        this.updater.Join();
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
        while (this.Running)
        {
            // Mockup to show the conditional add of new modes. (do also removal)
            if ((ActivePrograms.GetInstance().ActiveApps ?? Array.Empty<string>()).Contains("light"))
            {
                this.AddMode(new MqttLight(this));
            }

            if (InputCommandsQueue.Count > 0)
            {
                this.LastMode?.IncomingCommands(InputCommandsQueue.Dequeue());
            }

            this.LastMode?.Compute();
            Thread.Sleep(10);
        }
    }
}