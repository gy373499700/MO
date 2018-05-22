using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class sdAreaRenderSetting : MonoBehaviour {
    public static List<sdAreaRenderSetting> lstArea = new List<sdAreaRenderSetting>();
    public static sdAreaRenderSetting last_area = null;
    public int Layer = 0;
    public Vector3 Size = Vector3.one;
    public Color AmbientColor = Color.white;
    public float AmbientScale = 1;
    public Color MainLightColor = Color.white;
    public float MainLightScale = 1;
    public Vector3 MainLightDirection = new Vector3(45, 0, 0);
    public Color FogColor = Color.white;
    public float FogStart = 10;
    public float FogEnd = 100;
    public float FogHeight = 10.0f;
    public float Rain = 0.0f;
    public Texture RainEnvCube = null;
    public float FadeTime = 1.0f;
    
    public GameObject EffectObject = null;
    public bool RotateEffect = false;
    float current_time = 0.0f;
    Color BakeAmbientColor = Color.white;
    float BakeAmbientScale = 1;
    Color BakeMainLightColor = Color.white;
    float BakeMainLightScale = 1;
    Vector3 BakeMainLightDirection = Vector3.zero;
    Color BakeFogColor = Color.white;
    float BakeFogStart = 10;
    float BakeFogEnd = 100;
    float BakeFogHeight = 10.0f;
    float BakeFadeTime = 1.0f;
    float BakeRain = 0.0f;
    Texture BakeRainEnvCube = null;

    Color gizmos_color = new Color(1, 1, 1, 0.5f);
    
    void Awake()
    {
        gizmos_color    =   new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 0.5f);
    }
    // Use this for initialization
    void OnEnable () {
        
        lstArea.Add(this);
    }
    void OnDisable()
    {
        lstArea.Remove(this);
    }
    public void OnEnter()
    {
        current_time = 0.0f;
        if (SceneRenderSetting._Setting != null)
        {
            BakeAmbientColor = SceneRenderSetting._Setting.AmbientColor;
            BakeAmbientScale = SceneRenderSetting._Setting.AmbientColorScale;
            BakeMainLightColor = SceneRenderSetting._Setting.MainLightColor;
            BakeMainLightScale = SceneRenderSetting._Setting.MainLightColorScale;
            BakeMainLightDirection = SceneRenderSetting._Setting.MainLightDirection;
            BakeFogColor = SceneRenderSetting._Setting.Fog_Color;
            BakeFogStart = SceneRenderSetting._Setting.Fog_Start;
            BakeFogEnd = SceneRenderSetting._Setting.Fog_End;
            BakeFogHeight = SceneRenderSetting._Setting.Fog_Height;
            BakeRain = SceneRenderSetting._Setting.FullScreenRain;
            BakeRainEnvCube = SceneRenderSetting._Setting.RainEnvCube;
            SceneRenderSetting._Setting.RainEnvCube = RainEnvCube;
        }
        if (EffectObject != null)
        {
            EffectObject.SetActive(true);
        }

        
    }
    public void OnLeave()
    {
        if (EffectObject != null)
        {
            EffectObject.SetActive(false);
        }
        if (SceneRenderSetting._Setting != null)
        {
            SceneRenderSetting._Setting.RainEnvCube = BakeRainEnvCube;
        }
    }
    public void Tick()
    {
        if(Application.isPlaying)
        {
            current_time += Time.deltaTime;
        }
        else
        {
            current_time += 0.1f;
        }
        if(SceneRenderSetting._Setting!= null)
        {
            float f = current_time / FadeTime;
            if(f > 1.0f)
            {
                f = 1.0f;
            }
            SceneRenderSetting._Setting.AmbientColor = Color.Lerp(BakeAmbientColor, AmbientColor, f);
            SceneRenderSetting._Setting.AmbientColorScale = BakeAmbientScale * (1 - f) + AmbientScale * f;
            SceneRenderSetting._Setting.MainLightColor = Color.Lerp(BakeMainLightColor,MainLightColor, f);
            SceneRenderSetting._Setting.MainLightColorScale = BakeMainLightScale*(1-f)+MainLightScale*f;
            SceneRenderSetting._Setting.MainLightDirection = Vector3.Lerp(BakeMainLightDirection,MainLightDirection, f);

            SceneRenderSetting._Setting.Fog_Color= BakeFogColor * (1 - f) + FogColor * f;
            SceneRenderSetting._Setting.Fog_Start = BakeFogStart * (1 - f) + FogStart * f;
            SceneRenderSetting._Setting.Fog_End= BakeFogEnd * (1 - f) + FogEnd * f;
            SceneRenderSetting._Setting.Fog_Height = BakeFogHeight * (1 - f) + FogHeight * f;
            SceneRenderSetting._Setting.FullScreenRain = BakeRain*(1-f)+Rain*f;
            SceneRenderSetting._Setting.RainEnvCube = RainEnvCube;
            if (Application.isPlaying)
            {
                if (EffectObject != null)
                {
                    EffectObject.transform.position = SceneRenderSetting._Setting.transform.TransformPoint(Vector3.forward*5.0f);
                    if (RotateEffect)
                    {
                        EffectObject.transform.rotation = SceneRenderSetting._Setting.transform.rotation;
                    }
                }
            }
        }

    }
    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "area.png");
        //Matrix4x4 mat = new Matrix4x4();
        //mat.SetTRS(transform.position,transform.rotation,transform.g)
        
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmos_color;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(Vector3.zero, Size);

        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = Color.white;
    }
    public bool IsInArea(Vector3 point)
    {
        if(Layer==-1)
        {
            return true;
        }
        Vector3 p = transform.InverseTransformPoint(point);
        Vector3 half = Size * 0.5f;
        if(p.x  > -half.x  && p.x < half.x &&
           p.y > -half.y && p.y < half.y &&
           p.z > -half.z && p.z < half.z )
        {
            return true;
        }
        return false;
    }
	// Update is called once per frame
	public static void CheckArea (Vector3 point) {
        sdAreaRenderSetting area = null;
        int area_layer = -1000;
        for (int i=0;i<lstArea.Count;i++)
        {
            if(lstArea[i].IsInArea(point))
            {
                if (area_layer < lstArea[i].Layer)
                {
                    area = lstArea[i];
                    area_layer = area.Layer;
                }
                
            }
        }
        if (last_area != area)
        {
            if (last_area != null)
            {
                last_area.OnLeave();
            }
            if (area != null)
            {
                area.OnEnter();
            }
        }

        if (area != null)
        {
            area.Tick();
        }
        last_area = area;
    }
}
