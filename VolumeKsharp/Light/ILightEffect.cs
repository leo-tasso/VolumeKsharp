// <copyright file="ILightEffect.cs" company="LeonardoTassinari">
// Copyright (c) LeonardoTassinari. All rights reserved.
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

    /// <summary>
    /// Gets or sets the speed of the effect, might be not used by all effects.
    /// </summary>
    int EffectSpeed { get; set; }
}