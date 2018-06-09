using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseSingleton : MonoBehaviour
{
    public virtual void OnLevelBegin(object userdata)
    {//virtual回调方式行不通除非abstract
    }
    public virtual void OnLevelEnd(object userdata)
    {//包括同场景切换

    }
    public virtual void OnReConnected(object userdata)
    {

    }
    public virtual void OnDisConnected(object userdata)
    {

    }
    public virtual void Init()
    {

    }

}


public class Singleton<T> : BaseSingleton where T : BaseSingleton
{
    private static T _instance;
    private static object _lock = new object();
    public static T GetInstance()
    {

        return Instance;
    }

    public static T Instance
    {
        get
        {
            if (applicationIsQuitting == true)
            {
                return null;
            }
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));
                    if (_instance == null)
                    {

                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = "(singleton) " + typeof(T).ToString();
                        if (Application.isPlaying)
                        {
                            DontDestroyOnLoad(singleton);
                        }
                        else
                        {
                            singleton.hideFlags = HideFlags.DontSave | HideFlags.DontSaveInBuild;
                        }
                        _instance.Init();

                    }
                    else
                    {

                    }

                    SingletonManager.RegistSingleton<T>(_instance);
                }
                return _instance;
            }
        }
    }

    private static bool applicationIsQuitting = false;
    /// <summary>
    /// When unity quits, it destroys objects in a random order.
    /// In principle, a Singleton is only destroyed when application quits.    
    /// If any script calls Instance after it have been destroyed,     
    ///   it will create a buggy ghost object that will stay on the Editor scene    
    ///   even after stopping playing the Application. Really bad!    
    /// So, this was made to be sure we're not creating that buggy ghost object.    
    /// </summary>
  
    public void OnDestroy()
    {
        SingletonManager.RemoveSingleton<T>(_instance);
        if (ClearMode == true)
            applicationIsQuitting = true;
        ClearMode = false;
    }

    private static bool ClearMode = false;
    public static void DestorySingleton()
    {
        lock (_lock)
        {
            if (_instance != null)
            {
                ClearMode = true;
                GameObject.Destroy(_instance.gameObject);
                _instance = null;
            }
        }
    }
}

public class SingletonManager : MonoBehaviour
{

    public static Dictionary<object, int> AllSingleton = new Dictionary<object, int>();
    public static void RegistSingleton<T>(T t) where T : BaseSingleton
    {
        AllSingleton[t] = 1;
    }
    public static void RemoveSingleton<T>(T t) where T : BaseSingleton
    {
        if (t != null)
            if (AllSingleton != null && AllSingleton.ContainsKey(t))
                AllSingleton.Remove(t);

    }
    public static void OnNotifyLevelStart(int sceneid, string sceneName, int scenetype, object userdata)
    {
        foreach (KeyValuePair<object, int> kv in AllSingleton)
        {
            BaseSingleton _Singleton = kv.Key as BaseSingleton;
            if (_Singleton != null)
            {
                _Singleton.OnLevelBegin(null);
            }
            //             System.Type fType = kv.Key.GetType();
            //             System.Reflection.MethodInfo method = fType.GetMethod("OnLevelBegin");
            //             if (method != null)
            //             {
            //                 method.Invoke(kv.Key, new object[] {1});
            //             }


            //   kv.Key.SendMessage("OnLevelBegin", "");
        }
    }
    public static void OnNotifyLevelEnd(int sceneid, string sceneName, int scenetype, object userdata)
    {
        foreach (KeyValuePair<object, int> kv in AllSingleton)
        {
            BaseSingleton _Singleton = kv.Key as BaseSingleton;
            if (_Singleton != null)
            {
                _Singleton.OnLevelEnd(null);
            }
        }
    }

    public static void OnNotifyReConnect()
    {
        Debug.Log("OnNotifyReConnect");
        foreach (KeyValuePair<object, int> kv in AllSingleton)
        {
            BaseSingleton _Singleton = kv.Key as BaseSingleton;
            if (_Singleton != null)
            {
                _Singleton.OnReConnected(null);
            }
        }
    }
    public static void OnNotifyDisConnect()
    {
        Debug.Log("OnNotifyDisConnect");
        foreach (KeyValuePair<object, int> kv in AllSingleton)
        {
            BaseSingleton _Singleton = kv.Key as BaseSingleton;
            if (_Singleton != null)
            {
                _Singleton.OnDisConnected(null);
            }
        }
    }


}
public class GameSingleton : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
