using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{

    public GameObject playerPrefab;

    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            return;
        }

        if (playerPrefab == null)
        {
            return;
        }
        
        if (PhotonNetwork.PlayerList[0].IsLocal)
        {
            PhotonNetwork.Instantiate(playerPrefab.name, Vector3.up, Quaternion.identity, 0);
            return;
        }
        PhotonNetwork.Instantiate(playerPrefab.name, Vector3.down, Quaternion.identity, 0);


    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitApplication();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        print(newPlayer.NickName + ": s'est connecté(e)");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        print(otherPlayer.NickName + ": s'est déconnecté(e)");
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("LoutreRushLauncher");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}
