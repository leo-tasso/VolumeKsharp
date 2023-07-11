// <copyright file="PercentageAppearanceCommand.cs" company="LeonardoTassinari">
// Copyright (c) LeonardoTassinari. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace VolumeKsharp.AppearanceCommands;

using System;
using System.Globalization;

/// <inheritdoc />
public class PercentageAppearanceCommand : IAppearanceCommand
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PercentageAppearanceCommand"/> class.
    /// </summary>
    /// <param name="r">Red value.</param>
    /// <param name="g">Green value.</param>
    /// <param name="b">Blue value.</param>
    /// <param name="w">White value.</param>
    /// <param name="brightness">Brightness value.</param>
    /// <param name="percentage"> The percentage of ring to color.</param>
    public PercentageAppearanceCommand(int r, int g, int b, int w, int brightness, float percentage)
    {
        if (percentage < 0 || percentage > 100)
        {
            throw new ArgumentException("Percentage must be between 0 and 100.");
        }

        this.Message = "p" + r.ToString().PadLeft(3, '0') + "," + g.ToString().PadLeft(3, '0') + "," + b.ToString().PadLeft(3, '0') + "," + w.ToString().PadLeft(3, '0') + "," + brightness.ToString().PadLeft(3, '0') + "," + percentage.ToString(CultureInfo.InvariantCulture).PadLeft(5, '0');
    }

    /// <inheritdoc/>
    public string? Message { get; }
}