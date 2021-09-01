using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDrone : RobotConnector2
{
    public JOYPED joypad;
    //����� �����ϱ� 
    public int step; // �ܰ踦 ǥ���ϴ� 

    bool isFlying; // ���� Ȯ�� ��

    Transform ViewBody;
    Rigidbody rb;

    int speed = 5;
    int spCount = 0;

    bool isSTbtning = false;
    float stbtntime = 0;

    bool isFliping = false;
    float flipTime;

    bool collSenser = false;

    public Space flymode = Space.Self;
    Vector3 rotate_aix;

    private void Awake()
    {
        Connect_Manager();
        ViewBody = transform.GetChild(0); // ������ ȿ���� �� �� ���� 
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        rb.WakeUp(); // ������ �ٵ� �� �� �ƿ��ֱ�.  
        Debug_tempBytes();//�ǽð� ���� �޴� �Լ� ������Ʈ���� �ʼ� 
        //�ܰ� ���� ������ 
        Co_START_STOP();// 1�ܰ迡���� ���� ����ɾƴٰ��� ���� �ϱ�! // 0,1,2 step
            // ^1 4�ܰ� �ӵ���ȭ // 9,10,11 ���� 
        FLIP_Motion(); // 5�ܰ� �ø� // 12,13,14 step
            //^! 6�ܰ� ����ȭ // 15 , 16, 17 
        if (isFlying && !isFlip) /// �������̰� �ø����°� �ƴҋ�! 
        {
            R_JoyStick();  // 2�ܰ� ������ ������ �����Ӹ� �ǰ� �ϱ� // 3, 4, 5 Step
            L_JoyStick(); // 3�ܰ� ������ ���� ���Ʒ� ȸ�� ������  // 6 , 7, 8 step
        }
    }

    void NextStep(int next)
    {
        if (step < next)
        {
            step = next;
        }
    }

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
            if (isFlying && stbtntime <= 1    &&  step >= 9) //�������̰� 1�̳��� ������ �־����� �ӵ����       // 9���� �̻󋚸� �۵�
            {
                speed /= (spCount % 3 + 1); //���� ���ڷθ���� 
                spCount++;
                speed *= (spCount % 3 + 1); //  �ϳ� ���� ����ŭ �����ֱ� !

                if (step == 9)
                {
                    StartCoroutine(WaitTime(3.0f,2.0f, 10)); 
                    //10 �������� �Ѱ� ���� // 3�� �ڿ� 11���� Ŭ���� �޼��� // 2�ʵ� 12���� (���� ���������� !)
                }
            }
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

        isFlying = true;

        NextStep(1); //  1���� Stop step
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

        NextStep(2); // 2���� Ŭ���� �޽���
        StartCoroutine(WaitTime(2.0f,3)); // 2�� ��ٸ��� 3�� step�� �Ѿ��! 

        if (step == 15) //  15�����ϋ� �Ǵٽ� �����ϸ� 16�������� !
        {
            NextStep(16);
        }
    }

    void FLIP_Motion()
    {
        if (step < 12) return;

        // ------�����е� ui Ȯ�� -------
        if (isFlip)//��ư�� �������� 
        {
            joypad.ST_Click(JOYPED.BTN_STATE.CLICK, 1);
        }
        else // ��ư �ն��� �������! 
        {
            joypad.ST_Click(JOYPED.BTN_STATE.NORMAL, 1);
        }

        //=========���� ����==========

        if (isFlip && !isFliping) //  isFlip�� �����е� �Է°����� bool�� ��ȯ, 
        {
            NextStep(13); 
            flipTime += Time.deltaTime;

            if (flipTime >= 3.0f && isFlying) // �������� �� ��ư�� �ٿ� ������ ������ �ø� �۵� ! 
            {
                StopAllCoroutines();
                StartCoroutine(Flip_H());
                StartCoroutine(Flip_V());
                isFliping = true;
            }
        }
        else
        {            
            isFliping = false;
            if (flipTime < 1 && step > 16  && !isFlying) // �����߾ƴϰ� 16���� �̻���� 1�� ������ ��쿡�� �۵� ! 
            {/// ���� �� ��ȭ �ڵ� 
                GameObject led = ViewBody.GetChild(0).gameObject;
                MeshRenderer mr = led.GetComponent<MeshRenderer>();
                float r = Random.Range(0.0f, 1.0f);
                float g = Random.Range(0.0f, 1.0f);
                float b = Random.Range(0.0f, 1.0f);
                mr.material.color = new Color(r, g, b);
                NextStep(17); // 17 Ŭ���� �޼��� �����ְ� ��! !
                print(led.name);
            }
            flipTime = 0;
        }


    }
    IEnumerator Flip_V()
    {
        float currtime = 0;

        while (currtime <= 2.0f)
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
        NextStep(14); // 14step ��ٸ��� ��������! 
        StartCoroutine(WaitTime(2.0f, 15)); // 15�ܰ��! 
    }
    IEnumerator Flip_H()// ������� ������ & �����̴� ���� ȸ�� 
    {
        //isFlip = true;
        float currtime = 0;
        Vector3 flip_dir = new Vector3(R_x, 0, R_y);
        flip_dir.Normalize();
        ViewBody.rotation = transform.rotation;//�ϴ� �������� ��ü �ʱ�ȭ

        Vector3 rotaix = Vector3.Cross(flip_dir, Vector3.up);
        while (currtime <= 2.0f)
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

        ViewBody.rotation = transform.rotation;
        //isFlip = false;
    }



    void R_JoyStick()
    {        
        if (step < 3) return; // ���� ������ �ܰ谡 �ȿ����� ����, 

        Vector3 dir = new Vector3(R_x, 0, R_y) * 0.01f; // �е� xz ���� �޾Ƽ� �׷��� ��п����ӿ� ���� 

        rotate_aix = Vector3.Cross(dir, Vector3.up); // ȸ�� �� ���ϴ� �Լ�
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

        if (step == 3)
        {
            StartCoroutine(WaitTime(7.0f,2.0f, 4)); 
            //4 step ���� �ѱ�� //  5�� ��ٷȴٰ� 5step�� �Ѿ / + 2�ʱ�ٷ��� Ŭ���� �޽��� �����ְ�  6��������(������)! 
        }
    }

    void L_JoyStick() // TL, TM, TR, ML, CN, MR, BL, BM, BR
    {
        if (step < 6) return;

        transform.Translate(Vector3.up * speed * L_y * 0.01f * Time.deltaTime);
        transform.Rotate(Vector3.up, 0.03f * L_x);
        if (step == 6)
        {
            StartCoroutine(WaitTime(7.0f,2.0f, 7)); 
            //7 step ���� �Ѱ� ���� // 5�ʱ�ٷ��ٰ� 8step���� �Ѿ�� /  + 2�ʱ�ٷ��� Ŭ���� �޽��� �����ְ�  9��������(���� ��������)! 
        }
    }




    IEnumerator WaitTime(float waitTime, int nextStep)
    {
        yield return new WaitForSeconds(waitTime);
        NextStep(nextStep);
    }
    IEnumerator WaitTime(float waitTime,float cleartime , int nextStep)
    {
        NextStep(nextStep);

        yield return new WaitForSeconds(waitTime);
        NextStep(nextStep + 1);
        
        yield return new WaitForSeconds(cleartime); // �ൿ�� �P���� ���� Ŭ���� �޽����� �����ְ� �� ���� �������� ���� 
        NextStep(nextStep + 2);
    }


    private void OnCollisionEnter(Collision collision)
    {
        rb.isKinematic = true;
        collSenser = true;
        print(collision.gameObject.name);
    }

    private void OnCollisionExit(Collision collision)
    {
        rb.isKinematic = false;
        collSenser = false;
        print("bye" + collision.gameObject.name);
    }
}
