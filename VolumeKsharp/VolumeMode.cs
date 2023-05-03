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
    private const float StepSize = 2;
    private const double Tolerance = 1;
    private readonly Stopwatch sw = Stopwatch.StartNew();
    private readonly Volume volume = new Volume();
    private readonly SerialCom serialcom;
    private bool on;
    private long swo;
    private float oldVolume;

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
        this.oldVolume = Convert.ToInt32(this.volume.GetVolume());
        switch (command)
        {
            case InputCommands.Minus:
            {
                this.volume.SetVolume(this.volume.GetVolume() - StepSize);
                break;
            }

            case InputCommands.Plus:
                this.volume.SetVolume(this.volume.GetVolume() + StepSize);
                break;
            case InputCommands.Press:
                return false;
            case InputCommands.Release:
                return false;
            default:
                throw new ArgumentOutOfRangeException();
        }

        this.Show();

        return true;
    }

    /// <inheritdoc/>
    public void Compute()
    {
        if (Math.Abs(this.oldVolume - Convert.ToInt32(this.volume.GetVolume())) > Tolerance)
        {
            this.Show();
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
        this.serialcom.AddCommand(new PercentageAppearanceCommand(Convert.ToInt32(this.volume.GetVolume())));
        this.oldVolume = Convert.ToInt32(this.volume.GetVolume());
        this.on = true;
    }
}