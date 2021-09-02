using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameFPV : MonoBehaviour
{
    public Slider hp;
    public Slider battery;

    public Text timer;

    public float maxHp = 100;
    float currHp;

    public List<Vector3> itemPos;
    public GameObject[] clearItem;

    Rigidbody rb;


    enum ENDING
    {
        DURING,
        CLEAR,
        TIMEOVER,
        CRASHOVER,
        None
    }
    ENDING gamestate = ENDING.DURING;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currHp = maxHp; // 시작하면 풀피
        DrawPos();//시작하면 위치 재설정 해주기!
    }

    void Update()
    {
        rb.WakeUp(); // 일어나! 
    }

    void DrawPos()
    {
        //10~20 개의 빈자리에 랜덤으로 번호를 추출해서 하나씩 배정하기. 
        for (int i = 0; i < clearItem.Length; i++)
        {
            int num = Random.Range(0, itemPos.Count);
            clearItem[i].transform.position = itemPos[num];
            itemPos.RemoveAt(num);
        }
    }

    void CollisionItem(GameObject coll)
    {
        // 몇번쨰부분인지 검사한다 
        // 몇번쨰 부분인지 해당자리의 UI 를 전광판에 표시한다.
        
        coll.SetActive(false);// 부디친애는 꺼준다

        // 전부 다 꺼졌으면 게임 종료 (Claer)
        for (int i = 0; i < clearItem.Length; i++)
        {
            if (clearItem[i].activeSelf)
            {
                return; // 아직 게임 안끝남
            }
        }
        //게임 끝남. 전부 꺼짐 
    }
    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "ITEM": // 아이템을 먹었을 떄 행동
                CollisionItem(collision.gameObject);
                break;
            case "PLANE": // 바닥에 착륙시 행동
                break;
            default: // 기본적으로 부디치면 그냥 다 체력깍임. 
                break;
        }
    }

}
