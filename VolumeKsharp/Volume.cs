// <copyright file="Volume.cs" company="LeonardoTassinari">
// Copyright (c) LeonardoTassinari. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace VolumeKsharp;

using System;
using NAudio.CoreAudioApi;

/// <summary>
/// Class to get and set the system audio volume.
/// </summary>
public class Volume
{
    private static readonly MMDeviceEnumerator Enumerator = new MMDeviceEnumerator();
    private readonly MMDevice device = Enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

/// <summary>
/// Method to get the current volume.
/// </summary>
/// <returns>The current volume.</returns>
    public float GetVolume()
    {
        var currentVolume = this.device.AudioEndpointVolume.MasterVolumeLevelScalar;
        return currentVolume * 100;
    }

/// <summary>
/// Method to set the audio volume.
/// </summary>
/// <param name="desiredVolume"> The percentage of the volume to set.</param>
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

        this.device.AudioEndpointVolume.MasterVolumeLevelScalar = desiredVolume;
        Console.WriteLine("System volume set to " + (desiredVolume * 100) + "%");
    }
}