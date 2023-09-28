using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEditor.SceneManagement;

namespace Bitwesgames {
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
            defaultBuildPath = "c:\\temp\\unity_builds\\TheBuild.exe";
        }

        private void InitOsx()
        {
            mode = MODE_OSX;
            execExtension = "app";
            defaultBuildPath = "~/temp/unity_builds/TheBuild.app";
        }

        private string RunBuildCmdWin(string path, string log, string args)
        {
            // In windows, you have to give it the full path to where the log should
            // go, it isn't cool like OSX.  So we have to construct the full path from
            // the path to the executable.  Then it's pretty much the same as OSX.
            string logPath = Path.GetDirectoryName(path);
            string fullLogPath = Path.Join(logPath, log);

            return $"{path} --logfile {fullLogPath} {args}";
        }

        private string RunBuildCmdOsx(string path, string log, string args)
        {
            // Uses open to launch the app so we do not have to know where the actual
            // executable is inside the .app bundle.  This also sets the logfile to
            // be the <app name>.log or whatever is specified in logfile.  The log
            // will be in the same directory as the applicaiton.  Each new run
            // overwrites the existing log file if it exists.
            return $"open -n {path} --args --logfile {log} {args}";
        }

        public string RunBuildCommand(string path, string log, string adtlArgs = "")
        {
            string toReturn = "";
            if (mode == MODE_WIN)
            {
                toReturn = RunBuildCmdWin(path, log, adtlArgs);
            }
            else if (mode == MODE_OSX)
            {
                toReturn = RunBuildCmdOsx(path, log, adtlArgs);
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

        public BuildPlayerOptions BuildOpts()
        {
            BuildPlayerOptions opts = new BuildPlayerOptions();
            if (mode == MODE_WIN)
            {
                opts = BuildOptsWin();
            }
            else if (mode == MODE_OSX)
            {
                opts = BuildOptsOsx();
            }
            return opts;
        }
    }
}