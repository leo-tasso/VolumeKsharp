// <copyright file="RainbowAppearanceCommand.cs" company="LeonardoTassinari">
// Copyright (c) LeonardoTassinari. All rights reserved.
// </copyright>

namespace VolumeKsharp.AppearanceCommands;

/// <summary>
/// Appearance command for the rainbow effect.
/// </summary>
public class RainbowAppearanceCommand : IAppearanceCommand
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RainbowAppearanceCommand"/> class.
    /// </summary>
    /// <param name="brightness">Brightness value.</param>
    /// <param name="speed">Speed value.</param>
    public RainbowAppearanceCommand(int brightness, int speed)
    {
        this.Message = "r" + brightness.ToString().PadLeft(3, '0') + "," + speed.ToString().PadLeft(3, '0');
    }

    /// <inheritdoc />
    public string? Message { get; }
}