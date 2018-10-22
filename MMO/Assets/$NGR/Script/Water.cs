using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[ExecuteInEditMode]
public class Water : MonoBehaviour {
    public static List<Water> WaterList = new List<Water>();
    
    public enum DirectMode
    {
        Quad,
        Plane,
    }

    public enum FollowMode
    {
        Sampler,
        Pos,
        RotAndPos,
    }

    public enum WaterType
    {
        ocean,
        lake,
    }

    void OnEnable()
    {
        WaterList.Add(this);
    }
    void OnDisable()
    {
        WaterList.Remove(this);

        ClearReflCam();
        //Debug.Log("Water OnDisable"+gameObject.name);
    }

    public Color color = new Color(1, 1, 1, 1);
    public Color deepcolor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    public Color reflectColor = Color.white;
    public Mesh renderMesh;
    private Vector3[] baseVertex;
    private Vector3[] nowVertex;
    public Texture2D _NormalTex;
    [Range(0,1)]
    public float CausticsLerp = 1f;//焦散比重

    public Texture2D[] _CausticsTex;
    public float refract_scale = 1.0f;//边缘折射扰动幅度
    public float water_fog_distance = 1.0f;//水距离
    public float water_fog_y_depth = 1.0f;//水深度
    public float water_refract_y_depth = 0.2f;//折射深度
    public float frenel_scale = 1.0f;//no use
    public float first_wave_scale =0.1f;
    public float first_wave_speed = 1.0f;
    public float second_wave_scale = 0.1f;
    public float second_wave_speed = 1.0f;// four 用来移动法线uv
    public Vector2 wave_dir = new Vector2(0, 90);//波的方向
    public bool use_mesh_normal = false;
    private bool visiable = false;
    public bool enableRefl = false;
    public bool enableReflShield = false;
    public Vector3[] cylinderList = new Vector3[0];
    public Vector4[] sphereList = new Vector4[0];
    public bool enableWave = false;
    public bool enableSubWater = false;
    public bool enableCausitic = false;
    public WaterType type = WaterType.lake;
    public float ScreenDistort = 1f;
    public float ScreenWaterMaxDis = 20f;
    public float ScreenWaterMinDis = 10f;
    public Vector3 subwaterPlaneFogSpeed = new Vector3(0.1f, 0, 0);
    public Texture subwaterFogNoise = null;
    //gwave
    Vector4 _TimeCalc;
    public float _GerstnerIntensity = 1.0f;
    public Vector4 _GSpeed = new Vector4(2.33f, 2.38f, 2.7f, 2.42f);
    public Vector4 _GAmplitude = new Vector4(0.13f, 0.06f, 0.12f, 0.12f);
    public Vector4 _GFrequency = new Vector4(1.72f, 1.02f, 1.95f, 1.46f);
    public Vector4 _GSteepness = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
    public Vector4 _GDirectionAB = new Vector4(1.0f, 0.0f, 0.51f, -1.39f);
    public Vector4 _GDirectionCD = new Vector4(-0.29f, 0.44f, -0.77f, 0.14f);
    //cewave
    public Vector4 _OceanParams0 = new Vector4(1, 1, 1, 1);
    public float _AnimGenParams = 1;
    public Vector4 _FlowDir = new Vector4(1, 1, 3, 4);

    //customMesh
    public bool customMesh = false;
    public DirectMode dirMode = DirectMode.Quad;
    private bool lastMeshSet = false;
    private Mesh m_customMesh;
    public int row = 70;
    public int column = 70;

    //syncMove
    public FollowMode followMode = FollowMode.RotAndPos;
    private FollowMode lastFollowMode = FollowMode.RotAndPos;
    public bool bActiveFollowCam = false;
    public float validSize = 50f;
    public bool extendEdge = true;
    public float noWaveSize = 300f;
    private bool bLastActive;
    private Transform destTrans;
    private Camera destCam;
    private Vector3 followPos;
    private Vector3 lastPos;
    private Vector3 lastFloatPos;
    private Vector3 lastEuler;
    private Vector3 waterEuler;
    public bool xMove = true;
    public bool yMove = true;
    public bool zMove = false;
    private bool meshDirty = true;
    private Vector2 m_cellSize;
    private Vector3 worldPlaneCenter;

    //Shield Refl Field
    
    public enum ShieldReflType
    {
        Cylinder,   //v1: (pos.x, pos.z, size)
        Cube,       //v1: (pos.x, pos.y, pos.z)  v2: (size.x, size.y, size.z)
        Sphere      //v1: (pos.x, pos.y, pos.z)  v2: (radius, null, null)
    }
    
    public bool CanRefl()
    {
        if (!enableRefl)
            return false;

        if (!enableReflShield)
            return true;

        Vector3 camWorldPos = !Application.isPlaying ? 
                                SceneRenderSetting._Setting.SceneCamera_Pos
                                : RenderPipeline._instance.transform.position;
        //check cylinder
        if (cylinderList != null && cylinderList.Length > 0)
        {
            for (int i = 0, iMax = cylinderList.Length; i < iMax; i++)
            {
                Vector3 sr = cylinderList[i];
                Vector2 p1 = new Vector2(sr.x, sr.y);
                Vector2 p2 = new Vector2(camWorldPos.x, camWorldPos.z);
                Vector3 off = p1 - p2;
                if (Vector3.Dot(off, off) < sr.z * sr.z)
                {
                    return false;
                }
            }
        }
        //checkSphere
        if(sphereList != null && sphereList.Length > 0)
        {
            for(int i = 0, iMax = sphereList.Length; i < iMax; i++)
            {
                Vector4 _set = sphereList[i];
                Vector3 sPos = new Vector3(_set.x, _set.y, _set.z);
                float size = _set.w;
                Vector3 offset = sPos - camWorldPos;
                if (Vector3.Dot(offset, offset) < size * size)
                {
                    return false;
                }
            }
        }

        return true;
        //checkCube
        //{
        //    float dHalf = sr.v2.x / 2f;
        //    if (sr.v1.x + dHalf < camWorldPos.x || sr.v1.x - dHalf > camWorldPos.x)
        //        return false;
        //    dHalf = sr.v2.y / 2f;
        //    if (sr.v1.y + dHalf < camWorldPos.y || sr.v1.y - dHalf > camWorldPos.y)
        //        return false;
        //    dHalf = sr.v2.z / 2f;
        //    if (sr.v1.z + dHalf < camWorldPos.z || sr.v1.z - dHalf > camWorldPos.z)
        //        return false;
        //}
    }

    public static int Count()
    {
        return WaterList.Count;
    }
    static Vector3[] temp = new Vector3[8];
    public static int CameraCull(Frustum mainCameraFrustum)
    {
        int count = 0;
        
        for(int i=0;i< WaterList.Count;i++)
        {
            Water w = WaterList[i];
            if (w.customMesh && w.bActiveFollowCam)
            {
                count++;
                w.visiable = true;
                continue;
            }

            if(w.renderMesh==null)
            {
                continue;
            }
            Bounds bound = w.renderMesh.bounds;
            Vector3 min = bound.min;
            Vector3 max = bound.max;
            temp[0] = bound.min;
            temp[1] = bound.max;
            temp[2].Set(min.x, max.y, min.z);
            temp[3].Set(min.x, min.y, max.z);
            temp[4].Set(min.x, max.y, max.z);

            temp[5].Set(max.x, max.y, min.z);
            temp[6].Set(max.x, min.y, max.z);
            temp[7].Set(max.x, min.y, min.z);

            for (int j = 0; j < 8; j++)
            {
                temp[j] = w.transform.TransformPoint(temp[j]);
                
            }
            if(mainCameraFrustum.IsVisiable(temp))
            {
                count++;
                w.visiable = true;
            }
            else
            {
                w.visiable = false;
            }
        }
        return count;
    }

