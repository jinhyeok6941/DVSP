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
        transform.position = Camera.main.transform.position + new Vector3(0, 0, 2);
    }
    public void JOYSTICK_MOVE(float lx,float ly,float rx,float ry)
    {
        joystickL.localPosition = new Vector3(lx * 0.003f , ly * 0.003f, -1);
        joystickR.localPosition = new Vector3(rx * 0.003f, ry * 0.003f, -1);        
    }

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


}
