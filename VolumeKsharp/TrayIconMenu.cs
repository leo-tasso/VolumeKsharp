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
        private ContextMenuStrip contextMenu = null!;
        private ToolStripComboBox comPortComboBox = null!;
        private ToolStripMenuItem connectMenuItem = null!;
        private ToolStripMenuItem lightToggleMenuItem = null!;
        private ToolStripMenuItem exitMenuItem = null!;

        private Controller? Controller { get; set; }

        /// <summary>
        /// Method to start the thread.
        /// </summary>
        /// <param name="masterController">The Main controller.</param>
        public void ContextMenuThread(Controller masterController)
        {
            // Create a new thread to host the context menu strip and message loop
            Thread contextMenuThread = new Thread(() =>
            {
                this.Controller = masterController;
                this.notifyIcon = new NotifyIcon();
                this.contextMenu = new ContextMenuStrip();
                this.notifyIcon.ContextMenuStrip = this.contextMenu;
                this.notifyIcon.Icon = new System.Drawing.Icon("res/VolumeKLogo.ico");
                this.notifyIcon.Text = "Volumek";
                this.notifyIcon.Visible = true;

                // Create the COM port selection dropdown
                this.comPortComboBox = new ToolStripComboBox("Select COM Port");
                this.contextMenu.Items.Add(this.comPortComboBox);

                // Populate the COM port dropdown with available ports
                string[] availablePorts = this.Controller.Serialcom.GetPorts();

                // ReSharper disable once CoVariantArrayConversion
                this.comPortComboBox.Items.AddRange(availablePorts);

                this.comPortComboBox.SelectedItem = this.Controller.Serialcom.Port.ToUpper();

                // Create the menu items
                this.connectMenuItem = new ToolStripMenuItem(this.Controller!.Serialcom.Running ? "Disconnect" : "Connect");
                this.lightToggleMenuItem = new ToolStripMenuItem("Light");
                this.lightToggleMenuItem.Checked = this.Controller.Light.State;
                this.exitMenuItem = new ToolStripMenuItem("Exit");

                // Add the menu items to the context menu
                this.contextMenu.Items.Add(this.connectMenuItem);
                this.contextMenu.Items.Add(this.lightToggleMenuItem);
                this.contextMenu.Items.Add(this.exitMenuItem);

                // Hook up event handlers
                this.comPortComboBox.SelectedIndexChanged += this.ComPortComboBox_SelectedIndexChanged!;
                this.connectMenuItem.Click += this.ConnectMenuItem_Click!;
                this.exitMenuItem.Click += this.ExitMenuItem_Click!;
                this.lightToggleMenuItem.Click += this.LightToggleMenuItem_Click!;
                Microsoft.Win32.SystemEvents.PowerModeChanged += this.OnPowerModeChanged;

                // Start the message loop
                Application.Run();
            })
            {
                // Set the thread to run in the background and start it
                IsBackground = true,
            };

            contextMenuThread.Start();
        }

        private void ComPortComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected COM port from the dropdown
            string selectedPort = this.comPortComboBox.SelectedItem.ToString() ?? string.Empty;
            var controller = this.Controller;
            if (controller is not null)
            {
                controller.Serialcom.Port = selectedPort;
            }
        }

        private void ConnectMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.Controller!.Serialcom.Running)
            {
                this.Controller.Serialcom.Start();
                this.connectMenuItem.Text = "Disconnect";
            }
            else
            {
                this.Controller.Serialcom.Stop();
                this.connectMenuItem.Text = "Connect";
            }
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            this.Controller!.Serialcom.Stop();
            this.Controller.Continue = false;
            this.notifyIcon!.Visible = false;
            Application.Exit();
        }

        private void OnPowerModeChanged(object sender, Microsoft.Win32.PowerModeChangedEventArgs e)
        {
            if (e.Mode == Microsoft.Win32.PowerModes.Resume)
            {
                // Computer has resumed from sleep, restart serial communication
                this.Controller!.Serialcom.Stop();
                this.Controller.Serialcom.Start();
            }
        }

        private void LightToggleMenuItem_Click(object sender, EventArgs e)
        {
            this.lightToggleMenuItem.Checked = !this.lightToggleMenuItem.Checked;
            this.Controller!.Light.State = this.lightToggleMenuItem.Checked;
            this.Controller.RgbwLightMqttClient.UpdateState(this.Controller.Light);
        }
    }
}