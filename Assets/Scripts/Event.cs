

using System;
using System.Collections.Generic;

class EventManager: Singleton<EventManager>
{
    private Dictionary<EventType, List<Delegate>> event_map = new Dictionary<EventType, List<Delegate>>();

    public void RegisterEvent(EventType eventType, Delegate func)
    {
        if (!event_map.ContainsKey(eventType))
        {
            event_map.Add(eventType, new List<Delegate>());
        }
        event_map[eventType].Add(func);
    }

    public void RegisterEvent(EventType eventType, Action func)
    {
        Delegate del = func;
        RegisterEvent(eventType, del);
    }

    public void RegisterEvent<T>(EventType eventType, Action<T> func)
    {
        Delegate del = func;
        RegisterEvent(eventType, del);
    }

    public void RegisterEvent<T, U>(EventType eventType, Action<T, U> func)
    {
        Delegate del = func;
        RegisterEvent(eventType, del);
    }

    public void RegisterEvent<T, U, Z>(EventType eventType, Action<T, U, Z> func)
    {
        Delegate del = func;
        RegisterEvent(eventType, del);
    }

    public void RegisterEvent<T, U, Z, X>(EventType eventType, Action<T, U, Z, X> func)
    {
        Delegate del = func;
        RegisterEvent(eventType, del);
    }

    public void RegisterEvent<T, U, Z, X, W>(EventType eventType, Action<T, U, Z, X, W> func)
    {
        Delegate del = func;
        RegisterEvent(eventType, del);
    }

    public void RegisterEvent<T, U, Z, X, W, Y>(EventType eventType, Action<T, U, Z, X, W, Y> func)
    {
        Delegate del = func;
        RegisterEvent(eventType, del);
    }

    public void PublicEvent(EventType eventType)
    {
        if (event_map.ContainsKey(eventType))
        {
            for (int i = 0; i < event_map[eventType].Count; i++)
            {
                Action func = event_map[eventType][i] as Action;
                func?.Invoke();
            }
        }
    }

    public void PublicEvent<T>(EventType eventType, T arg1)
    {
        if (event_map.ContainsKey(eventType))
        {
            for (int i = 0; i < event_map[eventType].Count; i++)
            {
                Action<T> func = event_map[eventType][i] as Action<T>;
                func?.Invoke(arg1);
            }
        }
    }

    public void PublicEvent<T, U>(EventType eventType, T arg1, U arg2)
    {
        if (event_map.ContainsKey(eventType))
        {
            for (int i = 0; i < event_map[eventType].Count; i++)
            {
                Action<T, U> func = event_map[eventType][i] as Action<T, U>;
                func?.Invoke(arg1, arg2);
            }
        }
    }

    public void PublicEvent<T, U, Z>(EventType eventType, T arg1, U arg2, Z arg3)
    {
        if (event_map.ContainsKey(eventType))
        {
            for (int i = 0; i < event_map[eventType].Count; i++)
            {
                Action<T, U, Z> func = event_map[eventType][i] as Action<T, U, Z>;
                func?.Invoke(arg1, arg2, arg3);
            }
        }
    }

    public void PublicEvent<T, U, Z, X>(EventType eventType, T arg1, U arg2, Z arg3, X arg4)
    {
        if (event_map.ContainsKey(eventType))
        {
            for (int i = 0; i < event_map[eventType].Count; i++)
            {
                Action<T, U, Z, X> func = event_map[eventType][i] as Action<T, U, Z, X>;
                func?.Invoke(arg1, arg2, arg3, arg4);
            }
        }
    }

    public void PublicEvent<T, U, Z, X, W>(EventType eventType, T arg1, U arg2, Z arg3, X arg4, W arg5)
    {
        if (event_map.ContainsKey(eventType))
        {
            for (int i = 0; i < event_map[eventType].Count; i++)
            {
                Action<T, U, Z, X, W> func = event_map[eventType][i] as Action<T, U, Z, X, W>;
                func?.Invoke(arg1, arg2, arg3, arg4, arg5);
            }
        }
    }

    public void PublicEvent<T, U, Z, X, W, Y>(EventType eventType, T arg1, U arg2, Z arg3, X arg4, W arg5, Y arg6)
    {
        if (event_map.ContainsKey(eventType))
        {
            for (int i = 0; i < event_map[eventType].Count; i++)
            {
                Action<T, U, Z, X, W, Y> func = event_map[eventType][i] as Action<T, U, Z, X, W, Y>;
                func?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
            }
        }
    }

    public void UnRegisterEvent(EventType eventType, Delegate func)
    {
        if (event_map.ContainsKey(eventType))
        {
            event_map[eventType].Remove(func);
            if (event_map[eventType].Count == 0)
            {
                event_map.Remove(eventType);
            }
        }
    }

    public void UnRegisterEvent(EventType eventType, Action func)
    {
        Delegate del = func;
        UnRegisterEvent(eventType, del);
    }

    public void UnRegisterEvent<T>(EventType eventType, Action<T> func)
    {
        Delegate del = func;
        UnRegisterEvent(eventType, del);
    }

    public void UnRegisterEvent<T, U>(EventType eventType, Action<T, U> func)
    {
        Delegate del = func;
        UnRegisterEvent(eventType, del);
    }

    public void UnRegisterEvent<T, U, Z>(EventType eventType, Action<T, U, Z> func)
    {
        Delegate del = func;
        UnRegisterEvent(eventType, del);
    }

    public void UnRegisterEvent<T, U, Z, X>(EventType eventType, Action<T, U, Z, X> func)
    {
        Delegate del = func;
        UnRegisterEvent(eventType, del);
    }

    public void UnRegisterEvent<T, U, Z, X, W>(EventType eventType, Action<T, U, Z, X, W> func)
    {
        Delegate del = func;
        UnRegisterEvent(eventType, del);
    }

    public void UnRegisterEvent<T, U, Z, X, W, Y>(EventType eventType, Action<T, U, Z, X, W, Y> func)
    {
        Delegate del = func;
        UnRegisterEvent(eventType, del);
    }
}