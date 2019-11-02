using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Boss : MonoBehaviourPunCallbacks, IPunObservable
{
    public static Boss _boss;
    public ParticleSystem _hitEffect;
    public int _hp = 300;

    public SpriteRenderer _renderer;
    public Sprite[] _eyes;
    float TwinkleTime = 2f;

    private readonly float coolDown__normalShotTime = 3f; // 일반공격
    public float _fnormalShotTime;

    private readonly float coolDown_Skill2 = 13f; // 360도
    public float _fSkill2Time;

    private readonly float coolDown_Skill = 18f; // 고치 생성
    public float _fSkillTime;
    
    private readonly float coolDown_Skill3 = 15f; // 360도
    public float _fSkill3Time;

    public Transform _mouth;
    public Transform _mouth1;
    public Transform _mouth2;

    public Transform Target1;
    public Transform Target2;
    public Transform ClosesetTarget;
    public float speed;

    private void Awake()
    {
        _boss = this;

        _hitEffect.Stop();
    }

    private void Start()
    {
        _fnormalShotTime = 3f;
        _fSkillTime = 3f;
        _fSkill2Time = 10f;
        _fSkill3Time = coolDown_Skill3;
    }

    IEnumerator Twinkle()
    {
        _renderer.sprite = _eyes[1];

        yield return new WaitForSeconds(0.1f);

        _renderer.sprite = _eyes[0];

        TwinkleTime = 2f;
    }

    // Update is called once per frame
    void Update ()
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
        {
            Vector3 vectorToTarget = ClosesetTarget.position - transform.position;
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(90+angle, Vector3.forward);
        }
        TwinkleTime -= Time.deltaTime;
        if (TwinkleTime < 0f)
            StartCoroutine(Twinkle());

        if (!PhotonNetwork.IsMasterClient)
            return;

        _fnormalShotTime -= Time.deltaTime;
        if (_fnormalShotTime < 0f)
        {
            _fnormalShotTime = coolDown__normalShotTime;
            NormalShot();
        }

        _fSkillTime -= Time.deltaTime;
        if (_fSkillTime < 0f)
        {
            _fSkillTime = coolDown_Skill;
            Skill_1();
        }

        _fSkill2Time -= Time.deltaTime;
        if(_fSkill2Time < 0f)
        {
            _fSkill2Time = coolDown_Skill2;
            StartCoroutine(skill_2());
        }

        _fSkill3Time -= Time.deltaTime;
        if (_fSkill3Time < 0f)
        {
            _fSkill3Time = coolDown_Skill3;
            SKill_3();
        }

    }

    public void Hit()
    {
        _hp--;

        photonView.RPC("Pun_Hit", RpcTarget.All, _hp);
    }

    [PunRPC]
    public void Pun_Hit(int hp)
    {
        _hp = hp;

        _hitEffect.Play();
        SoundManager._soundManager.Play_Effect("Boss_Hit");
        
        UIManager._uiManager._bossHP.fillAmount = _hp / 300f;

        if(_hp <= 0)
        {
            EffectManager._effectManager.Show_Particle("CFX2_BatsCloud", transform.position);
            //UIManager._uiManager._systemMessage.Show_Message("거미를 퇴치했습니다");
            UIManager._uiManager._victory.SetActive(true);

            if ( PhotonNetwork.IsMasterClient)
            {
                GameManager._gameManger.GoTo_Ending();

                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    private void NormalShot() // 일반 공격
    {
        GameObject obj1 = PhotonNetwork.Instantiate("Prefab/BossBullet", transform.position + transform.forward, Quaternion.identity, 0);
        obj1.GetComponent<Bullet>()._speed = 3f;
        obj1.GetComponent<Bullet>().SetInfo(_mouth.position, (_mouth.position - transform.position).normalized);

        GameObject obj2 = PhotonNetwork.Instantiate("Prefab/BossBullet", transform.position + transform.forward, Quaternion.identity, 0);
        obj2.GetComponent<Bullet>()._speed = 3f;
        obj2.GetComponent<Bullet>().SetInfo(_mouth1.position, (_mouth1.position - transform.position).normalized);

        GameObject obj3 = PhotonNetwork.Instantiate("Prefab/BossBullet", transform.position + transform.forward, Quaternion.identity, 0);
        obj3.GetComponent<Bullet>()._speed = 3f;
        obj3.GetComponent<Bullet>().SetInfo(_mouth2.position, (_mouth2.position - transform.position).normalized);


    }

    private void Skill_1() // 고치 생성
    {
        PhotonNetwork.Instantiate("Prefab/Cocoon", new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0f), Quaternion.identity, 0);
    }

    private IEnumerator skill_2() // 360도 발사
    {
        int iCount = 36;
        WaitForSeconds _sec = new WaitForSeconds(0.1f);
        Vector3 ShotPoint = transform.position;
        for (int i=0; i< iCount; ++i)
        {
            yield return _sec;

            Vector3 vDir = new Vector3( Mathf.Cos( 360 * (i / (float)iCount) * Mathf.Deg2Rad), 
                                    Mathf.Sin( 360 * (i / (float)iCount) * Mathf.Deg2Rad), 0f );

            Bullet bullet = PhotonNetwork.Instantiate("Prefab/BossBullet", ShotPoint+vDir , Quaternion.identity).GetComponent<Bullet>();
            //+vDir
            vDir.Normalize();

            bullet.SetInfo(transform.position + vDir * 2f, vDir);
            bullet._speed = 3f;
            bullet._isMine = true;


        }

        SoundManager._soundManager.Play_Effect("Boss_All_Attack");
    }
    
    private void SKill_3()
    {
        GameObject obj = PhotonNetwork.Instantiate("Prefab/BossBullet1", transform.position + transform.forward, Quaternion.identity, 0);
        obj.GetComponent<BossBullet1>().SetInfo(_mouth.position, _mouth.position - transform.position);
        obj.GetComponent<BossBullet1>()._isMine = true;
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
