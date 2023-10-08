using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;
using MultiRun;




public class TestLogDisplay
{

    public class TestLogDisplayBase
    {
        // Testable version of LogDisplay.
        private class LogDisplayBase : LogDisplay
        {
            public LogDisplayBase() {}
            public LogDisplayBase(VisualElement baseElement) : base(baseElement){}
            public LogDisplayBase(VisualElement baseElement, string path) : base(baseElement, path){ }

            public override string GetText() {
                return string.Empty;
            }
        }


        private LogDisplay NewLogDisplay() {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Packages/com.bitwes.MultiRun/Editor/LogViewer/LogDisplay.uxml");
            VisualElement uxmlElements = visualTree.Instantiate();
            var toReturn = new LogDisplayBase(uxmlElements);
            return toReturn;
        }


        [Test]
        public void TestCanMakeOne()
        {
            var ld = NewLogDisplay();
            Assert.NotNull(ld, "the thing itself");
            Assert.NotNull(ld.root, "root");
            Assert.NotNull(ld.btnMaximize, "btnMaximize");
        }
    }



    public class TestLogDisplayTailer
    {
        private LogDisplay NewLogDisplay()
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Packages/com.bitwes.MultiRun/Editor/LogViewer/LogDisplay.uxml");
            VisualElement uxmlElements = visualTree.Instantiate();
            var toReturn = new LogDisplayTailer(uxmlElements);
            return toReturn;
        }


        [Test]
        public void TestCanMakeOne()
        {
            var ld = NewLogDisplay();
            Assert.NotNull(ld, "the thing itself");
            Assert.NotNull(ld.root, "root");
            Assert.NotNull(ld.btnMaximize, "btnMaximize");
        }
    }




    //[Test]
    //public void TestAddLineIsInGetText()
    //{
    //    var ld = NewLogDisplay();
    //    ld.AddLine("hello there");
    //    Assert.AreEqual(ld.GetText(), "hello there");
    //}

    //[Test]
    //public void TestAdd2LineIsInGetText()
    //{
    //    var ld = NewLogDisplay();
    //    ld.AddLine("hello");
    //    ld.AddLine("there");
    //    Assert.AreEqual(ld.GetText(), "hello\nthere");
    //}

    //[Test]
    //public void TestAddLineSplitsAtMaxStringSize()
    //{
    //    var ld = NewLogDisplay();
    //    ld.maxStringSize = 100;
    //    string s = new string('a', 101);
    //    string expected = new string('a', 100) + "\na";
    //    ld.AddLine(s);
    //    Assert.AreEqual(ld.GetText(), expected);
    //}

    //[Test]
    //public void TestWhenExceedsMaxStringSizeClosestNewLineUsedForSplit()
    //{
    //    var ld = NewLogDisplay();
    //    ld.maxStringSize = 10;

    //    ld.AddLine("123456\n7890123");
    //    Assert.AreEqual("123456\n7890123", ld.GetText());
    //}

}
