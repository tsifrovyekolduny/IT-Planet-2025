using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public abstract class Singletone<T> : MonoBehaviour where T : class
{
    public static T Instance;
    protected virtual void Start()
    {
        if (Instance == null)
        {
            Instance = GameObject.FindObjectOfType(typeof(T)) as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != gameObject)
        {
            Destroy(gameObject);
        }
    }
}