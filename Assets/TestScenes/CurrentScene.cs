using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CurrentScene : MonoBehaviour
{
    public Button btnPrintHelloWorld;

    // Start is called before the first frame update
    void Start()
    {
        if (btnPrintHelloWorld)
        {
            btnPrintHelloWorld.onClick.AddListener(OnBtnHellowWorldClicked);
        }    
    }



    private void OnBtnHellowWorldClicked()
    {
        Debug.Log("Hello World");
    }
}
