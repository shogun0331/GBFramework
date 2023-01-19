using UnityEngine;
using UnityEngine.UI;

namespace GB.UI
{
    public class BinderButton : UIBinder
    {
        public string Key;
        [Header("설명")]
        [TextArea] public string Infomation;


        public override void SetBind()
        {
            base.SetBind();

            var screen = GetScreen();

            if (screen != null)
            {
                screen.AddButton(Key, GetComponent<Button>());

            }
        }
    }

}
