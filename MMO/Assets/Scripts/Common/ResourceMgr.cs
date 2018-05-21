using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 1.预加载调用PreLoadResource，纯异步加载，速度稍慢，几乎不卡。
/// 2.LoadResource:半异步加载（建议）
/// 3.LoadResourceSlow，纯异步加载（不需要立即显示或者超大文件建议调这个  不卡）
/// 4.LoadResourceImmediately，优先半异步加载
/// 5.bundle文件LoadResource比LoadResourceSlow速度略快
/// 6.大资源Resource文件LoadResourceSlow异步比LoadResource同步速度快,小资源相反。
/// 
//////////////////////资源规范////////////////////////////////
//1.大部分资源都走bundle模式，包括场景，UI，配置表，这些都容易出错需要更新。最好是所有资源都能更新。
//2.对于一部分确实完全不用改的，少量放在Resource目录。
//
//
//by gardonguo
/// </summary>

public enum  eLoadPriority
{
    eLP_Low_PreLoad,//最低优先级纯异步加载
    eLP_Low,//低优先级纯异步加载
    eLP_Middle,//半异步加载
    eLP_High,//高优先级半异步加载
    eLP_Max = 4
}
public class TaskParam
{
    public ResLoadDelegate _cb;
    public ResLoadParams _param;
}
public class ResourceTask
{
    public string _name;
    public Object _object;
    public ResLoadParams _param;
    public List<TaskParam> lstTask = new List<TaskParam>();
    public bool failed = false;
    public uint Ref = 0;//.unused
    public bool dontUnload = false;//资源不卸载的话 可以使其bundle卸载，但是依赖的bundle不要卸载。
    public void AddCB(ResLoadDelegate cb, ResLoadParams param)
    {
        if (cb == null)
            return;
        TaskParam tp = new TaskParam();
        tp._cb = cb;
        tp._param = param;
        lstTask.Add(tp);
    }
    public void OnLoadFinished(Object obj)
    {
        _object = obj;
        if (_object == null)
        {
            failed = true;
        }
        for (int index = 0; index < lstTask.Count; ++index)
        {
            TaskParam tp = lstTask[index];
            if (tp._cb != null)
            {
                tp._param._LoadTime = Time.realtimeSinceStartup - tp._param._LoadTime;
                if (Application.isEditor && !BundleManager.bundleTest)
                {
                    tp._cb(tp._param, _object);
                }
                else
                {
                    try
                    {
                        tp._cb(tp._param, _object);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError(tp._cb.Method.Name + "\n" + e.Message + "\n" + e.StackTrace);
                    }
                }
            }
        }
        lstTask.Clear();
    }
}

public class ResourceMgr : Singleton<ResourceMgr>
{
    Hashtable resourceDB = new Hashtable();//key path大写

    uint index = 0;


