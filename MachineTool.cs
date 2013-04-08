﻿/*
 * Copyright Copyright 2012, System Insights, Inc.
 *
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *       http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AdapterLab
{
    using MTConnect;
    using NAudio;
    using NAudio.Wave;

    public partial class MachineTool : Form
    {
        Adapter mAdapter = new Adapter();
        Event mAvail = new Event("avail");
        Event mEStop = new Event("estop");
        
        public MachineTool()
        {
            InitializeComponent();
            stop.Enabled = false;

            mAdapter.AddDataItem(mAvail);
            mAvail.Value = "AVAILABLE";

            mAdapter.AddDataItem(mEStop);
        }

        private void start_Click(object sender, EventArgs e)
        {
            // Start the adapter lib with the port number in the text box
            mAdapter.Port = Convert.ToInt32(port.Text);
            mAdapter.Start();

            // Disable start and enable stop.
            start.Enabled = false;
            stop.Enabled = true;

            // Start our periodic timer
            gather.Interval = 1000;
            gather.Enabled = true;
        }

        private void stop_Click(object sender, EventArgs e)
        {
            // Stop everything...
            mAdapter.Stop();
            stop.Enabled = false;
            start.Enabled = true;
            gather.Enabled = false;
        }

        private void gather_Tick(object sender, EventArgs e)
        {
            mAdapter.Begin();

            if (estop.Checked)
                mEStop.Value = "TRIGGERED";
            else
                mEStop.Value = "ARMED";

            mAdapter.SendChanged();
        }

        private void load_Scroll(object sender, ScrollEventArgs e)
        {
            loadValue.Text = load.Value.ToString();
        }

        private void position_Scroll(object sender, ScrollEventArgs e)
        {
            positionValue.Text = position.Value.ToString();
        }

        private void coolant_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CuttingToolForm toolWindow = new CuttingToolForm(mAdapter);
            toolWindow.Show(this);
        }

        void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
        }
     }
}
