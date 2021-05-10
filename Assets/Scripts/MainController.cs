using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{
    [SerializeField]
    GameObject mainMenu;
    [SerializeField]
    GameObject stageMenu;

    void Awake()
    {
        mainMenu.SetActive(true);
        stageMenu.SetActive(false);
    }

    public void StageMode()
    {
        mainMenu.SetActive(false);
        stageMenu.SetActive(true);
        //SceneManager.LoadScene("StageMode");
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
                Application.Quit();
        #endif
    }
}
