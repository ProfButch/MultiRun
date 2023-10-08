using System;using System.Collections.Generic;using System.IO;using System.Text;using System.Threading;using UnityEditor.UIElements;using UnityEngine;using UnityEngine.UI;using UnityEngine.UIElements;namespace MultiRun{    public abstract class LogDisplay {        public int readSize = 1024 * 2;        public int maxStringSize = 5000;                public string logPath = string.Empty;        // -- Controls --        public VisualElement root;        public Label title;        public Label logText;        public ToolbarButton btnMaximize;        protected ScrollView scrollView;        protected Scroller vertScroll;        protected Scroller horizScroll;


        public LogDisplay() {}        public LogDisplay(VisualElement baseElement) {
            SetupControls(baseElement);            
        }


        public LogDisplay(VisualElement baseElement, string path) {
            SetupControls(baseElement);
            logPath = path;
        }


        // -----------------------------
        // Abstract
        // -----------------------------

        // Depending on what this instance is putting into the ScrollView,
        // getting the text back out will be specific to what you're doing.
        public abstract string GetText();



        // -----------------------------
        // Private
        // -----------------------------
        protected void SetupControls(VisualElement baseElement)
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
        protected void UpdateTitle() {            if(logPath == string.Empty){                title.text = "<no log file specified>";            } else {                title.text = $"{Path.GetFileName(logPath)}";            }        }        public void ScrollToBottom() {            vertScroll.value = vertScroll.highValue;            horizScroll.value = 0;        }        public void ScrollToTop() {            vertScroll.value = 0;            horizScroll.value = 0;        }        public virtual void Clear(){            scrollView.Clear();        }        public void SetFontSize(float newSize) {            float adjustedSize = Math.Clamp(newSize, 5.0f, 50.0f);            scrollView.style.fontSize = adjustedSize;        }        public void IncrementFontSize(float howMuch) {            SetFontSize(GetFontSize() + howMuch);        }        public float GetFontSize() {                        return scrollView.resolvedStyle.fontSize;        }    }}// ---------------------------------------------------------------------// Methods that would (now) be used in a version of the LogDisplay// that would display the entire file// ---------------------------------------------------------------------//public override GetText()//{//    var toReturn = "";//    var entryCount = 0;//    foreach(Label entry in scrollView.Children()){//        if(entryCount == 0){//            toReturn = entry.text;//        }else{//            toReturn += "\n" + entry.text;//        }//        entryCount += 1;//    }//    return toReturn;//}/** Adapted from https://stackoverflow.com/questions/3791103/c-sharp-continuously-read-file*///private void ReadToEnd(){//    try{//        long fileSize = new FileInfo(logPath).Length;//        if (fileSize > lastReadLength) {//            using (var fs = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {//                fs.Seek(lastReadLength, SeekOrigin.Begin);//                var buffer = new byte[readSize];//                bool foundData = true;//                while (foundData) {//                    var bytesRead = fs.Read(buffer, 0, buffer.Length);//                    lastReadLength += bytesRead;//                    foundData = bytesRead != 0;//                    if(foundData){//                        AddLine(ASCIIEncoding.ASCII.GetString(buffer, 0, bytesRead));//                    }//                }//            }//        }//    }//    catch { }//}//private int FindLastCharBefore(string source, string search, int cutOff){//    int result = source.IndexOf("\n");//    int toReturn = result;//    while(result != -1 && result < cutOff){//        toReturn = result;//        result = source.IndexOf("\n", result + 1);//    }//    return toReturn;//}//private void UpdateLogContents(){//    long fileSize = new FileInfo(logPath).Length;//    if(fileSize < lastFileSize){//        Clear();//        lastReadLength = 0;//    }//    if(fileSize > lastFileSize) {
//        ReadEndOfFile(endOfFileBytes);
//    }            //    lastFileSize = fileSize;//}//public void AddLine(string text) {//    if(text.Length > maxStringSize) {//        int lastNewline = FindLastCharBefore(text, "\n", maxStringSize);//        int splitAt = maxStringSize;//        int otherSplit = maxStringSize;//        if(lastNewline != -1){//            splitAt = lastNewline;//            otherSplit = lastNewline + 1;//        }//        string toAdd = text.Substring(0, splitAt);//        AddLabel(toAdd);//        string doAgain = text.Substring(otherSplit);//        AddLine(doAgain);//    } else {//        AddLabel(text);//    }//}