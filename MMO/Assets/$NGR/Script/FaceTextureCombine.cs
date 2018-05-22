using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[ExecuteInEditMode]
public class FaceTextureCombine : Singleton<FaceTextureCombine> {
    public Vector2 FaceTextureSize = new Vector2(512,512);
    public Vector2 LeftEyeCenter = new Vector2(185, 207);
    public Vector2 RightEyeCenter = new Vector2(328, 207);
    public Vector2 MouseCenter = new Vector2(257, 291);
    public Texture2D eye_texture;
    public Texture2D mouse_texture;
    public Texture2D face_man_texture;
    public Texture2D face_female_texture;

    public string[] eye_name;
    public Vector4[] eye_pos;
    public string[] mouse_name;
    public Vector4[] mouse_pos;
    public Material matBase = null;
    public Material matBasemainchar = null;
    public Material matBaseInUI = null;

    public Shader GetMainCharShader()
    {
        if (matBasemainchar != null)
            return matBasemainchar.shader;
        return null;
    }
    // Use this for initialization
    public void Init () {
       /* sdFileSystem.Instance.SetDontUnloadFile("Model/$MainChar_Face/face_hero.png", true);
        sdFileSystem.Instance.SetDontUnloadFile("Model/$MainChar_Face/face_heroine.png", true);
        sdFileSystem.Instance.SetDontUnloadFile("Model/$MainChar_Face/eye.png", true);
        sdFileSystem.Instance.SetDontUnloadFile("Model/$MainChar_Face/mouth.png", true);
        sdFileSystem.Instance.SetDontUnloadFile("Model/$MainChar_Face/Face.mat", true);
        sdFileSystem.Instance.SetDontUnloadFile("Model/$MainChar_Face/FaceInUI.mat", true);
        sdFileSystem.Instance.SetDontUnloadFile("Model/$MainChar_Face/MainCharFace.mat", true);

        face_man_texture = sdFileSystem.Instance.Load("Model/$MainChar_Face/face_hero.png") as Texture2D;
        face_female_texture = sdFileSystem.Instance.Load("Model/$MainChar_Face/face_heroine.png") as Texture2D;
        eye_texture =   sdFileSystem.Instance.Load("Model/$MainChar_Face/eye.png") as Texture2D;
        mouse_texture = sdFileSystem.Instance.Load("Model/$MainChar_Face/mouth.png") as Texture2D;
        string eye_cfg = sdFileSystem.Instance.Load("Model/$MainChar_Face/eye.txt.csv") as string;
        if(eye_cfg!=null)
        {
            LoadElement(eye_cfg, ref eye_name, ref eye_pos);
        }
        string mouse_cfg = sdFileSystem.Instance.Load("Model/$MainChar_Face/mouth.txt.csv") as string;
        if (mouse_cfg != null)
        {
            LoadElement(mouse_cfg, ref mouse_name, ref mouse_pos);
        }
        matBase = sdFileSystem.Instance.Load("Model/$MainChar_Face/Face.mat") as Material;
        matBaseInUI = sdFileSystem.Instance.Load("Model/$MainChar_Face/FaceInUI.mat") as Material;
        matBasemainchar = sdFileSystem.Instance.Load("Model/$MainChar_Face/MainCharFace.mat") as Material;
        //Debug.LogError("matBaseInUI color "+ matBaseInUI.color);
        */
    }

    void LoadElement(string content,ref string[] name_array,ref Vector4[] pos_array)
    {
        List<string> lstName = new List<string>();
        List<Vector4> lstPos = new List<Vector4>();
        string[] lines = content.Split('\n');
        for(int i=1;i< lines.Length;i++)
        {
            if(lines[i].Length==0)
            {
                continue;
            }
            string[] l = lines[i].Split(',');
            Vector4 v = new Vector4();
            v.x = float.Parse(l[1]);
            v.y = float.Parse(l[2]);
            v.z = float.Parse(l[3]);
            v.w = float.Parse(l[4]);

            lstName.Add(l[0]);
            lstPos.Add(v);
        }

        name_array = lstName.ToArray();
        pos_array = lstPos.ToArray();

    }
    // Update is called once per frame
    void Update () {

	}
   
    public void SetMaterial(Material tFaceMat, string left,string mouse)
    {
        Vector4 leftpos = GetPos(left, eye_name, eye_pos);
        Vector4 mousepos = GetPos(mouse, mouse_name, mouse_pos);

        Vector4 show_area = Vector4.zero;
        Vector4 sample_uv = Vector4.zero;
        Calc(leftpos, LeftEyeCenter, 512, ref show_area, ref sample_uv);
        tFaceMat.SetVector("_LeftEyeTex_Src", show_area);
        tFaceMat.SetVector("_LeftEyeTex_ST_Dest", sample_uv);

        Calc(leftpos, RightEyeCenter, 512, ref show_area, ref sample_uv);
        tFaceMat.SetVector("_RightEyeTex_Src", show_area);

        Calc(mousepos, MouseCenter, 512, ref show_area, ref sample_uv);
        tFaceMat.SetVector("_MouseTex_Src", show_area);
        tFaceMat.SetVector("_MouseTex_ST_Dest", sample_uv);
    }

    public void SetMouseMaterial(Material tFaceMat,  string mouse)
    {
        Vector4 mousepos = GetPos(mouse, mouse_name, mouse_pos);
        Vector4 show_area = Vector4.zero;
        Vector4 sample_uv = Vector4.zero;

        Calc(mousepos, MouseCenter, 512, ref show_area, ref sample_uv);
        tFaceMat.SetVector("_MouseTex_Src", show_area);
        tFaceMat.SetVector("_MouseTex_ST_Dest", sample_uv);
    }

    public void SetEyesMaterial(Material tFaceMat, string left)
    {
        Vector4 leftpos = GetPos(left, eye_name, eye_pos);
        Vector4 show_area = Vector4.zero;
        Vector4 sample_uv = Vector4.zero;
        Calc(leftpos, LeftEyeCenter, 512, ref show_area, ref sample_uv);
        tFaceMat.SetVector("_LeftEyeTex_Src", show_area);
        tFaceMat.SetVector("_LeftEyeTex_ST_Dest", sample_uv);

        Calc(leftpos, RightEyeCenter, 512, ref show_area, ref sample_uv);
        tFaceMat.SetVector("_RightEyeTex_Src", show_area);
    }



    void Calc(Vector4 pos_size,Vector2 center,float size,ref Vector4 show_area,ref Vector4 sample_uv)
    {
        show_area.x = center.x / size;
        show_area.y = (size-center.y) / size;

        show_area.z = pos_size.z / size;
        show_area.w = pos_size.w / size;

        show_area.x -= show_area.z * 0.5f;
        show_area.y -= show_area.w * 0.5f;

        //show_area.y = 1 - show_area.y;

        //tex_size 1024
        int tex_size = eye_texture.width;

        pos_size.y += pos_size.w;
        pos_size.y = tex_size - pos_size.y;
        sample_uv = pos_size / tex_size;

        //sample_uv.y = 1 - sample_uv.y;
    }

    Vector4 GetPos(string sprite_name,string[] name_array,Vector4[] pos_array)
    {
        for(int i=0;i< name_array.Length;i++)
        {
            if(name_array[i] == sprite_name)
            {
                return pos_array[i];
            }
        }
        return Vector4.zero;
    }
}
