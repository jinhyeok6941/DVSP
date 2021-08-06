using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapToolManager : MonoBehaviour
{
    // 생성 된 모델 관리용 리스트
    private List<Transform> listOfModel = new List<Transform>();
    public Transform model;
    // indicator 위치 값 활용 변수
    private List<Vector3> listOfVector = new List<Vector3>();
    // indicator 관련 변수
    private GameObject[] indicators = new GameObject[2500];
    public GameObject indicatorFactory;
    public Transform indicator;

    public GameObject cube;
    Ray ray;
    RaycastHit hit;
    Vector3 pos1, pos2, dir;

    public enum Control
    {
        Dragg,
        Fixe,
        Line
    }

    public Control state;

    private void Awake()
    {
        for (int i = 0; i < indicators.Length; i++)
        {
            indicators[i] = Instantiate(indicatorFactory);
            indicators[i].transform.SetParent(indicator);
            indicators[i].SetActive(false);
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {

        }
    }
    // Update is called once per frame
    void Update()
    {
        // if (Input.GetMouseButtonDown(0))
        // {
        //     listOfVector.Add(Vector3.zero);
        //     listOfModel.Add(Instantiate(model));
        // }
        // else if (Input.GetMouseButton(0))
        // {
        //     ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //     if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        //     {
        //         if (hit.collider.CompareTag("INDICATOR")) return;
        //         Vector3 pos = new Vector3((int)hit.point.x, (int)hit.point.y, (int)hit.point.z);
        //         int index = listOfVector.Count - 1;
        //         if (listOfVector[index] != pos)
        //         {
        //             indicators[index].transform.position = pos + Vector3.up * 0.55f;
        //             indicators[index].SetActive(true);
        //             listOfVector.Add(pos);
        //         }
        //     }
        // }
        // else if (Input.GetMouseButtonUp(0))
        // {
        //     for (int i = 1; i < listOfVector.Count; i++)
        //     {
        //         indicators[i - 1].SetActive(false);
        //         GameObject block = Instantiate(cube);
        //         block.transform.position = listOfVector[i] + Vector3.up;
        //         block.transform.SetParent(listOfModel[listOfModel.Count - 1]);
        //     }
        //     listOfVector.Clear();
        // }

        if (Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                pos1 = Int_Vector3(hit.point);
                //Debug.Log(pos1);
                //listOfModel.Add(Instantiate(model));
                indicators[0].SetActive(true);
                //indicators[0].transform.position = pos1 + Vector3.up * 0.5f;
            }
        }
        else if (Input.GetMouseButton(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                pos2 = Int_Vector3(hit.point);
                int x = Get_Distance(pos1.x, pos2.x);
                int y = Get_Distance(pos1.y, pos2.y);
                int z = Get_Distance(pos1.z, pos2.z);

                indicators[0].transform.localScale = new Vector3(x, y, z);
                indicators[0].transform.position = Get_Pos(pos1, pos2, hit.transform.up);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Debug.Log(hit.transform.up + "  ,  " + Get_Pos(pos1, pos2, hit.transform.up));
            GameObject go = Instantiate(cube, indicators[0].transform.position, Quaternion.identity);
            go.transform.localScale = indicators[0].transform.localScale;
            indicators[0].transform.localScale = Vector3.one;
            indicators[0].SetActive(false);
        }
    }
    // Vector3 값 int 형 변환
    Vector3 Int_Vector3(Vector3 pos)
    {
        return new Vector3((int)pos.x, (int)pos.y, (int)pos.z);
    }
    // 위치 값 조정
    Vector3 Get_Pos(Vector3 pos1, Vector3 pos2, Vector3 dir)
    {
        return (pos1 + pos2) / 2 + dir * 0.5f;
    }
    // 첫 지점과 끝 지점의 x , y , z 의 편차 구하기 
    int Get_Distance(float xyz1, float xyz2)
    {
        if (xyz1 == xyz2)
            return 1;
        else
            return Mathf.Abs((int)(xyz1 - xyz2));
    }
    
}
