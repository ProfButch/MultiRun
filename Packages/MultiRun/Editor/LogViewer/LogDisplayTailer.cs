using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Contexts;
using System.Text;
using UnityEditor.MPE;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


namespace MultiRun {
    public class LogDisplayTailer : LogDisplay {

        public int endOfFileBytes = 1024 * 10;

        // -- Other vars --        
        private long lastFileSize = 0;
        private long lastReadLength = 0;
        private TextField theField;


        public LogDisplayTailer() {}        public LogDisplayTailer(VisualElement baseElement) : base(baseElement){}
        public LogDisplayTailer(VisualElement baseElement, string path) : base(baseElement, path){ }


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


        // -----------------------------
        // Public
        // -----------------------------
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


        public override string GetText() {
            string toReturn = string.Empty;
            if (theField != null) {
                toReturn = theField.value;
            }
            return toReturn;
        }


        public override void Clear() {
            base.Clear();
            theField = null;
        }
    }
}
