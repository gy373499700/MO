using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;
[ExecuteInEditMode]
public class VertexShaft : MonoBehaviour {

    static List<VertexShaft> m_list = new List<VertexShaft>();
    private Vector3 LightPosition = Vector3.zero;
    private float LightDistanceControlAlpha = -4f;
    private float Offset = 5f;
    private Texture ShaftTex = null;
    private Color m_Color = Color.white;
    private float LightDensity = 2f;
    private List<Mesh> m_Mesh = new List<Mesh>();
    private List<Renderer> m_Render = new List<Renderer>();
    private bool Statics = true;
    static CommandBuffer buff = null;
    private Vector2 uv = Vector2.one;
    void Awake()
    {
      /*  m_Render.Clear();
        m_Mesh.Clear();
        MeshFilter[] meshlst = transform.GetComponentsInChildren<MeshFilter>();
        SkinnedMeshRenderer[] skrender = transform.GetComponentsInChildren<SkinnedMeshRenderer>();
        for (int i = 0; i < skrender.Length; i++)
        {
            m_Mesh.Add(skrender[i].sharedMesh);
        }
        for (int i = 0; i < meshlst.Length; i++)
        {
            m_Mesh.Add(meshlst[i].mesh);
        }
        Renderer[] render = transform.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < render.Length; i++)
        {
            m_Render.Add(render[i]);
        }*/
  
    }
    void OnEnable()
    {
        m_list.Add(this);
    }
    void OnDisable()
    {
        m_list.Remove(this);
    }

	public static void DrawAllVertexShaft(Camera cloneCamera, RenderBuffer currentColorBuffer, RenderBuffer currentDepthBuffer,Material mat_VertexShaft,RenderTexture gbuffer, Texture x2,Vector4 InvViewport)
    {//模型拉伸效果不好 暂时取消
        return;//discard
        for(int i=0;i< m_list.Count; i++)
        { 
            VertexShaft vertex = m_list[i];
            if (vertex == null) continue;
            float dis = Vector3.Distance(vertex.transform.position, vertex.LightPosition);
            mat_VertexShaft.SetTexture("_MainTex", vertex.ShaftTex);
            mat_VertexShaft.SetFloat("_Offset", vertex.Offset);
            mat_VertexShaft.SetVector("_LightPos", vertex.LightPosition);
            mat_VertexShaft.SetVector("_Color", vertex.m_Color);
            mat_VertexShaft.SetFloat("_LightDis", vertex.LightDistanceControlAlpha);
            mat_VertexShaft.SetFloat("_LightDensity", vertex.LightDensity);
            mat_VertexShaft.SetTexture("_Sample2x2", x2);
            mat_VertexShaft.SetVector("_UV", vertex.uv);
            mat_VertexShaft.SetVector("invViewport", InvViewport);
            mat_VertexShaft.SetPass(0);
            Graphics.SetRenderTarget(currentColorBuffer, currentDepthBuffer);
            if (vertex.Statics)
            {

                for (int k = 0; k < vertex.m_Mesh.Count; k++)
                {
                    if(vertex.m_Mesh[i]!=null)
                        Graphics.DrawMeshNow(vertex.m_Mesh[i], vertex.transform.position, vertex.transform.rotation);
                }
            }
            else
            {//会多一个pass
                if (buff == null)
                    buff = new CommandBuffer();
                for (int k = 0; k < vertex.m_Render.Count; k++)
                {
                    if (vertex.m_Render[i] != null)
                        buff.DrawRenderer(vertex.m_Render[k], mat_VertexShaft);
                }
                Graphics.ExecuteCommandBuffer(buff);
                buff.Clear();//must clear ,cant release
            }
        }
    }
    public enum LightEnum
    {
        PointLight,
        VolumeLight,
    }
    public LightEnum light = LightEnum.PointLight;
    public Color mLightColor = Color.white;

