using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {

    public static PlayerCamera _playerCamera;
    public Camera _cam;

    public Transform _myPlayer;

    public float[] _range;

    private void Awake()
    {
        _playerCamera = this;
    }

    private void LateUpdate()
    {
        if(null != _myPlayer)
        {
            transform.position = new Vector3(
                Mathf.Clamp( _myPlayer.position.x, _range[0], _range[1]),
                Mathf.Clamp( _myPlayer.position.y, _range[2], _range[3]),
                transform.position.z);
        }
           
    }
}
