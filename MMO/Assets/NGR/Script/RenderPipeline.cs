using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[ExecuteInEditMode]
public class RenderPipeline : MonoBehaviour
{
    public static RenderPipeline _instance;
    [System.NonSerialized]
    public int ActorWhitingCount = 0;
    private Material mat_SSAO;
	private Material mat_SSAO2;
    private Material mat_combine;
    private Material mat_shadow_gen;
    public Shader ssao_shader = null;
	public Shader ssao_shader2 = null;
    public Shader combine_shader = null;
    public Shader gbuffer;
    public Shader diffusebuffershader = null;

    public Shader shadow_gen;
    public Texture2D ssao_2x2;
    public Texture2D ssao_2x2_inv;
    //public Texture2D IBL_Texture;
    //public Texture2D LUT_Texture;
    public Texture2D OIT_2X2;
    public Texture2D OIT_2X2_Inv;
    RenderTexture gbuffer_tex;
    RenderTexture colorbuffer_tex;
    public RenderTexture waterreflect_tex;
    RenderTexture shadow_depth0;
    RenderTexture shadow_depth1;

    //RenderTexture shadow_depth_far;
    public Camera cloneCamera;
    public Camera shadowCameraNear;
    public Camera shadowCameraFar;
    public Camera shadowCameraSuperFar;
    private bool render_far_camera = false;
    public Camera EffectCamera;
    public Camera DeferredLightCamera;

    public Mesh deferred_light_mesh;//= new Mesh()
    public Shader deferred_light_shader;
    private Material mat_deferred_light;
    public Shader SSR_Shader;
    private Material mat_SSR;
    public Mesh Quad;
    public Mesh Cube;
    public Mesh CyclePlane;

    public Texture2D ssr_2x2;
    public Texture2D ssr_2x2_step_inv;
    public Texture2D _HairTex;
    public Texture2D _SunTex;
    public Shader genNormal_shader;
    private Material mat_Normal_Gen;
    public Shader copy_shader;
    private Material mat_copy;
    public Shader gi_shader;
    private Material mat_GI;
    public Shader gi_blur_shader;
    private Material mat_GI_Blur;
    public Shader fog_shader;
    private Material mat_Fog;
    public Shader screenWater_shader;
    private Material mat_ScreenWater;
    public Shader blendframe_shader;
    private Material mat_BlendFrame;
    public Shader dof_shader;
    private Material mat_DOF;
    public Shader addtotarget_shader;
    private Material mat_AddToTarget;
    public Shader water_shader;
    private Material mat_Water;
    public Shader sssss_shader;
    private Material mat_SSSSS;
    public Shader downsample_glow_shader;
    private Material mat_downsample_glow;
    public Shader blur_glow_shader;
    private Material mat_blur_glow;
    public Shader BlendToTarget_glow_shader;
    private Material mat_BlendToTarget_glow;
    public Shader SelfIllum_shader;
    private Material mat_SelfIllum;
    public Shader ToneMapping_shader;
    private Material mat_ToneMapping;
    public Shader Decal_shader;
    private Material mat_Decal;
    public Shader ShadowLight_shader;
    private Material mat_ShadowLight;
    public Shader ActorHighLight_shader;
    private Material mat_ActorHighLight;
    public Shader Disturbance_Shader;
    private Material mat_Disturbance;
    public Shader LightShaft_Shader;
    private Material mat_LightShaft;
    public Shader ShadowMask_Shader;
    private Material mat_ShadowMask;
    public Shader SkyShader;
    private Material mat_Sky;
    public Shader UIMaskShader;
    private Material mat_UIMask;
    public Shader DebugRTShader;
    private Material mat_DebugRT;
    public Shader ShadowLightVolume;
    private Material matShadowLightVolume;
    public Shader SSSObjectShader;
    private Material matSSSObject;
    public Shader FXAAShader;
    private Material matFXAA;
    public Shader RimLightShader;
    private Material matRimLight;
    public Shader ActorWhitingShader;
    private Material matActorWhiting;
    public Shader SSAOBlurShader;
    private Material matSSAOBlur;
    public Shader mrtShader;
    private Material matMRT;
    public Shader DepthCombineShader;
    private Material matDepthCombine;
    public Shader ShadowDepthShader;
    public Shader ShadowDepthBAShader;
    private Material matShadowDepth;
    public Shader CubeShadowDepthShader;
    public Shader ForwardShader;
	public Shader PBRShader;
    public Shader WaterReflectShader;

    public Shader FullScreenRainShader;
    private Material FullScreenRainMaterial;
    public Shader RadialBlurShader;
    private Material RadialBlurMaterial;
    public Shader MipFogShader;
    private Material MipFogMaterial;
    public Shader CharacterTransShader;
    private Material CharacterMaterial;
    public Shader TwistShader;
    private Material TwistMaterial;
    public Shader SelfIllumGlowShader;
    private Material matSelfIllumGlow;
    public Shader GaussBlurShader;
    private Material matGaussBlur;
    public Shader GaussBloomShader;
    private Material matGaussBloom;
    public Shader VertexShaftshader;
    private Material matVertexShaft;//
    public Shader CheckerBoardShader;
    private Material matCheckerBoard;
    public Shader LutifyShader;
    private Material matLutify;
    public Shader PlaneFogShader;
    private Material matPlaneFog;
    public Shader ScreenRainShader;
    private Material matScreenRain;
    public Shader InputDisturbShader;
    private Material matInputDisturb;

    public Texture random256;

    //public Texture random256_2;
    public Texture rotate4x4;
    public Cubemap defaultDiffuseCube;
    public Cubemap defaultSpecularCube;
    private Texture[] rain_texs;
    private bool last_rain_valid = false;

    RenderTexture final_tex;

    Frustum _MainCameraFrustum = new Frustum();

    Vector3 LastMainCameraPos = Vector3.zero;
    Quaternion LastMainCameraRot = Quaternion.identity;

    bool NearStaticDepthRefresh = true;
    public GlobalQualitySetting quality = new GlobalQualitySetting();
    public bool showquality = false;
    //public bool es20 = false;
    UnityARCameraManager ar_manager = null;
    bool ar_enable = false;
    private RenderTexture CheckerBoard_Texture;
	public bool CheckerBoard_Enable = false;
    bool IsMoving = true;
    Texture Get2x2RotateTexture()
    {
        if (SceneRenderSetting._Setting.EnableAA)
        {
            if ((Time.frameCount & 1) == 1)
            {
                return ssao_2x2;
            }
            else
            {
                return ssao_2x2_inv;
            }
        }
        else
        {
            return ssao_2x2;
        }
    }

    Texture GetOIT2X2Texture()
    {
        if (SceneRenderSetting._Setting != null && SceneRenderSetting._Setting.EnableAA)
        {
            if ((Time.frameCount & 1) == 1)
            {
                return OIT_2X2;
            }
            else
            {
                return OIT_2X2_Inv;
            }
        }
        else
        {
            return OIT_2X2;
        }
    }
    Texture Get2x2StepTexture()
    {
        if (SceneRenderSetting._Setting.EnableAA)
        {
            if ((Time.frameCount & 1) == 1)
            {
                return ssr_2x2;
            }
            else
            {
                return ssr_2x2_step_inv;
            }
        }
        else
        {
            return ssr_2x2;
        }
    }

