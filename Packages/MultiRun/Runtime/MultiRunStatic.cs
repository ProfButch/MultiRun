using UnityEngine;
using System.Collections;


namespace MultiRun
{
    public static class MultiRunStatic
    {
        private static CommandLineParser cmdArgs;
       
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void InitializeOnLoad()
        {
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);            
            cmdArgs = new CommandLineParser();
            Debug.Log(cmdArgs.ToString());
            WindowPositioner.ArrangeWindow(cmdArgs.windowArrangement);
        }
    }
}