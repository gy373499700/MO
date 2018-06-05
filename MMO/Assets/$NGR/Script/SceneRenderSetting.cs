using UnityEngine;
using System.Collections;
using System.Threading;
#if UNITY_EDITOR
using UnityEditor;

#endif
[ExecuteInEditMode]
public class SceneRenderSetting : MonoBehaviour {
    [Tooltip("近裁剪面（主摄像机）")]
    public float CameraNearCullPlane = 1.0f;
    [Tooltip("远裁剪面（主摄像机）")]
    public float CameraFarCullPlane = 100.0f;
    [Tooltip("视角（主摄像机）")]
    public float CameraFOV = 60.0f;
    [Tooltip("SSAO采样半径（推荐 5~10）\n 半径大 整体感好 柔和 缺少细节\n 半径小 整体感差 细节更多 ")]
    public float SSAOSampleRadius = 40;
    [Tooltip("阴影采样半径（推荐 4左右） \n半径大 半影区域大 细节差 \n半径小 细节好 会有锯齿感")]
    public float MainLightShadowSampleRadius = 4.0f;
    [Tooltip("主灯颜色色调")]
    public Color MainLightColor         = Color.white;
    [Tooltip("主灯强度")]
    public float MainLightColorScale    = 5.0f;
    [Tooltip("主灯方向（默认 45 0，0） X俯仰角 为0代表水平 Y水平角 ")]
    public Vector3 MainLightDirection = new Vector3(45, -1, -1);
    [Tooltip("主灯阴影 最小自阴影距离 一般场景0.07 \n值太小 有可能出现条纹状错误效果\n 值太大 自阴影效果 细节缺失")]
    public Vector4 MainLightBias = new Vector4(0.03f, 0.15f, 1.0f);
    [Tooltip("主灯阴影范围 推荐15~25 \n太大 自阴影差 锯齿感强烈 \n太小 画面中有阴影区域会变少 并且容易察觉到阴影交界线")]
    public Vector4 MainLightCameraSize = new Vector4(5, 25, 125);
    [Tooltip("暂无")]
    public int MainLightShadowQulity = 3;
    [Tooltip("环境光颜色色调")]
    public Color AmbientColor           = Color.white;
    [Tooltip("环境光强度")]
    public float AmbientColorScale      = 1.0f;
    public Cubemap AmbientDiffuseCube;
    [Tooltip("全局光强度 建议1")]
    public float GI_Scale = 1.0f;
    [Tooltip("高度雾 最大高度")]
    public float Fog_Height = 10.0f;
    [Tooltip("高度雾 雾效开始距离")]
    public float Fog_Start = 0.0f;
    [Tooltip("高度雾 雾效结束距离")]
    public float Fog_End = 100.0f;
    [Tooltip("高度雾 衰减因子 1表示线性 其他值衰减公式pow(xxx,Fog_Attenuation)")]
    public float Fog_Attenuation = 1.0f;
    [Tooltip("高度雾 雾效颜色")]
    public Color Fog_Color = Color.white;
    [Tooltip("高度雾 雾效运动速度")]  
    public float Fog_Speed = 0.1f;
    [Tooltip("高度雾 雾效运动效果强化")]
    public float Fog_DensityMount = 0.5f;
    [Tooltip("高度雾 雾效运动")]
    public Texture2D Fog_DynamicNoise = null;

    [Tooltip("地面雾效强度,x 地面x方向运动速度，y地面y方向运动速度")]
    public Vector3 PlaneFogSpeed = new Vector3(0.1f,0,0);
    [Tooltip("地面雾效,x 雾效强度，y为噪声强度，z云的起始高度,w云的结束高度")]
    public Vector4 PlaneFogParam = new Vector4(0.1f, 0.1f, 0,5);
    [Tooltip("地面雾效颜色")]
    public Color PlaneFogColor = Color.white;
    [Tooltip("地面雾效 雾效运动")]
    public Texture2D FogPlane_DynamicNoise = null;

    [Tooltip("高度雾 是否影响天空")]
    public bool Fog_Sky = true;

    [Tooltip("景深 开始距离")]
    public float DOF_Focus = 100.0f;

