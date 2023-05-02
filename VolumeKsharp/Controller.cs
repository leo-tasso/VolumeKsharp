using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace VolumeKsharp;

public class Controller
{
    static readonly Queue<InputCommands> InputCommandsQueue = new Queue<InputCommands>();
    private const float StepSize = 2;
    private const double Tolerance = 1;


    public Controller()
    {
        var volume = new Volume();
        var serialcom = new SerialCom(this);
        bool on = false;
        var sw = Stopwatch.StartNew();
        long swo = 0;
        float oldVolume = 0;
        while (true)
        {
            if (InputCommandsQueue.Count > 0)
            {
                if (volume != null)
                {
                    oldVolume = Convert.ToInt32(volume.GetVolume());
                    switch (InputCommandsQueue.Dequeue())
                    {
                        case InputCommands.Minus:
                        {
                            volume.SetVolume(volume.GetVolume() - StepSize);
                            break;
                        }
                        case InputCommands.Plus:
                            volume.SetVolume(volume.GetVolume() + StepSize);
                            break;
                        case InputCommands.Press:
                            break;
                        case InputCommands.Release:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            if (volume != null && Math.Abs(oldVolume - Convert.ToInt32(volume.GetVolume())) > Tolerance)
            {
                swo = sw.ElapsedMilliseconds;
                serialcom?.AddCommand(new PercentageAppearanceCommand(Convert.ToInt32(volume.GetVolume())));
                oldVolume = Convert.ToInt32(volume.GetVolume());
                on = true;
            }

            if (sw.ElapsedMilliseconds - swo > 500 && on)
            {
                serialcom?.AddCommand(new PercentageAppearanceCommand(Convert.ToInt32(0)));
                on = false;
            }
            Thread.Sleep(20);
 
        }
    }

    public void AddInputCommand(InputCommands command)
    {
        InputCommandsQueue.Enqueue(command);
    }
}