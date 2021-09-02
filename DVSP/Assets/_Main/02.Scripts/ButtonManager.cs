using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ButtonManager : MonoBehaviour
{

    public void OnClickGameStartButton()
    {
        SceneManager.LoadScene(1);   
    }

    public void OnClickPracticeButton()
    {
        SceneManager.LoadScene(2);   
    }

    public void OnClickTutorialButton()
    {
        SceneManager.LoadScene(3);   
    }

}
