using System.Collections.Generic;
using UnityEngine;
using GB.Global;
using GB.UI;
using System;

namespace GB
{
    /// <remarks>
    /// <copyright file="Presenter.cs" company="GB">
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

    public class Presenter : MonoBehaviour
    {
        const HideFlags Flags = HideFlags.HideInHierarchy | HideFlags.DontSave;
        public static Presenter Instance => Application.isPlaying ? ComponentSingleton<Presenter>.Get(Flags) : null;
        Dictionary<string, List<View>> _dicView = new Dictionary<string, List<View>>();
        Dictionary<string, Model> _dicModel = new Dictionary<string, Model>();

        public static void Clear()
        {
            Instance._dicView.Clear();
        }

        public static void RegistModel(string key, Model model)
        {
            Instance._dicModel[key] = model;
        }

        public static Model GetModel(string key)
        {
            if (Instance._dicModel.ContainsKey(key) == false) return null;
            return Instance._dicModel[key];
        }

        public static void Bind(string domain, View view)
        {
            if (Instance._dicView.ContainsKey(domain))
            {
                if (Instance._dicView[domain] != null)
                    Instance._dicView[domain].Add(view);
            }
            else
            {
                List<View> viewList = new List<View>();
                viewList.Add(view);
                Instance._dicView.Add(domain, viewList);
            }
        }

        public static void UnBind(string domain, View view)
        {
            if (Instance._dicView.ContainsKey(domain) == false) return;
            Instance._dicView[domain].Remove(view);
        }

        public static void Send<T>(string domain, string key, T data)
        {
            MessageStuct<T> message = new MessageStuct<T>();
            message.Key = domain;
            message.Data = data;
            Notify<T>(domain, key, message);


        }


        public static void Notify<T>(string domain, string key, MessageStuct<T> data)
        {

            if (Instance._dicView.ContainsKey(domain) == false) return;

            List<View> viewList = Instance._dicView[domain];

            for (int i = 0; i < viewList.Count; ++i)
            {
                if (viewList[i] == null) continue;
                viewList[i].OnChangeValue(key, data);
            }

        }

        public static void Notify(string domain, string key, string value)
        {
            if (Instance._dicView.ContainsKey(domain) == false) return;

            List<View> viewList = Instance._dicView[domain];

            for (int i = 0; i < viewList.Count; ++i)
            {
                if (viewList[i] == null) continue;

                viewList[i].OnChangeValue(key, value);
            }

        }

        public static void Notify(string domain, string key)
        {
            if (Instance._dicView.ContainsKey(domain) == false) return;

            List<View> viewList = Instance._dicView[domain];

            for (int i = 0; i < viewList.Count; ++i)
            {
                if (viewList[i] == null) continue;

                viewList[i].OnChangeValue(key);
            }

        }

        public static void Notify(string domain)
        {
            if (Instance._dicModel.ContainsKey(domain) == false) return;
            if (Instance._dicView.ContainsKey(domain) == false) return;

            List<View> viewList = Instance._dicView[domain];

            for (int i = 0; i < viewList.Count; ++i)
            {
                if (viewList[i] == null) continue;

                viewList[i].OnChangeValue(Instance._dicModel[domain]);
            }

        }
    }

    public struct MessageStuct<T>
    {
        public string Key;
        public T Data;
    }

}

