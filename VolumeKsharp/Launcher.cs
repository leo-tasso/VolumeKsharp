// <copyright file="Launcher.cs" company="LeonardoTassinari">
// Copyright (c) LeonardoTassinari. All rights reserved.
// </copyright>

namespace VolumeKsharp;

using System;

/// <summary>
/// Launcher class that contains the entry point.
/// </summary>
public static class Launcher
{
    /// <summary>
    /// The entry point.
    /// </summary>
    [STAThread]
    public static void Main()
 {
     TrayIconMenu program = new TrayIconMenu();
     var controller = new Controller();
     program.ContextMenuThread(controller);
 }
}