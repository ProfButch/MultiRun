using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;



namespace com.bitwesgames {
    

    [InitializeOnLoadAttribute]
    public static class MultiRunMenu{

        private static MultiRunEditor mre = new MultiRunEditor();


        private static void ShowLogs() {
            var window = EditorWindow.GetWindow<com.bitwesgames.LogViewer>();
            window.basePath = Path.Join(Path.GetDirectoryName(mre.buildPath), Path.GetFileNameWithoutExtension(mre.buildPath));
            window.ShowPopup();
            window.LoadLogs();
        }



        //--------------------------------
        // Section 1
        //--------------------------------
        [MenuItem("MultiRun/Set Build Path", false, 1)]
        private static void MnuSetBuildPath() {
            string newPath = mre.getFilePathFromUser(mre.buildPath);
            if(newPath.Length != 0)
            {
                mre.buildPath = newPath;
            }
        }


        //--------------------------------
        // Section 100
        //--------------------------------
        [MenuItem("MultiRun/Build", false, 100)]
        private static void MnuBuild() {
            string curPath = mre.buildPath;
            // This handles the one case where it wasn't set and the user canceled
            // the dialog to set the build path.
            if (curPath != string.Empty) {
                mre.Build(curPath);
            }
        }


        [MenuItem("MultiRun/Build Current Scene", false, 100)]
        private static void MnuBuildCurrentScene() {
            string curPath = mre.buildPath;
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
        private static void MnuBuildRun4() {
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
        [MenuItem("MultiRun/View Logs", false, 999)]
        private static void MnuViewLogs() {
            ShowLogs();
        }

        [MenuItem("MultiRun/Show Files", false, 999)]
        private static void MnuViewFiles() {
            string parentDir = Path.GetDirectoryName(mre.buildPath);
            if (Directory.Exists(parentDir)) {
                EditorUtility.RevealInFinder(mre.buildPath);
            } else {
                Debug.LogError($"Cannot open {parentDir} because it does not exist.  Kicking off a build will create the path.");
            }
        }


        [MenuItem("MultiRun/About", false, 999)]
        private static void MnuAbout(){
            Debug.Log("About the thing.");
            Debug.Log(mre.LazyGetBuildPath());
        }
    }
}