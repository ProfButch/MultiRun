using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;
using MultiRun;
using MultiRun.OsHelpers;
using System.IO;

public class TestOsHelper {
    public class TestOsxHelper {
        private class MockApp {
            public string appPath;
            private string srcPlistName;
            private string resourcePath;
            private string execName;

            /// <summary>
            /// Note executableName should be the same name as in the plist.
            /// </summary>
            /// <param name="appName"></param>
            /// <param name="testResourcePath"></param>
            /// <param name="plistName"></param>
            /// <param name="executableName"></param>
            public MockApp(string appName, string testResourcePath, string plistName, string executableName) {
                appPath = Path.Join(
                    Application.persistentDataPath,
                    $"{appName}.app");
                srcPlistName = plistName;
                resourcePath = testResourcePath;
                execName = executableName;
            }

            public string ContentsPath() {
                return Path.Join(appPath, "Contents");
            }

            public string MacOsPath() {
                return Path.Join(ContentsPath(), "MacOS");
            }

            public string PlistPath() {
                return Path.Join(ContentsPath(), "Info.plist");
            }

            public void CreateApp() {
                Directory.CreateDirectory(appPath);
                Directory.CreateDirectory(ContentsPath());
                Directory.CreateDirectory(MacOsPath());
                File.Copy(
                    Path.Join(resourcePath, srcPlistName),
                    Path.Join(ContentsPath(), "Info.plist"));
                File.Create(Path.Join(MacOsPath(), execName));
            }

            public void Delete() {
                // Just make sure we don't accidentally set an app path to
                // root or something over the course of the tests.  At least
                // this will only delete one app.
                if (appPath.StartsWith(Application.persistentDataPath) &&
                    appPath.EndsWith(".app") &&
                    Directory.Exists(appPath)) {
                        Directory.Delete(appPath, true);
                }
            }
        }


        private MockApp fakeApp;
        private MockApp anotherApp;
        private MockApp invalidApp;
        private MockApp execDoesNotMatchApp;

        private string testResourcePath;


        private void CreateTestData() {
            fakeApp.CreateApp();
            anotherApp.CreateApp();
            invalidApp.CreateApp();
            execDoesNotMatchApp.CreateApp();
        }


        private void PopulatePaths() {
            testResourcePath = $"{Directory.GetCurrentDirectory()}";
            testResourcePath = Path.Join(testResourcePath, "Packages", "MultiRun");
            testResourcePath = Path.Join(testResourcePath, "Tests", "TestResources");
            DirectoryAssert.Exists(testResourcePath);
        }


        private void CleanUpTestData() {
            fakeApp.Delete();
            anotherApp.Delete();
            invalidApp.Delete();
            execDoesNotMatchApp.Delete();
        }


        [OneTimeSetUp]
        public void Init() {
            PopulatePaths();
            fakeApp = new MockApp(
                "FakeApp", testResourcePath, "FakeApp_Info.plist", "FakeAppExecutable");
            anotherApp = new MockApp(
                "AnotherApp", testResourcePath, "AnotherApp_Info.plist", "AnotherAppExecutable");
            invalidApp = new MockApp(
                "InvalidApp", testResourcePath, "Invalid_Info.plist", "whatever");
            execDoesNotMatchApp = new MockApp(
                "DoesNotMatchApp", testResourcePath, "FakeApp_Info.plist", "DoesNotMatchPlistEntry");
            CleanUpTestData();
            CreateTestData();
        }


        [OneTimeTearDown]
        public void CleanUp() {
            CleanUpTestData();
        }


        [Test]
        public void TestCanMakeOne() {
            OsxHelper h = new OsxHelper();
            Assert.NotNull(h);
        }

        [Test]
        public void CmdLaunchBuildFindsExecutableNameInPlistFileAndUsesIt() {
            OsxHelper h = new OsxHelper();
            string cmd = h.CmdLaunchBuild(fakeApp.appPath);
            StringAssert.Contains("FakeAppExecutable", cmd);
        }

        [Test]
        public void CmdLaunchBuildFindsExecutableInAnotherPlist() {
            OsxHelper h = new OsxHelper();
            string cmd = h.CmdLaunchBuild(anotherApp.appPath);
            StringAssert.Contains("AnotherAppExecutable", cmd);
        }

        [Test]
        public void CmdLaunchBuildUsesOpenWhenItCannotFindExecutableName() {
            OsxHelper h = new OsxHelper();
            string cmd = h.CmdLaunchBuild(invalidApp.appPath);
            StringAssert.StartsWith("open", cmd);
        }

        [Test]
        public void CmdLaunchBuiildUsesOpenWhenExecutableFileNotFound() {
            OsxHelper h = new OsxHelper();
            string cmd = h.CmdLaunchBuild(execDoesNotMatchApp.appPath);
            StringAssert.StartsWith("open", cmd);
        }
    }
}