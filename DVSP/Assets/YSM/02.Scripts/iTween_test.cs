using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class iTween_test : MonoBehaviour
{
    void Start()
    {
        iTween.MoveBy(gameObject,
            iTween.Hash("x",50, 
            "time",1,
            "easetype",iTween.EaseType.easeInBounce
            ));        
    }

    void Update()
    {
        
    }
}
