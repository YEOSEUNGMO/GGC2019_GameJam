using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;


public class InGameManager : MonoBehaviourPunCallbacks
{

    // Use this for initialization
    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene("Title");

            return;
        }

        // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
        PhotonNetwork.Instantiate("Prefab/Player", new Vector3(0f, 5f, 0f), Quaternion.identity, 0);

        if(PhotonNetwork.IsMasterClient)
            PhotonNetwork.Instantiate("Prefab/Boss", new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log("OnPlayerEnteredRoom() "); // not seen if you're the player connecting

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

        }
    }

    public override void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom() ");
    }
}