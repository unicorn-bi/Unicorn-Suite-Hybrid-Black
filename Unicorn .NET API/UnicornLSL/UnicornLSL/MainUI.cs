using System;
using System.Drawing;
using System.Windows.Forms;
using Gtec.Unicorn;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.InteropServices;
using LSL;

namespace UnicornLSL
{
    public partial class MainUI : Form
    {
        /// <summary>
        /// The frame length data is acquired with.
        /// </summary>
        private const int FrameLength = 1;

        /// <summary>
        /// Available UI states.
        /// </summary>
        private enum DeviceStates { NotConnected, Connecting, Connected, Acquiring };

        /// <summary>
        /// The current device.
        /// </summary>
        private Unicorn _device = null;

        /// <summary>
        /// The current lsl info.
        /// </summary>
        List<liblsl.StreamInfo> _lslInfos = null;

        /// <summary>
        /// The current lsl outlet.
        /// </summary>
        List<liblsl.StreamOutlet> _lslOutlets = null;

        /// <summary>
        /// Flag indicating the acquisition state.
        /// </summary>
        private bool _acquisitionRunning = false;

        /// <summary>
        /// The acquisition thread.
        /// </summary>
        private Thread _acquisitionThread = null;

        public MainUI()
        {
            InitializeComponent();

            rbSplitSignals.Checked = false;
            rbCombineSigbnals.Checked = true;

            UpdateUIElements(DeviceStates.NotConnected);
        }

        /// <summary>
        /// Terminates acquisition and stream if running
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainUI_FormClosed(object sender, FormClosedEventArgs e)
        {
            Uninitialize();
        }

        /// <summary>
        /// Update available devices when UI is shown.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainUI_Shown(object sender, EventArgs e)
        {
            try
            {
                GetAvailableDevices();
            }
            catch (DeviceException ex)
            {
                ShowErrorBox(ex.Message);
            }
            catch (Exception ex)
            {
                ShowErrorBox(String.Format("Could not determine number of available devices. {0}", ex.Message));
            }
        }

