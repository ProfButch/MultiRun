using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiRun
{
    public static class WindowPositioner  {
        public const string ARRANGE_TOP_LEFT = "tl";
        public const string ARRANGE_TOP_RIGHT = "tr";
        public const string ARRANGE_BOTTOM_LEFT = "bl";
        public const string ARRANGE_BOTTOM_RIGHT = "br";
        public const string ARRANGE_NONE = "none";


        public static void MoveWindow(int x = 100, int y = 100) {
            List<DisplayInfo> displays = new List<DisplayInfo>();
            Screen.GetDisplayLayout(displays);
            Screen.MoveMainWindowTo(displays[0], new Vector2Int(x, y));
        }


        public static void ArrangeWindow(string arrange)
        {
            List<DisplayInfo> displays = new List<DisplayInfo>();
            Screen.GetDisplayLayout(displays);

            DisplayInfo selectedDisplay = displays[0];
            int w = selectedDisplay.width;
            int h = selectedDisplay.height;            

            Vector2Int pos = new Vector2Int(-1, -1);
            if(arrange == ARRANGE_TOP_LEFT) {
                pos.x = 0;
                pos.y = 0;
            } else if(arrange == ARRANGE_TOP_RIGHT) {
                pos.x = w / 2;
                pos.y = 0;
            } else if(arrange == ARRANGE_BOTTOM_LEFT) {
                pos.x = 0;
                pos.y = h / 2;
            } else if(arrange == ARRANGE_BOTTOM_RIGHT) {
                pos.x = w / 2;
                pos.y = h / 2;
            }

            if(pos.x != -1) {
                Screen.MoveMainWindowTo(selectedDisplay, pos);
                Screen.SetResolution(w / 2, h / 2, false);
            }
            
        }
    }
}
