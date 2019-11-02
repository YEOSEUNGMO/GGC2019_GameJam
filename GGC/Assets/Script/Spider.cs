using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Spider : MonoBehaviourPunCallbacks, IPunObservable
{
    public Transform Target1;
    public Transform Target2;
    public Transform ClosesetTarget;
    public float Speed;
    private bool IsMoving = true;

    public SpriteRenderer _sprRenderer;
    public Sprite _dead;

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

    void Start()
    {

    }

    void Update()
    {
        if (GameManager._gameManger._listPlayer.Count == 1)
        {
            Target1 = GameManager._gameManger._listPlayer[0].transform;
        }
        else if (GameManager._gameManger._listPlayer.Count > 1)
        {
            Target1 = GameManager._gameManger._listPlayer[0].transform;
            Target2 = GameManager._gameManger._listPlayer[1].transform;
        }

        if (Target2 != null)
        {
            if (Vector3.Distance(transform.position, Target1.position) < Vector3.Distance(transform.position, Target2.position))
            {
                ClosesetTarget = Target1;
            }
            else
            {
                ClosesetTarget = Target2;
            }
        }
        else if (Target1 != null)
        {
            ClosesetTarget = Target1;
        }

        if (ClosesetTarget != null)
            transform.position = Vector3.MoveTowards(transform.position, ClosesetTarget.position, Speed * Time.deltaTime);
    }

    public void Hit() // by bullet
    {
        photonView.RPC("Pun_Hit", RpcTarget.All);
    }

    [PunRPC]
    public void Pun_Hit()
    {
        EffectManager._effectManager.Show_Particle("CFX3_Hit_Misc_D", transform.position);
        SoundManager._soundManager.Play_Effect("Spider_Dead");

        Dead_Effect();
    }

    void Dead_Effect()
    {
        _sprRenderer.sprite = _dead;
        GetComponent<BoxCollider2D>().enabled = false;
        enabled = false;

        Invoke("Dead", 2f);
    }

    void Dead()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider.tag == "Player")
        {
            collision.collider.GetComponent<GGC_Player>().Hit();

            photonView.RPC("Pun_Show_Destroy_Effect", RpcTarget.All);

            Dead_Effect();
        }
    }

    [PunRPC]
    void Pun_Show_Destroy_Effect()
    {
        EffectManager._effectManager.Show_Particle("CFX2_Blood", transform.position);
    }
}