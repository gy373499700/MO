using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class FogPlane : MonoBehaviour {
    //地面雾 中心点最浓 往外扩散慢慢变淡
    public static List<FogPlane> FogList = new List<FogPlane>();


    void OnEnable()
    {
        FogList.Add(this);
    }
    void OnDisable()
    {
        FogList.Remove(this);
    }
    public Texture Maintexture = null;
    public Texture Noicetexture = null;
    public Color color = new Color(1, 1, 1, 1);
    public Color deepcolor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    public float FogStart = 0;
    public float FogEnd = 1;
    public Mesh renderMesh;
    public Vector3 Direction = Vector3.up;
    // Update is called once per frame
    static Matrix4x4 matrix = new Matrix4x4();
    public static void DrawPlaneFog(Material mat, Mesh Quad,RenderTexture _Depth, Vector4 _FarCorner, RenderTexture target, Matrix4x4 ViewToWorld, Camera maincamera)
    {
        for (int i = 0; i < FogList.Count; i++)
        {
            FogPlane w = FogList[i];
            if (w == null)
                continue;
            if (!w.enabled)
                continue;
            // if (!w.visiable)
            // {
            //      continue;
            //  }
            matrix.SetTRS(w.transform.position, w.transform.rotation, w.transform.lossyScale);
            mat.SetTexture("_MainTex", w.Maintexture);
            mat.SetTexture("_NoiceTex", w.Noicetexture);
            mat.SetColor("_Color", w.color);
            mat.SetColor("_DeepColor", w.deepcolor);
            mat.SetVector("_Direction", w.Direction);
            mat.SetVector("_OrignalPos", w.transform.position);//雾的起始点
            mat.SetVector("farCorner", _FarCorner);
            Vector3 vec = new Vector3(w.FogStart, w.FogEnd, 0);
            mat.SetVector("_FogParam", vec);
            //mat.SetMatrix("_Matrix", matrix.inverse);
           // mat.SetMatrix("_ViewToWorld", ViewToWorld);
            //mat.SetMatrix("_WorldToView", ViewToWorld.inverse);
            mat.SetPass(0);
       
   
             Graphics.DrawMeshNow(w.renderMesh, matrix);
        
         
        }
     }
}
