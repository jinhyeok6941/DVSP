using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDrone : RobotConnector2
{
    //����� �����ϱ� 
    public int step; // �ܰ踦 ǥ���ϴ� 

    bool isFlying; // ���� Ȯ�� ��

    Transform ViewBody;
    Rigidbody rb;

    int speed = 5;

    bool isSTbtning = false;
    float stbtntime = 0;
    bool collSenser = false;



    JOYPED joypad;
    private void Awake()
    {
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
        //�ܰ� ���� ������ 
        Co_START_STOP();// 1�ܰ迡���� ���� ����ɾƴٰ��� ���� �ϱ�! 
        // 2�ܰ� ������ ������ �����Ӹ� �ǰ� �ϱ� 
        // 3�ܰ� ������ ���� ���Ʒ� ȸ�� ������ 
        // 4�ܰ� �ӵ���ȭ
        // 5�ܰ� �ø�
        // 6�ܰ� ���ٲٱ� 
        // 7�ܰ� ������,��Ʈ�� 
    }

    void NextStep(int next)
    {
        if (step > next)
        {
            step = next;
        }
    }

    //1�ܵ� ������ 
    public void Co_START_STOP() //  ��ư ������ �Լ� �ҷ��ٰ� ��� ��� ǥ������? 
    {
        // ------�����е� ui Ȯ�� -------//0������ st��ư ! 
        if (isSTbtn)//��ư�� �������� 
        {
            joypad.ST_Click(JOYPED.BTN_STATE.CLICK, 0);
        }
        else // ��ư �ն��� �������! 
        {
            joypad.ST_Click(JOYPED.BTN_STATE.NORMAL, 0);
        }
        //=========���� ����==========

        if (isSTbtn && !isSTbtning) //  isSTbtn �� �����е� �Է°����� bool�� ��ȯ, 
        {
            stbtntime += Time.deltaTime;

            if (stbtntime >= 3.0f) //  ��ư�� �ٿ� ������ ������ �ø� �۵� ! 
            {
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
                isSTbtning = true;
            }
        }
        else // ������ ��
        {
            isSTbtning = false;

            stbtntime = 0;
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

        NextStep(1);

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
        NextStep(2);
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
