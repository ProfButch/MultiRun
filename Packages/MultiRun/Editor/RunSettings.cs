using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Bitwesgames
{
    public class RunSettings : EditorWindow {
        Toggle tglArrangeWindows;
        Toggle tglDisableLogStackTrace;

        public void CreateGUI() {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.bitwesgames.multirun/Editor/RunSettings.uxml");
            VisualElement uxmlElements = visualTree.Instantiate();
            root.Add(uxmlElements);

            tglArrangeWindows = root.Query<Toggle>("ArrangeWindows").First();
            tglDisableLogStackTrace = root.Query<Toggle>("DisableStackTrace").First();

            tglArrangeWindows.value = false;
            tglDisableLogStackTrace.value = false;

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            //var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.bitwesgames.multirun/Editor/RunSettings.uss");
        }

        public bool ShouldArrangeWindows()
        {
            return tglArrangeWindows.value;
        }
    }
}
