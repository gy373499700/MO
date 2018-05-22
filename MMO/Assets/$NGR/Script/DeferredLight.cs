using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum IllumType
{
    Diffuse  = 1,
    Specular = 2,
    Both     = 3
}

public class sdCubeMap
{

    public Vector3  camera_point;
    public float    camera_farPlane;
    public sdCubeMap()
    {
    }
    public void Render(Camera camera,Vector3 pos,float nearPlane,float farPlane,RenderTexture cube_texture,string layer,Shader depth_shader)
    {

        camera_point = pos;
        camera_farPlane = farPlane;
        camera.transform.position = camera_point;

        camera.SetReplacementShader(depth_shader, "RenderType");
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Color.white;
        camera.cullingMask = 1 << LayerMask.NameToLayer(layer);


        camera.nearClipPlane = 0.1f;
        camera.farClipPlane = farPlane;
        camera.fieldOfView = 90.0f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Color.white;
        Vector3[] dir = new Vector3[6];
        dir[0] = new  Vector3(0,0,1);
        dir[1] = new  Vector3(1,0,0);
        dir[2] = new  Vector3(0,0,-1);
        dir[3] = new  Vector3(-1,0,0);
        dir[4] = new  Vector3(0,1,0);
        dir[5] = new Vector3(0,-1,0);

        //for (int i = 0; i < 6; i++)
        //{
        //    if (i < 4)
        //    {
        //        camera.transform.rotation = Quaternion.AngleAxis(i*90.0f, Vector3.up);
        //    }
        //    else
        //    {
        //        camera.transform.rotation = Quaternion.FromToRotation(Vector3.forward, dir[i]);
        //    }
        //    if(cache[i]==null)
        //    {
        //        int depth = 0;
        //        if (i == 0)
        //        {
        //            depth = 24;
        //        }
        //        cache[i] = new RenderTexture(256, 256, depth, RenderTextureFormat.ARGB32);
        //        cache[i].filterMode = FilterMode.Point;
        //    }
        //    camera.SetTargetBuffers(cache[i].colorBuffer, cache[0].depthBuffer);
        //    camera.Render();
        //    
        //}

        camera.RenderToCubemap(cube_texture);

        //camera.targetTexture = null;
        camera.transform.position = Vector3.zero;

        //cube2sphere.SetTexture("CubeFace0", cache[0]);
        //cube2sphere.SetTexture("CubeFace1", cache[1]);
        //cube2sphere.SetTexture("CubeFace2", cache[2]);
        //cube2sphere.SetTexture("CubeFace3", cache[3]);
        //cube2sphere.SetTexture("CubeFace4", cache[4]);
        //cube2sphere.SetTexture("CubeFace5", cache[5]);
        //Graphics.Blit(null, sphere_tex, cube2sphere, 0);
    }
}

public class CubeMapInfo
{
    public DeferredLight light;
    public RenderTexture spheremap;
    public Vector3 LastUpdatePos = Vector3.one*10000.0f;
    public float LastRadius = 100000.0f;
    public bool NeedRefresh = false;
    public bool NeedDynamic = false;

    public void Clear()
    {
        if(light!=null)
        {
            if (light.cubemap)
            {
                light.cubemap = null;
            }
        }
        light = null;
        LastUpdatePos = Vector3.one * 10000.0f;
        LastRadius = 100000.0f;
    }
    public void SetLight(DeferredLight l)
    {
        if(light!=l)
        {
            LastUpdatePos = Vector3.one * 10000.0f;
            LastRadius = 100000.0f;
            NeedRefresh = true;
        }
        light = l;

    }
}

public class LightCache
{
    public int light_count;
    DeferredLight[] lights;
    float[] distance;
    CubeMapInfo[] CubeShadowDepthInfo;
    RenderTexture dynamic_cubemap;

