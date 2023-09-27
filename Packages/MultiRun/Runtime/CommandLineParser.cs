using System;
using System.Collections;
using System.Collections.Generic;

namespace MultiRun
{
    public class CommandLineParser
    {
        private Dictionary<string, string> args;


        public CommandLineParser()
        {
            args = GetCommandlineArgs();
        }


        public override string ToString()
        {
            string toReturn = "";
            foreach (KeyValuePair<string, string> kvp in args) {
                toReturn += $"{kvp.Key} :: {kvp.Value}\n";
            }

            return toReturn;
        }


        private Dictionary<string, string> GetCommandlineArgs()
        {
            Dictionary<string, string> argDictionary = new Dictionary<string, string>();

            var args = System.Environment.GetCommandLineArgs();

            for (int i = 0; i < args.Length; ++i)
            {
                var arg = args[i].ToLower();
                if (arg.StartsWith("-"))
                {
                    var value = i < args.Length - 1 ? args[i + 1].ToLower() : null;
                    value = (value?.StartsWith("-") ?? false) ? null : value;

                    argDictionary.Add(arg, value);
                }
            }
            return argDictionary;
        }
    }
}