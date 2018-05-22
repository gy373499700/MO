using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;


[CustomEditor(typeof(MaterialAnimation))]
public class MaterialAnimationEditor : UnityEditor.Editor
{
    public int select = 0;
    public override void OnInspectorGUI()// Rect position,SerializedProperty property,GUIContent label)
    {
        //EditorGUI.TagField
        base.OnInspectorGUI();
        
        MaterialAnimation ma = target as MaterialAnimation;
        if (ma != null)
        {
            Renderer r = ma.GetComponent<Renderer>();
            if(r!=null)
            {
                Material[] mats = r.sharedMaterials;
                if(mats!=null && mats.Length>0)
                {
                    if(mats[0].shader!= null)
                    {
                        int count = ShaderUtil.GetPropertyCount(mats[0].shader);
                        List<string> lstPorp = new List<string>();
                        List<int> lstPropType = new List<int>();
                        for(int i=0;i< count;i++)
                        {
                            string prop_name = ShaderUtil.GetPropertyName(mats[0].shader, i);
                            ShaderUtil.ShaderPropertyType t = ShaderUtil.GetPropertyType(mats[0].shader, i);
                            if(t == ShaderUtil.ShaderPropertyType.Color ||
                                t == ShaderUtil.ShaderPropertyType.Float||
                                t == ShaderUtil.ShaderPropertyType.Vector||
                                t == ShaderUtil.ShaderPropertyType.Range)
                            {
                                lstPorp.Add(prop_name);
                                lstPropType.Add((int)t);
                            }
                        }
                        GUIContent[] contents = new GUIContent[lstPorp.Count];
                        GUILayoutOption[] opts = new GUILayoutOption[lstPorp.Count];
                        for (int i=0;i< lstPorp.Count;i++)
                        {
                            //EditorGUILayout.Popup(select,GUIContent.)
                            contents[i] = new GUIContent(lstPorp[i]);
                            opts[i] = GUILayout.Width(80f);
                            if(ma.PropertyName == lstPorp[i])
                            {
                                select = i;
                            }
                        }

                        select  =   EditorGUILayout.Popup(select, contents, opts);
                        ma.PropertyName = lstPorp[select];
                        int select_type = lstPropType[select];
                        if (select_type == (int)ShaderUtil.ShaderPropertyType.Color ||
                            select_type == (int)ShaderUtil.ShaderPropertyType.Vector)
                        {
                            ma.ktype = MaterialAnimation.KeyType.Vector;
                        }
                        else if(select_type == (int)ShaderUtil.ShaderPropertyType.Float ||
                                select_type == (int)ShaderUtil.ShaderPropertyType.Range)
                        {
                            ma.ktype = MaterialAnimation.KeyType.Float;
                        }
                        
                    }
                }
            }
            //if (GUILayout.Button(""))
            //{
            //
            //}
        }
    }
}