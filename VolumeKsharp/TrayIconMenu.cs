// <copyright file="TrayIconMenu.cs" company="LeonardoTassinari">
// Copyright (c) LeonardoTassinari. All rights reserved.
// </copyright>

// ReSharper disable CoVariantArrayConversion
namespace VolumeKsharp
{
    using System;
    using System.Drawing;
    using System.Threading;
    using System.Windows.Forms;

    /// <summary>
    /// Class of the windows tray icon.
    /// </summary>
    public class TrayIconMenu
    {
        private NotifyIcon? notifyIcon;
        private ContextMenuStrip contextMenu = null!;
        private ToolStripComboBox comPortComboBox = null!;
        private ToolStripMenuItem connectMenuItem = null!;
        private ToolStripMenuItem lightToggleMenuItem = null!;
        private ToolStripMenuItem exitMenuItem = null!;
        private Label brightnessLabel = null!;
        private Label speedLabel = null!;
        private TrackBar brightnessSlider = null!;
        private TrackBar speedSlider = null!;

        private Controller? Controller { get; set; }

        /// <summary>
        /// Thread of the tray icon.
        /// </summary>
        /// <param name="masterController">The calling controller.</param>
        public void ContextMenuThread(Controller masterController)
        {
            Thread contextMenuThread = new Thread(() =>
            {
                this.Controller = masterController;
                this.notifyIcon = new NotifyIcon();
                this.contextMenu = new ContextMenuStrip();
                this.notifyIcon.ContextMenuStrip = this.contextMenu;
                this.notifyIcon.Icon = new Icon("res/VolumeKLogo.ico");
                this.notifyIcon.Text = "Volumek";
                this.notifyIcon.Visible = true;

                this.comPortComboBox = new ToolStripComboBox("Select COM Port");
                this.contextMenu.Items.Add(this.comPortComboBox);

                string[] availablePorts = this.Controller.Communicator.GetPorts();
                this.comPortComboBox.Items.AddRange(availablePorts);
                this.comPortComboBox.SelectedItem = this.Controller.Communicator.Port.ToUpper();

                this.connectMenuItem = new ToolStripMenuItem(this.Controller!.Communicator.Running ? "Disconnect" : "Connect");
                this.lightToggleMenuItem = new ToolStripMenuItem("Light");
                this.lightToggleMenuItem.Checked = this.Controller.LightRgbwEffect.State;
                this.exitMenuItem = new ToolStripMenuItem("Exit");

                this.contextMenu.Items.Add(this.connectMenuItem);
                this.contextMenu.Items.Add(this.lightToggleMenuItem);

                this.brightnessLabel = new Label();
                this.brightnessLabel.Text = "Brightness:";
                this.brightnessLabel.BackColor = Color.White;
                this.brightnessLabel.ForeColor = Color.Black;
                this.contextMenu.Items.Add(new ToolStripControlHost(this.brightnessLabel));

                this.brightnessSlider = new TrackBar();
                this.brightnessSlider.Minimum = 0;
                this.brightnessSlider.Maximum = 255;
                this.brightnessSlider.Value = this.Controller.LightRgbwEffect.Brightness;
                this.brightnessSlider.TickStyle = TickStyle.None;
                this.brightnessSlider.BackColor = Color.White;
                this.brightnessSlider.Scroll += this.BrightnessSlider_Scroll!;
                this.contextMenu.Items.Add(new ToolStripControlHost(this.brightnessSlider));

                this.speedLabel = new Label();
                this.speedLabel.Text = "Speed:";
                this.speedLabel.BackColor = Color.White;
                this.speedLabel.ForeColor = Color.Black;
                this.contextMenu.Items.Add(new ToolStripControlHost(this.speedLabel));

                this.speedSlider = new TrackBar();
                this.speedSlider.Minimum = 0;
                this.speedSlider.Maximum = 255;
                this.speedSlider.Value = this.Controller.LightRgbwEffect.EffectSpeed;
                this.speedSlider.TickStyle = TickStyle.None;
                this.speedSlider.BackColor = Color.White;
                this.speedSlider.Scroll += this.SpeedSlider_Scroll!;
                this.contextMenu.Items.Add(new ToolStripControlHost(this.speedSlider));

                this.contextMenu.Items.Add(this.exitMenuItem);

                this.comPortComboBox.SelectedIndexChanged += this.ComPortComboBox_SelectedIndexChanged!;
                this.connectMenuItem.Click += this.ConnectMenuItem_Click!;
                this.exitMenuItem.Click += this.ExitMenuItem_Click!;
                this.lightToggleMenuItem.Click += this.LightToggleMenuItem_Click!;
                this.notifyIcon.Click += this.TrayIconOpened!;
                Microsoft.Win32.SystemEvents.PowerModeChanged += this.OnPowerModeChanged;

                Application.Run();
            })
            {
                IsBackground = true,
            };

            contextMenuThread.Start();
        }

        /// <summary>
        /// Method to close the tray icon.
        /// </summary>
        public void Stop()
        {
            this.notifyIcon!.Visible = false;
            Application.Exit();
        }

        private void ComPortComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedPort = this.comPortComboBox.SelectedItem.ToString() ?? string.Empty;
            var controller = this.Controller;
            if (controller is not null)
            {
                controller.Communicator.Port = selectedPort;
            }
        }

        private void ConnectMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.Controller!.Communicator.Running)
            {
                this.Controller.Communicator.Start();
                this.connectMenuItem.Text = "Disconnect";
            }
            else
            {
                this.Controller.Communicator.Stop();
                this.connectMenuItem.Text = "Connect";
            }
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            this.Controller?.Stop();
        }

        private void OnPowerModeChanged(object sender, Microsoft.Win32.PowerModeChangedEventArgs e)
        {
            if (e.Mode == Microsoft.Win32.PowerModes.Resume)
            {
                this.Controller!.Communicator.Stop();
                this.Controller.Communicator.Start();
            }
        }

        private void LightToggleMenuItem_Click(object sender, EventArgs e)
        {
            this.lightToggleMenuItem.Checked = !this.lightToggleMenuItem.Checked;
            this.Controller!.LightRgbwEffect.State = this.lightToggleMenuItem.Checked;
        }

        private void BrightnessSlider_Scroll(object sender, EventArgs e)
        {
            TrackBar slider = (TrackBar)sender;
            this.Controller!.LightRgbwEffect.Brightness = slider.Value;
        }

        private void SpeedSlider_Scroll(object sender, EventArgs e)
        {
            TrackBar slider = (TrackBar)sender;
            this.Controller!.LightRgbwEffect.EffectSpeed = slider.Value;
        }

        private void TrayIconOpened(object sender, EventArgs e)
        {
            this.lightToggleMenuItem.Checked = this.Controller!.LightRgbwEffect.State;
            string[] availablePorts = this.Controller.Communicator.GetPorts();
            this.comPortComboBox.Items.Clear();
            this.comPortComboBox.Items.AddRange(availablePorts);
            this.speedSlider.Value = this.Controller.LightRgbwEffect.EffectSpeed;
            this.brightnessSlider.Value = this.Controller.LightRgbwEffect.Brightness;
        }
    }
}
