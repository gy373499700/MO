using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Text;
public class ButtonClick : MonoBehaviour
{
    int Index = 0;
    float LoadTime = 0;
    private void Update()
    {
        Index++;
        if (Input.GetKeyDown(KeyCode.A))
        {//1.6

            //Scene a = SceneManager.CreateScene("aa");
            //  Debug.Log(a.path);
            ResourceMgr.Instance.LoadLevel("Level/$WW.unity.unity3d", "$WW");

            //             ResLoadParams kParam2 = new ResLoadParams();
            //             kParam2.info = "test2";
            //             ResourceMgr.Instance.LoadResourceSlow("UI/$Scene.prefab", OnPrefabImageNumberLoaded, kParam2);
            //             LoadTime = Time.realtimeSinceStartup;
            //             ResLoadParams kParam = new ResLoadParams();
            //             kParam.info = "test";
            //             ResourceMgr.Instance.LoadResource("UI/$Scene.prefab", OnPrefabImageNumberLoaded, kParam);

        }
        if (Input.GetKeyDown(KeyCode.B))
        {//1.69

            ResourceMgr.Instance.LoadLevel("Level/$level2.unity.unity3d", "$level2");


            //             LoadTime = Time.realtimeSinceStartup;
            //             ResLoadParams kParam = new ResLoadParams();
            //             kParam.info = "test";
            //             ResourceMgr.Instance.LoadResourceSlow("UI/$Scene.prefab", OnPrefabImageNumberLoaded, kParam);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {//2.5
            ResourceMgr.Instance.LoadLevel("Level/$empty.unity.unity3d", "$empty");
            //  ResourceMgr.Instance.LoadResource("UI/$Scene.prefab", OnPrefabImageNumberLoaded, kParam);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {//2.5

            ResLoadParams kParam = new ResLoadParams();
            kParam.info = "test";
            ResourceMgr.Instance.LoadResource("UI/$Big/Scene.prefab", OnPrefabImageNumberLoaded, kParam);
            //  ResourceMgr.Instance.LoadResource("UI/$Scene.prefab", OnPrefabImageNumberLoaded, kParam);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {//7.9
            LoadTime = Time.realtimeSinceStartup;
            ResLoadParams kParam2 = new ResLoadParams();
            kParam2.info = "test4";
            ResourceMgr.Instance.LoadResource("UI/$Big/Sphere.prefab", OnPrefabImageNumberLoaded, kParam2, typeof(GameObject));
        }
        if (Input.GetKeyDown(KeyCode.F10))
        {//2.5
            BundleManager.Instance.DebugBundle();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {//2.5
            LoadTime = Time.realtimeSinceStartup;
            ResLoadParams kParam2 = new ResLoadParams();
            kParam2.info = "test4";
            ResourceMgr.Instance.LoadResource("UI/$land1.prefab", OnPrefabImageNumberLoaded, kParam2, typeof(GameObject));
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {//2.5

            LoadTime = Time.realtimeSinceStartup;
            ResLoadParams kParam2 = new ResLoadParams();
            kParam2.info = "test4";
            ResourceMgr.Instance.LoadResource("UI/$land2.prefab", OnPrefabImageNumberLoaded, kParam2, typeof(GameObject)); ;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {//2.5

            LoadTime = Time.realtimeSinceStartup;
            ResLoadParams kParam2 = new ResLoadParams();
            kParam2.info = "test4";
            ResourceMgr.Instance.LoadResource("UI/$Scene.prefab", OnPrefabImageNumberLoaded, kParam2, typeof(GameObject)); ;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {//2.5

            LoadTime = Time.realtimeSinceStartup;
            ResLoadParams kParam2 = new ResLoadParams();
            kParam2.info = "test4";
            ResourceMgr.Instance.LoadResource("UI/$land/cardia_natural_palmtree01_02 (56).prefab", OnPrefabImageNumberLoaded, kParam2, typeof(GameObject)); ;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {//2.5

            LoadTime = Time.realtimeSinceStartup;
            ResLoadParams kParam2 = new ResLoadParams();
            kParam2.info = "test4";
            ResourceMgr.Instance.LoadResource("UI/$land3/nashgar_construct_rock_large02_01 (147).prefab", OnPrefabImageNumberLoaded, kParam2, typeof(GameObject)); ;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {//2.5

            LoadTime = Time.realtimeSinceStartup;
            ResLoadParams kParam2 = new ResLoadParams();
            kParam2.info = "test4";
            ResourceMgr.Instance.LoadResource("UI/$land/nashgar_construct_rock_large02_01 (129).prefab", OnPrefabImageNumberLoaded, kParam2, typeof(GameObject)); ;
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {//2.5

            LoadTime = Time.realtimeSinceStartup;
            ResLoadParams kParam2 = new ResLoadParams();
            kParam2.info = "test4";
            ResourceMgr.Instance.UnloadUnsuedResource("UI/$land1.prefab");
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {//2.5

            LoadTime = Time.realtimeSinceStartup;
            ResLoadParams kParam2 = new ResLoadParams();
            kParam2.info = "test4";
            ResourceMgr.Instance.UnloadUnsuedResource("UI/$land2.prefab");
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {//2.5

            LoadTime = Time.realtimeSinceStartup;
            ResLoadParams kParam2 = new ResLoadParams();
            kParam2.info = "test4";
            ResourceMgr.Instance.UnloadUnsuedResource("UI/$Scene.prefab");
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {//2.5

            LoadTime = Time.realtimeSinceStartup;
            ResLoadParams kParam2 = new ResLoadParams();
            kParam2.info = "test4";
            ResourceMgr.Instance.UnloadUnsuedResource("UI/$land/cardia_natural_palmtree01_02 (56).prefab");
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {//2.5

            LoadTime = Time.realtimeSinceStartup;
            ResLoadParams kParam2 = new ResLoadParams();
            kParam2.info = "test4";
            ResourceMgr.Instance.UnloadUnsuedResource("UI/$land3/nashgar_construct_rock_large02_01 (147).prefab");
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {//2.5

            LoadTime = Time.realtimeSinceStartup;
            ResLoadParams kParam2 = new ResLoadParams();
            kParam2.info = "test4";
            ResourceMgr.Instance.UnloadUnsuedResource("UI/$land/nashgar_construct_rock_large02_01 (129).prefab");
        }
        if (transform.GetComponent<Text>())
            transform.GetComponent<Text>().text = string.Format("{0}     {1}  ", Index, Time.time);
    }
    RawImage loadingTex = null;
    void OnTexLoad(ResLoadParams param, UnityEngine.Object obj)
    {
        if (obj == null)
        {
            Debug.LogError(param.info);
            return;
        }
        if (param.info == "loading")
        {
            GameObject m_PrefabImageNumber = (obj) as GameObject;
            GameObject test = GameObject.Instantiate(m_PrefabImageNumber);
            loadingTex = test.transform.FindChild("RawImage").GetComponent<RawImage>();
            DontDestroyOnLoad(test);
            //UIManager.SetNormalizedWindow(test);
        }
       else if (param.info == "x")
        {
            Texture m_PrefabImageNumber = (obj) as Texture;
            loadingTex.texture = m_PrefabImageNumber;
        }
        else if (param.info == "y")
        {
            Texture m_PrefabImageNumber = (obj) as Texture;
            loadingTex.texture = m_PrefabImageNumber;
        }
    }
    // Use this for initialization
    void OnEnable()
    {
        DontDestroyOnLoad(gameObject);
        Resources.UnloadUnusedAssets();
        ResLoadParams kParam2 = new ResLoadParams();
        kParam2.info = "loading";
        ResourceMgr.Instance.LoadResource("UI/$Canvas.prefab", OnTexLoad, kParam2, typeof(GameObject));
        ResourceMgr.Instance.PreLoadResource("UI/$Canvas.prefab", true);
         kParam2 = new ResLoadParams();
        kParam2.info = "x";
        ResourceMgr.Instance.LoadResource("UI/$tex/BG.png", OnTexLoad, kParam2, typeof(Texture));
        ResourceMgr.Instance.PreLoadResource("UI/$tex/BG.png", true);

        //    Test();
        /*     Button button = transform.GetComponent<Button>();
          if (button)
          {
              button .onClick.AddListener(OnClick);// = onClick;
          }
         string path = "";
          ResLoadParams kParam = new ResLoadParams();
          kParam.info = "test";
          ResourceMgr.Instance.LoadResource("UI/$Scene.prefab", OnPrefabImageNumberLoaded, kParam);
          kParam.info = "test2";
          ResourceMgr.Instance.LoadResource("res/$Cube.prefab", OnPrefabImageNumberLoaded, kParam);
        ResLoadParams kParam2 = new ResLoadParams();
          kParam2.info = "test4";
          ResourceMgr.Instance.LoadResource("Resources/Capsule.prefab", OnPrefabImageNumberLoaded, kParam2);
          ResLoadParams kParam3 = new ResLoadParams();
          kParam3.info = "test3";
          ResourceMgr.Instance.LoadResource("Resources/test/Capsule.prefab", OnPrefabImageNumberLoaded, kParam3);*/
        // StartCoroutine(test());
    }

    public virtual void OnClick()
    {
        isclick = true;
        Debug.Log("onclick");
    }
    void OnPrefabImageNumberLoaded(ResLoadParams param, UnityEngine.Object obj)
    {
        if (obj == null)
        {
            Debug.Log("OnPrefabImageNumberLoaded obj = null");
            return;
        }

      GameObject  m_PrefabImageNumber = (obj) as GameObject;
        Debug.Log("load time  :"+(Time.realtimeSinceStartup - LoadTime));

        GameObject test = GameObject.Instantiate(m_PrefabImageNumber);
        Debug.Log("load time Instantiate  :" + param._LoadTime);
        Debug.Log(obj);
        Debug.Log(param.info);
   //  Material mat=   test.GetComponent<MeshRenderer>().material;
//         Debug.Log(mat);
//           DestroyObject(test);
        //GameObject.Destroy(obj);
        
    }
    bool isclick = false;
    IEnumerator test()
    {
      
        string path = Application.dataPath.Replace("Assets", "") + "Bundles/";
        string MainBunldes = path + "Bundles";
        AssetBundle manifestBundle = AssetBundle.LoadFromFile(MainBunldes);
        AssetBundleManifest manifest = (AssetBundleManifest)manifestBundle.LoadAsset("AssetBundleManifest");
        string bundleName = "ui/$home.unity3d";//"ui/$scene.prefab.unity3d";//
        string bundleName2 = "ui/$big.unity3d";//"ui/$scene.prefab.unity3d";/
        string resname = "Cube";//"$scene.prefab";// 
        string resname2 = "sphere2";
        string[] dependents = manifest.GetAllDependencies(bundleName);
        string[] dependents2 = manifest.GetAllDependencies(bundleName2);



        Debug.Log(dependents.Length);
        AssetBundle[] dependsAssetbundle = new AssetBundle[dependents.Length];
        for (int index = 0; index < dependents.Length; index++)
        {  //加载所有的依赖文件;  
            
                dependsAssetbundle[index] = AssetBundle.LoadFromFile(path + dependents[index]);
                Debug.Log(dependsAssetbundle[index]);
            
        }
        AssetBundle item = AssetBundle.LoadFromFile(path + bundleName);
        Debug.Log("load bundle!");


        Object abr2 = item.LoadAsset(resname);
        GameObject a2 = GameObject.Instantiate(abr2) as GameObject;
        Debug.Log(abr2);

        while (true)
        {
            yield return null;
           // Object abr2 = item.LoadAsset(resname);
        
          //  Debug.Log(abr2.GetInstanceID());
           // Debug.Log(abr == abr2);
        }
    }
 
    void Test()
    {
        float time = Time.realtimeSinceStartup;
        int c=100000;
        Debug.Log("各种数据在100000次操作下的时间");
        List<BundleGlobalItem> lst = new List<BundleGlobalItem>();
        for(int i = 0; i < c; i++)
        {
            BundleGlobalItem t = new BundleGlobalItem();
            lst.Add(t);
        }
        Debug.Log("lst add +"+(Time.realtimeSinceStartup - time));
        time = Time.realtimeSinceStartup;

        Dictionary<string, BundleGlobalItem> dic = new Dictionary<string, BundleGlobalItem>();
        for (int i = 0; i < c; i++)
        {
            BundleGlobalItem t = new BundleGlobalItem();
            dic.Add(i.ToString(),t);
        }
        Debug.Log("dic add "+(Time.realtimeSinceStartup - time));
        time = Time.realtimeSinceStartup;

        Hashtable ha = new Hashtable();
        for (int i = 0; i < c; i++)
        {
            BundleGlobalItem t = new BundleGlobalItem();
            ha[i] = t;
        }
        Debug.Log("hash add " + (Time.realtimeSinceStartup - time));
        time = Time.realtimeSinceStartup;

        for(int i = 0; i < lst.Count; i++)
        {
            BundleGlobalItem t = lst[i];
        }
        Debug.Log("for  " + (Time.realtimeSinceStartup - time));
        time = Time.realtimeSinceStartup;

        foreach (KeyValuePair<string,BundleGlobalItem> kv in dic)
        {
            BundleGlobalItem t = kv.Value;
        }
        Debug.Log("dic KeyValuePair " + (Time.realtimeSinceStartup - time));
        time = Time.realtimeSinceStartup;



        foreach (string key in dic.Keys)
        {
            BundleGlobalItem t = ha[key] as BundleGlobalItem;
        }
        Debug.Log("dic  key" + (Time.realtimeSinceStartup - time));
        time = Time.realtimeSinceStartup;

        foreach (int key in ha.Keys)
        {
            BundleGlobalItem t = ha[key] as BundleGlobalItem;
        }
        Debug.Log("hash  key" + (Time.realtimeSinceStartup - time));
        time = Time.realtimeSinceStartup;

    }
}
