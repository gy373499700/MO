using UnityEngine;
using System.Collections;
public enum PaintType
{
    Dark,
    Light,
    Fixed,
    Wet,
    Dry,
    FixedWetness
}

[ExecuteInEditMode]
public class T4MAOPainter : MonoBehaviour {
    public bool Editing = true;
    public bool Erase = false;
    public int paint_size = 5;
    public Color paint_color = new Color(0.5f,0.5f,0.5f,1.0f);
    public PaintType paint_type = PaintType.Dark;
    public Vector3 intersect_point;
    public Mesh mesh = null;
    public Texture2D paint_tex = null;
    public Texture2D color_control_tex = null;
    public MeshCollider mc = null;

    void OnEnable()
    {

        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf != null)
        {
            mesh = mf.sharedMesh;
        }
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr != null)
        {
            Material m = mr.sharedMaterial;
            if (m != null)
            {
                paint_tex   =   m.GetTexture("_Control") as Texture2D;
                color_control_tex = m.GetTexture("_ColorControl") as Texture2D;
            }
        }
        mc = GetComponent<MeshCollider>();

    }
    // Update is called once per frame
    void Update () {

    }
    void OnDrawGizmos()
    {
        //if (Editing)
        {
            Gizmos.DrawWireSphere(intersect_point, paint_size*0.25f);
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
                m.SetTexture("_ColorControl", tex);
            }
        }
        color_control_tex = tex;
    }
}
