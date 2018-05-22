using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FootMark : MonoBehaviour
{
    //float startTime = 0;

    Vector3[] posList = new Vector3[VertexCount];
    Quaternion[] rotList = new Quaternion[VertexCount];
    float[] timeList = new float[VertexCount];

    Mesh newMesh = null;
    MeshRenderer render = null;
    public Material mat = null;
    public static int VertexCount = 100;

    void OnDestroy()
    {
        GameObject.Destroy(newMesh);
    }

    void Start()
    {
        //color = GetComponent<MeshRenderer>().materials[0].GetColor("_Color");
        if (newMesh == null)
            newMesh = new Mesh();

        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null) mf = gameObject.AddComponent<MeshFilter>();

        render = GetComponent<MeshRenderer>();

        SetMesh();

        mf.sharedMesh = newMesh;
        //mat = new Material(mat);
        render.sharedMaterial = mat;

    }

    Vector3[] pos = new Vector3[VertexCount * 4];
    Vector3[] nor = new Vector3[VertexCount * 4];
    Vector2[] uv = new Vector2[VertexCount * 4];
    int[] index = new int[VertexCount * 6];

    void SetMesh()
    {
        for (int i = 0; i < VertexCount; ++i)
        {
            index[i * 6] = i * 4;
            index[i * 6 + 1] = i * 4 + 1;
            index[i * 6 + 2] = i * 4 + 2;

            index[i * 6 + 3] = i * 4 + 2;
            index[i * 6 + 4] = i * 4 + 1;
            index[i * 6 + 5] = i * 4 + 3;

            uv[i * 4] = Vector2.zero;
            uv[i * 4 + 1] = new Vector2(1, 0);
            uv[i * 4 + 2] = new Vector2(0, 1);
            uv[i * 4 + 3] = Vector2.one;

            Vector3 pos1 = rotList[i] * new Vector3(-0.13f, 0, 0.2f);
            Vector3 pos2 = rotList[i] * new Vector3(0.13f, 0, 0.2f);
            Vector3 pos3 = rotList[i] * new Vector3(-0.13f, 0, -0.2f);
            Vector3 pos4 = rotList[i] * new Vector3(0.13f, 0, -0.2f);

            pos[i * 4] = pos1 + posList[i];
            pos[i * 4 + 1] = pos2 + posList[i];
            pos[i * 4 + 2] = pos3 + posList[i];
            pos[i * 4 + 3] = pos4 + posList[i];

            nor[i * 4] = new Vector3(0, 0, timeList[i]);
            nor[i * 4 + 1] = new Vector3(1, 0, timeList[i]);
            nor[i * 4 + 2] = new Vector3(0, 1, timeList[i]);
            nor[i * 4 + 3] = new Vector3(1, 1, timeList[i]);

        }

        newMesh.vertices = pos;
        newMesh.normals = nor;
        newMesh.uv = uv;
        newMesh.triangles = index;


    }

    int PosIndex = 0;

    float minX = 0;
    float minY = 0;
    float minZ = 0;
    float maxX = 0;
    float maxY = 0;
    float maxZ = 0;

    public void AddPos(Vector3 pos, Quaternion rot)
    {
        if (pos.x > maxX)
        {
            maxX = pos.x;
        }

        if (pos.x < minX)
        {
            minX = pos.x;
        }

        if (pos.y > maxY)
        {
            maxY = pos.y;
        }

        if (pos.y < minY)
        {
            minY = pos.y;
        }

        if (pos.z > maxZ)
        {
            maxZ = pos.z;
        }

        if (pos.z < minZ)
        {
            minZ = pos.z;
        }

        Vector3 center = new Vector3((maxX + minX) / 2, (maxY + minY) / 2, (maxZ + minZ) / 2);

        newMesh.bounds = new Bounds(center, new Vector3((maxX - minX), (maxY - minY), (maxZ - minZ)));
        posList[PosIndex] = pos;
        rotList[PosIndex] = rot;
        timeList[PosIndex] = Time.time;
        SetMesh();
        ++PosIndex;
        if (PosIndex >= VertexCount)
        {
            PosIndex = 0;
        }
    }

   //void OnRenderObject()
   //{
   //    Camera cam = Camera.current;
   //    Matrix4x4 m = new Matrix4x4();
   //    m = transform.localToWorldMatrix;
   //    Vector3 up = cam.transform.TransformDirection(Vector3.up);
   //    Vector3 right = cam.transform.TransformDirection(Vector3.right);
   //    mat.SetVector("_UpDir", up);
   //    mat.SetVector("_RightDir", right);
   //    mat.SetVector("_Size", Vector4.one);
   //    mat.SetPass(0);
   //    Graphics.DrawMeshNow(newMesh, m, 0);
   //
   //}

    void OnEnable()
    {
        //startTime = 0;
    }



    void Update()
    {
        if (mat == null) return;  
        float time = mat.GetFloat("_CurTime");

        mat.SetFloat("_CurTime", Time.time);
        //         startTime += Time.smoothDeltaTime;
        //         
        //         color.a = Mathf.Lerp(1, 0, startTime / 3);
        //         GetComponent<MeshRenderer>().materials[0].SetColor("_Color", color);
        //         if (startTime >= 3)
        //         {
        //             gameObject.SetActive(false);
        //             sdGlobalDatabase.Instance.footMarkList.Remove(this);
        //             sdGlobalDatabase.Instance.hideFootMarkList.Add(this);
        //         }
    }
}
