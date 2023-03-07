using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class LoutreRushLauncher : MonoBehaviourPunCallbacks
{

    public GameObject lobbyPanel;
    public GameObject connectingPanel;
    public GameObject createRoomPanel;
    public GameObject UIRoomPrefab;
    public RectTransform RoomPanel;

    public TextMeshProUGUI RoomCreatedName;


    public Button btn;
    public TextMeshProUGUI feedbackText;
    
    bool isConnecting;


    private byte maxPlayersPerRoom = 2;
    string gameVersion = "0.1";

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

    }


    public void Connect()
    {
        // we want to make sure the log is clear everytime we connect, we might have several failed attempted if connection failed.
        feedbackText.text = "";

        // keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
        isConnecting = true;

        btn.interactable = false;

        // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
        if (PhotonNetwork.IsConnected)
        {
            LogFeedback("Joining Lobby...");
            // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
            this.JoinLobby();
        }
        else
        {

            LogFeedback("Connecting...");

            // #Critical, we must first and foremost connect to Photon Online Server.
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = this.gameVersion;
        }
    }

    void LogFeedback(string message)
    {
        // we do not assume there is a feedbackText defined.
        if (feedbackText == null)
        {
            return;
        }

        // add new messages as a new line and at the bottom of the log.
        feedbackText.text += System.Environment.NewLine + message;
    }

    public override void OnConnectedToMaster()
    {
        // we don't want to do anything if we are not attempting to join a room. 
        // this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
        // we don't want to do anything.
        PhotonNetwork.LocalPlayer.NickName = (PhotonNetwork.CountOfPlayers).ToString();
        if (isConnecting)
        {
            LogFeedback("OnConnectedToMaster: Next -> try to Join Random Room");
            Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room.\n Calling: PhotonNetwork.JoinRandomRoom(); Operation will fail if no room found");

            // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
            this.JoinLobby();
            Debug.Log("Lobby name " +PhotonNetwork.CurrentLobby.Name);



        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        LogFeedback("<Color=Red>OnJoinRandomFailed</Color>: Next -> Create a new Room");
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = this.maxPlayersPerRoom });
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        LogFeedback("<Color=Red>OnDisconnected</Color> " + cause);
        Debug.LogError("PUN Basics Tutorial/Launcher:Disconnected");

        // #Critical: we failed to connect or got disconnected. There is not much we can do. Typically, a UI system should be in place to let the user attemp to connect again.

        isConnecting = false;
        btn.interactable = true;

    }



    #region ConnectLobby

    public void JoinLobby()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = this.maxPlayersPerRoom;
        roomOptions.IsOpen = true;
        LobbyType lobbyType = new LobbyType();
        lobbyType = LobbyType.Default;
        TypedLobby typedLobby = new TypedLobby("All", lobbyType);

        bool _result = PhotonNetwork.JoinLobby(typedLobby);
        if (!_result)
        {
            Debug.LogError("PunCockpit: Could not joinLobby");
        }
    }

    public override void OnJoinedLobby()
    {
        lobbyPanel.SetActive(true);
        connectingPanel.SetActive(false);
    }
    #endregion

    #region ConnectRoom


    public void CreateRoom()
    {
        string roomName = RoomCreatedName.text;
        if (roomName.Length <= 1)
        {
            Debug.Log("Room Name is empty");
            return;
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers= this.maxPlayersPerRoom;
        roomOptions.IsOpen = true;
        LobbyType lobbyType= new LobbyType();
        lobbyType = LobbyType.Default;
        TypedLobby typedLobby = new TypedLobby("All", lobbyType);

        PhotonNetwork.CreateRoom(roomName, roomOptions, typedLobby);

    }

    public override void OnCreatedRoom()
    {
        Debug.Log("A room is created");
    }

    public override void OnJoinedRoom()
    {
        LogFeedback("<Color=Green>OnJoinedRoom</Color> with " + PhotonNetwork.CurrentRoom.PlayerCount + " Player(s)");
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.\nFrom here on, your game would be running.");
        Debug.Log(PhotonNetwork.CurrentRoom.ToString());

        // #Critical: We only load if we are the first player, else we rely on  PhotonNetwork.AutomaticallySyncScene to sync our instance scene.

    }
    #endregion


    #region HubUI
    public void DisplayCreateRoomPanel()
    {
        if (lobbyPanel.activeSelf)
        {
            lobbyPanel.SetActive(false);
        }
        createRoomPanel.SetActive(true);
    }


    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        for (int i = 0; i < RoomPanel.childCount; i++)
        {
            RoomPanel.GetChild(i).gameObject.SetActive(false);
        }
        if (!PhotonNetwork.InRoom)
        {
            Debug.Log("RoomList Count: " + roomList.Count);

            foreach (var item in roomList)
            {
                GameObject newRoomSelection = Instantiate(UIRoomPrefab, Vector2.one, Quaternion.identity);
                newRoomSelection.GetComponent<RoomSelection>().RoomName.text = item.Name;
                newRoomSelection.GetComponent<RoomSelection>().RoomPlayers.text = item.PlayerCount.ToString();
                newRoomSelection.GetComponent<RectTransform>().parent = RoomPanel;
                newRoomSelection.GetComponent<RectTransform>().localScale= Vector2.one;
            }
        }
        
        
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            Debug.Log("We load the 'LoutreRushRoom' ");

            // #Critical
            // Load the Room Level. 
            PhotonNetwork.LoadLevel("LoutreRushRoom");

        }
    }
    #endregion
}