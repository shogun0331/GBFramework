using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace GB.UI
{
    public class BinderGameObject : UIBinder
    {
        public string Key;

        [Header("설명")]
        [TextArea] public string Infomation;


        public override void SetBind()
        {
            base.SetBind();

            var screen = GetScreen();

            if (screen != null)
                screen.AddGameObject(Key, gameObject);
        }
    }
}