    static Matrix4x4 matrix = new Matrix4x4();
    static Vector4 param = Vector4.zero;
    static Vector4 wave = Vector4.zero;
    public static void DrawAllES20(Material water_mat, Texture _Sky, Color ambient)
    {
        Vector3 lightdir = SceneRenderSetting._Setting.MainLightDirection;

        Quaternion q_X = Quaternion.AngleAxis(lightdir.x, Vector3.right);
        Quaternion q_Y = Quaternion.AngleAxis(lightdir.y, Vector3.up);
        Quaternion qRotate = q_Y * q_X;

        Vector3 shadow_space_dir = qRotate * (-Vector3.forward);

        for (int i = 0; i < WaterList.Count; i++)
        {
            Water w = WaterList[i];
            if (w == null)
                continue;
            if (!w.enabled)
                continue;
            if (!w.visiable)
            {
                continue;
            }
            matrix.SetTRS(w.bActiveFollowCam ? w.followPos : w.transform.position
                        , w.bActiveFollowCam && w.followMode == FollowMode.RotAndPos ? Quaternion.Euler(w.waterEuler) : w.transform.rotation
                        , w.transform.lossyScale);
            //matrix.SetTRS(w.transform.position, w.transform.rotation, w.transform.lossyScale);

            Color c = w.color;
            c.a = w.water_fog_distance;
            Color reflect_color = w.reflectColor;
            reflect_color.a = w.refract_scale;
            water_mat.SetColor("_Color", c);
            water_mat.SetColor("_DeepColor", w.deepcolor);
            water_mat.SetColor("_AmbientColor", reflect_color);

            water_mat.SetTexture("_Sky", _Sky);
            water_mat.SetTexture("_NormalTex", w._NormalTex);
            water_mat.SetMatrix("_WorldMatrix", matrix);

            //Vector4 wave = new Vector4(w.first_wave_scale, w.second_wave_scale, w.first_wave_speed, w.second_wave_speed);
            wave.x = w.first_wave_scale;
            wave.y = w.second_wave_scale;
            wave.z = w.first_wave_speed;
            wave.w = w.second_wave_speed;

            water_mat.SetVector("_WaterWaveScale", wave);

            Vector4 _WaveDir = Vector4.zero;
            _WaveDir.x = Mathf.Sin(w.wave_dir.x);
            _WaveDir.y = Mathf.Cos(w.wave_dir.x);
            _WaveDir.z = Mathf.Sin(w.wave_dir.y);
            _WaveDir.w = Mathf.Cos(w.wave_dir.y);

            water_mat.SetVector("_WaveDir", _WaveDir);
            water_mat.SetVector("_MainLightColor", SceneRenderSetting._Setting.MainLightColor * SceneRenderSetting._Setting.MainLightColorScale);
            water_mat.SetVector("_MainLightDir", shadow_space_dir);

            //draw water fog
            param.x = SceneRenderSetting._Setting.Fog_Start;
            param.y = SceneRenderSetting._Setting.Fog_End;
            param.z = SceneRenderSetting._Setting.Fog_Attenuation;
            water_mat.SetVector("_FogParam", param);
            water_mat.SetVector("_FogColor", SceneRenderSetting._Setting.Fog_Color);

            //draw water refract and reflect


            if (w.use_mesh_normal)
            {
                water_mat.SetPass(3);
            }
            else
            {
                water_mat.SetPass(2);
            }

            Graphics.DrawMeshNow(w.renderMesh, matrix, 0);

        }
    }
    private int m_water_level = 0;
    public static void DrawAll(Material water_mat, RenderTexture _Depth, Texture _Sky, RenderTexture _LastFrame, Vector4 _FarCorner,Matrix4x4 invViewMatrix,Color ambient, RenderBuffer currentColorBuffer,RenderBuffer currentDepthBuffer, Camera maincamera, int water_level)
    {
        if (Application.isEditor)
        {
            _FarCorner.w = -1;
        }
        else
        {
            _FarCorner.w = 1.0f;
        }
        Vector3 lightdir = SceneRenderSetting._Setting.MainLightDirection;

        Quaternion q_X = Quaternion.AngleAxis(lightdir.x, Vector3.right);
        Quaternion q_Y = Quaternion.AngleAxis(lightdir.y, Vector3.up);
        Quaternion qRotate = q_Y * q_X;

        Vector3 shadow_space_dir = qRotate * (-Vector3.forward);

        for (int i = 0; i < WaterList.Count; i++)
        {
            Water w = WaterList[i];
            if (w == null)
                continue;
            if (!w.enabled)
                continue;
            if(!w.visiable)
            {
                continue;
            }
            w.m_water_level = water_level;
            if(w.customMesh != w.lastMeshSet)
            {
                w.lastMeshSet = w.customMesh;
                if(w.customMesh)
                {
                    w.CreateCustomMesh();
                    w.Reset();
                }
            }
            bool noReflShield = w.CanRefl();
            if (w.enableRefl && w.m_water_level >= 2 && noReflShield)
            {
                Shader.SetGlobalVector("MainColor", SceneRenderSetting._Setting.MainLightColor * SceneRenderSetting._Setting.MainLightColorScale);
                Shader.SetGlobalVector("AmbientColor", SceneRenderSetting._Setting.AmbientColor * SceneRenderSetting._Setting.AmbientColorScale);
                Vector4 MainLightDir_WorldSpace = RenderPipeline._instance.shadowCameraNear.transform.TransformDirection(Vector3.forward);
                MainLightDir_WorldSpace.w = SceneRenderSetting._Setting.MainLightBias.x;

                Shader.SetGlobalVector("MainDir", MainLightDir_WorldSpace);
                Shader.SetGlobalVector("FogColor", SceneRenderSetting._Setting.Fog_Color);
                Vector4 fog_param = new Vector4(SceneRenderSetting._Setting.Fog_Start, SceneRenderSetting._Setting.Fog_End, SceneRenderSetting._Setting.Fog_Attenuation, 0);
                Shader.SetGlobalVector("FogParam", fog_param);

                w.RenderReflection(_FarCorner, invViewMatrix);
                Graphics.SetRenderTarget(currentColorBuffer, currentDepthBuffer);
            }

            matrix.SetTRS(w.bActiveFollowCam ? w.followPos : w.transform.position
                        , w.bActiveFollowCam && w.followMode == FollowMode.RotAndPos ? Quaternion.Euler(w.waterEuler) : w.transform.rotation
                        , w.bActiveFollowCam && w.followMode == FollowMode.RotAndPos ? Vector3.one : w.transform.lossyScale);

            Color c = w.color;
            c.a = w.water_fog_distance;
            Color reflect_color = w.reflectColor;
            reflect_color.a = w.refract_scale;
            if (water_mat == null)
            {
                Debug.LogError("WaterMat miss");
                return;
            }
            water_mat.SetColor("_Color", c);
            water_mat.SetColor("_DeepColor", w.deepcolor);
            water_mat.SetColor("_AmbientColor", reflect_color* SceneRenderSetting._Setting.AmbientColor * SceneRenderSetting._Setting.AmbientColorScale);
            water_mat.SetTexture("_Depth", _Depth);
            water_mat.SetTexture("_Sky", _Sky);
            if(w.enableRefl && noReflShield)
                water_mat.SetTexture("_ReflectionTex", RenderPipeline._instance.GetWaterReflectTex());
            water_mat.SetTexture("_LastFrame", _LastFrame);
            water_mat.SetVector("_FarCorner", _FarCorner);
            water_mat.SetTexture("_NormalTex", w._NormalTex);
            water_mat.SetMatrix("_WorldMatrix", matrix);
            Vector4 world_up = maincamera.transform.InverseTransformDirection(Vector3.up);
            world_up.w = maincamera.transform.position.y;
            water_mat.SetVector("_WorldUp", world_up);

            //wave
            if(w.enableWave && w.m_water_level >= 1)
            {
                w._TimeCalc += Time.deltaTime * w._GSpeed;
                if (w._TimeCalc.x > 6.283185306f)
                    w._TimeCalc.x -= 6.283185306f;
                if (w._TimeCalc.y > 6.283185306f)
                    w._TimeCalc.y -= 6.283185306f;
                if (w._TimeCalc.z > 6.283185306f)
                    w._TimeCalc.z -= 6.283185306f;
                if (w._TimeCalc.w > 6.283185306f)
                    w._TimeCalc.w -= 6.283185306f;

                water_mat.SetFloat("_GerstnerIntensity", w._GerstnerIntensity);
                water_mat.SetVector("_GSpeed", w._GSpeed);
                water_mat.SetVector("_GAmplitude", w._GAmplitude);
                water_mat.SetVector("_GFrequency", w._GFrequency);
                water_mat.SetVector("_GSteepness", w._GSteepness);
                water_mat.SetVector("_GDirectionAB", w._GDirectionAB);
                water_mat.SetVector("_GDirectionCD", w._GDirectionCD);
                water_mat.SetVector("_waterScale", w.transform.localScale);
                water_mat.SetVector("_TimeCalc", w._TimeCalc);
                water_mat.SetVector("_OceanParams0", w._OceanParams0);
                water_mat.SetFloat("_AnimGenParams", w._AnimGenParams);
                water_mat.SetVector("_FlowDir", w._FlowDir);

                water_mat.SetVector("_planeInfo", new Vector4(w.worldPlaneCenter.x, w.worldPlaneCenter.y,
                    w.worldPlaneCenter.z, w.validSize * 68f / 70f)); 
            }
            
            //Vector4 wave = new Vector4(w.first_wave_scale, w.second_wave_scale, w.first_wave_speed, w.second_wave_speed);
            wave.x = w.first_wave_scale;
            wave.y = w.second_wave_scale;
            wave.z = w.first_wave_speed;
            wave.w = w.second_wave_speed;

            water_mat.SetVector("_WaterWaveScale", wave);
            //Vector4 param = new Vector4();
            param.x = w.water_fog_y_depth;
            param.y = w.water_fog_distance;
            param.z = w.water_refract_y_depth;
            param.w = w.frenel_scale;
            water_mat.SetVector("_FogParam", param);

            Vector4 _WaveDir = Vector4.zero;
            _WaveDir.x = Mathf.Sin(w.wave_dir.x);
            _WaveDir.y = Mathf.Cos(w.wave_dir.x);
            _WaveDir.z = Mathf.Sin(w.wave_dir.y);
            _WaveDir.w = Mathf.Cos(w.wave_dir.y);

            water_mat.SetVector("_WaveDir", _WaveDir);
            water_mat.SetVector("_MainLightColor", SceneRenderSetting._Setting.MainLightColor* SceneRenderSetting._Setting.MainLightColorScale);



            water_mat.SetVector("_MainLightDir", shadow_space_dir);
            float y = maincamera.nearClipPlane * Mathf.Tan(maincamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            float InSubWater = 0;
            if (maincamera.transform.position.y < w.transform.position.y + y)
            {//水下       
                InSubWater = 1;
            }
            water_mat.SetFloat("_InSubWater", InSubWater);
            
            #region Set Water Pass
            if (w.use_mesh_normal && w.customMesh)
            {
                if (w.enableRefl && noReflShield && w.m_water_level >= 2)
                {
                    if(w.enableWave && w.m_water_level >= 1)
                    {
                        water_mat.SetPass(6);
                    }
                    else
                    {
                        water_mat.SetPass(5);
                    }
                }
                else
                {
                    if (w.enableWave && w.m_water_level >= 1)
                    {
                        water_mat.SetPass(7);
                    }
                    else
                    {
                        water_mat.SetPass(1);
                    }
                }
            }
            else
            {
                if (w.enableRefl && noReflShield && w.m_water_level >= 2)
                {
                    if (w.enableWave && w.m_water_level >= 1)
                    {
                        water_mat.SetPass(8);
                    }
                    else
                    {
                        water_mat.SetPass(4);
                    }
                }
                else
                {
                    if (w.enableWave && w.m_water_level >= 1)
                    {
                        water_mat.SetPass(9);
                    }
                    else
                    {
                        water_mat.SetPass(0);
                    }
                }
            }
            #endregion

            Graphics.DrawMeshNow(w.customMesh ? w.m_customMesh : w.renderMesh, matrix, 0);

        }
    }
	public static bool IsContainPoint(Transform bounds,Vector3 point)
	{
		Vector3 localpoint = bounds.InverseTransformPoint (point);

		if(Mathf.Abs(localpoint.x)<0.5f&&Mathf.Abs(localpoint.y)<0.5f&&Mathf.Abs(localpoint.z)<0.5f)
		{ 
			return true;
		}
		return false;
	}
    public static void DrawSubWater(Material mat_ScreenWater,Mesh Quad, RenderTexture _Depth, Vector4 _FarCorner, RenderTexture target,Matrix4x4 ViewToWorld,Camera maincamera)
    {
        for (int i = 0; i < WaterList.Count; i++)
        {
            Water w = WaterList[i];
            if (w.enableSubWater)
            {
                if (w == null)
                    continue;
                if (!w.enabled)
                    continue;
                if (!w.visiable)
                {
                    continue;
                }
                if (Application.isPlaying)
                {
                    /*Vector3 nearveiwpoint = new Vector3 (0, 0, maincamera.nearClipPlane);
					Vector3 nearworldpoint=maincamera.transform.TransformPoint (nearveiwpoint);	
					nearworldpoint.y = w.transform.position.y;
					Vector3 nearVpoint=maincamera.transform.InverseTransformPoint (nearworldpoint);	
					float nearY = nearVpoint.y;
					Vector3 farVpoint = Vector3.zero;
					for(int j=1;j<50;j++)
					{
						float len=maincamera.farClipPlane/49f *i;
						nearveiwpoint = new Vector3 (0, 0, len);
					    nearworldpoint=maincamera.transform.TransformPoint (nearveiwpoint);	
						nearworldpoint.y = w.transform.position.y;
						bool b = IsContainPoint (w.transform, nearworldpoint);
						if (b)
						{
							farVpoint = maincamera.transform.InverseTransformPoint (nearworldpoint);	
						} 
						else 
						{
							break;
						}只能通过摄像机原点往前射一条射线 ，然后检测改点队友水的y轴点，是否在水圈内和是否在摄像机可视范围内，然后判断水面的上下可视点、
					}
					float farY = farVpoint.y;
					Debug.Log (nearY + "    " + farY);*/
                }
                float y = maincamera.nearClipPlane * Mathf.Tan(maincamera.fieldOfView * 0.5f * Mathf.Deg2Rad);//camera h/2
                int tatalInwater = 0;
                float uvheight = 0f;
                if (maincamera.transform.position.y > w.transform.position.y + y + 0.2f) {//相机完全在水上不渲染
                    continue;
                }
                else if (maincamera.transform.position.y < w.transform.position.y - y) {//camera完全水下 只渲染水下效果
                    tatalInwater = 1;
                }
                else if (maincamera.transform.position.y < w.transform.position.y + 0.2f) {//水面交叉 camera在下半截
                    tatalInwater = 2;//offset
                }
                else
                {//水面交叉 camera在上半截
                    tatalInwater = 3;
                    if (Application.isPlaying)
                    {
                        Vector3 mainchar = SceneRenderSetting._Setting.transform.position;// sdGlobalDatabase.Instance.mainChar.transform.position;
                        Vector3 maincharwaterpoint = new Vector3(mainchar.x, w.transform.position.y, mainchar.z);
                        Vector3 viewcharpos = maincamera.transform.InverseTransformPoint(maincharwaterpoint);
                        uvheight = viewcharpos.y;
                    }

                }
                //compute water far and near uvheight

                matrix.SetTRS(w.transform.position, w.transform.rotation, w.transform.lossyScale);
                mat_ScreenWater.SetVector("farCorner", _FarCorner);
                mat_ScreenWater.SetMatrix("_WorldMatrix", matrix);
                mat_ScreenWater.SetMatrix("ViewToWorld", ViewToWorld);
                //  mat_ScreenWater.SetVector("FogColor", SceneRenderSetting._Setting.Fog_Color);
                //     mat_ScreenWater.SetVector("invViewport_Radius", invViewport);
                //  mat_ScreenWater.SetVector("FogDistance", new Vector4(SceneRenderSetting._Setting.Fog_Start, SceneRenderSetting._Setting.Fog_End, SceneRenderSetting._Setting.Fog_Attenuation, SceneRenderSetting._Setting.Fog_DensityMount));
                mat_ScreenWater.SetTexture("_Depth", _Depth);
                mat_ScreenWater.SetTexture("_MainTex", target);

                if (w.enableCausitic)
                {
                    int idx = (int)(Time.time * 8);//每秒切换8次
                    Texture2D _CausticsTex = null;
                    if (w._CausticsTex != null && w._CausticsTex.Length > 0)
                        _CausticsTex = w._CausticsTex[idx % w._CausticsTex.Length];
                    mat_ScreenWater.SetTexture("_CausticsTex", _CausticsTex);
                    mat_ScreenWater.SetFloat("CausticsLerp", w.CausticsLerp);
                    mat_ScreenWater.EnableKeyword("USE_Caustics");
                }
                else
                {
                    mat_ScreenWater.SetTexture("_CausticsTex", null);
                    mat_ScreenWater.DisableKeyword("USE_Caustics");
                }

                mat_ScreenWater.SetTexture("_NormalTex", w._NormalTex);
                mat_ScreenWater.SetFloat("refract_scale", w.ScreenDistort);
                mat_ScreenWater.SetFloat("WaterHeight", w.transform.position.y);

                Vector4 Para = Vector4.zero;
                Para.x = w.ScreenWaterMaxDis;
                Para.y = w.ScreenWaterMinDis;
                Para.z = tatalInwater;
                Para.w = uvheight;
                mat_ScreenWater.SetVector("ScreenPara", Para);
                mat_ScreenWater.SetColor("_Color", w.color);
                mat_ScreenWater.SetColor("_DeepColor", w.deepcolor);
                if (w.subwaterFogNoise != null)
                {
                    Vector3 viewspeed = maincamera.transform.InverseTransformDirection(w.subwaterPlaneFogSpeed);
                    mat_ScreenWater.SetVector("PlaneFogSpeed", viewspeed);
                    mat_ScreenWater.SetTexture("_FogNoiseTex", w.subwaterFogNoise);
                    mat_ScreenWater.EnableKeyword("USE_Dynamic_Fog");
                }
                else
                {
                    mat_ScreenWater.DisableKeyword("USE_Dynamic_Fog");
                }
                if (maincamera.transform.position.y> w.transform.position.y + y)
                {//水上

                }


                mat_ScreenWater.SetPass(0);

                Graphics.DrawMeshNow(Quad, Vector3.zero, Quaternion.identity, 0);
            }
        }
    }
    public static void DrawSubWaterES20(Material mat_ScreenWater, Mesh Quad, RenderTexture _Depth, Vector4 _FarCorner, RenderTexture target, Matrix4x4 ViewToWorld, Camera maincamera)
    {
        for (int i = 0; i < WaterList.Count; i++)
        {
            Water w = WaterList[i];
            if (w.enableSubWater)
            {
                if (w == null)
                    continue;
                if (!w.enabled)
                    continue;
                if (!w.visiable)
                {
                    continue;
                }
                float y = maincamera.nearClipPlane * Mathf.Tan(maincamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
                if (maincamera.transform.position.y > w.transform.position.y + y)
                {//水上
                    continue;
                }
                int tatalInwater = 0;
                if (maincamera.transform.position.y < w.transform.position.y - y)
                {//完全水下
                    tatalInwater = 1;
                }
                matrix.SetTRS(w.transform.position, w.transform.rotation, w.transform.lossyScale);
                mat_ScreenWater.SetVector("farCorner", _FarCorner);
                mat_ScreenWater.SetMatrix("_WorldMatrix", matrix);
                mat_ScreenWater.SetMatrix("ViewToWorld", ViewToWorld);
                //  mat_ScreenWater.SetVector("FogColor", SceneRenderSetting._Setting.Fog_Color);
                //     mat_ScreenWater.SetVector("invViewport_Radius", invViewport);
                //  mat_ScreenWater.SetVector("FogDistance", new Vector4(SceneRenderSetting._Setting.Fog_Start, SceneRenderSetting._Setting.Fog_End, SceneRenderSetting._Setting.Fog_Attenuation, SceneRenderSetting._Setting.Fog_DensityMount));
                mat_ScreenWater.SetTexture("_Depth", _Depth);
                mat_ScreenWater.SetTexture("_MainTex", target);
              //  mat_ScreenWater.SetTexture("_DiffuseTex", w._DiffuseTex);
                mat_ScreenWater.SetTexture("_NormalTex", w._NormalTex);
              //  mat_ScreenWater.SetFloat("refract_scale", SceneRenderSetting._Setting.ScreenDistort);
                mat_ScreenWater.SetFloat("WaterHeight", w.transform.position.y);
             //   mat_ScreenWater.SetFloat("tatalInwater", tatalInwater);
            //    mat_ScreenWater.SetColor("_Color", w.color);
             //   mat_ScreenWater.SetColor("_DeepColor", w.deepcolor);



                if (maincamera.transform.position.y > w.transform.position.y + y)
                {//水上

                }


                mat_ScreenWater.SetPass(1);

                Graphics.DrawMeshNow(Quad, Vector3.zero, Quaternion.identity, 0);
            }
        }
    }

    #region ReflectTest
    private static bool s_InsideWater = false;
    private Dictionary<Camera, Camera> m_ReflectionCameras = new Dictionary<Camera, Camera>();
    public float reflectionPlaneOffset = 0.07f;
    public bool customWaterNormal = false;
    public Vector3 waterNormal = Vector3.up;
    static float[] layer_cull_distance = new float[32];
    public void RenderReflection(Vector4 FarCorner,Matrix4x4 invViewMatrix)
    {
        RenderTexture m_ReflectionTexture = RenderPipeline._instance.GetWaterReflectTex();
        if (m_ReflectionTexture == null)
            return;

        Camera currentCamera = GetMainCam();
        if (!currentCamera)
        {
            return;
        }
        if (s_InsideWater)
            return;

        s_InsideWater = true;

        Camera reflectionCamera;
        m_ReflectionCameras.TryGetValue(currentCamera, out reflectionCamera);
        if (!reflectionCamera)
        {
            GameObject go = new GameObject("Water Reflect Camera id:" + GetInstanceID(), typeof(Camera));
            reflectionCamera = go.GetComponent<Camera>();
            reflectionCamera.enabled = false;
            go.hideFlags = HideFlags.DontSave;
            //go.hideFlags = HideFlags.HideAndDontSave;
            m_ReflectionCameras[currentCamera] = reflectionCamera;

            reflectionCamera.depthTextureMode = DepthTextureMode.None;
            reflectionCamera.clearFlags = CameraClearFlags.SolidColor;
            reflectionCamera.backgroundColor = Color.black;
            reflectionCamera.renderingPath = RenderingPath.Forward;

            reflectionCamera.farClipPlane = currentCamera.farClipPlane + 30f;
            reflectionCamera.nearClipPlane = currentCamera.nearClipPlane;
            reflectionCamera.orthographic = currentCamera.orthographic;
            reflectionCamera.fieldOfView = currentCamera.fieldOfView;
            reflectionCamera.aspect = currentCamera.aspect;
            reflectionCamera.orthographicSize = currentCamera.orthographicSize;
        }
        reflectionCamera.targetTexture = m_ReflectionTexture;
        
        Vector3 pos = transform.position;
        Vector3 normal = customWaterNormal ? waterNormal.normalized : Vector3.up;

        float d = -Vector3.Dot(normal, pos) - reflectionPlaneOffset;
        Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);

        Matrix4x4 reflection = Matrix4x4.zero;
        CalculateReflectMatrix(ref reflection, reflectionPlane);
        Vector3 oldpos = currentCamera.transform.position;
        Vector3 newpos = reflection.MultiplyPoint(oldpos);
        reflectionCamera.worldToCameraMatrix = currentCamera.worldToCameraMatrix * reflection;

        reflectionCamera.layerCullSpherical = true;

        layer_cull_distance[0] = currentCamera.farClipPlane + 10f;
        layer_cull_distance[4] = currentCamera.farClipPlane + 10f;
        layer_cull_distance[15] = currentCamera.farClipPlane + 10f;
        reflectionCamera.layerCullDistances = layer_cull_distance;

        Vector4 clipPlane = CameraSpacePlane(reflectionCamera, pos, normal, 1.0f);
        
        reflectionCamera.projectionMatrix = currentCamera.CalculateObliqueMatrix(clipPlane);

        reflectionCamera.cullingMask = 1 << LayerMask.NameToLayer("Water");
        reflectionCamera.targetTexture = m_ReflectionTexture;

        bool oldCulling = GL.invertCulling;
        GL.invertCulling = !oldCulling;
        reflectionCamera.transform.position = newpos;
        Vector3 euler = currentCamera.transform.eulerAngles;
        reflectionCamera.transform.eulerAngles = new Vector3(-euler.x, euler.y, euler.z); 
        reflectionCamera.RenderWithShader(RenderPipeline._instance.WaterReflectShader, "RenderType");
        reflectionCamera.transform.position = oldpos;
        GL.invertCulling = oldCulling;

        Graphics.SetRenderTarget(m_ReflectionTexture);
        RenderPipeline._instance.DrawSky(SceneRenderSetting._Setting.SkyTexture, FarCorner, invViewMatrix, SceneRenderSetting._Setting.AmbientColor * SceneRenderSetting._Setting.AmbientColorScale, true);

        s_InsideWater = false;
    }


    void CreateCustomMesh()
    {
        if(!bActiveFollowCam || followMode == FollowMode.Sampler || followMode == FollowMode.Pos)
        {
            float xScale = renderMesh.bounds.size.x / column;
            float yScale = renderMesh.bounds.size.y / row;
            Vector3 offset = new Vector3(-column * xScale / 2f, -row * yScale / 2f) + renderMesh.bounds.center;
            int numberOfVertices = (row + 1) * (column + 1);
            int numberOfIndex = row * column * 6;
            Vector3[] vertices = new Vector3[numberOfVertices];
            int[] indices = new int[numberOfIndex];
            m_customMesh = GetMesh(xScale, yScale, offset, vertices, indices, dirMode);
            
        }else if(followMode == FollowMode.RotAndPos)
        {
            float xScale = validSize / column;
            float yScale = validSize / row;
            Vector3 offset = new Vector3(-validSize / 2f, -validSize / 2f);
            int numberOfVertices = (row + 1) * (column + 1);
            int numberOfIndex = row * column * 6;
            Vector3[] vertices = new Vector3[numberOfVertices];
            int[] indices = new int[numberOfIndex];
            m_customMesh = GetMesh(xScale, yScale, offset, vertices, indices, dirMode);
        }
    }

    private Mesh GetMesh(float xScale, float yScale, Vector3 offset, Vector3[] vertices, int[] indices, DirectMode dir)
    {
        for (int r = 0; r < row; r++)
        {
            for (int c = 0; c < column; c++)
            {
                //leftTop
                if (c == 0)
                {
                    vertices[r * (row + 1) + c] = new Vector3(0, r * yScale, 0) + offset;
                }
                //rightTop
                vertices[r * (row + 1) + c + 1] = new Vector3((c + 1) * xScale, r * yScale, 0) + offset;
                //leftBottom
                if (r == row - 1 && c == 0)
                {
                    vertices[(r + 1) * (row + 1) + c] = new Vector3(0, (r + 1) * yScale, 0) + offset;
                }
                //rightBottom
                if (r == row - 1)
                {
                    vertices[(r + 1) * (row + 1) + c + 1] = new Vector3((c + 1) * xScale, (r + 1) * yScale, 0) + offset;
                }
                switch (dir)
                {
                    case DirectMode.Quad:
                        //quad mode
                        indices[6 * (r * column + c)] = r * (row + 1) + c;
                        indices[6 * (r * column + c) + 1] = (r + 1) * (row + 1) + c;
                        indices[6 * (r * column + c) + 2] = r * (row + 1) + c + 1;

                        indices[6 * (r * column + c) + 3] = r * (row + 1) + c + 1;
                        indices[6 * (r * column + c) + 4] = (r + 1) * (row + 1) + c;
                        indices[6 * (r * column + c) + 5] = (r + 1) * (row + 1) + c + 1;
                        break;
                    case DirectMode.Plane:
                        //plane mode
                        indices[6 * (r * column + c)] = r * (row + 1) + c;
                        indices[6 * (r * column + c) + 1] = r * (row + 1) + c + 1;
                        indices[6 * (r * column + c) + 2] = (r + 1) * (row + 1) + c;

                        indices[6 * (r * column + c) + 3] = r * (row + 1) + c + 1;
                        indices[6 * (r * column + c) + 4] = (r + 1) * (row + 1) + c + 1;
                        indices[6 * (r * column + c) + 5] = (r + 1) * (row + 1) + c;
                        break;
                }
            }
        }

        //圆形扩展
        if (customMesh && bActiveFollowCam && followMode == FollowMode.RotAndPos && extendEdge)
        {
            for (int rpIndex = 0; rpIndex < row + 1; rpIndex++)
            {
                for (int cpIndex = 0; cpIndex < column + 1; cpIndex++)
                {
                    if (rpIndex == 0 || rpIndex == row || cpIndex == 0 || cpIndex == column)
                    {
                        int seq = rpIndex * (column + 1) + cpIndex;
                        Vector3 oriPos = vertices[seq];
                        Vector3 extDir = Vector3.Normalize(oriPos - Vector3.zero);
                        vertices[seq] = extDir * noWaveSize;
                    }
                }
            }
        }

        Mesh m_mesh = new Mesh();
        m_mesh.vertices = vertices;
        m_mesh.triangles = indices;
        return m_mesh;
    }

    private void ClearReflCam()
    {
        foreach (var v in m_ReflectionCameras)
        {
            if(v.Value.gameObject != null)
                DestroyImmediate(v.Value.gameObject);
        }
        m_ReflectionCameras.Clear();
    }

    Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
    {
        Vector3 offsetPos = pos + normal * reflectionPlaneOffset;
        Matrix4x4 m = cam.worldToCameraMatrix;
        Vector3 cpos = m.MultiplyPoint(offsetPos);
        Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
        return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
    }

    static void CalculateReflectMatrix(ref Matrix4x4 reflectMatrix, Vector4 plane)
    {
        reflectMatrix.m00 = (1F - 2F * plane[0] * plane[0]);
        reflectMatrix.m01 = (-2F * plane[0] * plane[1]);
        reflectMatrix.m02 = (-2F * plane[0] * plane[2]);
        reflectMatrix.m03 = (-2F * plane[0] * plane[3]);

        reflectMatrix.m10 = (-2F * plane[1] * plane[0]);
        reflectMatrix.m11 = (1F - 2F * plane[1] * plane[1]);
        reflectMatrix.m12 = (-2F * plane[1] * plane[2]);
        reflectMatrix.m13 = (-2F * plane[1] * plane[3]);

        reflectMatrix.m20 = (-2F * plane[2] * plane[0]);
        reflectMatrix.m21 = (-2F * plane[2] * plane[1]);
        reflectMatrix.m22 = (1F - 2F * plane[2] * plane[2]);
        reflectMatrix.m23 = (-2F * plane[2] * plane[3]);

        reflectMatrix.m30 = 0F;
        reflectMatrix.m31 = 0F;
        reflectMatrix.m32 = 0F;
        reflectMatrix.m33 = 1F;
    }

    #region syncMove
    void Reset()
    {
        destTrans = SceneRenderSetting._Setting.transform;
        if (destTrans == null)
            return;
        followPos = transform.position;
        lastPos = destTrans.position;
        lastFloatPos = destTrans.position;
        lastEuler = destTrans.eulerAngles;
        waterEuler = transform.eulerAngles;
        destCam = GetMainCam();
        bLastActive = bActiveFollowCam;
        meshDirty = true;
    }

    private Camera mainCam = null;
    private Camera GetMainCam()
    {
        if (mainCam == null)
            mainCam = RenderPipeline._instance.GetComponent<Camera>();
        return mainCam;
    }
    
    void LateUpdate()
    {
        if (!customMesh)
            return;

        if (bActiveFollowCam != bLastActive)
        {
            bLastActive = bActiveFollowCam;
            if(bActiveFollowCam)
            {
                Reset();
                CreateCustomMesh();
            }
        }

        if(followMode != lastFollowMode)
        {
            lastFollowMode = followMode;
            CreateCustomMesh();
            Reset();
        }

        if (!bActiveFollowCam)
            return;
        
        if (destTrans == null)
            return;
        
        if (CheckExcute())
        {
            if(followMode == FollowMode.RotAndPos)
            {
                lastFloatPos = destTrans.position;
                lastEuler = destTrans.eulerAngles;
                RepositionRotAndPos();
            }
            else
            {
                if (meshDirty)
                    GetCellSize();

                Vector3 offs = GetOffset();
                if (offs != Vector3.zero)
                    followPos += offs;
            }
        }
    }

    /// <summary>
    /// 跟随模式：sampler
    /// </summary>
    Vector3 GetOffset()
    {
        Vector3 offs = Vector3.zero;
        Vector3 destOffs = destTrans.position - lastPos;
        lastFloatPos = destTrans.position;
        float complexor = 0f;
        int amount = 0;
        if (xMove)
        {
            complexor = Vector3.Dot(destOffs, transform.TransformDirection(Vector3.right));
            if(Mathf.Abs(complexor) >= m_cellSize.x)
            {
                amount = (int)(complexor / m_cellSize.x);
                offs.x = amount * m_cellSize.x;
            }
        }
        if(yMove)
        {
            complexor = Vector3.Dot(destOffs, transform.TransformDirection(Vector3.up));
            if (Mathf.Abs(complexor) >= m_cellSize.y)
            {
                amount = (int)(complexor / m_cellSize.y);
                offs.y = amount * m_cellSize.y;
            }
        }

        if(zMove)
        {
            offs.z = destOffs.z;
        }
        Vector3 worldOffs = transform.TransformDirection(offs);
        lastPos += worldOffs;
        return worldOffs;
    }
    
    /// <summary>
    /// 跟随模式：PosAndRot
    /// </summary>
    void RepositionRotAndPos()
    {
        if (destCam == null)
            return;
        //pos
        Ray _ray = destCam.ViewportPointToRay(new Vector2(0.5f, 0f));
        float dis = Vector3.Dot(followPos - _ray.origin, Vector3.up) / Vector3.Dot(_ray.direction, Vector3.up);
        float xScale = validSize / column;
        float yScale = validSize / row;
        Vector3 crossPoint = Vector3.zero;
        if (dis > 0)
        {
            crossPoint = dis * _ray.direction.normalized + _ray.origin;
        }
        else
        {
            crossPoint = destCam.transform.position;
            crossPoint.y = followPos.y;
        }

        Vector3 dir = destTrans.forward;
        dir.y = 0;
        Vector3 destPos = dir.normalized * validSize / 2f + crossPoint;

        destPos.x /= xScale;
        destPos.z /= yScale;
        destPos.x = Mathf.Floor(destPos.x);
        destPos.z = Mathf.Floor(destPos.z);
        destPos.x *= xScale;
        destPos.z *= yScale;

        followPos = destPos;
        //euler
        //waterEuler.y = destTrans.eulerAngles.y;
        //matrix    
        worldPlaneCenter = new Vector3(0,0, 0);// * validSize / 2f;
    }


    void GetCellSize()
    {
        m_cellSize =  new Vector2(renderMesh.bounds.size.x / column * transform.localScale.x
                        , renderMesh.bounds.size.y / row * transform.localScale.y);
        meshDirty = false;
    }

    bool CheckExcute()
    {
        if (followMode == FollowMode.RotAndPos)
        {
            //if (destTrans.position != lastFloatPos || destTrans.eulerAngles != lastEuler)
                return true;
        }
        else
        {
            if (lastFloatPos != destTrans.position)
                return true;
        }
        
        return false;
    }

    /// <summary>
    /// 获得Gerstner波浪位置
    /// </summary>
    /// <param name="worldPos">目标物体世界坐标</param>
    /// <returns>受波浪影响的坐标</returns>
    public void GetGerstnerWavePos(Vector3 worldPos, out Vector3 wavePos, out Vector3 waveNormal)
    {
        //获得经过物体世界坐标射线,垂直水平面的交点
        Vector3 planeNormal = transform.forward;
        if(dirMode == DirectMode.Plane)
        {
            planeNormal = -planeNormal;
        }
        Plane _p = new Plane(planeNormal, transform.position);
        
        float _height = -_p.GetDistanceToPoint(worldPos);

        Vector3 crossPoint = worldPos;
        crossPoint += _height * planeNormal;
        //如果没开浪，直接返回交点
        if (!enableWave)
        {
            wavePos = crossPoint;
            waveNormal = new Vector3(0f, 1f, 0f);
            return;
        }

        //计算位置偏移,法线
        Vector3 offset = Vector3.zero;
        Vector3 nrml = new Vector3(0f, 1f, 0f);
        Vector3 worldSpaceVertex = crossPoint;
        Vector3 vtxForAni = new Vector3(worldSpaceVertex.x, worldSpaceVertex.z, worldSpaceVertex.z);
        //float fWaveLerp = 1.0f;
        Vector3 localPos = matrix.MultiplyPoint3x4(crossPoint);
        Vector4 _planeInfo = new Vector4(worldPlaneCenter.x, worldPlaneCenter.y,
                    worldPlaneCenter.z, validSize * (column - 2f / column));
        float xDis = Mathf.Abs (_planeInfo.x -  localPos.x);
        float yDis = -(_planeInfo.y - localPos.y);
        float width = _planeInfo.w / 2.0f;
        float height = width;
        float xFactor = Mathf.Clamp01(1f - xDis / width);
        float yFactor = Mathf.Clamp01(1f - yDis / width);
        //fWaveLerp = Mathf.Clamp01(xFactor * yFactor * 4.0f);
        GerstnerWave(
            out offset, out nrml, vtxForAni,
            _GAmplitude,
            _GFrequency,
            _GSteepness,
            _GSpeed,
            _GDirectionAB,
            _GDirectionCD
        );
        //offset = Vector3.Lerp(new Vector3(0, 0, 0), offset, fWaveLerp);
        //nrml = Vector3.Lerp(new Vector3(0, 1, 0), nrml, fWaveLerp);
        wavePos = offset + worldSpaceVertex;
        waveNormal = nrml;
    }

    void GerstnerWave(out Vector3 offs, out Vector3 nrml,
        Vector3 tileableVtx, Vector4 amplitude,
        Vector4 frequency, Vector4 steepness,
        Vector4 speed, Vector4 directionAB, Vector4 directionCD)
    {

        offs = GerstnerOffset4(new Vector2(tileableVtx.x, tileableVtx.z), steepness, amplitude, frequency, speed, directionAB, directionCD);
        nrml = GerstnerNormal4(new Vector2(tileableVtx.x, tileableVtx.z) + new Vector2(offs.x, offs.z), amplitude, frequency, speed, directionAB, directionCD);
    }

    Vector3 GerstnerOffset4(Vector2 xzVtx, Vector4 steepness, Vector4 amp, Vector4 freq, Vector4 speed, Vector4 dirAB, Vector4 dirCD)
    {
        Vector3 offsets;
        
        Vector4 AB = new Vector4( steepness.x * amp.x * dirAB.x
                                , steepness.x * amp.x * dirAB.y
                                , steepness.y * amp.y * dirAB.z
                                , steepness.y * amp.y * dirAB.w);
        Vector4 CD = new Vector4( steepness.z * amp.z * dirCD.x
                                , steepness.z * amp.z * dirCD.y
                                , steepness.w * amp.w * dirCD.z
                                , steepness.w * amp.w * dirCD.w);

        Vector4 dotABCD = new Vector4(freq.x * Vector2.Dot(new Vector2(dirAB.x, dirAB.y), xzVtx)
                             , freq.y * Vector2.Dot(new Vector2(dirAB.z, dirAB.w), xzVtx)
                             , freq.z * Vector2.Dot(new Vector2(dirCD.x, dirCD.y), xzVtx)
                             , freq.w * Vector2.Dot(new Vector2(dirCD.z, dirCD.w), xzVtx));
        Vector4 COS = new Vector4( Mathf.Cos(dotABCD.x + _TimeCalc.x)
                                 , Mathf.Cos(dotABCD.y + _TimeCalc.y)
                                 , Mathf.Cos(dotABCD.z + _TimeCalc.z)
                                 , Mathf.Cos(dotABCD.w + _TimeCalc.w));
        Vector4 SIN = new Vector4( Mathf.Sin(dotABCD.x + _TimeCalc.x)
                                 , Mathf.Sin(dotABCD.y + _TimeCalc.y)
                                 , Mathf.Sin(dotABCD.z + _TimeCalc.z)
                                 , Mathf.Sin(dotABCD.w + _TimeCalc.w));

        offsets.x = Vector4.Dot(COS, new Vector4(AB.x, AB.z, CD.x, CD.z));
        offsets.z = Vector4.Dot(COS, new Vector4(AB.y, AB.w, CD.y, CD.w));
        offsets.y = Vector4.Dot(SIN, amp);

        return offsets;
    }

    Vector3 GerstnerNormal4(Vector2 xzVtx, Vector4 amp, Vector4 freq, Vector4 speed, Vector4 dirAB, Vector4 dirCD)
    {
        Vector3 nrml = new Vector3(0, 2.0f, 0);
        Vector4 AB = new Vector4( freq.x * amp.x * dirAB.x
                                , freq.x * amp.x * dirAB.y
                                , freq.y * amp.y * dirAB.z
                                , freq.y * amp.y * dirAB.w);
        Vector4 CD = new Vector4( freq.z * amp.z * dirCD.x
                                , freq.z * amp.z * dirCD.y
                                , freq.w * amp.w * dirCD.z
                                , freq.w * amp.w * dirCD.w);

        Vector4 dotABCD = new Vector4(freq.x * Vector2.Dot(new Vector2(dirAB.x, dirAB.y), xzVtx)
                            , freq.y * Vector2.Dot(new Vector2(dirAB.z, dirAB.w), xzVtx)
                            , freq.z * Vector2.Dot(new Vector2(dirCD.x, dirCD.y), xzVtx)
                            , freq.w * Vector2.Dot(new Vector2(dirCD.z, dirCD.w), xzVtx));
        Vector4 COS = new Vector4(Mathf.Cos(dotABCD.x + _TimeCalc.x)
                                 , Mathf.Cos(dotABCD.y + _TimeCalc.y)
                                 , Mathf.Cos(dotABCD.z + _TimeCalc.z)
                                 , Mathf.Cos(dotABCD.w + _TimeCalc.w));

        nrml.x -= Vector4.Dot(COS, new Vector4(AB.x, AB.z, CD.x, CD.z));
        nrml.z -= Vector4.Dot(COS, new Vector4(AB.y, AB.w, CD.y, CD.w));

        nrml.x *= _GerstnerIntensity;
        nrml.z *= _GerstnerIntensity;
        nrml = Vector3.Normalize(nrml);

        return nrml;
    }

    #endregion

    #endregion
    public static void DrawAllDepth(Material water_mat)
    {
        for (int i = 0; i < WaterList.Count; i++)
        {
            Water w = WaterList[i];
            if (w == null)
                continue;
            if (!w.enabled)
                continue;
            if (!w.visiable)
            {
                continue;
            }

            Color c = w.color;
            c.a = w.water_fog_distance;
            Color reflect_color = w.reflectColor;
            reflect_color.a = w.refract_scale;
            water_mat.SetColor("_Color", c);
            water_mat.SetColor("_DeepColor", w.deepcolor);
            //water_mat.SetVector("_FarCorner", _FarCorner);
            water_mat.SetTexture("_NormalTex", w._NormalTex);
            //water_mat.SetMatrix("_WorldMatrix", matrix);
            //water_mat.SetMatrix("_InvViewMatrix", invViewMatrix);
            Vector4 wave = new Vector4(w.first_wave_scale, w.second_wave_scale, w.first_wave_speed, w.second_wave_speed);
            water_mat.SetVector("_WaterWaveScale", wave);

            if (w.use_mesh_normal)
            {
                water_mat.SetPass(3);
            }
            else
            {
                water_mat.SetPass(2);
            }
            Graphics.DrawMeshNow(w.renderMesh, w.transform.localToWorldMatrix, 0);
        }
    }

    public bool debugMesh = false;
    void OnDrawGizmos()
    {
        //if (renderMesh == null)
        //{
        //    Gizmos.DrawCube(transform.position, Vector3.one);
        //}
        //else
        //{
        //    Matrix4x4 matrix = new Matrix4x4();
        //    matrix.SetTRS(transform.position, transform.rotation, transform.lossyScale);
        //    //Gizmos.Draw(transform.position, Radius * 0.5f);
        //    Graphics.DrawMeshNow(renderMesh, matrix, 0);
        //}
        Gizmos.DrawIcon(transform.position, "Water.png");
        
        if(customMesh && debugMesh)
        {
            //Gizmos.matrix = matrix;
            Gizmos.color = Color.red;
            Vector3[] v = m_customMesh.vertices;
            Vector3 tempPos;
            Vector3 tempNrml;

            for (int rpIndex = 0; rpIndex < row + 1; rpIndex++)
            {
                for (int cpIndex = 0; cpIndex < column + 1; cpIndex++)
                {
                    if (rpIndex == 0 ||  rpIndex == row || cpIndex == 0 || cpIndex == column)
                    {
                        Vector3 pos = matrix.MultiplyPoint3x4(v[rpIndex * (column + 1) + cpIndex]);
                        Gizmos.DrawCube(pos, new Vector3(2, 8, 2));
                    }
                }
            }
            Gizmos.DrawCube(matrix.MultiplyPoint3x4(Vector3.zero), new Vector3(4,16,4));

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    Vector3 a = matrix.MultiplyPoint3x4(v[i * (column + 1) + j]);
                    Vector3 b = matrix.MultiplyPoint3x4(v[i * (column + 1) + j + 1]);
                    Vector3 c = matrix.MultiplyPoint3x4(v[(i + 1) * (column + 1) + j]);
                    GetGerstnerWavePos(a, out tempPos, out tempNrml);
                    a = tempPos;
                    GetGerstnerWavePos(b, out tempPos, out tempNrml);
                    b = tempPos;
                    GetGerstnerWavePos(c, out tempPos, out tempNrml);
                    c = tempPos;

                    Gizmos.DrawLine(a, b);
                    Gizmos.DrawLine(a, c);
                }
            }
        }

