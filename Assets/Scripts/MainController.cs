using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class MainController : MonoBehaviour
{
    [SerializeField]
    GameObject mainMenu = null;
    [SerializeField]
    GameObject stageMenu = null;
    [SerializeField]
    GameObject myRecordMenu = null;
    [SerializeField]
    GameObject[] stageButtons = null;
    [SerializeField]
    Text StageText;
    [SerializeField]
    Text InfModeText;

    int onClickStage;

    void Start()
    {
        if (PlayerData.instance.ChangeSceneFlag == 2)
        {
            StageMode();
        }
        else
        {
            mainMenu.SetActive(true);
            stageMenu.SetActive(false);
            myRecordMenu.SetActive(false);
        }
        GameLoad();
    }

    public void GameSave()
    {
        #if UNITY_EDITOR
            Debug.Log("저장");
            //UnityEditor.EditorApplication.isPlaying = false;
#else
            PlayerPrefs.SetInt("canAccessStage", gameManager._canAccessStage);
            PlayerPrefs.Save();
#endif
    }

    void GameLoad() //단순 UI표현
    {
        if (PlayerData.instance.ChangeSceneFlag == 2)
        {
            int canAccessStage = PlayerData.instance.canAccessStage;
            for (int i = 0; i < canAccessStage; i++)
                stageButtons[i].GetComponent<Image>().color = new Color(146 / 255f, 194 / 255f, 255 / 255f, 255 / 255f);
            stageButtons[canAccessStage-1].GetComponent<Image>().color = new Color(255, 255, 255);
            PlayerData.instance.ChangeSceneFlag--;
        }
        else
        {
            int canAccessStage = PlayerData.instance.canAccessStage;
            Debug.Log(canAccessStage);
            for (int i = 0; i < canAccessStage; i++)
                stageButtons[i].GetComponent<Image>().color = new Color(146 / 255f, 194 / 255f, 255 / 255f, 255 / 255f);
            stageButtons[canAccessStage-1].GetComponent<Image>().color = new Color(255, 255, 255);
        }       
    }

    public void OnClickStage(int i)
    {
        onClickStage = i;
        //SceneManager.LoadScene("StageMode");
        StartStage(onClickStage);
    }

    public void StartStage(int onClickStage)
    {
        if(onClickStage > PlayerData.instance.canAccessStage)
        {
            Debug.Log("이전 스테이지를 완료하세요.");
        }
        else //curStage로 게임 실행
        {
            PlayerData.instance.curStage = onClickStage;
            SceneManager.LoadScene("StageMode");
        }
    }

    public void StageMode()
    {
        mainMenu.SetActive(false);
        stageMenu.SetActive(true);
    }
    public void InfiniteMode()
    {
        SceneManager.LoadScene("InfiniteMode");
    }
    public void MyRecordButton()
    {
        myRecordSetting();
        mainMenu.SetActive(false);
        myRecordMenu.SetActive(true);
    }

    public void Exit()
    {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                GameSave();
                Application.Quit();
#endif
    }

    public void myRecordSetting()
    {
        StageText.GetComponent<Text>().text = (PlayerData.instance.canAccessStage-1).ToString();
        InfModeText.GetComponent<Text>().text = PlayerData.instance.infModeBestScore.ToString();
    }

    public void BackButton()
    {
        if (stageMenu.activeSelf == true) //stageMenu 활성화중일경우
        {
            mainMenu.SetActive(true);
            stageMenu.SetActive(false);
        }

        else if (myRecordMenu.activeSelf == true) //myRecordMenu 활성화중일경우
        {
            mainMenu.SetActive(true);
            myRecordMenu.SetActive(false);
        }
    }
}
