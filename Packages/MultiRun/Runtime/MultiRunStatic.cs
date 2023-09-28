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
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);            
            cmdArgs = new Cli.Parser();
            Debug.Log(cmdArgs.ToString());
            WindowPositioner.ArrangeWindow(cmdArgs.windowArrangement);
        }
    }
}