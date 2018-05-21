using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public delegate void NotifyFree(object obj);
public class LinkedList
{
    public static uint ElementCount  =   1024;
    public static uint MaskCount     =   32;
    public struct Element
    {
        public  object  obj;
        public  uint    next;
    }
    public class Block
    {
        Element[] data = new Element[ElementCount];
        uint[] mask = new uint[MaskCount];
        uint alloc = 0;
        public uint Count
        {
            get
            {
                return alloc;
            }
        }
        public Block()
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i].obj = null;
                data[i].next = 0xffffffff;
            }
            for (int i = 0; i < MaskCount; i++)
            {
                mask[i] = 0;
            }
        }
        public  uint Alloc()
        {
            for (uint i = 0; i < MaskCount; i++)
            {
                if(mask[i]!=0xffffffff)
                {
                    for(int j=0;j<32;j++)
                    {
                        if((mask[i] & (1<<j))==0)
                        {
                            mask[i]|=(uint)(1<<j);
                            uint idx    =   i*32+(uint)j;
                            alloc++;
                            return idx;
                        }
                    }
                }
            }
            return 0xffffffff;
        }
        public void Free(uint idx)
        {
            uint i = idx / MaskCount;
            uint j = idx % MaskCount;
            mask[i]&=~(uint)(1<<(int)j);
            data[idx].obj=null;
            data[idx].next=0xffffffff;
            alloc--;
        }
        public Element GetElement(uint idx)
        {
            return data[idx];
        }
        public void SetElementData(uint idx, object obj, uint next)
        {
            data[idx].obj = obj;
            data[idx].next = next;
        }
        public void SetElementData(uint idx, uint next)
        {
            data[idx].next = next;
        }
    }
    List<Block> BlockList = new List<Block>();

    public uint Alloc()
    {
        for (uint i = 0; i < BlockList.Count; i++)
        {
            if (BlockList[(int)i].Count < ElementCount)
            {
                uint idx = BlockList[(int)i].Alloc();
                if (idx != 0xffffffff)
                {
                    return i * ElementCount + idx;
                }
            }
        }
        Block b = new Block();
        uint x = (uint)BlockList.Count * (uint)ElementCount;
        uint ret = x+b.Alloc();
        BlockList.Add(b);
        return ret;
    }
    public void Free(uint idx)
    {
        uint BlockIndex = idx / ElementCount;
        uint Index = idx % ElementCount;
        BlockList[(int)BlockIndex].Free(Index);
    }
    Element GetElement(uint idx)
    {
        uint BlockIndex = idx / ElementCount;
        uint Index = idx % ElementCount;
        return BlockList[(int)BlockIndex].GetElement(Index);
    }
    void SetElementData(uint idx, object obj, uint next)
    {
        uint BlockIndex = idx / ElementCount;
        uint Index = idx % ElementCount;
        BlockList[(int)BlockIndex].SetElementData(Index, obj, next);
    }
    void SetElementData(uint idx, uint next)
    {
        uint BlockIndex = idx / ElementCount;
        uint Index = idx % ElementCount;
        BlockList[(int)BlockIndex].SetElementData(Index, next);
    }
    uint GetEndIndex(uint idx)
    {
        uint current    =   idx;
        Element e = GetElement(current);
        while (true)
        {
            if (e.next == 0xffffffff)
            {
                return current;
            }
            else
            {
                current = e.next;
                e = GetElement(current);
            }
        }
        return 0xffffffff;
    }
    public uint Add(uint idx, object obj)
    {
        uint newIndex   =   Alloc();
        SetElementData(newIndex, obj, 0xffffffff);

        if (idx != 0xffffffff)
        {
            uint end = GetEndIndex(idx);
            SetElementData(end, newIndex);
        }
        return newIndex;
    }
    public uint New(object obj)
    {
        uint newIndex = Alloc();
        SetElementData(newIndex, obj, 0xffffffff);
        return newIndex;
    }
    public Element PopFront(uint idx)
    {
        Element e = GetElement(idx);
        Free(idx);
        return e;
    }
    public void FreeAll(uint idx, NotifyFree notify)
    {
        uint current = idx;
        Element e = GetElement(current);
        while (true)
        {
            notify(e.obj);
            Free(idx);
            if (e.next == 0xffffffff)
            {
                return;
            }
            else
            {
                e = GetElement(current);
            }
        }
    }
    public void Clear()
    {
        BlockList.Clear();
    }
}

