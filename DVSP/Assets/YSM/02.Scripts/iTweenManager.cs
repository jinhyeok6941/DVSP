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
        endtuto = false;


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


    public void StartTuto(GameStat stat)
    {
        switch (stat)
        {
            case GameStat.tuto:
            start.SetActive(false);
            quit.SetActive(false);

            iTween.MoveBy(tuto,
            iTween.Hash("x", -50,
            "time", 1,
            "easetype", iTween.EaseType.easeInBounce
            ));
                break;

            case GameStat.start:
                tuto.SetActive(false);
                quit.SetActive(false);
                break;

            case GameStat.quit:
                start.SetActive(false);
                tuto.SetActive(false);
                break;
        }

        Image img = quit.GetComponent<Image>();
        Text txt = quit.GetComponentInChildren<Text>();
        img.color = new Color(255, 255, 255, 0);

        iTween.MoveBy(quit,
            iTween.Hash("x", -50,
            "time", 1,
            "easetype", iTween.EaseType.easeInBounce
            ));

        iTween.MoveBy(tuto,
           iTween.Hash("x", 50,
           "time", 1,
           "easetype", iTween.EaseType.easeInBounce
           ));

    }
    

    IEnumerator fadeOut(GameObject go)
    {
        yield return new WaitForSeconds(2);
        go.SetActive(false);

    }

}
