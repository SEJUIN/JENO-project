using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadingScene : MonoBehaviour
{
    void Start()
    {
        PlayerData.instance.curStage = 1;
        PlayerData.instance.canAccessStage = 1;
        PlayerData.instance.infModeBestScore = 0;
        PlayerData.instance.ChangeSceneFlag = 1;
        
        if (PlayerPrefs.GetInt("canAccessStage") != 1)
            PlayerData.instance.canAccessStage = PlayerPrefs.GetInt("canAccessStage");
    }

    void Update()
    {
        
        if (Input.anyKey)
        {
            SceneManager.LoadScene("Main");
        }
    }
}
