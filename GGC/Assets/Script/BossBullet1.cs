using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BossBullet1 : MonoBehaviourPunCallbacks, IPunObservable { 

    Vector3 _startPos;
    public Vector2 _vDir;
    public Vector2 _vDir2;
    public float _speed = 0f;

    public float _angle;
    bool _isLive = true;
    public bool BeDestroy = false;

    public bool _isMine;
    private float originAngle;
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (BeDestroy)
        {
            if (collision.collider.tag == "Player")
            {
                _isLive = false;

                if (_isMine)
                {
                    Vector3 collisionPos = collision.contacts[0].point;
                    Vector3 incomingVec = collisionPos - _startPos;
                    Vector3 normalVec = collision.contacts[0].normal;
                    _vDir2 = -_vDir;
                    _vDir = Vector3.Reflect(incomingVec, normalVec);
                    _vDir.Normalize();
                    originAngle = Mathf.Atan2(_vDir.y, _vDir.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.AngleAxis(originAngle - 90, Vector3.forward);

                    _startPos = collisionPos;

                    Shoot_Bullet();

                    PhotonNetwork.Destroy(gameObject);
                }
            }
            else if (collision.collider.tag == "Wall")
            {
                if (BeDestroy && _isMine)
                {
                    Vector3 collisionPos = collision.contacts[0].point;
                    Vector3 incomingVec = collisionPos - _startPos;
                    Vector3 normalVec = collision.contacts[0].normal;
                    _vDir2 = -_vDir;
                    _vDir = Vector3.Reflect(incomingVec, normalVec);
                    _vDir.Normalize();
                    originAngle = Mathf.Atan2(_vDir.y, _vDir.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.AngleAxis(originAngle - 90, Vector3.forward);

                    _startPos = collisionPos;

                    Shoot_Bullet();

                    PhotonNetwork.Destroy(gameObject);
                }


            }
        }
    }

    public void Shoot_Bullet()
    {
        BossBullet2 bullet = PhotonNetwork.Instantiate("Prefab/BossBullet2", _startPos, transform.rotation).GetComponent<BossBullet2>();
        bullet._speed = _speed * 1.5f;
        bullet.SetInfo(_startPos, _vDir);
        bullet._isMine = true;

        BossBullet2 bullet2 = PhotonNetwork.Instantiate("Prefab/BossBullet2", _startPos, Quaternion.AngleAxis(originAngle - 180, Vector3.forward)).GetComponent<BossBullet2>();
        bullet2._speed = _speed * 1.5f;
        bullet2.SetInfo(_startPos, _vDir2);
        bullet2._isMine = true;
    }

    void Awake()
    {
        StartCoroutine(Countdown());
    }
    public IEnumerator Countdown()
    {
        WaitForSeconds _sec = new WaitForSeconds(0.05f);
        yield return _sec;
        BeDestroy = true;        
    }
    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(
            transform.position.x + _vDir.x * _speed * Time.deltaTime,
            transform.position.y + _vDir.y * _speed * Time.deltaTime
            );
    }

    public void SetInfo(Vector3 vPos, Vector3 vDir)
    {
        transform.position = vPos;
        _startPos = vPos;
        _vDir = vDir;
        _vDir.Normalize();
        //_speed = GameManager.Instance._moveBulletSpeed;

        Change_Dir();
    }

    void Change_Dir()
    {
        _angle = Mathf.Atan2(_vDir.y, _vDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(_angle - 90, Vector3.forward);
    }

    public void Set_Dir(float angle)
    {
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        _vDir = q * _vDir;
        _vDir.Normalize();

        Change_Dir();
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
