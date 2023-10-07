using System;using System.Collections.Generic;using System.IO;using System.Threading.Tasks;using Unity.VisualScripting;using UnityEditor;using UnityEditor.Callbacks;using UnityEditor.Graphs;using UnityEditor.UIElements;using UnityEngine;using UnityEngine.UIElements;namespace MultiRun{    public class LogViewer : EditorWindow    {        /**         * Logic for the HSplit containers that have the two LogDisplays in         * them.         */        private class LogSplit        {            public TwoPaneSplitView root;            public LogDisplayTailer leftLog;            public LogDisplayTailer rightLog;            public VisualElement dragLine;                       public LogSplit(TwoPaneSplitView baseElement)            {                root = baseElement;                leftLog = new LogDisplayTailer(root.Query<VisualElement>("LeftLog").First());                rightLog = new LogDisplayTailer(root.Query<VisualElement>("RightLog").First());                dragLine = root.Query<VisualElement>("unity-dragline-anchor").First();                           }            public void showLog(LogDisplayTailer which, bool should)            {                root.UnCollapse();                which.root.visible = should;                if (!leftLog.root.visible) {                    root.CollapseChild(0);                }                if (!rightLog.root.visible) {                    root.CollapseChild(1);                }            }            public void showOnlyLog(LogDisplayTailer which) {                LogDisplayTailer other = leftLog;                if(which == leftLog) {                    other = rightLog;                }                showLog(which, true);                showLog(other, false);            }            public bool AreAllLogsHidden() {                return !leftLog.root.visible && !rightLog.root.visible;            }            public bool IsALogHidden()            {                return !AreAllLogsVisible();            }            public bool AreAllLogsVisible()            {                return leftLog.root.visible &&                    rightLog.root.visible;            }            public void ReadLogs(){                leftLog.ReadLog();                rightLog.ReadLog();            }        }        [DidReloadScripts]        private static void OnReloadScripts()        {            if (EditorWindow.HasOpenInstances<LogViewer>())            {                var window = EditorWindow.GetWindow<LogViewer>();                // Since it goes all wacky when scripts are reloaded and I have                // grown tired trying to make it act right, we just close the                // window after scripts have been reloaded.                window.Close();                // ------------------------------------------------------------                // This all resulted in a HotControl.  Not sure what that is,                 // but you couldn't interact with it.  It did look right though,                // so I'm leaving it in here.                // ------------------------------------------------------------                //int maxWait = 60;                //int i = 0;                //bool ctrlsLoading = true;                //while(i < maxWait && ctrlsLoading) {                 //    await Task.Yield();                //    i += 1;                //    if(window.mainSplit != null) {                //        ctrlsLoading = window.mainSplit.fixedPane == null;                //    }                //}                //Debug.Log($"Waited {i}/{maxWait} frames");                //window.DefaultLayout();                //if (window.basePath != string.Empty) {                //    window.LoadLogs();                //}            }        }        public TwoPaneSplitView mainSplit;        private VisualElement mainSplitDragLine;        private Label lblInfo;        private LogSplit topSplit;        private LogSplit botSplit;        private Toolbar toolbar;        private ToolbarToggle[] showLogButtons = new ToolbarToggle[4];        private ToolbarToggle tglAutoRefresh;        private ToolbarButton btnRefresh;        private ToolbarButton btnIncreaseFontSize;        private ToolbarButton btnDecreaseFontSize;        private ToolbarButton btnRestoreLayout;        private bool autoRefresh = true;        private float refreshInterval = .5f;        private float timeSinceLastCheck = 0.0f;        private string _basePath = string.Empty;        public string basePath {            get {                return _basePath;            }            set {                ProjectSettings.instance.lastLoadedLogPath = value;                _basePath = value;            }        }                public void CreateGUI() {            VisualElement root = rootVisualElement;            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.bitwes.MultiRun/Editor/LogViewer/LogViewer.uxml");            VisualElement uxmlElements = visualTree.Instantiate();            root.Add(uxmlElements);            //var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/IT4080/Editor/LogViewer.uss");            root.RegisterCallback<GeometryChangedEvent>(OnRootResized);            SetupControls();        }        private void InitialLayout() {            MuRu.Log("LogViewer InitialLayout");            mainSplit.fixedPaneInitialDimension = mainSplit.resolvedStyle.height / 2.0f;            topSplit.root.fixedPaneInitialDimension = topSplit.root.resolvedStyle.width / 2.0f;            botSplit.root.fixedPaneInitialDimension = botSplit.root.resolvedStyle.width / 2.0f;            topSplit.root.fixedPane.RegisterCallback<GeometryChangedEvent>(OnTopLeftResized);            botSplit.root.fixedPane.RegisterCallback<GeometryChangedEvent>(OnBotLeftResized);        }        public async void DefaultLayout()        {            if(topSplit.IsALogHidden() || botSplit.IsALogHidden()) {                showLogButtons[0].value = true;                showLogButtons[1].value = true;                showLogButtons[2].value = true;                showLogButtons[3].value = true;                for(int i = 0; i < 5; i++) {                    await Task.Yield();                }                            }            mainSplit.style.height = rootVisualElement.resolvedStyle.height;                      float halfWidth = topSplit.root.resolvedStyle.width / 2.0f;            float halfHeight = mainSplit.resolvedStyle.height / 2;            mainSplit.fixedPane.style.height = halfHeight;            // I couldn't get the dragLine for the mainSplit to play nicely.            // It wouldn't follow the height of the pane.  I did this and it            // appears to work.  This thing is the jankest of janks.            mainSplit.CollapseChild(0);            mainSplit.UnCollapse();            topSplit.root.fixedPane.style.width = halfWidth;            topSplit.dragLine.style.left = halfWidth;            MatchVSplit(botSplit, topSplit);        }        private bool _is_first_update_call = true;        private bool _should_scroll_to_bottom = false;        public void Update() {            // I could not figure out what event was the first event where all            // the controls have been fully instanced and sized.  Calling            // InitialLayout anywhere else always resulted in the various sizes            // (sytle.width, resolvedStyle.width, contentRect.size.x) being NaN.            if (_is_first_update_call) {                InitialLayout();                _is_first_update_call = false;            }            if (_should_scroll_to_bottom) {                topSplit.leftLog.ScrollToBottom();                topSplit.rightLog.ScrollToBottom();                _should_scroll_to_bottom = false;            }            HandleAutoRefresh();        }        // ----------------------        // Private        // ----------------------        private void HandleAutoRefresh()        {            if (!autoRefresh) {                return;            }            timeSinceLastCheck += Time.deltaTime;            if(timeSinceLastCheck >= refreshInterval)            {                RefreshLogs();                timeSinceLastCheck = 0.0f;            }        }        private void SetupLogToggle(string toggleName, LogSplit split, LogDisplayTailer disp)        {            ToolbarToggle logToggle = rootVisualElement.Query<ToolbarToggle>(toggleName).First();            logToggle.RegisterValueChangedCallback((changeEvent) => OnLogToggleToggled(split, disp, changeEvent));        }        private void SetupMaximizeButton(LogDisplayTailer disp, int showLogButtonIndex) {            disp.btnMaximize.clicked += () => OnMaximizeButtonClicked(disp, showLogButtons[showLogButtonIndex]);        }        private void SetupControls()        {            MuRu.Log("LogViewer SetupControls");            mainSplit = rootVisualElement.Query<TwoPaneSplitView>("FourLogs");            mainSplitDragLine = mainSplit.Query<VisualElement>("unity-dragline-anchor").First();            toolbar = rootVisualElement.Query<Toolbar>().First();            topSplit = new LogSplit(rootVisualElement.Query<TwoPaneSplitView>("LogSplit1").First());            botSplit = new LogSplit(rootVisualElement.Query<TwoPaneSplitView>("LogSplit2").First());            SetupLogToggle("ShowLog1", topSplit, topSplit.leftLog);            SetupLogToggle("ShowLog2", topSplit, topSplit.rightLog);            SetupLogToggle("ShowLog3", botSplit, botSplit.leftLog);            SetupLogToggle("ShowLog4", botSplit, botSplit.rightLog);            btnRefresh = rootVisualElement.Query<ToolbarButton>("Refresh");            btnRefresh.clicked += OnRefreshPressed;            tglAutoRefresh = rootVisualElement.Query<ToolbarToggle>("AutoRefresh").First();            tglAutoRefresh.value = autoRefresh;            btnRefresh.SetEnabled(!tglAutoRefresh.value);            tglAutoRefresh.RegisterValueChangedCallback(OnAutoRefreshToggled);            lblInfo = rootVisualElement.Query<Label>("Info").First();            showLogButtons[0] = rootVisualElement.Query<ToolbarToggle>("ShowLog1").First();            showLogButtons[1] = rootVisualElement.Query<ToolbarToggle>("ShowLog2").First();            showLogButtons[2] = rootVisualElement.Query<ToolbarToggle>("ShowLog3").First();            showLogButtons[3] = rootVisualElement.Query<ToolbarToggle>("ShowLog4").First();            btnRestoreLayout =  toolbar.Query<ToolbarButton>("RestoreLayout").First();            btnRestoreLayout.clicked += OnRestoreLayoutClicked;            btnDecreaseFontSize = toolbar.Query<ToolbarButton>("FontSizeDown").First();            btnDecreaseFontSize.clicked += OnDecreaseFontSizePressed;            btnIncreaseFontSize = toolbar.Query<ToolbarButton>("FontSizeUp").First();            btnIncreaseFontSize.clicked += OnIncreaseFontSizePressed;            SetupMaximizeButton(topSplit.leftLog, 0);            SetupMaximizeButton(topSplit.rightLog, 1);            SetupMaximizeButton(botSplit.leftLog, 2);            SetupMaximizeButton(botSplit.rightLog, 3);        }        private void ShowSingleLog(LogDisplayTailer whichLog, ToolbarToggle whichToggle)        {            foreach (ToolbarToggle b in showLogButtons){                b.value = b == whichToggle;            }            mainSplit.UnCollapse();            LogSplit parentSplit;            if(whichLog == topSplit.leftLog || whichLog == topSplit.rightLog)            {                mainSplit.CollapseChild(1);                parentSplit = topSplit;            }            else            {                mainSplit.CollapseChild(0);                parentSplit = botSplit;            }            parentSplit.showOnlyLog(whichLog);        }        private void MatchVSplit(LogSplit matchThis, LogSplit toThis)        {            var newWidth = toThis.root.fixedPane.style.width;            matchThis.root.fixedPane.style.width = newWidth;            matchThis.dragLine.style.left = newWidth;        }        // ----------------------        // Events        // ----------------------        private void OnTopLeftResized(GeometryChangedEvent e) {            if (botSplit.AreAllLogsVisible() && topSplit.AreAllLogsVisible()) {                MatchVSplit(botSplit, topSplit);            }        }        private void OnBotLeftResized(GeometryChangedEvent e)        {            if (topSplit.AreAllLogsVisible() && botSplit.AreAllLogsVisible()) {                MatchVSplit(topSplit, botSplit);            }        }        private void OnRootResized(GeometryChangedEvent e)        {            // I hate UI Builder.  For some reason, everything looks and acts            // just fine in the editor, but when run logsBaseElement always            // has a height of 0 (unless hardcoded to be different).  After too            // much fighting this is the solution.  Also, HOORAY, yet another way            // to connect to a signal in C#.            mainSplit.style.width = e.newRect.size.x;            // I should be able to use mainSplit.<something>.position.y, but I            // couldn't figure out what <something> should be.  So here's a some            // tech debt for you:  This will be wrong if things move and the            // toolbar is not just above the mainSplit.  BTW, what if the actual            // ... is resolvedSytle.position?            mainSplit.style.height = e.newRect.size.y - toolbar.resolvedStyle.height - toolbar.contentRect.position.y;        }        private void OnMaximizeButtonClicked(LogDisplayTailer disp, ToolbarToggle button) {            ShowSingleLog(disp, button);        }        private void OnLogToggleToggled(LogSplit split, LogDisplayTailer disp, ChangeEvent<bool> changeEvent) {            split.showLog(disp, changeEvent.newValue);            mainSplit.UnCollapse();            if (topSplit.AreAllLogsHidden()) {                mainSplit.CollapseChild(0);            }            if (botSplit.AreAllLogsHidden()) {                mainSplit.CollapseChild(1);            }        }        private void OnRefreshPressed()        {            RefreshLogs();            btnRefresh.Focus();        }        private void OnAutoRefreshToggled(ChangeEvent<bool> changeEvent)        {            btnRefresh.SetEnabled(!changeEvent.newValue);            autoRefresh = changeEvent.newValue;        }        private void OnIncreaseFontSizePressed()        {            IncrementFontSizes(1.0f);        }        private void OnDecreaseFontSizePressed()        {            IncrementFontSizes(-1.0f);        }        private void OnRestoreLayoutClicked()        {            DefaultLayout();        }        // ----------------------        // Public        // ----------------------        public void IncrementFontSizes(float howMuch) {            topSplit.leftLog.IncrementFontSize(howMuch);            topSplit.rightLog.IncrementFontSize(howMuch);            botSplit.leftLog.IncrementFontSize(howMuch);            botSplit.rightLog.IncrementFontSize(howMuch);        }        public void LoadLogs() {            // Even w/o rich text labels still doesn't handle backslashes the            // way that it should            lblInfo.text = _basePath.Replace("\\", "\\\\");            topSplit.leftLog.logPath = $"{basePath}_1.log";            topSplit.rightLog.logPath = $"{basePath}_2.log";            botSplit.leftLog.logPath = $"{basePath}_3.log";            botSplit.rightLog.logPath = $"{basePath}_4.log";            _should_scroll_to_bottom = true;        }        public void RefreshLogs() {            topSplit.ReadLogs();            botSplit.ReadLogs();        }    }}