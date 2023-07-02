// <copyright file="VolumeMode.cs" company="LeonardoTassinari">
// Copyright (c) LeonardoTassinari. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace VolumeKsharp;

using System;
using System.Diagnostics;

/// <summary/> mode to show volume status on the ring.
public class VolumeMode : IMode
{
    private const double ChangeRate = 1;
    private const float StepSize = 2;
    private const double Tolerance = 1;
    private readonly SerialCom serialcom;
    private readonly Stopwatch sw = Stopwatch.StartNew();
    private readonly Stopwatch swMuted = Stopwatch.StartNew();
    private readonly Volume volume = new();
    private double volumeShown;
    private bool on;
    private long swoMuted;
    private long swo;
    private bool muted;
    private bool mutedOld;

    /// <summary>
    /// Initializes a new instance of the <see cref="VolumeMode"/> class.
    /// </summary>
    /// <param name="serialcom"> the serial communicator that will be used.</param>
    public VolumeMode(SerialCom serialcom)
    {
        this.serialcom = serialcom;
    }

    /// <inheritdoc/>
    public bool IncomingCommands(InputCommands command)
    {
        this.swo = this.sw.ElapsedMilliseconds;
        this.mutedOld = this.muted;
        switch (command)
        {
            case InputCommands.Minus:
            {
                if (this.swMuted.ElapsedMilliseconds > this.swoMuted + 500)
                {
                    this.volume.SetVolume(this.volume.GetVolume() - StepSize);
                }

                break;
            }

            case InputCommands.Plus:
                if (this.swMuted.ElapsedMilliseconds > this.swoMuted + 500)
                {
                    this.volume.SetVolume(this.volume.GetVolume() + StepSize);
                }

                break;
            case InputCommands.Press:
                this.muted = !this.muted;
                this.volume.Muted = this.muted;
                this.swoMuted = this.swMuted.ElapsedMilliseconds;
                break;
            case InputCommands.Release:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (command != InputCommands.Release)
        {
            this.Show();
        }

        return true;
    }

    /// <inheritdoc/>
    public void Compute()
    {
        if (Math.Abs(this.volume.GetVolume() - this.volumeShown) > Tolerance)
        {
            this.Show();
        }

        if (this.mutedOld != this.muted)
        {
            if (this.muted)
            {
                this.serialcom.AddCommand(new SolidAppearanceCommand(255, 0, 0, 0));
            }
        }

        if (this.sw.ElapsedMilliseconds - this.swo > 500 && this.on)
        {
            this.serialcom.AddCommand(new PercentageAppearanceCommand(Convert.ToInt32(0)));
            this.on = false;
        }
    }

    private void Show()
    {
        this.swo = this.sw.ElapsedMilliseconds;
        if (this.volumeShown < this.volume.GetVolume())
        {
            this.volumeShown += ChangeRate;
        }
        else if (this.volumeShown > this.volume.GetVolume())
        {
            this.volumeShown -= ChangeRate;
        }

        this.serialcom.AddCommand(new PercentageAppearanceCommand(Convert.ToInt32(this.volumeShown)));
        this.on = true;
    }
}