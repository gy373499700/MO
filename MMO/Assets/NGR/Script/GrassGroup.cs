using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum GrassAreaType
{
    Rect,
    Circle,
}
[ExecuteInEditMode]
public class GrassGroup : MonoBehaviour {
    public Mesh     BaseMesh        = null;//默认用quad
    public Vector4 MeshRotate = new Vector4(1,0,0,0);//模型以.w为轴旋转 旋转xyz  正常不需要改
    public float    randomDirection = 180;//方向随机分布范围
    public Vector2 randomSize = Vector2.one;//mesh大小随机分布范围
    public Vector2 distribution = Vector2.one;//x作为Circle的集中或者扩散分布，xy作为Rect的grass_area多个区域的位置分布情况
    public GrassAreaType type = GrassAreaType.Rect;//以矩形或者球形作为草的分布 与grass_area作为矩形或者球形的位置分布 
    public Color randomColor0 = Color.white;
    public Color randomColor1 = Color.white;//mesh颜色从randomColor0到randomColor1进行随机插值
    public bool UnifiedNormal = true;//是否以vector3.up作为法向量
    public int GrassCount = 10;//草的数量
    public bool ReBuild = false;//编辑器重新生成草
    public Vector4[] grass_area;//作为矩形或者球形的多个随机位置分布 ，xy作为位置，zw作为rect的大小，z作为Circle的大小
    public bool UseWind = false;
    public Vector3 WindDir =new Vector3(1, 0, 1);//风的方向和大小
  //  [Range(0,2)]
    public float WindStrength = 1f;//风的频率
    float area_radius = 0.0f;
   //摆动效果由shadervertex控制，而不是动态改mesh

    private Material    Mat = null;
    private Mesh        mesh = null;
    private int         LastGrassCount = 0;

    public static List<GrassGroup> GrassList = new List<GrassGroup>();
    static int iCurrentUpdate = 0;

    private MeshRenderer mesh_render = null;
    // Use this for initialization
    void Awake ()
    {
#if UNITY_EDITOR
        if(!Application.isPlaying)
        {
            mesh_render = gameObject.GetComponent<MeshRenderer>();
            if(mesh_render==null)
            {
                mesh_render = gameObject.AddComponent<MeshRenderer>();
            }
            MeshFilter mf = gameObject.GetComponent<MeshFilter>();
            if (mf == null)
            {
                mf = gameObject.AddComponent<MeshFilter>();
            }
        }
#endif
        mesh_render = gameObject.GetComponent<MeshRenderer>();
        CalcRadius();
    }
    void CreateMesh()
    {
        mesh = new Mesh();
        mesh.MarkDynamic();
        mesh.name = name + "(rmesh)";
        mesh.hideFlags = HideFlags.DontSaveInEditor;
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf != null)
        {
            mf.sharedMesh = mesh;
        }

