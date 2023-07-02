// <copyright file="SerialCom.cs" company="LeonardoTassinari">
// Copyright (c) LeonardoTassinari. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace VolumeKsharp;

using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

/// <summary>
/// Class to dialog serially with the knob.
/// </summary>
public class SerialCom
{
    private static readonly Queue<IAppearanceCommand> AppearanceCommandsQueue = new Queue<IAppearanceCommand>();
    private static readonly object CommandQueueLock = new object();

    private static readonly SerialPort SerialPort = new SerialPort();
    private readonly Controller controller;
    private Thread readThread;
    private Thread writeThread;

    /// <summary>
    /// Initializes a new instance of the <see cref="SerialCom"/> class.
    /// </summary>
    /// <param name="controller">The reference controller.</param>
    public SerialCom(Controller controller)
    {
        this.controller = controller;
        this.readThread = new Thread(this.Read);
        this.writeThread = new Thread(this.Write);
        SerialPort.PortName = "com7";
        SerialPort.ReadTimeout = 500;
        SerialPort.WriteTimeout = 500;
        SerialPort.DtrEnable = true;
    }

    /// <summary>
    /// Gets or sets selected Port.
    /// </summary>
    public string Port
    {
        get => SerialPort.PortName;
        set => SerialPort.PortName = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the connection is running.
    /// </summary>
    public bool Running { get; set; }

/// <summary>
/// Method to start the serial communications.
/// </summary>
    public void Start()
    {
        if (this.Running == false)
        {
            this.Running = true;
            SerialPort.Open();
            this.readThread = new Thread(this.Read);
            this.writeThread = new Thread(this.Write);
            this.readThread.Start();
            this.writeThread.Start();
        }
    }

/// <summary>
/// Method to get currently connected serial ports.
/// </summary>
/// <returns>Array of string of the names of the connected ports.</returns>
    public string[] GetPorts()
    {
        return SerialPort.GetPortNames();
    }

/// <summary>
    /// Method to add a command to send to the knob.
    /// </summary>
    /// <param name="command">The command to send to the knob.</param>
    public void AddCommand(IAppearanceCommand command)
    {
        lock (CommandQueueLock)
        {
            AppearanceCommandsQueue.Enqueue(command);
            Monitor.Pulse(CommandQueueLock);
        }
    }

    /// <summary>
    /// Method to stop the serial communication.
    /// </summary>
    /// <returns>If it stopped correctly.</returns>
    public bool Stop()
    {
        this.Running = false;
        this.readThread.Join();
        lock (CommandQueueLock)
        {
            Monitor.Pulse(CommandQueueLock);
        }

        this.writeThread.Join();
        SerialPort.Close();
        SerialPort.Dispose();
        return true;
    }

    private void Write()
    {
        while (this.Running)
        {
            lock (CommandQueueLock)
            {
                while (AppearanceCommandsQueue.Count == 0 && this.Running)
                {
                    Monitor.Wait(CommandQueueLock);
                }

                if (!this.Running)
                {
                    break;
                }

                string? message = AppearanceCommandsQueue.Dequeue().Message;
                if (message != null)
                {
                    SerialPort.WriteLine(message);
                }
            }
        }
    }

    private void Read()
    {
        while (this.Running)
        {
            try
            {
                string message = SerialPort.ReadLine();
                Console.WriteLine(message);
                if (message.Equals("-\r"))
                {
                    this.controller.AddInputCommand(InputCommands.Minus);
                }
                else if (message.Equals("+\r"))
                {
                    this.controller.AddInputCommand(InputCommands.Plus);
                }
                else if (message.Equals("0\r"))
                {
                    this.controller.AddInputCommand(InputCommands.Release);
                }
                else if (message.Equals("o\r"))
                {
                    this.controller.AddInputCommand(InputCommands.Press);
                }
            }
            catch (TimeoutException)
            {
            }
        }
    }
}