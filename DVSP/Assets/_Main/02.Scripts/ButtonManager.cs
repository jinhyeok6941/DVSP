using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ButtonManager : MonoBehaviour
{

    public void OnClickPlayGame()
    {
        SceneManager.LoadScene("ChooseMap");   
    }

    public void OnClickTutorial()
    {
        SceneManager.LoadScene("Tutorial_yu");   
    }

    public void OnClickMap1()
    {
        SceneManager.LoadScene("07.FPV_PracticeScene_KSH 1");
    }
    public void OnClickMap2()
    {
        SceneManager.LoadScene("VRGameScene_YSM");
    }

}
