using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRDroneCtrl : RobotConnector2
{
    public static VRDroneCtrl instance;

    //��� ������ 
    public float speed = 10; //  �⺻ ��� ������
    
    public Space flymode = Space.World; //  ��� �����带 HeadLess OnOff ctrl
    int countMode = 0; // flymode ��Ʈ�� �� bool �� �̿밡�� ������ �ϴ� int�� 

    bool isFlying = false; // ��*������ ���� Ȯ�ο� bool �� 
    
    bool isFlip = false; // �ø� �Է��߿��� �� ������ Ȯ�� �� 
    bool isFliping = false; 
    
    float flipTime;

    Transform ViewBody;
    Rigidbody rb;

    Vector3 xz_Dir;

    Vector3 rotate_aix;
    Vector3 rotate_value;

    public Transform joystickL;
    public Transform joystickR;

    bool up;//ȣ���� �͸����� ���� bool��

    bool collSenser = false;

    private void Awake()
    {

        if (this != null) instance = this;
        Connect_Manager();
        ViewBody = transform.GetChild(0); // ������ ȿ���� �� �� ���� 
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        Debug_tempBytes();//�ǽð� ���� �޴� �Լ� ������Ʈ���� �ʼ� 

        //TEST
        //if (Input.GetKeyDown(KeyCode.G))
        //{
        //    StopAllCoroutines();
        //    StartCoroutine(StartFly());
        //}
        //if (Input.GetKeyDown(KeyCode.B))
        //{
        //    StopAllCoroutines();
        //    StartCoroutine(StopFly());
        //}

        //fliptest
        if (Input.GetKey(KeyCode.G) && !isFliping)
        {
            flipTime += Time.deltaTime;
            isFlip = true;
            
            if (flipTime >= 3.0f)
            { 
                StartCoroutine(Flip_H());
                StartCoroutine(Flip_V());
                isFliping = true;
            }
        }

        if (Input.GetKeyUp(KeyCode.G))
        {
            isFlip = false;
            isFliping = false;
        }


        if (Input.GetKeyDown(KeyCode.Space)) // �ص帮�� ���� ���� �׽�Ʈ
        {
            SeletMode();
        }

        //--------------------------------//
        if (isFlying == false) return; // �������ϋ��� �Ʒ� ���� ���� 

        L_JoyStick();
        R_JoyStick();

        Hovering();//ȣ���� �ִϸ��̼�

        //---�����е� ������ ���� �E�� ������  ���� ����. ����. ----//
        joystickL.localPosition = new Vector3(L_x * 0.003f, L_y * 0.003f, -1);
        joystickR.localPosition = new Vector3(R_x * 0.003f, R_y * 0.003f, -1);
    }

    void L_JoyStick() // TL, TM, TR, ML, CN, MR, BL, BM, BR
    {
        transform.Translate(Vector3.up * speed * L_y * 0.01f * Time.deltaTime);
        transform.Rotate((Vector3.up * L_x).normalized , 0.03f * L_x);
        Debug.Log(L_x + "  ,  " + (Vector3.up * L_x).normalized);
    }
    void R_JoyStick()
    {
        //�ø����̸� �������� ���� 
        if (isFlip) return;

        Vector3 dir = new Vector3(R_x,0,R_y) * 0.01f ; // �е� xz ���� �޾Ƽ� �׷��� ��п����ӿ� ���� 

        xz_Dir = dir;
        rotate_aix = Vector3.Cross(dir,Vector3.up); // ȸ�� �� ���ϴ� �Լ�
        //print(rotate_aix.x+","+ rotate_aix.y +","+ rotate_aix.z);

        if (flymode == Space.Self) 
        {
            ViewBody.eulerAngles = new Vector3(rotate_aix.x * -30, ViewBody.eulerAngles.y, rotate_aix.z * -30);  //rotate_aix * -50;  //ViewBody.Rotate(rotate_aix);
        }
        else if (flymode == Space.World) 
        { 
            ViewBody.eulerAngles = new Vector3(rotate_aix.x * -30, -transform.rotation.y, rotate_aix.z * -30);  //rotate_aix * -50;  //ViewBody.Rotate(rotate_aix);
        }

        //������ ����ȭ �ϰ� ������ ���� ������ ũ�� ���� 
        //transform.Translate(dir.normalized * speed * (R_Sense * 0.01f) * Time.deltaTime, flymode); // ������� headless��������ƴ��� ���� 
        transform.Translate(dir * speed * Time.deltaTime, flymode); // ������� headless��������ƴ��� ���� 
    }

    //control ��� �������� �ص帮�� ��� or ��� ��ü ���� �����Ӹ�� !
    public void SeletMode()
    {
        countMode++;
        if (countMode % 2 == 0)
        {
            flymode = Space.World;
        }
        else if (countMode % 2 == 1)
        {
            flymode = Space.Self;
            ViewBody.rotation = transform.rotation;
        }


    }
    public void SeletMode(Space mode)
    {
        flymode = mode;        
    }


    public void Hovering()
    {
        if (up)
        {
            ViewBody.localPosition += new Vector3(0,0.001f,0);
        }
        else
        {
            ViewBody.localPosition -= new Vector3(0,0.001f,0);
        }
        //==========
        if (ViewBody.localPosition.y >= 0.3f)
        {
            up = false;
        }
        else if(ViewBody.localPosition.y <= 0)
        {
            up = true;
        }
    }

    public void Co_START_STOP()
    {
        print("�۵�!");
        StopAllCoroutines();
        if (!isFlying)
        {
            isFlying = true;
            StartCoroutine(StartFly());
        }
        else
        {
            StartCoroutine(StopFly());
        }
    }

    IEnumerator StartFly()
    {
        float now_Y = transform.position.y;
        while (now_Y + 3.0f >=transform.position.y)
        {
            transform.position += Vector3.up * 0.01f;
            yield return new WaitForEndOfFrame();
        }
        print("stst");
        isFlying = true;
    }

    IEnumerator StopFly()
    {
        isFlying = false;
        ViewBody.localPosition = Vector3.zero;
        while (!collSenser)
        {
            transform.position -= Vector3.up * 0.01f;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator Flip_V()
    {
        float currtime = 0;

        while (currtime <=2.0f)
        {
            currtime += Time.deltaTime;
            if (currtime <= 1.0f)//1�� ������ �ö󰡱� 
            {
                transform.position += Vector3.up * 0.02f;
            }
            else// �� ���� �������� 
            {
                transform.position -= Vector3.up * 0.02f;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator Flip_H()// ������� ������ 
    {
        //isFlip = true;
        float currtime = 0;
        Vector3 flip_dir = new Vector3(R_x, 0, R_y);
        flip_dir.Normalize();

        Vector3 rotaix = Vector3.Cross( flip_dir,Vector3.up);
        while (currtime <=2.0f)
        {
            currtime += Time.deltaTime;

            if (currtime <= 0.5f) // 0 ~ 0.5�� ����
            {
                // dir �������� �̵�
                transform.position += flip_dir * 0.01f;
            }
            else if (currtime > 0.5f && currtime <= 1.5f)
            { 
                // - dir �������� �̵� 
                transform.position -= flip_dir * 0.01f;
            }
            else // 1.5 �� ����  
            { 
                //dir �������� �̵� 
                transform.position += flip_dir * 0.01f;
            }

            ViewBody.Rotate(rotaix, 180 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
            //yield return new WaitForSeconds(0.3f);
        }

        //isFlip = false;
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
