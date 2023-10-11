using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TestScene : MonoBehaviour
{
    float logTime = .2f;
    float timeElapsed = 0.0f;
    int logCount = 0;

    public Button btnSceneTwo;
    public Button btnPrintEof;

    private void Start() {
        btnSceneTwo.onClick.AddListener(OnSceneTwoClicked);
        btnPrintEof.onClick.AddListener(OnPrintEofClicked);
    }

    private void OnSceneTwoClicked() {
        Debug.Log("clicked");
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

    void OnPrintEofClicked()
    {
        Debug.Log("-- multi run EOF --");
    }
}
