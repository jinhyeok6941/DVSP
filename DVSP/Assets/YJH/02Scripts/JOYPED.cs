using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JOYPED : MonoBehaviour
{
    public Transform joystickL;
    public Transform joystickR;

    public MeshRenderer[] btn;

    public Material normal;
    public Material click;

    //public int isStep { get; set; } // Unity 창에서는 안보이고 다른 대서 접근은 가능한 Public

    public enum BTN_STATE
    {
        NORMAL,
        CLICK
    }

    private void Start()
    {
        for (int i = 0; i < btn.Length; i++)
        {
            btn[i].material = normal;
        }
    }

    private void Update()
    {
        //transform.position = Camera.main.transform.position + new Vector3(0, 0, 2);
    }
    public void JOYSTICK_MOVE(float lx,float ly,float rx,float ry)
    {
        joystickL.localPosition = new Vector3(lx * 0.5f, ly * 0.5f, -1);
        joystickR.localPosition = new Vector3(rx * 0.5f, ry * 0.5f, -1);
    }

    /// <summary>
    /// btn_num 은 0번은 ST버튼 , 1번은 Flip버튼 , 2번은 L_joy , 3번은 R_joy이다. 
    /// </summary>
    /// <param 버튼클릭상태="state"></param
    public void ST_Click(BTN_STATE state , int btn_num) 
    {
        if (state == BTN_STATE.CLICK)
        {
            btn[btn_num].material = click;
        }
        else
        {
            btn[btn_num].material = normal;
        }
    }

    public void Guide_Reset()
    {
        for (int i = 0; i < btn.Length; i++)
        {
            btn[i].material = normal;
        }
    }

    public void Guide_Joy(Vector3 localPos)
    {
        joystickL.localPosition = Vector3.Lerp(joystickL.position, localPos , 0.2f);
    }

}
