using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[ExecuteInEditMode]
public class DeferredDecal : MonoBehaviour {
    public enum DecalType
    {
        Diffuse = 1,
        Roughness = 2,
        DiffuseAndRoughness = 3
    }
    public Vector3 size =   Vector3.one;
    public Color color = Color.white;
    public float color_scale = 1.0f;
    public float roughness = 0.0f;
    public Texture2D decal_tex;
    public DecalType decaltype = DecalType.Diffuse;
	// Use this for initialization
    public static List<DeferredDecal> DecalList = new List<DeferredDecal>();
    public static int Count()
    {
        return DecalList.Count;
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
        DecalList.Add(this);
    }
    void OnDisable()
    {
        DecalList.Remove(this);
    }
    public static void DrawAll(Material matDecal, Matrix4x4 invView, Mesh cube_mesh, Vector4 _FarCorner,RenderTexture GBuffer)
    {
        if (Application.isEditor||Application.platform== RuntimePlatform.WindowsPlayer)
        {
            _FarCorner.w = -1;
        }
        else
        {
            _FarCorner.w = 1.0f;
        }
        for (int i = 0; i < DecalList.Count; i++)
        {
            DeferredDecal decal = DecalList[i];

            if (decal.color.a <= 0.001f)
            {
                continue;
            }
            Matrix4x4 invProject = decal.transform.worldToLocalMatrix;
            invProject = invProject*invView;
            matDecal.SetMatrix("_ViewToProjector", invProject);
            matDecal.SetColor("_Color", decal.color * decal.color_scale);
            matDecal.SetTexture("_GBufferTex", GBuffer);
            matDecal.SetTexture("_MainTex", decal.decal_tex);

            Vector4 size = decal.transform.lossyScale;
            size.w = decal.roughness;
            matDecal.SetVector("_Size", size);
            matDecal.SetVector("_FarCorner", _FarCorner);

            if (((int)decal.decaltype & 1) == (int)DecalType.Diffuse)
            {
                matDecal.SetPass(0);
                Graphics.DrawMeshNow(cube_mesh, decal.transform.localToWorldMatrix, 0);
            }
            if (((int)decal.decaltype & 2) == (int)DecalType.Roughness)
            {
                matDecal.SetPass(1);
                Graphics.DrawMeshNow(cube_mesh, decal.transform.localToWorldMatrix, 0);
            }
            
            
        }
    }
    public static void DrawAllNormal(Material matDecal, Matrix4x4 invView, Mesh cube_mesh, Vector4 _FarCorner, RenderTexture GBuffer)
    {

    }
	// Update is called once per frame


    void OnDrawGizmos()
    {

        //Graphics.Draw
        Gizmos.DrawIcon(transform.position, "Projector.png");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = color;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, size);
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = Color.white;
    }
}
