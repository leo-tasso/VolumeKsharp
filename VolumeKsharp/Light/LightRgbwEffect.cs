// <copyright file="LightRgbwEffect.cs" company="LeonardoTassinari">
// Copyright (c) LeonardoTassinari. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace VolumeKsharp.Light;

using System;
using System.Collections.Generic;
using VolumeKsharp.AppearanceCommands;

// ReSharper disable NonReadonlyMemberInGetHashCode

/// <summary>
/// Class to represent a rgbw light.
/// </summary>
public class LightRgbwEffect : ILightRgbwEffect, IEquatable<LightRgbwEffect>
{
    /// <summary>
    /// The parent controller.
    /// </summary>
    private readonly Controller controller;

    /// <summary>
    /// Initializes a new instance of the <see cref="LightRgbwEffect"/> class.
    /// </summary>
    /// <param name="controller">The calling controller.</param>
    public LightRgbwEffect(Controller controller)
        : this(0, 0, 0, 0, controller)
    {
        this.W = this.MaxValue;
        this.Brightness = this.MaxValue;
        this.EffectSpeed = this.MaxValue / 2;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LightRgbwEffect"/> class.
    /// </summary>
    /// <param name="parentLight">The parent light.</param>
    /// <param name="parentController">The parent controller.</param>
    private LightRgbwEffect(ILightRgbwEffect parentLight, Controller parentController)
    {
        this.MaxValue = parentLight.MaxValue;
        this.controller = parentController;
        this.ActiveEffect = parentLight.ActiveEffect;
        this.R = parentLight.R;
        this.G = parentLight.G;
        this.B = parentLight.B;
        this.W = parentLight.W;
        this.Brightness = parentLight.Brightness;
        this.State = parentLight.State;
        this.EffectSpeed = parentLight.EffectSpeed;
    }

    private LightRgbwEffect(int r, int g, int b, int w, Controller controller)
    {
        this.R = r;
        this.G = g;
        this.B = b;
        this.W = w;
        this.controller = controller;
        this.State = false;
        this.Brightness = this.MaxValue;
        this.EffectSpeed = this.MaxValue / 2;
    }

    /// <summary>
    /// Gets the list of effects.
    /// </summary>
    public ISet<string> EffectsSet { get; } = new HashSet<string>(
        new[] { "Solid", "Rainbow", "Breath", "colorfade_slow", "colorfade_fast", "flash" });

    /// <summary>
    /// Gets the max value of the colors.
    /// </summary>
    public int MaxValue { get; } = 255;

    /// <summary>
    /// Gets or sets the active effect.
    /// </summary>
    public string? ActiveEffect { get; set; }

    /// <inheritdoc />
    public int EffectSpeed { get; set; }

    /// <summary>
    /// Gets or sets red value.
    /// </summary>
    public int R { get; set; }

    /// <summary>
    /// Gets or sets the green value.
    /// </summary>
    public int G { get; set; }

    /// <summary>
    /// Gets or sets the blue value.
    /// </summary>
    public int B { get; set; }

    /// <summary>
    /// Gets or sets the white value.
    /// </summary>
    public int W { get; set; }

    /// <summary>
    /// Gets or sets the brightness value.
    /// </summary>
    public int Brightness { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the on off state.
    /// </summary>
    public bool State { get; set; }

    /// <summary>
    /// Equals operator override.
    /// </summary>
    /// <param name="left">The left light.</param>
    /// <param name="right">The right light.</param>
    /// <returns>If they are equal.</returns>
    public static bool operator ==(LightRgbwEffect? left, LightRgbwEffect? right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// Different operator override.
    /// </summary>
    /// <param name="left">The left light.</param>
    /// <param name="right">The right light.</param>
    /// <returns>If they are different.</returns>
    public static bool operator !=(LightRgbwEffect? left, LightRgbwEffect? right)
    {
        return !Equals(left, right);
    }

    /// <summary>
    /// Method to update the physical light with the current parameters.
    /// </summary>
    public void UpdateLight()
    {
        if (this.State)
        {
            switch (this.ActiveEffect)
            {
                case "Breath":
                    this.controller.Communicator.AddCommand(new BreathAppearanceCommand(this.R, this.G, this.B, this.W, this.Brightness, this.EffectSpeed));
                    break;
                default:
                    this.SolidUpdate();
                    break;
            }
        }
        else
        {
            this.controller.Communicator.AddCommand(new SolidAppearanceCommand(0, 0, 0, 0, 0));
        }
    }

    /// <inheritdoc cref="object" />
    public bool Equals(LightRgbwEffect? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.controller.Equals(other.controller) && this.EffectsSet.SetEquals(other.EffectsSet) && this.MaxValue == other.MaxValue && this.ActiveEffect == other.ActiveEffect && this.EffectSpeed == other.EffectSpeed && this.R == other.R && this.G == other.G && this.B == other.B && this.W == other.W && this.Brightness == other.Brightness && this.State == other.State;
    }

    /// <inheritdoc cref="object" />
    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || (obj is LightRgbwEffect other && this.Equals(other));
    }

    /// <inheritdoc cref="object" />
    public override int GetHashCode()
    {
        var hashCode = default(HashCode);
        hashCode.Add(this.controller);
        hashCode.Add(this.EffectsSet);
        hashCode.Add(this.MaxValue);
        hashCode.Add(this.ActiveEffect);
        hashCode.Add(this.EffectSpeed);
        hashCode.Add(this.R);
        hashCode.Add(this.G);
        hashCode.Add(this.B);
        hashCode.Add(this.W);
        hashCode.Add(this.Brightness);
        hashCode.Add(this.State);
        return hashCode.ToHashCode();
    }

    /// <inheritdoc />
    public object Clone()
    {
        return new LightRgbwEffect(this, this.controller);
    }

    private void SolidUpdate()
    {
        this.controller.Communicator.AddCommand(new SolidAppearanceCommand(
            this.R,
            this.G,
            this.B,
            this.W,
            this.Brightness));
    }
}