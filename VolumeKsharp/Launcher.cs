// <copyright file="Launcher.cs" company="LeonardoTassinari">
// Copyright (c) LeonardoTassinari. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace VolumeKsharp;

/// <summary>
/// Launcher class that contains the entry point.
/// </summary>
public static class Launcher
{/// <summary>
 /// The entry point.
 /// </summary>
    public static void Main()
    {
        // ReSharper disable once ObjectCreationAsStatement
        new Controller();
    }
}