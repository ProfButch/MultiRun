using UnityEngine;
using System.Collections;


namespace MultiRun
{
    public static class MultiRunStatic
    {
        private static Cli.Parser cmdArgs;
       
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void InitializeOnLoad()
        {            
            cmdArgs = new Cli.Parser();
            WindowPositioner.ArrangeWindow(cmdArgs.windowArrangement);
            if (cmdArgs.disableDebugLogStackTrace) {
                Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
            }
        }
    }
}