    #region Load
    void AddTask(string orginPath, System.Type t, eLoadPriority priority)
    {

        string path = orginPath;
        ResLoadParams __param = new ResLoadParams();
        __param.info = orginPath;
        LoadRequest loadobj = new LoadRequest();
        loadobj.param = __param;
        loadobj.callbackFunc = resourceLoadCallback;
        loadobj.path = path.ToLower();
        loadobj.resType = t;
        loadobj.priority = priority;
        int tmpId = path.LastIndexOf("/");
        string resName = path;
        if (tmpId >= 0)
        {
            resName = path.Substring(tmpId + 1);
        }
        int dotIndex = resName.LastIndexOf(".");
        if (dotIndex >= 0)
        {
            resName = resName.Substring(0, dotIndex);
        }
        loadobj.resName = resName;

        int flagId = path.LastIndexOf("$");
        if (flagId >= 0)
        {
            int folderFlagId = path.IndexOf("/", flagId);
            string bundleName = path;
            if (folderFlagId >= 0)
            {
                bundleName = path.Substring(0, folderFlagId);
            }
            bundleName += ".unity3d";
            loadobj.bundleName = bundleName.ToLower();
            BundleManager.Instance.LoadObject(loadobj);
        }
        else
        {
            if (path.StartsWith("Resources/"))
            {
                string respath = path.Replace("Resources/", "");
                dotIndex = respath.LastIndexOf(".");
                if (dotIndex > 0)
                {
                    respath = respath.Substring(0, dotIndex);
                }
                if (priority > eLoadPriority.eLP_Low)
                {
                    Object obj = Resources.Load(respath, t);
                    loadobj.DoCallBack(obj);
                    return;
                }
                else
                {
                    StartCoroutine(ResourcesLoad(loadobj, respath));
                    return;
                }

            }
            if (path != "0")
            {
                Debug.LogError("bundle isn't exist!" + path);
            }
            return;
        }
    }
    IEnumerator ResourcesLoad(LoadRequest loadobj, string respath)
    {
        ResourceRequest req = Resources.LoadAsync(respath, loadobj.resType);
        yield return req;
        Object obj = null;
        if (req != null)
        {
            obj = req.asset;
        }
        loadobj.DoCallBack(req.asset);
    }
    public void PreLoadResource(string path,bool DontDestroy=false)
    {
        __PreLoadResource(path, 0, typeof(Object), eLoadPriority.eLP_Low_PreLoad);
        if(DontDestroy)
            MarkDontUnLoad(path);
    }
    public void PreLoadResource(string path, System.Type t, bool DontDestroy = false)
    {
        __PreLoadResource(path, 0, t, eLoadPriority.eLP_Low_PreLoad);
        if (DontDestroy)
            MarkDontUnLoad(path);
    }
    public void MarkDontUnLoad(string path)
    {//设置该资源的bundle的全部依赖资源不卸载
        string bundleName = BundleManager.GetBundleNameFromPath(path);
        BundleManager.Instance.MarkBundleDontUnload(bundleName);
#if UNITY_EDITOR
        string resName= BundleManager.GetResNameFromPath(path);
        if (!resName.Contains("$"))
        {
            Debug.Log("标志不卸载的资源最好独立打包！"+path);
        }
#endif
        if (resourceDB.ContainsKey(path))
        {
            ResourceTask task = resourceDB[path] as ResourceTask;
            task.dontUnload = true;
        }else
        {
            Debug.LogError("MarkDontUnLoad need load first " + path);
        }
    }
    void __PreLoadResource(string path, uint refCount, System.Type t, eLoadPriority priority)
    {
        if (path.Length == 0)
        {
            return;
        }
        bool resourceExist = resourceDB.ContainsKey(path);
        if (resourceExist)
        {
            ResourceTask task = resourceDB[path] as ResourceTask;
            if (task.Ref <= 0)
            {
                task.Ref = refCount;
            }
        }
        else
        {


            ResourceTask task = new ResourceTask();
            task._name = path;
            task._param = null;
            task.Ref = refCount;
            resourceDB[path] = task;

            AddTask(path, t, priority);
        }
    }
    public void LoadResourceSlow(string path, ResLoadDelegate cb, ResLoadParams param)
    {
        LoadResource(path, cb, param, typeof(Object), eLoadPriority.eLP_Low);
    }
    public void LoadResourceSlow(string path, ResLoadDelegate cb, ResLoadParams param, System.Type t)
    {
        LoadResource(path, cb, param, t, eLoadPriority.eLP_Low);
    }
    public void LoadResourceImmediately(string path, ResLoadDelegate cb, ResLoadParams param)
    {
        LoadResource(path, cb, param, typeof(Object), eLoadPriority.eLP_High);
    }
    public void LoadResourceImmediately(string path, ResLoadDelegate cb, ResLoadParams param, System.Type t)
    {
        LoadResource(path, cb, param, t, eLoadPriority.eLP_High);
    }
    public void LoadResource(string path, ResLoadDelegate cb, ResLoadParams param)
    {
        LoadResource(path, cb, param, typeof(Object), eLoadPriority.eLP_Middle);
    }
    public void LoadResource(string path, ResLoadDelegate cb, ResLoadParams param, System.Type t)
    {
        LoadResource(path, cb, param, t, eLoadPriority.eLP_Middle);
    }