    public bool DOF_Without_Sky = false;
    [Tooltip("次表面散射 采样半径")]
    public float SSS_Radius = 1.0f;
    [Tooltip("次表面散射 颜色")]
    public Color SSS_Color = Color.red;
    [Tooltip("次表面散射 强度")]
    public float SSS_Color_Scale = 1.0f;
    [Tooltip("角色勾边光 颜色")]
    public Color RimLightColor = Color.white;
    [Tooltip("角色勾边光 预计算贴图（来源美术手工绘制）")]
    public Texture RimLightTexture = null;
    [Tooltip("角色勾边光 强度 建议值1")]
    public float RimLightScale = 1.0f;
    [Tooltip("景深 模糊半径 值越大远景越模糊 算法原因值不宜太大 建议0~3")]
    public float Glow_BlurRadius = 2.0f;
    [Tooltip("godray 体积光 强度")]
    public float LightShaft_Intensity = 0.1f;
    [Tooltip("曝光系数 参考单反相机的曝光值 建议1 值越大画面越亮")]
    [Range(0.01f, 8)]
    public float ToneMapping_Exposure = 1.0f;
    [Tooltip("伽玛校正参数 调整最终画面的颜色饱和度 建议0.79")]
    [Range(0.01f, 5)]
    public float ToneMapping_gamma = 0.79f;
    [Tooltip("饱和度")]
    [Range(0, 1)]
    public float ToneMapping_Saturation = 1.0f;
    [Tooltip("对比度")]
    [Range(0, 1)]
    public float ToneMapping_Contract = 1.0f;

    [Tooltip("角色泛白")]
    public float ActorHightLight_Shiness = 0.2f;
    [Tooltip("角色泛白强度")]
    public float ActorHightLight_Scale = 1.0f;
    [Tooltip("天空盒 美术制作 全景球面图 尺寸一般为1024*512 2048*1024 宽高比2：1")]
    public Texture SkyTexture = null;
    [Tooltip("环境光高光IBL贴图 来源美术制作")]
    public Texture SkySpecular = null;

    public float ReflectIntensity = 1.0f;
    public Color ActorWhiting = Color.white;
    [Tooltip("阴影相机 近裁剪面 ")]
    public float ShadowCameraNear = 1.0f;
    [Tooltip("阴影相机 远裁剪面 ")]
    public float ShadowCameraFar = 100.0f;
    [Tooltip("全屏幕 下雨效果 0.0表示没有雨 大于0表示 雨水强度 地面湿润度")]
    public float FullScreenRain = 0.0f;
    [Tooltip("全屏幕 下雨效果 环境图")]
    public Texture RainEnvCube = null;
    [Tooltip("环境光是否影响天空")]
    public bool AmbientSky = true;
    [Tooltip("雨滴Mesh")]
    public Mesh RainCone;
    [Tooltip("雨滴贴图")]
    public Texture2D RainTex;
    [Tooltip("雨滴密度和速度,xy密度，zw速度")]
    public Vector4 RainData = new Vector4(20, 20, 6, 90);
    [Tooltip("雨滴距离和alpha,x远距离，y近距离，z是alpha")]
    public Vector3 RainParam=new Vector3(0.1f,-0.1f,1);

    [Tooltip("Bloom亮度阀值")]
    [Range(0f, 1f)]
    public float BloomLuminance = 0.2f;
    [Tooltip("Bloom强度")]
    public float BloomIntersity = 5f;
    [Tooltip("高斯模糊裁减")]
    public Texture2D GaussMask = null;
    [Tooltip("高斯模糊采样距离")]
    [Range(1f, 8f)]
    public float GaussRadius = 3;
    [Tooltip("高斯模糊采样降频，越大效率越高")]
    [Range(1,8)]
    public int GaussDownSize = 2;
    [Tooltip("高斯模糊采样次数 慎用")]
    [Range(1, 8)]
    public int GaussTime = 2;
    [Tooltip("太阳大小")]
    public float SunShaftSize = 16f;
    [Tooltip("Input屏幕扰动贴图")]
    public Texture2D InputFracTex;
    [Tooltip("电影镜头特效选择切换")]
    [Range(0, 146)]
    public int LutifyColor = 0;
    [Tooltip("电影镜头特效Blend值")]
    [Range(0, 1)]
    public float LutifyAlpha = 1;
    [Tooltip("电影镜头特效所使用的text贴图")]
    public Texture2D LutifyTex;



    [System.NonSerialized]
    public float RadialBlurIntensity = 0.0f;
    [System.NonSerialized]
    public float RadialBlurRadius = 0.5f;
    [System.NonSerialized]
    public Vector3 RadialBlurPosition = Vector3.zero;
    public Texture2D MipFogTexture = null;
    //public float FullScreenRainRoughness = 0.0f;
    //private float HalfPixelAA = 0.5f;

