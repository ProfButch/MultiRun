using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;



namespace MultiRun {

    [InitializeOnLoadAttribute]
    public static class MultiRunMenu {

        public static BuildTools buildTools = new BuildTools();

        private static void ShowLogs() {
            var window = EditorWindow.GetWindow<LogViewer>("Log Watcher");
            window.basePath = Path.Join(Path.GetDirectoryName(buildTools.GetBuildPath()), Path.GetFileNameWithoutExtension(buildTools.GetBuildPath()));
            window.ShowPopup();
            window.LoadLogs();
        }


        // At least on Mac, moving the cursor from the window to a menu does not
        // trigger the loss of focus until after the menu item has been pressed,
        // so we have to "save if open" in most menu items.
        private static void SaveSettingsIfOpen() {
            if (EditorWindow.HasOpenInstances<MultiRun.SettingsUi>()) {
                EditorWindow.GetWindow<MultiRun.SettingsUi>().SaveIfChanged();
            }
        }


        private static bool IsBuildPathValid() {
            return buildTools.GetBuildPath() != string.Empty;
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
            var window = EditorWindow.GetWindow<MultiRun.SettingsUi>();
            window.titleContent.text = "Multi Run Settings";
            window.ShowPopup();
        }


        //--------------------------------
        // Section 100
        //--------------------------------
        [MenuItem("MultiRun/Build", false, 100)]
        private static void MnuBuild() {
            SaveSettingsIfOpen();
            string curPath = buildTools.GetBuildPath();
            // This handles the one case where it wasn't set and the user canceled
            // the dialog to set the build path.
            if (curPath != string.Empty) {
                buildTools.Build(curPath);
            }
        }
        

        [MenuItem("MultiRun/Build Current Scene", false, 100)]
        private static void MnuBuildCurrentScene() {
            SaveSettingsIfOpen();
            string curPath = buildTools.GetBuildPath();
            // This handles the one case where it wasn't set and the user canceled
            // the dialog to set the build path.
            if (curPath != string.Empty) {
                buildTools.Build(curPath, true);
            }
        }


        //--------------------------------
        // Section 200
        //--------------------------------
        [MenuItem("MultiRun/Build & Run/1", false, 200)]
        private static void MnuBuildRun1() {
            SaveSettingsIfOpen();
            buildTools.BuildThenRunX(1);
        }


        [MenuItem("MultiRun/Build & Run/2", false, 200)]
        private static void MnuBuildRun2() {
            SaveSettingsIfOpen();
            buildTools.BuildThenRunX(2);
        }


        [MenuItem("MultiRun/Build & Run/3", false, 200)]
        private static void MnuBuildRun3() {
            SaveSettingsIfOpen();
            buildTools.BuildThenRunX(3);
        }


        [MenuItem("MultiRun/Build & Run/4", false, 200)]
        private static void MnuBuildRun4(){
            SaveSettingsIfOpen();
            buildTools.BuildThenRunX(4);
        }


        [MenuItem("MultiRun/Run/1", false, 200)]
        private static void MnuRun1() {
            SaveSettingsIfOpen();
            buildTools.RunBuildXTimes(1);
        }


        [MenuItem("MultiRun/Run/2", false, 200)]
        private static void MnuRun2() {
            SaveSettingsIfOpen();
            buildTools.RunBuildXTimes(2);
        }


        [MenuItem("MultiRun/Run/3", false, 200)]
        private static void MnuRun3() {
            SaveSettingsIfOpen();
            buildTools.RunBuildXTimes(3);
        }


        [MenuItem("MultiRun/Run/4", false, 200)]
        private static void MnuRun4() {
            SaveSettingsIfOpen();
            buildTools.RunBuildXTimes(4);
        }


        //--------------------------------
        // Section 999
        //--------------------------------
        [MenuItem("MultiRun/Log Watcher (alpha)", false, 999)]
        private static void MnuViewLogs() {
            SaveSettingsIfOpen();
            ShowLogs();
        }


        [MenuItem("MultiRun/Open Build Directory", false, 999)]
        private static void MnuViewFiles() {
            SaveSettingsIfOpen();
            string parentDir = Path.GetDirectoryName(buildTools.GetBuildPath());
            if (Directory.Exists(parentDir)) {
                EditorUtility.RevealInFinder(buildTools.GetBuildPath());
            } else {
                MuRu.LogError($"Cannot open {parentDir} because it does not exist.  Kicking off a build will create the path.");
            }
        }
    }
}