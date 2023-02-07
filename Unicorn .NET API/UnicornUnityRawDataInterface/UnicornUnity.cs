using Gtec.Unicorn;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.Globalization;

public class UnicornUnity : MonoBehaviour
{
    private Unicorn _unicorn = null;
    private Thread _acquisitionThread = null;
    private bool _acquisitionThreadRunning = false;
    private int _frameLength = 1;
    void Start()
    {
        //get paired devices
        IList<string> serials = Unicorn.GetAvailableDevices(true);

        //connect to first device
        _unicorn = new Unicorn(serials[0]);

        //start acquisition
        _unicorn.StartAcquisition(false);

        //start acquisition thread
        _acquisitionThread = new Thread(AcquisitionThread_DoWork);
        _acquisitionThreadRunning = true;
        _acquisitionThread.Start();

        Debug.Log("Connected and started data acquisition");
    }

    private void AcquisitionThread_DoWork(object obj)
    {
        //allocate receive buffer
        uint numberOfAcquiredChannels = _unicorn.GetNumberOfAcquiredChannels();
        byte[] receiveBuffer = new byte[_frameLength * sizeof(float) * numberOfAcquiredChannels];
        GCHandle receiveBufferHandle = GCHandle.Alloc(receiveBuffer, GCHandleType.Pinned);
      
        //acquisition loop
        while (_acquisitionThreadRunning)
        {
            //get data
            _unicorn.GetData((uint)_frameLength, receiveBufferHandle.AddrOfPinnedObject(), (uint)(receiveBuffer.Length / sizeof(float)));

            //convert to µV
            float[] data = new float[numberOfAcquiredChannels];
            for(int i = 0; i < numberOfAcquiredChannels; i++)
                data[i] = BitConverter.ToSingle(receiveBuffer, i* sizeof(float));
            Debug.Log(string.Join(",", data));
        }

        //free receive buffer
        receiveBufferHandle.Free();
    }

    private void OnApplicationQuit()
    {
        //stop acquisition thread
        _acquisitionThreadRunning = false;
        _acquisitionThread.Join();

        //stop acquisition
        _unicorn.StopAcquisition();

        //disconnect
        _unicorn.Dispose();
        _unicorn = null;
        Debug.Log("Disconnected and stopped data acquisition.");
    }
}
