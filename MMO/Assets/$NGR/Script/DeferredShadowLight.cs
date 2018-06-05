using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum ShadowQuality
{
    NoShadow,
    Low,
    Middle,
    High
}
[ExecuteInEditMode]
public class DeferredShadowLight : MonoBehaviour {//spotlight
    public ShadowQuality Quility = ShadowQuality.NoShadow;
    public bool ShowVolume = false;
    public float VolumeNearPlane = 5.0f;
    public float VolumeFarPlane = 30.0f;
    public float VolumeInstencity = 1.0f;
    public float FogInstencity = 1.0f;
    public float Radius = 1.0f;
    public bool UseMainLightColor = false;
    public Color color = Color.white;
    public float color_scale = 1.0f;
    [Range(0.001f,0.2f)]
    public float bias = 0.02f;
    public float sample_radius = 2.0f;
    public float Light_Size = 0.0f;
    public Texture2D shadowMask;
	// Use this for initialization
    public static List<DeferredShadowLight> LightList = new List<DeferredShadowLight>();
    static Frustum shadowFrrustum = new Frustum();
    public static void DrawAll(
        Material mat_shadow_gen,
        Material matShadowVolume,
        RenderTexture tempGBuffer,
        RenderBuffer tempDepthBuffer,
        Shader gbuffer_shader, 
        Matrix4x4 MainView, 
        RenderTexture gbuffer_tex, 
        RenderTexture diffuse, 
        Vector4 farcorner,
        Vector4 invViewport,
        Texture _2X2Tex,
        Texture _Random256,
        Mesh Cube,
        RenderBuffer color_buffer,
        RenderBuffer depth_buffer,
        Frustum _MainCameraFrustum,
        Color MainLightColor,
        Vector3 MainCameraPos,
        int SpotLightLevel
        )
    {
        //if (tempGBuffer == null)
        //{
        //    return;
        //}
        if (Application.isEditor)
        {
            farcorner.w = -1;
        }
        else
        {
            farcorner.w = 1.0f;
        }

        

        for (int i = 0; i < LightList.Count; i++)
        {
            DeferredShadowLight dsl = LightList[i];
            Camera shadowCamera = dsl.GetComponent<Camera>();
            Vector3 dir = dsl.transform.TransformDirection(Vector3.forward);
            float radius = shadowCamera.farClipPlane * 0.5f;
            Vector3 center = dsl.transform.position + dir* radius;
            
            //计算灯光范围是否可见..
            if (!_MainCameraFrustum.IsVisiable(center, radius))
            {
                continue;
            }
            //计算灯光强度..强度太低不处理...
            Color LightColor = Color.black;
            if (dsl.UseMainLightColor)
            {
                LightColor = MainLightColor * dsl.color * dsl.color_scale;
            }
            else
            {
                LightColor = dsl.color * dsl.color_scale;// * shadowCamera.orthographicSize*0.1f);
            }
            float light_power = LightColor.r + LightColor.g + LightColor.b;
            if (light_power < 0.01f)
            {
                continue;
            }

            //shadowCamera.targetTexture = tempGBuffer;
            shadowCamera.clearFlags = CameraClearFlags.SolidColor;
            shadowCamera.backgroundColor = new Color(1, 1, 1, 1);
            Matrix4x4 shadowcam_proj_unity = Matrix4x4.identity;
            if (dsl.Quility != ShadowQuality.NoShadow && SpotLightLevel >=2)
            {
                shadowCamera.SetTargetBuffers(tempGBuffer.colorBuffer, tempDepthBuffer);
                shadowCamera.RenderWithShader(gbuffer_shader, "RenderType");
                shadowcam_proj_unity = GL.GetGPUProjectionMatrix(shadowCamera.projectionMatrix, true);
                if (!Application.isEditor)
                {
                    shadowcam_proj_unity.m11 *= -1;

                }
                shadowCamera.targetTexture = null;
            }
            else
            {
                shadowCamera.targetTexture = tempGBuffer;
                shadowcam_proj_unity = GL.GetGPUProjectionMatrix(shadowCamera.projectionMatrix, true);
                if (!Application.isEditor)
                {
                    shadowcam_proj_unity.m11 *= -1;

                }
                shadowCamera.targetTexture = null;
            }
            
            //shadowCamera.targetTexture = null;


            Graphics.SetRenderTarget(color_buffer, depth_buffer);

            Matrix4x4 shadow_cam_World = Matrix4x4.identity;
            RenderPipeline.ViewMatrix(shadowCamera, ref shadow_cam_World);

            Matrix4x4 mainview_shadowview = shadow_cam_World * MainView.inverse;//
            //Matrix4x4 shadowcam_proj = Matrix4x4.identity;
            //RenderPipeline.ProjMatrix(shadowCamera, ref shadowcam_proj);
            //Matrix4x4 shadowcam_proj_unity = GL.GetGPUProjectionMatrix(shadowCamera.projectionMatrix, true);
            //if (!Application.isEditor)
            //{
            //    shadowcam_proj_unity.m11 *= -1;
            //
            //}
            //Matrix4x4 shadow_cam_WorldToView = shadow_cam_ViewToWorld.inverse;
            //Vector3 wpos = main_shadow*(new Vector3(0, 0, 12));
            // *shadowCamera.worldToCameraMatrix * GL.GetGPUProjectionMatrix(shadowCamera.projectionMatrix);
            mat_shadow_gen.SetMatrix("mainView_shadowView", mainview_shadowview);// * shadow_cam_World);
            mat_shadow_gen.SetMatrix("shadowProj", shadowcam_proj_unity);
            mat_shadow_gen.SetTexture("_ShadowDepth", tempGBuffer);
            mat_shadow_gen.SetTexture("_Depth", gbuffer_tex);
            mat_shadow_gen.SetTexture("_Sample2x2", _2X2Tex);
            mat_shadow_gen.SetTexture("_Random256", _Random256);
            mat_shadow_gen.SetTexture("_ShadowMask", dsl.shadowMask);


            //Vector3 lightpos = MainView.MultiplyPoint(shadowCamera.transform.position);
            //Vector3 lightdir = MainView.MultiplyVector(shadowCamera.transform.TransformDirection(Vector3.forward));
            //mat_shadow_gen.SetVector("lightpos", lightpos);
            Vector4 light_dir = Vector3.zero;
            if (dsl.GetComponent<Camera>().orthographic)
            {
                light_dir = MainView.MultiplyVector(dsl.transform.TransformDirection(-Vector3.forward));
                light_dir.w = 0.0f;
            }
            else
            {
                light_dir = MainView.MultiplyPoint(shadowCamera.transform.position);
                light_dir.w = 1;
            }
            

            mat_shadow_gen.SetVector("lightdir", light_dir);
            mat_shadow_gen.SetVector("lightcolor", LightColor);

            Vector4 LightSize = Vector4.zero;
            LightSize.x = dsl.Light_Size;
            mat_shadow_gen.SetVector("LightSize", LightSize);

            mat_shadow_gen.SetTexture("_Diffuse", diffuse);

            mat_shadow_gen.SetVector("farCorner", farcorner);
            Vector4 v_shadow = new Vector4(1.0f / tempGBuffer.width, 1.0f / tempGBuffer.height, dsl.sample_radius, shadowCamera.farClipPlane);
            mat_shadow_gen.SetVector("invViewport_Radius", v_shadow);
            Vector4 temp = invViewport;
            if(dsl.bias>0.2f)
            {
                dsl.bias = 0.2f;
            }
            else if(dsl.bias < 0.0f)
            {
                dsl.bias = 0.001f;
            }
            temp.w = dsl.bias;
            mat_shadow_gen.SetVector("mainInvViewport", temp);
            //mat_shadow_gen.SetVector("ambientcolor", dsl.color * dsl.color_scale);

            //Graphics.Blit(null, target, mat_shadow_gen, 0);

            shadowFrrustum.Buildclipplane(shadowCamera);

            bool EyeInShadowVolumw = shadowFrrustum.IsVisiable(MainCameraPos, 0.0f);

            if(EyeInShadowVolumw)
            {
                if (SpotLightLevel == 1)
                {
                    mat_shadow_gen.SetPass(0);
                }
                else
                {
                    mat_shadow_gen.SetPass((int)dsl.Quility);
                }
                dsl.DrawFrustum(Cube, shadowcam_proj_unity);
            }
            else
            {
                mat_shadow_gen.SetPass(4);
                dsl.DrawFrustum(Cube, shadowcam_proj_unity);
                if (SpotLightLevel == 1)
                {
                    mat_shadow_gen.SetPass(5);
                }
                else
                {
                    mat_shadow_gen.SetPass(5 + (int)dsl.Quility);
                }
                dsl.DrawFrustum(Cube, shadowcam_proj_unity);
            }

            if(dsl.ShowVolume && SpotLightLevel == 3)
            {

                int width  = gbuffer_tex.width;
                int height = gbuffer_tex.height;

                RenderTexture temp_depth = RenderTexture.GetTemporary(width, height,0,RenderTextureFormat.ARGB32);

                Graphics.SetRenderTarget(temp_depth.colorBuffer, depth_buffer);
                GL.Clear(false, true, new Color(1, 1, 0, 0));
                //matShadowVolume.SetTexture("");
                Vector4 temp_invViewport = invViewport;
                temp_invViewport.w = shadowCamera.farClipPlane;
                matShadowVolume.SetVector("mainInvViewport", temp_invViewport);
                matShadowVolume.SetMatrix("mainView_shadowView", mainview_shadowview);
                matShadowVolume.SetMatrix("shadowProj", shadowcam_proj_unity);
                matShadowVolume.SetMatrix("MV_Matrix", MainView* dsl.GetFrustumMatrix(shadowcam_proj_unity));
                matShadowVolume.SetTexture("_FarDepth", temp_depth);
                matShadowVolume.SetTexture("_Depth", gbuffer_tex);
                matShadowVolume.SetTexture("_Random256", _Random256);
                matShadowVolume.SetColor("lightcolor", LightColor);
                matShadowVolume.SetTexture("_ShadowMask", dsl.shadowMask);
                matShadowVolume.SetTexture("_ShadowDepth", tempGBuffer);
                matShadowVolume.SetVector("lightdir", light_dir);
                matShadowVolume.SetVector("farCorner", farcorner);
                Vector4 volumeparam = Vector4.zero;
                volumeparam.x = dsl.VolumeNearPlane;
                volumeparam.y = dsl.VolumeInstencity;
                volumeparam.z = dsl.VolumeFarPlane;
                volumeparam.w = dsl.FogInstencity;
                matShadowVolume.SetVector("VolumeParams", volumeparam);
                //if (!EyeInShadowVolumw)
                {
                    matShadowVolume.SetPass(0);
                    dsl.DrawFrustum(Cube, shadowcam_proj_unity);
                }
                Graphics.SetRenderTarget(color_buffer, depth_buffer);
                if (dsl.Quility == ShadowQuality.NoShadow)
                {
                    if (EyeInShadowVolumw)
                    {
                        matShadowVolume.SetPass(1);
                    }
                    else
                    {
                        matShadowVolume.SetPass(2);
                    }
                }
                else
                {
                    if (EyeInShadowVolumw)
                    {
                        matShadowVolume.SetPass(3);
                    }
                    else
                    {
                        matShadowVolume.SetPass(4);
                    }
                }
                dsl.DrawFrustum(Cube, shadowcam_proj_unity);

                RenderTexture.ReleaseTemporary(temp_depth);

            }
            
        }

    }
	void Awake () {
        //LightList.Add(this);
	}
    void OnDestroy()
    {
        //LightList.Remove(this);
    }
    void OnEnable()
    {
        LightList.Add(this);
    }
    void OnDisable()
    {
        LightList.Remove(this);
    }
	// Update is called once per frame
	void Update () {
	
	}


