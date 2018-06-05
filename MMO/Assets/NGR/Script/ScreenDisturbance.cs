using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[ExecuteInEditMode]
public class ScreenDisturbance : MonoBehaviour
{
    public static List<ScreenDisturbance> ScreenDisturbanceList = new List<ScreenDisturbance>();
   

    void OnEnable()
    {
        ScreenDisturbanceList.Add(this);
    }
    void OnDisable()
    {
        ScreenDisturbanceList.Remove(this);
    }

    public Mesh renderMesh;
    public Texture2D _NormalTex;
    public Color _Color = Color.white;
    public float refract_scale = 1.0f;
    
	// Use this for initialization
    public static void DrawAll(Material Disturbance_mat,RenderTexture _DepthTex,RenderTexture _LastFrame, Vector4 _FarCorner)
    {
        if (Application.isEditor)
        {
            _FarCorner.w = -1;
        }
        else
        {
            _FarCorner.w = 1.0f;
        }
        for (int i = 0; i < ScreenDisturbanceList.Count; i++)
        {
            ScreenDisturbance w = ScreenDisturbanceList[i];
            if (w == null)
                continue;
            if (!w.enabled)
                continue;

            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetTRS(w.transform.position, w.transform.rotation, w.transform.lossyScale);


            Disturbance_mat.SetTexture("_Depth", _DepthTex);
            Disturbance_mat.SetTexture("_LastFrame", _LastFrame);
            Disturbance_mat.SetVector("_FarCorner", _FarCorner);
            Disturbance_mat.SetTexture("_NormalTex", w._NormalTex);
            Disturbance_mat.SetColor("_Color", w._Color);
            Vector4 param = new Vector4();
            param.x = w.refract_scale;
            //param.y = w.water_fog_distance;

            Disturbance_mat.SetVector("_Param", param);
            //draw water fog

            //draw water refract and reflect
            Disturbance_mat.SetPass(0);

            Graphics.DrawMeshNow(w.renderMesh, matrix, 0);

        }
    }

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
        Gizmos.DrawIcon(transform.position, "glass.png");
    }
}
