using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public static SoundManager _soundManager;

    public AudioSource _bgm;
    public AudioSource _effect;
    public AudioSource _shot;

    public AudioClip[] clip;

    private Dictionary<string, AudioClip> _dicClip = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        _soundManager = this;
        
        for (int i = 0; i < clip.Length; ++i)
        {
            _dicClip.Add(clip[i].name, clip[i]);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void Play_BGM(string bgm)
    {
        _bgm.Stop();
        _bgm.clip = _dicClip[bgm];

        _bgm.Play();
    }

    public void Play_Effect(string effect)
    {
        _effect.Stop();
        _effect.clip = _dicClip[effect];
        
        _effect.Play();
    }

    public void Play_Shot(string effect)
    {
        _shot.Stop();
        _shot.clip = _dicClip[effect];

        _shot.Play();
    }
}
