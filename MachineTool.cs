/*
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

        Event mMode = new Event("mode");
        Event mExec = new Event("exec");

        Event mAvail = new Event("avail");
        Event mEStop = new Event("estop");
        Sample mPosition = new Sample("position");
        Sample mLoad = new Sample("load");

        Condition mSystem = new Condition("system");
        Condition mCoolant = new Condition("cool_low", true);

        Message mMessage = new Message("message");

        Event mProgram = new Event("program");

        TimeSeries mAudio = new TimeSeries("audio", 1000);
        WaveIn mWave;
        
        public MachineTool()
        {
            InitializeComponent();
            stop.Enabled = false;

            mAdapter.AddDataItem(mAvail);
            mAvail.Value = "AVAILABLE";

            mAdapter.AddDataItem(mEStop);
            mAdapter.AddDataItem(mPosition);
            mAdapter.AddDataItem(mLoad);

            mAdapter.AddDataItem(mSystem);
            mSystem.Normal();

            mAdapter.AddDataItem(mCoolant);
            mCoolant.Normal();

            mAdapter.AddDataItem(mMessage);
            mAdapter.AddDataItem(mProgram);
            mAdapter.AddDataItem(mMode);
            mAdapter.AddDataItem(mExec);

            int count = WaveIn.DeviceCount;
            for (int dev = 0; dev < count; dev++)
            {
                WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(dev);
                Console.WriteLine("Device {0}: {1}, {2} channels",
                    dev, deviceInfo.ProductName, deviceInfo.Channels);
            }

            mWave = new WaveIn();
            mWave.DeviceNumber = 0;
            mWave.WaveFormat = new WaveFormat(1000, 1);
            mWave.DataAvailable += waveIn_DataAvailable;

            mAdapter.AddDataItem(mAudio);
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

            mWave.StartRecording();
        }

        private void stop_Click(object sender, EventArgs e)
        {
            // Stop everything...
            mAdapter.Stop();
            stop.Enabled = false;
            start.Enabled = true;
            gather.Enabled = false;

            mWave.StopRecording();
        }

        private void gather_Tick(object sender, EventArgs e)
        {
            mAdapter.Begin();

            if (estop.Checked)
                mEStop.Value = "TRIGGERED";
            else
                mEStop.Value = "ARMED";

            mPosition.Value = position.Value;

            if (something.Checked)
                mSystem.Add(Condition.Level.FAULT, "You AK AK is not behaving.", "AKAK", "HIGH");
            if (flazBat.Checked)
                mSystem.Add(Condition.Level.WARNING, "You flaz bat needs attention", "FLAZBAT");

            if (automatic.Checked)
                mMode.Value = "AUTOMATIC";
            else if (mdi.Checked)
                mMode.Value = "MANUAL_DATA_INPUT";
            else
                mMode.Value = "MANUAL";

            if (running.Checked)
                mExec.Value = "ACTIVE";
            else if (feedhold.Checked)
                mExec.Value = "FEED_HOLD";
            else
                mExec.Value = "READY";

            if (messageCode.Text.Length > 0)
            {
                mMessage.Value = messageText.Text;
                mMessage.Code = messageCode.Text;
            }

            mProgram.Value = program.Text;

            mAdapter.SendChanged();
        }

        private void load_Scroll(object sender, ScrollEventArgs e)
        {
            loadValue.Text = load.Value.ToString();
            mLoad.Value = load.Value;
            mAdapter.SendChanged();
        }

        private void position_Scroll(object sender, ScrollEventArgs e)
        {
            positionValue.Text = position.Value.ToString();
        }

        private void coolant_CheckedChanged(object sender, EventArgs e)
        {
            if (coolant.Checked)
                mCoolant.Add(Condition.Level.WARNING, "You coolant is low", "COOLLOW");
            else
                mCoolant.Clear("COOLLOW");

            mAdapter.SendChanged();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CuttingTool toolWindow = new CuttingTool(mAdapter);
            toolWindow.Show(this);
        }

        void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            double[] samples = new double[e.BytesRecorded / 2];
            for (int index = 0; index < e.BytesRecorded; index += 2)
            {
                short sample = (short)((e.Buffer[index + 1] << 8) |
                                        e.Buffer[index + 0]);
                float f = sample / 32768f;
                samples[index / 2] = f;
            }

            mAudio.Values = samples;
            mAdapter.SendChanged();
        }
     }
}
