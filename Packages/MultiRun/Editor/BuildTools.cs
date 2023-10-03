using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using MultiRun;
using MultiRun.Cli;




namespace MultiRun {

    public class BuildTools { 
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
                if (value.Length != 0) {
                    EditorPrefs.SetString(PREF_BUILD_PATH, value);
                } else {
                    EditorPrefs.SetString(PREF_BUILD_PATH, string.Empty);
                }
            }
        }


        /*
         * Adapted from one of the repsonses on
         * https://answers.unity.com/questions/1128694/how-can-i-get-a-list-of-all-scenes-in-the-build.html
         *
         * This will get all the scenes that have been configured in the build
         * settings.  
         * 
         * If you specify a path for runScenePath then it will put that
         * path at the start of the array instead of its default spot in the list
         * (if it has one).  This also allows for building a scene that has not
         * been added to the build settings.
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


        public string MakeRunBuildCmd(string path, string logfile, string args)
        {
            string allArgs = args;
            if(allArgs.IndexOf("--logfile") == -1) {
                allArgs = osHelper.GetLogfileArg(path, logfile) + args;
            }
            string cmd = osHelper.RunBuildCommand(path, allArgs);
            return cmd;
        }


        public void RunBuild(string path, string logfile, string args) {
            string cmd = MakeRunBuildCmd(path, logfile, args);
            MuRu.Log($"[running]:  {cmd}");
            ShellHelper.ProcessCommand(cmd, "/");
        }


        public void RunBuild(string path) {
            string logfile = makeLogNameFromPath(path);
            RunBuild(path, logfile, "");
        }


        public string MakeRunBuildXCmd(string path, int i) {
            Dictionary<string, object> args = new Dictionary<string, object>();
            string moreArgs = ProjectSettings.instance.allInstanceArgs + " ";

            if (ProjectSettings.instance.arrangeWindows) {
                moreArgs += ProjectSettings.instance.instanceArgs[i] + " ";
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

            if (ProjectSettings.instance.disableLogStackTrace) {
                args[ArgDef.ARG_DISABLE_LOG_STACK_TRACE] = true;
            }

            moreArgs += ArgDef.MakeArgString(args);
            return moreArgs;
        }


        public void RunBuildX(string path, int i)
        {
            string moreArgs = MakeRunBuildXCmd(path, i);
            RunBuild(path, makeLogNameFromPath(path, $"_{i + 1}"), moreArgs);
        }


        public void RunBuildXTimes(int x = 1) {
            string curPath = GetBuildPath();

            if (curPath != string.Empty) {
                for (int i = 0; i < x; i++) {
                    RunBuildX(curPath, i);
                }
            } else {
                MuRu.LogError("Cannot run, build path not set.");
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
                MuRu.LogError("There are no scenes in the build.  Add at least one scene in Build Settings.");
                return false;
            }

            BuildReport report = BuildPipeline.BuildPlayer(opts.scenes, path, opts.target, BuildOptions.Development);
            bool success = false;
            if (report.summary.result == BuildResult.Succeeded) {
                string logmsg = $"Build succeeded: {path}";
                if (runCurrentScene) {
                    logmsg += $"\n    Built scene {EditorSceneManager.GetActiveScene().path} instead of default scene";
                }
                MuRu.Log(logmsg);
                success = true;
            } else if (report.summary.result == BuildResult.Failed) {
                MuRu.LogError("Build failed");
                success = false;
            }

            return success;
        }


        public string GetBuildPath() {
            if (ProjectSettings.instance.projectBuildPath != string.Empty) {
                return ProjectSettings.instance.projectBuildPath;
            } else {
                return buildPath;
            }
        }

    }
}
