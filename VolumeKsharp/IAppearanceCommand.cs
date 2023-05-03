// <copyright file="IAppearanceCommand.cs" company="LeonardoTassinari">
// Copyright (c) LeonardoTassinari. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace VolumeKsharp;

/// <summary>
/// Command that tells the knob what aspect should have.
/// </summary>
public interface IAppearanceCommand
{
    /// <summary>
    /// Gets raw String message to send to the knob.
    /// </summary>
    string? Message { get; }
}