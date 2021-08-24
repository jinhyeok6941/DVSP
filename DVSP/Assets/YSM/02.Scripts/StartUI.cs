using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartUI : MonoBehaviour
{
    LineRenderer lr;
    public Transform lHand;
    public Transform rHand;
    public Transform dot;

    void Start()
    {
        //lr = gameObject.GetComponentInChildren<LineRenderer>();
    }

    void Update()
    {
        pointer();
        //DrawGuideLine();
    }

    void DrawGuideLine()
    {
        //1. ������ ��ġ, ������ �չ��⿡�� �߻��ϴ� Ray�� �����.
        Ray ray = new Ray(rHand.transform.position, rHand.transform.forward);
        //2. �ε��� ���� �ִٸ�
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            //3. �ε��� �������� Line �� �׸���
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, hit.point);
        }
        else
        {
            //4. �ε��� ������ ������ ��������ġ���� ������ �չ������� ����ͱ��� �׷���
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, rHand.transform.position + rHand.transform.forward * 1);
        }
    }
    void pointer()
    {
        //������ ��ġ���� ������ �չ���
        Ray ray = new Ray(rHand.transform.position, rHand.forward);
        //�ε����ٸ�
        RaycastHit hit;

        //int layer = 1 << LayerMask.NameToLayer("UI");
        //if(Physics.Raycast(ray, out hit,100))
        if (Physics.Raycast(ray, out hit))
        {
            //���� �ε������� layer�� UI���
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                //����ġ�� ������ ����
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
                //btn ��ũ��Ʈ �����´�
                Button btn = hit.transform.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.Invoke();
                }
            }
        }
    }
}