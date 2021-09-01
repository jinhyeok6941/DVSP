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

    // 단계가 클리어 되면 완료된 단계의 step의 불들어오게 하기.
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
    //단계 변화 감지
    void StepNext()
    {
        if (stepCount != drone.step)// 단계가 다르면
        {
            stepCount = drone.step;// 그 단계로 지정해주고 
            StopAllCoroutines();//모든 코루틴 멈춰주고
            ResetBtn();// 색을 전부 normal로 바꿔주고  
            SeletStep(); //해당 Step에 맞는 행동 실행 
        }
    }

    void SeletStep()
    {
        switch (stepCount % 17)
        {
            case 0:
                guide.text = "좌측 상단 버튼을 3초이상 누르면 드론이 이륙합니다.";
                StartCoroutine(PressHard(0));
                break;
            case 1:
                guide.text = "비행 중에 좌측 상단 버튼을 3초이상 누르면 드론이 착륙합니다.";
                StartCoroutine(PressHard(0));
                break;
            case 2:
                guide.text = "완료! 다시 이륙해봐요! ";
                ClearRound(0);
                break;
            //case 3:
            //    break; // 3번은 의미 없음 한번만 하기 위해 그냥 있는 숫자 
            case 4:
                guide.text = "우측 조이스틱으로 전후좌우 드론을 움직여보세요! ";
                StartCoroutine(PressBlink(3));
                break;
            case 5:
                guide.text = "완료! ";
                ClearRound(1);
                break;
            //case 6:
            //    break;
            case 7:
                guide.text = "좌측 조이스틱으로 드론을 상승 하강시키고 , 좌우로 회전 시킬수 있어요!";
                StartCoroutine(PressBlink(2));
                break;
            case 8:
                guide.text = "완료! ";
                ClearRound(2);
                break;
            //case 9:
            //    break;
            case 10:
                guide.text = "좌측 상단 버튼을 짧게 누르면 속도가 바뀌어요!(3단계)";
                StartCoroutine(PressBlink(0));
                break;
            case 11:
                guide.text = "완료! ";
                ClearRound(3);
                break;
            case 12:
                guide.text = "뒤집기는  우측 상단의 버튼을 꾹 3초이상 눌르면서";
                StartCoroutine(PressHard(1));
                break;
            case 13:
                guide.text = " 버튼을 누르면서 우측 조이스틱을 움직이면 움직인방향으로 뒤집어요!";
                StartCoroutine(PressHard(1));
                StartCoroutine(PressBlink(3));
                break;
            case 14:
                guide.text = "완료! ";
                ClearRound(4);
                break;
            case 15:
                guide.text = "일단 바닥에 한번 착륙해봐요! ";
                StartCoroutine(PressHard(0));
                break;
            case 16:
                guide.text = "착륙해있을 떄 우측 상단 버튼을 누르면 드론 색이 바뀌어요! ";
                StartCoroutine(PressBlink(1));
                break;
            case 17:
                guide.text = "완료! ";
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








    //꾹 누르는 모습 표현 
    IEnumerator PressHard(int btnNum)
    {
        while (true)
        {
            guidePed.ST_Click(JOYPED.BTN_STATE.CLICK, btnNum); //3초가 누르고
            yield return new WaitForSeconds(3f);
            guidePed.ST_Click(JOYPED.BTN_STATE.NORMAL, btnNum); //0.5초 손 똇다가 다시 반복 ! 
            yield return new WaitForSeconds(0.5f);
        }
    }

    //깜빡깜빡누르는
    IEnumerator PressBlink(int btnNum)
    {
        while (true)
        {//0.5초 손 똇다가 다시 반복 ! 
            guidePed.ST_Click(JOYPED.BTN_STATE.CLICK, btnNum); 
            yield return new WaitForSeconds(0.5f);
            guidePed.ST_Click(JOYPED.BTN_STATE.NORMAL, btnNum); 
            yield return new WaitForSeconds(0.5f);
        }
    }
    //자유롭게 움직이는 조이스틱 
    IEnumerator RandomJoy(int btnNum)
    {
        guidePed.ST_Click(JOYPED.BTN_STATE.CLICK, btnNum); // 누른상태로
        while (false)
        {//원 안에서 위치 랜덤 변경 

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
