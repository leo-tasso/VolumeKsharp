// <copyright file="InputCommands.cs" company="LeonardoTassinari">
// Copyright (c) LeonardoTassinari. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace VolumeKsharp;

/// <summary>
/// Possible commands that can come from the knob.
/// </summary>
public enum InputCommands
{
    /// <summary>
    /// If it gets rotated clockwise.
    /// </summary>
    Plus,

    /// <summary>
    /// If it gets rotated counterclockwise.
    /// </summary>
    Minus,

    /// <summary>
    /// If the knob got pressed.
    /// </summary>
    Press,

    /// <summary>
    /// If the knob got released.
    /// </summary>
    Release,
}