using UnityEngine;

namespace GB
{
    public class View : MonoBehaviour
    {

        public virtual void OnChangeValue(Model model)
        {

        }

        public virtual void OnChangeValue<T>(string key, MessageStuct<T> data)
        {

        }
        public virtual void OnChangeValue(string key)
        {

        }

        public virtual void OnChangeValue(string key, string value)
        {

        }

        public virtual void BindRegist()
        {

        }

    }
}
