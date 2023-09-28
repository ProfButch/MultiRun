using System;
using System.Collections;
using System.Collections.Generic;

namespace MultiRun.Cli
{
    public static class ArgDef
    {
        public const string ARG_WINDOW_ARRANGEMENT = "window_arrangement";
        public const string ARG_DISABLE_LOG_STACK_TRACE = "disable_log_stack_trace";

        const string PREFIX = "-mr_";

        private static Dictionary<string, object> argDefaults = new Dictionary<string, object>() {
            {ARG_WINDOW_ARRANGEMENT, WindowPositioner.ARRANGE_NONE },
            {ARG_DISABLE_LOG_STACK_TRACE, false }
        };


        public static string MakeArgString(Dictionary<string, object> args) {
            string toReturn = "";
            bool nonNoneArrangement = false;
            foreach(string key in args.Keys) {
                string argName = ArgName(key);
                string argValue = GetArgValue<string>(args, key, "");

                toReturn += $"{argName} {argValue} ";
                if(key == ARG_WINDOW_ARRANGEMENT && argValue != WindowPositioner.ARRANGE_NONE) {
                    nonNoneArrangement = true;
                }
            }

            if (nonNoneArrangement) {
                toReturn += "-screen-fullscreen 0 ";
            }
            return toReturn;
        }


        public static T GetArgValue<T>(Dictionary<string, string> d, string key, T defaultValue) {
            T toReturn = defaultValue;
            if (d.ContainsKey(key)) {
                toReturn = (T)Convert.ChangeType(d[key], typeof(T));
            }
            return toReturn;
        }


        public static T GetArgValue<T>(Dictionary<string, object> d, string key, T defaultValue) {
            T toReturn = defaultValue;
            if (d.ContainsKey(key)) {
                toReturn = (T)Convert.ChangeType(d[key], typeof(T));
            }
            return toReturn;
        }


        public static string ArgName(string baseName) {
            return $"{PREFIX}{baseName}";
        }
    }


    public class Parser {
        public bool disableDebugLogStackTrace = false;
        public string windowArrangement = WindowPositioner.ARRANGE_NONE;


        private Dictionary<string, string> cliArgs;


        public Parser() {
            Parse();
        }


        private void SetValuesFromCommandLineArgs(Dictionary<string, string> args) {
            windowArrangement = ArgDef.GetArgValue<string>(
                args,
                ArgDef.ArgName(ArgDef.ARG_WINDOW_ARRANGEMENT),
                windowArrangement);
        }


        private Dictionary<string, string> GetCommandlineArgs() {
            Dictionary<string, string> argDictionary = new Dictionary<string, string>();

            var args = System.Environment.GetCommandLineArgs();

            for (int i = 0; i < args.Length; ++i) {
                var arg = args[i].ToLower();
                if (arg.StartsWith("-")) {
                    var value = i < args.Length - 1 ? args[i + 1].ToLower() : null;
                    value = (value?.StartsWith("-") ?? false) ? null : value;

                    argDictionary.Add(arg, value);
                }
            }
            return argDictionary;
        }


        public override string ToString()
        {
            string toReturn = "";
            foreach (KeyValuePair<string, string> kvp in cliArgs) {
                toReturn += $"{kvp.Key} :: {kvp.Value}\n";
            }

            return toReturn;
        }


        public string GetArgumentString()
        {
            return "";
        }


        public void Parse() {
            cliArgs = GetCommandlineArgs();
            SetValuesFromCommandLineArgs(cliArgs);
        }
    }
}