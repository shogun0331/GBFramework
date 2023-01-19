using UnityEngine;

namespace GB.UI
{

    public class UIBinder : MonoBehaviour
    {
        public UIScreen GetScreen()
        {
            return UiUtil.FindUIScreen(transform);
        }

        public virtual void SetBind()
        {

        }

    }

}
