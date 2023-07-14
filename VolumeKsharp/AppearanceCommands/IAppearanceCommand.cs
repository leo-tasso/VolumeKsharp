// <copyright file="IAppearanceCommand.cs" company="LeonardoTassinari">
// Copyright (c) LeonardoTassinari. All rights reserved.
// </copyright>

namespace VolumeKsharp.AppearanceCommands;

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