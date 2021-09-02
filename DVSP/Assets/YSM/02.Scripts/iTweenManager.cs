using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class iTweenManager : MonoBehaviour
{
    public GameObject tuto;
    public GameObject start;
    public GameObject quit;

    public enum GameStat
    {
        tuto,
        start,
        quit,

    }
    public GameStat status;

    bool endtuto;

    void Start()
    {
        endtuto = true;


        if (!endtuto)
        {
            tuto.transform.position = new Vector3(-50, 0, 150);
            start.SetActive(false);
            quit.transform.position = new Vector3(50, 0, 150);
        }
        else
        {
            tuto.transform.position = new Vector3(-100, 0, 150);
            start.transform.position = new Vector3(0, 0, 150);
            quit.transform.position = new Vector3(100, 0, 150);
        }
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartTuto(GameStat.tuto);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartTuto(GameStat.start);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartTuto(GameStat.quit);
        }
    }
    /*
     hp
     item random
     
     
     
     */

    public void StartTuto(GameStat stat)
    {
        //더블클릭금지추가예정
        switch (stat)
        {
            case GameStat.tuto:
                start.SetActive(false);
                quit.SetActive(false);

                if (!endtuto)
                {
                    iTween.MoveBy(tuto,
                    iTween.Hash("x", 50,
                    "time", 1,
                    "easetype", iTween.EaseType.easeInBounce
                    ));
                }
                else
                {
                    iTween.MoveBy(tuto,
                    iTween.Hash("x", 50,
                    "time", 1,
                    "easetype", iTween.EaseType.easeInBounce
                    ));
                }


                break;

            case GameStat.start:
                tuto.SetActive(false);
                quit.SetActive(false);
                break;

            case GameStat.quit:
                start.SetActive(false);
                tuto.SetActive(false);

            iTween.MoveBy(quit,
            iTween.Hash("x", -50,
            "time", 1,
            "easetype", iTween.EaseType.easeInBounce
            ));
                break;
        }

       /* Image img = quit.GetComponent<Image>();
        Text txt = quit.GetComponentInChildren<Text>();
        img.color = new Color(255, 255, 255, 0);*/

    }

    IEnumerator fadeOut(GameObject go)
    {
        yield return new WaitForSeconds(2);
        go.SetActive(false);

    }
    public void OnClickTuto(GameStat stat)
    {
        stat = GameStat.tuto;
        StartTuto(stat);
        Debug.Log("tuto");
        //SceneManager.LoadScene("");
    }
    public void OnClickStart(GameStat stat)
    {
        stat = GameStat.start;
        StartTuto(stat);
        Debug.Log("start");
        //SceneManager.LoadScene("");
    }
    public void OnClickQuit(GameStat stat)
    {
        stat = GameStat.quit;
        StartTuto(stat);
        Debug.Log("quit");
        Application.Quit();
    }
}
