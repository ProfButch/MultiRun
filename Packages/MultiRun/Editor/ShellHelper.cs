/*
 * Copied Verbatim from
 * https://github.com/wlgys8/UnityShellHelper
 * I've since updated some things, so it's not a verbatim copy anymore.
 *
 * The author has a new, more robust solution, but this works and it's a single
 * file and fits my needs now.
 *
 * The new one is:  https://github.com/wlgys8/UnityShell
 */
using UnityEngine;
using System.Collections;
using System.Diagnostics;
using UnityEditor;
using System.Collections.Generic;

namespace MultiRun {
    public class ShellHelper {

        public class ShellRequest {
            public event System.Action<int, string> onLog;
            public event System.Action onError;
            public event System.Action onDone;

            public bool isDone = false;
            public bool hasErrored = false;
            public Process process;
            public string cmd = "";

            public void Log(int type, string log) {
                if (onLog != null) {
                    onLog(type, log);
                }
                if (type == 1) {
                    MuRu.LogError(log);
                }
            }

            public void NotifyDone() {
                isDone = true;
                if (onDone != null) {
                    onDone();
                }
            }

            public void Error() {
                hasErrored = true;
                if (onError != null) {
                    onError();
                }
            }
        }


        private static string shellApp {
            get {
#if UNITY_EDITOR_WIN
			    string app = "cmd.exe";
#elif UNITY_EDITOR_OSX
                string app = "bash";
#endif
                return app;
            }
        }


        private static List<System.Action> _queue = new List<System.Action>();


        static ShellHelper() {
            _queue = new List<System.Action>();
            EditorApplication.update += OnUpdate;
        }


        private static void OnUpdate() {
            for (int i = 0; i < _queue.Count; i++) {
                try {
                    var action = _queue[i];
                    if (action != null) {
                        action();
                    }
                } catch (System.Exception e) {
                    MuRu.LogException(e);
                }
            }
            _queue.Clear();
        }

        public static ShellRequest ProcessCommandAutoClose(string cmd, string workDirectory="/", List<string> environmentVars = null) {
            ShellRequest req = ProcessCommand(cmd, workDirectory, environmentVars);
            //req.process.Close();
            return req;
        }

        public static ShellRequest ProcessCommandKeepOpen(string cmd, string workDirectory="/", List<string> environmentVars = null) {
            return ProcessCommand(cmd, workDirectory, environmentVars);
        }

        private static ShellRequest ProcessCommand(string cmd, string workDirectory, List<string> environmentVars = null) {
            ShellRequest req = new ShellRequest();
            req.cmd = cmd;

            System.Threading.ThreadPool.QueueUserWorkItem(delegate (object state) {
                Process p = null;
                try {
                    ProcessStartInfo start = new ProcessStartInfo(shellApp);

#if UNITY_EDITOR_OSX
                    string splitChar = ":";
                    start.Arguments = "-c";
#elif UNITY_EDITOR_WIN
				    string splitChar = ";";
				    start.Arguments = "/c";
#endif

                    if (environmentVars != null) {
                        foreach (string var in environmentVars) {
                            start.EnvironmentVariables["PATH"] += (splitChar + var);
                        }
                    }

                    start.Arguments += (" \"" + cmd + " \"");
                    start.CreateNoWindow = true;
                    start.ErrorDialog = true;
                    start.UseShellExecute = false;
                    start.WorkingDirectory = workDirectory;

                    if (start.UseShellExecute) {
                        start.RedirectStandardOutput = false;
                        start.RedirectStandardError = false;
                        start.RedirectStandardInput = false;
                    } else {
                        start.RedirectStandardOutput = true;
                        start.RedirectStandardError = true;
                        start.RedirectStandardInput = true;
                        start.StandardOutputEncoding = System.Text.UTF8Encoding.UTF8;
                        start.StandardErrorEncoding = System.Text.UTF8Encoding.UTF8;
                    }

                    p = Process.Start(start);
                    req.process = p;


                    p.ErrorDataReceived += delegate (object sender, DataReceivedEventArgs e) {
                        MuRu.LogError(e.Data);
                    };
                    p.OutputDataReceived += delegate (object sender, DataReceivedEventArgs e) {
                        MuRu.LogError(e.Data);
                    };
                    p.Exited += delegate (object sender, System.EventArgs e) {
                        MuRu.LogError(e.ToString());
                    };

                    bool hasError = false;
                    do {
                        string line = p.StandardOutput.ReadLine();
                        if (line == null) {
                            break;
                        }

                        line = line.Replace("\\", "/");

                        _queue.Add(delegate () {
                            req.Log(0, line);
                        });

                    } while (true);
                    

                    while (true) {
                        string error = p.StandardError.ReadLine();
                        if (string.IsNullOrEmpty(error)) {
                            break;
                        }
                        hasError = true;
                        _queue.Add(delegate () {
                            req.Log(1, error);
                        });
                    }

                    //p.Close();
                    if (hasError) {
                        _queue.Add(delegate () {
                            req.Error();
                        });
                    } else {
                        _queue.Add(delegate () {
                            req.NotifyDone();
                        });
                    }


                } catch (System.Exception e) {
                    MuRu.LogException(e);
                    if (p != null) {
                        p.Close();
                    }
                }
            });
            return req;
        }


        //private List<string> _enviroumentVars = new List<string>();

        //public void AddEnvironmentVars(params string[] vars) {
        //    for (int i = 0; i < vars.Length; i++) {
        //        if (vars[i] == null) {
        //            continue;
        //        }
        //        if (string.IsNullOrEmpty(vars[i].Trim())) {
        //            continue;
        //        }
        //        _enviroumentVars.Add(vars[i]);
        //    }
        //}

        //public ShellRequest ProcessCMD(string cmd, string workDir) {
        //    return ShellHelper.ProcessCommand(cmd, workDir, _enviroumentVars);
        //}

    }
}