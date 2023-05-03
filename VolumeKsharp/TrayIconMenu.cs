// <copyright file="TrayIconMenu.cs" company="LeonardoTassinari">
// Copyright (c) LeonardoTassinari. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace VolumeKsharp
{
    using System;
    using System.Threading;
    using System.Windows.Forms;

    /// <summary>
    /// The class to create and manage the tray icon.
    /// </summary>
    public class TrayIconMenu
    {
        private NotifyIcon? notifyIcon;

        /// <summary>
        /// Method to start the thread.
        /// </summary>
        public void ContextMenuThread()
        {
            // Create a new thread to host the context menu strip and message loop
            Thread contextMenuThread = new Thread(() =>
            {
                // Create a new NotifyIcon instance
                this.notifyIcon = new NotifyIcon();

                // Create a new ContextMenuStrip instance
                ContextMenuStrip contextMenuStrip = new ContextMenuStrip();

                // Add items to the context menu strip
                ToolStripMenuItem item1 = new ToolStripMenuItem("Item 1");
                ToolStripMenuItem item2 = new ToolStripMenuItem("Item 2");
                contextMenuStrip.Items.Add(item1);
                contextMenuStrip.Items.Add(item2);

                // Hook up event handlers for the menu items
                item1.Click += this.OnItemClick!;
                item2.Click += this.OnItemClick!;

                // Set the NotifyIcon's context menu strip
                this.notifyIcon.ContextMenuStrip = contextMenuStrip;

                // Set the NotifyIcon's icon and text
                this.notifyIcon.Icon = new System.Drawing.Icon("LayerIcon.ico");
                this.notifyIcon.Text = "TrayIcon Example";

                // Display the NotifyIcon
                this.notifyIcon.Visible = true;

                // Start the message loop
                Application.Run();
            })
            {
                // Set the thread to run in the background and start it
                IsBackground = true,
            };

            contextMenuThread.Start();
        }

        private void OnItemClick(object sender, EventArgs e)
        {
            // Handle menu item click events here
            ToolStripMenuItem clickedItem = (ToolStripMenuItem)sender;
            string itemName = clickedItem.Text;
            MessageBox.Show($"You clicked {itemName}!");
        }
    }
}