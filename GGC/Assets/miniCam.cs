using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class miniCam : MonoBehaviour
{
    public static miniCam _miniCam;
    public Camera _cam;


    private void Awake()
    {
        _miniCam = this;
        _cam = GetComponent<Camera>();
    }
}
