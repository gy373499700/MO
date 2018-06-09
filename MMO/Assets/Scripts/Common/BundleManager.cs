using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ResLoadParams
{
    public Vector3 pos;
    public Vector3 scale;
    public Quaternion rot;
    public string info;

    public object userdata0 = null;
    public object userdata1 = null;
    public object userdata2 = null;
    public object userdata3 = null;
    public object userdata4 = null;
    public object userdata5 = null;
    public uint _reqIndex;	//请求时间 请求者不需要填写..
    public string _name;    //特效池需要保存原始路径名..
    public float _LoadTime = 0f;
}

public delegate void ResLoadDelegate(ResLoadParams param, Object obj);
public class LoadRequest
{
    public ResLoadDelegate callbackFunc = null;
    public ResLoadParams param;
    public string path;//res/$Cube.prefab      全部小写，Unity内部也是一样不区分大小写的。
    public string resName;//$Cube  
    public string bundleName;//res/$Cube.prefab.unity3d小写
    public System.Type resType;
    public eLoadPriority priority =  eLoadPriority.eLP_Middle;
    public bool isScene = false;
    public void DoCallBack(Object obj)
    {
        if (callbackFunc != null)
        {
            if (obj == null)
            {
                Debug.Log(path + " Load Failed!");
                //return;
            }
            callbackFunc(param, obj);
        }

    }
}


public class BundleGlobalItem
{
    public List<TFileInfo> selfileLst = new List<TFileInfo>();//该bundle内含的资源
    public AssetBundle bundle;
    public string bundleName = "";
    public bool dontUnload = false;//judge first
    public bool IsStreamAssets = true;
    public void OnSceneDestroy()
    {//when scene switch
        if (dontUnload) return;
        for (int i = 0; i < selfileLst.Count; i++)
        {
            if (selfileLst[i].IsFileFree() == false)
            {
                selfileLst[i].OnDestroy(true);
            }
        }
      
    }
    public void OnObjectDestroy()
    {//when a objectbundle be unload(true)
        if (dontUnload) return;
        int index = 0;
        for (int i = 0; i < selfileLst.Count; i++)
        {
            if (selfileLst[i].IsFileFree()==false)//只卸载已经加载的
            {
                //Debug.Log(selfileLst[i].bundleName + "   " + selfileLst[i].pathName);
                index++;
                selfileLst[i].OnDestroy(false);////////////////???????????
            }
        }
#if UNITY_EDITOR
        if (index > 1)
        {
            Debug.LogError("该资源不受独立的bundle，可能产生资源丢失或者无法加载,而且引用计数已坏，请修改！");
        }
#endif
    }
    bool IsSelfFileFree()
    {//该bundle的自己的资源源都free
        if (dontUnload) return false;
        bool free = true;
        for (int i = 0; i < selfileLst.Count; i++)
        {
            if (!selfileLst[i].IsFileFree())
            {
                free = false;
                break;
            }
        }
        return free;
    }
    public void UnLoadUnusedBundle()
    {///when a objectbundle be unload(true) and UnLoad Unused Ubndle
        if (dontUnload) return;
        int index = 0;
        List<TFileInfo> fileLst = new List<TFileInfo>();
        for (int i = 0; i < selfileLst.Count; i++)
        {
            if (selfileLst[i].IsFileFree() == false)//只卸载已经加载的
            {//应该先过滤场景bundle
                index++;
                selfileLst[i].OnDestroy(false);
                fileLst.AddRange(selfileLst[i].GetFilesDependcy());
            }
        }
#if UNITY_EDITOR
        if (index > 1)
        {
            Debug.LogError("该资源不受独立的bundle，可能产生资源丢失或者无法加载,而且引用计数已坏，请修改！");
        }
#endif
        Dictionary<string, int> freebundle = new Dictionary<string, int>();

        for (int i = 0; i < fileLst.Count; i++)//可能包含了多份相同依赖文件
        {//找出该bundle包含文件的所有未被依赖的资源 bundleName
            if (fileLst[i].IsFileFree())
            {
                if(!BundleManager.Instance.SceneDependentBundleName.Contains(fileLst[i].bundleName))
                    freebundle[fileLst[i].bundleName] = 1;
            }
        }
       
        
        foreach(KeyValuePair<string,int> kv in freebundle)
        {//计算每个bundle自己的资源是否全部自由了
            BundleGlobalItem bundle=BundleManager.Instance.AllLoadedBundle[kv.Key];
            if (bundle.bundle!=null&&bundle.IsSelfFileFree())
            {
                bundle.bundle.Unload(true);
                bundle.bundle = null;
                bool remove= BundleManager.Instance.CurrentExistBundle.Remove(bundle);
#if UNITY_EDITOR
                Debug.Log("UnLoadUnusedBundle : " + kv.Key+"   "+ remove);
#endif
            }
        }




    }
}/*
public class UnCompressTask
{
    public BundleGlobalItem item;
    public byte[] data;
}
public class CopyFlieTask
{
    public string path;
    public byte[] data;
}*/
public class VersionItem
{
    public bool _BundleTest;
    public string BundlePath = "";
    public string appName;
    public bool BundleTest
    {
        get
        {
           
            if (Application.isEditor)
                return _BundleTest;
            else
                return false;
        }
        set { _BundleTest = value; }
    }
  //  public bool EditorNeedUpdate = false;
 //   public string EditorUpdatePath = "";
    private bool StreamAssetsPageCompress = false;
    public int Area = 0;
    public string AreaUrl;
    public int versionCode;
    public string versionName;
    public uint versionNameCode;
    public bool BreakJail;
    public string BundleIdentifier;
    public int Plugin;
    public string ProductName;
    public bool FullResource;
    public bool Is64;
    public bool iosCompress;//is StreamAssetsPageCompress

