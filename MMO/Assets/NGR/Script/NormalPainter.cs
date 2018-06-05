using UnityEngine;
using System.Collections;
public enum NormalPaintType
{
    Concave,
    Convex,
    Flat,
    Roughness
}

[ExecuteInEditMode]
public class NormalPainter : MonoBehaviour {
    public bool Editing = true;
    public bool Erase = false;
    public int paint_size = 5;
    public float paint_power = 0.5f;
    public NormalPaintType paint_type = NormalPaintType.Flat;
    public Vector3 intersect_point;
    public Mesh mesh = null;
    public Texture2D paint_tex = null;
    public Texture2D color_control_tex = null;
    //public bool ShowNormal = false;
    //public MeshCollider mc = null;

    void OnEnable()
    {

        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf != null)
        {
            mesh = mf.sharedMesh;
        }
        if (mesh == null)
        {
            SkinnedMeshRenderer skr = GetComponent<SkinnedMeshRenderer>();
            if (skr)
            {
                mesh = skr.sharedMesh;
            }
        }
        Renderer mr = GetComponent<Renderer>();
        if (mr != null)
        {
            Material m = mr.sharedMaterial;
            if (m != null)
            {
                paint_tex   =   m.GetTexture("_MainTex") as Texture2D;
                color_control_tex = m.GetTexture("_SpecTex") as Texture2D;
            }
        }
        //mc = GetComponent<MeshCollider>();

    }
    // Update is called once per frame
    void Update () {

    }
    void OnDrawGizmos()
    {
        //if (Editing)
        {
            Gizmos.DrawSphere(intersect_point, 0.02f);
            Gizmos.color = new Color(0, 1.0f, 0, 0.2f);
            Gizmos.DrawSphere(intersect_point, paint_size*0.02f);
        }
    }
    public void SetColorControl(Texture2D tex)
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr != null)
        {
            Material m = mr.sharedMaterial;
            if (m != null)
            {
                m.SetTexture("_SpecTex", tex);
            }
        }
        color_control_tex = tex;
    }
    //void OnGUI()
    //{
        //string content = "ShowNormal";
        //if(SceneRenderSetting._Setting.ShowNormal)
        //{
        //    content = "HideNormal";
        //}
        //if (GUI.Button(new Rect(0,0,100,100), content))
        //{
        //    SceneRenderSetting._Setting.ShowNormal = !SceneRenderSetting._Setting.ShowNormal;
        //}
    //}
}
