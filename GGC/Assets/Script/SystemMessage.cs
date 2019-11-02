using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemMessage : MonoBehaviour {
    
    public Text _text;

    float _fTime = 1f;
    
    public void Show_Message(string text = null)
    {
        if (text != null) _text.text = text;

        gameObject.SetActive(true);
    }

    private void Update()
    {
        _fTime -= Time.deltaTime;
        if(_fTime < 0f)
        {
            gameObject.SetActive(false);
            _fTime = 2f;
        }
    }
}
