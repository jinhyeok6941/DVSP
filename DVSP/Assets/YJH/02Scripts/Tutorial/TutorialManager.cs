using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public Text guide;
    public JOYPED guidePed;
    public TutorialDrone drone;

    int count = 0;

    int stepCount;

    public MeshRenderer[] round;
    public Material clear;
    void Start()
    {
        SeletStep();
    }

    // �ܰ谡 Ŭ���� �Ǹ� �Ϸ�� �ܰ��� step�� �ҵ����� �ϱ�.
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    StopAllCoroutines();
        //    count++;
        //    ChangeTest();

        //}
        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    StopAllCoroutines();
        //    count--;
        //    ChangeTest();
        //}
        StepNext();
    }
    //�ܰ� ��ȭ ����
    void StepNext()
    {
        if (stepCount != drone.step)// �ܰ谡 �ٸ���
        {
            stepCount = drone.step;// �� �ܰ�� �������ְ� 
            StopAllCoroutines();//��� �ڷ�ƾ �����ְ�
            ResetBtn();// ���� ���� normal�� �ٲ��ְ�  
            SeletStep(); //�ش� Step�� �´� �ൿ ���� 
        }
    }

    void SeletStep()
    {
        switch (stepCount % 17)
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
                guide.text = "�Ϸ�! �ٽ� �̷��غ���! ";
                ClearRound(0);
                break;
            //case 3:
            //    break; // 3���� �ǹ� ���� �ѹ��� �ϱ� ���� �׳� �ִ� ���� 
            case 4:
                guide.text = "���� ���̽�ƽ���� �����¿� ����� ������������! ";
                StartCoroutine(PressBlink(3));
                break;
            case 5:
                guide.text = "�Ϸ�! ";
                ClearRound(1);
                break;
            //case 6:
            //    break;
            case 7:
                guide.text = "���� ���̽�ƽ���� ����� ��� �ϰ���Ű�� , �¿�� ȸ�� ��ų�� �־��!";
                StartCoroutine(PressBlink(2));
                break;
            case 8:
                guide.text = "�Ϸ�! ";
                ClearRound(2);
                break;
            //case 9:
            //    break;
            case 10:
                guide.text = "���� ��� ��ư�� ª�� ������ �ӵ��� �ٲ���!(3�ܰ�)";
                StartCoroutine(PressBlink(0));
                break;
            case 11:
                guide.text = "�Ϸ�! ";
                ClearRound(3);
                break;
            case 12:
                guide.text = "�������  ���� ����� ��ư�� �� 3���̻� �����鼭";
                StartCoroutine(PressHard(1));
                break;
            case 13:
                guide.text = " ��ư�� �����鼭 ���� ���̽�ƽ�� �����̸� �����ι������� �������!";
                StartCoroutine(PressHard(1));
                StartCoroutine(PressBlink(3));
                break;
            case 14:
                guide.text = "�Ϸ�! ";
                ClearRound(4);
                break;
            case 15:
                guide.text = "�ϴ� �ٴڿ� �ѹ� �����غ���! ";
                StartCoroutine(PressHard(0));
                break;
            case 16:
                guide.text = "���������� �� ���� ��� ��ư�� ������ ��� ���� �ٲ���! ";
                StartCoroutine(PressBlink(1));
                break;
            case 17:
                guide.text = "�Ϸ�! ";
                ClearRound(6);
                break;
            default:
                break;
        }
    }


    void ClearRound(int roundNum)
    {
        round[roundNum].material = clear;
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

    void ResetBtn()
    {
        for (int i = 0; i < 5; i++)
        {
            guidePed.ST_Click(JOYPED.BTN_STATE.NORMAL, i);
        }
    }


}
