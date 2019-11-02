using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySoon : MonoBehaviour {

    float _fTime = 2f;
	
	// Update is called once per frame
	void Update () {
        _fTime -= Time.deltaTime;
        if (_fTime < 0f)
            Destroy(gameObject);

    }
}
