// <copyright file="RGBWLight.cs" company="LeonardoTassinari">
// Copyright (c) LeonardoTassinari. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace VolumeKsharp;

// ReSharper disable NonReadonlyMemberInGetHashCode
using System;

/// <summary>
/// Class to represent a rgbw light.
/// </summary>
public class RgbwLight : IEquatable<RgbwLight>
{
    /// <summary>
    /// The max value of the colors.
    /// </summary>
    // ReSharper disable once ConvertToConstant.Global
#pragma warning disable SA1401
    public readonly int MaxValue = 255;
#pragma warning restore SA1401

    /// <summary>
    /// Initializes a new instance of the <see cref="RgbwLight"/> class.
    /// </summary>
    public RgbwLight()
        : this(0, 0, 0, 0)
    {
        this.W = this.MaxValue;
        this.Brightness = this.MaxValue;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RgbwLight"/> class.
    /// </summary>
    /// <param name="parentLight">The light to copy.</param>
    public RgbwLight(RgbwLight parentLight)
        : this(parentLight.R, parentLight.G, parentLight.B, parentLight.W)
    {
        this.State = parentLight.State;
        this.Brightness = parentLight.Brightness;
    }

    private RgbwLight(int r, int g, int b, int w)
    {
        this.R = r;
        this.G = g;
        this.B = b;
        this.W = w;
        this.State = false;
        this.Brightness = this.MaxValue;
    }

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
    public static bool operator ==(RgbwLight? left, RgbwLight? right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// Different operator override.
    /// </summary>
    /// <param name="left">The left light.</param>
    /// <param name="right">The right light.</param>
    /// <returns>If they are different.</returns>
    public static bool operator !=(RgbwLight? left, RgbwLight? right)
    {
        return !Equals(left, right);
    }

    /// <inheritdoc/>
    public bool Equals(RgbwLight? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.R == other.R && this.G == other.G && this.B == other.B && this.W == other.W && this.Brightness == other.Brightness && this.State == other.State;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || (obj is RgbwLight other && this.Equals(other));
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(this.R, this.G, this.B, this.W, this.Brightness, this.State);
    }
}