using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System;
using UnityEngine;
using System.Net;
using System.IO;
//using UnityEditor.XR;

public class server : MonoBehaviour
{
    public int port = 6321;

    private List<serverClient> clients;
    private List<serverClient> disconectList;

    private TcpListener serverr;
    private bool serverStarted;

    public void init()
    {
        DontDestroyOnLoad(gameObject);
        clients = new List<serverClient>();
        disconectList = new List<serverClient>();

        try
        {
            serverr = new TcpListener(IPAddress.Any, port);
            serverr.Start();

            startListening();
            serverStarted = true;
        }
        catch(Exception e)
        {
            Debug.Log("socket error: " + e.Message);
        }
    }

    private void Update()
    {
        if (!serverStarted)
        {
            return;
        }

        foreach (serverClient c in clients)
        {
            //is the client still connected?
            if(!isConnected(c.tcp))
            {
                c.tcp.Close();
                disconectList.Add(c);
                continue;
            }
            else
            {
                NetworkStream s = c.tcp.GetStream();
                if(s.DataAvailable)
                {
                    StreamReader reader = new StreamReader(s, true);
                    string data = reader.ReadLine();

                    if(data != null) 
                    {
                        onInComingData(c, data);
                    }
                }
            }
        }
        for(int i = 0; i < disconectList.Count - 1; i++)
        {
            //tell our player somebody disconected

            clients.Remove(disconectList[i]);
            disconectList.RemoveAt(i);
        }
    }

    private void startListening()
    {
        serverr.BeginAcceptTcpClient(acceptTcpClient, serverr);
    }
    private void acceptTcpClient(IAsyncResult ar)
    {
        TcpListener listener = (TcpListener)ar.AsyncState;

        String allUsers = "";
        foreach (serverClient cl in clients)
        {
            allUsers = allUsers + cl.clientName + '|';
        }

        serverClient sc = new serverClient(listener.EndAcceptTcpClient(ar));
        clients.Add(sc);

        startListening();

       
        broadcast("SWHO|" + allUsers, clients[clients.Count - 1]);
    }

    private bool isConnected(TcpClient c)
    {
        try
        {
            if(c != null && c.Client != null && c.Client.Connected)
            {
                if(c.Client.Poll(0, SelectMode.SelectRead))
                {
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) ==  0);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
    }

    
    private void broadcast(string data, List<serverClient> cl)//send from server
    {
        foreach(serverClient sc in cl)
        {
            try
            {
                StreamWriter writer = new StreamWriter(sc.tcp.GetStream());
                writer.WriteLine(data);
                writer.Flush();
            }
            catch (Exception e)
            { 
               Debug.Log("Write error: " + e.Message);
            }
        }
    }
    private void broadcast(string data, serverClient c)//send from server
    {
        List<serverClient> sc = new List<serverClient> { c };
        broadcast(data, sc);
    }
    private void onInComingData(serverClient c, string data)//read from server
    {
        Debug.Log("server: " + data);
        string[] aData = data.Split('|');
        switch (aData[0])
        {
            case "CWHO":
                c.clientName = aData[1];
                c.isHost = (aData[2] == "0") ? false : true;
                broadcast("SCNN|" + c.clientName, clients);
                break;

            case "CMOV":
                broadcast("SMOV|" + aData[1] + "|" + aData[2] + "|" + aData[3] + "|" + aData[4], clients);
                break;

            case "CMSG":
                broadcast("SMSG| " + c.clientName + ": " + aData[1], clients);
                break;
        }
    }
}

public class serverClient
{
    public string clientName;
    public TcpClient tcp;
    public bool isHost;

    public serverClient(TcpClient tcp)
    {
       this.tcp = tcp;
    }
}
