using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;
using MultiRun;

public class TestBuildTools
{

    public class TestMakeRunBuildCmd
    {
        [Test]
        public void TestLogfileOptionAddedAutomatically() {
            BuildTools bt = new BuildTools();
            string cmd = bt.MakeRunBuildCmd("/User/someone/theapp", "somelog.log", "");
            StringAssert.Contains("--logfile /User/someone/somelog.log", cmd);
        }

        [Test]
        public void TestWhenLogfileOptionExistsInArgsItIsNotAddedAutomactially() {
            BuildTools bt = new BuildTools();
            string cmd = bt.MakeRunBuildCmd("/User/someone/theapp", "somelog.log", "--logfile usethis");
            StringAssert.DoesNotContain("somelog.log", cmd);
        }
    }
}