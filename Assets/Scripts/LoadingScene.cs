using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadingScene : MonoBehaviour
{
    void Awake()
    {

    }
    void Start()
    {
        PlayerData.instance.curStage = 1;
        PlayerData.instance.infModeBestScore = 0;
        PlayerData.instance.ChangeSceneFlag = 1;

        if (PlayerPrefs.GetInt("curStage") != 1)
            PlayerData.instance.curStage = PlayerPrefs.GetInt("curStage");
    }

    void Update()
    {
        if (Input.anyKey)
        {
            SceneManager.LoadScene("Main");
        }
    }
}
