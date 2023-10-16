using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TestScene : MonoBehaviour
{
    float logTime = .2f;
    float timeElapsed = 0.0f;
    int logCount = 0;


    public Button btnSceneTwo;


    private void Start() {
        btnSceneTwo.onClick.AddListener(OnSceneTwoClicked);    
    }


    private void OnSceneTwoClicked() {
        SceneManager.LoadScene("SceneTwo");
    }


    void Update() {
        timeElapsed += Time.deltaTime;
        if(timeElapsed > logTime){
            logCount += 1;
            Debug.Log($"Logging a message {logCount}");
            timeElapsed = 0.0f;
        }
    }

}
