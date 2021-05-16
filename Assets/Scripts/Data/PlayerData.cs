using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    //싱글톤
    //instance라는 변수를 static으로 선언을 하여
    //다른 오브젝트 안의 스크립트에서도 instance를
    //불러올수 있게함.
    public static PlayerData instance = null;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (instance != this)
                Destroy(this.gameObject);
        }
    }

    //게임 내에서 씬이동시 유지하고 픈 값
    
    public int curStage;
    public int canAccessStage;
    public int infModeBestScore;
    public int ChangeSceneFlag;
    //ChangeSceneFlag == 1 : MainScene
    //ChangeSceneFlag == 2 : StageEnd and Go to MainScene(StageMenu)
}