// <copyright file="ILightRgbw.cs" company="LeonardoTassinari">
// Copyright (c) LeonardoTassinari. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace VolumeKsharp.Light;

/// <summary>
/// Interface for a rgb light with a white channel.
/// </summary>
public interface ILightRgbw : ILightRgb
{
    /// <summary>
    /// Gets or sets the white value.
    /// </summary>
    int W { get; set; }
}