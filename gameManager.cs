using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Text;
using UnityEngine.SceneManagement;
using TMPro;

public class gameManeger : MonoBehaviour
{
    public static gameManeger Instance { set; get; }
    public GameObject mainMenu;
    public GameObject serverMenu;//host menu
    public GameObject connectMenu;

    public GameObject serverPrefab;
    public GameObject clientPrefab;

    public TMP_InputField nameInput;

    private void Start()
    {
        Instance = this;
        serverMenu.SetActive(false);
        connectMenu.SetActive(false);
        DontDestroyOnLoad(gameObject);
    }

    public void connectButton()
    {
        
        mainMenu.SetActive(false);
        connectMenu.SetActive(true);
        
        
    }
    public void hostButton()
    {
        try
        {
            server s = Instantiate(serverPrefab).GetComponent<server>();
            s.init();

            client c = Instantiate(clientPrefab).GetComponent<client>();
            c.clientName = nameInput.text;
            c.isHost = true;
            if (c.clientName == "")
            {
                c.clientName = "Host";
            }
            c.connectToServer("127.0.0.1", 6321);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

        mainMenu.SetActive(false);
        serverMenu.SetActive(true);
    }

    public void ConnectedToServerButton()
    {
        string hostAddress = GameObject.Find("hostInput").GetComponent<TMP_InputField>().text;
        if (hostAddress == "")
        {
            hostAddress = "127.0.0.1";
        }


        try
        {
            client c = Instantiate(clientPrefab).GetComponent<client>();
            c.clientName = nameInput.text;
            if(c.clientName == "")
            {
                c.clientName = "Guest";
            }
            c.connectToServer(hostAddress, 6321);
            connectMenu.SetActive(false);
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
        }
    }
    public void backButton()
    {
        mainMenu.SetActive(true);
        serverMenu.SetActive(false);
        connectMenu.SetActive(false);

        server s = FindObjectOfType<server>();
        if(s != null)
        {
            Destroy(s.gameObject);
        }

        client c = FindObjectOfType<client>();
        if (c != null)
        {
            Destroy(c.gameObject);
        }
    }
    public void hotseatButton()
    {
        SceneManager.LoadScene("game");
    }

    public void startGame()
    {
        SceneManager.LoadScene("game");
    }
}
