// <copyright file="IMode.cs" company="LeonardoTassinari">
// Copyright (c) LeonardoTassinari. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace VolumeKsharp;

using System.Threading.Tasks;

/// <summary>
/// Interface to model a Mode, a specific behaviour
/// that the knob should have relative to the program.
/// </summary>
public interface IMode
{
    /// <summary>
    /// Method to send incoming commands to the relative mode.
    /// </summary>
    /// <param name="command"> The command to send.</param>
    /// <returns>If it got executed correctly.</returns>
    public bool IncomingCommands(InputCommands command);

    /// <summary>
    /// Method to compute a normal cycle of the mode.
    /// </summary>
    /// <returns>>A <see cref="Task"/> representing the asynchronous operation of computing a cycle.</returns>
    public Task Compute();
}