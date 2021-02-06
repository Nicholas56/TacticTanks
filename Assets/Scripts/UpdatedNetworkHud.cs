using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(NetworkManager))]

public class UpdatedNetworkHud : MonoBehaviour
{
    NetworkManager manager;
    public bool showGUI = true;
    GameObject networkHud;
    public Button host;
    public Button server;
    public Button client;
    public Text serverAddress;
    public Text infoTxt;

    void Awake()
    {
        manager = GetComponent<NetworkManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            if (!NetworkClient.active)
            {
                // LAN Host or Server can't be run on WebGL
                if ((Application.platform == RuntimePlatform.WebGLPlayer))
                {
                    host.gameObject.GetComponentInChildren<TMP_Text>().text = "Cannot Host on WebGL";
                    server.gameObject.GetComponentInChildren<TMP_Text>().text = "Cannot be a Server on WebGL";
                    host.interactable = false;
                    server.interactable = false;
                }
            }
        }
    }

    public void StartHost()
    {
        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            if (!NetworkClient.active)
            {
                manager.StartHost();
                showGUI = false;
            }
            else
            {
                manager.StopClient();
            }
        }
    }

    public void StartClient()
    {
        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            if (!NetworkClient.active)
            {
                manager.StartClient();
                manager.networkAddress = serverAddress.text;
                showGUI = false;
            }
            else
            {
                manager.StopClient();
            }
        }
        else if(NetworkClient.isConnected && !ClientScene.ready)
        {
            ClientScene.Ready(NetworkClient.connection);

            if (ClientScene.localPlayer == null)
            {
                ClientScene.AddPlayer();
                showGUI = false;
            }
        }
    }

    public void StartServer()
    {
        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            if (!NetworkClient.active)
            {
                manager.StartServer();
                showGUI = false;
            }
            else
            {
                manager.StopClient();
            }
        }

    }

    public void Stop()
    {
        if (NetworkServer.active || NetworkClient.isConnected)
        {
            manager.StopHost();
            showGUI = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!showGUI)
        {
            host.gameObject.transform.parent.transform.parent.gameObject.SetActive(false);
            return;
        }
        else if(showGUI)
        {
            host.gameObject.transform.parent.transform.parent.gameObject.SetActive(true);

        }
        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            if (NetworkClient.active)
            {
                host.gameObject.GetComponentInChildren<TMP_Text>().text = "Cancel Connection Attempt";
                server.gameObject.GetComponentInChildren<TMP_Text>().text = "Cancel Connection Attempt";
                client.gameObject.GetComponentInChildren<TMP_Text>().text = "Cancel Connection Attempt";

                infoTxt.text = "Connecting to " + manager.networkAddress;
            }
            else
            {
                host.gameObject.GetComponentInChildren<TMP_Text>().text = "Host";
                server.gameObject.GetComponentInChildren<TMP_Text>().text = "Server Only";
                client.gameObject.GetComponentInChildren<TMP_Text>().text = "Join";
            }
        }
        else
        {
            // server / client status message
            if (NetworkServer.active)
            {
                infoTxt.text=("Server: active. Transport: " + Transport.activeTransport);
            }
            if (NetworkClient.isConnected)
            {
                infoTxt.text=("Client: address=" + manager.networkAddress);
            }
        }

        // client ready
        if (NetworkClient.isConnected && !ClientScene.ready)
        {
            client.gameObject.GetComponentInChildren<TMP_Text>().text = "Join";
        }

    }

}
        
  
