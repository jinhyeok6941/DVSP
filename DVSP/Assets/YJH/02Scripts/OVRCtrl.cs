using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OVRCtrl : MonoBehaviour
{
    Transform ViewBody;
    Rigidbody rb;

    public float speed = 10;
    int spCount = 0;

    public float rotSpeed = 10;
    public float pedRange = 0.3f;

    public Slider stSlider;
    public Slider flipSlider;

    float currST;
    public float compST = 3;
    float currFlip;
    public float compFlip = 3;


    bool hadeLess = false;
    bool collSenser;
    bool isFlying = false;

    bool hover;

    void Start()
    {
        ViewBody = transform.GetChild(0); // 가시적 효과를 줄 몸 설정 
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        StartStop_Speed();

        if (isFlying)//비행중일때만 조정된
        {
            Fliping(); //플립
            if (currFlip == 0) // 플립버튼을 누르지않으면 움직여라
            {
                L_MOVE();// 상하움직임 ,좌우회전
                R_MOVE(); // 전우좌움 움직임
            }
            if (OVRInput.Get(OVRInput.Button.Any))
            {
                Hovering(0.3f);//호버링 
            }
        }
        else
        {
            Debug.LogWarning("비행중이 아님니다. ");
            LedColor();//색바꾸기
        }
    }

    void StartStop_Speed()
    {
        //if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
        //{ 
        //    currST = 0;
        //}
        //else 
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
        {
            currST += Time.deltaTime;
            stSlider.value = currST / compST;
        }
        else if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
        {
            if (isFlying && currST <= 1)//비행중이고 1이내로 누르고 있었으면 속도향상
            {
                spCount++;
                speed *= (spCount % 3 + 1);
            }
            currST = 0;
        }

        if (currST >= compST)
        {
            if (!isFlying)
            {
                StartCoroutine(StartFly()); // 비행전에는 비행시작
            }
            else
            {
                StartCoroutine(StopFly()); // 비행중에는 정지
            }
            currST = 0;//한번 눌렀으면 일단 0 만들기 ! 
        }
    }
    IEnumerator StartFly()
    {
        float now_Y = transform.position.y;
        while (now_Y + 3.0f >= transform.position.y)
        {
            transform.position += Vector3.up * 0.01f;
            yield return new WaitForEndOfFrame();
        }
        isFlying = true;
    }//시작하는 동작 
    IEnumerator StopFly()
    {
        isFlying = false;
        ViewBody.localPosition = Vector3.zero;
        while (!collSenser)
        {
            transform.position -= Vector3.up * 0.01f;
            yield return new WaitForEndOfFrame();
        }
    }//멈추는 동작 

    void Fliping()
    {
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            flipSlider.gameObject.SetActive(true);
            currFlip = 0;
        }
        else if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            currFlip += Time.deltaTime;
            flipSlider.value = currFlip / compFlip ;
        }
        else if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            flipSlider.gameObject.SetActive(false);
            currFlip = 0;
        }

        if (currFlip >= compFlip)
        {
            StartCoroutine(Flip_V(1)); // 수직방향 움직임 
            StartCoroutine(Flip_H(1)); // 수평방향 움직임 (패드의 입력값을받아서 이동함. )
            currFlip = 0;
        }
    }
    IEnumerator Flip_V(float fliptime)
    {
        float currtime = 0;

        while (currtime <= fliptime)
        {
            currtime += Time.deltaTime;
            if (currtime <= (fliptime / 2)) //반절의 시간만큼 올라갔다가 내려오기
            {
                transform.position += Vector3.up * 0.02f;
            }
            else// 후 에는 내려가고 
            {
                transform.position -= Vector3.up * 0.02f;
            }

            yield return new WaitForEndOfFrame();
        }
    }//수직방향 플립 움직임
    IEnumerator Flip_H(float fliptime)// 수평방향 플립 움직임 & 움직이는 방향 회전 
    {
        Vector2 pedR = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
        if (pedR == null)
        {
            Debug.Log("조이패드를 움직이세요.");
        }

        float currtime = 0;
        Vector3 flip_dir = new Vector3(pedR.x, 0,pedR.y);
        flip_dir.Normalize();
        ViewBody.rotation = transform.rotation;//일단 보여지는 몸체 초기화

        Vector3 rotaix = Vector3.Cross(flip_dir, Vector3.up);
        while (currtime <= fliptime)
        {
            currtime += Time.deltaTime;

            if (currtime > (fliptime/4) && currtime <= (fliptime*3/4))
            {
                // - dir 방향으로 이동 
                transform.position -= flip_dir * 0.01f;
            }
            else // 1.5 초 초과  OR  0.5 초 이전 
            {
                //dir 방향으로 이동 
                transform.position += flip_dir * 0.01f;
            }

            ViewBody.Rotate(rotaix, 180 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
            //yield return new WaitForSeconds(0.3f);
        }

        ViewBody.rotation = transform.rotation;//보여지는 몸체부분 초기화. 
    }

    void L_MOVE()
    {
        Vector2 pedL = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);

        print(pedL.x +","+pedL.y);
        
        if (hadeLess)//해드리스 모드에서는 회전 안먹힘. 
        {
            transform.Translate(Vector3.up * pedL.y * Time.deltaTime); //상하 움직임  // Y 섬세한 값도 다 받음 
        }
        else
        {
            //y축이 범위에 들어올때는 x는 항상 on 
            if (Mathf.Abs(pedL.y) <= pedRange)//절댓값이 범위 이하로 들어올떄. 
            {
                //y 값 안받음.
                transform.Rotate(Vector3.up * pedL.x, rotSpeed);//좌우전환 // x 값만 받기! 
            }
            else
            {
                transform.Translate(Vector3.up * pedL.y * Time.deltaTime); //상하 움직임 //y받고
                if (Mathf.Abs(pedL.x) >= pedRange)// x값은 범위 이상일떄만 가동
                {
                    transform.Rotate(Vector3.up * pedL.x, rotSpeed);//좌우전환
                }// 범위 이하일떄는 x 값 안받음 . 
            }

        }
    }
    void R_MOVE()
    {
        Vector2 pedR = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
        Vector3 dirR = new Vector3(pedR.x, 0, pedR.y);

        //print(pedR.x + "," + pedR.y);

        transform.Translate(dirR * speed * Time.deltaTime); // 전후좌우 움직임
    }

    public void Hovering(float hoverRange)
    {
        if (hover)
        {
            ViewBody.localPosition += new Vector3(0, 0.001f, 0);
        }
        else
        {
            ViewBody.localPosition -= new Vector3(0, 0.001f, 0);
        }
        //==========
        if (ViewBody.localPosition.y >= hoverRange)
        {
            hover = false;
        }
        else if (ViewBody.localPosition.y <= 0)
        {
            hover = true;
        }
    }

    void LedColor()
    {
        if (OVRInput.GetDown(OVRInput.Button.One,OVRInput.Controller.RTouch))
        {
            GameObject led = ViewBody.GetChild(0).gameObject;
            MeshRenderer mr = led.GetComponent<MeshRenderer>();
            float r = Random.Range(0, 255);
            float g = Random.Range(0, 255);
            float b = Random.Range(0, 255);
            mr.material.color = new Color(r, g, b);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        rb.isKinematic = true;
        collSenser = true;
        print(collision.gameObject.name);
    }

    private void OnCollisionExit(Collision collision)
    {
        collSenser = false;
        print("bye" + collision.gameObject.name);
    }
}
