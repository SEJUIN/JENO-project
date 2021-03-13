using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string[] _enemyObjs;
    public Transform[] _spawnPoints;

    public float _maxSpawnDelay;
    public float _curSpawnDelay;

    public GameObject _player;
    public Text _scoreText;
    public Image[] _lifeImage;
    public Image[] _boomImage;
    public GameObject _gameOverSet;
    public ObjectManager _objectManager;

    private void Awake()
    {
        _enemyObjs = new string[] { "EnemyS", "EnemyM", "EnemyL" };
    }

    void Update()
    {
        _curSpawnDelay += Time.deltaTime;

        if(_curSpawnDelay > _maxSpawnDelay)
        {
            SpawnEnemy();
            _maxSpawnDelay = Random.Range(0.5f, 3f);
            _curSpawnDelay = 0;
        }

        //#.UI Score Update
        Player playerLogic = _player.GetComponent<Player>();
        _scoreText.text = string.Format("{0:n0}", playerLogic._score);
    }

    void SpawnEnemy()
    {
        int ranEnemy = Random.Range(0, 3); //Enemy Kinds
        int ranPoint = Random.Range(0, 9); //Enemy SpawnPoint
        GameObject enemy = _objectManager.MakeObj(_enemyObjs[ranEnemy]);
        enemy.transform.position = _spawnPoints[ranPoint].position;

        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        Enemy enemyLogic = enemy.GetComponent<Enemy>();
        enemyLogic._player = _player;
        enemyLogic._objectManager = _objectManager;

        if (ranPoint == 5 || ranPoint == 6) //#.Right Spawn
        {
            enemy.transform.Rotate(Vector3.back * 90);
            rigid.velocity = new Vector2(enemyLogic._speed * (-1), -1);
        }
        else if (ranPoint == 7 || ranPoint == 8) //#.Left Spawn
        {
            enemy.transform.Rotate(Vector3.forward * 90);
            rigid.velocity = new Vector2(enemyLogic._speed, -1);
        }
        else //#.Front Spawn
            rigid.velocity = new Vector2(0, enemyLogic._speed*(-1));
    }
    public void UpdateLifeIcon(int life)
    {
        //#.UI Life Init Disable
        for (int index = 0; index < 3; index++)
        {
            _lifeImage[index].color = new Color(1, 1, 1, 0);
        }

        //#.UI Life Active
        for (int index =0; index<life; index++)
        {
            _lifeImage[index].color = new Color(1, 1, 1, 1);
        }
    }

    public void UpdateBoomIcon(int boom)
    {
        //#.UI Boom Init Disable
        for (int index = 0; index < 3; index++)
        {
            _boomImage[index].color = new Color(1, 1, 1, 0);
        }

        //#.UI Boom Active
        for (int index = 0; index < boom; index++)
        {
            _boomImage[index].color = new Color(1, 1, 1, 1);
        }
    }

    public void RespawnPlayer()
    {
        Invoke("RespawnPlayerExe", 2f);
    }

    void RespawnPlayerExe()
    {
        _player.transform.position = Vector3.down * 3.5f;
        _player.SetActive(true);

        Player playerLogic = _player.GetComponent<Player>();
        playerLogic._isHit = false;
    }

    public void GameOver()
    {
        _gameOverSet.SetActive(true);
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    }
}
