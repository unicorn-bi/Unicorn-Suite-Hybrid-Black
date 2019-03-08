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
        liblsl.StreamInfo _lslInfo = null;

        /// <summary>
        /// The current lsl outlet.
        /// </summary>
        liblsl.StreamOutlet _lslOutlet = null;

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
            UpdateUIElements(DeviceStates.NotConnected);
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
                try
                {
                    //connect to currently selected device   
                    string serial = cbDevices.SelectedItem.ToString();

                    string streamName;
                    if (tbStreamName.Text.Length <= 0 || tbStreamName.Text == null)
                    {
                        streamName = "Unicorn";
                        tbStreamName.Text = streamName;
                    }
                    else
                        streamName = tbStreamName.Text;

                    Thread connectionThread = new Thread(() => ConnectionThread_DoWork(serial,streamName));
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
            //if device opened
            else
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
                    _lslInfo = null;
                    _lslOutlet = null;

                    UpdateUIElements(DeviceStates.NotConnected);
                }              
                catch
                {
                    _device = null;
                    _lslInfo = null;
                    _lslOutlet = null;
                    UpdateUIElements(DeviceStates.NotConnected);
                }
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
                _lslInfo = new liblsl.StreamInfo(streamName, "Data", (int) _device.GetNumberOfAcquiredChannels(), Unicorn.SamplingRate, liblsl.channel_format_t.cf_float32, serial);
                _lslOutlet = new liblsl.StreamOutlet(_lslInfo);

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
                    _lslOutlet.push_sample(receiveBufferFloat);
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