    [Range(0,1)]
    public float mLightOffset = 1;
    public float mLightIntensity = 1;
    public Vector2 VolumeLightDirection = Vector2.zero; //理解为光源位置
    static Vector3 getInViewPosition(Camera MainCamera, VertexShaft vertex, ref Vector3 newViewPort)
    {
        Vector3 pos = vertex.transform.position;
        float scale = vertex.transform.lossyScale.magnitude;
        Vector3 viewport_center = MainCamera.WorldToViewportPoint(pos);
      
       if(viewport_center.x > 1)
        {
            viewport_center.x = 1;
        }
        else if (viewport_center.x < 0)
        {
            viewport_center.x = 0;
        }
        if (viewport_center.y > 1)
        {
            viewport_center.y= 1;
        }
        else if (viewport_center.y < 0)
        {
            viewport_center.y = 0;
        }

        Vector3 newworld = MainCamera.ViewportToWorldPoint(viewport_center);
        newViewPort = viewport_center;
        return newworld;
    }
    public float CullDistance = 50;//裁剪距离
    public static void DrawAllLightShit(Camera MainCamera, RenderTexture currentRenderTarget, Material mat_VertexShaft, RenderTexture gbuffer, Vector4 InvViewport, Mesh quad, Mesh sphere,Texture x2,Vector4 farcorner )
    {//点光源 体积光
        for (int i = 0; i < m_list.Count; i++)
        {
            VertexShaft vertex = m_list[i];
            if (vertex == null) continue;
            int w = currentRenderTarget.width;
            int h = currentRenderTarget.height;
            Vector3 viewport_center = MainCamera.WorldToViewportPoint(vertex.transform.position);
            if (viewport_center.z < 0)
            {
                continue;
            }
            if (vertex.light == LightEnum.PointLight)
            {
                if (viewport_center.x > 1.0f || viewport_center.x < 0.0f || viewport_center.y > 1.0f || viewport_center.x < 0.0f)
                {
                    continue;
                }
            }
             if (vertex.light == LightEnum.VolumeLight)
             {
                 if (viewport_center.x > 1.3f || viewport_center.x < -0.3f || viewport_center.y > 1.3f || viewport_center.y < -0.3f)
                 {
                     continue;
                 }
             }
            float dis = Vector3.Distance(vertex.transform.position, MainCamera.transform.position);
            float maxdis = 40f;
            float scale = vertex.transform.lossyScale.magnitude;
            if (dis> maxdis||dis> vertex.CullDistance * (vertex.mLightIntensity+ scale+ (vertex.Offset-0.5f)*5))
            {//temp
                continue;
            }
            //距离剪裁优化 and Ztest
    
            if (vertex.light == LightEnum.PointLight)
            {
                RenderTexture temp = RenderTexture.GetTemporary(w, h, 32);
                Graphics.SetRenderTarget(temp.colorBuffer, currentRenderTarget.depthBuffer);
                GL.Clear(false, true, Color.black);
                temp.filterMode = FilterMode.Bilinear;
                mat_VertexShaft.SetColor("_mColor", vertex.mLightColor);

                //mat_VertexShaft.SetVector("_MainUV", viewport_center);
                mat_VertexShaft.SetPass(1);//画圆
                Matrix4x4 matrix = Matrix4x4.identity;
                matrix.SetTRS(vertex.transform.position, vertex.transform.rotation, vertex.transform.lossyScale);
                Graphics.DrawMeshNow(sphere, matrix);
                ///
                Vector2 param = new Vector2();
                param.x = vertex.mLightOffset;
                param.y = vertex.mLightIntensity;
                mat_VertexShaft.SetVector("_LightParam", param);
                mat_VertexShaft.SetVector("invViewport", InvViewport);
                mat_VertexShaft.SetTexture("_MainTex", temp);
                mat_VertexShaft.SetVector("_MainUV", viewport_center);
                mat_VertexShaft.SetTexture("_Sample2x2", x2);
                mat_VertexShaft.SetVector("_CenterOffset", vertex.VolumeLightDirection);
                Graphics.SetRenderTarget(currentRenderTarget);
                mat_VertexShaft.SetPass(3);//拉丝到主rt
                Graphics.DrawMeshNow(quad, Matrix4x4.identity, 0);

                RenderTexture.ReleaseTemporary(temp);
            }
            else
            {
                RenderTexture temp = RenderTexture.GetTemporary(w, h);
                Graphics.SetRenderTarget(temp);
                GL.Clear(false, true, Color.black);
                temp.filterMode = FilterMode.Bilinear;
                mat_VertexShaft.SetColor("_mColor", vertex.mLightColor);

                Matrix4x4 matrix = Matrix4x4.identity;
                float viewZ = MainCamera.transform.InverseTransformPoint(vertex.transform.position).z;

                Vector3 NewCenter = viewport_center;//new cycle center
                Vector3 newpos = getInViewPosition(MainCamera, vertex,ref NewCenter);
                Vector3 pos = MainCamera.transform.TransformPoint(new Vector3(0, 0, viewZ));

                matrix.SetTRS(newpos, vertex.transform.rotation, vertex.transform.lossyScale);
                mat_VertexShaft.SetPass(2);//1.画圆
                Graphics.DrawMeshNow(sphere, matrix);
               // Graphics.DrawMeshNow(quad, Matrix4x4.identity, 0);
                ///
                Vector2 param = new Vector2();
                param.x = vertex.mLightOffset;
                param.y = vertex.mLightIntensity;
                mat_VertexShaft.SetVector("_LightParam", param);
                mat_VertexShaft.SetVector("invViewport", InvViewport);
                mat_VertexShaft.SetTexture("_MainTex", temp);
                mat_VertexShaft.SetVector("_MainUV", NewCenter);
                mat_VertexShaft.SetTexture("_Sample2x2", x2);
                Vector3 viewdir = MainCamera.transform.InverseTransformDirection(vertex.VolumeLightDirection);
                mat_VertexShaft.SetVector("_CenterOffset", viewdir);//////@!!!!change world to uv space 看看雾效流动
                RenderTexture temp2 = RenderTexture.GetTemporary(w, h, 32);
                temp2.filterMode = FilterMode.Bilinear;
                temp2.wrapMode = TextureWrapMode.Clamp;
                Graphics.SetRenderTarget(temp2);
                GL.Clear(true, true, Color.black);
                mat_VertexShaft.SetPass(3);//2拉丝圆
                Graphics.DrawMeshNow(quad, Matrix4x4.identity, 0);
                RenderTexture.ReleaseTemporary(temp);

                Graphics.SetRenderTarget(currentRenderTarget);
                mat_VertexShaft.SetTexture("_DepthTex", gbuffer);
                mat_VertexShaft.SetTexture("_MainTex", temp2);
                mat_VertexShaft.SetVector("_MainUVOffset", NewCenter - viewport_center);
                mat_VertexShaft.SetVector("farCorner", farcorner);
                float depth = viewZ / MainCamera.farClipPlane;
                mat_VertexShaft.SetFloat("_depth", depth);
                mat_VertexShaft.SetPass(4);//blend到主rt
                Graphics.DrawMeshNow(quad, Matrix4x4.identity, 0);

                RenderTexture.ReleaseTemporary(temp2);
            }

        }
    }



}