using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using MultiRun;
using MultiRun.Cli;

public class TestCliArgGeneration
{
    private void AssertHasArgValue(string args, string argName, string argValue, string msg ="")
    {
        string expected = $"{ArgDef.ArgName(argName)} {argValue}";
        StringAssert.Contains(expected, args, msg);
    }

    private void AssertHasArgFlag(string args, string argName, string msg = "") {
        string expected = $"{ArgDef.ArgName(argName)}";
        StringAssert.Contains(expected, args, $"{msg} has argument");
        StringAssert.DoesNotContain($"{expected} True", args, $"{msg} does not contain true value");
        StringAssert.DoesNotContain($"{expected} False", args, $"{msg} does not contain false value");
    }

    private void AssertDisablesFullscreen(string args, bool should) {
        if (should) {
            StringAssert.Contains("-screen-fullscreen 0", args, "disable fullscreen");
        } else {
            StringAssert.DoesNotContain("-screen-fullscreen 0", args, "disable fullscreen");
        }        
    }


    [Test]
    public void TestHandlesArrangeNone()
    {
        Dictionary<string, object> d = new Dictionary<string, object>();
        d[ArgDef.ARG_WINDOW_ARRANGEMENT] = WindowPositioner.ARRANGE_NONE;
        string result = ArgDef.MakeArgString(d);
        AssertHasArgValue(result, ArgDef.ARG_WINDOW_ARRANGEMENT, WindowPositioner.ARRANGE_NONE);
        AssertDisablesFullscreen(result, false);
    }

    [Test]
    public void TestHandlesArrangeTopLeft()
    {
        Dictionary<string, object> d = new Dictionary<string, object>();
        d[ArgDef.ARG_WINDOW_ARRANGEMENT] = WindowPositioner.ARRANGE_TOP_LEFT;
        string result = ArgDef.MakeArgString(d);
        AssertHasArgValue(
            result, ArgDef.ARG_WINDOW_ARRANGEMENT, WindowPositioner.ARRANGE_TOP_LEFT,
            "arrangement argument");
        AssertDisablesFullscreen(result, true);
    }

    [Test]
    public void TestHandlesArrangeTopRight() {
        Dictionary<string, object> d = new Dictionary<string, object>();
        d[ArgDef.ARG_WINDOW_ARRANGEMENT] = WindowPositioner.ARRANGE_TOP_RIGHT;
        string result = ArgDef.MakeArgString(d);
        AssertHasArgValue(
            result, ArgDef.ARG_WINDOW_ARRANGEMENT, WindowPositioner.ARRANGE_TOP_RIGHT,
            "arrangement argument");
        AssertDisablesFullscreen(result, true);
    }

    [Test]
    public void TestHandlesArrangeBottomLeft() {
        Dictionary<string, object> d = new Dictionary<string, object>();
        d[ArgDef.ARG_WINDOW_ARRANGEMENT] = WindowPositioner.ARRANGE_BOTTOM_LEFT;
        string result = ArgDef.MakeArgString(d);
        AssertHasArgValue(
            result, ArgDef.ARG_WINDOW_ARRANGEMENT, WindowPositioner.ARRANGE_BOTTOM_LEFT,
            "arrangement argument");
        AssertDisablesFullscreen(result, true);
    }

    [Test]
    public void TestHandlesArrangeBottomRight() {
        Dictionary<string, object> d = new Dictionary<string, object>();
        d[ArgDef.ARG_WINDOW_ARRANGEMENT] = WindowPositioner.ARRANGE_BOTTOM_RIGHT;
        string result = ArgDef.MakeArgString(d);
        AssertHasArgValue(
            result, ArgDef.ARG_WINDOW_ARRANGEMENT, WindowPositioner.ARRANGE_BOTTOM_RIGHT,
            "arrangement argument");
        AssertDisablesFullscreen(result, true);
    }

    [Test]
    public void TestHandlesDisableLogStackTrace()  {
        Dictionary<string, object> d = new Dictionary<string, object>();
        d[ArgDef.ARG_DISABLE_LOG_STACK_TRACE] = true;
        string result = ArgDef.MakeArgString(d);
        AssertHasArgFlag(
            result, ArgDef.ARG_DISABLE_LOG_STACK_TRACE);            
    }
}
