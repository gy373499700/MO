// Lutify - Unity Asset
// Copyright (c) 2015 - Thomas Hourdel
// http://www.thomashourdel.com

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Lutify))]
public class LutifyEditor : Editor
{
	SerializedProperty p_LookupTexture;
	SerializedProperty p_Split;
	SerializedProperty p_ForceCompatibility;
	SerializedProperty p_LutFiltering;
	SerializedProperty p_Blend;
    SerializedProperty m_Lut3D;
    SerializedProperty Shader2D;
    SerializedProperty Shader3D;
    SerializedProperty m_Material2D;
    SerializedProperty m_Material3D;
    void OnEnable()
	{

		p_LookupTexture = serializedObject.FindProperty("LookupTexture");
		p_Split = serializedObject.FindProperty("Split");
		p_ForceCompatibility = serializedObject.FindProperty("ForceCompatibility");
		p_LutFiltering = serializedObject.FindProperty("LutFiltering");
		p_Blend = serializedObject.FindProperty("Blend");
        m_Lut3D = serializedObject.FindProperty("m_Lut3D");
        Shader2D = serializedObject.FindProperty("Shader2D");
        Shader3D = serializedObject.FindProperty("Shader3D");
        m_Material2D = serializedObject.FindProperty("m_Material2D");
        m_Material3D = serializedObject.FindProperty("m_Material3D");
    }

	public override void OnInspectorGUI()
	{
    
		serializedObject.Update();

		Texture2D lut = (Texture2D)p_LookupTexture.objectReferenceValue;

		EditorGUILayout.BeginHorizontal();
		{
			EditorGUILayout.PrefixLabel("Lookup Texture");

			EditorGUILayout.BeginHorizontal();
			{
				lut = (Texture2D)EditorGUILayout.ObjectField(lut, typeof(Texture2D), false);
				if (GUILayout.Button("N", EditorStyles.miniButton)) lut = null;
			}
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndHorizontal();

		p_LookupTexture.objectReferenceValue = lut;

		EditorGUILayout.PropertyField(p_Split);
		EditorGUILayout.PropertyField(p_ForceCompatibility);
		EditorGUILayout.PropertyField(p_LutFiltering);
		EditorGUILayout.PropertyField(p_Blend);
        EditorGUILayout.PropertyField(m_Lut3D);
        EditorGUILayout.PropertyField(Shader2D);
        EditorGUILayout.PropertyField(Shader3D);
        EditorGUILayout.PropertyField(m_Material2D);
        EditorGUILayout.PropertyField(m_Material3D);
        if (LutifyBrowser.inst == null)
		{
			if (GUILayout.Button("Open LUT Gallery"))
				LutifyBrowser.Init(target as Lutify);
		}
		else
		{
			if (GUILayout.Button("Close LUT Gallery"))
				LutifyBrowser.inst.Close();
		}

		serializedObject.ApplyModifiedProperties();
	}
}
