using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.IO;
using Unity.VisualScripting.IonicZip;

namespace Bitwesgames
{
    public class RunSettings : EditorWindow {
        private OsHelper osHelper = new OsHelper();

        Toggle tglArrangeWindows;
        Toggle tglDisableLogStackTrace;
        TextField txtGlobalBuildPath;
        TextField txtProjectBuildPath;
        Button btnGlobalBuildPath;
        Button btnProjectBuildPath;

        private T FirstCtrl<T>(string name) where T : VisualElement {
            return rootVisualElement.Query<T>(name).First();
        }


        public void CreateGUI() {
            VisualElement root = rootVisualElement;
            root.RegisterCallback<BlurEvent>(OnRootBlur);


            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.bitwesgames.multirun/Editor/RunSettings.uxml");
            VisualElement uxmlElements = visualTree.Instantiate();
            root.Add(uxmlElements);

            tglArrangeWindows = FirstCtrl<Toggle>("ArrangeWindows");
            tglArrangeWindows.value = MultiRunSettings.instance.arrangeWindows;
            tglArrangeWindows.RegisterCallback<ChangeEvent<bool>>(OnArrangeWindowsChanged);

            tglDisableLogStackTrace = FirstCtrl<Toggle>("DisableStackTrace");
            tglDisableLogStackTrace.value = MultiRunSettings.instance.disableLogStackTrace;
            tglDisableLogStackTrace.RegisterCallback<ChangeEvent<bool>>(OnDisableLogStackTraceChanged);

            txtGlobalBuildPath = FirstCtrl<TextField>("GlobalBuildPath");
            txtGlobalBuildPath.value = MultiRunMenu.mre.buildPath;

            btnGlobalBuildPath = FirstCtrl<Button>("BrowseGlobalBuildPath");
            btnGlobalBuildPath.clicked += OnBrowseGlobalBuildPathClicked;

            txtProjectBuildPath = FirstCtrl<TextField>("ProjectBuildPath");
            txtProjectBuildPath.value = MultiRunSettings.instance.projectBuildPath;

            btnProjectBuildPath = FirstCtrl<Button>("BrowsProjectBuildPath");
            btnProjectBuildPath.clicked += OnBrowseProjectBuildPathClicked;
        }

        private void OnRootBlur(BlurEvent evt){
            Debug.Log("Root blurred");
        }

        private void OnBrowseGlobalBuildPathClicked() {   
            string result = getFilePathFromUser(txtGlobalBuildPath.value);
            if(result != string.Empty) {
                txtGlobalBuildPath.value = result;
            }
        }


        private void OnBrowseProjectBuildPathClicked() {
            string result = getFilePathFromUser(txtProjectBuildPath.value);
            if (result != string.Empty) {
                txtProjectBuildPath.value = result;
                MultiRunSettings.instance.projectBuildPath = result;
            }
        }


        private void OnArrangeWindowsChanged(ChangeEvent<bool> evt) {            
            Save();
        }


        private void OnDisableLogStackTraceChanged(ChangeEvent<bool> evt) {             
            Save();
        }


        public bool ShouldArrangeWindows() {
            return tglArrangeWindows.value;
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
            string curDir = "";
            string curFile = "";
            string ext = osHelper.execExtension;

            if(startDir != string.Empty) {
                curFile = Path.GetFileName(startDir);
                curDir = startDir.Replace(curFile, "");
            }

            Debug.Log($"{startDir}::{curFile}::{curDir}");

            string toReturn = EditorUtility.SaveFilePanel("Set Build Path", curDir, curFile, ext);

            return toReturn;
        }


        public void Save() {
            Debug.Log("Saving");

            MultiRunMenu.mre.buildPath = txtGlobalBuildPath.value;

            MultiRunSettings.instance.arrangeWindows = tglArrangeWindows.value;
            MultiRunSettings.instance.disableLogStackTrace = tglDisableLogStackTrace.value;
            MultiRunSettings.instance.projectBuildPath = txtProjectBuildPath.value;
            MultiRunSettings.instance.DoSave();            
        }
    }
}
