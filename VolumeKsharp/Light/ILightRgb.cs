// <copyright file="ILightRgb.cs" company="LeonardoTassinari">
// Copyright (c) LeonardoTassinari. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace VolumeKsharp.Light;

/// <summary>
/// Interface for a rgb light.
/// </summary>
public interface ILightRgb : ILight
{
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
}