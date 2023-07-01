// <copyright file="Launcher.cs" company="LeonardoTassinari">
// Copyright (c) LeonardoTassinari. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace VolumeKsharp;

using System;
using System.Runtime.InteropServices;

/// <summary>
/// Launcher class that contains the entry point.
/// </summary>
public static class Launcher
{
    private const int Swhide = 0;

    /// <summary>
    /// The entry point.
    /// </summary>
    [STAThread]
    public static void Main()
 {
     // Hide the console window
     IntPtr hWndConsole = GetConsoleWindow();
     if (hWndConsole != IntPtr.Zero)
     {
         ShowWindow(hWndConsole, Swhide);
     }

     TrayIconMenu program = new TrayIconMenu();
     var controller = new Controller();
     program.ContextMenuThread(controller);
 }

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
}