using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Xml;
using UnityEditor.MPE;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


namespace MultiRun {
    public class LogDisplayTailer : LogDisplay {

        private class TailFile
        {            
            // 10 seems to be about the limit.  Ran into excess vertex errors at 15.
            public int amountToRead = 1024 * 10;
            public int readIncrementSize = 1024 * 2;
            public string filePath = string.Empty;            

            private long endReadLoc = 0;
            private long lastFileSize = 0;

            
            /**
             * Brute force, reads the last `bytes` and puts it into the log display.
             */
            private string DoRead(long fileSize) {
                string toReturn = string.Empty;
                try {
                    long seekTo = Math.Max(fileSize - amountToRead, 0);
                    seekTo = Math.Max(seekTo, endReadLoc);

                    using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                        fs.Seek(seekTo, SeekOrigin.Begin);
                        var buffer = new byte[readIncrementSize];
                        bool foundData = true;
                        
                        while (foundData) {
                            var bytesRead = fs.Read(buffer, 0, buffer.Length);
                            foundData = bytesRead != 0;
                            endReadLoc += bytesRead;

                            if (foundData) {
                                toReturn += ASCIIEncoding.ASCII.GetString(buffer, 0, bytesRead);
                            }
                        }
                    }
                } catch { }
                return toReturn;
            }


            public string ReadEndOfFile() {
                string toReturn = string.Empty;
                long fileSize = new FileInfo(filePath).Length;

                if(fileSize < lastFileSize) {
                    Reset();
                }                

                if(fileSize > lastFileSize) {
                    toReturn = DoRead(fileSize);
                }
                lastFileSize = fileSize;

                return toReturn;
            }


            public void Reset() {
                lastFileSize = 0;
                endReadLoc = 0;
            }
        }


        public string ignoreAllAfterDelimiter = string.Empty;
        private bool encounteredIgnoreAfterDelim = false;

        private int maxTailBufferLength = 0;
        private long lastFileSize = 0;
        private string tailBuffer = string.Empty;
        private TextField theField;
        private TailFile tailFile = new TailFile();


        public LogDisplayTailer() : base() {            Init();        }        public LogDisplayTailer(VisualElement baseElement) : base(baseElement){
            Init();
        }

        public LogDisplayTailer(VisualElement baseElement, string path) : base(baseElement, path){
            Init();
        }


        private void Init() {
            maxTailBufferLength = 5 * maxStringSize;
        }


        // --------------------------------------
        // Private
        // --------------------------------------
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


        // -----------------------------
        // Public
        // -----------------------------

        // Adds text to the tailBuffer, truncates tailBuffer occasionally, sends
        // maxStringSize chars from the end of tailBuffer to SetText().
        public void AddTailText(string text) {
            if (encounteredIgnoreAfterDelim) {
                return;
            }

            tailBuffer += text;

            if(ignoreAllAfterDelimiter != string.Empty) {                
                int idx = tailBuffer.IndexOf(ignoreAllAfterDelimiter);
                if(idx != -1) {
                    encounteredIgnoreAfterDelim = true;
                    tailBuffer = tailBuffer.Substring(0, idx);
                }
            }

            // This might be too smart, but don't truncate the buffer until it
            // reaches 2x the size, just to cut down on substrings. Probably
            // premature optimization.
            int truncateLength = 2 * maxTailBufferLength;
            if(tailBuffer.Length > truncateLength) {
                tailBuffer = tailBuffer.Substring(tailBuffer.Length - maxTailBufferLength);
            }

            string dispText = string.Empty;
            if(tailBuffer.Length > maxStringSize) {
                SetText(tailBuffer.Substring(tailBuffer.Length - maxStringSize));
            } else {
                SetText(tailBuffer);
            }
        }


        public void ReadLog() {
            UpdateTitle();

            if (!File.Exists(logPath)) {
                SetText("\n\nFile not found\n\n");
                return;
            }

            long fileSize = new FileInfo(logPath).Length;
            if(fileSize < lastFileSize) {
                Clear();
            }
            lastFileSize = fileSize;

            tailFile.filePath = logPath;

            string result = tailFile.ReadEndOfFile();
            AddTailText(result);
            if(result.Length > 0) {
                ScrollToBottom();                
            }            
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
            tailBuffer = string.Empty;
            lastFileSize = 0;
            encounteredIgnoreAfterDelim = false;
        }
    }
}
