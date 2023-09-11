using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;


namespace Bitwesgames
{
    public class LogDisplay {
        public int readSize = 1024;
        public int maxStringSize = 10000;

        // -- Controls --
        public VisualElement root;
        public Label title;
        public Label logText;
        public ToolbarButton btnMaximize;
        private ScrollView scrollView;
        private Scroller vertScroll;
        private Scroller horizScroll;

        // -- Other vars --
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

            Clear();
        }


        private Label AddLabel(string text){
            var lbl = new Label(text);
            //lbl.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            scrollView.Add(lbl);
            return lbl;
        }


        private void UpdateTitle() {
            if(logPath == string.Empty){
                title.text = "<no log file specified>";
            } else {
                title.text = $"{Path.GetFileName(logPath)}";
            }
        }


        /*
         * Adapted from https://stackoverflow.com/questions/3791103/c-sharp-continuously-read-file
         */
        private void ReadToEnd(){
            try{
                long fileSize = new FileInfo(logPath).Length;
                if (fileSize > lastReadLength) {
                    using (var fs = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                        fs.Seek(lastReadLength, SeekOrigin.Begin);
                        var buffer = new byte[readSize];

                        bool foundData = true;
                        while (foundData) {
                            var bytesRead = fs.Read(buffer, 0, buffer.Length);
                            lastReadLength += bytesRead;
                            foundData = bytesRead != 0;
                            if(foundData){
                                AddLine(ASCIIEncoding.ASCII.GetString(buffer, 0, bytesRead));
                            }
                        }
                    }
                }
            }
            catch { }
        }


        private int FindLastCharBefore(string source, string search, int cutOff){
            int result = source.IndexOf("\n");
            int toReturn = result;
            while(result != -1 && result < cutOff){
                toReturn = result;
                result = source.IndexOf("\n", result + 1);
            }
            return toReturn;
        }


        private void UpdateLogContents(){
            long fileSize = new FileInfo(logPath).Length;
            if(fileSize < lastFileSize){
                Clear();
                lastReadLength = 0;
            }
            lastFileSize = fileSize;
            ReadToEnd();
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
            Clear();
            if (File.Exists(logPath)) {
                lastReadLength = 0;
                UpdateLogContents();
            } else {
                AddLine("\n\nFile not found\n\n");
            }
            UpdateTitle();
        }


        public void CallMeInUpdate(){
            if(logPath != string.Empty && File.Exists(logPath)){
                UpdateLogContents();
            } else{
                Clear();
                UpdateTitle();
                AddLine("\n\nFile not found\n\n");
            }
        }


        public string GetText(){
            var toReturn = "";
            var entryCount = 0;
            foreach(Label entry in scrollView.Children()){
                if(entryCount == 0){
                    toReturn = entry.text;
                }else{
                    toReturn += "\n" + entry.text;
                }
                entryCount += 1;
            }
            return toReturn;
        }


        public void Clear(){
            scrollView.Clear();
        }


        public void AddLine(string text) {
            if(text.Length > maxStringSize) {
                int lastNewline = FindLastCharBefore(text, "\n", maxStringSize);
                int splitAt = maxStringSize;
                int otherSplit = maxStringSize;

                if(lastNewline != -1){
                    splitAt = lastNewline;
                    otherSplit = lastNewline + 1;
                }
                string toAdd = text.Substring(0, splitAt);
                AddLabel(toAdd);

                string doAgain = text.Substring(otherSplit);
                AddLine(doAgain);
            } else {
                AddLabel(text);
            }
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
    }
}