using Unity.VisualScripting;
using UnityEngine;

public class Singleton<T> where T: Singleton<T>, new()
{
    private static T _instance;
    private static object _instanceLock = new object();

    public static T Instance
    {
        get
        { 
            lock (_instanceLock)
            {
                if (_instance == null)
                {
                    lock (_instanceLock)
                    {
                        _instance = new T();
                    }
                }
            }
            return _instance;
        }
    }
}


public class MonSingleton<T>: MonoBehaviour where T: MonSingleton<T>, new()
{
    public static T Instance {get; private set;}
    protected virtual void Awake()
    {
        Instance = this as T;
    }

}

public static class TransformHelper
{
    public static Transform FindChildByName(this Transform trans, string child_name)
    {
       if(trans.gameObject.name == child_name)
       {
            return trans;
       }

        for (int i = 0; i < trans.childCount; i++)
        {
            Transform childTrans = trans.GetChild(i);
            Transform trans_res = childTrans.FindChildByName(child_name);
            if (trans_res)
            {
                return trans_res;
            }
        }

        return null;
    }

    public static T FindChildComponentByName<T>(this Transform trans, string child_name) where T: Component
    {
        if (trans.gameObject.name == child_name)
        {
            return trans.GetComponent<T>();
        }

        for (int i = 0; i < trans.childCount; i++)
        {
            Transform childTrans = trans.GetChild(i);
            T trans_res = childTrans.FindChildComponentByName<T>(child_name);
            if (trans_res != null)
            {
                return trans_res;
            }
        }

        return default;
    }
}