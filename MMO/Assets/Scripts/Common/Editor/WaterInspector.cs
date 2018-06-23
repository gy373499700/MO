using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Water))]
public class WaterInspector : Editor
{
    bool baseInfoFold = true;
    bool waterEffectFold = false;
    bool gerstnerParamFold = true;
    bool customMeshFold = true;
    bool syncMoveFold = true;
	bool CausticsFold = false;
    Water.WaterType lastType;
    Water _water;

    void OnEnable()
    {
        _water = (Water)target;
        lastType = _water.type;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        //基本信息
        EditorGUI.BeginChangeCheck();
        baseInfoFold = EditorGUILayout.Foldout(baseInfoFold, "基本信息");
        if (baseInfoFold)
        {
            _water.color = EditorGUILayout.ColorField("主颜色", _water.color);
            _water.deepcolor = EditorGUILayout.ColorField("深度颜色", _water.deepcolor);
            _water.reflectColor = EditorGUILayout.ColorField("反射颜色", _water.reflectColor);
            EditorGUILayout.Space();
            _water.renderMesh = EditorGUILayout.ObjectField("渲染模型", _water.renderMesh, typeof(Mesh), false) as Mesh;
            _water._NormalTex = EditorGUILayout.ObjectField("法线贴图", _water._NormalTex, typeof(Texture2D), false) as Texture2D;
			CausticsFold = EditorGUILayout.Foldout(CausticsFold, "焦散贴图");
			if (CausticsFold) 
			{
				if (_water._CausticsTex == null)
					_water._CausticsTex = new Texture2D[16];
				int len = _water._CausticsTex.Length;
			//	EditorGUILayout.BeginScrollView ();
				for(int i=0;i<16;i++)
					_water._CausticsTex[i] = EditorGUILayout.ObjectField ("焦散贴图", _water._CausticsTex[i], typeof(Texture2D), false) as Texture2D;
			//	EditorGUILayout.EndScrollView ();
			}
            _water.refract_scale = EditorGUILayout.FloatField("折射系数", _water.refract_scale);
            _water.water_refract_y_depth = EditorGUILayout.FloatField("折射深度(Y)", _water.water_refract_y_depth);
            _water.water_fog_distance = EditorGUILayout.FloatField("水面雾效距离", _water.water_fog_distance);
            _water.water_fog_y_depth = EditorGUILayout.FloatField("水下雾效深度(Y)", _water.water_fog_y_depth);
            _water.frenel_scale = EditorGUILayout.FloatField("菲涅尔系数", _water.frenel_scale);
            _water.first_wave_scale = EditorGUILayout.FloatField("水面纹理1强度", _water.first_wave_scale);
            _water.first_wave_speed = EditorGUILayout.FloatField("水面纹理1速度", _water.first_wave_speed);
            _water.second_wave_scale = EditorGUILayout.FloatField("水面纹理2强度", _water.second_wave_scale);
            _water.second_wave_speed = EditorGUILayout.FloatField("水面纹理2速度", _water.second_wave_speed);
            _water.wave_dir = EditorGUILayout.Vector2Field("水面纹理方向", _water.wave_dir);
            _water.CausticsLerp = EditorGUILayout.FloatField("焦散系数", _water.CausticsLerp);
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //水面效果
        waterEffectFold = EditorGUILayout.Foldout(waterEffectFold, "水面效果");
        if (waterEffectFold)
        {
            _water.enableRefl = EditorGUILayout.Toggle("反射", _water.enableRefl);
            _water.enableSubWater = EditorGUILayout.Toggle("水下效果", _water.enableSubWater);
            _water.enableCausitic = EditorGUILayout.Toggle("水下焦散效果", _water.enableCausitic);
            ShieldField();
            if (_water.enableRefl)
            {
                _water.reflectionPlaneOffset = EditorGUILayout.FloatField("反射平面高度", _water.reflectionPlaneOffset);
            }

            if (_water.type != lastType)
            {
                lastType = _water.type;
                if (lastType == Water.WaterType.lake)
                {
                    _water.extendEdge = false;
                    _water.bActiveFollowCam = false;
                    _water.customMesh = false;
                    _water.enableWave = false;
                    _water.use_mesh_normal = false;
                }
                else if (lastType == Water.WaterType.ocean)
                {
                    _water.extendEdge = true;
                    _water.bActiveFollowCam = true;
                    _water.customMesh = true;
                    _water.enableWave = true;
                    _water.use_mesh_normal = true;
                }
            }

            _water.type = (Water.WaterType)EditorGUILayout.EnumPopup("水面类型", _water.type);
            if (_water.type == Water.WaterType.ocean)
            {
                _water.enableWave = EditorGUILayout.Toggle("海浪", _water.enableWave);
                _water.use_mesh_normal = _water.enableWave;
                if (_water.enableWave)
                {
                    //gWave
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Space();

                    gerstnerParamFold = EditorGUILayout.Foldout(gerstnerParamFold, "G波参数");
                    if (!gerstnerParamFold)
                    {
                        GUILayout.FlexibleSpace();
                    }

                    if (gerstnerParamFold)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.BeginVertical();
                        _water._GerstnerIntensity = EditorGUILayout.FloatField("强度", _water._GerstnerIntensity);
                        _water._GSpeed = EditorGUILayout.Vector4Field("速度", _water._GSpeed);
                        _water._GAmplitude = EditorGUILayout.Vector4Field("权重", _water._GAmplitude);
                        _water._GFrequency = EditorGUILayout.Vector4Field("间隔", _water._GFrequency);
                        _water._GSteepness = EditorGUILayout.Vector4Field("阻尼", _water._GSteepness);
                        _water._GDirectionAB = EditorGUILayout.Vector4Field("A浪B浪方向", _water._GDirectionAB);
                        _water._GDirectionCD = EditorGUILayout.Vector4Field("C浪D浪方向", _water._GDirectionCD);
                        _water._FlowDir = EditorGUILayout.Vector4Field("纹理扭曲强度", _water._FlowDir);
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();
                    //customMesh
                    _water.customMesh = EditorGUILayout.Toggle("自定义网格", _water.customMesh);
                    if (_water.customMesh)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.Space();
                        customMeshFold = EditorGUILayout.Foldout(customMeshFold, "自定义网格参数");
                        if (!customMeshFold)
                            GUILayout.FlexibleSpace();

                        if (customMeshFold)
                        {
                            EditorGUILayout.Space();
                            EditorGUILayout.BeginVertical();

                            _water.dirMode = (Water.DirectMode)EditorGUILayout.EnumPopup("方向模式", _water.dirMode);
                            _water.row = EditorGUILayout.IntField("行数", _water.row);
                            _water.column = EditorGUILayout.IntField("列数", _water.column);
                            _water.debugMesh = EditorGUILayout.Toggle("网格调试", _water.debugMesh);

                            EditorGUILayout.EndVertical();
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.Space();
                    //syncMove
                    if (_water.customMesh)
                    {
                        _water.bActiveFollowCam = EditorGUILayout.Toggle("跟随模式(无限大)", _water.bActiveFollowCam);
                        if (_water.bActiveFollowCam)
                        {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.Space();
                            syncMoveFold = EditorGUILayout.Foldout(syncMoveFold, "跟随参数");
                            if (!syncMoveFold)
                                GUILayout.FlexibleSpace();

                            if (syncMoveFold)
                            {
                                EditorGUILayout.Space();
                                EditorGUILayout.BeginVertical();
                                _water.followMode = (Water.FollowMode)EditorGUILayout.EnumPopup("跟随模式", _water.followMode);
                                if (_water.followMode == Water.FollowMode.RotAndPos)
                                {
                                    _water.validSize = EditorGUILayout.DelayedFloatField("有效区域(高精度|波浪)", _water.validSize);
                                    _water.extendEdge = EditorGUILayout.Toggle("边缘扩展", _water.extendEdge);
                                    _water.noWaveSize = EditorGUILayout.FloatField("扩展距离", _water.noWaveSize);
                                }
                                else
                                {
                                    _water.xMove = EditorGUILayout.Toggle("X轴跟随", _water.xMove);
                                    _water.yMove = EditorGUILayout.Toggle("Y轴跟随", _water.yMove);
                                }

                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                }
            } else if (_water.type == Water.WaterType.lake)
            {

            }

            if (EditorGUI.EndChangeCheck())
            {
                if (!EditorApplication.isPlaying)
                {
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                        UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
                }
            }
            EditorGUILayout.EndVertical();
        }
    }

    /// <summary>
    /// 反射遮蔽模块
    /// </summary>
    void ShieldField()
    {
        if (_water.enableRefl)
        {
            _water.enableReflShield = EditorGUILayout.Toggle("反射遮罩", _water.enableReflShield);
            if (_water.enableReflShield)
            {
                serializedObject.Update();
                EditorGUI.BeginChangeCheck();
                SerializedProperty cylinderProp = serializedObject.FindProperty("cylinderList");
                EditorGUILayout.PropertyField(cylinderProp, new GUIContent("圆柱遮罩"), true);
                SerializedProperty sphereProp = serializedObject.FindProperty("sphereList");
                EditorGUILayout.PropertyField(sphereProp, new GUIContent("球形遮罩"), true);
                if(EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
