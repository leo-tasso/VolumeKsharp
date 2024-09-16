// <copyright file="ActivePrograms.cs" company="LeonardoTassinari">
// Copyright (c) LeonardoTassinari. All rights reserved.
// </copyright>

namespace VolumeKsharp;

using System.Diagnostics;
using System.Linq;
using System.Threading;

/// <summary>
/// Singleton class to get the list of open programs.
/// </summary>
public class ActivePrograms
{
    private static readonly Lock Lock = new Lock();
    private static ActivePrograms? _instance;
    private readonly Thread checker;
    private bool running = true;

    private ActivePrograms()
    {
        this.checker = new Thread(() =>
            {
                while (this.running)
                {
                    this.ActiveApps = Process.GetProcesses()
                        .Select(process => process.MainWindowTitle)
                        .Where(processTitle => !string.IsNullOrEmpty(processTitle))
                        .ToArray();
                    Thread.Sleep(500);
                }
            });
        this.checker.Start();
    }

    /// <summary>
    /// Gets the list of active programs with a title.
    /// </summary>
    public string[]? ActiveApps { get; private set; }

    /// <summary>
    /// Method to get the instance of the singleton.
    /// </summary>
    /// <returns>The instance of the singleton.</returns>
    public static ActivePrograms GetInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }

            lock (Lock)
            {
                _instance ??= new ActivePrograms();
            }

            return _instance;
        }

    /// <summary>
    /// Method to stop the thread to close the singleton. It will stop working.
    /// </summary>
    public void Stop()
    {
        this.running = false;
        this.checker.Join();
    }
}