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
    private const double BaseChangeRate = 0.5;
    private const double ChangeRate = 2;
    private const float StepSize = 2;
    private const double Tolerance = 1;

    /// <summary>
    /// Timer to track the elapsed time since the last volume change shown.
    /// </summary>
    private readonly Stopwatch sw = Stopwatch.StartNew();

    /// <summary>
    /// Timer to track the elapsed time since the last mute state change shown.
    /// </summary>
    private readonly Stopwatch swMuted = Stopwatch.StartNew();
    private readonly Volume volume = new();
    private ILight lightRgbwOld;
    private double volumeShown;
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

/// <summary>
/// All the states of the light:
/// Light>Showing selected light state
/// MuteState>Showing if just muted or unmuted
/// VolumeState>Showing if volume just changed.
/// </summary>
    private enum State
    {
        LightState,
        MuteState,
        VolumeState,
    }

/// <summary>
/// Gets or sets the calling controller.
/// </summary>
    private Controller CallingController { get; set; }

    /// <inheritdoc/>
    public bool IncomingCommands(InputCommands command)
    {
        this.sw.Restart();
        switch (command)
        {
            case InputCommands.Minus:
                this.volume.SetVolume(this.volume.GetVolume() - StepSize);
                break;

            case InputCommands.Plus:
                this.volume.SetVolume(this.volume.GetVolume() + StepSize);
                break;

            case InputCommands.Press:
                this.volume.Muted = !this.volume.Muted;
                break;

            case InputCommands.Release:
                break;

            default:
                throw new ArgumentOutOfRangeException();
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
        if (!this.swMuted.IsRunning)
        {
            if (Math.Abs(this.volume.GetVolume() - this.volumeShown) > Tolerance)
            {
                this.sw.Restart();
                this.targetState = State.VolumeState;
            }
        }

        // Show mute status change.
        if (this.mutedOld != this.volume.Muted)
        {
            this.targetState = this.volume.Muted ? State.MuteState : State.VolumeState;
            this.mutedOld = this.volume.Muted;
            this.sw.Restart();
            this.swMuted.Restart();
        }

        // Turn off after having shown the volume.
        if (this.sw.ElapsedMilliseconds > ShowTime && this.targetState != State.LightState)
        {
            this.targetState = State.LightState;
        }

// Section to update the light.
        // If The active state is the same as the target state, update it's properties.
        if (this.targetState == this.activeState)
        {
            // If the final brightness is reached.
            if (this.transitionBrightness >= this.CallingController.LightRgbwEffect.Brightness)
            {
                // Update light class only on change.
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

            // If the final brightness is not reached yet.
            else
            {
                this.UpdateTransition();
            }
        }// If the active state is different from the target state, transition to it.
        else
        {
            // If the active state is light and the light is off, no need to fade out, it's already off.
            if (this.activeState == State.LightState && !this.CallingController.LightRgbwEffect.State)
            {
                this.transitionBrightness = 0;
            }

            // Fade out active state.
            this.UpdateTransition();

            // If reached limit of fadeout, change state.
            if (this.transitionBrightness <= 0)
            {
                this.activeState = this.targetState;
            }
        }

        return Task.CompletedTask;
    }

    private void UpdateTransition()
    {
        // Update transitionBrightness.
        if (this.targetState == this.activeState)
        {
            this.transitionBrightness += (int)(this.CallingController.LightRgbwEffect.Brightness * TransitionRate);
        }
        else
        {
            this.transitionBrightness -= (int)(this.CallingController.LightRgbwEffect.Brightness * TransitionRate);
        }

        // Limit the max and min value to the target.
        if (this.transitionBrightness > this.CallingController.LightRgbwEffect.Brightness)
        {
            this.transitionBrightness = this.CallingController.LightRgbwEffect.Brightness;
        }

        if (this.transitionBrightness < 0)
        {
            this.transitionBrightness = 0;
        }

        // After updating the new brightness update the state.
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
            default:
                throw new ArgumentOutOfRangeException();
        }
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
        if (Math.Abs(this.volumeShown - this.volume.GetVolume()) > StepSize / 4)
        {
            double changeRate = BaseChangeRate;
            changeRate *= Math.Abs(this.volumeShown - this.volume.GetVolume()) / ChangeRate;
            if (this.volumeShown < this.volume.GetVolume())
            {
                this.volumeShown += changeRate;
            }
            else if (this.volumeShown > this.volume.GetVolume())
            {
                this.volumeShown -= changeRate;
            }
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