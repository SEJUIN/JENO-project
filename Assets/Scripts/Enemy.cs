using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string _enemyName;
    public int _enemyScore;
    public float _speed;
    public int _health;
    public Sprite[] _sprites;

    public float _maxShotDelay;
    public float _curShotDelay;

    public GameObject _bulletObjA;
    public GameObject _bulletObjB;
    public GameObject _itemCoin;
    public GameObject _itemPower;
    public GameObject _itemBoom;
    public GameObject _player;
    public ObjectManager _objectManager;

    SpriteRenderer _sprRen;

    void Awake()
    {
        _sprRen = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        switch(_enemyName)
        {
            case "L":
                _health = 40;
                break;
            case "M":
                _health = 10;
                break;
            case "S":
                _health = 3;
                break;
        }
    }

    void Update()
    {
        Fire();
        Reload();
    }

    void Fire()
    {
        if (_curShotDelay < _maxShotDelay)
            return;

        if(_enemyName == "S")
        {
            GameObject bullet = _objectManager.MakeObj("BulletEnemyA");
            bullet.transform.position = transform.position;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector3 dirVec = _player.transform.position - transform.position;
            rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
        }
        else if(_enemyName == "L")
        {
            GameObject bulletR = _objectManager.MakeObj("BulletEnemyB");
            bulletR.transform.position = transform.position + Vector3.right * 0.3f;

            GameObject bulletL = _objectManager.MakeObj("BulletEnemyB");
            bulletL.transform.position = transform.position + Vector3.left * 0.3f;

            Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
            Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();

            Vector3 dirVecR = _player.transform.position - (transform.position + Vector3.right * 0.3f);
            Vector3 dirVecL = _player.transform.position - (transform.position + Vector3.left * 0.3f);

            rigidR.AddForce(dirVecR.normalized * 4, ForceMode2D.Impulse);
            rigidL.AddForce(dirVecL.normalized * 4, ForceMode2D.Impulse);
        }

        _curShotDelay = 0;
    }

    void Reload()
    {
        _curShotDelay += Time.deltaTime;
    }

    public void OnHit(int dmg)
    {
        if (_health <= 0)
            return;

        _health -= dmg;
        _sprRen.sprite = _sprites[1];
        Invoke("ReturnSprite", 0.1f);

        if (_health <= 0)
        {
            Player playerLogic = _player.GetComponent<Player>();
            playerLogic._score += _enemyScore;

            //#.Random Ratio Item Drop
            int ran = Random.Range(0, 10);
            if(ran < 3) //Not Item 30%
            {
                Debug.Log("Not Item");
            }
            else if (ran < 6)
            {
                //Coin 30%
                GameObject itemCoin = _objectManager.MakeObj("ItemCoin");
                itemCoin.transform.position = transform.position;
            }
            else if (ran < 8)
            {
                //Power
                GameObject itemPower = _objectManager.MakeObj("ItemPower");
                itemPower.transform.position = transform.position;
            }
            else if (ran < 10)
            {
                //Boom
                GameObject itemBoom = _objectManager.MakeObj("ItemBoom");
                itemBoom.transform.position = transform.position;
            }

            gameObject.SetActive(false);
            transform.rotation = Quaternion.identity;
        }
    }

    void ReturnSprite()
    {
        _sprRen.sprite = _sprites[0];
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BorderBullet")
        {
            gameObject.SetActive(false);
            transform.rotation = Quaternion.identity;
        }
            
        else if (collision.gameObject.tag == "PlayerBullet")
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            OnHit(bullet._dmg);

            collision.gameObject.SetActive(false);
        }
    }
}
