// <copyright file="ILightEffect.cs" company="LeonardoTassinari">
// Copyright (c) LeonardoTassinari. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace VolumeKsharp.Light;

using System.Collections.Generic;

/// <summary>
/// Interface for a light that supports effects.
/// </summary>
public interface ILightEffect : ILight
{
    /// <summary>
    /// Gets the list of effects.
    /// </summary>
    ISet<string> EffectsSet { get; }

    /// <summary>
    /// Gets or sets the active effect.
    /// </summary>
    string? ActiveEffect { get; set; }
}