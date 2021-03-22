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
    public GameManager _gameManager;
    public ObjectManager _objectManager;

    SpriteRenderer _sprRen;
    Animator _anim;

    public int _patternIndex;
    public int _curPatternCount;
    public int[] _maxPatternCount;

    void Awake()
    {
        _sprRen = GetComponent<SpriteRenderer>();

        if (_enemyName == "B")
            _anim = GetComponent<Animator>();
    }

    void OnEnable()
    {
        switch(_enemyName)
        {
            case "B":
                _health = 3000;
                Invoke("Stop", 2f);
                break;
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
    void Stop()
    {
        if (!gameObject.activeSelf)
            return;

        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.zero;

        Invoke("Think", 2f);
    }

    void Think()
    {
        _patternIndex = _patternIndex == 3 ? 0 : _patternIndex + 1;
        _curPatternCount = 0;

        //_patternIndex = 3; //Boss PatternTest
        switch(_patternIndex)
        {
            case 0:
                FireFoward();
                break;
            case 1:
                FireShot();
                break;
            case 2:
                FireArc();
                break;
            case 3:
                FireAround();
                break;
        }
    }

    void FireFoward()
    {
        //Debug.Log("앞으로 4발 발사.");
        GameObject bulletR = _objectManager.MakeObj("BulletBossA");
        bulletR.transform.position = transform.position + Vector3.right * 0.75f;
        GameObject bulletRR = _objectManager.MakeObj("BulletBossA");
        bulletRR.transform.position = transform.position + Vector3.right * 0.9f;
        GameObject bulletL = _objectManager.MakeObj("BulletBossA");
        bulletL.transform.position = transform.position + Vector3.left * 0.75f;
        GameObject bulletLL = _objectManager.MakeObj("BulletBossA");
        bulletLL.transform.position = transform.position + Vector3.left * 0.9f;

        Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();

        rigidR.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
        rigidRR.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
        rigidL.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
        rigidLL.AddForce(Vector2.down * 8, ForceMode2D.Impulse);

        //#.Pattern Counting
        _curPatternCount++;

        if(_curPatternCount < _maxPatternCount[_patternIndex])
            Invoke("FireFoward", 2f);
        else
        Invoke("Think", 3f);
    }

    void FireShot()
    {
        //Debug.Log("플레이어 방향으로 샷건.");
        for(int index = 0; index < 5; index ++)
        {
            GameObject bullet = _objectManager.MakeObj("BulletBossB");
            bullet.transform.position = transform.position;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector2 dirVec = _player.transform.position - transform.position;
            Vector2 ranVec = new Vector2(Random.Range(-1f, 1f), Random.Range(0f, 2f));
            dirVec += ranVec;
            rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
        }

        //#.Pattern Counting
        _curPatternCount++;

        if (_curPatternCount < _maxPatternCount[_patternIndex])
            Invoke("FireShot", 3.5f);
        else
            Invoke("Think", 3f);
    }

    void FireArc()
    {
        //Debug.Log("부채모양으로 발사.");
        GameObject bullet = _objectManager.MakeObj("BulletEnemyA");
        bullet.transform.position = transform.position;
        bullet.transform.rotation = Quaternion.identity;

        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 10 * _curPatternCount/_maxPatternCount[_patternIndex]), -1);
        rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);

        _curPatternCount++;
        if (_curPatternCount < _maxPatternCount[_patternIndex])
            Invoke("FireArc", 0.15f);
        else
            Invoke("Think", 3f);
    }

    void FireAround()
    {
        //Debug.Log("원 형태로 전체 공격.");
        int roundNumA = 50;
        int roundNumB = 40;
        int roundNum = _curPatternCount %2==0? roundNumA : roundNumB;
        for (int index=0; index < roundNumA; index++)
        {
            GameObject bullet = _objectManager.MakeObj("BulletBossB");
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.identity;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 2 * index / roundNum),
                                         Mathf.Sin(Mathf.PI * 2 * index / roundNum));
            rigid.AddForce(dirVec.normalized * 2, ForceMode2D.Impulse);

            Vector3 rotVec = Vector3.forward * 360 * index / roundNum + Vector3.forward * 90;
            bullet.transform.Rotate(rotVec);
        }


        _curPatternCount++;
        if (_curPatternCount < _maxPatternCount[_patternIndex])
            Invoke("FireAround", 0.7f);
        else
            Invoke("Think", 3f);
    }
    void Update()
    {
        if (_enemyName == "B")
            return;

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
        if (_enemyName == "B")
        {
            _anim.SetTrigger("OnHit");
        }
        else
        {
            _sprRen.sprite = _sprites[1];
            Invoke("ReturnSprite", 0.1f);
        }

        if (_health <= 0)
        {
            Player playerLogic = _player.GetComponent<Player>();
            playerLogic._score += _enemyScore;

            //#.Random Ratio Item Drop
            int ran = _enemyName == "B" ? 0 : Random.Range(0, 10);

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
            _gameManager.CallExplosion(transform.position, _enemyName);
        }
    }

    void ReturnSprite()
    {
        _sprRen.sprite = _sprites[0];
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BorderBullet" && _enemyName != "B")
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
