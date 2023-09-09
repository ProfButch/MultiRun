using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScene : MonoBehaviour
{
    float logTime = 1.0f;
    float timeElapsed = 0.0f;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;
        if(timeElapsed > logTime){
            Debug.Log($"Logging a message");
            timeElapsed = 0.0f;
        }
    }
}
