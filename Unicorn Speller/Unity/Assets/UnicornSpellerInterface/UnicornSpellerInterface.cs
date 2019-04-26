using System;
using UnityEngine;
using System.Net;
using Unity.ItemRecever;

public class UnicornSpellerInterface : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        try
        {
            //IP settings
            string ips = "127.0.0.1";
            int port = 1000;
            IPAddress ip = IPAddress.Parse(ips);
        
            //Start listening for Unicorn Speller network messages
            SpellerReceiver r = new SpellerReceiver(ip, port);

            //attach items received event
            r.OnItemReceived += OnItemReceived;

            Debug.Log(String.Format("Listening to {0} on port {1}.", ip, port));
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Do something...
    }

    // OnItemReceived is called if a classified item is received from Unicorn Speller via udp.
    private void OnItemReceived(object sender, EventArgs args)
    {
        ItemReceivedEventArgs eventArgs = (ItemReceivedEventArgs) args;
        Debug.Log(String.Format("Received BoardItem:\tName: {0}\tOutput Text: {1}", eventArgs.BoardItem.Name, eventArgs.BoardItem.OutputText));

        //Do something...
    }
}
