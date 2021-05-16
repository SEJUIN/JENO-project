using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
    public int _stage;
    public int _canAccessStage;
    public Animator _stageAnim;
    public Animator _clearAnim;
    public Animator _fadeAnim;
    public Transform _playerPos;

    public string[] _enemyObjs;
    public Transform[] _spawnPoints;

    public float _nextSpawnDelay;
    public float _curSpawnDelay;

    public GameObject _player;
    public Text _scoreText;
    public Image[] _lifeImage;
    public Image[] _boomImage;
    public GameObject _gameOverSet;
    public ObjectManager _objectManager;

    public List<Spawn> spawnList;
    public int spawnIndex;
    public bool spawnEnd;

    void Awake()
    {
        spawnList = new List<Spawn>();
        _enemyObjs = new string[] { "EnemyS", "EnemyM", "EnemyL", "EnemyB" };
        StageStart();
    }

    void Start()
    {
        
    }

    public void StageStart()
    {
        _stage = PlayerData.curStage;
        //#.Stage UI Load
        _stageAnim.SetTrigger("On");
        _stageAnim.GetComponent<Text>().text = "Stage "+ _stage + "\nStart";
        _clearAnim.GetComponent<Text>().text = "Stage " + _stage + "\nClear!";

        //#.Enemy Spawn File Read
        ReadSpawnFile();

        //#.Fade In
        _fadeAnim.SetTrigger("In");
    }

    public void StageEnd()
    {
        //#.Clear UI Load
        _clearAnim.SetTrigger("On");

        //#.Fade Out
        _fadeAnim.SetTrigger("Out");

        //#.Player RePosition
        _player.transform.position = _playerPos.position;

        //#.Stage Increament
        PlayerData.instance.curStage++;
        //_stage++;
        PlayerData.instance.ChangeSceneFlag++;
        Debug.Log(PlayerData.instance.ChangeSceneFlag);
        Invoke("LoadSceneMain", 5);
    }

    void LoadSceneMain()
    {
        SceneManager.LoadScene("Main");
    }

    void ReadSpawnFile()
    {
        //#1.변수 초기화
        spawnList.Clear();
        spawnIndex = 0;
        spawnEnd = false;

        //#2.리스폰 파일 읽기
        TextAsset textFile = Resources.Load("Stage " + _stage) as TextAsset;
        StringReader stringReader = new StringReader(textFile.text);

        while (stringReader != null)
        {
            string line = stringReader.ReadLine();
            Debug.Log(line);

            if (line == null)
                break;

            //#.리스폰 데이터 생성
            Spawn spawnData = new Spawn();
            spawnData.delay = float.Parse(line.Split(',')[0]);
            spawnData.type = line.Split(',')[1];
            spawnData.point = int.Parse(line.Split(',')[2]);
            spawnList.Add(spawnData);
        }

        //#.텍스트 파일 닫기
        stringReader.Close();

        //#.첫번째 스폰 딜레이 적용
        _nextSpawnDelay = spawnList[0].delay;
    }

    void Update()
    {
        _curSpawnDelay += Time.deltaTime;

        if(_curSpawnDelay > _nextSpawnDelay && !spawnEnd)
        {
            SpawnEnemy();
            _curSpawnDelay = 0;
        }

        //#.UI Score Update
        Player playerLogic = _player.GetComponent<Player>();
        _scoreText.text = string.Format("{0:n0}", playerLogic._score);
    }

    void SpawnEnemy()
    {
        int enemyIndex = 0;
        switch(spawnList[spawnIndex].type)
        {
            case "S":
                enemyIndex = 0;
                break;
            case "M":
                enemyIndex = 1;
                break;
            case "L":
                enemyIndex = 2;
                break;
            case "B":
                enemyIndex = 3;
                break;
        }
        int enemyPoint = spawnList[spawnIndex].point;
        GameObject enemy = _objectManager.MakeObj(_enemyObjs[enemyIndex]);
        enemy.transform.position = _spawnPoints[enemyPoint].position;

        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        Enemy enemyLogic = enemy.GetComponent<Enemy>();
        enemyLogic._player = _player;
        enemyLogic._gameManager = this;
        enemyLogic._objectManager = _objectManager;

        if (enemyPoint == 5 || enemyPoint == 6) //#.Right Spawn
        {
            enemy.transform.Rotate(Vector3.back * 90);
            rigid.velocity = new Vector2(enemyLogic._speed * (-1), -1);
        }
        else if (enemyPoint == 7 || enemyPoint == 8) //#.Left Spawn
        {
            enemy.transform.Rotate(Vector3.forward * 90);
            rigid.velocity = new Vector2(enemyLogic._speed, -1);
        }
        else //#.Front Spawn
            rigid.velocity = new Vector2(0, enemyLogic._speed*(-1));

        //#.리스폰 인덱스 증가
        spawnIndex++;
        if(spawnIndex == spawnList.Count)
        {
            spawnEnd = true;
            return;
        }

        //#.다음 리스폰 딜레이 갱신
        _nextSpawnDelay = spawnList[spawnIndex].delay;
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

    public void CallExplosion(Vector3 pos, string type)
    {
        GameObject explosion = _objectManager.MakeObj("Explosion");
        Explosion explosionLogic = explosion.GetComponent<Explosion>();

        explosion.transform.position = pos;
        explosionLogic.StartExplosion(type);
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
