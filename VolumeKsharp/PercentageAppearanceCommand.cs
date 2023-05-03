// <copyright file="PercentageAppearanceCommand.cs" company="LeonardoTassinari">
// Copyright (c) LeonardoTassinari. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace VolumeKsharp;

using System;

/// <inheritdoc />
public class PercentageAppearanceCommand : IAppearanceCommand
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PercentageAppearanceCommand"/> class.
    /// </summary>
    /// <param name="percentage"> The percentage of ring to color.</param>
    public PercentageAppearanceCommand(int percentage)
    {
        if (percentage < 0 || percentage > 100)
        {
            throw new ArgumentException("Percentage must be between 0 and 100.");
        }

        this.Message = $"p{percentage}";
    }

    /// <inheritdoc/>
    public string? Message { get; }
}