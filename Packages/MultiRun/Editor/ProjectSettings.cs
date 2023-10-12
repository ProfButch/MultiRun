using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace MultiRun
{
    [FilePath("UserSettings/MultiRun.yaml", FilePathAttribute.Location.ProjectFolder)]
    public class ProjectSettings : ScriptableSingleton<ProjectSettings>
    {
        [SerializeField]
        public bool disableLogStackTrace = true;

        [SerializeField]
        public bool arrangeWindows = true;

        [SerializeField]
        public string projectBuildPath = string.Empty;

        [SerializeField]
        public string allInstanceArgs= string.Empty;

        [SerializeField]
        public string[] instanceArgs = new string[4]{
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty
        };

         public string lastLoadedLogPath = string.Empty;

        public void DoSave()
        {
            Save(true);
        }

        public void PrintMe()
        {
            MuRu.Log("MultiRunSettings state: " + JsonUtility.ToJson(this, true));
        }
    }
}
