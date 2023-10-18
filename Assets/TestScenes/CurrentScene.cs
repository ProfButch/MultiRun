using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CurrentScene : MonoBehaviour
{
    public Button btnPrintHelloWorld;
    public Button btnPrintEOF;

    // Start is called before the first frame update
    void Start()
    {
        btnPrintHelloWorld.onClick.AddListener(OnBtnHellowWorldClicked);
        btnPrintEOF.onClick.AddListener(OnBtnPrintEofClicked);
    }



    private void OnBtnHellowWorldClicked()
    {
        Debug.Log("Hello World");
    }

    private void OnBtnPrintEofClicked()
    {
        Debug.Log(MultiRun.MultiRunMono.APP_END_IND);
    }
}
