using System.Collections.Generic;
using System.Threading;

namespace VolumeKsharp;

public class Controller
{
    static readonly Queue<InputCommands> InputCommandsQueue = new Queue<InputCommands>();
    private bool Continue { get; set; }


    public IMode Mode { get; set; }

    public Controller()
    {
        Continue = true;
        var serialcom = new SerialCom(this);
        Mode = new VolumeMode(serialcom);
        while (Continue)
        {
            if (InputCommandsQueue.Count > 0)
            {
                Mode.IncomingCommands(InputCommandsQueue.Dequeue());
            }
            Mode.Compute();
            Thread.Sleep(20);
 
        }
    }



    public void AddInputCommand(InputCommands command)
    {
        InputCommandsQueue.Enqueue(command);
    }
}