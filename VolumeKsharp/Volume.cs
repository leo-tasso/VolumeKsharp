using System;
using NAudio.CoreAudioApi;

namespace VolumeKsharp;

public class Volume
{
    static readonly MMDeviceEnumerator Enumerator = new MMDeviceEnumerator();
    readonly MMDevice _device = Enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

    public float GetVolume()
    {
        var currentVolume = _device.AudioEndpointVolume.MasterVolumeLevelScalar;
        return currentVolume * 100;
    }

    public void SetVolume(float desiredVolume)
    {
        desiredVolume /= 100;
        if (desiredVolume < 0)
        {
            desiredVolume = 0;
        }
        else if (desiredVolume > 1)
        {
            desiredVolume = 1;
        }
        _device.AudioEndpointVolume.MasterVolumeLevelScalar = desiredVolume;
            Console.WriteLine("System volume set to " + (desiredVolume * 100) + "%");
        
    }
}