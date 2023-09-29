using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[FilePath("UserSettings/MultiRun.yaml", FilePathAttribute.Location.ProjectFolder)]
public class MultiRunSettings : ScriptableSingleton<MultiRunSettings>
{
    [SerializeField]
    public bool disableLogStackTrace = true;

    [SerializeField]
    public bool arrangeWindows = true;

    [SerializeField]
    public string projectBuildPath = string.Empty;


    public void DoSave()
    {
        Save(true);
    }

    public void PrintMe()
    {
        Debug.Log("MultiRunSettings state: " + JsonUtility.ToJson(this, true));
    }
}