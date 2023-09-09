using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;

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

        private DateTime lastFileChangeTime;
        //private DateTime lastCreationTime;  REMEMBER, creation time was buggy on mac
        private string logPath = string.Empty;

        private int linesRead = 0;


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


        /**
         * This will read the entire file and add any lines that do not exist
         * already to the scrollView.  To refresh the contents completely
         * call scrollView.Clear() BEFORE calling this.
         */
        private void LoadFileAsLabels(string path) {
            IEnumerable<string> lines = File.ReadLines(path);
            var line_count = 0;
            var cur_line_count = scrollView.childCount;
            linesRead = 0;
            foreach (string line in lines) {
                line_count += 1;
                linesRead += 1;
                if (line_count > cur_line_count) {
                    AddLineToBuffer(line);
                }
            }
            FlushLineBuffer();
            UpdateTimestamps();
            UpdateTitle();
        }


        private void UpdateTitle() {
            string changeTimeDisplay = lastFileChangeTime.ToLocalTime().ToString();
            if (lastFileChangeTime == DateTime.MinValue) {
                changeTimeDisplay = "File not found";
            }
            title.text = $"{Path.GetFileName(logPath)} ({changeTimeDisplay}) {linesRead} lines";
        }

        private void UpdateTimestamps() {
            lastFileChangeTime = File.GetLastWriteTimeUtc(logPath);

        }


        /**
         * Clears the text and loads a file
         */
        public void LoadLog(string path) {
            logPath = path;

            scrollView.Clear();
            if (File.Exists(path)) {
                LoadFileAsLabels(path);
            } else {
                AddLine("File not found");
                UpdateTitle();
            }
            ScrollToBottom();
        }


        public void ScrollToBottom() {
            vertScroll.value = vertScroll.highValue;
            horizScroll.value = 0;
        }


        public void ScrollToTop() {
            vertScroll.value = 0;
            horizScroll.value = 0;
        }


        public bool HasLogFileChanged() {
            bool toReturn = false;
            if (logPath != string.Empty && File.Exists(logPath)) {
                if (lastFileChangeTime == null) {
                    toReturn = true;
                } else {
                    toReturn = File.GetLastWriteTimeUtc(logPath) != lastFileChangeTime;
                }
            }

            return toReturn;
        }


        /**
         * Only clears the text if the creation time is different.
         */
        public void RefreshLog() {
            if (HasLogFileChanged()) {
                // Could not get good data for the creation time of a file so
                // we reload the whole thing if it's been 5 seconds or more
                // since the last time we loaded any of the file.  This
                // stops excessive Clear calls during AutoRefresh.
                TimeSpan minTime = new TimeSpan(0, 0, 5);
                TimeSpan diff = File.GetLastWriteTimeUtc(logPath).Subtract(lastFileChangeTime);
                if (diff > minTime) {
                    scrollView.Clear();
                }
                LoadFileAsLabels(logPath);
                ScrollToBottom();
            }
        }
    }
}