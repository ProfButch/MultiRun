using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;


namespace MultiRun {
    public class LogDisplayTailer : LogDisplay {

        private class TailFile
        {
            // This has to be big enough that when the app quits it can hold all
            // the junk that Unity spits out.  Otherwise we don't always see the
            // EOF_delim.  We could get rid of this, but then we'd be reading the
            // entire file and that might have some consequences.  So this
            // stays for now, until I can get smarter about it.
            public int amountToRead = 1024 * 20;
            public int readIncrementSize = 1024 * 2;
            public string filePath = string.Empty;            

            private long endReadLoc = 0;
            private long lastFileSize = 0;

            
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
        private Font font;
        private Label lblEofDisclaimer;

        public LogDisplayTailer() : base() {            Init();        }        public LogDisplayTailer(VisualElement baseElement) : base(baseElement){
            Init();
        }

        public LogDisplayTailer(VisualElement baseElement, string path) : base(baseElement, path){
            Init();
        }


        private void Init() {
            maxTailBufferLength = 5 * maxStringSize;
            font = new Font("SourceCodePro-Regular");
        }


        private void AddDEofDisclaimerLabel()
        {
            if(lblEofDisclaimer == null)
            {
                lblEofDisclaimer = new Label();
                lblEofDisclaimer.text =  
                    "Application quit detected, tailing log stopped.\n" +
                    "There might be more output in the log file.";
                scrollView.Add(lblEofDisclaimer);
            }
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
            scrollView.Insert(0, lbl);
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
                    AddDEofDisclaimerLabel();
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
            lblEofDisclaimer = null;
        }
    }
}
