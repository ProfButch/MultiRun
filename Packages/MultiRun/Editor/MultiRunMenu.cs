using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;



namespace Bitwesgames {


    [InitializeOnLoadAttribute]
    public static class MultiRunMenu{

        public static MultiRunEditor mre = new MultiRunEditor();

        private static void ShowLogs() {
            var window = EditorWindow.GetWindow<Bitwesgames.LogViewer>();
            window.basePath = Path.Join(Path.GetDirectoryName(mre.GetBuildPath()), Path.GetFileNameWithoutExtension(mre.GetBuildPath()));
            window.ShowPopup();
            window.LoadLogs();
        }


        private static bool IsBuildPathValid() {
            return mre.GetBuildPath() != string.Empty;
        }

        // ------------------------------
        // Validation
        //
        // Note, I thought about validating that the build file exists for the
        // run menu items, but File.Exist is false for directories and .app are
        // directories so it will be different validation for different OS and
        // that made me real mad.
        // ------------------------------
        [MenuItem("MultiRun/Build", true)]
        [MenuItem("MultiRun/Build Current Scene", true)]
        [MenuItem("MultiRun/Build & Run/1", true)]
        [MenuItem("MultiRun/Build & Run/2", true)]
        [MenuItem("MultiRun/Build & Run/3", true)]
        [MenuItem("MultiRun/Build & Run/4", true)]
        [MenuItem("MultiRun/Run/1", true)]
        [MenuItem("MultiRun/Run/2", true)]
        [MenuItem("MultiRun/Run/3", true)]
        [MenuItem("MultiRun/Run/4", true)]
        [MenuItem("MultiRun/Log Watcher (alpha)", true)]
        [MenuItem("MultiRun/Show Files", true)]
        private static bool ValidateBuildPathDependant() {
            return IsBuildPathValid();
        }



        //--------------------------------
        // Section 1
        //--------------------------------
        [MenuItem("MultiRun/Settings", false, 1)]
        private static void MnuRunSettings() {
            var window = EditorWindow.GetWindow<Bitwesgames.RunSettings>();
            window.ShowPopup();
        }


        //--------------------------------
        // Section 100
        //--------------------------------
        [MenuItem("MultiRun/Build", false, 100)]
        private static void MnuBuild() {
            string curPath = mre.GetBuildPath();
            // This handles the one case where it wasn't set and the user canceled
            // the dialog to set the build path.
            if (curPath != string.Empty) {
                mre.Build(curPath);
            }
        }

        

        [MenuItem("MultiRun/Build Current Scene", false, 100)]
        private static void MnuBuildCurrentScene() {
            string curPath = mre.GetBuildPath();
            // This handles the one case where it wasn't set and the user canceled
            // the dialog to set the build path.
            if (curPath != string.Empty) {
                mre.Build(curPath, true);
            }
        }

        //--------------------------------
        // Section 200
        //--------------------------------
        [MenuItem("MultiRun/Build & Run/1", false, 200)]
        private static void MnuBuildRun1() {
            mre.BuildThenRunX(1);
        }


        [MenuItem("MultiRun/Build & Run/2", false, 200)]
        private static void MnuBuildRun2() {
            mre.BuildThenRunX(2);
        }


        [MenuItem("MultiRun/Build & Run/3", false, 200)]
        private static void MnuBuildRun3() {
            mre.BuildThenRunX(3);
        }


        [MenuItem("MultiRun/Build & Run/4", false, 200)]
        private static void MnuBuildRun4(){
            mre.BuildThenRunX(4);
        }


        [MenuItem("MultiRun/Run/1", false, 200)]
        private static void MnuRun1() {
            mre.RunBuildXTimes(1);
        }

        [MenuItem("MultiRun/Run/2", false, 200)]
        private static void MnuRun2() {
            mre.RunBuildXTimes(2);
        }

        [MenuItem("MultiRun/Run/3", false, 200)]
        private static void MnuRun3() {
            mre.RunBuildXTimes(3);
        }

        [MenuItem("MultiRun/Run/4", false, 200)]
        private static void MnuRun4() {
            mre.RunBuildXTimes(4);
        }


        //--------------------------------
        // Section 999
        //--------------------------------
        [MenuItem("MultiRun/Log Watcher (alpha)", false, 999)]
        private static void MnuViewLogs() {
            ShowLogs();
        }


        [MenuItem("MultiRun/Show Files", false, 999)]
        private static void MnuViewFiles() {
            string parentDir = Path.GetDirectoryName(mre.GetBuildPath());
            if (Directory.Exists(parentDir)) {
                EditorUtility.RevealInFinder(mre.GetBuildPath());
            } else {
                Debug.LogError($"Cannot open {parentDir} because it does not exist.  Kicking off a build will create the path.");
            }
        }
    }
}