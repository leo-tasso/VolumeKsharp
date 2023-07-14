// <copyright file="ILight.cs" company="LeonardoTassinari">
// Copyright (c) LeonardoTassinari. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace VolumeKsharp.Light;

using System;

/// <summary>
/// Interface for a simple light with brightness.
/// </summary>
public interface ILight : ICloneable
{
    /// <summary>
    /// Gets the maximum value of the parameters.
    /// </summary>
    int MaxValue { get; }

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

    /// <summary>
    /// Method to update the physical light.
    /// </summary>
    /// <param name="brightness">The brightness value.</param>
    void UpdateLight(int brightness);

    /// <inheritdoc cref="object" />
    bool Equals(LightRgbwEffect? other);

    /// <inheritdoc cref="object" />
    bool Equals(object? obj);

    /// <inheritdoc cref="object" />
    int GetHashCode();
}