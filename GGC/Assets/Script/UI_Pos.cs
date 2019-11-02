using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Pos : MonoBehaviour {

    Transform _player;
    Transform player
    {
        get
        {
            if(_player == null && GGC_Player._player != null)
                _player = GGC_Player._player.transform;
            return _player;
        }
    }

    Camera _cam;
    Camera cam
    {
        get
        {
            if (_cam == null)
                _cam = miniCam._miniCam._cam;
            return _cam;
        }
    }


    public Text _uiPos;
	
	// Update is called once per frame
	void Update () {

        if (null == cam || null == player)
            return;

        Vector2 vPos = cam.WorldToScreenPoint(player.position);

        _uiPos.text = string.Format("{0},{1}", (int)vPos.x, (int)vPos.y);
    }
}
