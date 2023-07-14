// <copyright file="BreathAppearanceCommand.cs" company="LeonardoTassinari">
// Copyright (c) LeonardoTassinari. All rights reserved.
// </copyright>

namespace VolumeKsharp.AppearanceCommands;

/// <summary>
/// Command to breath effect.
/// </summary>
public class BreathAppearanceCommand : IAppearanceCommand
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BreathAppearanceCommand"/> class.
    /// </summary>
    /// <param name="r">Red value.</param>
    /// <param name="g">Green value.</param>
    /// <param name="b">Blue value.</param>
    /// <param name="w">White value.</param>
    /// <param name="brightness">Brightness value.</param>
    /// <param name="speed">Speed value.</param>
    public BreathAppearanceCommand(int r, int g, int b, int w, int brightness, int speed)
    {
        this.Message = "b" + r.ToString().PadLeft(3, '0') + "," + g.ToString().PadLeft(3, '0') + "," + b.ToString().PadLeft(3, '0') + "," + w.ToString().PadLeft(3, '0') + "," + brightness.ToString().PadLeft(3, '0') + "," + speed.ToString().PadLeft(3, '0');
    }

    /// <inheritdoc />
    public string? Message { get; }
}