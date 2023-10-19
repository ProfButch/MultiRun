using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEditor.SceneManagement;
using System;

namespace MultiRun.OsHelpers {
    public abstract class BaseHelper {
        public abstract string CmdLaunchBuild(string path, string args = "");
        public abstract BuildPlayerOptions BuildOpts();
        public abstract string CmdBringToFront(ShellHelper.ShellRequest req);
    }


    public class WinHelper : BaseHelper {
        override public string CmdLaunchBuild(string path, string args = "") {
            return $"{path} {args}";
        }


        override public BuildPlayerOptions BuildOpts() {
            BuildPlayerOptions opts = new BuildPlayerOptions();
            opts.target = BuildTarget.StandaloneWindows;
            opts.targetGroup = BuildTargetGroup.Standalone;

            return opts;
        }


        override public string CmdBringToFront(ShellHelper.ShellRequest req) {
            return "not implemented";
        }
    }


    public class OsxHelper : BaseHelper {
        const string PLIST_EXEC_KEY = "CFBundleExecutable";

        /// <summary>
        /// Hackish parsing of a plist file.  This should work fine for our
        /// purposes since the plist files generated for apps should be pretty
        /// consistent and reliable.  Famous last words right?  There's a lot of
        /// "shoulds" in this description...just saying.
        /// </summary>
        /// <param name="appPath"></param>
        /// <returns></returns>
        private Dictionary<string, string> ParsePlist(string appPath) {
            string plistPath = Path.Join(appPath, "Contents", "Info.plist");
            Dictionary<string, string> plistData = new Dictionary<string, string>();

            if (!File.Exists(plistPath)) {
                return plistData;
            }

            var lines = File.ReadLines(plistPath);
            string curKey = "";
            string curValue = "";
            foreach (string line in lines) {
                string fline = line.Trim();
                if (fline.StartsWith("<key>")) {
                    curKey = fline.Replace("<key>", "").Replace("</key>", "");
                } else if (curKey != string.Empty && fline.StartsWith("<string>")) {
                    curValue = fline.Replace("<string>", "").Replace("</string>", "");
                }

                if (curValue != string.Empty && curValue != string.Empty) {
                    plistData[curKey] = curValue;
                    curKey = string.Empty;
                    curValue = string.Empty;
                }
            }
            return plistData;
        }


        private string GetExecutableNameForApp(string appPath) {
            Dictionary<string, string> data = ParsePlist(appPath);
            if (data.ContainsKey(PLIST_EXEC_KEY)) {
                return data[PLIST_EXEC_KEY];
            } else {
                return string.Empty;
            }
        }


        override public string CmdLaunchBuild(string path, string args = "") {
            string execName = GetExecutableNameForApp(path);
            if (execName == string.Empty) {
                MuRu.LogWarning(
                    $"Could not find executable path for {path} using open instead.  " +
                    "Some features may not work with this launched instance of the game.");
                return $"open -n {path} --args {args}";
            } else {
                return $"{path}/Contents/MacOS/{execName} {args}";
            }
        }


        override public BuildPlayerOptions BuildOpts() {
            BuildPlayerOptions opts = new BuildPlayerOptions();
            opts.target = BuildTarget.StandaloneOSX;
            opts.targetGroup = BuildTargetGroup.Standalone;

            return opts;
        }


        override public string CmdBringToFront(ShellHelper.ShellRequest req) {
            string osascript = $"tell application \\\"System Events\\\"  to set frontmost of every process whose unix id is {req.process.Id} to true";
            //string cmd = $"osascript -e activate application \\\"{path}\\\"";
            string cmd = $"osascript -e '{osascript}'";
            return cmd;
        }
    }



    public class OsHelper {
        const string MODE_NONE = "none";
        const string MODE_WIN = "win";
        const string MODE_OSX = "osx";

        public string mode = MODE_NONE;
        public string execExtension = "";
        public string defaultBuildPath = "";


        private BaseHelper theHelper;

        public OsHelper() {
#if UNITY_EDITOR_WIN
            InitWindows();
#elif UNITY_EDITOR_OSX
            InitOsx();
#endif
        }

        private void InitWindows() {
            mode = MODE_WIN;
            execExtension = "exe";
            defaultBuildPath = "c:\\MultiRunBuild.exe";
            theHelper = new WinHelper();
        }

        private void InitOsx() {
            mode = MODE_OSX;
            execExtension = "app";
            defaultBuildPath = "~/MultiRunBuild.app";
            theHelper = new OsxHelper();
        }


        // Must use the full path the to log so that it works on windows and
        // mac.  I think mac changes the working directory when using open, but
        // windows does not, since you just run the exe.
        public string GetLogfileArg(string path, string logName) {
            string logPath = Path.GetDirectoryName(path);
            string fullLogPath = Path.Join(logPath, logName);
            string toReturn = $"--logfile {fullLogPath} ";

            return toReturn;
        }


        public BuildPlayerOptions BuildOpts() {
            BuildPlayerOptions opts = new BuildPlayerOptions();
            if (theHelper != null) {
                opts = theHelper.BuildOpts();
            }
            return opts;
        }


        public string CmdLaunchBuild(string path, string adtlArgs = "") {
            string toReturn = "";
            if (theHelper != null) {
                toReturn = theHelper.CmdLaunchBuild(path, adtlArgs);
            }
            return toReturn;
        }


        public string CmdBringToFront(ShellHelper.ShellRequest req) {
            if (mode == MODE_WIN) {
                return "";
            } else if (mode == MODE_OSX) {
                return theHelper.CmdBringToFront(req);
            } else {
                return "";
            }
        }
    }
}