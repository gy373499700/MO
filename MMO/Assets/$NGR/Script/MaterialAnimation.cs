using UnityEngine;
using System.Collections;
//[ExecuteInEditMode]
public class MaterialAnimation : MonoBehaviour {
    public enum KeyType
    {
        Int,
        Float,
        Vector,
    }

    public float DelayTime = 0.0f;
    public float LoopTime = 1.0f;
    public WrapMode wrapmode = WrapMode.Loop;
    public KeyType ktype = KeyType.Float;
    public float[] keys;
    public Vector4[] values;
    public string PropertyName;
    Material[] mats = null;
    float currentTime = 0.0f;
    // Use this for initialization
    void Start () {
        
	}

	void Update () {
        currentTime += Time.deltaTime;
        float time = currentTime;
        if (wrapmode == WrapMode.Clamp)
        {
            if(time > LoopTime)
            {
                time = LoopTime;
            }
        }
        else if (wrapmode == WrapMode.ClampForever)
        {
            if (time > LoopTime)
            {
                time = LoopTime;
            }
        }
        else if (wrapmode == WrapMode.Default)
        {
            if (time > LoopTime)
            {
                time = 0.0f;
            }
        }
        else if (wrapmode == WrapMode.Loop)
        {
            if (time > LoopTime)
            {
                currentTime -= LoopTime;
                time = currentTime;
            }
        }
        else if (wrapmode == WrapMode.Once)
        {
            if (time > LoopTime)
            {
                time = 0.0f;
            }
        }
        else if (wrapmode == WrapMode.PingPong)
        {
            if (currentTime > LoopTime)
            {
                if(currentTime > LoopTime*2)
                {
                    currentTime -= LoopTime * 2;
                    time = currentTime;
                }
                else
                {
                    time = LoopTime * 2- currentTime;
                }
            }
        }
        if (mats != null && keys!=null && values!=null)
        {
            Vector4 val = Sample(time);
            for (int i = 0; i < mats.Length; i++)
            {
                Apply(mats[i], val);
            }
        }
    }
    Vector4 Sample(float time)
    {
        Vector4 ret = Vector4.zero;

        float t = time / LoopTime;
        for(int i=0;i< keys.Length;i++)
        {
            if(t > keys[i])
            {
                continue;
            }
            if(i==0)
            {
                return values[i];
            }
            float f = (t - keys[i - 1])/(keys[i]-keys[i - 1]);
            return Vector4.Lerp(values[i - 1], values[i], f);
        }

        return values[values.Length-1];
    }
    void Apply(Material m,Vector4 v)
    {
        if(ktype == KeyType.Int)
        {
            m.SetInt(PropertyName, (int)v.x);
        }
        else if (ktype == KeyType.Float)
        {
            m.SetFloat(PropertyName, v.x);
        }
        else if (ktype == KeyType.Vector)
        {
            m.SetVector(PropertyName, v);
        }
        
    }

    void OnEnable()
    {
        currentTime = -DelayTime;
        Renderer r = GetComponent<Renderer>();
        if (r != null)
        {
            if (Application.isPlaying)
            {
                mats = r.materials;
            }
            else
            {
                mats = r.sharedMaterials;
            }
        }
    }
    void OnDisable()
    {
        currentTime = -DelayTime;
    }
}