    Texture GetRandomTex()
    {
        return random256;
    }
    void Awake()
    {
        _instance = this;
        quality.Init();
    }
    void Start()
    {
        Debug.Log(
            "deviceModel=" + UnityEngine.SystemInfo.deviceModel + "\n" +
            "deviceName=" + UnityEngine.SystemInfo.deviceName + "\n" +
            "deviceType=" + UnityEngine.SystemInfo.deviceType + "\n" +
            "deviceUniqueIdentifier" + UnityEngine.SystemInfo.deviceUniqueIdentifier + "\n" +
            "DeviceID=" + UnityEngine.SystemInfo.graphicsDeviceID + "\n" +
            "DeviceName=" + UnityEngine.SystemInfo.graphicsDeviceName + "\n" +
            "DeviceType=" + UnityEngine.SystemInfo.graphicsDeviceType + "\n" +
            "DeviceVendor=" + UnityEngine.SystemInfo.graphicsDeviceVendor + "\n" +
            "VendorID=" + UnityEngine.SystemInfo.graphicsDeviceVendorID + "\n" +
            "DeviceVersion=" + UnityEngine.SystemInfo.graphicsDeviceVersion + "\n" +
            "MemorySize=" + UnityEngine.SystemInfo.graphicsMemorySize + "\n" +
            "MultiThreaded=" + UnityEngine.SystemInfo.graphicsMultiThreaded + "\n" +
            "ShaderLevel=" + UnityEngine.SystemInfo.graphicsShaderLevel + "\n" +
            "maxTextureSize=" + UnityEngine.SystemInfo.maxTextureSize + "\n" +
            "npotSupport=" + SystemInfo.npotSupport + "\n" +
            "operatingSystem=" + SystemInfo.operatingSystem + "\n" +
            "processorCount=" + SystemInfo.processorCount + "\n" +
            "processorFrequency=" + SystemInfo.processorFrequency + "\n" +
            "processorType=" + SystemInfo.processorType + "\n" +
            "supportsStencil=" + SystemInfo.supportsStencil + "\n"
            );
        CreateMaterials();


        //shadowCameraFar.targetTexture = shadow_depth1;


        SceneRenderSetting._Setting.depth_srgb = shadow_depth0.sRGB;

    }
    private static void DestroyMaterial(Material ref_mat)
    {
        if (ref_mat)
        {
            DestroyImmediate(ref_mat);
            ref_mat = null;
        }
    }
    private void CreateMaterials()
    {
        if (!mat_SSAO && ssao_shader.isSupported)
        {
            mat_SSAO = new Material(ssao_shader);
		}  
		if (!mat_SSAO2 && ssao_shader.isSupported)
		{
			mat_SSAO2 = new Material(ssao_shader2);
		}
        if (!mat_combine && combine_shader.isSupported)
        {
            mat_combine = new Material(combine_shader);
        }
        if (!mat_shadow_gen && shadow_gen.isSupported)
        {

            mat_shadow_gen = new Material(shadow_gen);
        }
        if (!mat_deferred_light && deferred_light_shader.isSupported)
        {
            mat_deferred_light = new Material(deferred_light_shader);
        }
        if (!mat_SSR && SSR_Shader.isSupported)
        {
            mat_SSR = new Material(SSR_Shader);
        }
        if (!mat_Normal_Gen && genNormal_shader.isSupported)
        {
            mat_Normal_Gen = new Material(genNormal_shader);
        }
        if (!mat_copy && copy_shader.isSupported)
        {
            mat_copy = new Material(copy_shader);
        }
        if (!mat_GI && gi_shader.isSupported)
        {
            mat_GI = new Material(gi_shader);
        }
        if (!mat_GI_Blur && gi_blur_shader.isSupported)
        {
            mat_GI_Blur = new Material(gi_blur_shader);
        }
        if (!mat_Fog && fog_shader.isSupported)
        {
            mat_Fog = new Material(fog_shader);
        }

        if (!mat_ScreenWater && screenWater_shader.isSupported)
        {
            mat_ScreenWater = new Material(screenWater_shader);
        }
        if (!mat_BlendFrame && blendframe_shader.isSupported)
        {
            mat_BlendFrame = new Material(blendframe_shader);
        }
        if (!mat_DOF && dof_shader.isSupported)
        {
            mat_DOF = new Material(dof_shader);
        }
        if (!mat_AddToTarget && addtotarget_shader.isSupported)
        {
            mat_AddToTarget = new Material(addtotarget_shader);
        }
        if (!mat_Water && water_shader.isSupported)
        {
            mat_Water = new Material(water_shader);
        }
        if (!mat_SSSSS && sssss_shader.isSupported)
        {
            mat_SSSSS = new Material(sssss_shader);
        }
        if (!mat_downsample_glow && downsample_glow_shader.isSupported)
        {
            mat_downsample_glow = new Material(downsample_glow_shader);
        }
        if (!mat_blur_glow && blur_glow_shader.isSupported)
        {
            mat_blur_glow = new Material(blur_glow_shader);
        }
        if (!mat_BlendToTarget_glow && BlendToTarget_glow_shader.isSupported)
        {
            mat_BlendToTarget_glow = new Material(BlendToTarget_glow_shader);
        }
        if (!mat_SelfIllum && SelfIllum_shader.isSupported)
        {
            mat_SelfIllum = new Material(SelfIllum_shader);
        }
        if (!mat_ToneMapping && ToneMapping_shader.isSupported)
        {
            mat_ToneMapping = new Material(ToneMapping_shader);
        }
        if (!mat_Decal && Decal_shader.isSupported)
        {
            mat_Decal = new Material(Decal_shader);
        }
        if (!mat_ShadowLight && ShadowLight_shader.isSupported)
        {
            mat_ShadowLight = new Material(ShadowLight_shader);
        }
        if (!mat_ActorHighLight && ActorHighLight_shader.isSupported)
        {
            mat_ActorHighLight = new Material(ActorHighLight_shader);
        }
        if (!mat_Disturbance && Disturbance_Shader.isSupported)
        {
            mat_Disturbance = new Material(Disturbance_Shader);
        }
        if (!mat_LightShaft && LightShaft_Shader.isSupported)
        {
            mat_LightShaft = new Material(LightShaft_Shader);
        }
        if (!mat_ShadowMask && ShadowMask_Shader.isSupported)
        {
            mat_ShadowMask = new Material(ShadowMask_Shader);
        }
        if (!mat_Sky && SkyShader.isSupported)
        {
            mat_Sky = new Material(SkyShader);
        }
        if (!mat_UIMask && UIMaskShader.isSupported)
        {
            mat_UIMask = new Material(UIMaskShader);
        }
        if (!mat_DebugRT && DebugRTShader.isSupported)
        {
            mat_DebugRT = new Material(DebugRTShader);
        }
        if (!matShadowLightVolume && ShadowLightVolume.isSupported)
        {
            matShadowLightVolume = new Material(ShadowLightVolume);
        }
        if (!matSSSObject && SSSObjectShader.isSupported)
        {
            matSSSObject = new Material(SSSObjectShader);
        }
        if (!matFXAA && FXAAShader.isSupported)
        {
            matFXAA = new Material(FXAAShader);
        }
        if (!matRimLight && RimLightShader.isSupported)
        {
            matRimLight = new Material(RimLightShader);
        }
        if (!matActorWhiting && ActorWhitingShader.isSupported)
        {
            matActorWhiting = new Material(ActorWhitingShader);
        }
        if (!matSSAOBlur && SSAOBlurShader.isSupported)
        {
            matSSAOBlur = new Material(SSAOBlurShader);
        }
        if (!matMRT && mrtShader.isSupported)
        {
            matMRT = new Material(mrtShader);
        }
        if (!matDepthCombine && DepthCombineShader.isSupported)
        {
            matDepthCombine = new Material(DepthCombineShader);
        }
        if (!matShadowDepth && ShadowDepthShader.isSupported)
        {
            matShadowDepth = new Material(ShadowDepthShader);
        }
        if (!FullScreenRainMaterial && FullScreenRainShader.isSupported)
        {
            FullScreenRainMaterial = new Material(FullScreenRainShader);
        }
        if (!RadialBlurMaterial && RadialBlurShader.isSupported)
        {
            RadialBlurMaterial = new Material(RadialBlurShader);
        }
        if (!MipFogMaterial && MipFogShader.isSupported)
        {
            MipFogMaterial = new Material(MipFogShader);

        }
        if (!CharacterMaterial && CharacterTransShader.isSupported)
        {
            CharacterMaterial = new Material(CharacterTransShader);

        }
        if (!TwistMaterial && TwistShader.isSupported)
        {
            TwistMaterial = new Material(TwistShader);

        }

        if (!matSelfIllumGlow && SelfIllumGlowShader.isSupported)
        {
            matSelfIllumGlow = new Material(SelfIllumGlowShader);
        }
        
        if (!matGaussBlur && GaussBlurShader.isSupported)
        {
            matGaussBlur = new Material(GaussBlurShader);
        }
        if (!matGaussBloom && GaussBloomShader.isSupported)
        {
            matGaussBloom = new Material(GaussBloomShader);
        }
        if (!matVertexShaft && VertexShaftshader.isSupported)
        {
            matVertexShaft = new Material(VertexShaftshader);
        }
        if (!matCheckerBoard && CheckerBoardShader.isSupported)
        {
            matCheckerBoard = new Material(CheckerBoardShader);
        }
        if (!matLutify && LutifyShader.isSupported)
        {
            matLutify = new Material(LutifyShader);
        }
        if (!matPlaneFog && PlaneFogShader.isSupported)
        {
            matPlaneFog = new Material(PlaneFogShader);
        }
        if (!matScreenRain && ScreenRainShader.isSupported)
        {
            matScreenRain = new Material(ScreenRainShader);
        }
        if (!matInputDisturb && InputDisturbShader.isSupported)
        {
            matInputDisturb = new Material(InputDisturbShader);
        }
    }
    void DestroyRenderTexture(ref RenderTexture tex)
    {
        if (tex != null)
        {
            tex.Release();
            if (!Application.isPlaying)
            {
                Object.DestroyImmediate(tex);
            }
            else
            {
                Object.Destroy(tex);
            }
            tex = null;
        }
    }
    void DestoryRes()
    {
        DestroyMaterial(mat_SSAO);
		DestroyMaterial(mat_SSAO2);
        DestroyMaterial(mat_combine);
        DestroyMaterial(mat_shadow_gen);
        DestroyMaterial(mat_SSR);
        DestroyMaterial(mat_Normal_Gen);
        DestroyMaterial(mat_GI);
        DestroyMaterial(mat_GI_Blur);
        DestroyMaterial(mat_Fog);
        DestroyMaterial(mat_BlendFrame);
        DestroyMaterial(mat_DOF);
        DestroyMaterial(mat_AddToTarget);
        DestroyMaterial(mat_Water);
        DestroyMaterial(mat_SSSSS);
        DestroyMaterial(mat_downsample_glow);
        DestroyMaterial(mat_blur_glow);
        DestroyMaterial(mat_BlendToTarget_glow);
        DestroyMaterial(mat_SelfIllum);
        DestroyMaterial(mat_ToneMapping);
        DestroyMaterial(mat_Decal);
        DestroyMaterial(mat_ShadowLight);
        DestroyMaterial(mat_ActorHighLight);
        DestroyMaterial(mat_Disturbance);
        DestroyMaterial(mat_LightShaft);
        DestroyMaterial(mat_ShadowMask);
        DestroyMaterial(mat_Sky);
        DestroyMaterial(mat_DebugRT);
        DestroyMaterial(matShadowLightVolume);
        DestroyMaterial(matSSSObject);
        DestroyMaterial(matFXAA);
        DestroyMaterial(mat_deferred_light);
        DestroyMaterial(matRimLight);
        DestroyMaterial(matActorWhiting);
        DestroyMaterial(matSSAOBlur);
        DestroyMaterial(matMRT);
        DestroyMaterial(matDepthCombine);
        DestroyMaterial(matShadowDepth);
        DestroyMaterial(FullScreenRainMaterial);
        DestroyMaterial(RadialBlurMaterial);
        DestroyMaterial(matGaussBlur);
        DestroyMaterial(matGaussBloom);
        DestroyMaterial(matVertexShaft);
        DestroyMaterial(MipFogMaterial);
        DestroyMaterial(CharacterMaterial);
        DestroyMaterial(TwistMaterial);
        DestroyMaterial(matSelfIllumGlow);
        DestroyMaterial(matCheckerBoard);
        DestroyMaterial(matLutify);
        DestroyMaterial(matPlaneFog);
        DestroyMaterial(matScreenRain);

        DestroyRenderTexture(ref final_tex);
        DestroyRenderTexture(ref gbuffer_tex);
        DestroyRenderTexture(ref colorbuffer_tex);
        DestroyRenderTexture(ref waterreflect_tex);

        DestroyRenderTexture(ref shadow_depth0);
        DestroyRenderTexture(ref shadow_depth1);
    }
    void OnDisable()
    {
        DestoryRes();
    }
    void OnDestory()
    {
        DestoryRes();
    }
    void OnEnable()
    {

        RenderTextureFormat fmt = RenderTextureFormat.ARGB32;
        if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RGInt))
        {
            //fmt = RenderTextureFormat.RGInt;
            Debug.Log("Support RenderTextureFormat RGInt");
        }
        if (shadow_depth0 == null)
        {
            shadow_depth0 = new RenderTexture(1024, 1024, 16, fmt);
        }
        if (shadow_depth1 == null)
        {
            shadow_depth1 = new RenderTexture(1024, 1024, 0, fmt);
        }

        shadowCameraNear.targetTexture = shadow_depth0;
        //if (cloneCamera != null)
        //{
        //    cloneCamera.SetReplacementShader(gbuffer, "RenderType");
        //    cloneCamera.enabled = true;
        //    cloneCamera.depth = 1;
        //    cloneCamera.targetTexture = null;
        //    cloneCamera.targetTexture = GetColorBuffer();
        //    cloneCamera.clearFlags = CameraClearFlags.SolidColor;
        //}
    }
    // Update is called once per frame
    public void tick() {
        if (SceneRenderSetting._Setting == null)
        {
            return;
        }
        Camera mainCam = GetComponent<Camera>();
        SceneRenderSetting._Setting.MainCamera = mainCam;
        Vector3 halfPixelOffset = Vector3.zero;

        Vector3 lastPos = transform.position;
        Quaternion lastRot = transform.rotation;
        Vector3 currentPos = lastPos;
        Quaternion currentRotate = lastRot;

        if (SceneRenderSetting._Setting.SyncSceneView && !Application.isPlaying)
        {
            currentPos = SceneRenderSetting._Setting.SceneCamera_Pos + halfPixelOffset;
            currentRotate = SceneRenderSetting._Setting.SceneCamera_Rot;


        }
        else
        {
            if (ar_enable)
            {
                Vector3 p = Vector3.zero;
                Quaternion q = Quaternion.identity;
                ar_manager.GetARPosition(ref p, ref q);
                currentPos = SceneRenderSetting._Setting.transform.position + p;
                currentRotate = q;
            }
            else
            {
                currentPos = SceneRenderSetting._Setting.transform.position + halfPixelOffset;
                currentRotate = SceneRenderSetting._Setting.transform.rotation;
            }

        }
        if((currentPos-lastPos).magnitude > 0.001f || currentRotate!=lastRot)
        {
            IsMoving = true;
        }
        else
        {
            IsMoving = false;
        }
        transform.position = currentPos;
        transform.rotation = currentRotate;

        if (mainCam != null)
        {
            if (mainCam.clearFlags == CameraClearFlags.Depth)
            {
                mainCam.clearFlags = CameraClearFlags.SolidColor;
            }
        }


        _MainCameraFrustum.Buildclipplane(mainCam);


        ForceField.SubmitShaderParam(mainCam.transform.position, _MainCameraFrustum);
        float size = SceneRenderSetting._Setting.MainLightCameraSize.x;
        if (quality.HalfShadowResolution == 1)
        {
            size *= 0.75f;
        }
        if (size != shadowCameraNear.orthographicSize)
        {
            shadowCameraNear.orthographicSize = size;
            NearStaticDepthRefresh = true;
        }

        if (SceneRenderSetting._Setting.MainLightCameraSize.y != shadowCameraFar.orthographicSize)
        {
            shadowCameraFar.orthographicSize = SceneRenderSetting._Setting.MainLightCameraSize.y;
        }

        if (SceneRenderSetting._Setting.MainLightCameraSize.z != shadowCameraSuperFar.orthographicSize)
        {
            shadowCameraSuperFar.orthographicSize = SceneRenderSetting._Setting.MainLightCameraSize.z;
        }

#if UNITY_EDITOR

#endif
        //quality.Level = CurrentQuality;
        quality.Update();

    }
    public static void ViewMatrix(Camera cam, ref Vector4[] view)
    {
        Vector3 right = cam.transform.TransformDirection(Vector3.right);
        Vector3 up = cam.transform.TransformDirection(Vector3.up);
        Vector3 forward = cam.transform.TransformDirection(Vector3.forward);
        Vector3 pos;
        pos.x = Vector3.Dot(right, cam.transform.position);
        pos.y = Vector3.Dot(up, cam.transform.position);
        pos.z = Vector3.Dot(forward, cam.transform.position);

        view[0].x = right.x; view[1].x = up.x; view[2].x = forward.x; view[3].x = 0.0f;
        view[0].y = right.y; view[1].y = up.y; view[2].y = forward.y; view[3].y = 0.0f;
        view[0].z = right.z; view[1].z = up.z; view[2].z = forward.z; view[3].z = 0.0f;
        view[0].w = -pos.x; view[1].w = -pos.y; view[2].w = -pos.z; view[3].w = 1.0f;

    }
    public static void ViewMatrix(Camera cam, ref Matrix4x4 view)
    {
        Vector3 right = cam.transform.TransformDirection(Vector3.right);
        Vector3 up = cam.transform.TransformDirection(Vector3.up);
        Vector3 forward = cam.transform.TransformDirection(Vector3.forward);
        Vector3 pos;
        pos.x = Vector3.Dot(right, cam.transform.position);
        pos.y = Vector3.Dot(up, cam.transform.position);
        pos.z = Vector3.Dot(forward, cam.transform.position);

        view.m00 = right.x; view.m10 = up.x; view.m20 = forward.x; view.m30 = 0.0f;
        view.m01 = right.y; view.m11 = up.y; view.m21 = forward.y; view.m31 = 0.0f;
        view.m02 = right.z; view.m12 = up.z; view.m22 = forward.z; view.m32 = 0.0f;
        view.m03 = -pos.x; view.m13 = -pos.y; view.m23 = -pos.z; view.m33 = 1.0f;

    }
    public static void ViewMatrix(Transform node, ref Matrix4x4 view)
    {
        Vector3 right = node.TransformDirection(Vector3.right);
        Vector3 up = node.TransformDirection(Vector3.up);
        Vector3 forward = node.TransformDirection(Vector3.forward);
        Vector3 pos;
        pos.x = Vector3.Dot(right, node.position);
        pos.y = Vector3.Dot(up, node.position);
        pos.z = Vector3.Dot(forward, node.position);

        view.m00 = right.x; view.m10 = up.x; view.m20 = forward.x; view.m30 = 0.0f;
        view.m01 = right.y; view.m11 = up.y; view.m21 = forward.y; view.m31 = 0.0f;
        view.m02 = right.z; view.m12 = up.z; view.m22 = forward.z; view.m32 = 0.0f;
        view.m03 = -pos.x; view.m13 = -pos.y; view.m23 = -pos.z; view.m33 = 1.0f;

    }
    public static void ProjMatrix(Camera cam, ref Matrix4x4 proj)
    {
        float angle = cam.fieldOfView * 0.5f * Mathf.Deg2Rad;
        float height = 1.0f / Mathf.Tan(angle);
        float width = height / cam.aspect;
        float near = cam.nearClipPlane;
        float far = cam.farClipPlane;
        float temp = far / (far - near);

        proj.m00 = width; proj.m01 = 0.0f; proj.m02 = 0.0f; proj.m03 = 0.0f;
        proj.m10 = 0.0f; proj.m11 = height; proj.m12 = 0.0f; proj.m13 = 0.0f;
        proj.m20 = 0.0f; proj.m21 = 0.0f; proj.m22 = temp; proj.m23 = 1.0f;
        proj.m30 = 0.0f; proj.m31 = 0.0f; proj.m32 = temp * near; proj.m33 = 0.0f;
    }
    public static Vector4 CameraFarCorner(Camera cam, float w_h)
    {
        Vector4 corner = Vector4.one;
        corner.y = cam.farClipPlane * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        corner.z = cam.farClipPlane;
        corner.x = w_h * corner.y;
        return corner;
    }

    Vector4 invViewport_main = Vector4.zero;
    public void OnRenderImageES20(RenderTexture source2, RenderTexture destination)
    {
        int ScreenWidth = quality.CurrentWidth;
        int ScreenHeight = quality.CurrentHeight;

        invViewport_main.x = 1.0f / (float)ScreenWidth;
        invViewport_main.y = 1.0f / (float)ScreenHeight;
        invViewport_main.z = 2.2f;// SceneRenderSetting._Setting.SSAOSampleRadius;
        invViewport_main.w = 0;


        Camera MainCamera = GetComponent<Camera>();
        Vector4 farcorner = CameraFarCorner(MainCamera, (float)quality.CurrentWidth / (float)quality.CurrentHeight);
        Matrix4x4 inView = Matrix4x4.identity;
        ViewMatrix(MainCamera, ref inView);

        if (SceneRenderSetting._Setting.EnableTwistScreen)
        {
            Twist += Time.deltaTime * 15;
            TwistMaterial.SetTexture("_MainTex", source2);
            TwistMaterial.SetFloat("_Twist", Twist);
            TwistMaterial.SetPass(0);
            Graphics.Blit(source2, destination, TwistMaterial);
            return;
        }
        else
        {
            Twist = 0f;
        }
     
        Graphics.SetRenderTarget(source2.colorBuffer, source2.depthBuffer);
        if (SceneRenderSetting._Setting.SkyTexture != null)
        {
            if (SceneRenderSetting._Setting.AmbientSky)
            {
                DrawSky(SceneRenderSetting._Setting.SkyTexture, farcorner, inView.inverse, SceneRenderSetting._Setting.AmbientColor * SceneRenderSetting._Setting.AmbientColorScale,false);

            }
            else
            {
                DrawSky(SceneRenderSetting._Setting.SkyTexture, farcorner, inView.inverse, new Color(1,1,1,1),false);
            }
        }
        //角色泛白...
        if (ActorWhitingCount > 0 && SceneRenderSetting._Setting.EnableActorWhiting)
        {
            DrawActorWhiting(SceneRenderSetting._Setting.ActorWhiting);
        }
        int visiable_water_count = Water.CameraCull(_MainCameraFrustum);
        RenderTexture lastFrame = GetLastFinalTex();
        Matrix4x4 MainViewMatrix = Matrix4x4.identity;
        ViewMatrix(GetComponent<Camera>(), ref MainViewMatrix);
        Matrix4x4 InvMainViewMatrix = MainViewMatrix.inverse;
        if (visiable_water_count > 0)
        {
            DrawUIMask();
            if (SceneRenderSetting._Setting.AmbientSky)
            {
                Water.DrawAllES20(mat_Water, SceneRenderSetting._Setting.SkyTexture, SceneRenderSetting._Setting.AmbientColor * SceneRenderSetting._Setting.AmbientColorScale);
            }
            else
            {
                Water.DrawAllES20(mat_Water, SceneRenderSetting._Setting.SkyTexture, Color.white);
            }
            Water.DrawSubWaterES20(mat_ScreenWater, Quad, gbuffer_tex, farcorner, lastFrame, InvMainViewMatrix, MainCamera);
        }

        invViewport_main.z = 1.0f;
        CopyBuffer(source2, lastFrame, invViewport_main);

        EffectCamera.targetTexture = null;
        EffectCamera.SetTargetBuffers(source2.colorBuffer, source2.depthBuffer);
        EffectCamera.Render();


        invViewport_main.z = 2.2f;
        //Graphics.Blit(source2, destination);
        mat_copy.SetVector("invViewport_Radius", invViewport_main);
        Graphics.Blit(source2, destination, mat_copy, 0);//power(1/2.2)
    }
    float Twist = 0f;
    public void OnRenderImageMerge(RenderTexture source2, RenderTexture destination)
    {

        int ScreenWidth = quality.CurrentWidth;
        int ScreenHeight = quality.CurrentHeight;


        RenderTexture source = GetColorBuffer();
        Camera MainCamera = GetComponent<Camera>();
        Vector4 farcorner = CameraFarCorner(MainCamera, (float)quality.CurrentWidth / (float)quality.CurrentHeight);
        if (SceneRenderSetting._Setting.ShowNormal)
        {
            Graphics.SetRenderTarget(source);
            mat_DebugRT.SetTexture("_MainTex", gbuffer_tex);
            mat_DebugRT.SetVector("_FarCorner", farcorner);
            mat_DebugRT.SetPass(0);
            Graphics.DrawMeshNow(Quad, Matrix4x4.identity);
            Graphics.Blit(source, destination);
            return;
        }
        if (SceneRenderSetting._Setting.ShowDepth)
        {
            Graphics.SetRenderTarget(source);
            mat_DebugRT.SetTexture("_MainTex", gbuffer_tex);
            mat_DebugRT.SetVector("_FarCorner", farcorner);
            mat_DebugRT.SetPass(1);
            Graphics.DrawMeshNow(Quad, Matrix4x4.identity);
            Graphics.Blit(source, destination);
            return;
        }


        invViewport_main.x = 1.0f / (float)ScreenWidth;
        invViewport_main.y = 1.0f / (float)ScreenHeight;
        invViewport_main.z = SceneRenderSetting._Setting.SSAOSampleRadius;
        invViewport_main.w = 0;
        CreateMaterials();

        RenderBuffer currentColorBuffer = source.colorBuffer;
        RenderBuffer currentDepthBuffer = source.depthBuffer;
        RenderTexture currentRenderTarget = source;//主
        RenderTexture diffuseRT = null;//辅
        if (source2.width == ScreenWidth && source2.height == ScreenHeight)
        {
            diffuseRT = source2;
        }
        else
        {
            diffuseRT = RenderTexture.GetTemporary(ScreenWidth, ScreenHeight, 0);
        }
        Graphics.Blit(source, diffuseRT);

        if (SceneRenderSetting._Setting.DebugRT)
        {
            Graphics.Blit(diffuseRT, destination);//because of no gammaspace changge
            return;

        }
        RenderTexture lastFrame = GetLastFinalTex();
        Matrix4x4 inView = Matrix4x4.identity;
        ViewMatrix(GetComponent<Camera>(), ref inView);
        if (DeferredDecal.Count() > 0)
        {
            Graphics.SetRenderTarget(source.colorBuffer, source.depthBuffer);
            DeferredDecal.DrawAll(mat_Decal, inView.inverse, Cube, farcorner, gbuffer_tex);
            Graphics.Blit(currentRenderTarget, diffuseRT);
        }
        Vector4 _AmbientColor = SceneRenderSetting._Setting.AmbientColor * SceneRenderSetting._Setting.AmbientColorScale;
        Vector4 _MainLightColor = SceneRenderSetting._Setting.MainLightColor * SceneRenderSetting._Setting.MainLightColorScale;

        Graphics.SetRenderTarget(currentColorBuffer, currentDepthBuffer);
        DrawUIMask();
        if (SceneRenderSetting._Setting.SkyTexture != null)
        {
            if (SceneRenderSetting._Setting.AmbientSky)
            {
                DrawSky(SceneRenderSetting._Setting.SkyTexture, farcorner, inView.inverse, SceneRenderSetting._Setting.AmbientColor * SceneRenderSetting._Setting.AmbientColorScale, false);

            }
            else
            {
                DrawSky(SceneRenderSetting._Setting.SkyTexture, farcorner, inView.inverse, new Color(1, 1, 1, 1), false);
            }
        }
        int specular_light_count = 0;
      /*  if (SceneRenderSetting._Setting.EnableActorHightLight && quality.PointLightSpecularLevel > 0)
        {
            specular_light_count = DrawActorHighLight(farcorner, invViewport_main, diffuseRT, quality.PointLightSpecularLevel);
        }
        if (SceneRenderSetting._Setting.EnableRimLight && quality.RimLightLevel == 1)
        {
            DrawRimLight(diffuseRT, invViewport_main, farcorner);
        }*/
        if (SceneRenderSetting._Setting.AmbientColorScale > 0.001f)
        {
            if (SceneRenderSetting._Setting.EnableSSAOPro)
            {
                RenderTexture cb = RenderTexture.GetTemporary(ScreenWidth / 2, ScreenHeight / 2, 0);
                Graphics.SetRenderTarget(cb);
                DrawSSAOMergeMode(cb.colorBuffer, cb.depthBuffer, invViewport_main, farcorner, diffuseRT, currentColorBuffer, currentDepthBuffer, 3, null);
                if (SceneRenderSetting._Setting.EnableSSAODebug)
                {
                    Graphics.Blit(cb, destination);
                    RenderTexture.ReleaseTemporary(cb);
                    return;
                }
                Graphics.SetRenderTarget(currentColorBuffer, currentDepthBuffer);
                DrawSSAOMergeMode(currentColorBuffer, currentDepthBuffer, invViewport_main, farcorner, diffuseRT, currentColorBuffer, currentDepthBuffer, 2, cb);
                RenderTexture.ReleaseTemporary(cb);
            }
            else if (SceneRenderSetting._Setting.EnableSSAO)
            {
                RenderTexture cb = RenderTexture.GetTemporary(ScreenWidth / 2, ScreenHeight / 2, 0);//屏幕变花的问题一定是temp的texture没有初始化Clear
                Graphics.SetRenderTarget(cb);

                DrawSSAOMergeMode(cb.colorBuffer, cb.depthBuffer, invViewport_main, farcorner, diffuseRT, currentColorBuffer, currentDepthBuffer, 4, null);
                if (SceneRenderSetting._Setting.EnableSSAODebug)
                {
                    Graphics.Blit(cb, destination);
                    RenderTexture.ReleaseTemporary(cb);
                    return;
                }
                Graphics.SetRenderTarget(currentColorBuffer, currentDepthBuffer);
                DrawSSAOMergeMode(currentColorBuffer, currentDepthBuffer, invViewport_main, farcorner, diffuseRT, currentColorBuffer, currentDepthBuffer, 2, cb);
                RenderTexture.ReleaseTemporary(cb);
            }
        }

        if (SceneRenderSetting._Setting.EnableGI)
        {

            RenderTexture gi_data = RenderTexture.GetTemporary(ScreenWidth / 2, ScreenHeight / 2, 0);
            RenderTexture gi_data_2 = RenderTexture.GetTemporary(ScreenWidth / 2, ScreenHeight / 2, 0);
            Vector4 temp_viewport = invViewport_main;
            temp_viewport.x = 1.0f / (float)gi_data.width;
            temp_viewport.y = 1.0f / (float)gi_data.height;
            temp_viewport.z = SceneRenderSetting._Setting.GI_Scale;

            DrawGI(temp_viewport, farcorner, diffuseRT, lastFrame, gi_data);
            BlurGI(temp_viewport, gi_data, gi_data_2);
            Graphics.SetRenderTarget(gi_data);
            GL.Clear(false, true, Color.clear);
            BlurGI(temp_viewport, gi_data_2, gi_data);
            RenderTexture.ReleaseTemporary(gi_data_2);
            temp_viewport.x = invViewport_main.x;
            temp_viewport.y = invViewport_main.y;
            temp_viewport.z = SceneRenderSetting._Setting.GI_Scale;
            RenderTexture Shadow_AO = null;
            AddToTarget(temp_viewport, gi_data, diffuseRT, currentRenderTarget, Shadow_AO);
            RenderTexture.ReleaseTemporary(gi_data);
        }
        if (SceneRenderSetting._Setting.EnableDeferredLight)
        {//point light
            //UpdateDeferredLight();

            Graphics.SetRenderTarget(currentColorBuffer, currentDepthBuffer);
            //dl
            Matrix4x4 MainCameraWorld = Matrix4x4.identity;
            ViewMatrix(MainCamera, ref MainCameraWorld);
            DeferredLight.DrawDeferredLight(
                deferred_light_mesh,
                mat_deferred_light,
                farcorner,
                invViewport_main,
                diffuseRT,
                gbuffer_tex,
                MainCamera,
                _MainCameraFrustum,
                MainCameraWorld,
                DeferredLightCamera,
                CubeShadowDepthShader,
                currentColorBuffer,
                currentDepthBuffer,
                quality.PointLightLevel,
                quality.PointLightDisappearLevel);
        }
        if (SceneRenderSetting._Setting.EnableShadowLight && quality.SpotLightLevel > 0)
        {//spotloight 体积光  no effect
            Color MainLightColor = Color.black;
            if (SceneRenderSetting._Setting.EnableShadow)
            {
                MainLightColor = SceneRenderSetting._Setting.MainLightColor * SceneRenderSetting._Setting.MainLightColorScale;
            }
            Matrix4x4 MainView = Matrix4x4.identity;
            ViewMatrix(GetComponent<Camera>(), ref MainView);
            DeferredShadowLight.DrawAll(
                mat_ShadowLight,
                matShadowLightVolume,
                shadow_depth1,
                shadow_depth0.depthBuffer,
                ShadowDepthShader,
                MainView,
                gbuffer_tex,
                diffuseRT,
                farcorner,
                invViewport_main,
                Get2x2RotateTexture(),
                GetRandomTex(),
                Cube,
                currentColorBuffer,
                currentDepthBuffer,
                _MainCameraFrustum,
                MainLightColor,
                transform.position,
                quality.SpotLightLevel
                );
        }
        if (SceneRenderSetting._Setting.EnableSSSSS)
        {
            DrawSSSSS(farcorner, invViewport_main, lastFrame, diffuseRT);
        }


  
        Matrix4x4 MainViewMatrix = Matrix4x4.identity;
        ViewMatrix(GetComponent<Camera>(), ref MainViewMatrix);
        Matrix4x4 InvMainViewMatrix = MainViewMatrix.inverse;

        Matrix4x4 shadow_cam_World = Matrix4x4.identity;
        ViewMatrix(shadowCameraNear, ref shadow_cam_World);
        Matrix4x4 main_shadow = MainViewMatrix.inverse;
        Matrix4x4 mainview_shadowview = shadow_cam_World * main_shadow;
        Matrix4x4 shadowcam_proj_unity = GL.GetGPUProjectionMatrix(shadowCameraNear.projectionMatrix, true);
        if (!Application.isEditor)
        {
            shadowcam_proj_unity.m11 *= -1;

        }

        //Vector3 MainLightPos = MainViewMatrix.MultiplyPoint(shadowCameraNear.transform.position);
        Vector4 MainLightDir_WorldSpace =  shadowCameraNear.transform.TransformDirection(Vector3.forward);
        Vector4 MainLightDir = MainViewMatrix.MultiplyVector(MainLightDir_WorldSpace).normalized;
        MainLightDir.w = SceneRenderSetting._Setting.MainLightBias.x;
        MainLightDir.z *= -1;//ViewMatrix坐标系转换问题
        Shader.SetGlobalMatrix("mainView_shadowView", mainview_shadowview);
        Shader.SetGlobalMatrix("shadowProj", shadowcam_proj_unity);
      //  Shader.SetGlobalVector("lightdir", MainLightDir);
       // Shader.SetGlobalVector("lightcolor", SceneRenderSetting._Setting.MainLightColor * SceneRenderSetting._Setting.MainLightColorScale);





        Graphics.SetRenderTarget(currentColorBuffer, currentDepthBuffer);
        //角色泛白...
        if (ActorWhitingCount > 0 && SceneRenderSetting._Setting.EnableActorWhiting)
        {
            DrawActorWhiting(SceneRenderSetting._Setting.ActorWhiting);
        }
        DrawScreenRain(farcorner, invViewport_main, currentRenderTarget,MainCamera);
        DrawRain(invViewport_main, farcorner, MainLightDir_WorldSpace, _MainLightColor, SceneRenderSetting._Setting.AmbientColor * SceneRenderSetting._Setting.FullScreenRain, InvMainViewMatrix, lastFrame, specular_light_count);
        int visiable_water_count = Water.CameraCull(_MainCameraFrustum);
        if (visiable_water_count > 0)
        {
            Vector4 copy_invViewport = invViewport_main;
            copy_invViewport.x = 0;
            copy_invViewport.y = 0;
            copy_invViewport.z = 1.0f;
            //Graphics.SetRenderTarget(lastFrame);
            //GL.Clear(false, true, new Color(0, 0, 0, 0));
            CopyBuffer(currentRenderTarget, lastFrame, copy_invViewport);
        }
        RenderTexture temp_copy = null;
        if (visiable_water_count > 0)
        {
            temp_copy = lastFrame;
            Graphics.SetRenderTarget(currentColorBuffer, currentDepthBuffer);
            int water_level = quality.WaterLevel;
            Water.DrawAll(
                mat_Water,
                gbuffer_tex,
                SceneRenderSetting._Setting.SkyTexture,
                temp_copy,
                farcorner,
                InvMainViewMatrix,
                SceneRenderSetting._Setting.AmbientColor * SceneRenderSetting._Setting.AmbientColorScale,
                currentColorBuffer,
                currentDepthBuffer,
                MainCamera,
                water_level);
            Water.DrawSubWater(mat_ScreenWater, Quad, gbuffer_tex, farcorner, lastFrame, InvMainViewMatrix, MainCamera);
            //if (SceneRenderSetting._Setting.EnableReflect)
            //{
            //    Graphics.SetRenderTarget(gbuffer_tex.colorBuffer, currentDepthBuffer);
            //    Water.DrawAllDepth(mat_Water);
            //    Graphics.SetRenderTarget(currentColorBuffer, currentDepthBuffer);
            //}
        }
        if (SceneRenderSetting._Setting.EnableReflect)
        {
            //reflect hightlight
            //note:
            //need stencil buffer
            DrawSSR(invViewport_main, farcorner, lastFrame, SceneRenderSetting._Setting.SkyTexture, diffuseRT);
        }
        if (SceneRenderSetting._Setting.EnablePlaneFog)
        {
            DrawPlaneFog(farcorner, invViewport_main, currentRenderTarget);
        }
        if (SceneRenderSetting._Setting.EnableFog && quality.FogLevel == 1)
        {
            if (SceneRenderSetting._Setting.MipFogTexture != null)
            {
                DrawMipFog(farcorner, invViewport_main, currentRenderTarget);
            }
            else
            {
                DrawFog(farcorner, invViewport_main, currentRenderTarget);
            }

        }
        if (SceneRenderSetting._Setting.LightShaft_Intensity > 0.0f)
        {
            if (SceneRenderSetting._Setting.EnableLightShaft)
            {
                if(quality.LightShaftLevels == 2)
                     DrawLightShaft(currentRenderTarget, invViewport_main);
                else if (quality.LightShaftLevels == 1)
                    DrawLightShaftLow(currentRenderTarget, invViewport_main);
            }
        }
        if (SceneRenderSetting._Setting.EnableGlow && quality.DOFLevel == 1)
        {
            DrawDof(invViewport_main, currentRenderTarget, currentColorBuffer, currentDepthBuffer, farcorner, lastFrame);
        }



        Graphics.Blit(currentRenderTarget, lastFrame);
        Graphics.SetRenderTarget(currentColorBuffer, currentDepthBuffer);
        DrawRadialBlur(lastFrame, invViewport_main);

        if (SceneRenderSetting._Setting.EnableInputDisturb)
        {
            DrawInputMesh.Instance.DrawMesh(Quad, matInputDisturb, currentRenderTarget, lastFrame);
  
        }

        if (SceneRenderSetting._Setting.EnableTransparent)
        {
            // draw particle effect

            EffectCamera.targetTexture = null;
            EffectCamera.SetTargetBuffers(currentColorBuffer, currentDepthBuffer);
            EffectCamera.Render();
        }
        if (SceneRenderSetting._Setting.EnableMainCharXRay)
        {
            if (quality.XRayLevel == 1)
            {
                cloneCamera.targetTexture = null;
                cloneCamera.SetTargetBuffers(currentColorBuffer, currentDepthBuffer);
                cloneCamera.clearFlags = CameraClearFlags.Nothing;
                cloneCamera.RenderWithShader(CharacterTransShader, "Tag");
            }
            else
            {
                cloneCamera.targetTexture = null;
                cloneCamera.SetTargetBuffers(currentColorBuffer, currentDepthBuffer);
                cloneCamera.clearFlags = CameraClearFlags.Nothing;
                cloneCamera.RenderWithShader(CharacterTransShader, "MainCharTag");
            }
        }
        if (SceneRenderSetting._Setting.EnableVertexShaft)
        {
           // VertexShaft.DrawAllVertexShaft(cloneCamera, currentColorBuffer, currentDepthBuffer, matVertexShaft, currentRenderTarget, Get2x2StepTexture(), invViewport_main);
            //  VertexShaft.DrawAllVolumeShit(cloneCamera, currentRenderTarget, matVertexShaft, currentRenderTarget, invViewport_main,Quad,deferred_light_mesh);
            VertexShaft.DrawAllLightShit(cloneCamera, currentRenderTarget, matVertexShaft, gbuffer_tex, invViewport_main, Quad, deferred_light_mesh,Get2x2StepTexture(),farcorner);
        }
        if (SceneRenderSetting._Setting.EnableGaussBlur)
        {
            DrawGuassBlur(currentRenderTarget);
        }
        if (SceneRenderSetting._Setting.EnableBloom)
        {
            DrawGuassBloom(currentRenderTarget);
        }



        if (SceneRenderSetting._Setting.EnableTwistScreen)
        {
            Twist += Time.deltaTime * 30;
            Graphics.Blit(currentRenderTarget, diffuseRT);
            TwistMaterial.SetTexture("_MainTex", diffuseRT);
            TwistMaterial.SetFloat("_Twist", Twist);
            Graphics.Blit(diffuseRT, currentRenderTarget, TwistMaterial, 0);
           
        }
        else
        {
            Twist = 0f;
        }



        invViewport_main.z = 2.2f;
        invViewport_main.x = 0.0f;
        invViewport_main.y = 0.0f;

        int width = quality.CurrentWidth;
        int height = quality.CurrentHeight;
        if (SceneRenderSetting._Setting.EnableToneMapping && quality.ToneMappingLevel == 1)
        {//becauese of tonemap ,no need 伽马矫正
            if (SceneRenderSetting._Setting.EnableLutify)
            {
                RenderTexture temp = RenderTexture.GetTemporary(width, height, 0);
                DrawToneMapping(currentRenderTarget, temp);
                Lutifys.DrawLutify(temp, destination, matLutify, SceneRenderSetting._Setting.LutifyColor);
                RenderTexture.ReleaseTemporary(temp);
            }
            else
            {
                DrawToneMapping(currentRenderTarget, destination);
            }
        }
        else
        {
            if (SceneRenderSetting._Setting.EnableLutify)
            {
                RenderTexture temp = RenderTexture.GetTemporary(width, height, 0);
                if (SceneRenderSetting._Setting.EnableFXAA && quality.FXAALevel == 1)
                {
                    mat_copy.SetVector("invViewport_Radius", invViewport_main);
                    Graphics.Blit(currentRenderTarget, temp, mat_copy, 0);
                    DoFXAA(temp, currentRenderTarget);
                    Lutifys.DrawLutify(currentRenderTarget, destination, matLutify, SceneRenderSetting._Setting.LutifyColor);
                }
                else
                {
                    mat_copy.SetVector("invViewport_Radius", invViewport_main);
                    Graphics.Blit(currentRenderTarget, temp, mat_copy, 0);
                    Lutifys.DrawLutify(temp, destination, matLutify, SceneRenderSetting._Setting.LutifyColor);
                }
                RenderTexture.ReleaseTemporary(temp);
            }
            else
            {
                if (SceneRenderSetting._Setting.EnableFXAA && quality.FXAALevel == 1)
                {
                    RenderTexture temp = RenderTexture.GetTemporary(width, height, 0);
                    mat_copy.SetVector("invViewport_Radius", invViewport_main);
                    Graphics.Blit(currentRenderTarget, temp, mat_copy, 0);
                    DoFXAA(temp, destination);
                    RenderTexture.ReleaseTemporary(temp);
                }
                else
                {
                    mat_copy.SetVector("invViewport_Radius", invViewport_main);
                    Graphics.Blit(currentRenderTarget, destination, mat_copy, 0);
                }
            }
        }

        if (source2.width == ScreenWidth && source2.height == ScreenHeight)
        {

        }
        else
        {
            RenderTexture.ReleaseTemporary(diffuseRT);
        }

     

    }
    public void OnRenderImage(RenderTexture source2, RenderTexture destination)
    {

        if (SceneRenderSetting._Setting == null)
        {

            Graphics.Blit(source2, destination);
            return;
        }

        //根据画质级别  检查阴影分辨率
        CheckShadowResolution();
        GetCheckerBoardTexture();

        if (quality.mode == GlobalQualitySetting.Mode.Fast ||
            UnityEngine.SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2)
        {
            OnRenderImageES20(GetColorBuffer(), destination);
            return;
        }
        if(quality.mode == GlobalQualitySetting.Mode.Merge)
        {
           OnRenderImageMerge(source2, destination);
           return;
        }

        int ScreenWidth = quality.CurrentWidth;
        int ScreenHeight = quality.CurrentHeight;

        RenderTexture old_source = source2;
        source2 = GetColorBuffer();
        Camera MainCamera = GetComponent<Camera>();
        Vector4 farcorner = CameraFarCorner(MainCamera, (float)quality.CurrentWidth / (float)quality.CurrentHeight);
        if (SceneRenderSetting._Setting.ShowNormal)
        {
            Graphics.SetRenderTarget(source2);
            mat_DebugRT.SetTexture("_MainTex", gbuffer_tex);
            mat_DebugRT.SetVector("_FarCorner", farcorner);
            mat_DebugRT.SetPass(0);
            Graphics.DrawMeshNow(Quad, Matrix4x4.identity);
            Graphics.Blit(source2, destination);
            return;
        }

        //surport_stencil =   RenderTexture.SupportsStencil(source);
        invViewport_main.x = 1.0f / (float)ScreenWidth;
        invViewport_main.y = 1.0f / (float)ScreenHeight;
        invViewport_main.z = SceneRenderSetting._Setting.SSAOSampleRadius;
        invViewport_main.w = 0;

        CreateMaterials();
        //Graphics.Blit (source, destination);
        //
        RenderTexture Lighting2 = null;
        if (old_source.width == ScreenWidth && old_source.height == ScreenHeight)
        {
            Lighting2 = old_source;
        }
        else
        {
            Lighting2 = RenderTexture.GetTemporary(ScreenWidth, ScreenHeight, 0);
            Graphics.SetRenderTarget(Lighting2);
            GL.Clear(true, true, Color.clear);
        }


        RenderTexture lastFrame = GetLastFinalTex();

        //Graphics.SetRenderTarget(Lighting);
        if (SceneRenderSetting._Setting.SkyTexture != null)
        {
            //Graphics.SetRenderTarget(source2.colorBuffer, source2.depthBuffer);
        }
        Matrix4x4 inView = Matrix4x4.identity;
        ViewMatrix(GetComponent<Camera>(), ref inView);
        if (DeferredDecal.Count() > 0)
        {
            Graphics.SetRenderTarget(source2.colorBuffer, source2.depthBuffer);

            DeferredDecal.DrawAll(mat_Decal, inView.inverse, Cube, farcorner, gbuffer_tex);
        }


        Vector4 _AmbientColor = SceneRenderSetting._Setting.AmbientColor * SceneRenderSetting._Setting.AmbientColorScale;
        Vector4 _MainLightColor = SceneRenderSetting._Setting.MainLightColor * SceneRenderSetting._Setting.MainLightColorScale;



        RenderBuffer currentColorBuffer = Lighting2.colorBuffer;
        RenderBuffer currentDepthBuffer = source2.depthBuffer;
        RenderTexture currentRenderTarget = Lighting2;
        RenderTexture diffuseRT = source2;

        //Graphics.SetRenderTarget(currentRenderTarget);
        //GL.Clear(true, true, new Color(0,0,0,0));
        Graphics.SetRenderTarget(currentColorBuffer, currentDepthBuffer);



       	CopySelfIllum(diffuseRT);

        DrawUIMask();
        if (SceneRenderSetting._Setting.SkyTexture != null)
        {
            if (SceneRenderSetting._Setting.AmbientSky)
            {
                DrawSky(SceneRenderSetting._Setting.SkyTexture, farcorner, inView.inverse, SceneRenderSetting._Setting.AmbientColor * SceneRenderSetting._Setting.AmbientColorScale, false);
            }
            else
            {
                DrawSky(SceneRenderSetting._Setting.SkyTexture, farcorner, inView.inverse, new Color(1.0f, 1.0f, 1.0f, 1.0f), false);
            }
        }
        int specular_light_count = 0;
        if (SceneRenderSetting._Setting.EnableActorHightLight && quality.PointLightSpecularLevel > 0)
        {
            specular_light_count = DrawActorHighLight(farcorner, invViewport_main, diffuseRT, quality.PointLightSpecularLevel);
        }
        if (SceneRenderSetting._Setting.EnableRimLight && quality.RimLightLevel == 1)
        {
            DrawRimLight(diffuseRT, invViewport_main, farcorner);
        }
        if (SceneRenderSetting._Setting.AmbientColorScale > 0.001f)
        {
			
				if (SceneRenderSetting._Setting.EnableSSAO) 
				{
					if (SceneRenderSetting._Setting.EnableSSAODebug) 
					{
						RenderTexture cb = RenderTexture.GetTemporary (ScreenWidth, ScreenHeight, 0);
						Graphics.SetRenderTarget (cb);
						DrawSSAO (cb.colorBuffer, cb.depthBuffer, invViewport_main, farcorner, diffuseRT, currentColorBuffer, currentDepthBuffer, 9);
						Graphics.Blit (cb, destination);
						RenderTexture.ReleaseTemporary (cb);
						return;
					}   

					DrawSSAO (currentColorBuffer, currentDepthBuffer, invViewport_main, farcorner, diffuseRT, currentColorBuffer, currentDepthBuffer, 3 - quality.AOLevel);
				} 
				else if (SceneRenderSetting._Setting.EnableSSAOPro) 
				{
					if (SceneRenderSetting._Setting.EnableSSAODebug) 
					{
						RenderTexture cb = RenderTexture.GetTemporary (ScreenWidth, ScreenHeight, 0);
						Graphics.SetRenderTarget (cb);
						DrawSSAO2 (cb.colorBuffer, cb.depthBuffer, invViewport_main, farcorner, diffuseRT, currentColorBuffer, currentDepthBuffer, 1);
						Graphics.Blit (cb, destination);
						RenderTexture.ReleaseTemporary (cb);
						return;
					}   

					DrawSSAO2 (currentColorBuffer, currentDepthBuffer, invViewport_main, farcorner, diffuseRT, currentColorBuffer, currentDepthBuffer, 0);
				}
				else if(!SceneRenderSetting._Setting.EnableSSAO || quality.AOLevel == 4)
				{
					DrawSSAO_Off (invViewport_main, farcorner, diffuseRT, 4);
				}

        }

        RenderTexture Shadow_AO = null;


        if (SceneRenderSetting._Setting.EnableGI)
        {


            RenderTexture gi_data = RenderTexture.GetTemporary(ScreenWidth / 2, ScreenHeight / 2, 0);
            RenderTexture gi_data_2 = RenderTexture.GetTemporary(ScreenWidth / 2, ScreenHeight / 2, 0);
            Vector4 temp_viewport = invViewport_main;
            temp_viewport.x = 1.0f / (float)gi_data.width;
            temp_viewport.y = 1.0f / (float)gi_data.height;
            temp_viewport.z = SceneRenderSetting._Setting.GI_Scale;

            DrawGI(temp_viewport, farcorner, diffuseRT, lastFrame, gi_data);
            BlurGI(temp_viewport, gi_data, gi_data_2);
            Graphics.SetRenderTarget(gi_data);
            GL.Clear(false, true, Color.clear);
            BlurGI(temp_viewport, gi_data_2, gi_data);
            RenderTexture.ReleaseTemporary(gi_data_2);
            temp_viewport.x = invViewport_main.x;
            temp_viewport.y = invViewport_main.y;
            temp_viewport.z = SceneRenderSetting._Setting.GI_Scale;
            AddToTarget(temp_viewport, gi_data, diffuseRT, currentRenderTarget, Shadow_AO);
            RenderTexture.ReleaseTemporary(gi_data);
        }
        if (SceneRenderSetting._Setting.EnableDeferredLight)
        {
            //UpdateDeferredLight();

            Graphics.SetRenderTarget(currentColorBuffer, currentDepthBuffer);
            //dl
            Matrix4x4 MainCameraWorld = Matrix4x4.identity;
            ViewMatrix(MainCamera, ref MainCameraWorld);
            DeferredLight.DrawDeferredLight(
                deferred_light_mesh,
                mat_deferred_light,
                farcorner,
                invViewport_main,
                diffuseRT,
                gbuffer_tex,
                MainCamera,
                _MainCameraFrustum,
                MainCameraWorld,
                DeferredLightCamera,
                CubeShadowDepthShader,
                currentColorBuffer,
                currentDepthBuffer,
                quality.PointLightLevel,
                quality.PointLightDisappearLevel);
        }
        //if (SceneRenderSetting._Setting.EnableReflect || SceneRenderSetting._Setting.EnableSSAO) {
        //	RenderTexture.ReleaseTemporary(Shadow_AO);
        //}
        if (SceneRenderSetting._Setting.EnableShadowLight && quality.SpotLightLevel > 0)
        {
            Color MainLightColor = Color.black;
            if (SceneRenderSetting._Setting.EnableShadow)
            {
                MainLightColor = SceneRenderSetting._Setting.MainLightColor * SceneRenderSetting._Setting.MainLightColorScale;
            }
            Matrix4x4 MainView = Matrix4x4.identity;
            ViewMatrix(GetComponent<Camera>(), ref MainView);
            DeferredShadowLight.DrawAll(
                mat_ShadowLight,
                matShadowLightVolume,
                shadow_depth1,
                shadow_depth0.depthBuffer,
                ShadowDepthShader,
                MainView,
                gbuffer_tex,
                diffuseRT,
                farcorner,
                invViewport_main,
                Get2x2RotateTexture(),
                GetRandomTex(),
                Cube,
                currentColorBuffer,
                currentDepthBuffer,
                _MainCameraFrustum,
                MainLightColor,
                transform.position,
                quality.SpotLightLevel
                );
        }

        Matrix4x4 MainViewMatrix = Matrix4x4.identity;
        ViewMatrix(GetComponent<Camera>(), ref MainViewMatrix);
        Matrix4x4 InvMainViewMatrix = MainViewMatrix.inverse;

        Matrix4x4 shadow_cam_World = Matrix4x4.identity;
        ViewMatrix(shadowCameraNear, ref shadow_cam_World);
        Matrix4x4 main_shadow = MainViewMatrix.inverse;
        Matrix4x4 mainview_shadowview = shadow_cam_World * main_shadow;
        Matrix4x4 shadowcam_proj_unity = GL.GetGPUProjectionMatrix(shadowCameraNear.projectionMatrix, true);
        if (!Application.isEditor)
        {
            shadowcam_proj_unity.m11 *= -1;

        }

        //Vector3 MainLightPos = MainViewMatrix.MultiplyPoint(shadowCameraNear.transform.position);
        Vector4 MainLightDir_WorldSpace = shadowCameraNear.transform.TransformDirection(Vector3.forward);
        Vector4 MainLightDir = MainViewMatrix.MultiplyVector(MainLightDir_WorldSpace).normalized;
        MainLightDir.w = SceneRenderSetting._Setting.MainLightBias.x;
        Shader.SetGlobalMatrix("mainView_shadowView", mainview_shadowview);
        Shader.SetGlobalMatrix("shadowProj", shadowcam_proj_unity);
        
        Shader.SetGlobalVector("lightdir", MainLightDir);
        Shader.SetGlobalVector("lightcolor", SceneRenderSetting._Setting.MainLightColor * SceneRenderSetting._Setting.MainLightColorScale);// * shadowCamera.orthographicSize*0.1f);    
        {
            if (SceneRenderSetting._Setting.MainLightColorScale > 0.001f)
            {


                if (SceneRenderSetting._Setting.EnableShadow && quality.ShadowLevel > 0)
                {
                    int id = Time.frameCount % 4;
                    DrawShadowDepth();

                    shadowCameraNear.targetTexture = shadow_depth1;

                    Matrix4x4 ShadowView = Matrix4x4.identity;
                    ViewMatrix(shadowCameraNear, ref ShadowView);
                    shadow_depth1.filterMode = FilterMode.Point;
                    Shader.SetGlobalTexture("_ShadowDepth", shadow_depth1);


                    Graphics.SetRenderTarget(currentColorBuffer, currentDepthBuffer);

                    if (CheckerBoard_Enable && quality.ResolutionLevel > 1)
                    {
                        DrawShadowCheckerBoard(currentColorBuffer, currentDepthBuffer, MainViewMatrix, farcorner, invViewport_main, diffuseRT, shadowCameraNear, SceneRenderSetting._Setting.MainLightBias.x, 4 - quality.ShadowLevel);
                    }
                    else
                    {//blend one one  后效果更佳明亮
                        DrawShadow(MainViewMatrix, farcorner, invViewport_main, diffuseRT, shadowCameraNear, SceneRenderSetting._Setting.MainLightBias.x, 4 - quality.ShadowLevel);
                        DrawShadow(MainViewMatrix, farcorner, invViewport_main, diffuseRT, shadowCameraSuperFar, SceneRenderSetting._Setting.MainLightBias.z, 4);
                    }
                }
                else
                {
                    DrawShadow(MainViewMatrix, farcorner, invViewport_main, diffuseRT, shadowCameraSuperFar, SceneRenderSetting._Setting.MainLightBias.z, 4);
                }
            }
        }




        if (SceneRenderSetting._Setting.EnableSSSSS)
        {
            DrawSSSSS(farcorner, invViewport_main, lastFrame, diffuseRT);

        }

        Graphics.SetRenderTarget(currentColorBuffer, currentDepthBuffer);
        //角色泛白...
        if (ActorWhitingCount > 0 && SceneRenderSetting._Setting.EnableActorWhiting)
        {
            DrawActorWhiting(SceneRenderSetting._Setting.ActorWhiting);
        }
        DrawRain(invViewport_main, farcorner, MainLightDir_WorldSpace, _MainLightColor, SceneRenderSetting._Setting.AmbientColor * SceneRenderSetting._Setting.FullScreenRain, InvMainViewMatrix, lastFrame, specular_light_count);
        int visiable_water_count = Water.CameraCull(_MainCameraFrustum);
        if (visiable_water_count > 0)
        {
            Vector4 copy_invViewport = invViewport_main;
            copy_invViewport.x = 0;
            copy_invViewport.y = 0;
            copy_invViewport.z = 1.0f;
            //Graphics.SetRenderTarget(lastFrame);
            //GL.Clear(false, true, new Color(0, 0, 0, 0));
            CopyBuffer(currentRenderTarget, lastFrame, copy_invViewport);
        }
        RenderTexture temp_copy = null;
        if (visiable_water_count > 0)
        {
            temp_copy = lastFrame;
            Graphics.SetRenderTarget(currentColorBuffer, currentDepthBuffer);
            int water_level = quality.WaterLevel;
            Water.DrawAll(
                mat_Water,
                gbuffer_tex,
                SceneRenderSetting._Setting.SkyTexture,
                temp_copy,
                farcorner,
                InvMainViewMatrix,
                SceneRenderSetting._Setting.AmbientColor * SceneRenderSetting._Setting.AmbientColorScale,
                currentColorBuffer,
                currentDepthBuffer,
                MainCamera,
                water_level);
            Water.DrawSubWater(mat_ScreenWater, Quad, gbuffer_tex, farcorner, lastFrame, InvMainViewMatrix, MainCamera);
            //if (SceneRenderSetting._Setting.EnableReflect)
            //{
            //    Graphics.SetRenderTarget(gbuffer_tex.colorBuffer, currentDepthBuffer);
            //    Water.DrawAllDepth(mat_Water);
            //    Graphics.SetRenderTarget(currentColorBuffer, currentDepthBuffer);
            //}
        }

        if (SceneRenderSetting._Setting.EnableReflect)
        {
            //reflect hightlight
            //note:
            //need stencil buffer
            DrawSSR(invViewport_main, farcorner, lastFrame, SceneRenderSetting._Setting.SkyTexture, diffuseRT);
        }


        if (SceneRenderSetting._Setting.EnableFog && quality.FogLevel == 1)
        {
            if (SceneRenderSetting._Setting.MipFogTexture != null)
            {
                DrawMipFog(farcorner, invViewport_main, currentRenderTarget);
            }
            else
            {
                DrawFog(farcorner, invViewport_main, currentRenderTarget);
            }
        }
        else
        {
            //DrawMipFog
        }
        FogPlane.DrawPlaneFog(matPlaneFog,Quad, gbuffer_tex, farcorner, lastFrame, InvMainViewMatrix, MainCamera);
        if (SceneRenderSetting._Setting.LightShaft_Intensity > 0.0f)
        {
            if (SceneRenderSetting._Setting.EnableLightShaft && quality.LightShaftLevels == 1)
            {
                DrawLightShaft(currentRenderTarget, invViewport_main);
            }
            else
            {
                DrawLightShaftLow(currentRenderTarget, invViewport_main);
            }
        }
        if (SceneRenderSetting._Setting.EnableGlow && quality.DOFLevel == 1)
        {
            DrawDof(invViewport_main, currentRenderTarget, currentColorBuffer, currentDepthBuffer, farcorner, lastFrame);
        }
        {
            Graphics.Blit(currentRenderTarget, lastFrame);
            //if (Water.Count() > 0)
            //{

            //}
            Graphics.SetRenderTarget(currentColorBuffer, currentDepthBuffer);
            DrawRadialBlur(lastFrame, invViewport_main);
        }
        Shader.SetGlobalTexture("_ColorTex", lastFrame);
        Shader.SetGlobalTexture("_DepthTex", gbuffer_tex);
        Shader.SetGlobalVector("_InvViewport", invViewport_main);
        Shader.SetGlobalVector("_FarCorner", farcorner);
        if (SceneRenderSetting._Setting.EnableShadow)
        {
            Shader.SetGlobalTexture("_ShadowDepth", shadow_depth1);
        }
        else
        {
            Shader.SetGlobalTexture("_ShadowDepth", Texture2D.whiteTexture);
        }
        Vector4 invVPShadow = new Vector4(1.0f / (float)shadow_depth1.width, 1.0f / (float)shadow_depth1.height, SceneRenderSetting._Setting.MainLightShadowSampleRadius, shadowCameraNear.farClipPlane);
        Shader.SetGlobalVector("invViewportShadow", invVPShadow);
        Shader.SetGlobalColor("_AmbientColor", SceneRenderSetting._Setting.AmbientColor * SceneRenderSetting._Setting.AmbientColorScale);
        //
        Shader.SetGlobalColor("_Time01", Vector4.one * Time.time);
        cloneCamera.SetTargetBuffers(currentColorBuffer, currentDepthBuffer);
        cloneCamera.clearFlags = CameraClearFlags.Nothing;
        Shader.SetGlobalVector("unilightDir", SceneRenderSetting._Setting.MainLightDirection);
        Shader.SetGlobalVector("uniLightColor", SceneRenderSetting._Setting.MainLightColor * SceneRenderSetting._Setting.MainLightColorScale);
        cloneCamera.RenderWithShader(SelfIllumGlowShader, "RenderTag");

        
        if (SceneRenderSetting._Setting.EnableTransparent)
        {
            // draw particle effect

            EffectCamera.targetTexture = null;
            EffectCamera.SetTargetBuffers(currentColorBuffer, currentDepthBuffer);
            EffectCamera.Render();
        }
        if (SceneRenderSetting._Setting.EnableMainCharXRay)
        {
            if (quality.XRayLevel == 1)
            {
                cloneCamera.targetTexture = null;
                cloneCamera.SetTargetBuffers(currentColorBuffer, currentDepthBuffer);
                cloneCamera.clearFlags = CameraClearFlags.Nothing;
                cloneCamera.RenderWithShader(CharacterTransShader, "Tag");
            }
            else
            {
                cloneCamera.targetTexture = null;
                cloneCamera.SetTargetBuffers(currentColorBuffer, currentDepthBuffer);
                cloneCamera.clearFlags = CameraClearFlags.Nothing;
                cloneCamera.RenderWithShader(CharacterTransShader, "MainCharTag");
            }
        }
        if (SceneRenderSetting._Setting.EnableTwistScreen)
        {
            Twist += Time.deltaTime * 30;
            TwistMaterial.SetTexture("_MainTex", lastFrame);
            TwistMaterial.SetFloat("_Twist", Twist);
            TwistMaterial.SetPass(0);
            Graphics.Blit(lastFrame, destination, TwistMaterial);
            return;
        }
        else
        {
            Twist = 0f;
        }
        if (SceneRenderSetting._Setting.EnableVertexShaft)
        {
          //  VertexShaft.DrawAllVertexShaft(cloneCamera, currentColorBuffer, currentDepthBuffer, matVertexShaft, currentRenderTarget,Get2x2StepTexture());
        }
        if (SceneRenderSetting._Setting.EnableGaussBlur)
        {
            DrawGuassBlur(currentRenderTarget);
        }
        if (SceneRenderSetting._Setting.EnableBloom)
        {
            DrawGuassBloom(currentRenderTarget);
        }
        invViewport_main.z = 2.2f;
        invViewport_main.x = 0.0f;
        invViewport_main.y = 0.0f;
   
        int width = quality.CurrentWidth;
        int height = quality.CurrentHeight;
        if (SceneRenderSetting._Setting.EnableToneMapping && quality.ToneMappingLevel == 1)
        {//becauese of tonemap ,no need 伽马矫正
            if (SceneRenderSetting._Setting.EnableLutify)
            {
                RenderTexture temp = RenderTexture.GetTemporary(width, height, 0);
                DrawToneMapping(currentRenderTarget, temp);
                Lutifys.DrawLutify(temp, destination, matLutify, SceneRenderSetting._Setting.LutifyColor);
                RenderTexture.ReleaseTemporary(temp);
            }
            else
            {
                DrawToneMapping(currentRenderTarget, destination);
            }
        }
        else
        {
            if (SceneRenderSetting._Setting.EnableLutify)
            {
                RenderTexture temp = RenderTexture.GetTemporary(width, height, 0);
                if (SceneRenderSetting._Setting.EnableFXAA && quality.FXAALevel == 1)
                {
                    mat_copy.SetVector("invViewport_Radius", invViewport_main);
                    Graphics.Blit(currentRenderTarget, temp, mat_copy, 0);
                    DoFXAA(temp, currentRenderTarget);
                    Lutifys.DrawLutify(currentRenderTarget, destination, matLutify, SceneRenderSetting._Setting.LutifyColor);
                }
                else
                {
                    mat_copy.SetVector("invViewport_Radius", invViewport_main);
                    Graphics.Blit(currentRenderTarget, temp, mat_copy, 0);
                    Lutifys.DrawLutify(temp, destination, matLutify, SceneRenderSetting._Setting.LutifyColor);
                }
                RenderTexture.ReleaseTemporary(temp);
            }
            else
            {
                if (SceneRenderSetting._Setting.EnableFXAA && quality.FXAALevel == 1)
                {
                    RenderTexture temp = RenderTexture.GetTemporary(width, height, 0);
                    mat_copy.SetVector("invViewport_Radius", invViewport_main);
                    Graphics.Blit(currentRenderTarget, temp, mat_copy, 0);
                    DoFXAA(temp, destination);
                    RenderTexture.ReleaseTemporary(temp);
                }
                else
                {
                    mat_copy.SetVector("invViewport_Radius", invViewport_main);
                    Graphics.Blit(currentRenderTarget, destination, mat_copy, 0);
                }
            }
        }

        if (old_source.width == ScreenWidth && old_source.height == ScreenHeight)
        {

        }
        else
        {
            RenderTexture.ReleaseTemporary(Lighting2);
        }

    }

    void OnPreRender()
    {

    }
    void OnPostRender()
    {

    }

    static Vector3 FixPosition(Camera dstCamera, Vector3 pos, int texture_width)
    {
        Vector3 shadow_space_pos = dstCamera.transform.InverseTransformPoint(pos);
        float pixel_size = dstCamera.orthographicSize * 2 / (float)texture_width;
        Vector3 ceil_pos = shadow_space_pos / pixel_size;
        ceil_pos = new Vector3(Mathf.Ceil(ceil_pos.x), Mathf.Ceil(ceil_pos.y), Mathf.Ceil(ceil_pos.z));
        ceil_pos *= pixel_size;
        ceil_pos = dstCamera.transform.TransformPoint(ceil_pos);
        return ceil_pos;
    }

    Vector4 screenParam = Vector4.zero;
    Vector4 aaoffset = Vector4.zero;
    RenderBuffer[] renderbuffs = new RenderBuffer[2];

    void DrawShadowDepth()
    {
        if (NearStaticDepthRefresh)
        {
            shadowCameraNear.targetTexture = shadow_depth0;
            shadowCameraNear.clearFlags = CameraClearFlags.SolidColor;
            int static_mask = 1 << LayerMask.NameToLayer("Default");
            static_mask |= 1 << LayerMask.NameToLayer("Water");
            shadowCameraNear.cullingMask = static_mask;
            shadowCameraNear.RenderWithShader(ShadowDepthShader, "RenderType");
            NearStaticDepthRefresh = false;
        }

        shadowCameraNear.targetTexture = null;
        shadowCameraNear.SetTargetBuffers(shadow_depth0.colorBuffer, shadow_depth0.depthBuffer);
        shadowCameraNear.clearFlags = CameraClearFlags.Depth;
        shadowCameraNear.cullingMask = 1 << LayerMask.NameToLayer("Player");
        shadowCameraNear.RenderWithShader(ShadowDepthBAShader, "RenderType");
        shadowCameraNear.targetTexture = null;
        shadow_depth0.filterMode = FilterMode.Point;
        shadow_depth1.filterMode = FilterMode.Point;
        //清理背景为白色
        Graphics.SetRenderTarget(shadow_depth1.colorBuffer, shadow_depth0.depthBuffer);
        matDepthCombine.SetTexture("_ShadowDepth", shadow_depth0);
        matDepthCombine.SetPass(0);
        Graphics.DrawMeshNow(Quad, Matrix4x4.identity, 0);

        //通道合并
        matDepthCombine.SetTexture("_ShadowDepth", shadow_depth0);
        matDepthCombine.SetPass(1);
        Graphics.DrawMeshNow(Quad, Matrix4x4.identity, 0);
    }
    void OnPreCull()
    {
        if (SceneRenderSetting._Setting == null)
        {
            return;
        }
        int ScreenWidth = quality.CurrentWidth;
        int ScreenHeight = quality.CurrentHeight;

        Shader.SetGlobalTexture("_OIT2X2Tex", GetOIT2X2Texture());
        screenParam.x = 1.0f / (float)ScreenWidth;
        screenParam.y = 1.0f / (float)ScreenHeight;
        Shader.SetGlobalVector("_OITScreenParams", screenParam);
        //Shader.SetGlobalVector("_OITScreenParams", new Vector4(1.0f / (float)ScreenWidth, 1.0f / (float)ScreenHeight, 0, 0));
        if (SceneRenderSetting._Setting != null && SceneRenderSetting._Setting.EnableAA)
        {
            //Vector4 aaoffset = Vector4.zero;
            float x = 0.0f;
            float y = 0.0f;
            int idx = Time.frameCount % 2;
            if (idx == 0)
            {
                x = 0.5f;
                y = 0.5f;
            }

            //aaoffset = new Vector4(x / (float)ScreenWidth, y / (float)ScreenHeight, 0, 0);
            aaoffset.x = x / (float)ScreenWidth;
            aaoffset.y = y / (float)ScreenHeight;
            Shader.SetGlobalVector("_AAOffset", aaoffset);
        }
        else
        {
            Shader.SetGlobalVector("_AAOffset", Vector4.zero);
        }

        Camera mainCam = GetComponent<Camera>();

        mainCam.nearClipPlane = SceneRenderSetting._Setting.CameraNearCullPlane;
        mainCam.farClipPlane = SceneRenderSetting._Setting.CameraFarCullPlane;
        mainCam.fieldOfView = SceneRenderSetting._Setting.CameraFOV;



        //
        cloneCamera.nearClipPlane = mainCam.nearClipPlane;
        cloneCamera.farClipPlane = mainCam.farClipPlane;
        cloneCamera.fieldOfView = mainCam.fieldOfView;
        //cloneCamera.cullingMask   = mainCam.cullingMask;
        cloneCamera.depth = mainCam.depth;
        cloneCamera.orthographic = mainCam.orthographic;
        if (cloneCamera.enabled)
        {
            cloneCamera.enabled = false;
        }

        EffectCamera.nearClipPlane = mainCam.nearClipPlane;
        EffectCamera.farClipPlane = mainCam.farClipPlane;
        EffectCamera.fieldOfView = mainCam.fieldOfView;
        EffectCamera.depth = mainCam.depth;
        EffectCamera.orthographic = mainCam.orthographic;

        bool bMRT =  true;
        if (UnityEngine.SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2)
        {
            bMRT = false;
        }
        //es20 = true;
        if(Application.isEditor)
        {
            bMRT = SceneRenderSetting._Setting.UseMRT;
        }

        Vector3 _WorldUpInViewSpace = mainCam.transform.InverseTransformDirection(Vector3.up);
        Shader.SetGlobalVector("_WorldUpInViewSpace", _WorldUpInViewSpace.normalized);



        //检查是否需要 更新阴影深度信息 
        if (SceneRenderSetting._Setting != null && SceneRenderSetting._Setting.EnableShadow)
        {
            Vector3 lightdir = SceneRenderSetting._Setting.MainLightDirection;
            Vector3 mainCameraPos = transform.position;

            Quaternion q_X = Quaternion.AngleAxis(lightdir.x, Vector3.right);
            Quaternion q_Y = Quaternion.AngleAxis(lightdir.y, Vector3.up);
            Quaternion qRotate = q_Y * q_X;


            Vector3 dir = transform.TransformDirection(Vector3.forward);
            Vector3 shadow_space_dir = qRotate * (-Vector3.forward);
            Vector3 shadow_space_up = qRotate * (Vector3.up);
            Vector3 shadow_space_right = qRotate * (Vector3.right);


            shadowCameraNear.clearFlags = CameraClearFlags.SolidColor;
            shadowCameraFar.clearFlags = CameraClearFlags.SolidColor;
            shadowCameraSuperFar.clearFlags = CameraClearFlags.SolidColor;
            shadowCameraNear.backgroundColor = Color.white;
            shadowCameraFar.backgroundColor = Color.white;
            shadowCameraSuperFar.backgroundColor = Color.white;
            shadowCameraNear.nearClipPlane = SceneRenderSetting._Setting.ShadowCameraNear;
            shadowCameraNear.farClipPlane = SceneRenderSetting._Setting.ShadowCameraFar;

            int width = 1024;
            if (shadow_depth0 != null)
                width = shadow_depth0.width;

            Vector3 focus_center = mainCameraPos + dir * (shadowCameraNear.orthographicSize + mainCam.nearClipPlane);

            Vector3 near = focus_center + shadow_space_dir * shadowCameraNear.farClipPlane * 0.5f;
            Vector3 far = focus_center + shadow_space_dir * shadowCameraFar.farClipPlane * 0.5f;
            Vector3 superfar = mainCameraPos + dir * (shadowCameraNear.orthographicSize * 2 + shadowCameraFar.orthographicSize + shadowCameraSuperFar.orthographicSize) + shadow_space_dir * shadowCameraSuperFar.farClipPlane * 0.5f;

            int id = Time.frameCount % 4;
            Vector3 dst_dir = qRotate * Vector3.forward;
            Vector3 origin_dir = shadowCameraNear.transform.rotation * Vector3.forward;
            Vector3 dst_point = FixPosition(shadowCameraNear, near, width);
            Vector3 farPos = FixPosition(shadowCameraSuperFar, superfar, width);
            if (Mathf.Acos(Vector3.Dot(dst_dir, origin_dir)) > 0.0349f || !Application.isPlaying)
            {
                NearStaticDepthRefresh = true;
                shadowCameraNear.transform.rotation = qRotate;
                shadowCameraNear.transform.position = dst_point;
                shadowCameraSuperFar.transform.rotation = qRotate;
                shadowCameraSuperFar.transform.position = farPos;
            }
            else
            {

                Vector3 origin_point = shadowCameraNear.transform.position;
                if (Vector3.Distance(dst_point, origin_point) > 2.0f)
                {
                    NearStaticDepthRefresh = true;
                    shadowCameraNear.transform.rotation = qRotate;
                    shadowCameraNear.transform.position = dst_point;
                    shadowCameraSuperFar.transform.rotation = qRotate;
                    shadowCameraSuperFar.transform.position = farPos;
                }
            }
  
        }
        //更新阴影 深度信息 
        if (quality.mode == GlobalQualitySetting.Mode.Fast|| quality.mode == GlobalQualitySetting.Mode.Merge)
        {
            if (SceneRenderSetting._Setting.MainLightColorScale > 0.001f)
            {
                if (SceneRenderSetting._Setting.EnableShadow)
                {
                    DrawShadowDepth();

                    shadowCameraNear.targetTexture = shadow_depth1;

                    Matrix4x4 ShadowView = Matrix4x4.identity;
                    ViewMatrix(shadowCameraNear, ref ShadowView);

                    Shader.SetGlobalTexture("_ShadowDepth", shadow_depth1);
                    Vector4 v_shadow = new Vector4(1.0f / shadow_depth0.width, 1.0f / shadow_depth0.height, SceneRenderSetting._Setting.MainLightShadowSampleRadius, shadowCameraNear.farClipPlane);
                    Shader.SetGlobalVector("invShadowViewport", v_shadow);
                    Shader.SetGlobalMatrix("_ShadowView", ShadowView);
                    Vector4 ShadowProj = Vector4.zero;

                    Matrix4x4 proj_matrix = GL.GetGPUProjectionMatrix(shadowCameraNear.projectionMatrix, true);

                    ShadowProj.x = Mathf.Abs(proj_matrix.m00);
                    ShadowProj.y = Mathf.Abs(proj_matrix.m11);
                    ShadowProj.z = Mathf.Abs(proj_matrix.m22);
                    ShadowProj.w = Mathf.Abs(proj_matrix.m23);

                    Shader.SetGlobalMatrix("_ShadowProj", proj_matrix);
                    Shader.SetGlobalVector("MainColor", SceneRenderSetting._Setting.MainLightColor * SceneRenderSetting._Setting.MainLightColorScale);
                    Shader.SetGlobalVector("AmbientColor", SceneRenderSetting._Setting.AmbientColor * SceneRenderSetting._Setting.AmbientColorScale);
                    Vector4 MainLightDir_WorldSpace = shadowCameraNear.transform.TransformDirection(Vector3.forward);
                    MainLightDir_WorldSpace.w = SceneRenderSetting._Setting.MainLightBias.x;
      
                    Shader.SetGlobalVector("MainDir", MainLightDir_WorldSpace);
                    Shader.SetGlobalVector("FogColor", SceneRenderSetting._Setting.Fog_Color);
                    Vector4 fog_param = new Vector4(SceneRenderSetting._Setting.Fog_Start, SceneRenderSetting._Setting.Fog_End, SceneRenderSetting._Setting.Fog_Attenuation, 0);
                    Shader.SetGlobalVector("FogParam", fog_param);
                    RenderTexture lastFrame = GetLastFinalTex();
                    Shader.SetGlobalTexture("_ColorTex", lastFrame);
                    //mat_shadow_gen.SetVector("mainInvViewport", invViewport);
                }
            }
        }
   
        RenderTexture cbuffer = GetColorBuffer();
        if (quality.mode == GlobalQualitySetting.Mode.Fast)
        {
            cloneCamera.clearFlags = CameraClearFlags.SolidColor;
            cloneCamera.SetTargetBuffers(cbuffer.colorBuffer, cbuffer.depthBuffer);
            cloneCamera.RenderWithShader(ForwardShader, "RenderType");
        }
        else if (quality.mode == GlobalQualitySetting.Mode.Merge)
        {//光照和阴影前面算 其他后处理 后面算
            //按照材质本身shader渲染 场景中同时存在PBR和Lambert，如果要规范化就用perfect的MRt渲染 给不同材质不同shader定义不同的rendertype去分开计算光照模型
            Matrix4x4 MainViewMatrix = Matrix4x4.identity;
            ViewMatrix(GetComponent<Camera>(), ref MainViewMatrix);
            Vector4 MainLightDir_WorldSpace = shadowCameraNear.transform.TransformDirection(Vector3.forward);

            Vector4 MainLightDir = MainViewMatrix.MultiplyVector(MainLightDir_WorldSpace).normalized;
            MainLightDir.w = SceneRenderSetting._Setting.MainLightBias.x;


            Shader.SetGlobalTexture("_Depth", GetGbuffer());
            Shader.SetGlobalVector("lightdir", MainLightDir);
            Shader.SetGlobalVector("lightcolor", SceneRenderSetting._Setting.MainLightColor * SceneRenderSetting._Setting.MainLightColorScale);// * shadowCamera.orthographicSize*0.1f);
            Shader.SetGlobalVector("ambientcolor", SceneRenderSetting._Setting.AmbientColor * SceneRenderSetting._Setting.AmbientColorScale);
            Cubemap env_spec = SceneRenderSetting._Setting.SkySpecular as Cubemap;
            if (env_spec == null)
            {
                env_spec = defaultSpecularCube;
            }
            Shader.SetGlobalTexture("_EnvCube", env_spec);

            RenderTexture depthbuffer = GetGbuffer();

            cloneCamera.clearFlags = CameraClearFlags.SolidColor;
            string[] name = { "Shadow_Off", "Shadow_Low", "Shadow_Mid", "Shadow_High", "Shadow_High" };
             for(int i = 0; i < name.Length; i++)
            {
                if(i== quality.ShadowLevel||i==4)
                    Shader.EnableKeyword(name[i]);
                else
                    Shader.DisableKeyword(name[i]);
            }

            if (bMRT)
            {
                
                renderbuffs[0] = cbuffer.colorBuffer;
                renderbuffs[1] = depthbuffer.colorBuffer;
                cloneCamera.SetTargetBuffers(renderbuffs, cbuffer.depthBuffer);
                cloneCamera.Render();//无法支持MRT，到时候估计要分layer渲染
            }
            else
            {
                cloneCamera.SetTargetBuffers(cbuffer.colorBuffer, cbuffer.depthBuffer);
                cloneCamera.Render();
                cloneCamera.SetTargetBuffers(depthbuffer.colorBuffer, cbuffer.depthBuffer);
                cloneCamera.RenderWithShader(gbuffer, "RenderType");//不过最好还是统一渲染MRT
            }
            //cloneCamera.clearFlags = CameraClearFlags.Nothing;
            // cloneCamera.SetTargetBuffers(cbuffer.colorBuffer, cbuffer.depthBuffer);
            // cloneCamera.RenderWithShader(PBRShader, "RenderType");颜色渲染放在相机自己里边
        }
        else
        {
            RenderTexture depthbuffer = GetGbuffer();
            if (bMRT)
            {//MRT里边可以根据不同rendertype设置不同的光照模型，后续增加
                cloneCamera.clearFlags = CameraClearFlags.SolidColor;
                renderbuffs[0] = cbuffer.colorBuffer;
                renderbuffs[1] = depthbuffer.colorBuffer;
                cloneCamera.SetTargetBuffers(renderbuffs, cbuffer.depthBuffer);
                //cloneCamera.SetTargetBuffers(new RenderBuffer[] { cbuffer.colorBuffer,depthbuffer.colorBuffer}, cbuffer.depthBuffer);
                cloneCamera.RenderWithShader(mrtShader, "RenderType");
            }
            else
            {

                //cbuffer.DiscardContents(true, true);

                cloneCamera.clearFlags = CameraClearFlags.SolidColor;
                cloneCamera.SetTargetBuffers(depthbuffer.colorBuffer, cbuffer.depthBuffer);
                cloneCamera.RenderWithShader(gbuffer, "RenderType");
                //Graphics.SetRenderTarget(cbuffer);
               // GL.Clear(false, true, Color.white);
                cloneCamera.clearFlags = CameraClearFlags.Nothing;
                cloneCamera.SetTargetBuffers(cbuffer.colorBuffer, cbuffer.depthBuffer);
                cloneCamera.RenderWithShader(diffusebuffershader, "RenderType");
            }
        }
        Shader.SetGlobalVector("_AAOffset", Vector4.zero);

        Shader.SetGlobalTexture("_OIT2X2Tex", GetOIT2X2Texture());


        //if (id == 1  || SceneRenderSetting._Setting.DebugShadow)
        //{
        //    shadowCameraFar.transform.rotation = qRotate;
        //    shadowCameraFar.transform.position = FixPosition(shadowCameraFar, far, width);
        //}
        //Vector3 farPos = FixPosition(shadowCameraSuperFar, superfar, width);
        //if (qRotate != shadowCameraSuperFar.transform.rotation || (farPos- shadowCameraSuperFar.transform.position).magnitude > 4.0f)
        //{
        //    shadowCameraSuperFar.transform.rotation = qRotate;
        //    shadowCameraSuperFar.transform.position = farPos;
        //    render_far_camera = true;
        //}

	}


    void CopySelfIllum(RenderTexture diffuse)
    {

        mat_SelfIllum.SetTexture("_MainTex", diffuse);
        mat_SelfIllum.SetPass(0);
        //Vector3 pos = Vector3.zero;
        Graphics.DrawMeshNow(Quad, Vector3.zero, Quaternion.identity, 0);
    }
    //void UpdateDeferredLight()
    //{
    //    for (int i = 0; i < DeferredLight.LightList.Count; i++)
    //    {
    //        DeferredLight l = DeferredLight.LightList[i];
    //
    //        {
    //            l.RenderDepth(DeferredLightCamera, CubeShadowDepthShader);
    //        }
    //    }
    //}
    void DrawSSAOMergeMode(RenderBuffer cbuffer, RenderBuffer dbuffer, Vector4 invViewport, Vector4 _FarCorner, RenderTexture diffuse, RenderBuffer colorBuffer, RenderBuffer depthBuffer, int pass, RenderTexture cb)
    {
        int w = quality.CurrentWidth;// / 2;
        int h = quality.CurrentHeight;/// 2;

        //Vector4 temp = new Vector4(1.0f/(float)w, 1.0f / (float)h,SceneRenderSetting._Setting.SSAOSampleRadius,0);
        temp.x = 1.0f / (float)w;
        temp.y = 1.0f / (float)h;
        temp.z = SceneRenderSetting._Setting.SSAOSampleRadius;
        temp.w = 0;
        //RenderTexture rt0 = RenderTexture.GetTemporary(w, h,0);
        //Graphics.SetRenderTarget(rt0);
        //GL.Clear(false, true, new Color(0, 0, 0, 0));
        Camera MainCamera = GetComponent<Camera>();

        temp.w = MainCamera.farClipPlane;
        mat_SSAO2.SetVector("invViewport_Radius", temp);
        mat_SSAO2.SetTexture("_Sample2x2", rotate4x4);
        mat_SSAO2.SetTexture("_Depth", gbuffer_tex);
        mat_SSAO2.SetTexture("_Diffuse", diffuse);
        Cubemap sky_specular = SceneRenderSetting._Setting.AmbientDiffuseCube as Cubemap;
        if (sky_specular == null)
        {
            sky_specular = defaultDiffuseCube;
        }
        mat_SSAO2.SetTexture("_SkyTexture", sky_specular);
        mat_SSAO2.SetVector("_SkyColor", SceneRenderSetting._Setting.AmbientColor * SceneRenderSetting._Setting.AmbientColorScale);
        mat_SSAO2.SetMatrix("ViewToWorld", MainCamera.transform.localToWorldMatrix);

        //mat_SSAO.SetTexture("_IBL", IBL_Texture);
        //mat_SSAO.SetTexture("_LUT", LUT_Texture);
        mat_SSAO2.SetVector("_AmbientColor", SceneRenderSetting._Setting.AmbientColor * SceneRenderSetting._Setting.AmbientColorScale);
        Vector3 worldup = transform.InverseTransformDirection(Vector3.up);
        mat_SSAO2.SetVector("_WorldUp", worldup);
        mat_SSAO2.SetVector("_FarCorner", _FarCorner);
        //mat_SSAO2.SetTexture("_NoiseTex", random256);
        mat_SSAO2.SetVector("_Params1", new Vector4(rotate4x4 == null ? 0f : rotate4x4.width, SceneRenderSetting._Setting.SSAOSampleRadius, 0, 0));
       

        mat_SSAO2.SetFloat("_Debug", 1);
       
        mat_SSAO2.SetTexture("_SSAO", cb);
        mat_SSAO2.SetPass(pass);
        Vector3 pos = Vector3.zero;
        Graphics.DrawMeshNow(Quad, Vector3.zero, Quaternion.identity, 0);
    }
    void DrawSSAO2(RenderBuffer cbuffer,RenderBuffer dbuffer,Vector4 invViewport, Vector4 _FarCorner, RenderTexture diffuse, RenderBuffer colorBuffer, RenderBuffer depthBuffer, int pass)
	{
		int w = quality.CurrentWidth;// / 2;
		int h = quality.CurrentHeight;/// 2;

		//Vector4 temp = new Vector4(1.0f/(float)w, 1.0f / (float)h,SceneRenderSetting._Setting.SSAOSampleRadius,0);
		temp.x = 1.0f / (float)w;
		temp.y = 1.0f / (float)h;
		temp.z = SceneRenderSetting._Setting.SSAOSampleRadius;
		temp.w = 0;
		//RenderTexture rt0 = RenderTexture.GetTemporary(w, h,0);
		//Graphics.SetRenderTarget(rt0);
		//GL.Clear(false, true, new Color(0, 0, 0, 0));
		Camera MainCamera = GetComponent<Camera>();

		temp.w = MainCamera.farClipPlane;
		mat_SSAO2.SetVector("invViewport_Radius", temp);
		mat_SSAO2.SetTexture("_Sample2x2", rotate4x4);
		mat_SSAO2.SetTexture("_Depth", gbuffer_tex);
		mat_SSAO2.SetTexture("_Diffuse", diffuse);
		Cubemap sky_specular = SceneRenderSetting._Setting.AmbientDiffuseCube as Cubemap;
		if (sky_specular == null)
		{
			sky_specular = defaultDiffuseCube;
		}
		mat_SSAO2.SetTexture("_SkyTexture", sky_specular);
		mat_SSAO2.SetVector("_SkyColor", SceneRenderSetting._Setting.AmbientColor * SceneRenderSetting._Setting.AmbientColorScale);
		mat_SSAO2.SetMatrix("ViewToWorld", MainCamera.transform.localToWorldMatrix);

		//mat_SSAO.SetTexture("_IBL", IBL_Texture);
		//mat_SSAO.SetTexture("_LUT", LUT_Texture);
		mat_SSAO2.SetVector("_AmbientColor", SceneRenderSetting._Setting.AmbientColor * SceneRenderSetting._Setting.AmbientColorScale);
		Vector3 worldup = transform.InverseTransformDirection(Vector3.up);
		mat_SSAO2.SetVector("_WorldUp", worldup);
		mat_SSAO2.SetVector("_FarCorner", _FarCorner);
		//mat_SSAO2.SetTexture("_NoiseTex", random256);
		mat_SSAO2.SetVector("_Params1", new Vector4(rotate4x4 == null ? 0f :rotate4x4.width, SceneRenderSetting._Setting.SSAOSampleRadius,0,0));
		if (SceneRenderSetting._Setting.EnableSSAODebug)
        {	
			mat_SSAO2.SetFloat ("_Debug", 1);
		}
        else {
			mat_SSAO2.SetFloat ("_Debug", 0);
		}
		mat_SSAO2.SetPass(pass);
		Vector3 pos = Vector3.zero;
		Graphics.DrawMeshNow(Quad, Vector3.zero, Quaternion.identity, 0);
	}
    Vector4 temp = Vector4.zero;
    void DrawSSAO(RenderBuffer cbuffer,RenderBuffer dbuffer,Vector4 invViewport, Vector4 _FarCorner, RenderTexture diffuse, RenderBuffer colorBuffer, RenderBuffer depthBuffer, int pass)
    {


        int w = quality.CurrentWidth;// / 2;
        int h = quality.CurrentHeight;/// 2;
        if (CheckerBoard_Enable)
        {
            w /= 2;
            h /= 2;
        }
        //Vector4 temp = new Vector4(1.0f/(float)w, 1.0f / (float)h,SceneRenderSetting._Setting.SSAOSampleRadius,0);
        temp.x = 1.0f / (float)w;
        temp.y = 1.0f / (float)h;
        temp.z = SceneRenderSetting._Setting.SSAOSampleRadius;
        temp.w = 0;
        //RenderTexture rt0 = RenderTexture.GetTemporary(w, h,0);
        //Graphics.SetRenderTarget(rt0);
        //GL.Clear(false, true, new Color(0, 0, 0, 0));
        Camera MainCamera = GetComponent<Camera>();

        temp.w = MainCamera.farClipPlane;
        mat_SSAO.SetVector("invViewport_Radius", temp);
        mat_SSAO.SetTexture("_Sample2x2", rotate4x4);
        mat_SSAO.SetTexture("_Depth", gbuffer_tex);
        mat_SSAO.SetTexture("_Diffuse", diffuse);
        Cubemap sky_specular = SceneRenderSetting._Setting.AmbientDiffuseCube as Cubemap;
        if (sky_specular == null)
        {
            sky_specular = defaultDiffuseCube;
        }
        mat_SSAO.SetTexture("_SkyTexture", sky_specular);
        mat_SSAO.SetVector("_SkyColor", SceneRenderSetting._Setting.AmbientColor * SceneRenderSetting._Setting.AmbientColorScale);
        mat_SSAO.SetMatrix("ViewToWorld", MainCamera.transform.localToWorldMatrix);

        //mat_SSAO.SetTexture("_IBL", IBL_Texture);
        //mat_SSAO.SetTexture("_LUT", LUT_Texture);
        mat_SSAO.SetVector("_AmbientColor", SceneRenderSetting._Setting.AmbientColor * SceneRenderSetting._Setting.AmbientColorScale);
        Vector3 worldup = transform.InverseTransformDirection(Vector3.up);
        mat_SSAO.SetVector("_WorldUp", worldup);
        mat_SSAO.SetVector("_FarCorner", _FarCorner);
        
		if (SceneRenderSetting._Setting.EnableSSAODebug) 
		{		
			mat_SSAO.SetPass(pass);
			Vector3 pos = Vector3.zero;
			Graphics.DrawMeshNow(Quad, Vector3.zero, Quaternion.identity, 0);
			return;
		}

		if (CheckerBoard_Enable)
		{

			Graphics.SetRenderTarget(CheckerBoard_Texture);
			if (IsMoving)
			{
				mat_SSAO.SetPass(8);
			}
			else
			{
				int n = Time.frameCount & 1;
				mat_SSAO.SetPass(5 + n);
			}
			Vector3 pos = Vector3.zero;
			Graphics.DrawMeshNow(Quad, Vector3.zero, Quaternion.identity, 0);
			mat_SSAO.SetTexture("_AO", CheckerBoard_Texture);
			Graphics.SetRenderTarget(cbuffer,dbuffer);
			mat_SSAO.SetPass(7);
			Graphics.DrawMeshNow(Quad, Vector3.zero, Quaternion.identity, 0);
		}
		else
		{
			mat_SSAO.SetPass(pass);
			Vector3 pos = Vector3.zero;
			Graphics.DrawMeshNow(Quad, Vector3.zero, Quaternion.identity, 0);
		}
        


    }
    void DrawSSAO_Off(Vector4 invViewport, Vector4 _FarCorner, RenderTexture diffuse, int pass)
    {

        Vector4 temp = invViewport;
        temp.w = GetComponent<Camera>().farClipPlane;
        mat_SSAO.SetVector("invViewport_Radius", temp);
        mat_SSAO.SetTexture("_Sample2x2", Get2x2RotateTexture());
        mat_SSAO.SetTexture("_Depth", gbuffer_tex);
        mat_SSAO.SetTexture("_Diffuse", diffuse);

        //mat_SSAO.SetTexture("_IBL", IBL_Texture);
        //mat_SSAO.SetTexture("_LUT", LUT_Texture);
        mat_SSAO.SetVector("_AmbientColor", SceneRenderSetting._Setting.AmbientColor * SceneRenderSetting._Setting.AmbientColorScale*0.5f);
        Vector3 worldup = transform.InverseTransformDirection(Vector3.up);
        mat_SSAO.SetVector("_WorldUp", worldup);
        mat_SSAO.SetVector("_FarCorner", _FarCorner);
        //Graphics.Blit(null, target, mat_SSAO, 0);
        mat_SSAO.SetPass(pass);
        Vector3 pos = Vector3.zero;
        Graphics.DrawMeshNow(Quad, Vector3.zero, Quaternion.identity, 0);

    }

    static Matrix4x4 GetFrustumMatrix(Camera cam)
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
        Matrix4x4 shadowcam_proj_unity = GL.GetGPUProjectionMatrix(cam.projectionMatrix, true);
        Vector3 vscale = new Vector3(1, 1, -1);
        if (!Application.isEditor)
        {
            vscale.y = -1;
        }

        Matrix4x4 mattest2 = Matrix4x4.identity;
        mattest2.SetTRS(Vector3.zero, Quaternion.identity, vscale);
        //
        return cam.transform.localToWorldMatrix * mattest2 * shadowcam_proj_unity.inverse * mattest;
    }
    void DrawShadow(Matrix4x4 MainViewMatrix, Vector4 farcorner, Vector4 invViewport, RenderTexture diffuse, Camera shadowCamera, float bias, int Pass)
    {
        if (shadow_depth0 == null)
        {
            return;
        }
        if (Application.isEditor)
        {
            farcorner.w = -1;
        }
        else
        {
            farcorner.w = 1.0f;
        }
        //shadow

        //Matrix4x4 maincam_World = Matrix4x4.identity;
        //ViewMatrix(GetComponent<Camera>(), ref maincam_World);
        //Matrix4x4 shadow_cam_World = Matrix4x4.identity;
        //ViewMatrix(shadowCamera, ref shadow_cam_World);
        //Matrix4x4 main_shadow = maincam_World.inverse;
        //Matrix4x4 mainview_shadowview = shadow_cam_World * main_shadow;//
        ////Matrix4x4 shadowcam_proj = Matrix4x4.identity;
        ////ProjMatrix(shadowCamera, ref shadowcam_proj);
        //Matrix4x4 shadowcam_proj_unity = GL.GetGPUProjectionMatrix(shadowCamera.projectionMatrix, true);
        //if (!Application.isEditor)
        //{
        //    shadowcam_proj_unity.m11 *= -1;
        //
        //}
        //Matrix4x4 shadow_cam_WorldToView = shadow_cam_ViewToWorld.inverse;
        //Vector3 wpos = main_shadow*(new Vector3(0, 0, 12));
        // *shadowCamera.worldToCameraMatrix * GL.GetGPUProjectionMatrix(shadowCamera.projectionMatrix);

        FilterMode old_filtermode = gbuffer_tex.filterMode;
        gbuffer_tex.filterMode = FilterMode.Point;
        // shadowCamera.targetTexture.filterMode = FilterMode.Point;
        //mat_shadow_gen.SetTexture("_ShadowDepth", shadowCamera.targetTexture);
        mat_shadow_gen.SetTexture("_Depth", gbuffer_tex);
        mat_shadow_gen.SetTexture("_Sample2x2", rotate4x4);
        mat_shadow_gen.SetColor("_AmbientColor", Color.white*SceneRenderSetting._Setting.AmbientColorScale);



        //Vector3 lightpos = MainViewMatrix.MultiplyPoint(shadowCamera.transform.position);
        //Vector3 lightdir = MainViewMatrix.MultiplyVector(shadowCamera.transform.TransformDirection(Vector3.forward));
        ////mat_shadow_gen.SetVector("lightpos", lightpos);
        //Vector4 light_dir = lightdir;
        //light_dir.w = bias;



        diffuse.filterMode = FilterMode.Point;
        mat_shadow_gen.SetTexture("_Diffuse", diffuse);

        mat_shadow_gen.SetVector("farCorner", farcorner);
        Vector4 v_shadow = new Vector4(1.0f / shadow_depth0.width, 1.0f / shadow_depth0.height, SceneRenderSetting._Setting.MainLightShadowSampleRadius, shadowCamera.farClipPlane);
        mat_shadow_gen.SetVector("invViewport_Radius", v_shadow);
        mat_shadow_gen.SetVector("mainInvViewport", invViewport);
        Cubemap env_spec = SceneRenderSetting._Setting.SkySpecular as Cubemap;
        if(env_spec==null)
        {
            env_spec = defaultSpecularCube;
        }
        mat_shadow_gen.SetTexture("_EnvCube", env_spec);
        //mat_shadow_gen.SetVector("ambientcolor", SceneRenderSetting._Setting.AmbientColor * SceneRenderSetting._Setting.AmbientColorScale);

        //Graphics.Blit(null, target, mat_shadow_gen, 0);
        if (Pass >= 0 && Pass <= 3)
        {
            mat_shadow_gen.SetPass(Pass);
            Matrix4x4 matFrustum = GetFrustumMatrix(shadowCamera);

            Graphics.DrawMeshNow(Cube, matFrustum);
        }
        else
        {
            mat_shadow_gen.SetPass(Pass);
            Graphics.DrawMeshNow(Quad, Matrix4x4.identity, 0);
        }

        gbuffer_tex.filterMode = old_filtermode;
        diffuse.filterMode = FilterMode.Bilinear;
    }
    void DrawShadowCheckerBoard(RenderBuffer cbuffer,RenderBuffer dbuffer,Matrix4x4 MainViewMatrix, Vector4 farcorner, Vector4 invViewport, RenderTexture diffuse, Camera shadowCamera, float bias, int Pass)
    {
        if (shadow_depth0 == null)
        {
            return;
        }
        if (Application.isEditor)
        {
            farcorner.w = -1;
        }
        else
        {
            farcorner.w = 1.0f;
        }

        FilterMode old_filtermode = gbuffer_tex.filterMode;
        gbuffer_tex.filterMode = FilterMode.Point;

        mat_shadow_gen.SetTexture("_Depth", gbuffer_tex);
        mat_shadow_gen.SetTexture("_Sample2x2", rotate4x4);
        mat_shadow_gen.SetColor("_AmbientColor", Color.white * SceneRenderSetting._Setting.AmbientColorScale);


        diffuse.filterMode = FilterMode.Point;
        mat_shadow_gen.SetTexture("_Diffuse", diffuse);

        mat_shadow_gen.SetVector("farCorner", farcorner);
        Vector4 v_shadow = new Vector4(1.0f / shadow_depth0.width, 1.0f / shadow_depth0.height, SceneRenderSetting._Setting.MainLightShadowSampleRadius, shadowCamera.farClipPlane);
        mat_shadow_gen.SetVector("invViewport_Radius", v_shadow);
        Vector4 local_InvViewport = invViewport;
        local_InvViewport.x *= 2.0f;
        local_InvViewport.y *= 2.0f;
        mat_shadow_gen.SetVector("mainInvViewport", local_InvViewport);
        Cubemap env_spec = SceneRenderSetting._Setting.SkySpecular as Cubemap;
        if (env_spec == null)
        {
            env_spec = defaultSpecularCube;
        }
        mat_shadow_gen.SetTexture("_EnvCube", env_spec);

        Graphics.SetRenderTarget(CheckerBoard_Texture);
        int n = Time.frameCount & 1;
        if(!IsMoving)
        {
            if (n == 0)
            {
                //clear shadow mask 0
                //mat_shadow_gen.SetPass(5);
                //Graphics.DrawMeshNow(Quad, Matrix4x4.identity, 0);
                //calc shadow mask 0
                mat_shadow_gen.SetPass(8 + Pass);
                Graphics.DrawMeshNow(Quad, Matrix4x4.identity, 0);
            }
            else
            {
                //clear shadow mask 1
                //mat_shadow_gen.SetPass(6);
                //Graphics.DrawMeshNow(Quad, Matrix4x4.identity, 0);
                //calc shadow mask 1
                mat_shadow_gen.SetPass(12 + Pass);
                Graphics.DrawMeshNow(Quad, Matrix4x4.identity, 0);
            }
        }
        else
        {
            //mat_shadow_gen.SetPass(7);
            //Graphics.DrawMeshNow(Quad, Matrix4x4.identity, 0);
            mat_shadow_gen.SetPass(16 + Pass);
            Graphics.DrawMeshNow(Quad, Matrix4x4.identity, 0);
        }
        mat_shadow_gen.SetVector("mainInvViewport", invViewport);
        //Combine Shadow Mask
        Graphics.SetRenderTarget(cbuffer, dbuffer);
        mat_shadow_gen.SetTexture("_ShadowMask", CheckerBoard_Texture);
        mat_shadow_gen.SetPass(20);
        Graphics.DrawMeshNow(Quad, Matrix4x4.identity, 0);


        gbuffer_tex.filterMode = old_filtermode;
        diffuse.filterMode = FilterMode.Bilinear;
    }
    void DrawBackBuffer(Vector4 invViewport, RenderTexture diffuse, RenderTexture lighting, RenderTexture target)
    {

        //copy to backbuffer
        mat_combine.SetVector("invViewport_Radius", invViewport);
        mat_combine.SetTexture("_Diffuse", diffuse);
        mat_combine.SetColor("ambientColor", SceneRenderSetting._Setting.AmbientColor * SceneRenderSetting._Setting.AmbientColorScale);
        mat_combine.SetColor("lightColor", SceneRenderSetting._Setting.MainLightColor * SceneRenderSetting._Setting.MainLightColorScale);
        //mat.SetTexture("_Depth", tex);
        Graphics.Blit(lighting, target, mat_combine, 0);
    }

    Vector4 ProjVector = Vector4.zero;
    void DrawSSR(Vector4 invViewport, Vector4 _FarCorner, RenderTexture finalDiffuse, Texture _Specular, Texture _Diffuse)
    {

        Matrix4x4 proj_matrix = GL.GetGPUProjectionMatrix(GetComponent<Camera>().projectionMatrix, true);


        //Vector4 ProjVector = new Vector4(Mathf.Abs(proj_matrix.m00), Mathf.Abs(proj_matrix.m11), Mathf.Abs(proj_matrix.m22), Mathf.Abs(proj_matrix.m32));
        ProjVector.x = Mathf.Abs(proj_matrix.m00);
        ProjVector.y = Mathf.Abs(proj_matrix.m11);
        ProjVector.z = Mathf.Abs(proj_matrix.m22);
        ProjVector.w = Mathf.Abs(proj_matrix.m32);
        //if (!Application.isEditor)
        //{
        //    ProjVector.w *= 0.5f;
        //}
        mat_SSR.SetTexture("_FinalDiffuse", finalDiffuse);
        mat_SSR.SetTexture("_MainTex", gbuffer_tex);
        mat_SSR.SetTexture("_Specular", _Specular);
        mat_SSR.SetTexture("_Diffuse", _Diffuse);
        mat_SSR.SetVector("_FarCorner", _FarCorner);
        mat_SSR.SetTexture("_Sample2x2", GetRandomTex());
        mat_SSR.SetVector("ProjVector", ProjVector);
        mat_SSR.SetMatrix("ViewToWorld", transform.localToWorldMatrix);
        mat_SSR.SetVector("AmbientColor", SceneRenderSetting._Setting.AmbientColor * SceneRenderSetting._Setting.AmbientColorScale);
        Vector4 temp = invViewport;
        temp.w = 0;
        mat_SSR.SetVector("invViewport", temp);
        mat_SSR.SetVector("ReflectColor", Color.white * SceneRenderSetting._Setting.ReflectIntensity);
        mat_SSR.SetPass(0);
        // Vector3 pos = new Vector3(-6.29f, 1.94f, -12.75f);
        Graphics.DrawMeshNow(Quad, Vector3.zero, Quaternion.identity, 0);

    }
    void GenNormal(RenderTexture target, Vector4 farCorner, Vector4 invViewport)
    {
        mat_Normal_Gen.SetVector("farCorner", farCorner);
        mat_Normal_Gen.SetVector("invViewport_Radius", invViewport);
        mat_Normal_Gen.SetTexture("_Depth", gbuffer_tex);
        Graphics.Blit(null, target, mat_Normal_Gen, 0);
    }
    void BlendFrame(RenderTexture lastFrame, RenderTexture _CurrentFrame, RenderTexture target, Vector4 invViewport)
    {
        mat_BlendFrame.SetVector("invViewport_Radius", invViewport);
        mat_BlendFrame.SetTexture("_LastFrame", lastFrame);
        mat_BlendFrame.SetTexture("_CurrentFrame", _CurrentFrame);
        Graphics.Blit(null, target, mat_BlendFrame, 0);
    }
    void CopyBuffer(RenderTexture source, RenderTexture target, Vector4 invViewport)
    {
        mat_copy.SetVector("invViewport_Radius", invViewport);
        Graphics.Blit(source, target, mat_copy, 0);
    }
    void BlendBuffer(RenderTexture source, RenderTexture target, Vector4 invViewport)
    {
        Vector4 temp = invViewport;
        temp.w = 0.5f;
        mat_copy.SetVector("invViewport_Radius", temp);
        Graphics.Blit(source, target, mat_copy, 0);
    }

    Vector4 ProjVector_GI = Vector4.zero;
    void DrawGI(Vector4 invViewport, Vector4 _FarCorner, RenderTexture _Diffuse, RenderTexture lastFrame, RenderTexture target)
    {
        Matrix4x4 proj_matrix = GL.GetGPUProjectionMatrix(GetComponent<Camera>().projectionMatrix, true);


        //Vector4 ProjVector = new Vector4(Mathf.Abs(proj_matrix.m00), Mathf.Abs(proj_matrix.m11), Mathf.Abs(proj_matrix.m22), Mathf.Abs(proj_matrix.m23));
        ProjVector_GI.x = Mathf.Abs(proj_matrix.m00);
        ProjVector_GI.y = Mathf.Abs(proj_matrix.m11);
        ProjVector_GI.z = Mathf.Abs(proj_matrix.m22);
        ProjVector_GI.w = Mathf.Abs(proj_matrix.m23);
        if (!Application.isEditor)
        {
            ProjVector_GI.w *= 0.5f;
        }
        mat_GI.SetTexture("_Depth", gbuffer_tex);
        mat_GI.SetTexture("_FinalResult", lastFrame);
        mat_GI.SetTexture("_RandomTex", GetRandomTex());
        mat_GI.SetTexture("_RandomTex2x2", Get2x2StepTexture());
        mat_GI.SetTexture("_Diffuse", _Diffuse);
        mat_GI.SetVector("farCorner", _FarCorner);
        Vector4 temp = invViewport;
        temp.z = SceneRenderSetting._Setting.GI_Scale;
        temp.w = 0;// -Mathf.Floor(Time.time);
        mat_GI.SetVector("invViewport", temp);
        mat_GI.SetVector("ProjVector", ProjVector_GI);

        Graphics.Blit(null, target, mat_GI, 0);
    }
    void BlurGI(Vector4 invViewport, RenderTexture gi_tex, RenderTexture target)
    {
        Vector4 temp = invViewport;
        temp.z = SceneRenderSetting._Setting.GI_Scale;
        temp.w = Time.unscaledTime;// -Mathf.Floor(Time.time);

        mat_GI_Blur.SetVector("invViewport_Radius", temp);
        mat_GI_Blur.SetTexture("_MainTex", gi_tex);

        mat_GI_Blur.SetTexture("_Sample2x2", Get2x2RotateTexture());
        Graphics.Blit(null, target, mat_GI_Blur, 0);
    }
    void AddToTarget(Vector4 invViewport, RenderTexture gi_data, RenderTexture _Diffuse, RenderTexture target, RenderTexture ao)
    {

        mat_AddToTarget.SetVector("invViewport_Radius", invViewport);
        mat_AddToTarget.SetTexture("_Diffuse", _Diffuse);
        mat_AddToTarget.SetTexture("_MainTex", gi_data);
        Graphics.Blit(null, target, mat_AddToTarget, 0);
    }
    void DrawPlaneFog(Vector4 farCorner, Vector4 invViewport, RenderTexture target)
    {

        matPlaneFog.SetVector("farCorner", farCorner);

        matPlaneFog.SetTexture("_Depth", gbuffer_tex);
        matPlaneFog.SetTexture("_Noice", SceneRenderSetting._Setting.FogPlane_DynamicNoise);
        matPlaneFog.SetMatrix("ViewToWorld", transform.localToWorldMatrix);
        matPlaneFog.SetVector("_PlaneFogParam", SceneRenderSetting._Setting.PlaneFogParam);
        Vector3 viewspeed = GetComponent<Camera>().transform.InverseTransformDirection(SceneRenderSetting._Setting.PlaneFogSpeed);//.InverseTransformDirection(SceneRenderSetting._Setting.PlaneFogSpeed); ;//worldspeed
        matPlaneFog.SetVector("PlaneFogSpeed", viewspeed);
        
        matPlaneFog.SetVector("FogColor", SceneRenderSetting._Setting.PlaneFogColor);
        matPlaneFog.SetPass(0);
        Graphics.DrawMeshNow(Quad, Vector3.zero, Quaternion.identity, 0);
    }
    void DrawFog(Vector4 farCorner, Vector4 invViewport, RenderTexture target)
    {
        if (SceneRenderSetting._Setting.Fog_Attenuation < 0)
        {
            SceneRenderSetting._Setting.Fog_Attenuation = 0.0f;
        }

        Vector4 view_up = GetComponent<Camera>().transform.InverseTransformDirection(Vector3.up);
        view_up.w = SceneRenderSetting._Setting.Fog_Height - GetComponent<Camera>().transform.position.y;
        mat_Fog.SetVector("_ViewUp", view_up);
        mat_Fog.SetVector("farCorner", farCorner);
        mat_Fog.SetVector("FogColor", SceneRenderSetting._Setting.Fog_Color);
        mat_Fog.SetVector("invViewport_Radius", invViewport);
        mat_Fog.SetVector("FogDistance", new Vector4(SceneRenderSetting._Setting.Fog_Start, SceneRenderSetting._Setting.Fog_End, SceneRenderSetting._Setting.Fog_Attenuation, SceneRenderSetting._Setting.Fog_DensityMount));
        mat_Fog.SetTexture("_Depth", gbuffer_tex);
        mat_Fog.SetFloat("_Speed", SceneRenderSetting._Setting.Fog_Speed);
        mat_Fog.SetTexture("_Noice", SceneRenderSetting._Setting.Fog_DynamicNoise);
        mat_Fog.SetMatrix("ViewToWorld", transform.localToWorldMatrix);

        if (SceneRenderSetting._Setting.Fog_Sky)
        {
            if (SceneRenderSetting._Setting.Fog_DynamicNoise == null)
                mat_Fog.SetPass(0);
            else
                mat_Fog.SetPass(2);
        }
        else
        {
            if (SceneRenderSetting._Setting.Fog_DynamicNoise == null)
                mat_Fog.SetPass(1);
            else
                mat_Fog.SetPass(3);
        }
        Graphics.DrawMeshNow(Quad, Vector3.zero, Quaternion.identity, 0);
    }
    void DrawMipFog(Vector4 farCorner, Vector4 invViewport, RenderTexture target)
    {
        MipFogMaterial.SetMatrix("ViewToWorld", transform.localToWorldMatrix);
        MipFogMaterial.SetVector("farCorner", farCorner);
        MipFogMaterial.SetVector("FogColor", SceneRenderSetting._Setting.Fog_Color);
        MipFogMaterial.SetVector("invViewport_Radius", invViewport);
        MipFogMaterial.SetVector("FogDistance", new Vector4(SceneRenderSetting._Setting.Fog_Start, SceneRenderSetting._Setting.Fog_End, SceneRenderSetting._Setting.Fog_Attenuation, 0));
        MipFogMaterial.SetTexture("_Depth", gbuffer_tex);
        MipFogMaterial.SetTexture("_SkyTexture", SceneRenderSetting._Setting.MipFogTexture);
        MipFogMaterial.SetVector("_AmbientColor", SceneRenderSetting._Setting.AmbientColor * SceneRenderSetting._Setting.AmbientColorScale);
        MipFogMaterial.SetPass(0);
        //mat_Fog.SetTexture("_MainTex", FinalResult);
        //Graphics.Blit(null, target, MipFogMaterial, 0);
        Graphics.DrawMeshNow(Quad, Matrix4x4.identity);

    }
    //void DrawDOF(Vector4 farCorner, Vector4 invViewport, RenderTexture FinalResult, RenderTexture target)
    //{
    //    farCorner.w = SceneRenderSetting._Setting.DOF_Focus;
    //    mat_DOF.SetVector("farCorner", farCorner);
    //    mat_DOF.SetVector("invViewport_Radius", invViewport);
    //    mat_DOF.SetTexture("_Depth", gbuffer_tex);
    //    mat_DOF.SetTexture("_MainTex", FinalResult);
    //    mat_DOF.SetTexture("_Sample2x2", Get2x2RotateTexture());
    //    Graphics.Blit(null, target, mat_DOF, 0);
    //}
    void DrawSSSSS(Vector4 farCorner, Vector4 invViewport, RenderTexture FinalResult, RenderTexture _Diffuse)
    {
        Vector4 invVP = invViewport;
        invVP.z = SceneRenderSetting._Setting.SSS_Radius;
        mat_SSSSS.SetTexture("_Depth", gbuffer_tex);
        mat_SSSSS.SetTexture("_Sample2x2", rotate4x4);
        mat_SSSSS.SetTexture("_Diffuse", _Diffuse);
        mat_SSSSS.SetTexture("_Random", GetRandomTex());
        mat_SSSSS.SetTexture("_LastFrame", FinalResult);
        //iphone 阴影算法与android不一样 对SSS效果会有一定的影响 所以ios下面需要略微增强SSS强度...
        float scale = SceneRenderSetting._Setting.SSS_Color_Scale;
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            scale *= 1.175f;
        }
        mat_SSSSS.SetColor("_SSSColor", SceneRenderSetting._Setting.SSS_Color * scale);
        mat_SSSSS.SetVector("invViewport_Radius", invVP);
        mat_SSSSS.SetVector("_FarCorner", farCorner);

        mat_SSSSS.SetPass(0);
        Vector3 pos = Vector3.zero;
        Graphics.DrawMeshNow(Quad, Vector3.zero, Quaternion.identity, 0);
    }

    Vector4[] lpos = new Vector4[4] { Vector4.zero, Vector4.zero, Vector4.zero, Vector4.zero };
    Color[] lcolor = new Color[4] { Color.black, Color.black, Color.black, Color.black };

    int DrawActorHighLight(Vector4 farCorner, Vector4 invViewport, RenderTexture _Diffuse, int max_light_count)
    {


        Camera mainCamera = GetComponent<Camera>();
        Matrix4x4 maincam_World = Matrix4x4.identity;
        ViewMatrix(mainCamera, ref maincam_World);

        Vector3 camerapos = transform.position;



        // if light in visable in maincamera ,add to sort list
        // else if the distance  between maincamera and light less max distance,add to sort list
        // final sort the list,Find nearest lights by given count;
        int count = DeferredLight.FindSpecularLight(camerapos, max_light_count, 10.0f);

        int realcount = 0;
        for (int i = 0; i < count && i < 4; i++)
        {
            DeferredLight light = DeferredLight.NearestLightList[i];
            if (light == null)
            {
                continue;
            }
            realcount++;
            if (SceneRenderSetting._Setting.FullScreenRain > 0.0f)
            {
                lpos[i] = light.transform.position;
            }
            else
            {
                lpos[i] = maincam_World.MultiplyPoint(light.transform.position);
            }
            lpos[i].w = 1.0f;
            lcolor[i] = light.color * light.intensity;
        }

        if (realcount == 0)
        {
            return realcount;
        }

        Vector4 invVP = invViewport;
        invVP.z = SceneRenderSetting._Setting.ActorHightLight_Shiness;
        invVP.w = SceneRenderSetting._Setting.ActorHightLight_Scale;
        mat_ActorHighLight.SetTexture("_Depth", gbuffer_tex);
        mat_ActorHighLight.SetTexture("_Diffuse", _Diffuse);
        mat_ActorHighLight.SetTexture("_HairTex", _HairTex);

        mat_ActorHighLight.SetColor("_Color", Color.white);
        mat_ActorHighLight.SetVector("invViewport_Radius", invVP);
        mat_ActorHighLight.SetVector("_FarCorner", farCorner);

        if (SceneRenderSetting._Setting.FullScreenRain > 0.0f)
        {
            if (FullScreenRainMaterial != null)
            {
                FullScreenRainMaterial.SetVector("_LPos0", lpos[0]);
                FullScreenRainMaterial.SetVector("_LPos1", lpos[1]);
                FullScreenRainMaterial.SetVector("_LPos2", lpos[2]);
                FullScreenRainMaterial.SetVector("_LPos3", lpos[3]);
                //FullScreenRainMaterial.SetVector("_LPos4", lpos[4]);
                //FullScreenRainMaterial.SetVector("_LPos5", lpos[5]);

                FullScreenRainMaterial.SetVector("_LColor0", lcolor[0]);
                FullScreenRainMaterial.SetVector("_LColor1", lcolor[1]);
                FullScreenRainMaterial.SetVector("_LColor2", lcolor[2]);
                FullScreenRainMaterial.SetVector("_LColor3", lcolor[3]);
                //FullScreenRainMaterial.SetVector("_LColor4", lcolor[4]);
                //FullScreenRainMaterial.SetVector("_LColor5", lcolor[5]);
            }
        }
        else
        {
            mat_ActorHighLight.SetVector("_LPos0", lpos[0]);
            mat_ActorHighLight.SetVector("_LPos1", lpos[1]);
            mat_ActorHighLight.SetVector("_LPos2", lpos[2]);
            mat_ActorHighLight.SetVector("_LPos3", lpos[3]);
            //mat_ActorHighLight.SetVector("_LPos4", lpos[4]);
            //mat_ActorHighLight.SetVector("_LPos5", lpos[5]);

            mat_ActorHighLight.SetVector("_LColor0", lcolor[0]);
            mat_ActorHighLight.SetVector("_LColor1", lcolor[1]);
            mat_ActorHighLight.SetVector("_LColor2", lcolor[2]);
            mat_ActorHighLight.SetVector("_LColor3", lcolor[3]);
            //mat_ActorHighLight.SetVector("_LColor4", lcolor[4]);
            //mat_ActorHighLight.SetVector("_LColor5", lcolor[5]);

            mat_ActorHighLight.SetPass(realcount - 1);
            Graphics.DrawMeshNow(Quad, Vector3.zero, Quaternion.identity, 0);
            //mat_ActorHighLight.SetPass(1);
            //Graphics.DrawMeshNow(Quad, Vector3.zero, Quaternion.identity, 0);
        }

        return realcount;
    }
     
 
    void DrawDof(Vector4 invViewport, RenderTexture currentRenderTarget, RenderBuffer color_buffer, RenderBuffer depth_buffer, Vector4 _FarCorner, RenderTexture temp_RenderTarget)
    {
        int width = quality.CurrentWidth;// / 2;
        int height = quality.CurrentHeight;/// 2;


        //RenderTexture temp_HalfSize = RenderTexture.GetTemporary(width, height, 0);

        Graphics.SetRenderTarget(temp_RenderTarget.colorBuffer, depth_buffer);
        GL.Clear(false, true, new Color(0,0,0,0));
        Vector4 temp = invViewport;
        temp.x = 1.0f / (float)width;
        temp.y = 1.0f / (float)height;
        temp.z = SceneRenderSetting._Setting.Glow_BlurRadius;
        temp.w = 0;// SceneRenderSetting._Setting.Glow_Brightness;
        mat_downsample_glow.SetVector("invViewport", temp);
        mat_downsample_glow.SetTexture("_DepthTex", gbuffer_tex);
        mat_downsample_glow.SetTexture("_FinalTex", currentRenderTarget);
        Vector4 temp2 = _FarCorner;
        temp2.w = SceneRenderSetting._Setting.DOF_Focus;
        mat_downsample_glow.SetVector("_FarCorner", temp2);

        Camera cam = GetComponent<Camera>();
        float f = cam.farClipPlane;
        float n = cam.nearClipPlane;

        float focus = SceneRenderSetting._Setting.DOF_Focus;
        if (focus < n)
        {
            focus = n + 0.0001f;
        }
        if (focus > f)
        {
            focus = f;
        }

        float z = (f / (f - n) * focus - f * n / (f - n));
        float w = focus;
        float zdepth = z / w;
        mat_downsample_glow.SetFloat("_ZDepth", zdepth);
        //Graphics.Blit(FinalResult, temp_HalfSize, mat_downsample_glow, 0);

        mat_downsample_glow.SetPass(1);
        Graphics.DrawMeshNow(Quad, Matrix4x4.identity, 0);
        //blur x



        //if (v.x < 1)


        ////blur y
        //temp.x = 1.0f / (float)width;
        //temp.y = 1.0f / (float)height;
        //temp.z = Glow_BlurRadius;
        //mat_blur_glow.SetVector("invViewport", temp);
        //Graphics.Blit(temp_X_Blur, temp_HalfSize, mat_blur_glow, 0);

        Graphics.SetRenderTarget(color_buffer, depth_buffer);
        mat_downsample_glow.SetTexture("_FinalTex", temp_RenderTarget);
        if (!SceneRenderSetting._Setting.DOF_Without_Sky)
        {
            mat_downsample_glow.SetPass(0);
        }
        else
        {
            mat_downsample_glow.SetPass(2);
        }
        Graphics.DrawMeshNow(Quad, Matrix4x4.identity, 0);

        //RenderTexture.ReleaseTemporary(temp_HalfSize);

    }
    void DrawToneMapping(RenderTexture Source, RenderTexture Target)
    {
        int width = quality.CurrentWidth;
        int height = quality.CurrentHeight;

        Vector4 param = new Vector4();
        param.x = SceneRenderSetting._Setting.ToneMapping_Exposure;
        param.y = SceneRenderSetting._Setting.ToneMapping_gamma;
        param.z = SceneRenderSetting._Setting.ToneMapping_Saturation;
        param.w = SceneRenderSetting._Setting.ToneMapping_Contract;
        mat_ToneMapping.SetVector("tonemapping_param", param);
        if (SceneRenderSetting._Setting.EnableFXAA && quality.FXAALevel == 1)
        {
            RenderTexture tempTex = RenderTexture.GetTemporary(width, height, 0);
            Graphics.Blit(Source, tempTex, mat_ToneMapping, 0);
            DoFXAA(tempTex, Target);
            RenderTexture.ReleaseTemporary(tempTex);
        }
        else
        {
            Graphics.Blit(Source, Target, mat_ToneMapping, 0);
       }
    }
    void BlurShadowAO(Vector4 invViewport, Vector4 farcorner, RenderTexture _ShadowAO, RenderTexture _Diffuse, RenderTexture Target)
    {

        Matrix4x4 maincam_World = Matrix4x4.identity;
        ViewMatrix(GetComponent<Camera>(), ref maincam_World);
        Vector3 lightdir = maincam_World.MultiplyVector(shadowCameraNear.transform.TransformDirection(Vector3.forward));
        mat_ShadowMask.SetVector("invViewport_Radius", invViewport);
        mat_ShadowMask.SetVector("farCorner", farcorner);
        mat_ShadowMask.SetVector("lightdir", lightdir);
        mat_ShadowMask.SetColor("lightcolor", SceneRenderSetting._Setting.MainLightColor * SceneRenderSetting._Setting.MainLightColorScale);
        mat_ShadowMask.SetColor("ambientcolor", SceneRenderSetting._Setting.AmbientColor * SceneRenderSetting._Setting.AmbientColorScale);
        mat_ShadowMask.SetTexture("_Depth", gbuffer_tex);
        mat_ShadowMask.SetTexture("_ShadowAO", _ShadowAO);
        mat_ShadowMask.SetTexture("_Diffuse", _Diffuse);
        Graphics.Blit(null, Target, mat_ShadowMask, 0);
    }
    RenderTexture GetLastFinalTex()
    {
        int width = quality.CurrentWidth;
        int height = quality.CurrentHeight;

        if (final_tex == null)
        {
            final_tex = new RenderTexture(width, height, 0);
        }
        else
        {
            if (final_tex.width != width || final_tex.height != height)
            {
                if (Application.isPlaying)
                {
                    Object.Destroy(final_tex);
                }
                else
                {
                    Object.DestroyImmediate(final_tex);
                }
                final_tex = new RenderTexture(width, height, 0);
            }
        }
        return final_tex;
    }
    RenderTexture GetGbuffer()
    {
        int width = quality.CurrentWidth;
        int height = quality.CurrentHeight;

        if (gbuffer_tex == null)
        {
            gbuffer_tex = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
        }
        else
        {
            if (gbuffer_tex.width != width || gbuffer_tex.height != height)
            {
                if (Application.isPlaying)
                {
                    Object.Destroy(gbuffer_tex);
                }
                else
                {
                    Object.DestroyImmediate(gbuffer_tex);
                }
                gbuffer_tex = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
            }
        }
        return gbuffer_tex;
    }
    RenderTexture GetColorBuffer()
    {
        int width = quality.CurrentWidth;
        int height = quality.CurrentHeight;
        if (colorbuffer_tex == null)
        {
            colorbuffer_tex = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        }
        else
        {
            if (colorbuffer_tex.width != width || colorbuffer_tex.height != height)
            {
                if (Application.isPlaying)
                {
                    Object.Destroy(colorbuffer_tex);
                }
                else
                {
                    Object.DestroyImmediate(colorbuffer_tex);
                }
                colorbuffer_tex = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            }
        }
        return colorbuffer_tex;

    }

    public RenderTexture GetWaterReflectTex()
    {
        int width = quality.CurrentWidth / 2;
        int height = quality.CurrentHeight / 2;
        if (waterreflect_tex == null)
        {
            waterreflect_tex = new RenderTexture(width, height, 16, RenderTextureFormat.ARGB32);
        } else
        {
            if (waterreflect_tex.width != width || waterreflect_tex.height != height)
            {
                if (Application.isPlaying)
                {
                    Object.Destroy(waterreflect_tex);
                }
                else
                {
                    Object.DestroyImmediate(waterreflect_tex);
                }
                waterreflect_tex = new RenderTexture(width, height, 16, RenderTextureFormat.ARGB32);
            }
        }
        return waterreflect_tex;
    }
    public void DrawUIMask()
    {
        int count = UIWndMask.m_Mask.Count;
       // CheckerBoard_Enable = true;
        if (count > 0)
        {
            /* UIMesh = new Mesh();
             Vector3[] vertex = new Vector3[4* count];
             Vector2[] uvs = new Vector2[4* count];
             int[] triangles = new int[6* count];
             for (int i = 0; i < count; i++)
             {
                 vertex[i * 4] = UIWndMask.m_Mask[i].P1;
                 vertex[i * 4 + 1] = UIWndMask.m_Mask[i].P2;
                 vertex[i * 4 + 2] = UIWndMask.m_Mask[i].P3;
                 vertex[i * 4 + 3] = UIWndMask.m_Mask[i].P4;
                 triangles[i * 6 + 2] = i * 4;
                 triangles[i * 6 + 1] = i * 4 + 1;
                 triangles[i * 6 + 0] = i * 4 + 2;

                 triangles[i * 6 + 5] = i * 4 + 2;
                 triangles[i * 6 + 4] = i * 4 + 3;
                 triangles[i * 6 + 3] = i * 4 + 0;
             }
             UIMesh.vertices = vertex;
             UIMesh.uv = uvs;
             UIMesh.triangles = triangles;
             mat_UIMask.SetTexture("_MainTex", gbuffer_tex);*/
            float rate = 0f;
            for (int i = 0; i < count; i++)
            {
                Vector4 offset = Vector4.zero;
                float _WhRate = (float)Screen.width / (float)Screen.height;
                offset.x = UIWndMask.m_Mask[i].m_Bounds.center.x / _WhRate;
                offset.y = UIWndMask.m_Mask[i].m_Bounds.center.y;
                offset.z = UIWndMask.m_Mask[i].m_Bounds.extents.x / _WhRate;
                offset.w = UIWndMask.m_Mask[i].m_Bounds.extents.y;
                mat_UIMask.SetVector("_Offset", offset);
                if(SceneRenderSetting._Setting.DebugUIMask)
                    mat_UIMask.SetPass(1);
                else
                    mat_UIMask.SetPass(0);
                Graphics.DrawMeshNow(Quad, Vector3.zero, Quaternion.identity, 0);
                rate += offset.z * offset.w;
            }
            if (rate > 0.3f)
            {
             //   CheckerBoard_Enable = false;
            }
        }
    }
    public void DrawSky(Texture skyTexture, Vector4 farCorner, Matrix4x4 _InvViewMatrix, Color SkyColor,bool bReflect)
    {
        
        mat_Sky.SetVector("_Color", SkyColor);
        mat_Sky.SetVector("_FarCorner", farCorner);
        mat_Sky.SetMatrix("_InvViewMatrix", _InvViewMatrix);
        if(bReflect)
        {
            mat_Sky.SetFloat("_DirY", -1.0f);
        }
        else
        {
            mat_Sky.SetFloat("_DirY", 1.0f);
        }
        if (skyTexture is Cubemap)
        {
            mat_Sky.SetTexture("_SkyCubeTex", skyTexture);
            mat_Sky.SetPass(1);
        }
        else
        {
            mat_Sky.SetTexture("_MainTex", skyTexture);
            mat_Sky.SetPass(0);
        }
       
        
        Graphics.DrawMeshNow(Quad, Vector3.zero, Quaternion.identity, 0);
    }
    public void DrawLightShaft(RenderTexture FinalResult, Vector4 invViewport)
    {
        int width = quality.CurrentWidth;
        int height = quality.CurrentHeight;
        width = width / 4;
        height = height / 4;

        Camera MainCamera = GetComponent<Camera>();


        Vector3 E = MainCamera.transform.TransformDirection(Vector3.forward);
        Vector3 L = shadowCameraNear.transform.TransformDirection(-Vector3.forward);

        Vector3 v = L * 1000.0f + MainCamera.transform.position;
        v = MainCamera.WorldToViewportPoint(v);

        float fdot = Vector3.Dot(E, L);

        Vector4 temp = invViewport;
        temp.x = 1.0f / (float)width;
        temp.y = 1.0f / (float)height;

        //cos 50
        if (fdot > 0.64)
        {


            RenderTexture temp_HalfSize = RenderTexture.GetTemporary(width, height, 0);

            temp.x = 1.0f / (float)width;
            temp.y = 1.0f / (float)height;
            temp.z = 0;// SceneRenderSetting._Setting.LightShaft_Length;
            temp.w = SceneRenderSetting._Setting.LightShaft_Intensity;
            mat_LightShaft.SetVector("invViewport", temp);
            mat_LightShaft.SetTexture("_Sample2x2", Get2x2StepTexture());
            v.z = fdot;// *0.5f + 0.5f;// (fdot - 0.5f) * 2.0f;
            mat_LightShaft.SetVector("MainLight_UV", v);
            mat_LightShaft.SetColor("MainLightColor", SceneRenderSetting._Setting.MainLightColor * SceneRenderSetting._Setting.MainLightColorScale * SceneRenderSetting._Setting.LightShaft_Intensity);    
            mat_LightShaft.SetTexture("_SunTex", _SunTex);
            mat_LightShaft.SetFloat("_SunSize", SceneRenderSetting._Setting.SunShaftSize);
            //mat_LightShaft.SetPass(0);
            //Graphics.DrawMeshNow(Quad, Matrix4x4.identity, 0);
            FinalResult.wrapMode = TextureWrapMode.Repeat;
            Graphics.Blit(gbuffer_tex, temp_HalfSize, mat_LightShaft, 0);
            FinalResult.wrapMode = TextureWrapMode.Clamp;
            temp.x = invViewport.x;
            temp.y = invViewport.y;

            mat_LightShaft.SetVector("invViewport", temp);
            mat_LightShaft.SetTexture("_MainTex", temp_HalfSize);
            Graphics.SetRenderTarget(FinalResult);
            mat_LightShaft.SetPass(1);
            Graphics.DrawMeshNow(Quad, Matrix4x4.identity, 0);
            //Graphics.Blit(temp_HalfSize, FinalResult, mat_LightShaft, 1);

            RenderTexture.ReleaseTemporary(temp_HalfSize);
        }


    }
    public void DrawLightShaftLow(RenderTexture FinalResult, Vector4 invViewport)
    {
        int width = quality.CurrentWidth;
        int height = quality.CurrentHeight;
        width = width / 4;
        height = height / 4;

        Camera MainCamera = GetComponent<Camera>();


        Vector3 E = MainCamera.transform.TransformDirection(Vector3.forward);
        Vector3 L = shadowCameraNear.transform.TransformDirection(-Vector3.forward);

        Vector3 v = L * 1000.0f + MainCamera.transform.position;
        v = MainCamera.WorldToViewportPoint(v);

        float fdot = Vector3.Dot(E, L);

        Vector4 temp = invViewport;
        temp.x = 1.0f / (float)width;
        temp.y = 1.0f / (float)height;

        //cos 50
        if (fdot > 0.64)
        {
           
            temp.x = 1.0f / (float)width;
            temp.y = 1.0f / (float)height;
            temp.z = 0;// SceneRenderSetting._Setting.LightShaft_Length;
            temp.w = SceneRenderSetting._Setting.LightShaft_Intensity;
            mat_LightShaft.SetVector("invViewport", temp);
            mat_LightShaft.SetTexture("_MainTex", null);
            v.z = fdot;// *0.5f + 0.5f;// (fdot - 0.5f) * 2.0f;
            mat_LightShaft.SetVector("MainLight_UV", v);
            mat_LightShaft.SetColor("MainLightColor", SceneRenderSetting._Setting.MainLightColor * SceneRenderSetting._Setting.MainLightColorScale * SceneRenderSetting._Setting.LightShaft_Intensity);
            mat_LightShaft.SetFloat("_SunSize", SceneRenderSetting._Setting.LightShaft_Intensity);
            temp.x = invViewport.x;
            temp.y = invViewport.y;
            FinalResult.wrapMode = TextureWrapMode.Clamp;
           // mat_LightShaft.SetTexture("_MainTex", temp_HalfSize);
            mat_LightShaft.SetVector("invViewport", temp);

            Graphics.SetRenderTarget(FinalResult);
            mat_LightShaft.SetPass(3);

            Graphics.DrawMeshNow(Quad, Matrix4x4.identity, 0);
  
        }


    }
    void DrawGuassBlur(RenderTexture currentRenderTarget)
    {
        int w = currentRenderTarget.width / SceneRenderSetting._Setting.GaussDownSize;
        int h = currentRenderTarget.height / SceneRenderSetting._Setting.GaussDownSize;
        RenderTexture temp = RenderTexture.GetTemporary(w, h, 0);
        temp.filterMode = FilterMode.Bilinear;
        Graphics.Blit(currentRenderTarget, temp);
        // if(SceneRenderSetting._Setting.GaussMask!=null)
        {
            for (int i = 0; i < SceneRenderSetting._Setting.GaussTime; i++)
            {
                matGaussBlur.SetTexture("_MaskTex", SceneRenderSetting._Setting.GaussMask);
                matGaussBlur.SetFloat("_BlurSize", SceneRenderSetting._Setting.GaussRadius);
                RenderTexture temp2 = RenderTexture.GetTemporary(w, h, 0);
                Graphics.Blit(temp, temp2, matGaussBlur, 0);//为效果好可以循环多次采样，但是效率会变低
                RenderTexture.ReleaseTemporary(temp);
                temp = temp2;
            }
        }
        Graphics.Blit(temp, currentRenderTarget);
        RenderTexture.ReleaseTemporary(temp);

    }

    void DrawGuassBloom(RenderTexture currentRenderTarget)
    {//适合写实 场景明暗相差较大的场景，我们较亮的场景不适合。
        int w = currentRenderTarget.width / SceneRenderSetting._Setting.GaussDownSize;
        int h = currentRenderTarget.height / SceneRenderSetting._Setting.GaussDownSize;
        RenderTexture temp = RenderTexture.GetTemporary(w, h, 0);
        temp.filterMode = FilterMode.Bilinear;
        matGaussBloom.SetFloat("_Luminance", SceneRenderSetting._Setting.BloomLuminance);
        matGaussBloom.SetFloat("_Intensity", SceneRenderSetting._Setting.BloomIntersity);
        Graphics.Blit(currentRenderTarget, temp, matGaussBloom, 0);
 
        RenderTexture temp2 = RenderTexture.GetTemporary(w, h, 0);
        for (int i = 0; i < SceneRenderSetting._Setting.GaussTime; i++)
        {
            matGaussBlur.SetFloat("_BlurSize", SceneRenderSetting._Setting.GaussRadius);
           // RenderTexture temp2 = RenderTexture.GetTemporary(w, h, 0);
            Graphics.Blit(temp, temp2, matGaussBlur, 1);
           // RenderTexture.ReleaseTemporary(temp);
           // temp = temp2;

            //temp2 = RenderTexture.GetTemporary(w, h, 0);
            Graphics.Blit(temp2, temp, matGaussBlur, 2);
           // RenderTexture.ReleaseTemporary(temp);
            //temp = temp2;
        }

        RenderTexture main = RenderTexture.GetTemporary(currentRenderTarget.width, currentRenderTarget.height, 0);
        Graphics.Blit(currentRenderTarget, main);
        matGaussBloom.SetTexture("_BloomTex", temp);
       // matGaussBloom.SetTexture("_MainTex", main);
        Graphics.Blit(main, currentRenderTarget, matGaussBloom, 1);
        RenderTexture.ReleaseTemporary(temp);
        RenderTexture.ReleaseTemporary(main);
        RenderTexture.ReleaseTemporary(temp2);
    }

    public void DrawSSSObject(Material matSSS_SceneObject,RenderTexture FinalResult, RenderTexture Diffuse, Vector4 invViewport,Vector4 _FarCorner)
    {
        matSSS_SceneObject.SetTexture("_FinalDiffuse", FinalResult);
        matSSS_SceneObject.SetTexture("_Diffuse", Diffuse);
        matSSS_SceneObject.SetVector("_FarCorner", _FarCorner);
        matSSS_SceneObject.SetTexture("_MainTex", gbuffer_tex);
        Matrix4x4 maincam_World = Matrix4x4.identity;
        ViewMatrix(GetComponent<Camera>(), ref maincam_World);

        Vector4 lightdir = maincam_World.MultiplyVector(shadowCameraNear.transform.TransformDirection(Vector3.forward));
        matSSS_SceneObject.SetVector("LightDir", lightdir);
        matSSS_SceneObject.SetVector("LightColor", SceneRenderSetting._Setting.MainLightColor* SceneRenderSetting._Setting.MainLightColorScale);
        matSSS_SceneObject.SetPass(0);
        Graphics.DrawMeshNow(Quad, Vector3.zero, Quaternion.identity, 0);
    }
    public void DoFXAA(RenderTexture src, RenderTexture dst)
    {
        Graphics.Blit(src, dst, matFXAA);
    }

    public void DrawRimLight(RenderTexture _Diffuse, Vector4 invViewport, Vector4 _FarCorner)
    {
        Vector4 temp = invViewport;
        temp.w = 0;// SceneRenderSetting._Setting.RimLightThickness;
        gbuffer_tex.filterMode = FilterMode.Point;
        matRimLight.SetTexture("_Depth", gbuffer_tex);
        //matRimLight.SetTexture("_LastFrame", _LastFrame);
        matRimLight.SetTexture("_Diffuse", SceneRenderSetting._Setting.RimLightTexture);
        matRimLight.SetVector("invViewport", temp);
        matRimLight.SetVector("_FarCorner", _FarCorner);
        matRimLight.SetVector("RimLightColor", SceneRenderSetting._Setting.RimLightColor * SceneRenderSetting._Setting.RimLightScale);

        //matRimLight.SetVector("LightColor", SceneRenderSetting._Setting.MainLightColor * SceneRenderSetting._Setting.MainLightColorScale);
        matRimLight.SetPass(0);
        Graphics.DrawMeshNow(Quad, Vector3.zero, Quaternion.identity, 0);
    }
    public void DrawActorWhiting(Color c)
    {
        matActorWhiting.SetColor("_Color", c);
        matActorWhiting.SetPass(0);
        Graphics.DrawMeshNow(Quad, Vector3.zero, Quaternion.identity, 0);
    }
    public static void SetMaterialWhiting(Material mat,bool whiting)
    {
        if(mat == null)
        {
            return;
        }
        if (!mat.HasProperty("StencilRef"))
        {
            return;
        }
        int StencilRef = mat.GetInt("StencilRef");

        if(whiting)
        {
            StencilRef |= 2;
        }
        else
        {
            StencilRef &= 253;
        }
        mat.SetInt("StencilRef", StencilRef);

        if(whiting)
        {
            if (_instance != null)
            {
                _instance.ActorWhitingCount++;
            }
        }
        else
        {
            if (_instance != null)
            {
                _instance.ActorWhitingCount--;
            }
        }
    }

    public bool IsVisibleInMainCamera(Vector3 pos, float fRaduis)
    {
        return _MainCameraFrustum.IsVisiable(pos, fRaduis);
    }
    void OnLoadRainTexture(ResLoadParams param, UnityEngine.Object obj)// (sdFileInfo info, object obj, FileLoadParas param)
    {
        if(obj!=null)
        {
            int id = (int)param.userdata0;
            if(rain_texs!=null)
            {
                if(rain_texs.Length > id)
                {
                    rain_texs[id] = obj as Texture;
                }
            }
        }
    }
    public void DrawRain(Vector4 invViewport,Vector3 farCorner,Vector4 _MainLightDir,Vector4 _MainLightColor,Vector4 _AmbientColor,Matrix4x4 ViewToWorld,RenderTexture lastFrame,int light_count)
    {
        if (SceneRenderSetting._Setting.FullScreenRain > 0.0f)
        {
            if(!last_rain_valid)
            {
                last_rain_valid = true;
                if (rain_texs == null)
                {
                    rain_texs = new Texture[16];
                }
                for (int i = 0; i < rain_texs.Length; i++)
                {
                    if (Application.isPlaying)
                    {
                       // FileLoadParas param = new FileLoadParas();
                        ResLoadParams param = new ResLoadParams();
                        param.userdata0 = i;
                        ResourceMgr.Instance.LoadResource("$NGR/Texture/rain" + i + ".jpg", OnLoadRainTexture, param);
                        //sdFileSystem.Instance.Load("$NGR/Texture/rain" + i + ".jpg", OnLoadRainTexture, param, eLoadPriority.eLP_Middle);
                    }
                    else
                    {
#if UNITY_EDITOR
                        rain_texs[i] = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture>("Assets/$NGR/Texture/rain" + i + ".jpg");
#endif
                    }
                }

            }
            if (FullScreenRainMaterial != null)
            {
                int idx = (int)(Time.time * 16.0f);
                int count = rain_texs.Length;

                Vector4 rain_roughness = Vector4.one;
                rain_roughness.x = 0.999f;
                if(SceneRenderSetting._Setting.FullScreenRain<0.999f)
                {
                    rain_roughness.x = 1- SceneRenderSetting._Setting.FullScreenRain;
                }
                else
                {
                    rain_roughness.x = 0.0f;
                }

                Texture _RainTex = rain_texs[idx % count];
                if(_RainTex==null)
                {
                    _RainTex = rain_texs[0];
                }
                FullScreenRainMaterial.SetTexture("_RainTex", _RainTex);
                FullScreenRainMaterial.SetTexture("_Depth", gbuffer_tex);
                Cubemap rainEnv = SceneRenderSetting._Setting.SkyTexture as Cubemap;
                if(SceneRenderSetting._Setting.RainEnvCube!=null)
                {
                    rainEnv = SceneRenderSetting._Setting.RainEnvCube as Cubemap;
                }
                FullScreenRainMaterial.SetTexture("_SkyTex", rainEnv);
                FullScreenRainMaterial.SetTexture("_LastFrame", lastFrame);
                FullScreenRainMaterial.SetTexture("_Diffuse", colorbuffer_tex);
                FullScreenRainMaterial.SetVector("invViewport_Radius", invViewport);
                FullScreenRainMaterial.SetVector("farCorner", farCorner);
                FullScreenRainMaterial.SetVector("lightdir", _MainLightDir);
                FullScreenRainMaterial.SetVector("lightcolor", _MainLightColor);
                FullScreenRainMaterial.SetVector("ambientcolor", _AmbientColor);
                FullScreenRainMaterial.SetMatrix("ViewToWorld", ViewToWorld);
                FullScreenRainMaterial.SetVector("RainRoughness", rain_roughness);
                FullScreenRainMaterial.SetPass(light_count);
                Graphics.DrawMeshNow(Quad, Matrix4x4.identity);
            }
        }
        else
        {
            if (last_rain_valid)
            {
                last_rain_valid = false;
                for(int i=0;i<rain_texs.Length;i++)
                {
                    if (Application.isPlaying)
                    {
                        rain_texs[i] = null;
                        //sdFileSystem.Instance.UnloadAssets("$NGR/Texture/rain" + i + ".jpg");
                    }
                    else
                    {
                        Object.DestroyImmediate(rain_texs[i],true);
                        rain_texs[i] = null;
                    }
                }
                //rain_texs = null;
            }
        }
    }

  
    public void DrawScreenRain(Vector4 farCorner, Vector4 invViewport, RenderTexture target,Camera cam)
    {

        if (SceneRenderSetting._Setting.FullScreenRain > 0.0f&&SceneRenderSetting._Setting.RainCone != null)
        {

            matScreenRain.SetTexture("_RainTexture", SceneRenderSetting._Setting.RainTex);
            matScreenRain.SetTexture("_Depth", GetGbuffer());
            matScreenRain.SetVector("_UVData", SceneRenderSetting._Setting.RainData);
            Vector3 param = SceneRenderSetting._Setting.RainParam;
            param.y = param.y - param.x;//lerp
            matScreenRain.SetVector("_UVParam", SceneRenderSetting._Setting.RainParam);


            matScreenRain.SetPass(0);

            //  t.GetComponent<MeshFilter>().mesh = SceneRenderSetting._Setting.RainCone;
            Matrix4x4 xform = Matrix4x4.TRS(cam.transform.position, Quaternion.identity, Vector3.one * 10);
            Graphics.DrawMeshNow(SceneRenderSetting._Setting.RainCone, xform);
         //   t.transform.position = xform.MultiplyPoint(Vector3.zero);


        }
    }
    void DrawRadialBlur(RenderTexture CurrentBuffer,Vector4 InvViewport)
    {
        //if (SceneRenderSetting._Setting.RadialBlurIntensity > 0.0f)
        {
            if (RadialBlurMaterial != null)
            {

                Camera MainCamera = GetComponent<Camera>();

                sdRadialBlur rb = sdRadialBlur.Find(MainCamera);
                if(rb==null)
                {
                    return;
                }

                Vector3 viewport_center = MainCamera.WorldToViewportPoint(rb.transform.position);


                viewport_center.x = viewport_center.x * 2.0f - 1.0f;
                viewport_center.y = viewport_center.y * 2.0f - 1.0f;

                RadialBlurMaterial.SetTexture("_MainTex", CurrentBuffer);
                RadialBlurMaterial.SetTexture("_Kernel2x2", Get2x2StepTexture());
                float BlurLength = rb.Radius;

                Vector4 blur_param = new Vector4(viewport_center.x, viewport_center.y, BlurLength, rb.Intensity);
                RadialBlurMaterial.SetVector("blur_param", blur_param);
                RadialBlurMaterial.SetVector("invViewport", InvViewport);
                RadialBlurMaterial.SetPass(0);
                Graphics.DrawMeshNow(CyclePlane, Matrix4x4.identity);
            }
        }
    }
    public int ResolutionLevel()
    {
        return quality.ResolutionLevel;
    }
    public void ChangeResolution(int lvl)
    {
        quality.ChangeResolutionLevel(lvl);
    }
    void OnGUI()
    {
        if (showquality)
        {
            float val = quality.Level;
            quality.Level = (int)GUI.HorizontalScrollbar(new Rect(100, 10, 200, 50), val, 1, 0, (int)GlobalQualitySetting.Quality.MAX + 1);
            GUI.TextField(new Rect(300, 10, 50, 50), quality.Level.ToString() + "\n"+quality.CurrentWidth + "\n"+quality.CurrentHeight);
        }
    }
    public void Command(string key,int val)
    {
        if(key == "showquality")
        {
            showquality = val == 1;
        }
        
        if(quality!=null)
        {
            quality.Command(key, val);
        }
    }
    public int GetQualityLevel()
    {
        if(quality!=null)
        {
            return quality.Level;
        }
        return (int)GlobalQualitySetting.Quality.MAX;
    }
    public void SetQualityLevel(int quility_level)
    {
        if (quality != null)
        {
            quality.Level = quility_level;
        }
    }
    public int QualityMode
    {
        get
        {
            if (quality != null)
            {
                return (int)quality.mode;
            }
            return (int)GlobalQualitySetting.Mode.AutoFPS;
        }
        set
        {
            if (quality != null)
            {
                quality.mode = (GlobalQualitySetting.Mode)value;
                if(quality.mode == GlobalQualitySetting.Mode.Fast)
                {
                    quality.Level = (int)GlobalQualitySetting.Quality.MIN;

                }
                else if(quality.mode == GlobalQualitySetting.Mode.Perfect)
                {
                    quality.Level = (int)GlobalQualitySetting.Quality.MAX;
                }
            }
        }
    }
    public void SetQualityForceMode(GlobalQualitySetting.AutoForceMode mode)
    {
        if (quality != null)
        {
            if (mode == GlobalQualitySetting.AutoForceMode.CancelUIMode ||
                mode == GlobalQualitySetting.AutoForceMode.UIMode)
                quality.SetQualityForceMode(mode);
        }
    }

    public void SetResolutionLevel(int level)
    {
        if (quality != null)
        {
            quality.ResolutionLevel = level;
        }
    }
    public int GetResolutionLevel()
    {
        if (quality != null)
        {
            return quality.ResolutionLevel;
        }
        return (int)2;
    }

    public void OnLevelBegin(object userdata)
    {
        if (quality != null)
        {
            quality.OnLevelBegin(userdata);
        }
    }
    public void OnLevelEnd(object userdata)
    {
        if (quality != null)
        {
            quality.OnLevelEnd(userdata);
        }
    }

    void CheckShadowResolution()
    {
        
        if (quality!=null)
        {
            int res_level = quality.HalfShadowResolution;
            if(quality.mode == GlobalQualitySetting.Mode.Fast)
            {
                res_level = 1;
            }
            if (res_level == 1)
            {
                if(shadow_depth0.width != 512 || shadow_depth0.height!=512)
                {
                    DestroyRenderTexture(ref shadow_depth0);
                    DestroyRenderTexture(ref shadow_depth1);


                    if (shadow_depth0 == null)
                    {
                        shadow_depth0 = new RenderTexture(512, 512, 16, RenderTextureFormat.ARGB32);
                    }
                    if (shadow_depth1 == null)
                    {
                        shadow_depth1 = new RenderTexture(512, 512, 0, RenderTextureFormat.ARGB32);
                    }

                    shadowCameraNear.targetTexture = shadow_depth0;
                    NearStaticDepthRefresh = true;
                }
            }
            else
            {
                if (shadow_depth0.width != 1024 || shadow_depth0.height != 1024)
                {
                    DestroyRenderTexture(ref shadow_depth0);
                    DestroyRenderTexture(ref shadow_depth1);


                    if (shadow_depth0 == null)
                    {
                        shadow_depth0 = new RenderTexture(1024, 1024, 16, RenderTextureFormat.ARGB32);
                    }
                    if (shadow_depth1 == null)
                    {
                        shadow_depth1 = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGB32);
                    }

                    shadowCameraNear.targetTexture = shadow_depth0;
                    NearStaticDepthRefresh = true;
                }
            }
        }
    }
    public int GetCurrentWidth()
    {
        if(quality!=null)
        {
            return quality.CurrentWidth;
        }
        return Screen.width;
    }
    public int GetCurrentHeight()
    {
        if (quality != null)
        {
            return quality.CurrentHeight;
        }
        return Screen.height;
    }
    public float GetCullingLevel()
    {
        if (quality != null)
        {
            return quality.CullLevel;
        }
        return 1.0f;
    }
    public void RefreshStaticShadow()
    {
        NearStaticDepthRefresh = true;
    }
    public void StartAR()
    {
        if (!ar_enable)
        {
            ar_enable = true;
            if (ar_manager == null)
            {
                ar_manager = gameObject.AddComponent<UnityARCameraManager>();
            }
            ar_manager.StartAR();
        }
    }
    public void StopAR()
    {
        if(ar_enable)
        {
            ar_enable = false;
            
            ar_manager.StopAR();
        }
    }

    public int GetParticleLevel()
    {
        if (quality == null)
            return 2;
        else
            return quality.ParticleLevel;
    }

    RenderTexture GetCheckerBoardTexture()
    {
        if(!CheckerBoard_Enable)
        {
            if (CheckerBoard_Texture != null)
            {
                if (Application.isPlaying)
                {
                    Object.Destroy(CheckerBoard_Texture);
                }
                else
                {
                    Object.DestroyImmediate(CheckerBoard_Texture);
                }
                CheckerBoard_Texture = null;
            }
            return null;
        }
        int w = quality.CurrentWidth / 2;
        int h = quality.CurrentHeight / 2;
        if(CheckerBoard_Texture==null)
        {
            CheckerBoard_Texture = new RenderTexture(w, h, 16);
            InitCheckerBoardDepth();
        }
        if(w!= CheckerBoard_Texture.width || h!= CheckerBoard_Texture.height)
        {
            if(Application.isPlaying)
            {
                Object.Destroy(CheckerBoard_Texture);
            }
            else
            {
                Object.DestroyImmediate(CheckerBoard_Texture);
            }
            CheckerBoard_Texture = new RenderTexture(w, h, 16);
            InitCheckerBoardDepth();
        }
        
        
        return CheckerBoard_Texture;
    }
    void InitCheckerBoardDepth()
    {
        Graphics.SetRenderTarget(CheckerBoard_Texture);
        GL.Clear(true, true, new Color(1, 1, 1, 1));
        int w = quality.CurrentWidth / 2;
        int h = quality.CurrentHeight / 2;

        Vector4 invViewport = new Vector4(1.0f / (float)w, 1.0f / (float)h, 0, 0);
        matCheckerBoard.SetVector("_InvViewport", invViewport);
        matCheckerBoard.SetTexture("_OIT2X2", OIT_2X2);
        matCheckerBoard.SetPass(0);
        Graphics.DrawMeshNow(Quad, Matrix4x4.identity);
    }

    public static void DestroyPipeline()
    {
        if(_instance!=null)
        {
            Transform root = _instance.transform;
            while(true)
            {
                if(root.parent==null)
                {
                    break;
                }
                else
                {
                    root = root.parent;
                }
            }
            _instance = null;
            GameObject.Destroy(root.gameObject);
            
        }
    }
}
