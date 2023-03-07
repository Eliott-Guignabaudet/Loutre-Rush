using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using Unity.VisualScripting;

public class RoomSelection : MonoBehaviour
{
    public TextMeshProUGUI RoomName;
    public TextMeshProUGUI RoomPlayers;
    public RoomInfo RoomInfo;




    public void OnClick()
    {
        LoutreRushLauncher.Instance.ChangeTogglvalue(this.gameObject);
    }
}
