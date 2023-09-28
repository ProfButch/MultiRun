using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using MultiRun;
using MultiRun.Cli;
using static UnityEngine.Networking.UnityWebRequest;

public class TestCli
{
    private void AssertHasArgValue(string args, string argName, string argValue, string msg ="")
    {
        string expected = $"{ArgDef.ArgName(argName)} {argValue}";
        StringAssert.Contains(expected, args, msg);
    }

    [Test]
    public void TestHandlesArrangeNone()
    {
        Dictionary<string, object> d = new Dictionary<string, object>();
        d[ArgDef.ARG_WINDOW_ARRANGEMENT] = WindowPositioner.ARRANGE_NONE;
        string result = ArgDef.MakeArgString(d);
        AssertHasArgValue(result, ArgDef.ARG_WINDOW_ARRANGEMENT, WindowPositioner.ARRANGE_NONE);
        StringAssert.DoesNotContain(
            "-screen-fullscreen 0", result, "disable fullscreen");
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
        StringAssert.Contains(
            "-screen-fullscreen 0", result, "disable fullscreen");
    }

    [Test]
    public void TestHandlesArrangeTopRight() {
        Dictionary<string, object> d = new Dictionary<string, object>();
        d[ArgDef.ARG_WINDOW_ARRANGEMENT] = WindowPositioner.ARRANGE_TOP_RIGHT;
        string result = ArgDef.MakeArgString(d);
        AssertHasArgValue(
            result, ArgDef.ARG_WINDOW_ARRANGEMENT, WindowPositioner.ARRANGE_TOP_RIGHT,
            "arrangement argument");
        StringAssert.Contains(
            "-screen-fullscreen 0", result, "disable fullscreen");
    }

    [Test]
    public void TestHandlesArrangeBottomLeft() {
        Dictionary<string, object> d = new Dictionary<string, object>();
        d[ArgDef.ARG_WINDOW_ARRANGEMENT] = WindowPositioner.ARRANGE_BOTTOM_LEFT;
        string result = ArgDef.MakeArgString(d);
        AssertHasArgValue(
            result, ArgDef.ARG_WINDOW_ARRANGEMENT, WindowPositioner.ARRANGE_BOTTOM_LEFT,
            "arrangement argument");
        StringAssert.Contains(
            "-screen-fullscreen 0", result, "disable fullscreen");
    }

    [Test]
    public void TestHandlesArrangeBottomRight() {
        Dictionary<string, object> d = new Dictionary<string, object>();
        d[ArgDef.ARG_WINDOW_ARRANGEMENT] = WindowPositioner.ARRANGE_BOTTOM_RIGHT;
        string result = ArgDef.MakeArgString(d);
        AssertHasArgValue(
            result, ArgDef.ARG_WINDOW_ARRANGEMENT, WindowPositioner.ARRANGE_BOTTOM_RIGHT,
            "arrangement argument");
        StringAssert.Contains(
            "-screen-fullscreen 0", result, "disable fullscreen");
    }

}
