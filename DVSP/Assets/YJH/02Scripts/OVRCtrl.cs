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
        ViewBody = transform.GetChild(0); // ������ ȿ���� �� �� ���� 
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        StartStop_Speed();

        if (isFlying)//�������϶��� ������
        {
            Fliping(); //�ø�
            if (currFlip == 0) // �ø���ư�� ������������ ��������
            {
                L_MOVE();// ���Ͽ����� ,�¿�ȸ��
                R_MOVE(); // �����¿� ������
            }
            if (OVRInput.Get(OVRInput.Button.Any))
            {
                Hovering(0.3f);//ȣ���� 
            }
        }
        else
        {
            Debug.LogWarning("�������� �ƴԴϴ�. ");
            LedColor();//���ٲٱ�
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
            if (isFlying && currST <= 1)//�������̰� 1�̳��� ������ �־����� �ӵ����
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
                StartCoroutine(StartFly()); // ���������� �������
            }
            else
            {
                StartCoroutine(StopFly()); // �����߿��� ����
            }
            currST = 0;//�ѹ� �������� �ϴ� 0 ����� ! 
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
    }//�����ϴ� ���� 
    IEnumerator StopFly()
    {
        isFlying = false;
        ViewBody.localPosition = Vector3.zero;
        while (!collSenser)
        {
            transform.position -= Vector3.up * 0.01f;
            yield return new WaitForEndOfFrame();
        }
    }//���ߴ� ���� 

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
            StartCoroutine(Flip_V(1)); // �������� ������ 
            StartCoroutine(Flip_H(1)); // ������� ������ (�е��� �Է°����޾Ƽ� �̵���. )
            currFlip = 0;
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
        Vector2 pedR = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
        if (pedR == null)
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

            ViewBody.Rotate(rotaix, 180 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
            //yield return new WaitForSeconds(0.3f);
        }

        ViewBody.rotation = transform.rotation;//�������� ��ü�κ� �ʱ�ȭ. 
    }

    void L_MOVE()
    {
        Vector2 pedL = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);

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
        Vector2 pedR = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
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
