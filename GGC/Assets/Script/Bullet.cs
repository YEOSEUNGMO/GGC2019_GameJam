using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPunCallbacks, IPunObservable
{
    Vector3 _startPos;
    public Vector2 _vDir;
    public float _speed = 0f;

    public float _angle;
    bool _isLive = true;

    public bool _isMine;
    public bool IsBossBullet = false;
    public int CollisionCount=3;
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_isLive)
            return;

        if (collision.collider.tag == "Player")
        {
            _isLive = false;

            collision.collider.GetComponent<GGC_Player>().Hit();

            if (_isMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }            
        }
        else if (collision.collider.tag == "Spider")
        {
            if (!IsBossBullet)
            {
                _isLive = false;

                collision.collider.GetComponent<Spider>().Hit();

                if (_isMine)
                {
                    PhotonNetwork.Destroy(gameObject);
                }
            }
               
        }
        else if (collision.collider.tag == "Boss")
        {
            _isLive = false;

            collision.collider.GetComponent<Boss>().Hit();

            if (_isMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
        else if (collision.collider.tag == "Cocoon")
        {
            if (!IsBossBullet)
            {
                _isLive = false;

                collision.collider.GetComponent<Cocoon>().Hit();

                if (_isMine)
                {
                    PhotonNetwork.Destroy(gameObject);
                }
            }
        }
        else if (collision.collider.tag == "Wall")
        {
            Vector3 collisionPos = collision.contacts[0].point;
            Vector3 incomingVec = collisionPos - _startPos;
            Vector3 normalVec = collision.contacts[0].normal;
            //_vDir = Vector3.Reflect(incomingVec, normalVec);
            _vDir = Vector3.Reflect(_vDir, normalVec);
            _vDir.Normalize();
            float originAngle = Mathf.Atan2(_vDir.y, _vDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(originAngle - 90, Vector3.forward);

            _startPos = collisionPos;

            CollisionCount--;
            if(CollisionCount==0 && _isMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }


        }
    }

    void Start()
    {
        //_startPos = gameObject.transform.position;
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
