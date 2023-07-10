// <copyright file="VolumeMode.cs" company="LeonardoTassinari">
// Copyright (c) LeonardoTassinari. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace VolumeKsharp;

using System;
using System.Diagnostics;
using System.Threading.Tasks;

/// <summary/> mode to show volume status on the ring.
public class VolumeMode : IMode
{
    private const int ShowTime = 500;
    private const double ChangeRate = 1;
    private const float StepSize = 2;
    private const double Tolerance = 1;
    private readonly SerialCom serialcom;
    private readonly Stopwatch sw = Stopwatch.StartNew();
    private readonly Stopwatch swMuted = Stopwatch.StartNew();
    private readonly Volume volume = new();
    private Light lightOld;
    private double volumeShown;
    private bool showing;
    private bool muted;
    private bool mutedOld;

    /// <summary>
    /// Initializes a new instance of the <see cref="VolumeMode"/> class.
    /// </summary>
    /// <param name="calligController">The controller father.</param>
    public VolumeMode(Controller calligController)
    {
        this.CalligController = calligController;
        this.serialcom = calligController.Serialcom;
        this.lightOld = new Light(calligController);
        calligController.RgbwLightMqttClient.UpdateState(calligController.Light);
    }

    private Controller CalligController { get; set; }

    /// <inheritdoc/>
    public bool IncomingCommands(InputCommands command)
    {
        this.sw.Restart();
        this.mutedOld = this.muted;
        switch (command)
        {
            case InputCommands.Minus:
                this.volume.SetVolume(this.volume.GetVolume() - StepSize);
                break;

            case InputCommands.Plus:
                this.volume.SetVolume(this.volume.GetVolume() + StepSize);
                break;

            case InputCommands.Press:
                this.muted = !this.muted;
                this.volume.Muted = this.muted;
                this.swMuted.Restart();
                break;

            case InputCommands.Release:
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        if (this.swMuted.ElapsedMilliseconds > ShowTime)
        {
            this.ShowVolume();
        }

        return true;
    }

    /// <inheritdoc/>
    public Task Compute()
    {
        if (!this.lightOld.Equals(this.CalligController.Light))
        {
            this.CalligController.RgbwLightMqttClient.UpdateState(this.CalligController.Light);
            if (!this.showing)
            {
                this.CalligController.Light.UpdateLight();
            }

            this.lightOld = new Light(this.CalligController.Light, this.CalligController);
        }

        // Stops clocks to avoid overflows.
        if (this.swMuted is { ElapsedMilliseconds: > ShowTime, IsRunning: true })
        {
            this.swMuted.Stop();
        }

        if (this.sw is { ElapsedMilliseconds: > ShowTime, IsRunning: true })
        {
            this.sw.Stop();
        }

        // If not showing muted, show volume status.
        if (this.swMuted.ElapsedMilliseconds > ShowTime)
        {
            if (Math.Abs(this.volume.GetVolume() - this.volumeShown) > Tolerance)
            {
                this.ShowVolume();
            }
        }

        // Show mute status change.
        if (this.mutedOld != this.muted)
        {
            if (this.muted)
            {
                this.serialcom.AddCommand(new SolidAppearanceCommand(255, 0, 0, 0));
                this.showing = true;
            }
            else
            {
                this.ShowVolume();
            }
        }

        // Turn off after having shown the volume.
        if (this.sw.ElapsedMilliseconds > ShowTime && this.showing)
        {
            this.CalligController.Light.UpdateLight();
            this.showing = false;
        }

        return Task.CompletedTask;
    }

    private void ShowVolume()
    {
        this.sw.Restart();
        if (this.volumeShown < this.volume.GetVolume())
        {
            this.volumeShown += ChangeRate;
        }
        else if (this.volumeShown > this.volume.GetVolume())
        {
            this.volumeShown -= ChangeRate;
        }

        this.serialcom.AddCommand(new PercentageAppearanceCommand(Convert.ToInt32(this.volumeShown)));
        this.showing = true;
    }
}