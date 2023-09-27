using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiRun
{
    public static class WindowPositioner  {

        public static void MoveWindow() {
            List<DisplayInfo> displays = new List<DisplayInfo>();
            Screen.GetDisplayLayout(displays);
            Screen.MoveMainWindowTo(displays[0], new Vector2Int(10, 10));
        }
    }
}