    public void LoadResource(string path, ResLoadDelegate cb, ResLoadParams param, System.Type t, eLoadPriority priority)
    {
        if (cb == null)
        {
            return;
        }
        if (param != null)
        {
            param._reqIndex = index++;
        }
        else
        {
            index++;
        }
        if (t == typeof(GameObject))
        {

        }
        bool resourceExist = resourceDB.ContainsKey(path);//如果多个文件打到一个bundle里边卸载后，全部资源丢失，这儿会加载不进来。so设置不卸载和可以卸载的资源必须单独打包！
        if (resourceExist)
        {
            ResourceTask task = resourceDB[path] as ResourceTask;
            if (task._object != null || task.failed)
            {
                if (Application.isEditor)
                {
                    cb(param, task._object);
                }
                else
                {
                    try
                    {
                        cb(param, task._object);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError(cb.Method.Name + "\n" + e.Message + "\n" + e.StackTrace);
                    }

                }
            }
            else
            {
                task.AddCB(cb, param);
            }
        }
        else
        {
            ResourceTask task = new ResourceTask();
            task._name = path;
            task._param = param;
            task.AddCB(cb, param);
            resourceDB[path] = task;
            param._LoadTime = Time.realtimeSinceStartup;
            AddTask(path, t, priority);
        }
    }
    public void LoadLevel(string bundleName, string levelName)
    {
        LoadRequest loadobj = new LoadRequest();
        loadobj.isScene = true;
        loadobj.resName = levelName;
        loadobj.bundleName = bundleName.ToLower();
        loadobj.path = bundleName.Replace(".unity3d","").ToLower();
        loadobj.priority =  eLoadPriority.eLP_Middle;
        BundleManager.Instance.LoadObject(loadobj);
    }
    public void ClearTaskRef()
    {//login to home
        foreach (DictionaryEntry item in resourceDB)
        {
            ResourceTask task = item.Value as ResourceTask;
            if (task.Ref != 0xffffffff)
            {
                task.Ref = 0;
            }
        }
    }
    public void ClearAll()
    {//quit to login
        resourceDB.Clear();
    }
    public void Clear()
    {
        Hashtable table = resourceDB.Clone() as Hashtable;
        resourceDB.Clear();
        foreach (DictionaryEntry item in table)
        {
            ResourceTask task = item.Value as ResourceTask;
            if (task.lstTask.Count != 0 || task.dontUnload)// task.Ref != 0)
            {//|| task.dontUnload
                resourceDB.Add(item.Key, item.Value);
            }
        }
        table.Clear();
    }
    public static void resourceLoadCallback(ResLoadParams param, Object obj)
    {
        string fileName = param.info;

        ResourceTask task = ResourceMgr.Instance.resourceDB[fileName] as ResourceTask;
        if (task != null)
        {
            task.OnLoadFinished(obj);
        }
    }
    #endregion

    #region unload
    public void UnloadResource(string path)//only prefab
    {//仅卸载该资源缓存和其bundle文件，不卸载依赖文件。卸载prefab场景中的还有效，但是卸载Texture等Asset场景中会丢。
#if UNITY_EDITOR
        string resName = BundleManager.GetResNameFromPath(path);
        if (!resName.Contains("$"))
        {
            Debug.LogError("可以卸载的资源必须独立打包成Bundle！" + path);
            return;
        }
#endif
        bool resourceExist = resourceDB.ContainsKey(path);
        if (resourceExist)
        {
            resourceDB.Remove(path);
            string bundlename = BundleManager.GetBundleNameFromPath(path);
            BundleManager.Instance.UnloadBundle(bundlename);
        }
        else
        {
            Debug.LogError("UnloadResource failed!你要卸载的资源还没加载" + path);
        }
    }
    //该资源必须独立打包并且不会被任何其他资源引用
    public void UnloadUnsuedResource(string path)//only prefab
    {//慎用，场景中确定短期再也不用的资源销毁实例后可以调用这个 会卸载期缓存的全部不用的依赖资源
#if UNITY_EDITOR//如果该资源和场景中引用一样  可能会丢失
        string resName = BundleManager.GetResNameFromPath(path);
        if (!resName.Contains("$"))
        {
            Debug.LogError("可以卸载的资源必须独立打包成Bundle！并且不能直接在场景中引用" + path);
            return;
        }
#endif
        bool resourceExist = resourceDB.ContainsKey(path);
        if (resourceExist)
        {
            resourceDB.Remove(path);
            string bundlename = BundleManager.GetBundleNameFromPath(path);
            BundleManager.Instance.UnloadUnUsedBundle(bundlename);
        }
        else
        {
            Debug.LogError("UnloadUnsuedResource failed!你要卸载的资源还没加载" + path);
        }
    }


    #endregion



    public void DebugResourceDB()
    {
        string s = "";
        s+=("///////////////////DebugResourceDB//////////////////////\n");
        foreach(string key in resourceDB.Keys)
        { 
            ResourceTask task = resourceDB[key] as ResourceTask;
            s += (key + "   " + task._object+"\n");
        }
        Object[] objAry = Resources.FindObjectsOfTypeAll(typeof(Object));
        Object[] objAry2 = Resources.FindObjectsOfTypeAll(typeof(Texture));

        s += ("objAry size " + objAry.Length);
        s += ("   texture size " +objAry2.Length+"\n");
        Debug.Log(s);
    }

   
}
