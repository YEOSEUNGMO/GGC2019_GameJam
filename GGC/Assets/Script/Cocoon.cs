using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Cocoon : MonoBehaviourPunCallbacks, IPunObservable
{ 
    public int HP = 5;
    private float _fEvolve = 7f;
    private float _fEvolveAfter = 3f;

    bool bEvolve = false;

    public SpriteRenderer _renderer;
    public Sprite _coCoon2;

    void Spawn_Spider(int count)
    {
        //sipder 스폰
        for (int i = 0; i < count; ++i)
        {
            Vector3 vPos = new Vector3(
                Mathf.Cos(360 * (i / (count / 2f))) * Mathf.Deg2Rad,
                        Mathf.Sin(360 * (count / 2f)) * Mathf.Deg2Rad, 0f) * 20f;

            PhotonNetwork.Instantiate("Prefab/Spider", vPos + transform.position
                , Quaternion.identity, 0);
        }

        photonView.RPC("Summon_Effect", RpcTarget.All);
    }

    [PunRPC]
    public void Summon_Effect()
    {
        EffectManager._effectManager.Show_Particle("CFX3_Hit_SmokePuff", transform.position);
        SoundManager._soundManager.Play_Effect("Summon");
    }

    public void Hit()
    {
        photonView.RPC("Pun_Hit", RpcTarget.All);
    }

    [PunRPC]
    public void Pun_Hit()
    {

        EffectManager._effectManager.Show_Particle("CFX3_Skull_Explosion", transform.position, transform);

        if (PhotonNetwork.IsMasterClient)
        {
            HP--;
            if (HP == 0)
            {
                Spawn_Spider(4);

                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
    // Use this for initialization
    void Start ()
    {
        photonView.RPC("Pun_Show_Message", RpcTarget.All);
    }

    [PunRPC]
    void Pun_Show_Message()
    {
        UIManager._uiManager._systemMessage.Show_Message("[System] 고치가 생성되었습니다");
    }

    [PunRPC]
    void Pun_Show_Effect()
    {
        _renderer.sprite = _coCoon2;

        EffectManager._effectManager.Show_Particle("CFX4 Disruptive Force", transform.position);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        _fEvolve -= Time.deltaTime;

        if(_fEvolve < 0f)
        {
            if(!bEvolve)
            {
                // 진화!
                bEvolve = true;
                photonView.RPC("Pun_Show_Effect", RpcTarget.All);
            }
        }

        if(bEvolve)
        {
            _fEvolveAfter -= Time.deltaTime;

            if (_fEvolveAfter < 0f)
            {
                Spawn_Spider(10);
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
        }
        else
        {
            // Network player, receive data

        }
    }
}
