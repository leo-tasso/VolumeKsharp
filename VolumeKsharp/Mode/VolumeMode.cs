// <copyright file="VolumeMode.cs" company="LeonardoTassinari">
// Copyright (c) LeonardoTassinari. All rights reserved.
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
    private const float TransitionRate = 0.1f;
    private const int ShowTime = 700;
    private const double ChangeRate = 0.5;
    private const float StepSize = 2;
    private const double Tolerance = 1;
    private readonly Stopwatch sw = Stopwatch.StartNew();
    private readonly Stopwatch swMuted = Stopwatch.StartNew();
    private readonly Volume volume = new();
    private ILight lightRgbwOld;
    private double volumeShown;
    private bool muted;
    private bool mutedOld;
    private State activeState;
    private State targetState;
    private int transitionBrightness;

    /// <summary>
    /// Initializes a new instance of the <see cref="VolumeMode"/> class.
    /// </summary>
    /// <param name="callingController">The controller father.</param>
    public VolumeMode(Controller callingController)
    {
        this.CallingController = callingController;
        this.lightRgbwOld = (callingController.LightRgbwEffect.Clone() as ILight)!;
        this.activeState = State.VolumeState;
        this.targetState = State.VolumeState;
        this.transitionBrightness = callingController.LightRgbwEffect.Brightness;
    }

    private enum State
    {
        LightState,
        MuteState,
        VolumeState,
    }

    private Controller CallingController { get; set; }

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
            this.targetState = State.VolumeState;
        }

        return true;
    }

    /// <inheritdoc/>
    public Task Compute()
    {
// Section to set the target state.
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
                this.sw.Restart();
                this.targetState = State.VolumeState;
            }
        }

        // Show mute status change.
        if (this.mutedOld != this.muted)
        {
            this.targetState = this.muted ? State.MuteState : State.VolumeState;
        }

        // Turn off after having shown the volume.
        if (this.sw.ElapsedMilliseconds > ShowTime && this.targetState != State.LightState)
        {
            this.targetState = State.LightState;
        }

// Section to update the light.
        if (this.targetState == this.activeState)
        {
            if (this.transitionBrightness >= this.CallingController.LightRgbwEffect.Brightness)
            {
                // Update light on change.
                if (!this.lightRgbwOld.Equals(this.CallingController.LightRgbwEffect) && this.activeState == State.LightState)
                {
                    this.CallingController.LightRgbwEffect.UpdateLight();
                    this.transitionBrightness = this.CallingController.LightRgbwEffect.Brightness;
                    this.lightRgbwOld = (this.CallingController.LightRgbwEffect.Clone() as ILight)!;
                }
                else if (this.activeState == State.VolumeState)
                {
                    this.UpdateVolumeState();
                }
            }
            else
            {
                this.transitionBrightness += (int)(this.CallingController.LightRgbwEffect.Brightness * TransitionRate);
                if (this.transitionBrightness > this.CallingController.LightRgbwEffect.Brightness)
                {
                    this.transitionBrightness = this.CallingController.LightRgbwEffect.Brightness;
                }

                switch (this.activeState)
                {
                    case State.LightState:
                        this.UpdateLightState();
                        break;
                    case State.MuteState:
                        this.UpdateMuteState();
                        break;
                    case State.VolumeState:
                        this.UpdateVolumeState();
                        break;
                }
            }
        }
        else
        {
            if (this.activeState == State.LightState && !this.CallingController.LightRgbwEffect.State)
            {
                this.transitionBrightness = 0;
            }

            this.transitionBrightness -= (int)(this.CallingController.LightRgbwEffect.Brightness * TransitionRate);
            if (this.transitionBrightness < 0)
            {
                this.transitionBrightness = 0;
            }

            switch (this.activeState)
            {
                case State.LightState:
                    this.UpdateLightState();
                    break;
                case State.MuteState:
                    this.UpdateMuteState();
                    break;
                case State.VolumeState:
                    this.UpdateVolumeState();
                    break;
            }

            if (this.transitionBrightness <= 0)
            {
                this.activeState = this.targetState;
            }
        }

        return Task.CompletedTask;
    }

    private void UpdateMuteState()
    {
        int brightness;
        if (this.transitionBrightness == this.CallingController.LightRgbwEffect.Brightness)
        {
            brightness = this.CallingController.LightRgbwEffect.Brightness < 20
                ? 20
                : this.CallingController.LightRgbwEffect.Brightness;
        }
        else
        {
            brightness = this.transitionBrightness;
        }

        this.CallingController.Communicator.AddCommand(new SolidAppearanceCommand(255, 0, 0, 0, brightness));
    }

    private void UpdateLightState()
    {
        this.CallingController.LightRgbwEffect.UpdateLight(this.transitionBrightness);
    }

    private void UpdateVolumeState()
    {
        if (this.volumeShown < this.volume.GetVolume())
        {
            this.volumeShown += ChangeRate;
        }
        else if (this.volumeShown > this.volume.GetVolume())
        {
            this.volumeShown -= ChangeRate;
        }

        int brightness;
        if (this.transitionBrightness == this.CallingController.LightRgbwEffect.Brightness)
        {
            brightness = this.CallingController.LightRgbwEffect.Brightness < 20
                ? 20
                : this.CallingController.LightRgbwEffect.Brightness;
        }
        else
        {
            brightness = this.transitionBrightness;
        }

        this.CallingController.Communicator.AddCommand(new PercentageAppearanceCommand(0, this.CallingController.LightRgbwEffect.MaxValue, 0, 0, brightness, Convert.ToSingle(this.volumeShown)));
    }
}