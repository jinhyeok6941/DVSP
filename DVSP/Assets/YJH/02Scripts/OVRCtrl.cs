using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OVRCtrl : MonoBehaviour
{
    public JOYPED joyped;

    public Transform[] wings;

    Transform ViewBody;
    Rigidbody rb;

    Vector2 pedL;
    Vector2 pedR;
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
    bool ani;

    bool hover;

    void Start()
    {
        ViewBody = transform.GetChild(0); // ������ ȿ���� �� �� ���� 
        rb = GetComponent<Rigidbody>();
        stSlider.gameObject.SetActive(false);
        flipSlider.gameObject.SetActive(false);
    }

    void Update()
    {
        pedL = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);
        pedR = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
        joyped.JOYSTICK_MOVE(pedL.x,pedL.y,pedR.x,pedR.y);//�����е� ������ �Լ�
        StartStop_Speed();

        Fliping(); //�ø�
        if (isFlying)//�������϶��� ������
        {
            if (currFlip == 0) // �ø���ư�� ������������ ��������
            {
                L_MOVE(); // ���Ͽ����� ,�¿�ȸ��
                R_MOVE(); // �����¿� ������
            }
            if (OVRInput.Get(OVRInput.Button.Any))
            {
                Hovering(0.3f); //ȣ���� 
            }
        }
        else
        {
           // Debug.LogWarning("�������� �ƴԴϴ�. ");
            LedColor(); //���ٲٱ�
        }
        WingRote();// �������ϋ� ���� ���
    }

    void StartStop_Speed()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
        {//��������
            stSlider.gameObject.SetActive(true);
            joyped.ST_Click(JOYPED.BTN_STATE.CLICK, 0);
            currST = 0;
        }
        else if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
        {
            currST += Time.deltaTime;
            stSlider.value = currST / compST;

            if (currST >= compST)
            {
                if (!isFlying)
                {
                    ani = true;
                    StartCoroutine(StartFly()); // ���������� �������
                }
                else
                {
                    StartCoroutine(StopFly()); // �����߿��� ����
                }
                currST = 0; //�ѹ� �������� �ϴ� 0 ����� ! 
            }
        }
        else if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
        {//����
            if (isFlying && currST <= 1) //�������̰� 1�̳��� ������ �־����� �ӵ����
            {
                spCount++;
                speed *= (spCount % 3 + 1);
            }
            currST = 0;
            joyped.ST_Click(JOYPED.BTN_STATE.NORMAL, 0);
            stSlider.gameObject.SetActive(false);
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
    }//�̷��ϴ� ���� 
    IEnumerator StopFly()
    {
        isFlying = false;
        ViewBody.localPosition = Vector3.zero;
        while (!collSenser)
        {
            transform.position -= Vector3.up * 0.01f;
            yield return new WaitForEndOfFrame();
        }
        ani = false;
    } //�����Ѵ� ���� 

    void Fliping()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            joyped.ST_Click(JOYPED.BTN_STATE.CLICK, 1);
            flipSlider.gameObject.SetActive(true);
            currFlip = 0;
        }
        else if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            currFlip += Time.deltaTime;
            flipSlider.value = currFlip / compFlip ;
            //Debug.LogWarning(currFlip +","+ flipSlider.value);
        }
        else if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            joyped.ST_Click(JOYPED.BTN_STATE.NORMAL, 1);
            flipSlider.gameObject.SetActive(false);
            currFlip = 0;
        }

        if (currFlip >= compFlip && isFlying)
        {
            StartCoroutine(Flip_V(1)); // �������� ������ 
            StartCoroutine(Flip_H(1)); // ������� ������ (�е��� �Է°����޾Ƽ� �̵���. )
            currFlip = 0;
            print("�ø�!");
        }
    }
    IEnumerator Flip_V(float fliptime)
    {
        float currtime = 0;

        while (currtime <= fliptime)
        {
            currtime += Time.deltaTime;
            if (currtime <= (fliptime / 2)) //������ �ð���ŭ �ö󰬴ٰ� ��������
            {
                transform.position += Vector3.up * 0.02f;
            }
            else// �� ���� �������� 
            {
                transform.position -= Vector3.up * 0.02f;
            }

            yield return new WaitForEndOfFrame();
        }
    }//�������� �ø� ������
    IEnumerator Flip_H(float fliptime)// ������� �ø� ������ & �����̴� ���� ȸ�� 
    {
        if (pedR == new Vector2(0,0))
        {
            Debug.Log("�����е带 �����̼���.");
        }

        float currtime = 0;
        Vector3 flip_dir = new Vector3(pedR.x, 0,pedR.y);
        flip_dir.Normalize();
        ViewBody.rotation = transform.rotation;//�ϴ� �������� ��ü �ʱ�ȭ

        Vector3 rotaix = Vector3.Cross(flip_dir, Vector3.up);
        while (currtime <= fliptime)
        {
            currtime += Time.deltaTime;

            if (currtime > (fliptime/4) && currtime <= (fliptime*3/4))
            {
                // - dir �������� �̵� 
                transform.position -= flip_dir * 0.01f;
            }
            else // 1.5 �� �ʰ�  OR  0.5 �� ���� 
            {
                //dir �������� �̵� 
                transform.position += flip_dir * 0.01f;
            }

            ViewBody.Rotate(rotaix, (360/ fliptime) * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        ViewBody.rotation = transform.rotation;//�������� ��ü�κ� �ʱ�ȭ. 
    }

    void L_MOVE()
    {
        print(pedL.x +","+pedL.y);
        
        if (hadeLess)//�ص帮�� ��忡���� ȸ�� �ȸ���. 
        {
            transform.Translate(Vector3.up * pedL.y * Time.deltaTime); //���� ������  // Y ������ ���� �� ���� 
        }
        else
        {
            //y���� ������ ���ö��� x�� �׻� on 
            if (Mathf.Abs(pedL.y) <= pedRange)//������ ���� ���Ϸ� ���Ë�. 
            {
                //y �� �ȹ���.
                transform.Rotate(Vector3.up * pedL.x, rotSpeed);//�¿���ȯ // x ���� �ޱ�! 
            }
            else
            {
                transform.Translate(Vector3.up * pedL.y * Time.deltaTime); //���� ������ //y�ް�
                if (Mathf.Abs(pedL.x) >= pedRange)// x���� ���� �̻��ϋ��� ����
                {
                    transform.Rotate(Vector3.up * pedL.x, rotSpeed);//�¿���ȯ
                }// ���� �����ϋ��� x �� �ȹ��� . 
            }

        }
    }
    void R_MOVE()
    {
        Vector3 dirR = new Vector3(pedR.x, 0, pedR.y);

        //print(pedR.x + "," + pedR.y);

        transform.Translate(dirR * speed * Time.deltaTime); // �����¿� ������
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
            GameObject led = ViewBody.GetChild(6).gameObject;
            MeshRenderer mr = led.GetComponent<MeshRenderer>();
            float r = Random.Range(0.0f, 1.0f);
            float g = Random.Range(0.0f, 1.0f);
            float b = Random.Range(0.0f, 1.0f);
            mr.material.color = new Color(r, g, b);
        }
    }

    void WingRote()
    {
        if (ani)
        {
            for (int i = 0; i < wings.Length; i++)
            {
                wings[i].transform.Rotate(wings[i].transform.up,20);
            }
            print("������");
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
