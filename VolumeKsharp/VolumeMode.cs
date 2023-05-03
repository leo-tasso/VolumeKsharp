using System;
using System.Diagnostics;

namespace VolumeKsharp;

public class VolumeMode : IMode
{
    private bool _on;
    private readonly Stopwatch _sw = Stopwatch.StartNew();
    private long _swo;
    private float _oldVolume;
    private const float StepSize = 2;
    private const double Tolerance = 1;
    private readonly Volume _volume = new Volume();
    private readonly SerialCom _serialcom;

    public VolumeMode(SerialCom serialcom)
    {
        this._serialcom = serialcom;
    }

    public bool IncomingCommands(InputCommands command)
    {
        _oldVolume = Convert.ToInt32(_volume.GetVolume());
        switch (command)
        {
            case InputCommands.Minus:
            {
                _volume.SetVolume(_volume.GetVolume() - StepSize);
                break;
            }
            case InputCommands.Plus:
                _volume.SetVolume(_volume.GetVolume() + StepSize);
                break;
            case InputCommands.Press:
                return false;
            case InputCommands.Release:
                return false;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return true;
    }

    public void Compute()
    {
        if (Math.Abs(_oldVolume - Convert.ToInt32(_volume.GetVolume())) > Tolerance)
        {
            _swo = _sw.ElapsedMilliseconds;
            _serialcom.AddCommand(new PercentageAppearanceCommand(Convert.ToInt32(_volume.GetVolume())));
            _oldVolume = Convert.ToInt32(_volume.GetVolume());
            _on = true;
        }

        if (_sw.ElapsedMilliseconds - _swo > 500 && _on)
        {
            _serialcom.AddCommand(new PercentageAppearanceCommand(Convert.ToInt32(0)));
            _on = false;
        }
    }
}