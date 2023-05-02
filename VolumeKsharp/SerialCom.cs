using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

namespace VolumeKsharp;

public class SerialCom
{
    private static readonly Queue<IAppearanceCommand> AppearanceCommandsQueue = new Queue<IAppearanceCommand>();
    private static readonly object CommandQueueLock = new object();

    private static bool Continue { get; set; }
    private static readonly SerialPort SerialPort = new SerialPort();
    private readonly Controller _controller;
    private readonly Thread _readThread;
    private readonly Thread _writeThread;

    public SerialCom(Controller controller)
    {
        this._controller = controller;
        _readThread = new Thread(this.Read);
        _writeThread = new Thread(Write);
        SerialPort.PortName = "com3";
        SerialPort.ReadTimeout = 500;
        SerialPort.WriteTimeout = 500;
        SerialPort.DtrEnable = true;
        SerialPort.Open();
        Continue = true;
        _readThread.Start();
        _writeThread.Start();
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
                if (message != null) SerialPort.WriteLine(message);
            }
        }
    }

    public void AddCommand(IAppearanceCommand command)
    {
        lock (CommandQueueLock)
        {
            AppearanceCommandsQueue.Enqueue(command);
            Monitor.Pulse(CommandQueueLock);
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
                    _controller.AddInputCommand(InputCommands.Minus);
                }
                else if (message.Equals("+\r"))
                {
                    _controller.AddInputCommand(InputCommands.Plus);
                }
            }
            catch (TimeoutException)
            {
            }
        }
    }

    public bool Stop()
    {
        _readThread.Join();
        _writeThread.Join();
        SerialPort.Close();
        return true;
    }
}