    //public Mesh mcube;
    void OnDrawGizmos()
    {
        //DrawFrustum(mcube);
        Gizmos.DrawIcon(transform.position, "ShadowLight.png");
    }
    Matrix4x4 GetFrustumMatrix(Matrix4x4 shadowcam_proj_unity)
    {
        Matrix4x4 mattest = Matrix4x4.identity;
        Vector3 scale = new Vector3(2, 2, 1);
        Vector3 v_pos = new Vector3(0, 0, 0.5f);
        if (!Application.isEditor)
        {
            scale.z = 2;
            v_pos.z = 0;
        }
        mattest.SetTRS(v_pos, Quaternion.identity, scale);

        Vector3 vscale = new Vector3(1, 1, -1);
        if (!Application.isEditor)
        {
            vscale.y = -1;
        }

        Matrix4x4 mattest2 = Matrix4x4.identity;
        mattest2.SetTRS(Vector3.zero, Quaternion.identity, vscale);
        //
        return transform.localToWorldMatrix * mattest2 * shadowcam_proj_unity.inverse * mattest;
    }
    void DrawFrustum(Mesh Cube,Matrix4x4 shadowcam_proj_unity)
    {

        Matrix4x4 total = GetFrustumMatrix(shadowcam_proj_unity);
        //Gizmos.DrawWireSphere(transform.position, Radius);
        Graphics.DrawMeshNow(Cube, total);
    }
}
