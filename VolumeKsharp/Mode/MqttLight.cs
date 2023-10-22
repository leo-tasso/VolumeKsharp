// <copyright file="MqttLight.cs" company="LeonardoTassinari">
// Copyright (c) LeonardoTassinari. All rights reserved.
// </copyright>

namespace VolumeKsharp.Mode;

using System.Threading.Tasks;
using VolumeKsharp.Light;

/// <summary>
/// The mode handling a light with mqtt.
/// </summary>
public class MqttLight : Mode
{
    private const float TransitionRate = 0.1f;

    private ILightRgbwEffect lightRgbwOld;
    private int transitionBrightness;
    private State activeState;
    private State targetState;

    /// <summary>
    /// Initializes a new instance of the <see cref="MqttLight"/> class.
    /// </summary>
    /// <param name="callingController">The calling controller.</param>
    public MqttLight(Controller callingController)
    {
        this.CallingController = callingController;
        this.transitionBrightness = 0;
        this.lightRgbwOld = ((ILightRgbwEffect?)callingController.LightRgbwEffect.Clone())!;
        this.activeState = State.Other;
        this.targetState = State.Other;
    }

    private enum State
    {
        LightState,
        Other,
    }

    /// <inheritdoc />
    protected override bool IsActive => this.activeState == State.LightState;

    /// <inheritdoc />
    protected override bool RequiresShow => this.CallingController.LightRgbwEffect.State;

    /// <inheritdoc />
    protected override Mode? PrevMode { get; set; }

    /// <inheritdoc />
    protected override Mode? NextMode { get; set; }

    /// <summary>
    /// Gets or sets the calling controller.
    /// </summary>
    private Controller CallingController { get; set; }

    /// <inheritdoc />
    public override void IncomingCommands(InputCommands command)
    {
        this.PrevMode?.IncomingCommands(command);
    }

    /// <inheritdoc />
    public override Task Compute()
    {
// Set state.
        if (this.HigherPriorityRequired() || !this.RequiresShow)
        {
            this.targetState = State.Other;
        }
        else
        {
            this.targetState = State.LightState;
        }

        // Update status.
        if (this.targetState == this.activeState)
        {
            if (this.transitionBrightness >= this.CallingController.LightRgbwEffect.Brightness)
            {
                // If not in a transition update light class only on change.
                if (!this.lightRgbwOld.Equals(this.CallingController.LightRgbwEffect))
                {
                    this.CallingController.LightRgbwEffect.UpdateLight();
                    this.lightRgbwOld = (this.CallingController.LightRgbwEffect.Clone() as ILightRgbwEffect)!;
                }
            }
            else
            {
                this.UpdateTransition();
            }
        }
        else
        {
            if (!this.AnyOtherIsShowing())
            {
                this.UpdateTransition();

                // If reached limit of fadeout, change state.
                if (this.transitionBrightness <= 0)
                {
                    this.activeState = this.targetState;
                }
            }
        }

        this.PrevMode?.Compute();
        return Task.CompletedTask;
    }

    private void UpdateLightState()
    {
        this.CallingController.LightRgbwEffect.UpdateLight(this.transitionBrightness);
    }

    private void UpdateTransition()
    {
        // Update transitionBrightness.
        if (this.targetState == this.activeState && this.activeState != State.Other)
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

        if (this.targetState == this.activeState && this.activeState != State.Other)
        {
            // After updating the new brightness update the state.
            this.UpdateLightState();
        }
    }
}