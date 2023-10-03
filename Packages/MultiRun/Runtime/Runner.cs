using UnityEngine;
using System.Collections;


namespace MultiRun
{
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

            cmdArgs = new Cli.Parser();
            WindowPositioner.ArrangeWindow(cmdArgs.windowArrangement);
            if (cmdArgs.disableDebugLogStackTrace) {
                Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
            }

            hasInitialized = true;
        }
    }
}