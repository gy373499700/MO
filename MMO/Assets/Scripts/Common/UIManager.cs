using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ResourceLoad
{
    public BaseWnd m_wnd = null;
    public float mTime = 0.0f;
    public ushort mCount = 0;
    public string effectName = "";
    public bool CanDestroy()
    {
        if (m_wnd && m_wnd.gameObject && m_wnd.gameObject.activeSelf==false)
            return true;
        else
            return false;
    }

    public void ShowWnd(bool bShow, ResLoadParams kParam)
    {
        if(m_wnd&& m_wnd.gameObject&& m_wnd.gameObject.activeSelf != bShow)
        {
            m_wnd.gameObject.SetActive(bShow);
            if (bShow)
                m_wnd.OnShow(kParam);
            else
                m_wnd.OnHide();
        }
    }
}
public class UIManager : Singleton<UIManager>
{//通用单一UI 切换场景卸载 不卸载的可以自己管理 不走这个

    Transform _Board = null;
    public Transform Board
    {
        get
        {
            if (_Board == null)
            {
                GameObject t = new GameObject();
                _Board = t.transform;
                _Board.name = "Board";
                _Board.localPosition = Vector3.zero;
                _Board.localScale = Vector3.one;
                _Board.localRotation = Quaternion.identity;
            }
            return _Board;
        }
    }
    public static void SetNormalizedWindow(GameObject obj)
    {
        if (obj)
        {
            obj.transform.parent = Instance.Board;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            obj.transform.localRotation = Quaternion.identity;
        }
    }

    public void test()
    {
        ResLoadParams kParam2 = new ResLoadParams();
        ShowWindow("UI/$Big/Sphere.prefab",true, kParam2);
    }

    static Dictionary<string, ResourceLoad> AllLoad = new Dictionary<string, ResourceLoad>();
    public void ShowWindow(string PathName,bool bShow,ResLoadParams kParam=null)
    {
        if (bShow)
        {
            if (AllLoad.ContainsKey(PathName)&& AllLoad[PathName].m_wnd!=null)
            {
                AllLoad[PathName].ShowWnd(bShow, kParam);
                AllLoad[PathName].mTime = Time.realtimeSinceStartup;
            }
            else
            {
                ResLoadParams kParam2 = new ResLoadParams();
                kParam2.info = PathName;
                kParam2.userdata0 = kParam;
                ResourceMgr.Instance.LoadResource(PathName, OnLoad, kParam2, typeof(GameObject));
            }
            
        }
        else
        {
            if (AllLoad.ContainsKey(PathName))
            {
                AllLoad[PathName].mTime = Time.realtimeSinceStartup;
                AllLoad[PathName].ShowWnd(bShow, kParam);
            }
        }
    }
    void OnLoad(ResLoadParams param, UnityEngine.Object obj)
    {
        if (obj == null)
        {
            Debug.Log("OnLoad obj = null"+param.info);
            return;
        }
        GameObject a = GameObject.Instantiate(obj) as GameObject;
        SetNormalizedWindow(a);
        BaseWnd bw = a.GetComponent<BaseWnd>();
        ResLoadParams cb = param.userdata0 as ResLoadParams;
        if (!AllLoad.ContainsKey(param.info))
        {
            ResourceLoad loader = new ResourceLoad();
            loader.mTime = Time.realtimeSinceStartup;
            AllLoad[param.info] = loader;
        }
        if (bw)
        {
            bw.OnInit(cb);
            AllLoad[param.info].m_wnd = bw;
            StartCoroutine(waitOpen(AllLoad[param.info], cb));
        }
        else
        {
            Debug.LogError("把继承于BaseWnd的代码挂在改UI上！" + param.info);
        }
    }

    IEnumerator waitOpen(ResourceLoad loader, ResLoadParams kParam)
    {
        yield return null;
        loader.ShowWnd(true, kParam);
    }
    public void Clear()
    {
        AllLoad.Clear();
    }
    private void Update()
    {
        CheckUnUsedEffect();

    }
    float mfCheckTime = 0.0f;
    public void CheckUnUsedEffect()
    {
        mfCheckTime += Time.unscaledDeltaTime;
        if (mfCheckTime < 10.0f)
        {
            return;
        }
        mfCheckTime = 0.0f;
        foreach (KeyValuePair<string, ResourceLoad> kv in AllLoad)
        {
            if(kv.Value.CanDestroy() && Time.unscaledTime - kv.Value.mTime > 60.0f)
            {
                Destroy(kv.Value.m_wnd.gameObject);
                ResourceMgr.Instance.UnloadUnsuedResource(kv.Key);
                kv.Value.m_wnd = null;
            }
        }
    }
}