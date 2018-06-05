using UnityEngine;
using System.Collections;

public class PropertyModify : MonoBehaviour {
    public enum Property
    {
        none,
        LightDirectionX,
        LightDirectionY,
        SSAORadius,
        DirectionLightScale,
        AmbientColorLight,
        Dof_Focus,
        ReflectIntensity,
        RainIntensity,
        BloomIntensity,
        SunShaftSize,
        LutifyColor,
        LutifyAphpa,
        ShadowLevel,
    }
    public float RangeSize = 1f;
    UISlider slider = null;
    public Property property = Property.none;
    private void OnEnable()
    {
        OnOpen();
    }
    void Start()
    {
        slider = transform.GetComponent<UISlider>();
        //slider.onDragFinished += OnDragFinished;
        EventDelegate.Add(slider.onChange, OnDragChange);
        OnOpen();
    }
    void OnOpen()
    {
        if (slider == null) return;
        if (property == Property.LightDirectionX)
        {
            slider.value = SceneRenderSetting._Setting.MainLightDirection.x / 180f;
        }
        else if (property == Property.LightDirectionY)
        {
            slider.value = SceneRenderSetting._Setting.MainLightDirection.y / 360f;
        }
        else if (property == Property.SSAORadius)
        {
            slider.value = SceneRenderSetting._Setting.SSAOSampleRadius / RangeSize;
        }
        else if (property == Property.DirectionLightScale)
        {
            slider.value = SceneRenderSetting._Setting.MainLightColorScale / RangeSize;
        }
        else if (property == Property.AmbientColorLight)
        {
            slider.value = SceneRenderSetting._Setting.AmbientColorScale / RangeSize;
        }
        else if (property == Property.Dof_Focus)
        {
            slider.value = SceneRenderSetting._Setting.DOF_Focus / RangeSize;
        }
        else if (property == Property.ReflectIntensity)
        {
            slider.value = SceneRenderSetting._Setting.ReflectIntensity / RangeSize;
        }
        else if (property == Property.RainIntensity)
        {
            slider.value = SceneRenderSetting._Setting.FullScreenRain / RangeSize;
        }
        else if (property == Property.BloomIntensity)
        {
            slider.value = SceneRenderSetting._Setting.BloomLuminance / RangeSize;
        }
        else if (property == Property.SunShaftSize)
        {
            slider.value = SceneRenderSetting._Setting.LightShaft_Intensity / RangeSize;
        }
        else if (property == Property.LutifyColor)
        {
            slider.value = SceneRenderSetting._Setting.LutifyColor / RangeSize;
        }
        else if (property == Property.LutifyAphpa)
        {
            slider.value = SceneRenderSetting._Setting.LutifyAlpha / RangeSize;
        }
        else if (property == Property.ShadowLevel)
        {
                if(RenderPipeline._instance!=null)
                slider.value = RenderPipeline._instance.quality.ShadowLevel / RangeSize;
        }

    }
    void OnDragChange()
    {
        float t = slider.value;
        if (property== Property.LightDirectionX)
        {
            SceneRenderSetting._Setting.MainLightDirection.x = slider.value * 180f;

        }
        else if (property == Property.LightDirectionY)
        {
            SceneRenderSetting._Setting.MainLightDirection.y = slider.value * 360f;
        }
        else if (property == Property.SSAORadius)
        {
            SceneRenderSetting._Setting.SSAOSampleRadius = slider.value * RangeSize;
        }
        else if (property == Property.DirectionLightScale)
        {
            SceneRenderSetting._Setting.MainLightColorScale = slider.value * RangeSize;
        }
        else if (property == Property.AmbientColorLight)
        {
            SceneRenderSetting._Setting.AmbientColorScale = slider.value * RangeSize;
        }
        else if (property == Property.Dof_Focus)
        {
            SceneRenderSetting._Setting.DOF_Focus = slider.value * RangeSize;
        }
        else if (property == Property.ReflectIntensity)
        {
            SceneRenderSetting._Setting.ReflectIntensity = slider.value * RangeSize;
        }
        else if (property == Property.RainIntensity)
        {
            SceneRenderSetting._Setting.FullScreenRain = slider.value * RangeSize;
        }
        else if (property == Property.BloomIntensity)
        {
            SceneRenderSetting._Setting.BloomLuminance = slider.value * RangeSize;
        }
        else if (property == Property.SunShaftSize)
        {
            SceneRenderSetting._Setting.LightShaft_Intensity = slider.value * RangeSize;
        }
        else if (property == Property.LutifyColor)
        {
            SceneRenderSetting._Setting.LutifyColor = (int)(slider.value * RangeSize);
        }
        else if (property == Property.LutifyAphpa)
        {
            SceneRenderSetting._Setting.LutifyAlpha = slider.value * RangeSize;
        }

        else if (property == Property.ShadowLevel)
        {
            if (RenderPipeline._instance != null)
                RenderPipeline._instance.quality.ShadowLevel = (int)(slider.value * RangeSize);
        }

    }

    // Update is called once per frame
    void Update () {
	
	}
}