        if (BaseMesh != null && !BaseMesh.isReadable)
        {
            Debug.LogError("grass group " + name + " BaseMesh isReadable = false");
        }
    }
    void OnEnable()
    {
        GrassList.Add(this);
    }
    void OnDisable()
    {
        GrassList.Remove(this);
    }
    public static void Tick()
    {
        ProfilingProfiler.BeginSample("GrassGroup.Tick");
        if (Application.isPlaying)
        {

            Vector3 cam_pos = Vector3.zero;
            if(RenderPipeline._instance!=null)
            {
                cam_pos=RenderPipeline._instance.transform.position;
            }
            if (iCurrentUpdate >= GrassList.Count)
            {
                iCurrentUpdate = 0;
            }
            //每帧更新10个草， 一般场景草数量大约为300 每个草的更新频率大约为1秒一次
            int grass_update_count = 10;
            for (int i = 0; i < grass_update_count; i++)
            {
                if (iCurrentUpdate < GrassList.Count)
                {
                    GrassGroup gg = GrassList[iCurrentUpdate];
                    if (gg != null)
                    {
                        if (RenderPipeline._instance != null)
                        {

                            float radius = (gg.area_radius);
                            float len = (cam_pos - gg.transform.position).magnitude;
                            if (len < radius + 30.0f)
                            {
                                gg.SetVisiable(true);
                                if (RenderPipeline._instance.IsVisibleInMainCamera(gg.transform.position, radius))
                                {
                                    gg.InternalUpdate();
                                    //grass_update_count++;
                                }
                            }
                            else
                            {
                                gg.SetVisiable(false);
                            }
                        }
                        else
                        {
                            gg.InternalUpdate();
                            //grass_update_count++;
                        }
                    }

                    iCurrentUpdate++;
                    if (iCurrentUpdate >= GrassList.Count)
                    {
                        iCurrentUpdate = 0;
                    }
                }
            }
           // Debug.Log("grass_update_count = " + grass_update_count+"/"+ GrassList.Count);
        }
        else
        {
            for (int i = 0; i < GrassList.Count; i++)
            {
                GrassGroup gg = GrassList[i];
                if (gg != null)
                {
                    gg.InternalUpdate();
                }
            }
        }
        ProfilingProfiler.EndSample();
    }
    float Distribution(float dis)
    {
        float val = Random.Range(0.0f, 1.0f);
        float AbsVal = Mathf.Abs(val);
        return  Mathf.Pow(AbsVal, dis)-0.5f;
    }
    Vector3 RandomGrassPos(float[] area_array)
    {
        if (grass_area == null)
        {
            return Vector3.zero;
        }
        if (grass_area.Length == 0)
            return Vector3.zero;

        {
            float frand = Random.Range(0, 1000)*0.001f;
            int id = area_array.Length - 1;
            for (int i=0;i< area_array.Length;i++)
            {
                if(frand< area_array[i])
                {
                    id = i;
                    break;
                }
            }
            Vector4 area = grass_area[id];


            Vector3 p = Vector3.zero;
            if(type==GrassAreaType.Rect)
            {
                p.x = Distribution(distribution.x) * area.z + area.x;
                p.z = Distribution(distribution.y) * area.w + area.y;
            }
            else if (type == GrassAreaType.Circle)
            {
                Vector3 dir = Vector3.forward;
                Quaternion q = Quaternion.AngleAxis(Random.Range(-180.0f, 180.0f), Vector3.up);
                dir = q * dir;
                float len = Mathf.Pow(Random.Range(0.0f, 1.0f),distribution.x);

                p.x = dir.x * area.z* len + area.x;
                p.z = dir.z * area.z * len + area.y;
            }
            return p;
        }
    }
    void CalcRadius()
    {
        area_radius = 0.0f;
        if (grass_area==null)
        { 
            return;
        }
        if (grass_area.Length == 0)
        {
            return;
        }

        for (int i = 0; i < grass_area.Length; i++)
        {
            Vector4 v = grass_area[i];
            float r = 0.0f;
            if (type == GrassAreaType.Rect)
            {
                r = Mathf.Sqrt(v.x*v.x+v.y*v.y);
                r += Mathf.Sqrt(v.z * v.z + v.w * v.w);
            }
            else if (type == GrassAreaType.Circle)
            {
                r = Mathf.Sqrt(v.x * v.x + v.y * v.y);
                r += v.z;
            }
            if(r > area_radius)
            {
                area_radius = r;
            }
        }
    }
    void BuildMesh()
    {
        
        

        int total_grass_count = GrassCount;
        if(RenderPipeline._instance!=null)
        {
            int level = RenderPipeline._instance.quality.GrassLevel;
            if(level==3)
            {
                total_grass_count = (int)(GrassCount * 0.4f);
            }
            else if (level == 2)
            {
                total_grass_count = (int)(GrassCount * 0.2f);
            }
            else if (level == 1)
            {
                total_grass_count = (int)(GrassCount * 0.1f);
            }
            else if (level == 0)
            {
                total_grass_count = (int)(GrassCount * 0.05f);
            }
        }

        if(total_grass_count<=1)
        {
            return;
        }

        if (mesh == null)
        {
            CreateMesh();
        }

        if (BaseMesh.vertexCount*total_grass_count > 65535)
        {
            Debug.LogError("Vertex Count > 65535 Grass BuildMesh Failed!");
            return;
        }

        CalcRadius();

        Bounds bb = BaseMesh.bounds;

            Vector3 vMin = bb.min;
            vMin.x = 0.0f;
            vMin.z = 0.0f;

            int vertex_count = BaseMesh.vertexCount;
            Vector3[] srcPos = BaseMesh.vertices;
            Vector3[] srcNor = BaseMesh.normals;
            Vector2[] srcUV = BaseMesh.uv;
            Vector3[] pos = new Vector3[vertex_count * total_grass_count];
            Vector3[] nor = new Vector3[vertex_count * total_grass_count];
            Vector2[] uv  = new Vector2[vertex_count * total_grass_count];
            Color32[] colors = new Color32[vertex_count * total_grass_count];

            Quaternion rot = Quaternion.AngleAxis(MeshRotate.w, new Vector3(MeshRotate.x, MeshRotate.y, MeshRotate.z));
            
            for (int j = 0; j < vertex_count; j++)
            {
                srcPos[j] = rot * srcPos[j];
                srcNor[j]  = rot * srcNor[j];
            }
            float vminy = srcPos[0].y;
            float vmaxy = srcPos[0].y;
            for (int j = 1; j < vertex_count; j++)
            {
                Vector3 v = srcPos[j];

                if (vminy > v.y)
                {
                    vminy = v.y;
                }
                if(vmaxy < v.y)
                {
                    vmaxy = v.y;
                }
            }

            vmaxy -= vminy;

            for (int j = 0; j < vertex_count; j++)
            {
                srcPos[j].y -= vminy;
            }


            Vector3 temp = Vector3.zero;

            float[] area = new float[grass_area.Length];
            float total = 0;
            for(int i=0;i< area.Length;i++)
            {
                Vector2 xy = new Vector2(grass_area[i].z, grass_area[i].w);
                if (type == GrassAreaType.Rect)
                {
                    area[i] = xy.x * xy.y;
                    
                }
                else
                {
                    area[i] = xy.x * xy.x;
                }
                total += area[i];
            }
            for (int i = 0; i < area.Length; i++)
            {
                area[i] /= total;
                if(i>0)
                {
                    area[i] += area[i - 1];
                }
            }

            for (int i=0;i< total_grass_count; i++)
            {
                Quaternion q=Quaternion.AngleAxis(Random.Range(-randomDirection, randomDirection), Vector3.up);
                float size = Random.Range(randomSize.x, randomSize.y);
                
                temp = RandomGrassPos(area);
                Color c = Color.Lerp(randomColor0,randomColor1, Random.Range(0.0f,1.0f));
                float a = temp.x * 100.0f + temp.z;
                c.a = a - Mathf.Floor(a);
                Color32 c32 = c;
                
                for (int j=0;j<vertex_count;j++)
                {
                    uv[i * vertex_count + j] = srcUV[j];

                    Vector3 p = (srcPos[j] ) * size ;
                    if (UnifiedNormal)
                    {
                        nor[i * vertex_count + j] = Vector3.up;
                    }
                    else
                    {
                        nor[i * vertex_count + j] = q * srcNor[j];
                    }
                    Vector3 dst_pos = q * p + temp;
                    pos[i * vertex_count + j] = dst_pos;
                    colors[i * vertex_count + j] = c32;
                    colors[i * vertex_count + j].a = (byte)(c32.a*p.y/ vmaxy);//每个面片根据y的坐标存储alpha   shader里边再根据alpha决定扰动幅度,
                }
            }
            if (total_grass_count > LastGrassCount)
            {
                mesh.vertices = pos;
                mesh.normals = nor;
                mesh.uv = uv;
                mesh.colors32 = colors;
                //mesh.triangles = Index;

            }

            mesh.subMeshCount = BaseMesh.subMeshCount;
            for (int sub = 0; sub < BaseMesh.subMeshCount; sub++)
            {

                int[] srcIndex = BaseMesh.GetIndices(sub);
                int indexcount = srcIndex.Length;
                int[] Index = new int[indexcount * total_grass_count];
                for (int i = 0; i < total_grass_count; i++)
                {
                    for (int j = 0; j < indexcount; j++)
                    {
                        Index[i * indexcount + j] = srcIndex[j] + vertex_count * i;
                    }
                }

                mesh.SetTriangles(Index, sub);

            }
            if (total_grass_count <= LastGrassCount)
            {
                mesh.vertices = pos;
                mesh.normals = nor;
                mesh.colors32 = colors;
                mesh.uv = uv;
            }


            LastGrassCount = total_grass_count;

    }
    void Unload()
    {
        if(mesh!=null)
        {
            Object.DestroyImmediate(mesh);
            mesh = null;

#if UNITY_EDITOR
            //Debug.Log("unload grass " + name + " " + LastGrassCount);

#endif
            LastGrassCount = 0;
        }
    }
    public void SetVisiable(bool bVisiable)
    {
        if(mesh_render!=null)
        {
            if (bVisiable&&!mesh_render.enabled)
            {
                mesh_render.enabled = true;
            }
            else if (!bVisiable && mesh_render.enabled)
            {
                mesh_render.enabled = false;
                Unload();
            }
        }
    }
    // Update is called once per frame
    void InternalUpdate()
    {
        if (GrassCount < 0)
        {
            GrassCount = 0;
        }
        if (GrassCount > 65535)
        {
            GrassCount = 65535;
        }
        bool need_rebuild = false;
        if (Application.isPlaying)
        {
            need_rebuild = mesh == null;
        }
        else
        {
            need_rebuild = LastGrassCount != GrassCount;
        }
        if (need_rebuild)
        {
            if (BaseMesh == null)
            {
                return;
            }
            if (!BaseMesh.isReadable)
            {
                return;
            }
            BuildMesh();
        }
#if UNITY_EDITOR
        if (ReBuild)
        {//test
            BuildMesh();
            ReBuild = false;
        }
#endif
        if (Mat == null)
        {
            Mat = GetComponent<MeshRenderer>().sharedMaterial;
        }
        if (UseWind)
        {
            Mat.SetVector("_WindDir", WindDir);
            Mat.SetFloat("_WindStrength", WindStrength);
            Mat.SetVector("_GrassPos", transform.position);
            Mat.EnableKeyword("Wind_On");
        }
        else
        {
            Mat.DisableKeyword("Wind_On");
        }
    }
    void OnDestroy()
    {

        DrstroyObj(mesh);
        mesh = null;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "grass.jpg");
        
    }

    void OnDrawGizmosSelected()
    {
        if(grass_area==null)
        {
            return;
        }
        Gizmos.matrix = transform.localToWorldMatrix;
        for (int i=0;i< grass_area.Length;i++)
        {
            Vector4 area = grass_area[i];
            Vector3 center = new Vector3(area.x, 0, area.y);
            Vector3 size = new Vector3(area.z, 0.5f, area.w);
            if (type == GrassAreaType.Rect)
            {
                Gizmos.DrawWireCube(center, size);
            }
            else if (type == GrassAreaType.Circle)
            {
                Gizmos.DrawWireSphere(center, area.z);
            }
        }
        Gizmos.matrix = Matrix4x4.identity;
    }

    static void DrstroyObj(Material obj)
    {
        if (obj == null)
            return;
        if (!Application.isPlaying)
        {
            Object.DestroyImmediate(obj);
        }
        else
        {
            Object.Destroy(obj);
        }
    }
    static void DrstroyObj(Mesh obj)
    {
        if (obj == null)
            return;
        if (!Application.isPlaying)
        {
            Object.DestroyImmediate(obj);
        }
        else
        {
            Object.Destroy(obj);
        }
    }
}
