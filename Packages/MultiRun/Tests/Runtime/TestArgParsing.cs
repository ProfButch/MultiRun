using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using MultiRun;
using MultiRun.Cli;


public class TestArgParsing {

    [Test]
    public void TestDefaultValues() {
        var p = new Parser();
        Assert.AreEqual(false, p.disableDebugLogStackTrace, "disable log trace");
        Assert.AreEqual(WindowPositioner.ARRANGE_NONE, p.windowArrangement, "window arrange");
    }

    [Test]
    public void TestDisableDebugLogStackTrace() {
        var p = new Parser();
        p.Parse($"{ArgDef.ArgName(ArgDef.ARG_DISABLE_LOG_STACK_TRACE)}");
        Assert.IsTrue(p.disableDebugLogStackTrace);
    }
}