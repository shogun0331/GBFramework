using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconManager : MonoBehaviour
{
    static IconManager _Instance;
    public static IconManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = FindObjectOfType<IconManager>();
                if (_Instance != null)
                    _Instance.Init();
            }
            if (_Instance == null)
            {
                _Instance = new GameObject("IconManager").AddComponent<IconManager>();
                _Instance.Init();
            }

            return _Instance;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Init()
    {
        var icons = Resources.LoadAll<Sprite>("Icons/");

        _dicSprs.Clear();
        for (int i = 0; i < icons.Length; ++i)
        {
            var pair = new SerializableDictionary<string, Sprite>.Pair(icons[i].name, icons[i]);
            _dicSprs.Add(pair);
        }
    }

    [SerializeField] SerializableDictionary<string, Sprite> _dicSprs = new SerializableDictionary<string, Sprite>();

    public static Sprite GetSprite(string name)
    {
        if (Instance._dicSprs.ContainsKey(name) == false) return null;

        return Instance._dicSprs[name];
    }
}