public class EffectPool : Singleton<EffectPool>
{
    Hashtable effectTable = new Hashtable();
    LinkedList linklist = new LinkedList();
    List<string> DontClearEffect = new List<string>();
    static Component[] coms = null;
    public static void GetComponents<T>(Transform node,List<object> lst)
    {
        coms = node.gameObject.GetComponents(typeof(T));
        for (int i = 0; i < coms.Length; i++)
        {
            lst.Add(coms[i]);
        }
        for (int i = 0; i < node.childCount; i++)
        {
            Transform child = node.GetChild(i);
            GetComponents<T>(child, lst);
        }
        coms = null;
    }
   // static NcDuplicator[] duplicator = null;
    static void ResetComponet(GameObject obj)
    {
       /* duplicator = obj.GetComponentsInChildren<NcDuplicator>();
        for (int i = 0; i < duplicator.Length; ++i)
        {
            GameObject owner = duplicator[i].gameObject;
            GameObject copy = duplicator[i].GetCopyObject();
            if (copy != null)
            {
                Destroy(owner);
                duplicator[i].ClearCloneObject();
                copy.transform.parent = obj.transform;
            }
        }
        duplicator = null;
  */
    }
    List<object> lst0 = new List<object>();
    public void UnloadResource(string effectName ,GameObject obj)
    {
        ResetComponet(obj);
        obj.SetActive(false);
        obj.transform.parent = transform;

        lst0.Clear();
        GetComponents<ParticleSystem>(obj.transform, lst0);
        ParticleSystem anim = null;
        for (int i = 0; i < lst0.Count; i++)
        {
            anim = (ParticleSystem)lst0[i];
            anim.Stop(true);
            anim.Clear(true);
        }
        anim = null;
        object temp =   effectTable[effectName];
        if (temp!=null)
        {
            uint idx = (uint)temp;
            if (idx == 0xffffffff)
            {
                effectTable[effectName] =   linklist.New(obj);
            }
            else
            {
                linklist.Add(idx, obj);
            }
        }
        else
        {
            effectTable[effectName] = linklist.New(obj);
        }
    }
    public void SetDontClear(string effectName)
    {
        if (!DontClearEffect.Contains(effectName))
        DontClearEffect.Add(effectName);
    }
    public void LoadResource(string effectName, ResLoadDelegate cb, ResLoadParams param)
    {
        object temp = effectTable[effectName];
        param._name = effectName;
        if (temp == null)
        {
            ResourceMgr.Instance.LoadResourceImmediately(effectName, cb, param);
        }
        else
        {
            uint idx = (uint)temp;
            if (idx == 0xffffffff)
            {
                ResourceMgr.Instance.LoadResourceImmediately(effectName, cb, param);
            }
            else
            {
                LinkedList.Element e = linklist.PopFront(idx);
                effectTable[effectName] = e.next;

                ResLoadParams param_temp = new ResLoadParams();
                param_temp.userdata0 = e.obj;
                param_temp.userdata1 = param;
                param_temp.userdata2 = cb;
                ResourceMgr.Instance.LoadResourceImmediately(effectName, OnLoadResource, param_temp);
            }
            
        }
    }
    static void InitTransform(Transform orgin, Transform cache)
    {
        cache.localPosition = orgin.localPosition;
        cache.localScale = orgin.localScale;
        cache.localRotation = orgin.localRotation;
        for (int i = 0; i < orgin.childCount; i++)
        {
            InitTransform(orgin.GetChild(i), cache.GetChild(i));
        }
    }
    static List<object> lst = new List<object>();
    static void OnLoadResource(ResLoadParams param,UnityEngine.Object obj)
    {

        UnityEngine.Object cacheObj = (UnityEngine.Object)param.userdata0;
        ResLoadParams param_cb = (ResLoadParams)param.userdata1;
        ResLoadDelegate cb = (ResLoadDelegate)param.userdata2;
        if (obj == null)
        {
            cb(param_cb, null);
            return;
        }
        if (cacheObj.GetType() == typeof(GameObject))
        {
            GameObject orginGameObj =   (GameObject)obj;
            GameObject cacheGameObj =   (GameObject)cacheObj;
            InitTransform(orginGameObj.transform, cacheGameObj.transform);
            cacheGameObj.transform.parent = null;
            cacheGameObj.SetActive(true);
            lst.Clear();
            GetComponents<Animation>(cacheGameObj.transform, lst);
            Animation anim = null;
            for (int i = 0; i < lst.Count; i++)
            {
                 anim = (Animation)lst[i];
                if (anim.playAutomatically)
                {
                    if (anim.clip != null)
                    {
                        anim.Stop(anim.clip.name);
                        anim.Play(anim.clip.name);
                    }
                }
            }
            anim = null;
            lst.Clear();
        /*    GetComponents<NcCurveAnimation>(cacheGameObj.transform, lst);
            NcCurveAnimation animnc = null;
            for (int i = 0; i < lst.Count; i++)
            {
                animnc = (NcCurveAnimation)lst[i];
                animnc.ResetAnimation();
            }
            animnc = null;
            lst.Clear();
            GetComponents<NcUvAnimation>(cacheGameObj.transform, lst);
            NcUvAnimation animuv = null;
            for (int i = 0; i < lst.Count; i++)
            {
                animuv = (NcUvAnimation)lst[i];
                animuv.ResetAnimation();
            }
            animuv = null;
            lst.Clear();
            GetComponents<NcSpriteAnimation>(cacheGameObj.transform, lst);
            NcSpriteAnimation anims = null;
            for (int i = 0; i < lst.Count; i++)
            {
                anims = (NcSpriteAnimation)lst[i];
                anims.ResetAnimation();
            }
            anims = null;
            lst.Clear();
            GetComponents<ParticleSystem>(cacheGameObj.transform, lst);
            ParticleSystem animp = null;
            for (int i = 0; i < lst.Count; i++)
            {
                animp = (ParticleSystem)lst[i];
                //void Clear(bool withChildren);
                animp.Clear(true);
                //anim.Stop(true);
                //anim.Play();
            }
            lst.Clear();
            GetComponents<NcDuplicator>(cacheGameObj.transform, lst);
            NcDuplicator comn = null;
            for (int i = 0; i < lst.Count; i++)
            {
                comn = (NcDuplicator)lst[i];
                comn.gameObject.SetActive(true);
                comn.Call_Awake();
            }
            comn = null;
            lst.Clear();*/
            //GetComponents<TrailRenderer>(cacheGameObj.transform, lst);
            //for (int i = 0; i < lst.Count; i++)
            //{
            //    TrailRenderer com = (TrailRenderer)lst[i];
            //    com.enabled = false;
            //    com.enabled = true; 
            //}
        }
        param_cb._reqIndex = 0xffffffff;
        cb(param_cb, cacheObj);
    }
    static void OnObjectFree(object obj)
    {
        GameObject gameObj = (GameObject)obj;
        GameObject.Destroy(gameObj);

    }
    public void Clear()
    {
        Hashtable temp = new Hashtable();
        foreach (DictionaryEntry de in effectTable)
        {
            string effectName = de.Key as string;
            uint idx = (uint)de.Value;
            bool bContinue = false;
            for(int i=0;i<DontClearEffect.Count;i++)
            {
                if (DontClearEffect[i].Equals(effectName))
                {
                    temp[effectName] = idx;
                    bContinue = true;
                    break;
                }
            }
            if (bContinue)
            {
                continue;
            }
            
            while(idx!=0xffffffff)
            {
                LinkedList.Element e = linklist.PopFront(idx);
                GameObject.Destroy((GameObject)e.obj);
                idx = e.next;
            }
        }
        effectTable = temp;

    }


}
