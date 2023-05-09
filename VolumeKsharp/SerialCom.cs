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
    private readonly Thread readThread;
    private readonly Thread writeThread;

    /// <summary>
    /// Initializes a new instance of the <see cref="SerialCom"/> class.
    /// </summary>
    /// <param name="controller">The reference controller.</param>
    public SerialCom(Controller controller)
    {
        this.controller = controller;
        this.readThread = new Thread(this.Read);
        this.writeThread = new Thread(Write);
        SerialPort.PortName = "com3";
        SerialPort.ReadTimeout = 500;
        SerialPort.WriteTimeout = 500;
        SerialPort.DtrEnable = true;
        SerialPort.Open();
        Continue = true;
    }

    public void Start()
    {
        this.readThread.Start();
        this.writeThread.Start();
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
/// Method to set the port to transmit to.
/// </summary>
/// <param name="portName">The string name of the port to transmit to.</param>
    public void SetPort(string portName)
{
    SerialPort.PortName = portName;
}

    private static bool Continue { get; set; }

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
        this.readThread.Join();
        this.writeThread.Join();
        SerialPort.Close();
        return true;
    }

    private static void Write()
    {
        while (Continue)
        {
            lock (CommandQueueLock)
            {
                while (AppearanceCommandsQueue.Count == 0)
                {
                    Monitor.Wait(CommandQueueLock);
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
        while (Continue)
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
            }
            catch (TimeoutException)
            {
            }
        }
    }
}