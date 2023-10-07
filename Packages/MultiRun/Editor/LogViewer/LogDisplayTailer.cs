using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Contexts;
using System.Text;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace MultiRun {
    public class LogDisplayTailer : MonoBehaviour {

        public int readSize = 1024 * 2;
        public int maxStringSize = 5000;
        public int endOfFileBytes = 1024 * 8;
        public string logPath = string.Empty;

        // -- Controls --
        public VisualElement root;
        public Label title;
        public Label logText;
        public ToolbarButton btnMaximize;
        private ScrollView scrollView;
        private Scroller vertScroll;
        private Scroller horizScroll;

        // -- Other vars --        
        private long lastFileSize = 0;
        private long lastReadLength = 0;
        private TextField theField;


        public LogDisplayTailer(VisualElement baseElement) {
            SetupControls(baseElement);            
        }


        public LogDisplayTailer(VisualElement baseElement, string path) {
            SetupControls(baseElement);
            logPath = path;
        }


        private void SetupControls(VisualElement baseElement)
        {
            root = baseElement;
            title = root.Query<Label>("Title").First();
            logText = root.Query<Label>("LogText").First();
            vertScroll = root.Query<ScrollView>().First().verticalScroller;
            horizScroll = root.Query<ScrollView>().First().horizontalScroller;
            btnMaximize = root.Query<ToolbarButton>("Maximize").First();
            scrollView = root.Query<ScrollView>().First();
            Clear();
        }


        private TextField AddLabel(string text) {
            var lbl = new TextField();
            lbl.isReadOnly = true;
            lbl.label = string.Empty;
            lbl.value = text;
            lbl.style.backgroundColor = new Color(0, 0, 0, 0);
            lbl.style.paddingBottom = 0;
            lbl.style.paddingTop = 0;
            lbl.style.paddingLeft = 0;
            lbl.style.paddingRight = 0;
            scrollView.Add(lbl);
            return lbl;
        }

        
        private void SetText(string text) {
            if (theField == null) {
                theField = AddLabel("");
            }

            theField.value = text;
        }


        private void UpdateTitle() {
            if (logPath == string.Empty) {
                title.text = "<no log file specified>";
            } else {
                title.text = $"{Path.GetFileName(logPath)}";
            }
        }



        /**
         * Brute force, reads the last `bytes` and puts it into the log display.
         */
        private void ReadEndOfFile(long bytes) {
            try {
                long fileSize = new FileInfo(logPath).Length;
                string allText = string.Empty;
                using (var fs = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                    long seekpoint = Math.Clamp(fileSize - bytes, 0, fileSize);
                    fs.Seek(seekpoint, SeekOrigin.Begin);
                    var buffer = new byte[readSize];
                    bool foundData = true;

                    while (foundData) {
                        var bytesRead = fs.Read(buffer, 0, buffer.Length);
                        foundData = bytesRead != 0;
                        if (foundData) {
                            allText += ASCIIEncoding.ASCII.GetString(buffer, 0, bytesRead);
                        }
                    }
                }

                SetText(allText);
                ScrollToBottom();
            } catch { }
        }


        public void ReadLog() {
            UpdateTitle();

            if (!File.Exists(logPath)) {
                SetText("\n\nFile not found\n\n");
                return;
            }

            long fileSize = new FileInfo(logPath).Length;

            if (fileSize < lastFileSize) {
                Clear();
                lastReadLength = 0;
            }

            if (fileSize > lastFileSize) {
                ReadEndOfFile(endOfFileBytes);
            }

            lastFileSize = fileSize;
        }


        public string GetText() {
            string toReturn = string.Empty;
            if (theField != null) {
                toReturn = theField.value;
            }
            return toReturn;
        }


        public void Clear() {
            scrollView.Clear();
            theField = null;
        }


        public void SetFontSize(float newSize) {
            float adjustedSize = Math.Clamp(newSize, 5.0f, 50.0f);
            scrollView.style.fontSize = adjustedSize;
        }


        public void IncrementFontSize(float howMuch) {
            SetFontSize(GetFontSize() + howMuch);
        }


        public float GetFontSize() {
            return scrollView.resolvedStyle.fontSize;
        }


        public void ScrollToBottom() {
            vertScroll.value = vertScroll.highValue;
            horizScroll.value = 0;
        }


        public void ScrollToTop() {
            vertScroll.value = 0;
            horizScroll.value = 0;
        }

    }
}