    public bool SyncSceneView = false;
    private GameObject sync_obj = null;
    public bool HideUI = false;
    public bool EnableReflect = true;
    public bool EnableShadow = false;
    public bool EnableSSAO = false;
	public bool EnableSSAOPro = false;
	public bool EnableSSAODebug = false;
    public bool EnableDeferredLight = false;
    public bool EnableGI = false;
    public bool EnableFog = false;
    public bool EnablePlaneFog = false;
    public bool EnableAA = false;
    //public bool EnableDOF = false;
    public bool EnableWater = false;
    public bool EnableSSSSS = false;
    public bool EnableGlow = false;
    public bool EnableToneMapping = false;
    public bool EnableTransparent = true;
    //public bool EnableDecal = false;
    public bool EnableShadowLight = false;
    public bool EnableActorHightLight = false; 
    public bool EnableLightShaft = false;
   // public bool EnableSSSObject = false;
    public bool EnableFXAA = false;
    public bool EnableRimLight = false;
    public bool ShowNormal = false;
    public bool DebugRT = false;
    // public bool DebugShadow = false;
     public bool UseMRT = false;
    public bool ShowLightmap = false;
    [System.NonSerialized]
    public bool EnableActorWhiting = true;
    public bool EnableMainCharXRay = true;
    public bool EnableTwistScreen = false;
    public bool EnableGaussBlur = false;
    public bool EnableBloom = false;
    public bool EnableVertexShaft = false;
    public bool DebugUIMask = false;
    public bool EnableLutify = false;
    public bool EnableInputDisturb = false;
    //public GameObject RenderPipelineObj = null;
    private GameObject privatePipeline = null;
    [System.NonSerialized]
    public bool depth_srgb = false;
    public int SleepTime = 0;
    //public bool HideSkyBox = false;

    public Vector3      SceneCamera_Pos;
    public Quaternion   SceneCamera_Rot;
    

    public static SceneRenderSetting _Setting = null;
    public static SceneRenderSetting _OrginalSetting = null;



    public float current_fps_time = 0;
    public int framecount = 0;
    float fps = 0.0f;
    private Camera _MainCamera = null;
  //  private sdGameCamera _gameCamera = null;
    public Camera MainCamera
    {
        get { return _MainCamera; }
        set { _MainCamera = value; }
    }

    static void SetDontSave(GameObject obj)
    {
        if(obj==null)
        {
            return;
        }
        obj.hideFlags = HideFlags.DontSave;
        for(int i=0;i<obj.transform.childCount;i++)
        {
            SetDontSave(obj.transform.GetChild(i).gameObject);
        }
    }
    void Awake()
    {


    }
    void OnLoadPipeline(ResLoadParams param, UnityEngine.Object obj)
    {
        privatePipeline = GameObject.Instantiate(obj) as GameObject;
        GameObject.DontDestroyOnLoad(privatePipeline);
    }
    void OnEnable()
    {
        _Setting = this;
        if (!Application.isPlaying)
        {
            DestroyPipeline();

#if UNITY_EDITOR
            StartCoroutine(Load());
           /* if (privatePipeline == null)
            {
                GameObject RenderPipelineObj = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/$NGR/Prefab/RenderPipeline.prefab") as GameObject;
                privatePipeline = GameObject.Instantiate(RenderPipelineObj) as GameObject;
                SetDontSave(privatePipeline);

            }*/
#endif
        }
        else
        {
            if(RenderPipeline._instance==null)
            {
                ResLoadParams kParam2 = new ResLoadParams();
                kParam2.info = "RenderPipeline";
                ResourceMgr.Instance.MarkDontUnLoad("$NGR/Prefab/RenderPipeline.prefab");
                ResourceMgr.Instance.LoadResourceImmediately("$NGR/Prefab/RenderPipeline.prefab", OnLoadPipeline, kParam2);
                // sdFileSystem.Instance.SetDontUnloadFile("$NGR/Prefab/RenderPipeline.prefab",true);
              //  GameObject RenderPipelineObj = (GameObject)sdFileSystem.Instance.Load("$NGR/Prefab/RenderPipeline.prefab");
              //  privatePipeline = GameObject.Instantiate(RenderPipelineObj) as GameObject;
               // GameObject.DontDestroyOnLoad(privatePipeline);
            }
            
        }
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (SyncSceneView && sync_obj == null)
            {

                sync_obj = new GameObject();
                sync_obj.name = "SyncObj(Dont Delete)";
                sync_obj.AddComponent<MeshRenderer>();
                sync_obj.AddComponent<SceneCameraSync>();
                SetDontSave(sync_obj);
                
            }
        }
#endif

    }
    IEnumerator Load()
    {
        yield return null;
        yield return null;
        string path = Application.dataPath.Replace("Assets", "") + "Bundles/";
        string MainBunldes = path + "Bundles";
        AssetBundle manifestBundle = AssetBundle.LoadFromFile(MainBunldes);
        AssetBundleManifest manifest = (AssetBundleManifest)manifestBundle.LoadAsset("AssetBundleManifest");
        string bundleName = "$ngr.unity3d";//"ui/$scene.prefab.unity3d";//

        string resname = "RenderPipeline";//"$scene.prefab";// 

        string[] dependents = manifest.GetAllDependencies(bundleName.ToLower());



        Debug.Log(dependents.Length); 
        AssetBundle[] dependsAssetbundle = new AssetBundle[dependents.Length];
        for (int index = 0; index < dependents.Length; index++)
        {  //加载所有的依赖文件;  

            dependsAssetbundle[index] = AssetBundle.LoadFromFile(path + dependents[index]);
            Debug.Log(dependsAssetbundle[index]);

        }
        AssetBundle item = AssetBundle.LoadFromFile(path + bundleName);
        Debug.Log(item);


        Object abr2 = item.LoadAsset(resname.ToLower());
        privatePipeline = GameObject.Instantiate(abr2) as GameObject;
        SetDontSave(privatePipeline);
        Debug.Log(abr2);
        manifestBundle.Unload(false); 
        item.Unload(false);
        if (RenderPipeline._instance != null)
        {
            RenderPipeline._instance.quality.mode = GlobalQualitySetting.Mode.Merge;
        }
        for (int index = 0; index < dependents.Length; index++)
        {  //加载所有的依赖文件;  

            dependsAssetbundle[index].Unload(false);

        }
        yield return null;
    }
    void OnDisable()
    {
        //_Setting = null;
        if (!Application.isPlaying)
        {
            DestroyPipeline();
        }
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (sync_obj != null)
            {
               GameObject.DestroyImmediate(sync_obj);
            }
        }
