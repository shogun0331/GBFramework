using UnityEngine;
using UnityEngine.UI;

namespace GB.UI
{

    public class BinderTransform : UIBinder
    {
        public string Key;
        [Header("설명")]
        [TextArea] public string Infomation;

        public override void SetBind()
        {
            base.SetBind();

            var screen = GetScreen();

            if (screen != null)
                screen.AddTransform(Key, transform);
        }
    }

}
