using System;
using System.Net;
using Intendix.Board;

namespace Unity.ItemRecever
{
    public class ItemReceivedEventArgs : EventArgs
    {
        public BoardItem BoardItem { get; set; }
    }

    public class SpellerReceiver : ItemReceiver.ItemReceiver
    {
        #region Events...

        /// <summary>
        /// This method is called if a classified item is received from Unicorn Speller.
        /// </summary>
        public event EventHandler OnItemReceived;

        #endregion

        #region Private members...

        private int _port = 0;
        private IPAddress _ip = null;
        private ItemReceivedEventArgs _eventArgs;

        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="SpellerReceiver"/>. 
        /// </summary>
        /// <param name="ip">The <see cref="SpellerReceiver"/> is listening for network messages on this ip address.</param>
        /// <param name="port">The <see cref="SpellerReceiver"/> is listening for network messages on this port.</param>
        public SpellerReceiver(IPAddress ip, int port)
        {
            _ip = ip;
            _port = port;
            _eventArgs = new ItemReceivedEventArgs();
            Start();
        }

        /// <summary>
        /// This method is called if a classified item is received from Unicorn Speller.
        /// </summary>
        /// <param name="item"><see cref="BoardItem"/> represents one item of the speller board.</param>
        public override void ItemReceived(BoardItem item)
        {
            _eventArgs.BoardItem = item;
            OnItemReceived(this, _eventArgs);
        }

        /// <summary>
        /// Starts listening for network messages from speller on the defined ip and port.
        /// </summary>
        public void Start()
        {
            if (!IsRunning)
            {
                BeginReceiving(_ip, _port);
            }
            else
                throw new InvalidOperationException(String.Format("Receiver is already running."));
        }

        /// <summary>
        /// Stopps listening for network messages from speller.
        /// </summary>
        public void Stop()
        {
            if (IsRunning)
            {
                BeginReceiving(_ip, _port);
            }
            else
                throw new InvalidOperationException(String.Format("Receiver is not running yet."));
        }
    }
}