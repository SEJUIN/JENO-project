using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool _isTouchTop;
    public bool _isTouchBottom;
    public bool _isTouchRight;
    public bool _isTouchLeft;

    public int _life;
    public int _score;
    public float _speed;
    public int _maxPower;
    public int _power;
    public int _maxBoom;
    public int _boom;
    public float _maxShotDelay;
    public float _curShotDelay;

    public GameObject _bulletObjA;
    public GameObject _bulletObjB;
    public GameObject _boomEffect;
    
    public GameManager _gameManager;
    public ObjectManager _objectManager;
    public bool _isHit;
    public bool _isBoomTime;

    Animator _anim;

    void Awake()
    {
        _anim = GetComponent<Animator>();
    }
    void Update()
    {
        Move();
        Fire();
        Boom();
        Reload();
    }

    void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        if ((_isTouchRight && h == 1) || (_isTouchLeft && h == -1))
            h = 0;

        float v = Input.GetAxisRaw("Vertical");
        if ((_isTouchTop && v == 1) || (_isTouchBottom && v == -1))
            v = 0;

        Vector3 curPos = transform.position;
        Vector3 nextPos = new Vector3(h, v, 0) * _speed * Time.deltaTime;

        transform.position = curPos + nextPos;

        if (Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal"))
        {
            _anim.SetInteger("Input", (int)h);
        }
    }

    void Fire()
    {
        if (!Input.GetButton("Fire1"))
            return;

        if (_curShotDelay < _maxShotDelay)
            return;

        switch (_power)
        {
            case 1:
                GameObject bullet = _objectManager.MakeObj("BulletPlayerA");
                bullet.transform.position = transform.position;

                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 2:
                GameObject bulletR = _objectManager.MakeObj("BulletPlayerA");
                bulletR.transform.position = transform.position + Vector3.right * 0.1f;
                
                GameObject bulletL = _objectManager.MakeObj("BulletPlayerA");
                bulletL.transform.position = transform.position + Vector3.left * 0.1f;
                
                Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
                rigidR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 3:
                GameObject bulletRR = _objectManager.MakeObj("BulletPlayerA");
                bulletRR.transform.position = transform.position + Vector3.right * 0.35f;

                GameObject bulletCC = _objectManager.MakeObj("BulletPlayerB");
                bulletCC.transform.position = transform.position;

                GameObject bulletLL = _objectManager.MakeObj("BulletPlayerA");
                bulletLL.transform.position = transform.position + Vector3.left * 0.35f;
                
                Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidCC = bulletCC.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();
                rigidRR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidCC.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidLL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
        }
        
        _curShotDelay = 0;
    }

    void Reload()
    {
        _curShotDelay += Time.deltaTime;
    }

    void Boom()
    {
        if (!Input.GetButton("Fire2"))
            return;

        if (_isBoomTime)
            return;

        if (_boom == 0)
            return;

        _boom--;
        _isBoomTime = true;
        _gameManager.UpdateBoomIcon(_boom);

        //#1.Effect visible
        _boomEffect.SetActive(true);
        Invoke("OffBoomEffect", 4f);

        //#2.Remove Enemy
        GameObject[] enemiesL = _objectManager.GetPool("EnemyL");
        GameObject[] enemiesM = _objectManager.GetPool("EnemyM");
        GameObject[] enemiesS = _objectManager.GetPool("EnemyS");

        for (int index = 0; index < enemiesL.Length; index++)
        {
            if (enemiesL[index].activeSelf)
            {
                Enemy enemyLogic = enemiesL[index].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }
        for (int index = 0; index < enemiesM.Length; index++)
        {
            if (enemiesM[index].activeSelf)
            {
                Enemy enemyLogic = enemiesM[index].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }
        for (int index = 0; index < enemiesS.Length; index++)
        {
            if (enemiesS[index].activeSelf)
            {
                Enemy enemyLogic = enemiesS[index].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }

        //#3.Remove Enemy Bullet
        GameObject[] bulletsA = _objectManager.GetPool("BulletEnemyA");
        GameObject[] bulletsB = _objectManager.GetPool("BulletEnemyB");
        for (int index = 0; index < bulletsA.Length; index++)
        {
            if(bulletsA[index].activeSelf)
                bulletsA[index].SetActive(false);
        }
        for (int index = 0; index < bulletsB.Length; index++)
        {
            if (bulletsB[index].activeSelf)
                bulletsB[index].SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Border")
        {
            switch(collision.gameObject.name)
            {
                case "Top":
                    _isTouchTop = true;
                    break;
                case "Bottom":
                    _isTouchBottom = true;
                    break;
                case "Right":
                    _isTouchRight = true;
                    break;
                case "Left":
                    _isTouchLeft = true;
                    break;
            }
        }

        else if(collision.gameObject.tag == "Enemy"|| collision.gameObject.tag =="EnemyBullet") //플레이어가 적이나, 적총알에 맞았을 경우
        {
            if (_isHit)
                return;

            _isHit = true;
            _life--;
            _gameManager.UpdateLifeIcon(_life);

            if (_life == 0)
            {
                _gameManager.GameOver();
            }
            else
            {
                _gameManager.RespawnPlayer();
            }
            gameObject.SetActive(false);
            collision.gameObject.SetActive(false);
            //Destroy(collision.gameObject);
        }

        else if (collision.gameObject.tag == "Item")
        {
            Item item = collision.gameObject.GetComponent<Item>();
            switch (item._type)
            {
                case "Coin":
                    _score += 1000;
                    break;
                case "Power":
                    if (_power == _maxPower)
                        _score += 500;
                    else
                        _power++;
                    break;
                case "Boom":
                    if (_boom == _maxBoom)
                        _score += 500;
                    else
                    {
                        _boom++;
                        _gameManager.UpdateBoomIcon(_boom);
                    }
                    break;
            }
            collision.gameObject.SetActive(false);
            //Destroy(collision.gameObject);
        }
    }
    
    void OffBoomEffect()
    {
        _boomEffect.SetActive(false);
        _isBoomTime = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        switch (collision.gameObject.name)
        {
            case "Top":
                _isTouchTop = false;
                break;
            case "Bottom":
                _isTouchBottom = false;
                break;
            case "Right":
                _isTouchRight = false;
                break;
            case "Left":
                _isTouchLeft = false;
                break;
        }
    }
}
