using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.IO;
using Unity.VisualScripting.IonicZip;

namespace MultiRun
{
    public class SettingsUi : EditorWindow {
        /**
         */
        private class InstanceSettings {
            public TextField txtArguments;

            public InstanceSettings(VisualElement baseElement) {
                txtArguments = baseElement.Query<TextField>("CliArgs").First();
            }
        }



        // ---------------------------------------------------------------------
        // Start RunSettings
        // ---------------------------------------------------------------------
        private OsHelper osHelper = new OsHelper();
        public bool hasChanges = false;

        Toggle tglArrangeWindows;
        Toggle tglDisableLogStackTrace;
        TextField txtGlobalBuildPath;
        TextField txtProjectBuildPath;
        Button btnGlobalBuildPath;
        Button btnProjectBuildPath;
        Button btnApply;
        ScrollView scroll;
        Label lblBuildError;

        InstanceSettings allInstanceSettings;
        InstanceSettings[] instanceSettings = new InstanceSettings[4];


        public void CreateGUI() {
            VisualElement root = rootVisualElement;

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.bitwes.multirun/Editor/SettingsUi/SettingsUi.uxml");
            VisualElement uxmlElements = visualTree.Instantiate();
            root.Add(uxmlElements);

            SetupControls();
            LoadValues();
        }


        private void Update() {
            if(txtGlobalBuildPath.value == string.Empty && txtProjectBuildPath.value == string.Empty) {
                lblBuildError.visible = true;
                lblBuildError.text = "* You must have one build path set.";
            } else {
                lblBuildError.visible = false;
            }
        }


        private T FirstCtrl<T>(string name) where T : VisualElement {
            return rootVisualElement.Query<T>(name).First();
        }


        private void WireForChange(VisualElement elem) {
            if(elem is Toggle) {
                elem.RegisterCallback<ChangeEvent<bool>>(OnSomeValueChanged);
            } else if(elem is TextField) {
                elem.RegisterCallback<ChangeEvent<string>>(OnSomeValueChanged);
            }
        }


        private void LoadValues() {
            var mrsi = ProjectSettings.instance;

            // Build Settings
            txtGlobalBuildPath.value = MultiRunMenu.buildTools.buildPath;
            txtProjectBuildPath.value = mrsi.projectBuildPath;


            // Run Settings
            tglArrangeWindows.value = mrsi.arrangeWindows;
            tglDisableLogStackTrace.value = mrsi.disableLogStackTrace;
            allInstanceSettings.txtArguments.value = mrsi.allInstanceArgs;
            instanceSettings[0].txtArguments.value = mrsi.instanceArgs[0];
            instanceSettings[1].txtArguments.value = mrsi.instanceArgs[1];
            instanceSettings[2].txtArguments.value = mrsi.instanceArgs[2];
            instanceSettings[3].txtArguments.value = mrsi.instanceArgs[3];

            MarkUnchanged();
        }


        private void SetupControls()
        {            
            // General
            scroll = FirstCtrl<ScrollView>("MainScroll");

            btnApply = FirstCtrl<Button>("Apply");
            btnApply.clicked += OnApplyClicked;


            // Build Settings
            txtGlobalBuildPath = FirstCtrl<TextField>("GlobalBuildPath");
            WireForChange(txtGlobalBuildPath);

            btnGlobalBuildPath = FirstCtrl<Button>("BrowseGlobalBuildPath");
            btnGlobalBuildPath.clicked += OnBrowseGlobalBuildPathClicked;

            txtProjectBuildPath = FirstCtrl<TextField>("ProjectBuildPath");
            WireForChange(txtProjectBuildPath);
            
            btnProjectBuildPath = FirstCtrl<Button>("BrowsProjectBuildPath");
            btnProjectBuildPath.clicked += OnBrowseProjectBuildPathClicked;

            lblBuildError = FirstCtrl<Label>("BuildSettingError");


            // Run Settings
            tglArrangeWindows = FirstCtrl<Toggle>("ArrangeWindows");            
            WireForChange(tglArrangeWindows);

            tglDisableLogStackTrace = FirstCtrl<Toggle>("DisableStackTrace");            
            tglDisableLogStackTrace.RegisterCallback<ChangeEvent<bool>>(OnSomeValueChanged);

            allInstanceSettings = new InstanceSettings(FirstCtrl<VisualElement>("AllInstanceSettings"));
            WireForChange(allInstanceSettings.txtArguments);
            instanceSettings[0] = new InstanceSettings(FirstCtrl<VisualElement>("Instance1Settings"));
            WireForChange(instanceSettings[0].txtArguments);
            instanceSettings[1] = new InstanceSettings(FirstCtrl<VisualElement>("Instance2Settings"));
            WireForChange(instanceSettings[1].txtArguments);
            instanceSettings[2] = new InstanceSettings(FirstCtrl<VisualElement>("Instance3Settings"));
            WireForChange(instanceSettings[2].txtArguments);
            instanceSettings[3] = new InstanceSettings(FirstCtrl<VisualElement>("Instance4Settings"));
            WireForChange(instanceSettings[3].txtArguments);
        }


        private void MarkChanged() {
            btnApply.text = "* Apply";
            hasChanges = true;
        }


        private void MarkUnchanged() {
            btnApply.text = "Apply";
            hasChanges = false;
        }


        // -------------------------
        // Events
        // -------------------------
        private void OnLostFocus() {
            SaveIfChanged();
        }


        private void OnDestroy() {
            SaveIfChanged();
        }

        private void OnApplyClicked() {
            SaveChanges();
        }


        private void OnSomeValueChanged(ChangeEvent<bool> evt) {
            MarkChanged();
        }


        private void OnSomeValueChanged(ChangeEvent<string> evt) {
            MarkChanged();
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
                ProjectSettings.instance.projectBuildPath = result;
            }
        }


        // -------------------------
        // Public
        // -------------------------
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

            string toReturn = EditorUtility.SaveFilePanel("Set Build Path", curDir, curFile, ext);
            return toReturn;
        }


        public override void SaveChanges() {            
            MultiRunMenu.buildTools.buildPath = txtGlobalBuildPath.value;

            var mrsi = ProjectSettings.instance;
            mrsi.arrangeWindows = tglArrangeWindows.value;
            mrsi.disableLogStackTrace = tglDisableLogStackTrace.value;
            mrsi.projectBuildPath = txtProjectBuildPath.value;
            mrsi.allInstanceArgs = allInstanceSettings.txtArguments.value;
            mrsi.instanceArgs[0] = instanceSettings[0].txtArguments.value;
            mrsi.instanceArgs[1] = instanceSettings[1].txtArguments.value;
            mrsi.instanceArgs[2] = instanceSettings[2].txtArguments.value;
            mrsi.instanceArgs[3] = instanceSettings[3].txtArguments.value;

            ProjectSettings.instance.DoSave();
            MarkUnchanged();
            base.SaveChanges();
            MuRu.Log("Settings Saved");
        }

        public void SaveIfChanged() {
            if (hasChanges) {
                SaveChanges();
            }
        }
    }
}