    sdCubeMap temp_cube;
    public LightCache(int count)
    {
        light_count = count;
        lights = new DeferredLight[count];
        distance = new float[count];
        CubeShadowDepthInfo = new CubeMapInfo[count];
        for(int i = 0;i< CubeShadowDepthInfo.Length;i++)
        {
            CubeShadowDepthInfo[i] = new CubeMapInfo();
        }
    }
    public void Reset()
    {
        for(int i=0;i< light_count;i++)
        {
            lights[i] = null;
            distance[i] = 10000.0f;
        }
        light_count = 0;
    }
    public void Add(DeferredLight l,float dis)
    {
        float temp_dis = dis - l.Radius;
        if(light_count< distance.Length)
        {
            distance[light_count] = temp_dis;
            lights[light_count] = l;
            light_count++;
            return;
        }
        int max_index = -1;
        float max_distance = 0.0f;
        for (int j = 0; j < light_count; j++)
        {
            if (distance[j] > max_distance )
            {
                max_index = j;
                max_distance = distance[j];
            }
        }
        if(max_index!=-1 && temp_dis < max_distance)
        {
            distance[max_index] = temp_dis;
            lights[max_index] = l;
        }
    }
    public float GetRadius(DeferredLight l )
    {
        for(int i=0;i<CubeShadowDepthInfo.Length;i++)
        {
            if(CubeShadowDepthInfo[i].light == l )
            {
                return CubeShadowDepthInfo[i].LastRadius;
            }
        }
        return l.Radius;
    }
    public Vector3 GetPosition(DeferredLight l)
    {
        for (int i = 0; i < CubeShadowDepthInfo.Length; i++)
        {
            if (CubeShadowDepthInfo[i].light == l)
            {
                return CubeShadowDepthInfo[i].LastUpdatePos;
            }
        }
        return l.transform.position;
    }
    public RenderTexture GetDynamicCubemap(DeferredLight l)
    {
        for (int i = 0; i < CubeShadowDepthInfo.Length; i++)
        {
            if (CubeShadowDepthInfo[i].light == l && CubeShadowDepthInfo[i].NeedDynamic)
            {
                return dynamic_cubemap;
            }
        }
        return null;
    }
    public RenderTexture GetDynamicCubemap()
    {
        return dynamic_cubemap;
    }
    public void Update(Vector3 MainCharPos,Camera PointLightCamera,Shader depth_shader,int PointLevel)
    {
        //Check Already Exist Light
        for (int i = 0; i < CubeShadowDepthInfo.Length; i++)
        {
            if (CubeShadowDepthInfo[i].light != null)
            {
                bool exist = false;
                for (int j = 0; j < lights.Length; j++)
                {
                    if (CubeShadowDepthInfo[i].light == lights[j])
                    {
                        exist = true;
                        lights[j] = null;
                        break;
                    }
                }
                if (!exist)
                {
                    CubeShadowDepthInfo[i].Clear();
                }
            }
        }
        //Check New Light(Need Shadow Depth)
        for (int i = 0; i < CubeShadowDepthInfo.Length; i++)
        {
            if (CubeShadowDepthInfo[i].light == null)
            {
                for (int j = 0; j < lights.Length; j++)
                {
                    if (lights[j] != null)
                    {
                        CubeShadowDepthInfo[i].SetLight( lights[j]);
                        lights[j] = null;
                    }
                }
            }

        }
        


        //Update Shadow Depth
        for (int i = 0; i < CubeShadowDepthInfo.Length; i++)
        {
            CubeMapInfo cmInfo = CubeShadowDepthInfo[i];
            if (cmInfo.light!=null)
            {
                if(cmInfo.spheremap == null)
                {
                    cmInfo.spheremap = new RenderTexture(256,256, 16);
                    cmInfo.spheremap.isCubemap = true;
                    cmInfo.spheremap.isPowerOfTwo = true;
                }
                Vector3 current_pos = cmInfo.light.transform.position;
                
                if(Vector3.Distance(current_pos, cmInfo.LastUpdatePos)>0.1f)
                {
                    cmInfo.NeedRefresh = true;
                }
                if (Mathf.Abs(cmInfo.light.Radius - cmInfo.LastRadius) > 0.1f)
                {
                    cmInfo.NeedRefresh = true;
                }

                if(cmInfo.NeedRefresh)
                {
                    cmInfo.LastUpdatePos = current_pos;
                    cmInfo.LastRadius = cmInfo.light.Radius;
                    
                    if(temp_cube==null)
                    {
                        temp_cube = new sdCubeMap();
                    }
                    temp_cube.Render(PointLightCamera, cmInfo.LastUpdatePos, cmInfo.light.NearClipPlane,cmInfo.LastRadius, cmInfo.spheremap, "Default", depth_shader);
                    cmInfo.light.cubemap = cmInfo.spheremap;
                    cmInfo.NeedRefresh = false;
                }

                
            }
        }

        //Check Dynamic Shadow Depth(MainChar In Light Radius)
        int idx = -1;
        float min_len = 10000.0f;
        for (int i = 0; i < CubeShadowDepthInfo.Length; i++)
        {
            CubeMapInfo cmInfo = CubeShadowDepthInfo[i];
            cmInfo.NeedDynamic = false;
            if (cmInfo.light != null)
            {
                float len = (cmInfo.LastUpdatePos - MainCharPos).magnitude;
                if (len < cmInfo.LastRadius + 0.5f)
                {
                    idx = i;
                    min_len = len;
                }
            }
        }
        if (PointLevel >= 2)
        {
            if (idx != -1)
            {
                if (dynamic_cubemap == null)
                {
                    dynamic_cubemap = new RenderTexture(256, 256, 16);
                    dynamic_cubemap.isCubemap = true;
                    dynamic_cubemap.isPowerOfTwo = true;
                }

                CubeMapInfo cmInfo = CubeShadowDepthInfo[idx];

                if (temp_cube == null)
                {
                    temp_cube = new sdCubeMap();
                }
                temp_cube.Render(PointLightCamera, cmInfo.LastUpdatePos, cmInfo.light.NearClipPlane, cmInfo.LastRadius, dynamic_cubemap, "Player", depth_shader);

                //cmInfo.light.cubemap = cmInfo.cubemap;
                cmInfo.NeedDynamic = true;
            }
        }
    }
}
[ExecuteInEditMode]
public class DeferredLight : MonoBehaviour {
    public float Radius = 1.0f;
    public Color color = Color.white;
    public float intensity = 2.0f;
    public float Attenuation = 2.0f;
    public float glow = 0.0f;
    [System.NonSerialized]
    public bool IsVisableInMainCamera = false;
    public IllumType _IllumType = IllumType.Diffuse;
    public bool CastShadow = false;
    [System.NonSerialized]
    public Texture cubemap;
    public float NearClipPlane = 0.1f;
    public float GetRadius()
    {
        if(cubemap==null)
        {
            return Radius;
        }
        return lightcache.GetRadius(this);
    }
    public Vector3 GetPosition()
    {
        if (cubemap == null)
        {
            return transform.position;
        }
        return lightcache.GetPosition(this);
    }
    


