using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

namespace GB.UI
{
    public class UICreate : MonoBehaviour
    {
#if UNITY_EDITOR
        public string UIName;

        const string EDITOR_TEXT_PATH = "$PATH$/EditorTxt/UIText.txt";
        const string EDITOR_TEXT_PROCESS_PATH = "$PATH$/EditorTxt/UIProcess.txt";
        const string EDITOR_SAVECS_PATH = "$PATH$/$FILENAME$";

        public void Button_Bind()
        {
            GetComponent<UIScreen>().SetBind();
        }

        public void Button_Generate()
        {
            if (GetComponent<UIScreen>() != null) return;
            if (string.IsNullOrEmpty(UIName)) return;

            string path = Application.dataPath+"/GB/UI";
            //Debug.Log(path);
            

            DirectoryInfo info = new DirectoryInfo(EDITOR_SAVECS_PATH.Replace("$PATH$", path).Replace("$FILENAME$", ""));
            if (info.Exists == false)
                info.Create();

            info = new DirectoryInfo(EDITOR_SAVECS_PATH.Replace("$PATH$", path).Replace("$FILENAME$", "Process"));
            if (info.Exists == false)
                info.Create();

            string text = ReadTxt(EDITOR_TEXT_PATH.Replace("$PATH$", path));
            text = text.Replace("$POPUPNAME$", UIName);
            WriteTxt(EDITOR_SAVECS_PATH.Replace("$PATH$", path).Replace("$FILENAME$", UIName + ".cs"), text);

            text = ReadTxt(EDITOR_TEXT_PROCESS_PATH.Replace("$PATH$", path));

            text = text.Replace("$POPUPNAME$", UIName);
            WriteTxt(EDITOR_SAVECS_PATH.Replace("$PATH$", path).Replace("$FILENAME$", "Process/Process" + UIName + ".cs"), text);



            AssetDatabase.Refresh();

        }


        public void Button_Setting()
        {
            if (GetComponent<UIScreen>() != null) return;
            if (string.IsNullOrEmpty(UIName)) return;

            System.Type componentType = System.Type.GetType(UIName);
            Component component = gameObject.AddComponent(componentType);
        }


        void WriteTxt(string filePath, string message)
        {
            File.WriteAllText(filePath, message);

        }

        string ReadTxt(string filePath)
        {
            return File.ReadAllText(filePath);
        }

#endif

    }

}

