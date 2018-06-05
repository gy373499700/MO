using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(Material),true)]
public class sdMaterialEditor : MaterialEditor
{
    public GUILayoutOption[] opts = new GUILayoutOption[8]; 
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (isVisible)
        {
            Material mat = target as Material;
            StencilUI(mat);
            CullModeUI(mat);
        }
    }
    void CullModeUI(Material mat)
    {
        if (!mat.HasProperty("CullMode"))
        {
            return;
        }
        int cullmode = mat.GetInt("CullMode");
        string content;
        if(cullmode==0)
        {
            content = "Cull Off";
        }
        else if (cullmode == 1)
        {
            content = "Cull Front";
        }
        else 
        {
            content = "Cull Back";
        }

        if (GUILayout.Button(content))
        {
            cullmode++;
            cullmode = cullmode % 3;
        }
        mat.SetInt("CullMode", cullmode);
    }
    void StencilUI(Material mat)
    {
        if (!mat.HasProperty("StencilRef"))
        {
            return;
        }
        int StencilRef = mat.GetInt("StencilRef");
        //bool XRay = (StencilRef & 1) == 1;
        bool Whiting = (StencilRef & 2) == 2;
        bool SelfIllum = (StencilRef & 4) == 4;
        bool SSS = (StencilRef & 8) == 8;
        bool RimLight = (StencilRef & 16) == 16;
        string content;
        //if (XRay)
        //{
        //    content = "XRay(On)";
        //}
        //else
        //{
        //    content = "XRay(Off)";
        //}
        //
        //if (GUILayout.Button(content))
        //{
        //    StencilRef ^= 1;
        //}

        //if (Whiting)
        //{
        //    content = "Whiting(On)";
        //}
        //else
        //{
        //    content = "Whiting(Off)";
        //}

        //if (GUILayout.Button(content))
        {
            StencilRef &= 252;
        }
        if (SelfIllum)
        {
            content = "SelfIllum(On)";
        }
        else
        {
            content = "SelfIllum(Off)";
        }

        if (GUILayout.Button(content))
        {
            StencilRef ^= 4;
        }
        if (SSS)
        {
            content = "SSS(On)";
        }
        else
        {
            content = "SSS(Off)";
        }

        if (GUILayout.Button(content))
        {
            StencilRef ^= 8;
        }
        if (RimLight)
        {
            content = "RimLight(On)";
        }
        else
        {
            content = "RimLight(Off)";
        }

        if (GUILayout.Button(content))
        {
            StencilRef ^= 16;
        }

        mat.SetInt("StencilRef", StencilRef);
    }
}