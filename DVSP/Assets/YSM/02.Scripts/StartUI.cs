using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartUI : MonoBehaviour
{
    LineRenderer lr;
    public Transform lHand;
    public Transform rHand;
    public Transform dot;
    Image img;

    void Start()
    {
        lr = gameObject.GetComponentInChildren<LineRenderer>();
    }

    void Update()
    {
        //pointer();
        DrawGuideLine();
    }

    void DrawGuideLine()
    {
        //1. 오른손 위치, 오른손 앞방향에서 발사하는 Ray를 만든다.
        Ray ray = new Ray(rHand.transform.position, rHand.transform.forward);
        //2. 부딪힌 곳이 있다면
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            //3. 부딪힌 지점까지 Line 을 그린다
            lr.SetPosition(0, rHand.transform.position);
            lr.SetPosition(1, hit.point);
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                //그위치에 빨간점 생성
                //dot.gameObject.SetActive(true);
                //dot.position = hit.point;
                img = hit.transform.GetComponent<Image>();
                img.color = Color.green;

                if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
                {
                    //btn 스크립트 가져온다
                    Button btn = hit.transform.GetComponent<Button>();
                    if (btn != null)
                    {
                        btn.onClick.Invoke();
                    }
                }

            }
        }
        else
        {
            img.color = Color.white;
            //4. 부딪힌 지점이 없으면 오른손위치에서 오른손 앞방향으로 몇미터까지 그려라
            lr.SetPosition(0, rHand.transform.position);
            lr.SetPosition(1, rHand.transform.position + rHand.transform.forward * 3);
        }
    }
    void pointer()
    {
        //오른손 위치에서 오른손 앞방향
        Ray ray = new Ray(rHand.transform.position, rHand.forward);
        //부딪혔다면
        RaycastHit hit;

        //int layer = 1 << LayerMask.NameToLayer("UI");
        //if(Physics.Raycast(ray, out hit,100))
        if (Physics.Raycast(ray, out hit))
        {
            //만약 부딪힌놈의 layer가 UI라면
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                //그위치에 빨간점 생성
                dot.gameObject.SetActive(true);
                dot.position = hit.point;
            }
            else
            {
                dot.gameObject.SetActive(false);
            }
        }
        else
        {
            dot.gameObject.SetActive(false);
        }

        if (dot.gameObject.activeSelf == true)
        {
            if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
            {
                //btn 스크립트 가져온다
                Button btn = hit.transform.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.Invoke();
                }
            }
        }
    }

    public void OnClickTuto() 
    {
        Debug.Log("tuto");
        //SceneManager.LoadScene("");
    }
    public void OnClickStart()
    {
        Debug.Log("start");
        //SceneManager.LoadScene("");
    }
    public void OnClickQuit()
    {
        Debug.Log("quit");
        Application.Quit();
    }

}