	// Use this for initialization
    public static List<DeferredLight> LightList = new List<DeferredLight>();
    public static DeferredLight[] NearestLightList = new DeferredLight[6];
    public static int NearestLightCount = 0;
    private static LightCache lightcache = new LightCache(4);



    static int[] FillArray(int count)
    {
        int[] arr = new int[count];
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = -1;
        }
        return arr;
    }
    public static void Splite(float[] dislist, int count, ref int[] ids, ref int id_count, ref int[] out_list,ref int out_count)
    {
        int left_count = id_count;
        int id = ids[0];
        float temp_dis = dislist[id];
        //把所有小于基准值的元素放进表里
        for (int i = 1; i < id_count; i++)
        {
            if (dislist[ids[i]] < temp_dis)
            {
                out_list[out_count] = ids[i];
                out_count++;
                ids[i] = -1;
                left_count--;
            }
        }
        //个数依然不满足 把当前元素也放进去...
        if (out_count < count)
        {
            out_list[out_count] = ids[0];
            out_count++;

            ids[0] = -1;
            left_count--;
        }
        //个数已经满足 剩余全部抛弃...
        if (out_count >= count)
        {
            id_count = 0;
            for (int i = 0; i < ids.Length; i++)
            {
                ids[i] = -1;
            }
            if (out_count == count)
            {
                return;
            }
            else
            {
                int[] temp_lst = FillArray(out_count);
                int temp_count = 0;
                Splite(dislist, count, ref out_list, ref out_count, ref temp_lst, ref temp_count);
                out_count = temp_count;
                for (int i = 0; i < temp_count; i++)
                {
                    out_list[i] = temp_lst[i];
                }
            }
        }
        else
        {
            //移动有效的数据到队列头部...
            for (int i = 0; i < left_count; i++)
            {
                if (ids[i] == -1)
                {
                    for (int j = i + 1; j < id_count; j++)
                    {
                        if (ids[j] != -1)
                        {
                            ids[i] = ids[j];
                            ids[j] = -1;
                            break;
                        }
                    }
                }

            }
            id_count = left_count;

            int find_count = count - out_count;

            int[] temp_lst = FillArray(id_count);
            int temp_count = 0;
            Splite(dislist, find_count, ref ids, ref id_count, ref temp_lst, ref temp_count);
            for (int i = 0; i < temp_count; i++)
            {
                out_list[out_count + i] = temp_lst[i];
            }
            out_count += temp_count;
        }
    }
    
    public static int FindNearestLight(Vector3 pos, int count,float maxdistance)
    {
        if (NearestLightList.Length < count)
        {
            NearestLightList = new DeferredLight[count];
        }
        for (int i = 0; i < NearestLightList.Length; i++)
        {
            NearestLightList[i] = null;
        }
        NearestLightCount = 0;

        if (LightList.Count == 0)
        {
            return 0;
        }
        if (count <= 0)
        {
            return 0;
        }
        if (count >= LightList.Count)
        {
            for (int i = 0; i < NearestLightList.Length && i < LightList.Count; i++)
            {
                DeferredLight l = LightList[i];
                if ((l._IllumType & IllumType.Specular) == IllumType.Specular)
                {
                    NearestLightList[NearestLightCount] = LightList[i];
                    NearestLightCount++;
                }
            }

            return NearestLightCount;
        }

        if (count == 1)
        {
            float maxdis = 9999.0f;
            int id = -1;
            for (int i = 0; i < LightList.Count; i++)
            {
                DeferredLight l = LightList[i];
                if ((l._IllumType & IllumType.Specular) == IllumType.Specular)
                {
                    float len = (l.transform.position - pos).magnitude;
                    if (len < maxdis)
                    {
                        id = i;
                        maxdis = len;
                    }
                }
            }
            if (id != -1)
            {
                NearestLightList[0] =   LightList[id];
                NearestLightCount = 1;
                return NearestLightCount;
            }
            else
            {
                return 0;
            }
        }
        else
        {
            float[] dis = new float[LightList.Count];
            int[] ids = FillArray(LightList.Count);
            int temp_count = 0;
            for (int i = 0; i < LightList.Count; i++)
            {
                DeferredLight l = LightList[i];
                dis[i] = (l.transform.position - pos).magnitude;
                if ((l._IllumType & IllumType.Specular) == IllumType.Specular)
                {
                    //if (l.IsVisableInMainCamera)
                    //{
                    //    ids[temp_count] = i;
                    //    temp_count++;
                    //}
                    //else 
                    if (dis[i] < maxdistance + l.Radius)
                    {
                        ids[temp_count] = i;
                        temp_count++;
                    }
                }
            }
            if (count >= temp_count)
            {
                for (int i = 0; i < temp_count; i++)
                {
                    NearestLightList[i] = LightList[ids[i]];
                }
                NearestLightCount = temp_count;
                return NearestLightCount;
            }
            int[] out_list = FillArray(temp_count);
            int out_count = 0;
            Splite(dis, count, ref ids, ref temp_count, ref out_list, ref out_count);
            if (out_count == 0)
            {
                return 0;
            }
            //string print_str = "";
            for (int i = 0; i < out_count; i++)
            {
                NearestLightList[i] = LightList[out_list[i]];
                //print_str += NearestLightList[i].Radius + " ";
            }
            //Debug.Log(print_str);
            NearestLightCount = out_count;
            return NearestLightCount;
        }
        

    }
    public static int FindFarLight(Vector3 center)
    { 
        if(NearestLightCount==0)
        {
            return -1;
        }
        if (NearestLightCount == 1)
        {
            return 0;
        }
        else
        {
            float distance = Vector3.Distance(center, NearestLightList[0].transform.position)- NearestLightList[0].Radius;
            int id = 0;
            for(int i=1;i< NearestLightCount;i++)
            {
                float length = Vector3.Distance(center, NearestLightList[i].transform.position)- NearestLightList[i].Radius;
                if (length > distance)
                {
                    distance = length;
                    id = i;
                }
            }
            return id;
        }
        return -1;
    }
    public static void AddLight(DeferredLight l,Vector3 center,int max_count)
    {
        
        if(NearestLightCount < max_count)
        {
            NearestLightList[NearestLightCount] = l;
            NearestLightCount++;
            return;
        }
        else
        {
            int id = FindFarLight(center);
            if(id == -1)
            {
                NearestLightList[NearestLightCount] = l;
                NearestLightCount++;
                return;
            }
            else
            {
                float length = Vector3.Distance(center, l.transform.position) - l.Radius;
                float origin_length = Vector3.Distance(center, NearestLightList[id].transform.position) - l.Radius;
                if (length < origin_length)
                {
                    NearestLightList[id] = l;
                }
                return;
            }
        }
    }
    public static int FindSpecularLight(Vector3 pos, int count, float maxdistance)
    {
        if (NearestLightList.Length < count)
        {
            NearestLightList = new DeferredLight[count];
        }
        for (int i = 0; i < NearestLightList.Length; i++)
        {
            NearestLightList[i] = null;
        }
        NearestLightCount = 0;

        for(int i=0;i< LightList.Count;i++)
        {
            DeferredLight l = LightList[i];
            if ((l._IllumType & IllumType.Specular) == IllumType.Specular)
            {
                if (l.IsVisableInMainCamera)
                {
                    AddLight(l, pos, count);
                }
                //else if(Vector3.Distance(pos,l.transform.position) < 10.0f)
                //{
                //    AddLight(l, pos, count);
                //}

            }
        }
        return NearestLightCount;
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
#if UNITY_EDITOR
        if(Attenuation<0.0001f)
        {
            Attenuation = 0.0001f;
        }
#endif
    }

    void OnDrawGizmos()
    {
        
        Gizmos.DrawIcon(transform.position, "Light.png");
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
    //public DeferredLight[] FindNearestLight(Vector3 MainCameraPos, List<DeferredLight> lstLight,int count)
    //{
    //    float[] distance = new float[count];
    //    DeferredLight[] lights = new DeferredLight[count];
    //    for(int i=0;i< distance.Length;i++)
    //    {
    //        distance[i] = 10000.0f;
    //    }
    //    for(int i=0;i< lstLight.Count;i++)
    //    {
    //        Vector3 pos = lstLight[i].transform.position;
    //        float len = (pos - MainCameraPos).sqrMagnitude;
    //        int max_index = -1;
    //        float max_distance = 100000.0f;
    //        for(int j=0; j< count;j++)
    //        {
    //            if(distance[j]<max_distance && lights[j]==null)
    //            {
    //                max_index = j;
    //                max_distance = distance[j];
    //            }
    //        }
    //    }
    //}

    static List<DeferredLight> lstLight = new List<DeferredLight>();
    public static void DrawDeferredLight(
        Mesh deferred_light_mesh,
        Material mat_deferred_light, 
        Vector4 farCorner, 
        Vector4 _InvViewport, 
        RenderTexture _Diffuse, 
        RenderTexture gbuffer_tex,
        Camera MainCamera,
        Frustum MainCameraFrustum,
        Matrix4x4 maincam_World,
        Camera PointLightCamera,
        Shader ShadowDepthShader,
        RenderBuffer colorBuffer,
        RenderBuffer depthBuffer,
        int PointLevel,
        int DisappearLevel
        )
    {
        if (Application.isEditor)
        {
            farCorner.w = -1;
        }
        else
        {
            farCorner.w = 1.0f;
        }

        //Matrix4x4 maincam_World = Matrix4x4.identity;

        Camera cam_main = MainCamera;

        //ViewMatrix(cam_main, ref maincam_World);

        Vector3 cam_pos = cam_main.transform.position;
        //List<DeferredLight> lstLight = new List<DeferredLight>();

        lstLight.Clear();
        lightcache.Reset();
        for (int i = 0; i < DeferredLight.LightList.Count; i++)
        {
            DeferredLight l = DeferredLight.LightList[i];
            l.IsVisableInMainCamera = false;
            if (l.intensity <= 0.0f)
            {
                continue;
            }
            if (l.Radius <= 0.0f)
            {
                continue;
            }
            Vector3 light_pos = l.transform.position;
            if (!MainCameraFrustum.IsVisiable(light_pos, l.Radius * 0.5f))
            {
                continue;
            }

            l.IsVisableInMainCamera = true;
            lstLight.Add(l);
            if (l.CastShadow)
            {
                lightcache.Add(l,Vector3.Distance(light_pos, cam_pos));
            }
            
        }
        Vector3 MainCharPos = cam_pos + cam_main.transform.TransformDirection(Vector3.forward) * 8.0f;
        if (Application.isPlaying)
        {
         //   if(sdGlobalDatabase.Instance.mainChar!= null)
            {
          //      MainCharPos = sdGlobalDatabase.Instance.mainChar.transform.position;
            }
        }
        if (PointLevel >= 1)
        {
            lightcache.Update(MainCharPos, PointLightCamera, ShadowDepthShader, PointLevel);
        }


        float min_disappear_distance = 10.0f + 15.0f * (float)DisappearLevel;
        float max_disappear_distance = 30.0f + 15.0f * (float)DisappearLevel;

        Graphics.SetRenderTarget(colorBuffer, depthBuffer);
        int Rendered_Light_Count = 0;
        for (int i = 0; i < lstLight.Count; i++)
        {
            DeferredLight l = lstLight[i];

            Vector3 light_pos = l.GetPosition();

            float distance = (light_pos - cam_main.transform.position).magnitude - l.Radius;
            float fLightColorScale = 1.0f;
            if(distance > min_disappear_distance)
            {
                fLightColorScale =  (max_disappear_distance - distance) / 20.0f;
            }
            //超过衰减 距离 不再处理
            if (fLightColorScale < 0)
            {
                continue;
            }
            Rendered_Light_Count++;
            Vector4 viewpos = maincam_World.MultiplyPoint(light_pos);
            viewpos.w = l.GetRadius();
            Vector3 front = cam_main.transform.TransformDirection(Vector3.forward);

            float len = Vector3.Dot(front, light_pos - cam_main.transform.position);

            float farplane = cam_main.farClipPlane + l.Radius;


            if ((l._IllumType & IllumType.Diffuse) == IllumType.Diffuse)
            {
               // ProfilingProfiler.BeginSample("DLight SetMaterial");
                l.IsVisableInMainCamera = true;
                Color color = l.color * l.intensity* fLightColorScale;
                color.a = l.glow;
                mat_deferred_light.SetVector("_LightPos", viewpos);
                mat_deferred_light.SetVector("_FarCorner", farCorner);
                mat_deferred_light.SetColor("_Color", color);
                Vector4 attenuation = Vector4.zero;
                attenuation.x = l.Attenuation;
                mat_deferred_light.SetColor("_Attenuation", attenuation);
                mat_deferred_light.SetTexture("_MainTex", gbuffer_tex);
                mat_deferred_light.SetTexture("_Diffuse", _Diffuse);
                //mat_deferred_light.SetTexture("_AOTex", ShadowAO);
                mat_deferred_light.SetVector("_InvViewport", _InvViewport);
             //   ProfilingProfiler.EndSample();
             //   ProfilingProfiler.BeginSample("DLight Render");

                Texture cubemap = null;
                Texture dynamic_cubemap = null;
                if (l.CastShadow && PointLevel >= 1)
                {
                    cubemap = l.cubemap;
                    if (cubemap != null)
                    {
                        cubemap.filterMode = FilterMode.Point;
                    }
                    if (PointLevel >= 2)
                    {
                        dynamic_cubemap = lightcache.GetDynamicCubemap(l);
                    }
                    else
                    {
                        dynamic_cubemap = null;
                    }
                }

                if (cubemap)
                {

                    mat_deferred_light.SetTexture("_DepthCube", cubemap);
                    mat_deferred_light.SetMatrix("_InvView", cam_main.transform.localToWorldMatrix);
                }
                if(dynamic_cubemap!=null)
                {
                    mat_deferred_light.SetTexture("_DynamicDepthCube", dynamic_cubemap);
                }

                Vector3 offset = viewpos;
                if (len < l.Radius + cam_main.nearClipPlane)
                {
                    if (cubemap)
                    {
                        if (dynamic_cubemap != null)
                        {
                            mat_deferred_light.SetPass(6);
                        }
                        else
                        {
                            mat_deferred_light.SetPass(4);
                        }
                    }
                    else
                    {
                        mat_deferred_light.SetPass(2);
                    }
                    Graphics.DrawMeshNow(deferred_light_mesh, light_pos, Quaternion.identity, 0);
                }
                else
                {
                    mat_deferred_light.SetPass(0);
                    Graphics.DrawMeshNow(deferred_light_mesh, light_pos, Quaternion.identity, 0);
                    if (cubemap)
                    {
                        if (dynamic_cubemap != null)
                        {
                            mat_deferred_light.SetPass(5);
                        }
                        else
                        {
                            mat_deferred_light.SetPass(3);
                        }
                    }
                    else
                    {
                        mat_deferred_light.SetPass(1);
                    }
                    Graphics.DrawMeshNow(deferred_light_mesh, light_pos, Quaternion.identity, 0);
                }
             //   ProfilingProfiler.EndSample();
            }
        }
        //Debug.LogWarning("DL Count=" + Rendered_Light_Count + "/" + lstLight.Count);
    }
    public static void RenderDepth(Camera cam, Shader shader)
    {
        

    }
}
