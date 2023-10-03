using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEditor.SceneManagement;
using System;

namespace MultiRun {
    public class OsHelper
    {
        const string MODE_NONE = "none";
        const string MODE_WIN = "win";
        const string MODE_OSX = "osx";

        public string mode = MODE_NONE;
        public string execExtension = "";
        public string defaultBuildPath = "";


        public OsHelper()
        {
#if UNITY_EDITOR_WIN
            InitWindows();
#elif UNITY_EDITOR_OSX
            InitOsx();
#endif
        }

        private void InitWindows()
        {
            mode = MODE_WIN;
            execExtension = "exe";
            defaultBuildPath = "c:\\MultiRunBuild.exe";
        }

        private void InitOsx()
        {
            mode = MODE_OSX;
            execExtension = "app";
            defaultBuildPath = "~/MultiRunBuild.app";
        }

        private string RunBuildCmdWin(string path, string args)
        {
            return $"{path} {args}";
        }

        private string RunBuildCmdOsx(string path, string args)
        {
            // Uses open to launch the app so we do not have to know where the 
            // actual executable is inside the .app bundle.
            return $"open -n {path} --args {args}";
        }

        public string RunBuildCommand(string path,string adtlArgs = "")
        {
            string toReturn = "";
            if (mode == MODE_WIN)
            {
                toReturn = RunBuildCmdWin(path, adtlArgs);
            }
            else if (mode == MODE_OSX)
            {
                toReturn = RunBuildCmdOsx(path, adtlArgs);
            }
            return toReturn;
        }

        private BuildPlayerOptions BuildOptsOsx()
        {
            BuildPlayerOptions opts = new BuildPlayerOptions();
            opts.target = BuildTarget.StandaloneOSX;
            opts.targetGroup = BuildTargetGroup.Standalone;

            return opts;
        }

        private BuildPlayerOptions BuildOptsWin()
        {
            BuildPlayerOptions opts = new BuildPlayerOptions();
            opts.target = BuildTarget.StandaloneWindows;
            opts.targetGroup = BuildTargetGroup.Standalone;

            return opts;
        }


        // Must use the full path the to log so that it works on windows and
        // mac.  I think mac changes the working directory when using open, but
        // windows does not, since you just run the exe.
        public string GetLogfileArg(string path, string logName)
        {
            string logPath = Path.GetDirectoryName(path);
            string fullLogPath = Path.Join(logPath, logName);
            string toReturn = $"--logfile {fullLogPath} ";

            return toReturn;
        }


        public BuildPlayerOptions BuildOpts() {
            BuildPlayerOptions opts = new BuildPlayerOptions();
            if (mode == MODE_WIN) {
                opts = BuildOptsWin();
            } else if (mode == MODE_OSX) {
                opts = BuildOptsOsx();
            }
            return opts;
        }
    }
}