    public void LoadVersion(string txt)
    {
        Dictionary<string, string> tab = BundleManager.ReadINI(txt);
        _BundleTest = int.Parse(tab["BundleTest"]) == 1;
        BundlePath = tab["BundlePath"];
        StreamAssetsPageCompress = int.Parse(tab["StreamAssetsCompress"])==1;
        appName = tab["appName"];
        Area = int.Parse(tab["Area"]);
        AreaUrl = tab["AreaUrl"];
        versionCode = int.Parse(tab["versionCode"]);
        versionName = tab["versionName"];
        BreakJail = int.Parse(tab["BreakJail"])==1;
        BundleIdentifier = tab["BundleIdentifier"];
        ProductName = tab["ProductName"];
        FullResource = int.Parse(tab["FullResource"])==1;
        Is64 = int.Parse(tab["Is64"])==1;
        Plugin = int.Parse(tab["Plugin"]);
        iosCompress = int.Parse(tab["iosCompress"])==1;
    }
}
public class BundleManager : Singleton<BundleManager>
{
    #region Setting
    public static VersionItem Version = new VersionItem(); 
    public static string bundlePath
    {
        get
        {
            if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                return Application.dataPath + "/Bundles/";
            }
            else if(Application.platform== RuntimePlatform.Android)
            {//mean android streamingAssetsPath
                return Application.dataPath + "!assets/Bundles/";
            }///jar:file:/data/app/com.SDGame.DSHDTESTW-1/base.apk!/assets/AppVersion.txt
            else if ( Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return Application.streamingAssetsPath + "/Bundles/";
            }
            else
            {
                return Version.BundlePath;// Application.dataPath.Replace("Assets", "") + "Bunldes/";
            }
        }
    }
    


    public static bool bundleTest
    {
        get
        {
            return Version.BundleTest;
        }
    }
    #endregion
    public override void Init()
    {
        StartCoroutine(StartInit());
    }
    IEnumerator  StartInit()
    {
      //  return;
        base.Init();
        string data;
        string url = Application.streamingAssetsPath + "/AppVersion.txt";
        if (Application.platform == RuntimePlatform.Android)
        {
            WWW  wwwPercent = new WWW(url);
            yield return wwwPercent;
            data = wwwPercent.text;
        }
        else
        {
            data = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/AppVersion.txt");
        } 
        Version.LoadVersion(data);
        //version
        string MainBunldes = bundlePath + "Bundles";

        AssetBundle manifestBundle = AssetBundle.LoadFromFile(MainBunldes);
        if (manifestBundle != null)
        {
            manifest = (AssetBundleManifest)manifestBundle.LoadAsset("AssetBundleManifest");
            InitAllBundle(manifest);//init bundle info
            string content;
          
            if (Application.platform == RuntimePlatform.Android)
            {
                string fileinfo = Application.streamingAssetsPath + "/Bundles/fileinfo.txt";
                WWW wwwPercent = new WWW(fileinfo);
                yield return wwwPercent;
                content = wwwPercent.text;
                Debug.Log(content);
            }
            else
            {
                string fileinfo = bundlePath + "fileinfo.txt";
                content = File.ReadAllText(fileinfo);
            }
            TFileInfo.LoadFileInfo(content, AllLoadedBundle);
        }else
        {
            Debug.Log("No bunlde mode");
        }
        //streamasset
  
        //file
        StartCoroutine(updateLoad());
    }
    #region UpdateProgress
    IEnumerator UpdateAllBundle()
    {
        yield return null;
    }
    #endregion

    #region LoadProcess
    private List<LoadRequest> loadingObjsFast = new List<LoadRequest>();
    private List<LoadRequest> loadingObjs = new List<LoadRequest>();
    private List<LoadRequest> loadingObjsSlow = new List<LoadRequest>();
    public AsyncOperation sceneOperation = null;
    AssetBundleManifest manifest = null;
    public List<string> SceneDependentBundleName = new List<string>();//当前场景依赖的bundle 不含自己
    LoadRequest PopLoadItem()
    {
        LoadRequest item = null;
        if (loadingObjsFast.Count > 0)
        {
            item = loadingObjsFast[0];
            loadingObjsFast.RemoveAt(0);
        }
        else if (loadingObjs.Count > 0)
        {
            item = loadingObjs[0];
            loadingObjs.RemoveAt(0);
        }
        else if (loadingObjsSlow.Count > 0)
        {
            item = loadingObjsSlow[0];
            loadingObjsSlow.RemoveAt(0);

        }
        if (item.isScene)
        { //如果是场景，发一个心跳包，防止加载场景慢 服务器判定断线.....
        }
        return item;
    }
    IEnumerator updateLoad()
    {
        while (true)
        {
            if (loadingObjs.Count == 0 && loadingObjsFast.Count == 0 && loadingObjsSlow.Count == 0)
            {
                yield return 0;
            }
            else
            {
                LoadRequest req = PopLoadItem();
                if (req.bundleName == null|| req.bundleName == "")
                {
                    req.DoCallBack(null);
                    yield return 0;
                    continue;
                }
                if (!AllLoadedBundle.ContainsKey(req.bundleName))
                {
                    Debug.Log("Bundle Don't Exist" + req.bundleName + req.path);
                    req.DoCallBack( null);
                    continue;
                }
                BundleGlobalItem globalItem = AllLoadedBundle[req.bundleName];
                TFileInfo file = TFileInfo.GetFile(req.path);
                if (file == null)
                {
                    Debug.Log("Load TFileInfo Object Failed!=" + req.path);
                    req.DoCallBack(null);
                    continue;
                }
                if (globalItem == null)
                {
                    Debug.Log("Load globalItem Object Failed!=" + req.path);
                    req.DoCallBack(null);
                    continue;
                }
                AssetBundle item = globalItem.bundle;
                if (req.isScene)
                {//考虑场景加载前的任务不加载，场景加载后的资源要处理
                    float LoadTime = Time.realtimeSinceStartup;
                    EffectPool.Instance.Clear();
                    UIManager.Instance.Clear();
                    List<string> dependents = file.GetBundleDependcy();//string[] dependents3 =  manifest.GetAllDependencies(req.bundleName); //
                 //   List<string> dontUnloadlst = new List<string>();
                 //   dontUnloadlst.Add(req.bundleName);
                    Debug.Log("current  dependents : " + CurrentExistBundle.Count+ "     next scene dependents : " + dependents.Count);
                   // for (int index = 0; index < dependents.Count; ++index)
                    {
                    //    dontUnloadlst.Add(dependents[index]);
                    }
                    UnloadSceneBundle(dependents);
                    ResourceMgr.Instance.Clear();
                    int x = 0;
                    for (int index = 0; index < dependents.Count; ++index)
                    {
                        if (AllLoadedBundle[dependents[index]].bundle == null)
                        {
                            AssetBundle bundle = LoadBundle(dependents[index]);//场景如果过大，后边可以优化成异步加载看是否会快点？
                            SaveBundle(dependents[index], bundle);
                            x++;
                        }
                    }
                    if (item == null)
                    {   x++;
                        item = LoadBundle(req.bundleName);
                        SaveBundle(req.bundleName, item);
                    }
                    Debug.Log("next scene dependents load: " + x);
                    sceneOperation = SceneManager.LoadSceneAsync(req.resName);
                    while (true)
                    {//计算加载流程
                        if (sceneOperation == null)
                        {
                            break;
                        }
                        if (sceneOperation.isDone)
                        {
                            break;
                        }
                       // Debug.Log(" load level progress "+sceneOperation.progress);
                        yield return null;
                    }
                    SceneDependentBundleName = dependents;
                    file.OnCreate(true);
                    // if (item != null )
                    {//正常情况场景切换不管bundle清不清，缓存资源都会清理
                     //但是当场景依赖资源和逻辑资源共用时，unload（false）会导致缓存资源无法卸载干净，一直存在内存中
                        //so场景加载完bundle不能卸载，要等下一次切换场景的时候卸载，不然该部分的卸载资源会冗余
                      /* for (int index = 0; index < dependents.Length; ++index)
                        {
                            UnloadBundle(dependents[index]);

                        }*/
                       // UnloadBundle(req.bundleName);//切换场景时 如果scene的bundle早已经被清了 就需要手动unload(true) 否则资源就一直卸载不掉
                       //这里考虑到场景万一场景切换有其他数据需要bundle依赖，也许场景bundle数据会重复冗余。
                    }
                    Debug.Log("Load Scene Time is " +(Time.realtimeSinceStartup- LoadTime));
                   // Resources.UnloadUnusedAssets(); 切换场景会自动清空未引用的资源
                }
                else
                {
                    Object callback_obj = null;

                    //if (item == null)改成按文件依赖加载了
                    {
                        List<string> dependents = file.GetBundleDependcy();// manifest.GetAllDependencies(req.bundleName);
                        //依赖关系要重新做，要获取到具体资源的依赖关系，而不是bundle所有资源的依赖关系。
                        if ((int)req.priority > (int)eLoadPriority.eLP_Low)
                        {
                            // AssetBundle[] dependsAssetbundle = new AssetBundle[dependents.Length];
                            for (int index = 0; index < dependents.Count; index++)
                            {  //加载所有的依赖文件;  
                                if (AllLoadedBundle[dependents[index]].bundle == null)
                                {
                                    AssetBundle bundle = LoadBundle(dependents[index]);
                                    SaveBundle(dependents[index], bundle);
                                }
                            }

                            if (item == null)
                            {
                                if (AllLoadedBundle[req.bundleName].bundle == null)
                                    item = LoadBundle(req.bundleName);
                                else
                                    item = AllLoadedBundle[req.bundleName].bundle;//上步可能已经加载
                            }
                                
                        }
                        else
                        {//total async
                            AssetBundleCreateRequest[] dependsAssetbundle = new AssetBundleCreateRequest[dependents.Count];
                            for (int index = 0; index < dependents.Count; index++)
                            {  //加载所有的依赖文件;  
                                if (AllLoadedBundle[dependents[index]].bundle == null)
                                {
                                    dependsAssetbundle[index] = LoadBundleAsync(dependents[index]);
                                    if(dependents[index]== req.bundleName)
                                    {//in case of
                                        yield return dependsAssetbundle[index];//
                                        item = dependsAssetbundle[index].assetBundle;
                                    }
                                }
                            }

                            if (item == null)
                            {
                                AssetBundleCreateRequest reqABR = LoadBundleAsync(req.bundleName);
                                yield return reqABR;//
                                item = reqABR.assetBundle;

                            }
                            while (true)
                            {
                                bool AllDone = true;
                                for (int i = 0; i < dependsAssetbundle.Length; i++)
                                {
                                    if (dependsAssetbundle[i] != null)
                                    {
                                        if (dependsAssetbundle[i].isDone == false)
                                        {
                                            AllDone = false;
                                            yield return null;
                                            break;
                                        }
                                        else
                                        {
                                            SaveBundle(dependents[i], dependsAssetbundle[i].assetBundle);
                                        }
                                    }
                                }
                                if (AllDone)
                                    break;
                            }
                        }
                    }
                    if (item != null)
                    {
                        SaveBundle(req.bundleName, item);

                        AssetBundleRequest abr = item.LoadAssetAsync(req.resName, req.resType);
                        yield return abr;
                        callback_obj = abr.asset;
                        file.OnCreate(false);
                    }
                    req.DoCallBack(callback_obj);
                }
            }
        }
    }
    AssetBundleCreateRequest LoadBundleAsync(string bundleName)
    {
        AssetBundleCreateRequest bundle = null;
        string path = bundlePath + bundleName;
        try
        {
            if (Application.isEditor || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                bundle = AssetBundle.LoadFromFileAsync(path);
            }
            else
            {

                bundle = AssetBundle.LoadFromFileAsync(path);//adnroid

            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message + "\n" + e.StackTrace);
        }
        if (bundle == null)
        {
            Debug.Log(bundleName + " Load Failed\n");
        }

        //Debug.Log("LoadBundle=[" + bundlecount + "]" + item.itemInfo.bundlePath);

        return bundle;
    }
    AssetBundle LoadBundle(string bundleName)
    {
        AssetBundle bundle = null;
        string path = bundlePath + bundleName;
        try
        {
            if (Application.isEditor || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                bundle = AssetBundle.LoadFromFile(path);
            }
            else
            {

                bundle = AssetBundle.LoadFromFile(path);//adnroid

            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message + "\n" + e.StackTrace);
        }
        if (bundle == null)
        {
            Debug.Log(bundleName + " Load Failed\n");
        }
        //Debug.Log("LoadBundle=[" + bundlecount + "]" + item.itemInfo.bundlePath);

        return bundle;
    }
    public void LoadObject(LoadRequest loadobj)
    {
        if (IsMobile())
        {
            if (loadobj.isScene)
            {
                loadingObjs.Add(loadobj);
            }
            else
            {
                if (loadobj.priority == eLoadPriority.eLP_High)
                {
                    loadingObjsFast.Add(loadobj);
                }
                else if (loadobj.priority == eLoadPriority.eLP_Middle)
                {  
                    loadingObjs.Add(loadobj);
                }
                else if (loadobj.priority == eLoadPriority.eLP_Low)
                {
                    loadingObjsSlow.Add(loadobj);
                }
                else
                {
                    loadingObjsSlow.Add(loadobj);
                }
            }
        }
        else
        {
            if (loadobj.isScene)
            {
                UIManager.Instance.Clear();
                ResourceMgr.Instance.Clear();
                EffectPool.Instance.Clear();

                string bundlePath = loadobj.bundleName;
                //检查手机加载路径的正确性，如果错误 日志提示..
                if (!bundlePath.EndsWith(".unity3d"))
                {
                    Debug.LogError("Level Path Isn't correct!(" + bundlePath + ")!");
                }
                else
                {
                    string strPath = loadobj.path;// bundlePath.Replace(".unity3d", "");
#if UNITY_EDITOR
                    Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/" + strPath, typeof(Object));
                    if (obj == null)
                    {
                        Debug.LogError("Level Path Isn't correct!(" + bundlePath + ")!");
                    }
                    else
                    {
                        Resources.UnloadAsset(obj);
                    }
#endif
                }

                SceneManager.LoadScene(loadobj.resName);
            }
            else
            {
                if (loadobj.resType != typeof(AssetBundle))
                {
                    StartCoroutine(DoLoadFromFile(loadobj));
                }
            }
        }
    }
    static IEnumerator DoLoadFromFile(LoadRequest req)
    {//模拟手机异步
        yield return null;
#if UNITY_EDITOR
        Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/" + req.path, req.resType);
        req.DoCallBack(obj);
#endif
    }
    #endregion



    #region  优化和资源依赖管理
    public Dictionary<string, BundleGlobalItem> AllLoadedBundle = new Dictionary<string, BundleGlobalItem>();//bundlename,Allbundle,改成一开始从配置文件全部初始化好
    public List<BundleGlobalItem> CurrentExistBundle = new List<BundleGlobalItem>();//当前现存的bundle_

    void InitAllBundle(AssetBundleManifest manifest)
    {
        string[] dependents = manifest.GetAllAssetBundles();
        for(int i=0;i< dependents.Length; i++)
        {
            BundleGlobalItem loadedbundle = new BundleGlobalItem();
            loadedbundle.bundleName = dependents[i];
            AllLoadedBundle[dependents[i]] = loadedbundle;
        }
        Debug.Log("All Bundle Count " + dependents.Length);
    }
    public void MarkBundleDontUnload(string bundleName)//必须设置bundle的全部依赖不卸载，不能只设置期中某个文件的依赖不卸载。
    {//会导致该bundle的所有其他资源和依赖不会被卸载   ！！！！！！！！！！！！！！
        //但是切换场景的依赖资源会被卸载，只要那部分资源没有和不卸载的重合，重合部分无法卸载。
        if (!IsMobile()) return;
        if (manifest == null || !AllLoadedBundle.ContainsKey(bundleName))
        {
            Debug.LogError("first InitAllBundle " + bundleName + "    " + manifest);
            return;
        }
#if UNITY_EDITOR
        if (AllLoadedBundle[bundleName].selfileLst.Count > 1)
        {
            Debug.Log("设置不卸载的资源 最好单独打成bundle,否则所有都会不卸载导致内存过大！" + bundleName);//but没有冗余
        }

#endif
        AllLoadedBundle[bundleName].dontUnload = true;
        string[] dependents = manifest.GetAllDependencies(bundleName);
        for (int i = 0; i < dependents.Length; i++)
        {
            AllLoadedBundle[dependents[i]].dontUnload = true;
        }

    }
    void SaveBundle(string bundleName,AssetBundle bundle)
    {
        if (bundle == null) return;
        AllLoadedBundle[bundleName].bundle = bundle;
        if (!CurrentExistBundle.Contains(AllLoadedBundle[bundleName]))
            CurrentExistBundle.Add(AllLoadedBundle[bundleName]);
        if (CurrentExistBundle.Count > 250)
        {
            Debug.LogError("可能达到了IOS的最大句柄数限制了");
        }
    }
    
    public void UnloadBundle(string bundleName)
    {//该接口不能随便调
        if (!IsMobile()) return;
        if (AllLoadedBundle[bundleName].bundle != null)
        {
#if UNITY_EDITOR
            string[] allasset = AllLoadedBundle[bundleName].bundle.GetAllAssetNames();
            if (allasset.Length > 1)
            {
                Debug.LogError("强制卸载的资源 只能单独达成一个bundle  否则会导致可能其他资源丢失" + bundleName);
            }
#endif
            if (SceneDependentBundleName.Contains(bundleName))
            {
                Debug.LogError("UnloadBundle error 不能卸载场景依赖的资源" + bundleName);
                return;
            }
            if (AllLoadedBundle[bundleName].dontUnload == false)
            {//unload(false)少用，因为单纯卸载bundle减少的内存有限，而且导致资源引用丢失无法彻底卸载缓存资源
                //（就算切换场景和UnloadUnusedAssets，贴图登资源也无法清除）,当然也有可能是Unity设置了内存上限后才清。无论如何会导致内存上涨。
                AllLoadedBundle[bundleName].bundle.Unload(true);
                AllLoadedBundle[bundleName].bundle = null;
                AllLoadedBundle[bundleName].OnObjectDestroy();
                CurrentExistBundle.Remove(AllLoadedBundle[bundleName]);
                Debug.Log("UnloadBundle=" + bundleName);
            }
        }
        else
        {
            Debug.LogError("UnloadBundle failed =" + bundleName);
        }
    }
    public void UnloadUnUsedBundle(string bundleName)
    {//该接口更不能随便调 不仅会删掉被资源，还会把本资源引用的资源 如果计数为0  也清空、
        if (!IsMobile()) return;
        if (AllLoadedBundle[bundleName].bundle != null)
        {
#if UNITY_EDITOR
            string[] allasset = AllLoadedBundle[bundleName].bundle.GetAllAssetNames();
            if (allasset.Length > 1)
            {
                Debug.LogError("强制卸载的资源 只能单独达成一个bundle  否则会导致可能其他资源丢失" + bundleName);
            }
#endif
            if (SceneDependentBundleName.Contains(bundleName))
            {
                Debug.LogError("UnloadUnUsedBundle error 不能卸载场景依赖的资源" + bundleName);
                return;
            }
            if (AllLoadedBundle[bundleName].dontUnload == false)//????????????过滤场景bundle
            {//unload(false)少用，因为单纯卸载bundle减少的内存有限，而且导致资源引用丢失无法彻底卸载缓存资源
                //（就算切换场景和UnloadUnusedAssets，贴图登资源也无法清除）,当然也有可能是Unity设置了内存上限后才清。无论如何会导致内存上涨。
                AllLoadedBundle[bundleName].bundle.Unload(true);
                AllLoadedBundle[bundleName].bundle = null;
                AllLoadedBundle[bundleName].UnLoadUnusedBundle();
                CurrentExistBundle.Remove(AllLoadedBundle[bundleName]);
                Debug.Log("UnloadBundle=" + bundleName);
            }
        }
        else
        {
            Debug.LogError("UnloadBundle failed =" + bundleName);
        }
      
    }
    void UnloadSceneBundle(List<string> dontUnloadlst)
    {//场景卸载流程没问题
        List<BundleGlobalItem> exist = new List<BundleGlobalItem>();
        int preCount = CurrentExistBundle.Count;
        int deleteIndex = 0;
        int unloadIndex = 0;
        for (int i = 0; i < CurrentExistBundle.Count; i++)
        {
            if (CurrentExistBundle[i] == null)
            {
                Debug.LogError(i);
                continue;
            }
            if (CurrentExistBundle[i].bundle != null)
            {

                if (CurrentExistBundle[i].dontUnload)
                {
                    unloadIndex++;
                    exist.Add(CurrentExistBundle[i]);
                    continue;
                }
                if (dontUnloadlst != null)
                {
                    if (dontUnloadlst.Contains(CurrentExistBundle[i].bundleName))
                    {
                        exist.Add(CurrentExistBundle[i]);
                        continue;
                    }
                }
                deleteIndex++;
                CurrentExistBundle[i].bundle.Unload(true);//false没法卸载缓存资源，会导致内存比不卸载更多
                CurrentExistBundle[i].bundle = null;
                CurrentExistBundle[i].OnSceneDestroy();
            }
        }
        CurrentExistBundle.Clear();
        for (int i = 0; i < exist.Count; i++)
        {
            if (exist[i].bundle != null)
                CurrentExistBundle.Add(exist[i]);
            else
                Debug.LogError("Error Logic " + exist[i].bundleName);
        }
#if UNITY_EDITOR
        Debug.Log("UnloadSceneBundle pre all " + preCount+ "    UnloadSceneBundle UnloadCount " + deleteIndex+ "     UnloadSceneBundle after all " + CurrentExistBundle.Count);
        if (unloadIndex > 50)
        {
            Debug.LogError("不卸载的bundle资源超过了50个 麻烦检查一下是不是有冗余的？" + unloadIndex);
        }
#endif
    }

    void UnLoadAllBundle()
    {//quit game
        foreach(KeyValuePair<string, BundleGlobalItem> kv in AllLoadedBundle)
        {
            if(kv.Value!=null && kv.Value.bundle != null)
            {
                kv.Value.bundle.Unload(true);
            }
        }
        AllLoadedBundle.Clear();
        CurrentExistBundle.Clear();
        Debug.Log("UnLoadAllBundle");
    }

    void OnApplicationQuit()
    {
        UnLoadAllBundle();
    }
    public void DebugBundle()
    {
#if UNITY_EDITOR
        string s = "";
        s += ("///////////////DebugBundle//////////////////////////") + "\n";
        s +="Max Bundle Size " + AllLoadedBundle.Count + "     CurrentBundle Size " + CurrentExistBundle.Count+"\n";
        for (int i = 0; i < CurrentExistBundle.Count; i++)
        {
            s += (CurrentExistBundle[i] + "    " + CurrentExistBundle[i].bundle) + "\n";
        }
   
        s += ("///////////////All//////////////////////////") + "\n";
        foreach (KeyValuePair<string, BundleGlobalItem> kv in AllLoadedBundle)
        {
            s += (kv.Key + "       "+kv.Value.bundle+"    "+kv.Value.dontUnload) + "\n";
        }
        Debug.Log(s);
        TFileInfo.DebugFileRef();
        ResourceMgr.Instance.DebugResourceDB();
#endif
    }
