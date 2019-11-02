using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour {

    public static EffectManager _effectManager;

    public GameObject[] _prefabs;
    private Dictionary<string, GameObject> _dicEffect = new Dictionary<string, GameObject>();

    private void Awake()
    {
        _effectManager = this;

        for(int i=0; i<_prefabs.Length; ++i)
        {
            _dicEffect.Add(_prefabs[i].name, _prefabs[i]);
        }
    }

    public void Show_Particle(string name, Vector3 vPos, Transform parent = null)
    {
        GameObject obj = Instantiate(_dicEffect[name], vPos, Quaternion.identity, transform);
        obj.AddComponent<MyParticle>();

        if (null != parent)
            obj.transform.SetParent(parent);
    }
}
