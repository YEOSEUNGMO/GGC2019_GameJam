using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GGC_Player : MonoBehaviourPunCallbacks, IPunObservable
{
    public static GGC_Player _player;

    public PhotonView _photonView;

    public SpriteRenderer _playerImage;
    public Sprite[] _sprites;

    public Transform _fire;
    public Transform _range;
    float _curAngle;

    private Vector3 _vDir;

    private Camera _cam;

    public float[] _fRange;

    public TextMesh _textMesh;

    public GameObject[] _trHp;

    float _fTime = 0.3f;

    [PunRPC]
    void Pun_Set_Hp(int hp)
    {
        _hp = hp;

        for (int i = 0; i < _trHp.Length; ++i)
        {
            _trHp[i].SetActive(i < _hp);
        }

        StartCoroutine(NoHit());

        if (_hp <= 0)
        {
            if(photonView.IsMine)
            {
                // 사망띠
                SoundManager._soundManager.Play_Effect("Player_Dead");

                UIManager._uiManager._deadUI.SetActive(true);

                enabled = false;

                StartCoroutine(Revive());
            }

            EffectManager._effectManager.Show_Particle("CFX2_EnemyDeathSkull", transform.position);

        }
    }

    IEnumerator Revive()
    {
        yield return new WaitForSeconds(3f);

        enabled = true;
        Initialize();
        UIManager._uiManager._deadUI.SetActive(false);
    }

    // Value
    public int _hp;
    public BoxCollider2D _collider;

    void Initialize()
    {
        photonView.RPC("Pun_Set_Hp", RpcTarget.All, 3);
    }

    IEnumerator NoHit()
    {
        _collider.enabled = false;

        float fTime = 3f;
        float fTwinkle = 0.15f;

        while (true)
        {
            fTime -= Time.deltaTime;
            if (fTime < 0f)
                break;

            fTwinkle -= Time.deltaTime;
            if(fTwinkle < 0f)
            {
                fTwinkle = 0.15f;
                _playerImage.color = new Color(1f, 1f, 1f,
                    _playerImage.color.a == 1.0f ? 0.5f : 1.0f);
            }
            
            yield return null;
        }

        _playerImage.color = new Color(1f, 1f, 1f, 1.0f);

        _collider.enabled = true;
    }

    void Awake()
    {
        if (_photonView.IsMine)
            _player = this;

        //_textMesh.text = "Player " + PhotonNetwork.CurrentRoom.PlayerCount;
        if (_photonView.IsMine)
        {
            _playerImage.sprite = _sprites[0];
            _textMesh.text = "1P";
        }
        else
        {
            _playerImage.sprite = _sprites[1];
            _textMesh.text = "2P";
        }
            
        Initialize();
    }

    void Start()
    {
        if (_photonView.IsMine)
        {
            PlayerCamera._playerCamera._myPlayer = transform;
            _cam = PlayerCamera._playerCamera._cam;
        }

        //EffectManager._effectManager.Show_Particle("CFX3_ResurrectionLight_Circle", transform.position, transform);

        GameManager._gameManger._listPlayer.Add(this);
    }

    private void OnDestroy()
    {
        GameManager._gameManger._listPlayer.Remove(this);
    }

    float _moveSpeed = 3f;

    private void Update()
    {
        if (!_photonView.IsMine)
            return;

        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(0f, _moveSpeed * Time.deltaTime, 0f);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(_moveSpeed  * - Time.deltaTime, 0f, 0f);

            _playerImage.flipX = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(0f, _moveSpeed * -Time.deltaTime, 0f);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(_moveSpeed* Time.deltaTime, 0f, 0f);

            _playerImage.flipX = false;
        }

        transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, _fRange[0], _fRange[1]),
                Mathf.Clamp(transform.position.y, _fRange[2], _fRange[3]),
                transform.position.z);
        
        _vDir = _cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;

        _fTime -= Time.deltaTime;
        if (Input.GetMouseButton(0))
        {
            if (_fTime < 0f)
            {
                _fTime = 0.3f;
                Shoot_Bullet();
            }
        }

        if(Input.GetKeyDown(KeyCode.P))
        {
            
        }

        // 각도
        _curAngle = Mathf.Atan2(_vDir.y, _vDir.x) * Mathf.Rad2Deg - 90f;
        _range.rotation = Quaternion.AngleAxis(_curAngle, Vector3.forward);

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
        
    }
    

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data

            stream.SendNext(_curAngle); // 총알 방향 UI 
        }
        else
        {
            // Network player, receive data

            _curAngle = (float)stream.ReceiveNext();

            _range.rotation = Quaternion.AngleAxis(_curAngle, Vector3.forward);
            
        }
    }

    public void Shoot_Bullet()
    {
        Bullet bullet = PhotonNetwork.Instantiate("Prefab/Bullet", _fire.position, Quaternion.identity).GetComponent<Bullet>();

        bullet.SetInfo(_fire.position, _vDir);
        //bullet._speed = 10f;
        bullet._isMine = true;
        
        SoundManager._soundManager.Play_Shot("Shoot");
    }

    public void Hit() // by bullet
    {
        photonView.RPC("Pun_Hit", RpcTarget.All);
    }

    [PunRPC]
    public void Pun_Hit()
    {
        photonView.RPC("Pun_Set_Hp", RpcTarget.All, _hp - 1);
        
        EffectManager._effectManager.Show_Particle("CFX2_BrokenHeart", transform.position, transform);
        SoundManager._soundManager.Play_Effect("Player_Hit");
    }

}