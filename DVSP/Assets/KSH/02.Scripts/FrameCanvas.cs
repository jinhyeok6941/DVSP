using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrameCanvas : MonoBehaviour
{
    //������ī��Ʈ ����
    private int frameCount = 0;
    private float currTime = 0;
    public Text frameText;

    void Update()
    {
        //1�������� ���� ������ ������ī��Ʈ ������ ���ϰ� �� ���� Time.Deltatime�� �����ش�.
        //�� ������ ���� TEXT�� ����Ѵ�.
        FrameCount();
    }
    private void FrameCount()
    {
        frameCount++;
        currTime += Time.deltaTime;
        if (currTime >= 1)
        {
            //print(frameCount.ToString());
            frameText.text = "Frame :" + frameCount.ToString() + " FPS";
            frameCount = 0;
            currTime = 0;
        }
    }
}
