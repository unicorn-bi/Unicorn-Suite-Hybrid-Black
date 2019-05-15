using System;
using System.Drawing;
using System.Windows.Forms;
using Gtec.Unicorn;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.Sockets;

namespace UnicornUDP
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
        /// Flag indicating the acquisition state.
        /// </summary>
        private bool _acquisitionRunning = false;

        /// <summary>
        /// The acquisition thread.
        /// </summary>
        private Thread _acquisitionThread = null;

        /// <summary>
        /// The udp socket.
        /// </summary>
        private Socket _socket = null;

        /// <summary>
        /// The ip endpoint.
        /// </summary>
        IPEndPoint _endPoint;

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

                    IPAddress ip = IPAddress.Parse(tbIP.Text);
                    int port = Int32.Parse(tbPort.Text);

                    Thread connectionThread = new Thread(() => ConnectionThread_DoWork(serial, ip, port));
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

                    UpdateUIElements(DeviceStates.NotConnected);
                }              
                catch
                {
                    _device = null;;
                    UpdateUIElements(DeviceStates.NotConnected);
                }
                finally
                {
                    try
                    {
                        //close socket connection
                        _socket.Shutdown(SocketShutdown.Both);
                        _socket.Close();
                    }
                    catch(Exception ex)
                    {
                        ShowErrorBox(String.Format("Could not close socket. {0}", ex.Message));
                    }
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
        private void ConnectionThread_DoWork(string serial, IPAddress ip, int port)
        {
            try
            {
                UpdateUIElements(DeviceStates.Connecting);

                //Open device
                _device = new Unicorn(serial);

                //Initialize upd socket
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                _endPoint = new IPEndPoint(ip, port);
                _socket.Connect(_endPoint);

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

                //Start acquisition
                _device.StartAcquisition(false);
                _acquisitionRunning = true;
                UpdateUIElements(DeviceStates.Acquiring);

                //acquisition loop
                while (_acquisitionRunning)
                {
                    //get data
                    _device.GetData(FrameLength, receiveBufferHandle.AddrOfPinnedObject(), (uint) (receiveBuffer.Length / sizeof(float)));

                    // send sample via UDP
                    _socket.SendTo(receiveBuffer, _endPoint);
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
                    tbIP.Enabled = true;
                    tbPort.Enabled = true;
                }
                else if (state == DeviceStates.Connecting)
                {
                    cbDevices.Enabled = false;
                    btnRefresh.Enabled = false;
                    btnOpenClose.Text = "Open";
                    btnOpenClose.Enabled = false;
                    btnStartStop.Text = "Start";
                    btnStartStop.Enabled = false;
                    tbIP.Enabled = false;
                    tbPort.Enabled = false;
                }
                else if (state == DeviceStates.Connected)
                {
                    cbDevices.Enabled = false;
                    btnRefresh.Enabled = false;
                    btnOpenClose.Text = "Close";
                    btnOpenClose.Enabled = true;
                    btnStartStop.Text = "Start";
                    btnStartStop.Enabled = true;
                    tbIP.Enabled = false;
                    tbPort.Enabled = false;
                }
                else if (state == DeviceStates.Acquiring)
                {
                    cbDevices.Enabled = false;
                    btnRefresh.Enabled = false;
                    btnOpenClose.Text = "Close";
                    btnOpenClose.Enabled = true;
                    btnStartStop.Text = "Stop";
                    btnStartStop.Enabled = true;
                    tbIP.Enabled = false;
                    tbPort.Enabled = false;
                }
                else
                {
                    cbDevices.Enabled = false;
                    btnRefresh.Enabled = false;
                    btnOpenClose.Text = "Open";
                    btnOpenClose.Enabled = false;
                    btnStartStop.Text = "Start";
                    btnStartStop.Enabled = false;
                    tbIP.Enabled = false;
                    tbPort.Enabled = false;
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
                MessageBox.Show(this, message, "Unicorn UDP - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