#endregion
    // Update is called once per frame


    #region API
    public static bool IsMobile()
    {
        return Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.Android ||
                Application.platform == RuntimePlatform.WP8Player ||
                Application.platform == RuntimePlatform.WindowsPlayer ||
                bundleTest;
    }
    public static string GetBundleNameFromPath(string path)
    {
        int flagId = path.LastIndexOf("$");
        string bundleName = path;
        if (flagId >= 0)
        {
            int folderFlagId = path.IndexOf("/", flagId);
            if (folderFlagId >= 0)
            {
                bundleName = path.Substring(0, folderFlagId);
            }
            bundleName += ".unity3d";
        }
        else
        {
            Debug.LogError("GetBundleNameFromPath error  " + path);
        }
        return bundleName.ToLower();
    }
    public static string GetResNameFromPath(string path)
    {//如果该路径
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
        return resName;
    }
    public static Dictionary<string, string> ReadINI(string content)
    {
        Dictionary<string, string> table = new Dictionary<string, string>();

        if (content.Length == 0)
        {
            return table;
        }
        string[] lines = content.Replace("\r", "").Split('\n');


        foreach (string s in lines)
        {
            if (s.Length > 0)
            {
                string[] element = s.Split('=');
                string val = element[1];
                for (int i = 2; i < element.Length; i++)
                {
                    val += "=" + element[i];
                }
                table[element[0]] = val;
            }
        }
        return table;
    }

    public static uint CalcCRCDataPath(string path)
    {

        FileStream read_stream = new FileStream(path, FileMode.Open);
        if (read_stream == null)
        {
            return 0;
        }
        if (read_stream.Length == 0)
        {
            read_stream.Close();
            return 0;
        }
        int len = (int)read_stream.Length;
        byte[] buffer = new byte[len];

        read_stream.Read(buffer, 0, len);
        read_stream.Close();

        return SevenZip.CRC.CalculateDigest(buffer, (uint)0, (uint)len);
    }
    public static int CalcDataPathLength(string path)
    {

        FileStream read_stream = new FileStream(path, FileMode.Open);
        if (read_stream == null)
        {
            return 0;
        }
        if (read_stream.Length == 0)
        {
            read_stream.Close();
            return 0;
        }
        int len = (int)read_stream.Length;
        return len;
    }
    #endregion


}