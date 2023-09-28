using System;
using System.Collections;
using System.Collections.Generic;

namespace MultiRun
{
    public class CommandLineParser
    {
        public bool disableDebugLogStackTrace = false;

        public bool resizeWindow = false;
        public int windowWidth = -1;
        public int windowHeight = -1;
        
        public bool moveWindow = false;
        public int windowX = -1;
        public int windowY = -1;

        public string windowArrangement = WindowPositioner.ARRANGE_NONE;


        private Dictionary<string, string> cliArgs;

        public CommandLineParser()
        {
            cliArgs = GetCommandlineArgs();
            SetValuesFromCommandLineArgs(cliArgs);
        }

        private T GetArgValue<T>(Dictionary<string, string> d, string key, T defaultValue) {
            T toReturn = defaultValue;
            if (d.ContainsKey(key)) {
                toReturn = (T)Convert.ChangeType(d[key], typeof(T));
            }
            return toReturn;
        }


        private void SetValuesFromCommandLineArgs(Dictionary<string, string> args)
        {
            windowArrangement = GetArgValue<string>(args, "-mr-arrangement", windowArrangement);
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
    }
}