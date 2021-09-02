using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeFlow : MonoBehaviour
{
    private float currTime = 0;
    public Text TimeText;
    
    void Update()
    {
        currTime += Time.deltaTime;
        TimeText.text = string.Format("{0:N2}", currTime);
    }
}
