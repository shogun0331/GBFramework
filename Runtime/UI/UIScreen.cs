using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
namespace GB.UI
{

    /// <remarks>
    /// <copyright file="UIScreen.cs" company="GB">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2022 GB
    /// 
    /// Permission is hereby granted, free of charge, to any person obtaining a copy
    /// of this software and associated documentation files (the "Software"), to deal
    /// in the Software without restriction, including without limitation the rights
    /// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    /// copies of the Software, and to permit persons to whom the Software is
    /// furnished to do so, subject to the following conditions:
    /// 
    /// The above copyright notice and this permission notice shall be included in
    /// all copies or substantial portions of the Software.
    /// 
    /// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    /// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    /// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    /// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    /// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    /// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    /// THE SOFTWARE.
    public enum UI_TYPE { Screen = 0, Popup }
    [RequireComponent(typeof(UICreate))]
    public class UIScreen : View
    {
        [SerializeField] UI_TYPE _type = UI_TYPE.Popup;

        public void Regist()
        {
            if (_type == UI_TYPE.Popup)
                UIManager.Instance.RegistPopup(this.gameObject.name, this);
            else
                UIManager.Instance.RegistScreen(this.gameObject.name, this);
        }

        [SerializeField] protected SerializableDictionary<string, Image> _dicImages = new SerializableDictionary<string, Image>();
        [SerializeField] protected SerializableDictionary<string, Text> _dicTexts = new SerializableDictionary<string, Text>();
        [SerializeField] protected SerializableDictionary<string, Button> _dicButtons = new SerializableDictionary<string, Button>();
        [SerializeField] protected SerializableDictionary<string, Transform> _dicTransforms = new SerializableDictionary<string, Transform>();
        [SerializeField] protected SerializableDictionary<string, GameObject> _dicGameObject = new SerializableDictionary<string, GameObject>();


        public void SetBind()
        {
            Clear();

            UIBinder[] allChildren = GetComponentsInChildren<UIBinder>(true);
            for (int i = 0; i < allChildren.Length; ++i)
                allChildren[i].SetBind();
        }







        public void AddText(string key, Text text)
        {
            var pair = new SerializableDictionary<string, Text>.Pair(key, text);
            _dicTexts.Add(pair);
        }

        public void AddImage(string key, Image img)
        {
            var pair = new SerializableDictionary<string, Image>.Pair(key, img);
            _dicImages.Add(pair);
        }

        public void AddButton(string key, Button btn)
        {
            var pair = new SerializableDictionary<string, Button>.Pair(key, btn);
            _dicButtons.Add(pair);
        }

        public void AddTransform(string key, Transform tr)
        {
            var pair = new SerializableDictionary<string, Transform>.Pair(key, tr);
            _dicTransforms.Add(pair);
        }

        public void AddGameObject(string key, GameObject oj)
        {
            var pair = new SerializableDictionary<string, GameObject>.Pair(key, oj);
            _dicGameObject.Add(pair);
        }


        void Clear()
        {
            _dicGameObject.Clear();
            _dicButtons.Clear();
            _dicTransforms.Clear();
            _dicImages.Clear();
            _dicTexts.Clear();
        }


        public int Weight
        {
            get
            {
                return GetComponent<RectTransform>().GetSiblingIndex();
            }
            set
            {
                GetComponent<RectTransform>().SetSiblingIndex(value);
            }
        }

        public void ShowFirstLayer()
        {
            GetComponent<RectTransform>().SetAsLastSibling();
        }

        public void ChangeScene(string sceneName)
        {
            Presenter.Clear();
            UIManager.Instance.ChangeScene(sceneName);
        }

        virtual public void Close()
        {
            if (_type == UI_TYPE.Popup)
                UIManager.Instance.ClosePopup();
            else
                UIManager.Instance.CloseScene();
        }

        virtual public bool Backkey()
        {
            Close();
            return true;
        }
    }
}