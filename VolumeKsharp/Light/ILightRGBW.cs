// <copyright file="ILightRGBW.cs" company="LeonardoTassinari">
// Copyright (c) LeonardoTassinari. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace VolumeKsharp.Light;

using System;
using System.Collections.Generic;

/// <summary>
/// Interface For Rgbw light.
/// </summary>
public interface ILightRgbw : IEquatable<LightRgbw>
{
    /// <summary>
    /// Gets the list of effects.
    /// </summary>
    ISet<string> EffectsSet { get; }

    /// <summary>
    /// Gets or sets the active effect.
    /// </summary>
    string? ActiveEffect { get; set; }

    /// <summary>
    /// Gets or sets red value.
    /// </summary>
    int R { get; set; }

    /// <summary>
    /// Gets or sets the green value.
    /// </summary>
    int G { get; set; }

    /// <summary>
    /// Gets or sets the blue value.
    /// </summary>
    int B { get; set; }

    /// <summary>
    /// Gets or sets the white value.
    /// </summary>
    int W { get; set; }

    /// <summary>
    /// Gets or sets the brightness value.
    /// </summary>
    int Brightness { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the on off state.
    /// </summary>
    bool State { get; set; }

    /// <summary>
    /// Method to update the physical light with the current parameters.
    /// </summary>
    void UpdateLight();

    /// <inheritdoc cref="Equals(LightRgbw?)" />
    new bool Equals(LightRgbw? other);

    /// <inheritdoc cref="Equals(LightRgbw?)" />
    bool Equals(object? obj);

    /// <inheritdoc cref="GetHashCode" />
    int GetHashCode();
}