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
            StopAllCoroutines();
            count++;
            ChangeTest();
            
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StopAllCoroutines();
            count--;
            ChangeTest();
        }

        
    }

    void ChangeTest()
    {
        guidePed.Guide_Reset();
        switch (count % 8)
        {
            case 0:
                guide.text = "���� ��� ��ư�� 3���̻� ������ ����� �̷��մϴ�.";
                StartCoroutine(PressHard(0));
                break;
            case 1:
                guide.text = "���� �߿� ���� ��� ��ư�� 3���̻� ������ ����� �����մϴ�.";
                StartCoroutine(PressHard(0));
                break;
            case 2:
                guide.text = "���� ���̽�ƽ���� �����¿� ����� ������������! ";
                StartCoroutine(PressBlink(3));
                break;
            case 3:
                guide.text = "���� ���̽�ƽ���� ����� ��� �ϰ���Ű�� , �¿�� ȸ�� ��ų�� �־��!";
                StartCoroutine(PressBlink(2));
                break;
            case 4:
                guide.text = "���� ��� ��ư�� ª�� ������ �ӵ��� �ٲ���!(3�ܰ�)";
                StartCoroutine(PressBlink(0));
                break;
            case 5:
                guide.text = "�������  ���� ����� ��ư�� �ڴ����鼭 ���� ���̽�ƽ�� �����̸� �����ι������� �������!";
                StartCoroutine(PressHard(1));
                StartCoroutine(PressBlink(3));
                break;
            case 6:
                guide.text = "������ ���� �� ���� ��� ��ư�� ª�� ������ ��� ���� �ٲ���!";
                StartCoroutine(PressBlink(1));
                break;
            case 7:
                guide.text = "headLess���� �����ϰ������ ��� ��ư�� ������ �ǿ�!";
                guidePed.ST_Click(JOYPED.BTN_STATE.CLICK, 4);
                break;
        }

    }

    //�� ������ ��� ǥ�� 
    IEnumerator PressHard(int btnNum)
    {
        while (true)
        {
            guidePed.ST_Click(JOYPED.BTN_STATE.CLICK, btnNum); //3�ʰ� ������
            yield return new WaitForSeconds(3f);
            guidePed.ST_Click(JOYPED.BTN_STATE.NORMAL, btnNum); //0.5�� �� �H�ٰ� �ٽ� �ݺ� ! 
            yield return new WaitForSeconds(0.5f);
        }
    }

    //��������������
    IEnumerator PressBlink(int btnNum)
    {
        while (true)
        {//0.5�� �� �H�ٰ� �ٽ� �ݺ� ! 
            guidePed.ST_Click(JOYPED.BTN_STATE.CLICK, btnNum); 
            yield return new WaitForSeconds(0.5f);
            guidePed.ST_Click(JOYPED.BTN_STATE.NORMAL, btnNum); 
            yield return new WaitForSeconds(0.5f);
        }
    }
    //�����Ӱ� �����̴� ���̽�ƽ 
    IEnumerator RandomJoy(int btnNum)
    {
        guidePed.ST_Click(JOYPED.BTN_STATE.CLICK, btnNum); // �������·�
        while (false)
        {//�� �ȿ��� ��ġ ���� ���� 

            //guidePed
            yield return new WaitForSeconds(0.5f);
            
        }
    }



}