#endif

    }
    void DestroyPipeline()
    {
        if (privatePipeline != null)
        {
            if (Application.isPlaying)
            {
                Object.Destroy(privatePipeline);
            }
            else
            {
                Object.DestroyImmediate(privatePipeline);
            }
            privatePipeline = null;
        }
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        //MainLightDirection.Normalize();


        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");

#if UNITY_EDITOR

#endif


        float fcurrent = Time.unscaledTime;
        float offset = fcurrent - current_fps_time;
        if (offset > 1.0f)
        {
            float count = Time.frameCount - framecount;
            fps = count / offset;
            current_fps_time = Time.unscaledTime;
            framecount = Time.frameCount;
        }


        if(SleepTime>0)
        {
            Thread.Sleep(SleepTime);
        }

        if (Application.isPlaying)
        {
          //  if (sdGlobalDatabase.Instance.NeedCheckArea)
          //      sdAreaRenderSetting.CheckArea(transform.position);
        }
        else
        {
            sdAreaRenderSetting.CheckArea(transform.position);
        }
	}

    public int GetFps()
    {
        return (int)fps;
    }

    void OnGUI()
    {
        return;
        string txt = "HideUI";
        if (HideUI)
        {
            txt = "";
        }
        int iScreenWidth = Screen.width;
        int iScreenHeight = Screen.height;
		if (GUI.Button(new Rect(0, 0, 50, 50), "ShowMenu"))//if (GUI.Button(new Rect(iScreenWidth/2 - 150, 0, 150, 150), txt, GUI.skin.label))
        {
            HideUI = !HideUI;
        }
        if (!HideUI)
        {
            string depth_type = UnityEngine.SystemInfo.graphicsDeviceType.ToString() + "\n"+Application.targetFrameRate;
            //if (depth_srgb)
            //{
            //    depth_type = "srgb";
            //}
                //string fps =  (1.0f / Time.deltaTime).ToString();
            if (GUI.Button(new Rect(300, 100, 100, 50), depth_type))
            {
                if(Application.targetFrameRate==30)
                {
                    Application.targetFrameRate = -1;
                }
                else
                {
                    Application.targetFrameRate = 30;
                }
            }
            

            string fpstext = Screen.width + "|"+Screen.height+"\n"+ fps;
            if(RenderPipeline._instance!=null)
            {
                fpstext = RenderPipeline._instance.GetCurrentWidth() + "|"+ RenderPipeline._instance.GetCurrentHeight()+"\n" + fps;
            }
            if (GUI.Button(new Rect(300, 150, 100, 50), fpstext))
            {

            }

            if (GUI.Button(new Rect(300, 0, 100, 100), "up"))
            {
                gameObject.transform.position += Vector3.forward;// *Time.deltaTime;
            }
            if (GUI.Button(new Rect(300, 200, 100, 100), "down"))
            {
                gameObject.transform.position -= Vector3.forward; ;// *Time.deltaTime;
            }
            if (GUI.Button(new Rect(200, 100, 100, 100), "left"))
            {
                gameObject.transform.position += Vector3.left;// *Time.deltaTime;
            }
            if (GUI.Button(new Rect(400, 100, 100, 100), "right"))
            {
                gameObject.transform.position += Vector3.right;// *Time.deltaTime;
            }

            txt = "Reffect(On)";
            if (!EnableReflect)
            {
                txt = "Reffect(Off)";
            }
            if (GUI.Button(new Rect(0, 50, 100, 50), txt))
            {
                EnableReflect = !EnableReflect;
            }

            txt = "Shadow(On)";
            if (!EnableShadow)
            {
                txt = "Shadow(Off)";
            }
            if (GUI.Button(new Rect(0, 100, 100, 50), txt))
            {
                EnableShadow = !EnableShadow;
            }

            txt = "SSAO(On)";
            if (!EnableSSAO)
            {
                txt = "SSAO(Off)";
            }
            if (GUI.Button(new Rect(0, 150, 100, 50), txt))
            {
                EnableSSAO = !EnableSSAO;
            }
            txt = "DLight(On)";
            if (!EnableDeferredLight)
            {
                txt = "DLight(Off)";
            }
            if (GUI.Button(new Rect(0, 200, 100, 50), txt))
            {
                EnableDeferredLight = !EnableDeferredLight;
            }
            txt = "GI(On)";
            if (!EnableGI)
            {
                txt = "GI(Off)";
            }
            if (GUI.Button(new Rect(0, 250, 100, 50), txt))
            {
                EnableGI = !EnableGI;
            }

            txt = "Fog(On)";
            if (!EnableFog)
            {
                txt = "Fog(Off)";
            }
            if (GUI.Button(new Rect(0, 300, 100, 50), txt))
            {
                EnableFog = !EnableFog;
            }

            txt = "AA(On)";
            if (!EnableAA)
            {
                txt = "AA(Off)";
            }
            if (GUI.Button(new Rect(0, 350, 100, 50), txt))
            {
                EnableAA = !EnableAA;
            }


            //             txt = "DOF(On)";
            //             if (!EnableDOF)
            //             {
            //                 txt = "DOF(Off)";
            //             }
            //             if (GUI.Button(new Rect(0, 400, 100, 50), txt))
            //             {
            //                 EnableDOF = !EnableDOF;
            //             }

            //txt = "Water(On)";
            //if (!EnableWater)
            //{
            //    txt = "Water(Off)";
            //}
            //if (GUI.Button(new Rect(0, 400, 100, 50), txt))
            //{
            //    EnableWater = !EnableWater;
            //}

            txt = "SSS(On)";
            if (!EnableSSSSS)
            {
                txt = "SSS(Off)";
            }
            if (GUI.Button(new Rect(0, 400, 100, 50), txt))
            {
                EnableSSSSS = !EnableSSSSS;
            }
            txt = "Bloom(On)";
            if (!EnableBloom)
            {
                txt = "Bloom(Off)";
            }
            if (GUI.Button(new Rect(0, 450, 100, 50), txt))
            {
                EnableBloom = !EnableBloom;
            }

            txt = "Dof(On)";
            if (!EnableGlow)
            {
                txt = "Dof(Off)";
            }
            if (GUI.Button(new Rect(100, 0, 100, 50), txt))
            {
                EnableGlow = !EnableGlow;
            }

            txt = "ToneMap(On)";
            if (!EnableToneMapping)
            {
                txt = "ToneMap(Off)";
            }
            if (GUI.Button(new Rect(100, 50, 100, 50), txt))
            {
                EnableToneMapping = !EnableToneMapping;
            }

            //txt = "Decal(On)";
            //if (!EnableDecal)
            //{
            //    txt = "Decal(Off)";
            //}
            //if (GUI.Button(new Rect(100, 100, 100, 50), txt))
            //{
            //    EnableDecal = !EnableDecal;
            //}

            txt = "ShadowLight(On)";
            if (!EnableShadowLight)
            {
                txt = "ShadowLight(Off)";
            }
            if (GUI.Button(new Rect(100, 150, 100, 50), txt))
            {
                EnableShadowLight = !EnableShadowLight;
            }
            txt = "ActorSpec(On)";
            if (!EnableActorHightLight)
            {
                txt = "ActorSpec(Off)";
            }
            if (GUI.Button(new Rect(100, 200, 100, 50), txt))
            {
                EnableActorHightLight = !EnableActorHightLight;
            }

             txt = "LightShaft(On)";
             if (!EnableLightShaft)
            {
                txt = "LightShaft(Off)";
            }
            if (GUI.Button(new Rect(100, 250, 100, 50), txt))
            {
                EnableLightShaft = !EnableLightShaft;
            }

            txt = "ShowNormal(On)";
            if (!ShowNormal)
            {
                txt = "ShowNormal(Off)";
            }
            if (GUI.Button(new Rect(100, 300, 100, 50), txt))
            {
                ShowNormal = !ShowNormal;
            }
            txt = "FXAA(On)";
            if (!EnableFXAA)
            {
                txt = "FXAA(Off)";
            }
            if (GUI.Button(new Rect(100, 350, 100, 50), txt))
            {
                EnableFXAA = !EnableFXAA;
            }

            txt = "RimLight(On)";
            if (!EnableRimLight)
            {
            	txt = "RimLight(Off)";
            }
            if (GUI.Button(new Rect(100, 400, 100, 50), txt))
            {
            	EnableRimLight = !EnableRimLight;
            }

          /*  txt = "MRT(On)";
            if (!UseMRT)
            {
                txt = "MRT(Off)";
            }
            if (GUI.Button(new Rect(100, 450, 100, 50), txt))
            {
                UseMRT = !UseMRT;
            }*/
            txt = "MaskDE(Off)";
            if (DebugUIMask)
            {
                txt = "MaskDE(On)";
            }
            if (GUI.Button(new Rect(100, 500, 100, 50), txt))
            {
                DebugUIMask = !DebugUIMask;
            }

            txt = "Res";
            //if (ScreenRes == 1)
            //{
            //    txt = "Res(Middle)";
            //}
            //else if (ScreenRes == 2)
            //{
            //    txt = "Res(Low)";
            //}
            if (GUI.Button(new Rect(200, 200, 100, 50), txt))
            {
                // ScreenRes = (ScreenRes + 1) % 3;
                // if (ScreenRes == 0)
                // {
                //     Screen.SetResolution(OriginWidth , OriginHeight, false);
                // }
                // else if (ScreenRes == 1)
                // {
                //     Screen.SetResolution(OriginWidth *2/ 3, OriginHeight *2/ 3, false);
                // }
                // else if (ScreenRes == 2)
                // {
                //     Screen.SetResolution(OriginWidth / 2, OriginHeight / 2, false);
                // }
                if(RenderPipeline._instance!=null)
                {
                    int lvl = RenderPipeline._instance.ResolutionLevel();
                    RenderPipeline._instance.ChangeResolution((lvl + 1) % 3);
                }


            }
            txt = "FightStatistics";
            if (GUI.Button(new Rect(200, 50, 100, 50), txt))
            {
              //  sdUICharacter.Instance.ShowFightStatistics(true);
            }
        }
    }
    public Ray ScreenPointToRay(Vector3 p)
    {
        if(MainCamera!=null)
        {
            return MainCamera.ScreenPointToRay(p);
        }
        return new Ray();
    }
    public void LateUpdate()
    {
      /*  if (_gameCamera == null) 
            _gameCamera = gameObject.GetComponent<sdGameCamera>();
        if (_gameCamera != null)
        {
            if (_gameCamera.enabled)
            {
                _gameCamera.tick();
            }
        }*/
        
        if (RenderPipeline._instance != null)
        {
            RenderPipeline._instance.tick();
        }
        GrassGroup.Tick();

        if (Application.isPlaying)
        {
            //血条依赖摄像机位置和角色位置  所以需要放到 摄像机更新之后
//             if (sdHpbarMgr.RefreshAllHpbar() == false)
//             {
//                 sdHPBar_1.Tick();
//             }
        }

        
        if (Application.isPlaying)
        {
//             sdNpcHudMgr.RefreshAllNpcHud();
//             sdGlobalDatabase.Instance.UpdateNpcHudAndHpBarDis();
        }
    }
}
