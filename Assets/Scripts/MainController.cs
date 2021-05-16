using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    int onClickStage;

    void Awake()
    {
        
    }

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
            Debug.Log("저장 및 종료");
            UnityEditor.EditorApplication.isPlaying = false;
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
            stageButtons[canAccessStage].GetComponent<Image>().color = new Color(255, 255, 255);
            PlayerData.instance.ChangeSceneFlag--;
        }
        else
        {
            int canAccessStage = PlayerData.instance.canAccessStage;

            for (int i = 0; i < canAccessStage; i++)
                stageButtons[i].GetComponent<Image>().color = new Color(146 / 255f, 194 / 255f, 255 / 255f, 255 / 255f);
            stageButtons[canAccessStage].GetComponent<Image>().color = new Color(255, 255, 255);
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

    public void Exit()
    {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                GameSave();
                Application.Quit();
#endif
    }

    public void BackButton()
    {
        if (stageMenu.activeSelf == true) //stageMenu 활성화중일경우
        {
            mainMenu.SetActive(true);
            stageMenu.SetActive(false);
        }
    }
}
