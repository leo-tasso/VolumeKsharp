// <copyright file="VolumeMode.cs" company="LeonardoTassinari">
// Copyright (c) LeonardoTassinari. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace VolumeKsharp.Mode;

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using VolumeKsharp.AppearanceCommands;
using VolumeKsharp.Light;

/// <summary/> mode to show volume status on the ring.
public class VolumeMode : IMode
{
    private const int ShowTime = 500;
    private const double ChangeRate = 1;
    private const float StepSize = 2;
    private const double Tolerance = 1;
    private readonly Stopwatch sw = Stopwatch.StartNew();
    private readonly Stopwatch swMuted = Stopwatch.StartNew();
    private readonly Volume volume = new();
    private ILight lightRgbwOld;
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
        this.lightRgbwOld = (calligController.LightRgbwEffect.Clone() as ILight)!;
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
        if (!this.lightRgbwOld.Equals(this.CalligController.LightRgbwEffect))
        {
            this.CalligController.RgbwLightMqttClient.UpdateState(this.CalligController.LightRgbwEffect);
            if (!this.showing)
            {
                this.CalligController.LightRgbwEffect.UpdateLight();
            }

            this.lightRgbwOld = (this.CalligController.LightRgbwEffect.Clone() as ILight)!;
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
                this.CalligController.Communicator.AddCommand(new SolidAppearanceCommand(255, 0, 0, 0, this.CalligController.LightRgbwEffect.Brightness < 20 ? 20 : this.CalligController.LightRgbwEffect.Brightness));
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
            this.CalligController.LightRgbwEffect.UpdateLight();
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

        this.CalligController.Communicator.AddCommand(new PercentageAppearanceCommand(0, this.CalligController.LightRgbwEffect.MaxValue, 0, 0, this.CalligController.LightRgbwEffect.Brightness < 20 ? 20 : this.CalligController.LightRgbwEffect.Brightness, Convert.ToSingle(this.volumeShown)));
        this.showing = true;
    }
}