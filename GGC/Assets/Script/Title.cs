using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title : MonoBehaviour {

    private void Start()
    {
        SoundManager._soundManager.Play_BGM("TitleBgm");
    }
}
