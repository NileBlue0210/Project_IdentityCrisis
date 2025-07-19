using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();

                if (instance == null)
                {
                    GameObject obj = new GameObject("GameManager");
                    instance = obj.AddComponent<GameManager>();
                }
            }

            return instance;
        }
    }

    private Player player;
    public Player Player { get { return player; } set { player = value; } }

    private Dictionary<string, MonoBehaviour> _managers = new Dictionary<string, MonoBehaviour>();

    private Type[] _managerArr = {

    };

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            foreach (Type managerType in _managerArr)
            {
                AddManager(managerType, managerType.Name);
            }

            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {

    }

    void Update()
    {

    }

    public void AddManager(Type type, string key)
    {
        GameObject obj = new GameObject(type.Name);
        obj.transform.parent = this.transform;

        var manager = obj.AddComponent(type) as MonoBehaviour;

        _managers.Add(key, manager);
    }

    public T GetManager<T>(string key) where T : MonoBehaviour
    {
        if (_managers.ContainsKey(key))
        {
            return _managers[key] as T;
        }
        else
        {
            Debug.LogError($"Manager with key '{key}' not found.");

            return null;
        }
    }
}
