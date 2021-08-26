using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public Text guide;
    public JOYPED guidePed;

    int count = 0;
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            count++;
            
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            count--;
        }

        ChangeTest();
        
    }

    void ChangeTest()
    {
        guidePed.Guide_Reset();
        switch (count % 8)
        {
            case 0:
                guide.text = "���� ��� ��ư�� 3���̻� ������ ����� �̷��մϴ�.";
                guidePed.ST_Click(JOYPED.BTN_STATE.CLICK,0); 
                // 3�� ������ ���� ? 
                break;
            case 1:
                guide.text = "���� �߿� ���� ��� ��ư�� 3���̻� ������ ����� �����մϴ�.";
                guidePed.ST_Click(JOYPED.BTN_STATE.CLICK, 0); 
                // 3�� ������ ��� 
                break;
            case 2:
                guide.text = "���� ���̽�ƽ���� �����¿� ����� ������������! ";
                guidePed.ST_Click(JOYPED.BTN_STATE.CLICK, 3); 
                // ���̽�ƽ�� �����̴�
                break;
            case 3:
                guide.text = "���� ���̽�ƽ���� ����� ��� �ϰ���Ű�� , �¿�� ȸ�� ��ų�� �־��!";
                guidePed.ST_Click(JOYPED.BTN_STATE.CLICK, 2); 
                // ���̽�ƽ�� �����̴� 
                break;
            case 4:
                guide.text = "���� ��� ��ư�� ª�� ������ �ӵ��� �ٲ���!(3�ܰ�)";
                guidePed.ST_Click(JOYPED.BTN_STATE.CLICK, 0); 
                // ��������
                break;
            case 5:
                guide.text = "�������  ���� ����� ��ư�� �ڴ����鼭 ���� ���̽�ƽ�� �����̸� �����ι������� �������!";
                guidePed.ST_Click(JOYPED.BTN_STATE.CLICK, 1); // �� ������ ���
                guidePed.ST_Click(JOYPED.BTN_STATE.CLICK, 3); // ���̽�ƽ�� ȸ���� 
                break;
            case 6:
                guide.text = "������ ���� �� ���� ��� ��ư�� ª�� ������ ��� ���� �ٲ���!";
                guidePed.ST_Click(JOYPED.BTN_STATE.CLICK, 1); // ��������
                break;
            case 7:
                guide.text = "headLess���� �����ϰ������ ��� ��ư�� ������ �ǿ�!";
                guidePed.ST_Click(JOYPED.BTN_STATE.CLICK, 4);
                break;
        }

    }


}
