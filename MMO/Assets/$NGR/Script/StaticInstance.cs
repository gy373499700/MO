using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class StaticInstance : MonoBehaviour
{
    public Mesh BaseMesh = null;
    public Quaternion MeshRotate = new Quaternion(1, 0, 0, 0);



    private Mesh mesh = null;
    private MeshRenderer mr = null;
    private int LastCount = 0;

    void Awake()
    {
       

        mesh = new Mesh();
        mesh.MarkDynamic();
        mesh.name = name + "(rmesh)";
        mesh.hideFlags = HideFlags.DontSaveInEditor ;
        mr = GetComponent<MeshRenderer>();
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf != null)
        {
            if (!Application.isPlaying)
            {
                if (mf.sharedMesh != null && BaseMesh == null)
                {
                    BaseMesh = mf.sharedMesh;
                }
            }
            mf.sharedMesh = mesh;
        }
        //mf.hideFlags = HideFlags.HideAndDontSave;
    }

    void BuildMesh()
    {
        if(BaseMesh==null)
        {
            return;
        }
        int InstanceCount = transform.childCount;

        if (InstanceCount < 0)
        {
            InstanceCount = 0;
        }
        if (InstanceCount > 65535)
        {
            InstanceCount = 65535;
        }
        if (LastCount != InstanceCount)
        {
            //RandomGrassPos();
            if (BaseMesh.vertexCount * InstanceCount > 65535)
            {
                Debug.LogError("Vertex Count > 65535 Grass BuildMesh Failed!");
                return;
            }
            Bounds bb = BaseMesh.bounds;

            Vector3 vMin = bb.min;
            vMin.x = 0.0f;
            vMin.z = 0.0f;

            int vertex_count = BaseMesh.vertexCount;
            Vector3[] srcPos = BaseMesh.vertices;
            Vector3[] srcNor = BaseMesh.normals;
            Vector2[] srcUV = BaseMesh.uv;
            Vector3[] pos = new Vector3[vertex_count * InstanceCount];
            Vector3[] nor = new Vector3[vertex_count * InstanceCount];
            Vector2[] uv = new Vector2[vertex_count * InstanceCount];
            //Color32[] colors = new Color32[vertex_count * InstanceCount];

            Quaternion rot = Quaternion.AngleAxis(MeshRotate.w, new Vector3(MeshRotate.x, MeshRotate.y, MeshRotate.z));

            for (int j = 0; j < vertex_count; j++)
            {
                srcPos[j] = rot * srcPos[j];
                srcNor[j] = rot * srcNor[j];
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
                if (vmaxy < v.y)
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

            for (int i = 0; i < InstanceCount; i++)
            {
                Transform node = transform.GetChild(i);

                Quaternion q = node.localRotation;
                Vector3 size = node.localScale;

                temp = node.localPosition;
                
                //float a = temp.x * 100.0f + temp.z;
               // c.a = a - Mathf.Floor(a);
                //Color32 c32 = c;

                for (int j = 0; j < vertex_count; j++)
                {
                    uv[i * vertex_count + j] = srcUV[j];

                    Vector3 p = (srcPos[j]);
                    p.x *=size.x;
                    p.y *= size.y;
                    p.z *= size.z;

                    {
                        nor[i * vertex_count + j] = q * srcNor[j];
                    }
                    Vector3 dst_pos = q * p + temp;
                    pos[i * vertex_count + j] = dst_pos;
                    //colors[i * vertex_count + j] = c32;
                    //colors[i * vertex_count + j].a = (byte)(c32.a * p.y / vmaxy);
                }
            }
            if (InstanceCount > LastCount)
            {
                mesh.vertices = pos;
                mesh.normals = nor;
                mesh.uv = uv;
                //mesh.colors32 = colors;
                //mesh.triangles = Index;

            }

            mesh.subMeshCount = BaseMesh.subMeshCount;
            for (int sub = 0; sub < BaseMesh.subMeshCount; sub++)
            {

                int[] srcIndex = BaseMesh.GetIndices(sub);
                int indexcount = srcIndex.Length;
                int[] Index = new int[indexcount * InstanceCount];
                for (int i = 0; i < InstanceCount; i++)
                {
                    for (int j = 0; j < indexcount; j++)
                    {
                        Index[i * indexcount + j] = srcIndex[j] + vertex_count * i;
                    }
                }

                mesh.SetTriangles(Index, sub);

            }
            if (InstanceCount <= LastCount)
            {
                mesh.vertices = pos;
                mesh.normals = nor;
                //mesh.colors32 = colors;
                mesh.uv = uv;
            }


            LastCount = InstanceCount;
        }

    }

    void Update()
    {
        BuildMesh();

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
    void OnRenderObject()
    {
        
    }

    void OnDrawGizmosSelected()
    {

        
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform node = transform.GetChild(i);
            Gizmos.DrawIcon(node.position, "grass.jpg");
        }
        Gizmos.matrix = Matrix4x4.identity;
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