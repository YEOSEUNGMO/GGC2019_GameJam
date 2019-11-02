using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour {

    public static GameManager _gameManger;

    public List<GGC_Player> _listPlayer = new List<GGC_Player>();

    private void Awake()
    {
        _gameManger = this;

        
    }


    // Use this for initialization
    void Start () {
        if(SoundManager._soundManager != null)
            SoundManager._soundManager.Play_BGM("BGM");
    }

    public void GoTo_Ending()
    {
        Invoke("Go", 2f);
    }

    void Go()
    {
        PhotonNetwork.LoadLevel("Ending");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
