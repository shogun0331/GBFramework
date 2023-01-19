#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace GB.UI
{

    [CustomEditor(typeof(UICreate))]
    public class UIBtnEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            UICreate t = (UICreate)target;

            if (GUILayout.Button("Bind"))
            {
                t.Button_Bind();
            }

            if (GUILayout.Button("Setting"))
            {
                t.Button_Setting();
            }



            if (GUILayout.Button("Generate"))
            {
                t.Button_Generate();
            }
        }
    }
}
#endif