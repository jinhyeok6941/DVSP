using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrameCanvas : MonoBehaviour
{
    //프레임카운트 변수
    private int frameCount = 0;
    private float currTime = 0;
    public Text frameText;

    void Update()
    {
        //1프레임이 지날 때마다 프레임카운트 변수를 더하고 그 값에 Time.Deltatime을 더해준다.
        //그 더해준 값을 TEXT로 출력한다.
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
