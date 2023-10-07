using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

namespace MultiRun
{
    public class MultiRunMono : MonoBehaviour
    {
        private void Awake()
        {
            Debug.Log("----------------- Awake -----------------");
            DontDestroyOnLoad(this.gameObject);
        }

        private void OnApplicationQuit() {
            Debug.Log("----------------- Application quit -----------------");
        }
    }


    public static class Runner {
        private static Cli.Parser cmdArgs;
        private static bool hasInitialized = false;
       
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void InitializeOnLoad() {
            // since InitializeOnLoad might need to be called manually, and I do not
            // understand when you would have to do this, it is possible that
            // InitializeOnLoad might get called twice.  This prevents everything
            // from running a second time, making extra calls harmless.
            if (hasInitialized) {
                return;
            }

            ApplyCliArgs();
            AddMultiRunMonToStartScene();
            hasInitialized = true;
        }

        private static void ApplyCliArgs()
        {
            cmdArgs = new Cli.Parser();
            WindowPositioner.ArrangeWindow(cmdArgs.windowArrangement);
            if (cmdArgs.disableDebugLogStackTrace)
            {
                Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
            }
        }


        private static async void AddMultiRunMonToStartScene()
        {
            Scene active = SceneManager.GetActiveScene();
            Debug.Log($"active scene = {active}::{active.name}::{active.isLoaded}");
            int waitLimit = 30;
            int waited = 0;
            while (waited < waitLimit && !active.isLoaded)
            {
                await Task.Yield();
                Debug.Log($"active scene = {active}::{active.name}::{active.isLoaded}");
            }

            if (active.isLoaded)
            {
                GameObject toAdd = new GameObject("MultiRunMonoGO");
                SceneManager.MoveGameObjectToScene(toAdd, active);
                toAdd.AddComponent<MultiRunMono>();
                
            }
        }
    }
}