        /// <summary>
        /// The refresh button click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                GetAvailableDevices();
            }
            catch (DeviceException ex)
            {
                ShowErrorBox(ex.Message);
            }
            catch (Exception ex)
            {
                ShowErrorBox(String.Format("Could not determine number of available devices. {0}", ex.Message));
            }
        }

        /// <summary>
        /// The open/close button click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenClose_Click(object sender, EventArgs e)
        {
            //if no device opened
            if(_device == null)
            {
                Initialize();
            }
            //if device opened
            else
            {
                Uninitialize();
            }        
        }

        /// <summary>
        /// Connects to a device and creates lsl streams.
        /// </summary>
        private void Initialize()
        {
            try
            {
                //connect to currently selected device   
                string serial = cbDevices.SelectedItem.ToString();

                string streamName;
                if (tbStreamName.Text.Length <= 0 || tbStreamName.Text == null)
                {
                    streamName = serial;
                    tbStreamName.Text = streamName;
                }
                else
                    streamName = tbStreamName.Text;

                Thread connectionThread = new Thread(() => ConnectionThread_DoWork(serial, streamName));
                connectionThread.Start();
            }
            catch (DeviceException ex)
            {
                _device = null;
                UpdateUIElements(DeviceStates.NotConnected);
                ShowErrorBox(ex.Message);
            }
            catch (Exception ex)
            {
                _device = null;
                UpdateUIElements(DeviceStates.NotConnected);
                ShowErrorBox(String.Format("Could not open device. {0}", ex.Message));
            }
        }

        /// <summary>
        /// Terminates a running data acquisition, closes the device and lsl outlets.
        /// </summary>
        private void Uninitialize()
        {
            try
            {
                //stop acquisition thread if running
                if (_acquisitionRunning)
                {
                    _acquisitionRunning = false;
                    _acquisitionThread.Join();
                }

                //close device
                _device.Dispose();
                _device = null;

                //uninitialize lsl
                if (_lslInfos.Count > 0)
                    _lslInfos.Clear();

                _lslInfos = null;

                if (_lslOutlets.Count > 0)
                    _lslOutlets.Clear();

                _lslOutlets = null;

                UpdateUIElements(DeviceStates.NotConnected);
            }
            catch
            {
                _device = null;
                if (_lslInfos != null &&_lslInfos.Count > 0)
                    _lslInfos.Clear();

                _lslInfos = null;

                if (_lslOutlets != null && _lslOutlets.Count > 0)
                    _lslOutlets.Clear();

                _lslOutlets = null;

                UpdateUIElements(DeviceStates.NotConnected);
            }
        }

        /// <summary>
        /// The start/stop button click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if(!_acquisitionRunning)
            {
                //start acquisition thread
                _acquisitionThread = new Thread(() => AcquisitionThread_DoWork());
                _acquisitionThread.Start();
            }
            else
            {
                //stop acquisition thread
                _acquisitionRunning = false;
                _acquisitionThread.Join();
                UpdateUIElements(DeviceStates.Connected);
            }         
        }

        /// <summary>
        /// Determines available device serials and writes them to the available devices box.
        /// </summary>
        private void GetAvailableDevices()
        {
            List<string> devices = new List<string>(Unicorn.GetAvailableDevices(true));
            if (devices.Count > 0 && cbDevices != null)
            {
                cbDevices.DataSource = devices;
            }
        }

        /// <summary>
        /// Connects to a Unicorn.
        /// </summary>
        /// <param name="serial">The serial of the device to connect.</param>
        private void ConnectionThread_DoWork(string serial, string streamName)
        {
            try
            {
                UpdateUIElements(DeviceStates.Connecting);

                //Open device
                _device = new Unicorn(serial);

                //Initialize lsl
                if(_lslInfos == null)
                    _lslInfos = new List<liblsl.StreamInfo>();

                if(_lslOutlets == null)
                    _lslOutlets = new List<liblsl.StreamOutlet>();

                if (rbCombineSigbnals.Checked)
                {
                    _lslInfos.Add(new liblsl.StreamInfo(streamName, "Data", (int)_device.GetNumberOfAcquiredChannels(), Unicorn.SamplingRate, liblsl.channel_format_t.cf_float32, serial));
                    _lslOutlets.Add(new liblsl.StreamOutlet(_lslInfos[0]));
                }
                else
                {
                    int cnt = 0;
                    _lslInfos.Add(new liblsl.StreamInfo(streamName + "_EEG", "EEG", (int)Unicorn.EEGChannelsCount, Unicorn.SamplingRate, liblsl.channel_format_t.cf_float32, serial));
                    _lslOutlets.Add(new liblsl.StreamOutlet(_lslInfos[cnt]));
                    cnt++;
                    _lslInfos.Add(new liblsl.StreamInfo(streamName + "_ACC", "ACC", (int)Unicorn.AccelerometerChannelsCount, Unicorn.SamplingRate, liblsl.channel_format_t.cf_float32, serial));
                    _lslOutlets.Add(new liblsl.StreamOutlet(_lslInfos[cnt]));
                    cnt++;
                    _lslInfos.Add(new liblsl.StreamInfo(streamName + "_GYR", "GYR", (int)Unicorn.GyroscopeChannelsCount, Unicorn.SamplingRate, liblsl.channel_format_t.cf_float32, serial));
                    _lslOutlets.Add(new liblsl.StreamOutlet(_lslInfos[cnt]));
                    cnt++;
                    _lslInfos.Add(new liblsl.StreamInfo(streamName + "_CNT", "CNT", (int)1, Unicorn.SamplingRate, liblsl.channel_format_t.cf_float32, serial));
                    _lslOutlets.Add(new liblsl.StreamOutlet(_lslInfos[cnt]));
                    cnt++;
                    _lslInfos.Add(new liblsl.StreamInfo(streamName + "_BAT", "BAT", (int)1, Unicorn.SamplingRate, liblsl.channel_format_t.cf_float32, serial));
                    _lslOutlets.Add(new liblsl.StreamOutlet(_lslInfos[cnt]));
                    cnt++;
                    _lslInfos.Add(new liblsl.StreamInfo(streamName + "_VALID", "VALID", (int)1, Unicorn.SamplingRate, liblsl.channel_format_t.cf_float32, serial));
                    _lslOutlets.Add(new liblsl.StreamOutlet(_lslInfos[cnt]));
                }

                UpdateUIElements(DeviceStates.Connected);
            }
            catch (DeviceException ex)
            {
                _device = null;
                UpdateUIElements(DeviceStates.NotConnected);
                ShowErrorBox(ex.Message);
            }
            catch (Exception ex)
            {
                _device = null;
                UpdateUIElements(DeviceStates.NotConnected);
                ShowErrorBox(String.Format("Could not open device. {0}", ex.Message));
            }
        }

        /// <summary>
        /// The acquisition thread.
        /// Acquires data from the Unicorn and sends it to LSL.
        /// </summary>
        private void AcquisitionThread_DoWork()
        {
            try
            {
                //Initialize Unicorn acquisition members
                uint numberOfAcquiredChannels = _device.GetNumberOfAcquiredChannels();
                byte[] receiveBuffer = new byte[FrameLength * sizeof(float) * numberOfAcquiredChannels];
                GCHandle receiveBufferHandle = GCHandle.Alloc(receiveBuffer, GCHandleType.Pinned);
                float[] receiveBufferFloat = new float[receiveBuffer.Length / sizeof(float)];

                //Start acquisition
                _device.StartAcquisition(false);
                _acquisitionRunning = true;
                UpdateUIElements(DeviceStates.Acquiring);

                //acquisition loop
                while (_acquisitionRunning)
                {
                    //get data
                    _device.GetData(FrameLength, receiveBufferHandle.AddrOfPinnedObject(), (uint) (receiveBuffer.Length / sizeof(float)));

                    //convert byte array to float array for LSL
                    for (int j = 0; j < receiveBuffer.Length / sizeof(float); j++)
                        receiveBufferFloat[j] = BitConverter.ToSingle(receiveBuffer, j * sizeof(float));

                    // send sample via LSL
                    if (rbCombineSigbnals.Checked)
                    {
                        _lslOutlets[0].push_sample(receiveBufferFloat);
                    }
                    else
                    {
                        for(int i = 0; i < _lslOutlets.Count; i ++)
                        {
                            if(i==0)
                            {
                                float[] eeg = new float[Unicorn.EEGChannelsCount];
                                Array.Copy(receiveBufferFloat, 0, eeg, 0, Unicorn.EEGChannelsCount);
                                _lslOutlets[i].push_sample(eeg);
                            }
                            else if (i == 1)
                            {
                                float[] acc = new float[Unicorn.AccelerometerChannelsCount];
                                Array.Copy(receiveBufferFloat, Unicorn.EEGChannelsCount, acc, 0, Unicorn.AccelerometerChannelsCount);
                                _lslOutlets[i].push_sample(acc);
                            }
                            else if (i == 2)
                            {
                                float[] gyr = new float[Unicorn.GyroscopeChannelsCount];
                                Array.Copy(receiveBufferFloat, Unicorn.EEGChannelsCount + Unicorn.AccelerometerChannelsCount, gyr, 0, Unicorn.GyroscopeChannelsCount);
                                _lslOutlets[i].push_sample(gyr);
                            }
                            else if (i == 3)
                            {
                                float[] cnt = new float[1];
                                Array.Copy(receiveBufferFloat, Unicorn.EEGChannelsCount + Unicorn.AccelerometerChannelsCount + Unicorn.GyroscopeChannelsCount, cnt, 0, 1);
                                _lslOutlets[i].push_sample(cnt);
                            }
                            else if (i == 4)
                            {
                                float[] bat = new float[1];
                                Array.Copy(receiveBufferFloat, Unicorn.EEGChannelsCount + Unicorn.AccelerometerChannelsCount + Unicorn.GyroscopeChannelsCount + 1, bat, 0, 1);
                                _lslOutlets[i].push_sample(bat);
                            }
                            else if (i == 5)
                            {
                                float[] val = new float[1];
                                Array.Copy(receiveBufferFloat, Unicorn.EEGChannelsCount + Unicorn.AccelerometerChannelsCount + Unicorn.GyroscopeChannelsCount + 2, val, 0, 1);
                                _lslOutlets[i].push_sample(val);
                            }
                        }
                    }  
                }
            }
            finally
            {
                try
                {
                    _device.StopAcquisition();
                }
                catch
                {
                    _device.Dispose();
                    _device = null;
                    UpdateUIElements(DeviceStates.NotConnected);
                }
            }
        }

        #region UI Methods...

        private void UpdateUIElements(DeviceStates state)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker) delegate
                {
                    UpdateUIElements(state);
                });
            }
            else
            {
                if (state == DeviceStates.NotConnected)
                {
                    cbDevices.Enabled = true;
                    btnRefresh.Enabled = true;
                    btnOpenClose.Text = "Open";
                    btnOpenClose.Enabled = true;
                    btnStartStop.Text = "Start";
                    btnStartStop.Enabled = false;
                    tbStreamName.Enabled = true;
                    rbSplitSignals.Enabled = true;
                    rbCombineSigbnals.Enabled = true;
                }
                else if (state == DeviceStates.Connecting)
                {
                    cbDevices.Enabled = false;
                    btnRefresh.Enabled = false;
                    btnOpenClose.Text = "Open";
                    btnOpenClose.Enabled = false;
                    btnStartStop.Text = "Start";
                    btnStartStop.Enabled = false;
                    tbStreamName.Enabled = false;
                    rbSplitSignals.Enabled = false;
                    rbCombineSigbnals.Enabled = false;
                }
                else if (state == DeviceStates.Connected)
                {
                    cbDevices.Enabled = false;
                    btnRefresh.Enabled = false;
                    btnOpenClose.Text = "Close";
                    btnOpenClose.Enabled = true;
                    btnStartStop.Text = "Start";
                    btnStartStop.Enabled = true;
                    tbStreamName.Enabled = false;
                    rbSplitSignals.Enabled = false;
                    rbCombineSigbnals.Enabled = false;
                }
                else if (state == DeviceStates.Acquiring)
                {
                    cbDevices.Enabled = false;
                    btnRefresh.Enabled = false;
                    btnOpenClose.Text = "Close";
                    btnOpenClose.Enabled = true;
                    btnStartStop.Text = "Stop";
                    btnStartStop.Enabled = true;
                    tbStreamName.Enabled = false;
                    rbSplitSignals.Enabled = false;
                    rbCombineSigbnals.Enabled = false;
                }
                else
                {
                    cbDevices.Enabled = false;
                    btnRefresh.Enabled = false;
                    btnOpenClose.Text = "Open";
                    btnOpenClose.Enabled = false;
                    btnStartStop.Text = "Start";
                    btnStartStop.Enabled = false;
                    tbStreamName.Enabled = false;
                    rbSplitSignals.Enabled = false;
                    rbCombineSigbnals.Enabled = false;
                }
            }
        }

        private void ShowErrorBox(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker) delegate
                {
                    ShowErrorBox(message);
                });
            }
            else
            {
                MessageBox.Show(this, message, "Unicorn LSL - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_EnabledChanged(object sender, EventArgs e)
        {
            if (sender is Button)
                ApplyButtonStyle((Button) sender);
        }

        private void btnStartStop_EnabledChanged(object sender, EventArgs e)
        {
            if (sender is Button)
                ApplyButtonStyle((Button) sender);
        }

        private void btnOpenClose_EnabledChanged(object sender, EventArgs e)
        {
            if (sender is Button)
                ApplyButtonStyle((Button) sender);
        }

        private void ApplyButtonStyle(Button btn)
        {
            btn.ForeColor = Color.White;
            if (btn.Enabled)
                btn.BackColor = Color.FromArgb(255, 64, 64, 64);
            else
                btn.BackColor = Color.LightGray;
        }

        private void pnlLayout_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            if (e.Row == 0)
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(255, 44, 44, 44)), e.CellBounds);
        }

        #endregion
    }
}
