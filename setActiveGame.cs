using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class setActiveGame : NetworkBehaviour
{
    [SerializeField] private GameObject blackPieceTimer;
    [SerializeField] private GameObject whitePieceTimer;
    [SerializeField] private GameObject alertWindow;

    public void setActiveGameElements()
    {
        blackPieceTimer.SetActive(true);
        whitePieceTimer.SetActive(true);
        alertWindow.SetActive(true);
    }

    public void setNonActiveGameElements()
    {
        blackPieceTimer.SetActive(false);
        whitePieceTimer.SetActive(false);
        alertWindow.SetActive(false);
    }
}
