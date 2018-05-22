using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class DrawInputMesh : Singleton<DrawInputMesh> {

    public enum MeshQuad
    {
        Quad,
        Cycle,
    }
	// Use this for initialization
	void Start () {
	
	}
    public float pointSize = 0.02f;//mesh大小
    public float pixelSize = 0.01f;//像素周围采样半径
    public float InputRefractScale = 0.1f;//扰动uv缩放
 //   public int pointFPSLifeTime = 10;
    public bool Debug = false;
    List<Vector3> inputpos = new List<Vector3>();

	public void DrawMesh(Mesh quad,Material matInputDisturb, RenderTexture currentRenderTarget, RenderTexture lastFrame)
    {

        UpdateMesh();
        if (Debug)
        {    //可以做一个画板
            matInputDisturb.SetPass(0);
            Graphics.DrawMeshNow(mesh, Vector3.zero, Quaternion.identity, 0);
        }
        else
        {
            RenderTexture rt = RenderTexture.GetTemporary(currentRenderTarget.width / 4, currentRenderTarget.height / 4);
            Graphics.SetRenderTarget(rt);
            GL.Clear(true, true, Color.black);
            matInputDisturb.SetPass(0);
            Graphics.DrawMeshNow(mesh, Vector3.zero, Quaternion.identity, 0);
            Graphics.SetRenderTarget(currentRenderTarget);
            matInputDisturb.SetTexture("_MaskTex", rt);
            matInputDisturb.SetTexture("_MainTex", lastFrame);
            matInputDisturb.SetTexture("_ColorTex", SceneRenderSetting._Setting.InputFracTex);
            matInputDisturb.SetFloat("_RefractScale",InputRefractScale);
            matInputDisturb.SetFloat("_PixelSize", pixelSize);
            matInputDisturb.SetPass(1);
            Graphics.DrawMeshNow(quad, Vector3.zero, Quaternion.identity, 0);
            RenderTexture.ReleaseTemporary(rt);
        }

    }

  //  Vector3[] vertex = new Vector3[64 * 4];
  //  Vector2[] uvs = new Vector2[64 * 4];
    public Mesh mesh = null;
    Vector3 outvec = new Vector3(-1, -1,-1);
    //  float timer = 0;
    int outindex = 0;
    public MeshQuad m_MeshQuad = MeshQuad.Quad;
    void UpdateMesh()
    {
        Camera maincamera = SceneRenderSetting._Setting.MainCamera;
        Vector3 pos = maincamera.ScreenToViewportPoint(Input.mousePosition);//0-1
        if (pos.x > 0 && pos.x < 1 && pos.y > 0 && pos.y < 1)
        {
            outindex = 0;
            inputpos.Add(pos);
        }
        else
        {//disappear
            outindex++;
            inputpos.Add(outvec);
        }
        if (mesh == null)
        {
            mesh = new Mesh();
        }
        if (outindex>64)
        {
            inputpos.Clear();
            mesh.Clear();
            return;
        }
        if (inputpos.Count > 64)
        {
            inputpos.RemoveAt(0);
        }
        if (m_MeshQuad == MeshQuad.Quad)
        {
            Vector3 up =  Vector3.up;
            Vector3 right =  Vector3.right;
            Vector2 uvRightTop = new Vector2(1, 1);
            Vector2 uvLeftTop = new Vector2(0, 1);
            Vector2 uvRightBottom = new Vector2(1, 0);
            Vector2 uvLeftBottom = new Vector2(0, 0);
            Vector3[] vertex = new Vector3[inputpos.Count * 4];
            Vector2[] uvs = new Vector2[inputpos.Count * 4];
            int[] triangles = new int[inputpos.Count * 6];
            for (int i = 0; i < inputpos.Count; i++)
            {

                vertex[i * 4] = -right * pointSize + up * pointSize + inputpos[i];
                vertex[i * 4 + 1] = right * pointSize + up * pointSize + inputpos[i];
                vertex[i * 4 + 2] = -right * pointSize - up * pointSize + inputpos[i];
                vertex[i * 4 + 3] = right * pointSize - up * pointSize + inputpos[i];

                triangles[i * 6] = i * 4 + 0;
                triangles[i * 6 + 1] = i * 4 + 1;
                triangles[i * 6 + 2] = i * 4 + 2;

                triangles[i * 6 + 3] = i * 4 + 2;
                triangles[i * 6 + 4] = i * 4 + 1;
                triangles[i * 6 + 5] = i * 4 + 3;

                uvs[i * 4] = uvRightTop;
                uvs[i * 4 + 1] = uvLeftTop;
                uvs[i * 4 + 2] = uvRightBottom;
                uvs[i * 4 + 3] = uvLeftBottom;
            }
            mesh.Clear();
            mesh.vertices = vertex;
            mesh.uv = uvs;
            mesh.triangles = triangles;
        }
        else if(m_MeshQuad == MeshQuad.Cycle)
        {
            int segments = 10;
            float radius = pointSize;
     
            Vector3[] vertices = new Vector3[inputpos.Count * (segments + 1)];
            int[] triangles = new int[inputpos.Count * segments * 3];
            for (int k = 0; k < inputpos.Count;k++)
            {
                Vector3 centerCircle = inputpos[k];
                int index = k * (segments + 1);
                int triangleindex = 3 * k * segments;
                vertices[index] = centerCircle;
                float deltaAngle = Mathf.Deg2Rad * 360f / segments;
                float currentAngle = 0;
                for (int i = 1; i < segments + 1; i++)
                {
                    float cosA = Mathf.Cos(currentAngle);
                    float sinA = Mathf.Sin(currentAngle);
                    vertices[index + i] = new Vector3(cosA * radius + centerCircle.x, sinA * radius + centerCircle.y, 0);
                    currentAngle += deltaAngle;
                }

                for (int i = 0, j = 0; i < segments-1; i++,j+=3)
                {
                    
                    triangles[triangleindex + j] = index;
                    triangles[triangleindex + j + 1] = index+ i + 1;
                    triangles[triangleindex + j + 2] = index+ i + 2;
                }
                triangles[triangleindex + segments * 3 - 3] = index + segments;
                triangles[triangleindex + segments * 3 - 2] = index+1;
                triangles[triangleindex + segments * 3 - 1] = index;
            }

            mesh.Clear();

            mesh.vertices = vertices;
            mesh.triangles = triangles;
        }
    }

    private void OnDisable()
    {
        inputpos.Clear();
    }
}
