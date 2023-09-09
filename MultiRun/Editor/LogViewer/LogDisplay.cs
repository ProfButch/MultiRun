using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;
using System.Threading;
using System.Text;


namespace com.bitwesgames
{
    public class LogDisplay {
        public VisualElement root;
        public Label title;
        public Label logText;
        public ToolbarButton btnMaximize;

        private ScrollView scrollView;
        private Scroller vertScroll;
        private Scroller horizScroll;

        private string logPath = string.Empty;
        private long lastFileSize = 0;
        private long lastReadLength = 0;

        public LogDisplay(VisualElement baseElement) {
            root = baseElement;
            title = root.Query<Label>("Title").First();
            logText = root.Query<Label>("LogText").First();
            vertScroll = root.Query<ScrollView>().First().verticalScroller;
            horizScroll = root.Query<ScrollView>().First().horizontalScroller;
            btnMaximize = root.Query<ToolbarButton>("Maximize").First();
            scrollView = root.Query<ScrollView>().First();
        }



        // All this didn't seem to make much difference (it does some) when
        // compared to making a label for each line.
        //
        // Maybe concatination is just as expensive as making a new lablel?
        // On 7k lines in one log this shaves off about 1 second.  With
        // 15k lines in two logs each, this shaves off maybe 5 seconds.
        private Label curLabel = null;
        private int lineLimit = 10;
        private int linesAdded = 0;
        private string textBuffer = string.Empty;
        private void AddLineToBuffer(string text) {
            if (curLabel == null || linesAdded > lineLimit) {
                FlushLineBuffer();
            }

            if (linesAdded == 0) {
                textBuffer = text;
            } else {
                textBuffer += $"\n{text}";
            }
            linesAdded += 1;
        }


        private void FlushLineBuffer() {
            if (textBuffer != string.Empty) {
                curLabel = new Label();
                AddLine(textBuffer);
                linesAdded = 0;
                textBuffer = string.Empty;
            }
        }


        private void AddLine(string text) {
            var lbl = new Label(text);
            scrollView.Add(lbl);
        }


        private void UpdateTitle() {
            if(logPath == string.Empty){
                title.text = "<no log file specified>";
            } else {
                title.text = $"{Path.GetFileName(logPath)}";
            }
        }

        public void ScrollToBottom() {
            vertScroll.value = vertScroll.highValue;
            horizScroll.value = 0;
        }


        public void ScrollToTop() {
            vertScroll.value = 0;
            horizScroll.value = 0;
        }



        public void TailFile(string filePath){
            logPath = filePath;
            scrollView.Clear();
            if (File.Exists(logPath)) {
                lastReadLength = 0;
                ReadToEndAndAddToDisplay();
            } else {
                AddLine("File not found");
            }
            UpdateTitle();
        }


        private string ReadToEnd(){
            string toReturn = "";
            try{
                long fileSize = new FileInfo(logPath).Length;
                if (fileSize > lastReadLength) {
                    using (var fs = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                        fs.Seek(lastReadLength, SeekOrigin.Begin);
                        var buffer = new byte[1024];

                        bool foundData = true;
                        while (foundData) {
                            var bytesRead = fs.Read(buffer, 0, buffer.Length);
                            lastReadLength += bytesRead;
                            foundData = bytesRead != 0;
                            if(foundData){
                                toReturn += ASCIIEncoding.ASCII.GetString(buffer, 0, bytesRead);
                            }
                        }
                    }
                }
            }
            catch { }

            return toReturn;
        }


        private void ReadToEndAndAddToDisplay(){
            long fileSize = new FileInfo(logPath).Length;
            if(fileSize < lastFileSize){
                scrollView.Clear();
                lastReadLength = 0;
            }
            lastFileSize = fileSize;

            string data = ReadToEnd();
            if(data.Length != 0){
                string[] lines = data.Split("\n");
                foreach (string line in lines){
                    AddLineToBuffer(line);
                }
                FlushLineBuffer();
                ScrollToBottom();
            }
        }


        public void CallMeInUpdate(){
            if(logPath != string.Empty && File.Exists(logPath)){
                ReadToEndAndAddToDisplay();
            } else{
                scrollView.Clear();
                UpdateTitle();
                AddLine("File not found");
            }
        }
    }
}