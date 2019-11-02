using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyParticle : MonoBehaviour {

    ParticleSystem _particle;

    float _fMaxTime = 3f;

    private void Awake()
    {
        _particle = GetComponent<ParticleSystem>();

        if (_particle == null)
            return;
    }

    public void Update()
    {
        if (_particle)
        {
            if (!_particle.IsAlive())
            {
                Destroy(gameObject);
            }
        }

        _fMaxTime -= Time.deltaTime;
        if(_fMaxTime < 0f) Destroy(gameObject);
    }
}
