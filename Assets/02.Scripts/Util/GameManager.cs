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

    public Player Player { get; set; }

    private Dictionary<Type, MonoBehaviour> managers = new Dictionary<Type, MonoBehaviour>();

    private Type[] managerArr = {
        typeof(BattleManager),
        typeof(InputManager)
    };

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            foreach (Type managerType in managerArr)
            {
                AddManager(managerType);
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

    public void AddManager(Type type)
    {
        GameObject obj = new GameObject(type.Name);
        obj.transform.parent = this.transform;

        var manager = obj.AddComponent(type) as MonoBehaviour;

        managers.Add(type, manager);
    }

    public T GetManager<T>(Type type) where T : MonoBehaviour
    {
        if (managers.ContainsKey(type))
        {
            return managers[type] as T;
        }
        else
        {
            Debug.LogError($"Manager with key '{type}' not found.");

            return null;
        }
    }
}
