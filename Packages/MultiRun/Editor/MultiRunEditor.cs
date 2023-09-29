using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using MultiRun;
using MultiRun.Cli;




namespace Bitwesgames {

    public class MultiRunEditor{
        public const string PREF_BUILD_PATH = "MultiRun.BuildPath";
        private static OsHelper osHelper = new OsHelper();

        public string buildPath
        {
            get {
                if (EditorPrefs.HasKey(PREF_BUILD_PATH)) {
                    return EditorPrefs.GetString(PREF_BUILD_PATH);
                } else {
                    return string.Empty;
                }
            }

            set {
                if (value.Length != 0)
                {
                    EditorPrefs.SetString(PREF_BUILD_PATH, value);
                    Debug.Log($"Build path changed to {EditorPrefs.GetString(PREF_BUILD_PATH)}");
                } else
                {
                    EditorPrefs.SetString(PREF_BUILD_PATH, string.Empty);
                    Debug.LogError($"Invalid build path:  {value}");
                }
            }
        }


        /*
         * Adapted from one of the repsonses on
         * https://answers.unity.com/questions/1128694/how-can-i-get-a-list-of-all-scenes-in-the-build.html
         *
         * This will get all the scenes that have been configured in the build
         * settings.  If you specify a path for runScenePath then it will put that
         * path at the start of the array instead of its default spot in the list
         * (if it has one).
         */
        private string[] getBuildScenes(string runScenePath = "") {
            List<string> scenes = new List<string>();

            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
                if (scene.enabled && scene.path != runScenePath) {
                    scenes.Add(scene.path);
                }
            }

            if (runScenePath != string.Empty) {
                scenes.Insert(0, runScenePath);
            }

            return scenes.ToArray();
        }


        private string makeLogNameFromPath(string path, string extra = "") {
            string toReturn = $"{Path.GetFileNameWithoutExtension(path)}{extra}.log";
            return toReturn;
        }


        public void RunBuild(string path, string logfile, string args) {
            string cmd = osHelper.RunBuildCommand(path, logfile, args);
            Debug.Log($"[running]:  {cmd}");
            ShellHelper.ProcessCommand(cmd, "/");
        }


        public void RunBuild(string path) {
            string logPath = makeLogNameFromPath(path);
            RunBuild(path, logPath, "");
        }


        public void RunBuildX(string path, int i)
        {
            Dictionary<string, object> args = new Dictionary<string, object>();

            if (MultiRunSettings.instance.arrangeWindows) {
                if (i == 0) {
                    args[ArgDef.ARG_WINDOW_ARRANGEMENT] = WindowPositioner.ARRANGE_TOP_LEFT;
                } else if (i == 1) {
                    args[ArgDef.ARG_WINDOW_ARRANGEMENT] = WindowPositioner.ARRANGE_TOP_RIGHT;
                } else if (i == 2) {
                    args[ArgDef.ARG_WINDOW_ARRANGEMENT] = WindowPositioner.ARRANGE_BOTTOM_LEFT;
                } else if (i == 3) {
                    args[ArgDef.ARG_WINDOW_ARRANGEMENT] = WindowPositioner.ARRANGE_BOTTOM_RIGHT;
                }
            }


            if (MultiRunSettings.instance.disableLogStackTrace) {
                args[ArgDef.ARG_DISABLE_LOG_STACK_TRACE] = true;
            }

            string moreArgs = ArgDef.MakeArgString(args);
            RunBuild(path, makeLogNameFromPath(path, $"_{i + 1}"), moreArgs);
        }


        public void RunBuildXTimes(int x = 1) {
            string curPath = GetBuildPath();

            if (curPath != string.Empty) {
                for (int i = 0; i < x; i++) {
                    RunBuildX(curPath, i);
                }
            } else {
                Debug.LogError("Cannot run, build path not set.");
            }
        }


        public void BuildThenRunX(int x = 1, bool runCurrentScene = false) {
            if (GetBuildPath() != string.Empty) {
                bool result = Build(GetBuildPath(), runCurrentScene);
                if (result) {
                    RunBuildXTimes(x);
                }
            }
        }


        /*
         * Prompts the user for a path to a file.  Can optionally provide the
         * current path that is being used.
         *
         * This will set the the BUILD_PATH_PREF editor preference value to what
         * is chosen and also return that value.
         *
         * If the user cancels then the editor preference is not set and the current
         * value of the editor preference is returned.
         */
        public string getFilePathFromUser(string startDir = "") {
            string fullPath = startDir;
            string curDir = "";
            string curFile = "";
            string ext = osHelper.execExtension;

            if (fullPath == string.Empty) {
                fullPath = "";
            } else {
                curFile = Path.GetFileName(startDir);
                curDir = Path.GetFullPath(startDir);
            }

            string toReturn = EditorUtility.SaveFilePanel("Set Build Path", curDir, curFile, ext);

            return toReturn;
        }


        /*
         * Adapted from https://docs.unity3d.com/Manual/BuildPlayerPipeline.html
         */
        public bool Build(string path, bool runCurrentScene = false) {
            BuildPlayerOptions opts = osHelper.BuildOpts();
            opts.locationPathName = path;   // This will make the path if it does not exist.
            if (runCurrentScene) {
                opts.scenes = getBuildScenes(EditorSceneManager.GetActiveScene().path);
            } else {
                opts.scenes = getBuildScenes();
            }

            if(opts.scenes.Length == 0){
                Debug.LogError("There are no scenes in the build.  Add at least one scene in Build Settings.");
                return false;
            }

            BuildReport report = BuildPipeline.BuildPlayer(opts.scenes, path, opts.target, BuildOptions.Development);
            bool success = false;
            if (report.summary.result == BuildResult.Succeeded) {
                string logmsg = $"Build succeeded: {path}";
                if (runCurrentScene) {
                    logmsg += $"\n    Built scene {EditorSceneManager.GetActiveScene().path} instead of default scene";
                }
                Debug.Log(logmsg);
                success = true;
            } else if (report.summary.result == BuildResult.Failed) {
                Debug.Log("Build failed");
                success = false;
            }

            return success;
        }


        public string GetBuildPath() {
            if (MultiRunSettings.instance.projectBuildPath != string.Empty) {
                return MultiRunSettings.instance.projectBuildPath;
            } else {
                return buildPath;
            }
        }

    }
}