        if(enableRefl && enableReflShield)
        {
            Color c = new Color(0f, 1f, 1f, 0.5f);
            Gizmos.color = c;
            //draw cylinder
            if (cylinderList != null)
            {
                Vector3 tempP;
                Vector3 cubeSize = new Vector3(2, 60, 2);
                for (int i = 0, iMax = cylinderList.Length; i < iMax; i++) 
                {
                    Vector3 param = cylinderList[i];
                    Vector3 pos = new Vector3(param.x, 0, param.y);
                    float radius = param.z;
                    

                    for (int h = 0; h < 16; h++)
                    {
                        tempP = pos + Quaternion.Euler(0, h * 22.5f, 0) * Vector3.forward * radius;
                        Gizmos.DrawCube(tempP, cubeSize);
                    }
                }
            }
            //draw sphere
            if (sphereList != null)
            {
                for (int i = 0, iMax = sphereList.Length; i < iMax; i++)
                {
                    Vector4 param = sphereList[i];
                    Gizmos.DrawSphere(new Vector3(param.x, param.y, param.z), param.w);
                }
            }
        }
    }
    public static void Print()
    {
        for (int i = 0; i < WaterList.Count; i++)
        {
            Water w = WaterList[i];
            if (w == null)
                continue;
            string print_str = w.name;
            print_str += "\ncolor=" + w.color;
            print_str += "\ncolor=" + w.deepcolor;
            print_str += "\ncolor=" + w.reflectColor;
            print_str += "\ncolor=" + w.renderMesh;
            print_str += "\ncolor=" + w._NormalTex;
            print_str += "\ncolor=" + w.refract_scale;
            print_str += "\ncolor=" + w.water_fog_distance;
            print_str += "\ncolor=" + w.water_fog_y_depth;
            print_str += "\ncolor=" + w.water_refract_y_depth;
            print_str += "\ncolor=" + w.frenel_scale;
            print_str += "\ncolor=" + w.first_wave_scale;
            print_str += "\ncolor=" + w.first_wave_speed;
            print_str += "\ncolor=" + w.second_wave_scale;
            print_str += "\ncolor=" + w.second_wave_speed;
            print_str += "\ncolor=" + w.wave_dir;
            Debug.Log(print_str);
        }
    }
}
