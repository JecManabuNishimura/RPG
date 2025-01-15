using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    ScrollRect scrollRect;
    [SerializeField] GameObject obj;
    float pos;
    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        pos = 1 / ((obj.transform.childCount / 2.0f) - 13.0f);
    }

    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S)) 
        {
            scrollRect.verticalNormalizedPosition -= pos;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            scrollRect.verticalNormalizedPosition += pos;
        }
    }
}
