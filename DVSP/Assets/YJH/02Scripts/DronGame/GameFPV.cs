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
        currHp = maxHp; // �����ϸ� Ǯ��
        DrawPos();//�����ϸ� ��ġ �缳�� ���ֱ�!
    }

    void Update()
    {
        rb.WakeUp(); // �Ͼ! 
    }

    void DrawPos()
    {
        //10~20 ���� ���ڸ��� �������� ��ȣ�� �����ؼ� �ϳ��� �����ϱ�. 
        for (int i = 0; i < clearItem.Length; i++)
        {
            int num = Random.Range(0, itemPos.Count);
            clearItem[i].transform.position = itemPos[num];
            itemPos.RemoveAt(num);
        }
    }

    void CollisionItem(GameObject coll)
    {
        // ������κ����� �˻��Ѵ� 
        // ����� �κ����� �ش��ڸ��� UI �� �����ǿ� ǥ���Ѵ�.
        
        coll.SetActive(false);// �ε�ģ�ִ� ���ش�

        // ���� �� �������� ���� ���� (Claer)
        for (int i = 0; i < clearItem.Length; i++)
        {
            if (clearItem[i].activeSelf)
            {
                return; // ���� ���� �ȳ���
            }
        }
        //���� ����. ���� ���� 
    }
    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "ITEM": // �������� �Ծ��� �� �ൿ
                CollisionItem(collision.gameObject);
                break;
            case "PLANE": // �ٴڿ� ������ �ൿ
                break;
            default: // �⺻������ �ε�ġ�� �׳� �� ü�±���. 
                break;
        }
    }

}
