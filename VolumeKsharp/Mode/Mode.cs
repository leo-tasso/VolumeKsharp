// <copyright file="Mode.cs" company="LeonardoTassinari">
// Copyright (c) LeonardoTassinari. All rights reserved.
// </copyright>

namespace VolumeKsharp.Mode;

using System.Threading.Tasks;

/// <summary>
/// Interface to model a Mode, a specific behaviour
/// that the knob should have relative to the program.
/// </summary>
public abstract class Mode
{
    /// <summary>
    /// Gets a value indicating whether if the mode is showing.
    /// </summary>
    protected abstract bool IsActive { get; }

    /// <summary>
    /// Gets a value indicating whether if the mode wants to show.
    /// </summary>
    protected abstract bool RequiresShow { get; }

    /// <summary>
    /// Gets or sets the previous mode.
    /// </summary>
    protected abstract Mode? PrevMode { get; set; }

    /// <summary>
    /// Gets or sets the next mode.
    /// </summary>
    protected abstract Mode? NextMode { get; set; }

    /// <summary>
    /// Method to send incoming commands to the relative mode.
    /// </summary>
    /// <param name="command"> The command to send.</param>
    public abstract void IncomingCommands(InputCommands command);

    /// <summary>
    /// Method to compute a normal cycle of the mode.
    /// </summary>
    /// <returns>>A <see cref="Task"/> representing the asynchronous operation of computing a cycle.</returns>
    public abstract Task Compute();

    /// <summary>
    /// Method to add a new mode with higher priority on top of the mode.
    /// </summary>
    /// <param name="newMode">The new mode.</param>
    public void StackMode(Mode newMode)
    {
        if (this.NextMode != null)
        {
            this.NextMode.StackMode(newMode);
        }
        else
        {
            this.NextMode = newMode;
            newMode.PrevMode = this;
        }
    }

    /// <summary>
    /// Method to check if any of the other following modes requires showing.
    /// </summary>
    /// <returns>If any of the other following modes requires showing.</returns>
    protected bool HigherPriorityRequired()
    {
        var cursor = this.NextMode;
        while (cursor != null)
        {
            if (cursor.RequiresShow)
            {
                return true;
            }

            cursor = cursor.NextMode;
        }

        return false;
    }

    /// <summary>
    /// Method to check if any of the other modes is showing.
    /// </summary>
    /// <returns>If any of the other  modes is showing.</returns>
    protected bool AnyOtherIsShowing()
    {
        var cursor = this;
        while (cursor.PrevMode != null)
        {
            cursor = cursor.PrevMode;
        }

        while (cursor != null)
        {
            if (cursor.IsActive && cursor != this)
            {
                return true;
            }

            cursor = cursor.NextMode;
        }

        return false;
    }
}