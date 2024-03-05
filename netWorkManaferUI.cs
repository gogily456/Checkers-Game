using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button startClientBottun;
    
    

    private void Awake()
    {
        
        startHostButton.onClick.AddListener(() =>
        {
            Debug.Log("HOST");
            NetworkManager.Singleton.StartHost();
            hide();
           
        });

        startClientBottun.onClick.AddListener(() =>
        {
            Debug.Log("CLIENT");
            NetworkManager.Singleton.StartClient();
            hide();
        });
    }

    private void hide()
    {
        gameObject.SetActive(false);
    }

}
