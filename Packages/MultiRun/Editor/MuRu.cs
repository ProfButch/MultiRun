using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using MultiRun;
using MultiRun.Cli;


namespace MultiRun
{
    /**
     * Static general utilities for MultiRun (MuRu).
     * 
     */
    public static class MuRu
    {
        private const string PRE = "[MultiRun]";
        // Keep in sync with MultiRunMono until we can refactor this out into a
        // common library used by editor and runner.
        public const string APP_END_IND = "-- MultiRun Application End Indicator --";

        public static void Log(string msg) {
            Debug.Log($"{PRE}{msg}");
        }


        public static void LogWarning(string msg) {
            Debug.LogWarning($"{PRE}{msg}");
        }


        public static void LogError(string msg) {
            Debug.LogError($"{PRE}{msg}");
        }


        // Couldn't think of a clever way to add [MultiRun] to this one, but it
        // is here for completeness.
        public static void LogException(System.Exception e) {
            Debug.LogException(e);
        }
    }
}