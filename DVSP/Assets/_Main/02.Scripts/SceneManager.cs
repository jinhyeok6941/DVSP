using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public void OnClickPlayGame()
    {
        SceneManager.LoadScene("");


    }

    public void OnClickTutorial()
    {



    }


    public void OnClickPractice()
    {



    }


    public void OnClickExit()
    {
        Application.Quit();
    }

    internal static void LoadScene(string noname)
    {
        throw new NotImplementedException();
    }
    internal static void LoadScene(int v)
    {
        throw new NotImplementedException();
    }
    internal static object GetActiveScene()
    {
        throw new NotImplementedException();
    }


}