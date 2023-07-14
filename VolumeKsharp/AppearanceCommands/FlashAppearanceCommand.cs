// <copyright file="FlashAppearanceCommand.cs" company="LeonardoTassinari">
// Copyright (c) LeonardoTassinari. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace VolumeKsharp.AppearanceCommands;

/// <summary>
/// Appearance command to set the light in the chase effect mode.
/// </summary>
public class FlashAppearanceCommand : IAppearanceCommand
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FlashAppearanceCommand"/> class.
    /// </summary>
    /// <param name="r">Red value.</param>
    /// <param name="g">Green value.</param>
    /// <param name="b">Blue value.</param>
    /// <param name="w">White value.</param>
    /// <param name="brightness">Brightness value.</param>
    /// <param name="speed">Speed value.</param>
    public FlashAppearanceCommand(int r, int g, int b, int w, int brightness, int speed)
    {
        this.Message = "f" + r.ToString().PadLeft(3, '0') + "," + g.ToString().PadLeft(3, '0') + "," + b.ToString().PadLeft(3, '0') + "," + w.ToString().PadLeft(3, '0') + "," + brightness.ToString().PadLeft(3, '0') + "," + speed.ToString().PadLeft(3, '0');
    }

    /// <inheritdoc />
    public string? Message { get; }
}