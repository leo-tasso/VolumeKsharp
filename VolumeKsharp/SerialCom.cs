using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

namespace VolumeKsharp;

public class SerialCom
{
    private static readonly Queue<IApperanceCommand> ApperanceCommandsQueue = new Queue<IApperanceCommand>();
    private static readonly object CommandQueueLock = new object();
    
    private const float StepSize = 2;
    public static bool Continue { get; set; }
    private static readonly SerialPort SerialPort = new SerialPort();
    private readonly Controller _controller;
    
    public SerialCom(Controller controller)
    {
        this._controller = controller;
    }

    public static void Main()
    {
        var readThread = new Thread(Read);
        var writeThread = new Thread(Write);
        SerialPort.PortName = "com3";
        SerialPort.ReadTimeout = 500;
        SerialPort.WriteTimeout = 500;
        SerialPort.DtrEnable = true;
        SerialPort.Open();
        Continue = true;
        readThread.Start();
        writeThread.Start();
        readThread.Join();
        writeThread.Join();
        SerialPort.Close();
    }

    private static void Write(){
        while (Continue)
        {
            lock (CommandQueueLock)
            {
                while (ApperanceCommandsQueue.Count == 0)
                {
                    Monitor.Wait(CommandQueueLock);
                }
                string? message = ApperanceCommandsQueue.Dequeue().Message;
                if (message != null) SerialPort.WriteLine(message);
            }

            
        }
    }
    

    public static void AddCommand(IApperanceCommand command)
    {
        lock (CommandQueueLock)
        {
            ApperanceCommandsQueue.Enqueue(command);
            Monitor.Pulse(CommandQueueLock);
        }
    }

    private static void Read()
    {
        Volume volume = new Volume();

        while (Continue)
        {
            try
            {
                string message = SerialPort.ReadLine();
                Console.WriteLine(message);
                if (message.Equals("-\r"))
                {
                    volume.SetVolume(volume.GetVolume() - StepSize);
                }
                else if (message.Equals("+\r"))
                {
                    volume.SetVolume(volume.GetVolume() + StepSize);
                }
                AddCommand(new PercentageApperanceCommand(Convert.ToInt32(volume.GetVolume())));
            }
            catch (TimeoutException)
            {
            }
        }
    }
    
}