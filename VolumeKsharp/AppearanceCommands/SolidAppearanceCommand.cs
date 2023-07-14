// <copyright file="SolidAppearanceCommand.cs" company="LeonardoTassinari">
// Copyright (c) LeonardoTassinari. All rights reserved.
// </copyright>

namespace VolumeKsharp.AppearanceCommands;

/// <inheritdoc />
public class SolidAppearanceCommand : IAppearanceCommand
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SolidAppearanceCommand"/> class.
    /// </summary>
    /// <param name="r">The Red value.</param>
    /// <param name="g">The Green value.</param>
    /// <param name="b">The Blue value.</param>
    /// <param name="w">The White value.</param>
    /// <param name="brightness">The Brightness value.</param>
    public SolidAppearanceCommand(int r, int g, int b, int w, int brightness)
    {
        this.Message = "s" + r.ToString().PadLeft(3, '0') + "," + g.ToString().PadLeft(3, '0') + "," + b.ToString().PadLeft(3, '0') + "," + w.ToString().PadLeft(3, '0') + "," + brightness.ToString().PadLeft(3, '0');
    }

    /// <inheritdoc />
    public string? Message { get; }
}