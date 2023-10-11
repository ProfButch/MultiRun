using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

namespace MultiRun
{
    // This is how we can be notified that the application has terminated.
    public class MultiRunMono : MonoBehaviour
    {
        private void Awake() {
            DontDestroyOnLoad(this.gameObject);
        }

        private async void OnApplicationQuit() {
            Debug.Log("----------------- Application quit -----------------");
            Debug.Log("-- multi run EOF --");
            await Task.Yield();
            await Task.Yield();
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
            AddMultiRunMonoToActiveScene();
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


        private static async void AddMultiRunMonoToActiveScene() {
            Scene active = SceneManager.GetActiveScene();
            int waitLimit = 30;
            int waited = 0;

            while (waited < waitLimit && !active.isLoaded) {
                await Task.Yield();
            }

            if (active.isLoaded) {
                GameObject toAdd = new GameObject("MultiRunMonoGO");
                SceneManager.MoveGameObjectToScene(toAdd, active);
                toAdd.AddComponent<MultiRunMono>();
                
            }
        }
    }
}