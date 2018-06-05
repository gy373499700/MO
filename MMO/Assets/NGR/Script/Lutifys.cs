using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
public class LutifyItem
{
    public string path;
    public string Name;
}
public class Lutifys : MonoBehaviour {


    static List<LutifyItem> m_Collections = new List<LutifyItem>();
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
    static void FetchLuts()
    {
        string path = Application.dataPath + "/$NGR/lutify.txt";
        string content = File.ReadAllText(path);
        string[] lines = content.Split('\n');//youhua
        if (lines.Length == 0)
            UnityEngine.Debug.Log("lines.Length == 0");
        for (int i = 0; i < lines.Length; i++)
        {
            string s = lines[i].Replace("//","/");
            if (s.Length == 0)
            {
                UnityEngine.Debug.Log("s.Length == 0");
                continue;
            }
            string resName = GetResNameFromPath(s);
            LutifyItem item = new LutifyItem();
            item.Name = resName;
            item.path = s;
           // if (s.Contains("Retro Pack"))
           //     item.LutFiltering = FilterMode.Point;
          //  else
           //     item.LutFiltering = FilterMode.Bilinear;
            m_Collections.Add(item);
        }
    }


    static void SetIdentityLut3D()
    {
        int dim = 16;
        Color[] newC = new Color[dim * dim * dim];
        float oneOverDim = 1f / (1f * dim - 1f);

        for (int i = 0; i < dim; i++)
        {
            for (int j = 0; j < dim; j++)
            {
                for (int k = 0; k < dim; k++)
                {
                    newC[i + (j * dim) + (k * dim * dim)] = new Color((float)i * oneOverDim, (float)j * oneOverDim, (float)k * oneOverDim, 1f);
                }
            }
        }

        if (m_Lut3D)
            DestroyImmediate(m_Lut3D);

        m_Lut3D = new Texture3D(dim, dim, dim, TextureFormat.ARGB32, false);
        m_Lut3D.hideFlags = HideFlags.HideAndDontSave;
        m_Lut3D.SetPixels(newC);
        m_Lut3D.Apply();
        m_BaseTextureIntanceID = -1;
    }

    static bool ValidDimensions(Texture2D tex2D)
    {
        if (tex2D == null || tex2D.height != Mathf.FloorToInt(Mathf.Sqrt(tex2D.width)))
            return false;

        return true;
    }

    static void ConvertBaseTexture3D(Texture2D LookupTexture)
    {
        if (!ValidDimensions(LookupTexture))
        {
            Debug.LogWarning("The given 2D texture " + LookupTexture.name + " cannot be used as a LUT. Pick another texture or adjust dimension to e.g. 256x16.");
            return;
        }


        int dim = LookupTexture.height;
        m_BaseTextureIntanceID = LookupTexture.GetInstanceID();
        Color[] c = LookupTexture.GetPixels();//x=256*y=16, first x then y
        Color[] newC = new Color[c.Length];//16*16*16 所以对应起来 3Dpixel first xy then z

        for (int i = 0; i < dim; i++)
        {
            for (int j = 0; j < dim; j++)
            {
                for (int k = 0; k < dim; k++)
                {
                    int j_ = dim - j - 1;
                    newC[i + (j * dim) + (k * dim * dim)] = c[k * dim + i + j_ * dim * dim];
                    //对应关系//for3D先读z 再读yx
                    //for2D先读了最后一整行每隔16x像素的第一个点，也就是塞进3D的深度值。
                }
            }
        }

        if (m_Lut3D)
            DestroyImmediate(m_Lut3D);

        m_Lut3D = new Texture3D(dim, dim, dim, TextureFormat.ARGB32, false);
        m_Lut3D.hideFlags = HideFlags.HideAndDontSave;
        m_Lut3D.wrapMode = TextureWrapMode.Clamp;
        m_Lut3D.SetPixels(newC);
        m_Lut3D.filterMode = LookupTexture.filterMode;
        m_Lut3D.Apply();

    }
    static void Init()
    {
        if (Inited) return;
        if (SystemInfo.supports3DTextures)
            Use3d = true;
        FetchLuts();
        Inited = true;
    }
    static bool Inited = false;
    static bool Use3d = false;
    static Texture3D m_Lut3D;
    static Texture2D tex = null;
    static int lastIndex = 0;
    static int m_BaseTextureIntanceID;
    static LutifyItem GetItem(int index)
    {
        if (index >= m_Collections.Count) return null;
        LutifyItem item = m_Collections[index];
        return item;
    }
    static void GetTexture(LutifyItem item)
    {
        if (item != null)
        {
            ResLoadParams param = new ResLoadParams();
            param.userdata0 = item.Name;
            ResourceMgr.Instance.LoadResource(item.path, OnLoadRainTexture, param);// UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(item.path);
          //  return tex;
        }
       // return null;
    }
  static  void OnLoadRainTexture(ResLoadParams param, UnityEngine.Object obj)// (sdFileInfo info, object obj, FileLoadParas param)
    {
        if (obj != null)
        {
            tex = obj as Texture2D;
        }
    }
    public static  void DrawLutify(RenderTexture src, RenderTexture dst,Material matLutify,int Index)
    {
        Init();
        LutifyItem item = GetItem(Index);
        if(lastIndex!= Index)
        {
            lastIndex = Index;
            tex = null;
            SceneRenderSetting._Setting.LutifyTex = null;
            Resources.UnloadAsset(tex);
        }

        if (tex == null)
        {
            GetTexture(item);
            return;
        }
        if (tex == null)
        {
            Graphics.Blit(src, dst);
            return;
        }

        if (Use3d == true)
        {
            if (tex.GetInstanceID() != m_BaseTextureIntanceID)
                ConvertBaseTexture3D(tex);
            if (m_Lut3D == null)
                SetIdentityLut3D();

           // m_Lut3D.filterMode = tex.filterMode;//point
                                                     // Uniforms
            float lutSize = (float)m_Lut3D.width;
            matLutify.SetTexture("_LookupTex3D", m_Lut3D);
            matLutify.SetVector("_Params", new Vector3((lutSize - 1f) / lutSize, 1f / (2f * lutSize), SceneRenderSetting._Setting.LutifyAlpha));
            Graphics.Blit(src, dst, matLutify, 0);
        }
        else
        {
           // tex.filterMode= item.LutFiltering;
            float tileSize = Mathf.Sqrt((float)tex.width);
            matLutify.SetTexture("_LookupTex2D", tex);
            matLutify.SetVector("_Params", new Vector4(1f / (float)tex.width, 1f / (float)tex.height, tileSize - 1f, SceneRenderSetting._Setting.LutifyAlpha));

            Graphics.Blit(src, dst, matLutify, 1);
        }
        SceneRenderSetting._Setting.LutifyTex = tex;
    }
}
