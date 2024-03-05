using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using System;

public class client : MonoBehaviour
{
    public bool isHost;
    public string clientName;

    private bool socketReady;
    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;

    public List<gameClient> players = new List<gameClient>();

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }


    public bool connectToServer(string host, int port)
    {
        if(socketReady)
        {
            return false;
        }
        
        try
        {
            socket = new TcpClient(host, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);

            socketReady = true;

        }
        catch(Exception e)
        {
            Debug.Log("socket error " + e.Message);
        }

        return socketReady;
    }

    private void Update()
    {
        if(socketReady)
        {
            if(stream.DataAvailable)
            {
                string data = reader.ReadLine();
                if(data != null)
                {
                    onIncomingData(data);
                }
            }
        }
    }

    //sending meassages to the server 
    public void send(string data)
    {
        if(!socketReady)
        {
            return;
        }

        writer.WriteLine(data);
        writer.Flush();
    }

    //read messages from the server
    private void onIncomingData(string data) 
    {
        Debug.Log("client: " + data);
        string[] aData = data.Split('|');
        switch(aData[0])
        {
            case "SWHO":
                for(int i = 1; i < aData.Length - 1; i++)
                {
                    userConnected(aData[i], false);
                }
                send("CWHO|" + clientName + "|" + ((isHost) ? 1 : 0).ToString());
                break;
            case "SCNN":
                userConnected(aData[1], false);
                break;
            case "SMOV":
                checkersBoard.instance.tryMove(int.Parse(aData[1]), int.Parse(aData[2]), int.Parse(aData[3]), int.Parse(aData[4]));
                break;

            case "SMAG":
              //  checkersBoard.instance.chatMessage(aData[1]);
                break;
        }
    }

    private void userConnected(string name, bool host)
    {
        gameClient c = new gameClient();
        c.name = name;

        players.Add(c);

        if(players.Count == 2) 
        {
            gameManeger.Instance.startGame();
        }
    }

    private void OnApplicationQuit()
    {
        closeSocket();
    }
    private void OnDisable()
    {
        closeSocket();
    }
    private void closeSocket()
    {
        if(!socketReady)
        { 
            return;
        }
        writer.Close();
        reader.Close(); 
        socket.Close();
        socketReady = false;
    }
}

public class gameClient
{
    public string name;
    public bool isHost;
}
