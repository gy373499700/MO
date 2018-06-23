using UnityEngine;
using System.Collections;

public class ToggleModify : MonoBehaviour {
    public enum ToggleSelect
    {
        None,
        EnableFog,
        EnableFogPlane,
        EnableReflect,
        EnableSSAO,
        EnableSSAODebug,
        EnableSubWater,
        EnableSubgausitic,
        EnableDof,
        EnableToneMapping,
        EnableSunShaft,
        EnableFXAA,
        EnableNormal,
        EnableTwist,
        EnableBlur,
        EnbaleBloom,
        EnableUIMAsk,
        EnableLutify,
        EnbaleMoveBlur,
        EnableInputBlur,
        EnableDepth,
        EnableSSAOPro,
        EnableWaterReflect,
    }
    UIToggle toggle = null;
    public ToggleSelect m_toggle = ToggleSelect.None;
    // Use this for initialization
    void OnEnable()
    {
        OnOpen();
    }
    void Start ()
    {
        toggle = transform.GetComponent<UIToggle>();
        EventDelegate.Add(toggle.onChange, OnChange);
        OnOpen();
    }
    void OnOpen()
    {
        if (toggle == null) return;
        if (m_toggle == ToggleSelect.EnableFog)
        {
            toggle.value = SceneRenderSetting._Setting.EnableFog;
        }
       else if (m_toggle == ToggleSelect.EnableFogPlane)
        {
            toggle.value = SceneRenderSetting._Setting.EnablePlaneFog;
        }
        else if (m_toggle == ToggleSelect.EnableFogPlane)
        {
            toggle.value = SceneRenderSetting._Setting.EnablePlaneFog;
        }
        else if (m_toggle == ToggleSelect.EnableReflect)
        {
            toggle.value = SceneRenderSetting._Setting.EnableReflect;
        }
        else if (m_toggle == ToggleSelect.EnableSSAO)
        {
            toggle.value = SceneRenderSetting._Setting.EnableSSAO;
        }
        else if (m_toggle == ToggleSelect.EnableSSAOPro)
        {
            toggle.value = SceneRenderSetting._Setting.EnableSSAOPro;
        }
        else if (m_toggle == ToggleSelect.EnableSSAODebug)
        {
            toggle.value = SceneRenderSetting._Setting.EnableSSAODebug;
        }
        else if (m_toggle == ToggleSelect.EnableDof)
        {
            toggle.value = SceneRenderSetting._Setting.EnableGlow;
        }
        else if (m_toggle == ToggleSelect.EnableToneMapping)
        {
            toggle.value = SceneRenderSetting._Setting.EnableToneMapping;
        }
        else if (m_toggle == ToggleSelect.EnableSunShaft)
        {
            toggle.value = SceneRenderSetting._Setting.EnableLightShaft;
        }
        else if (m_toggle == ToggleSelect.EnableFXAA)
        {
            toggle.value = SceneRenderSetting._Setting.EnableFXAA;
        }
        else if (m_toggle == ToggleSelect.EnableNormal)
        {
            toggle.value = SceneRenderSetting._Setting.ShowNormal;
        }
        else if (m_toggle == ToggleSelect.EnableTwist)
        {
            toggle.value = SceneRenderSetting._Setting.EnableTwistScreen;
        }
        else if (m_toggle == ToggleSelect.EnableBlur)
        {
            toggle.value = SceneRenderSetting._Setting.EnableGaussBlur;
        }
        else if (m_toggle == ToggleSelect.EnbaleBloom)
        {
            toggle.value = SceneRenderSetting._Setting.EnableBloom;
        }
        else if (m_toggle == ToggleSelect.EnableLutify)
        {
            toggle.value = SceneRenderSetting._Setting.EnableLutify;
        }
        else if (m_toggle == ToggleSelect.EnableBlur)
        {
            toggle.value = SceneRenderSetting._Setting.EnableGaussBlur;
        }
        else if (m_toggle == ToggleSelect.EnableUIMAsk)
        {
            toggle.value = SceneRenderSetting._Setting.DebugUIMask;
        }
        else if (m_toggle == ToggleSelect.EnbaleMoveBlur)
        {
            toggle.value = Player.EnableMoveBlur;
        }
        else if (m_toggle == ToggleSelect.EnableInputBlur)
        {
            toggle.value = SceneRenderSetting._Setting.EnableInputDisturb;
        }
        else if (m_toggle == ToggleSelect.EnableDepth)
        {
            toggle.value = SceneRenderSetting._Setting.ShowDepth;
        }
        else if (m_toggle == ToggleSelect.EnableSubWater)
        {
            Water water = GameObject.Find("SceneObject/Water").GetComponent<Water>();
            toggle.value = water.enableSubWater;
        }
        else if (m_toggle == ToggleSelect.EnableSubgausitic)
        {
            Water water = GameObject.Find("SceneObject/Water").GetComponent<Water>();
            toggle.value = water.enableCausitic;
        }
        else if (m_toggle == ToggleSelect.EnableWaterReflect)
        {
            Water water = GameObject.Find("SceneObject/Water").GetComponent<Water>();
            toggle.value = water.enableRefl;
        }
    }
    // Update is called once per frame
    void OnChange ()
    {
        bool t = toggle.value;
        if (m_toggle == ToggleSelect.EnableFog)
        {
            SceneRenderSetting._Setting.EnableFog=toggle.value;
        }
        else if (m_toggle == ToggleSelect.EnableFogPlane)
        {
            SceneRenderSetting._Setting.EnablePlaneFog = toggle.value;
        }
        else if (m_toggle == ToggleSelect.EnableReflect)
        {
            SceneRenderSetting._Setting.EnableReflect = toggle.value;
        }
        else if (m_toggle == ToggleSelect.EnableSSAO)
        {
            SceneRenderSetting._Setting.EnableSSAO = toggle.value;
        }
        else if (m_toggle == ToggleSelect.EnableSSAOPro)
        {
            SceneRenderSetting._Setting.EnableSSAOPro = toggle.value;
        }
        else if (m_toggle == ToggleSelect.EnableSSAODebug)
        {
            SceneRenderSetting._Setting.EnableSSAODebug = toggle.value;
        }
        else if (m_toggle == ToggleSelect.EnableDof)
        {
            SceneRenderSetting._Setting.EnableGlow = toggle.value;
        }
        else if (m_toggle == ToggleSelect.EnableToneMapping)
        {
            SceneRenderSetting._Setting.EnableToneMapping = toggle.value;
        }
        else if (m_toggle == ToggleSelect.EnableSunShaft)
        {
            SceneRenderSetting._Setting.EnableLightShaft = toggle.value;
        }
        else if (m_toggle == ToggleSelect.EnableFXAA)
        {
            SceneRenderSetting._Setting.EnableFXAA = toggle.value;
        }
        else if (m_toggle == ToggleSelect.EnableNormal)
        {
            SceneRenderSetting._Setting.ShowNormal = toggle.value;
        }
        else if (m_toggle == ToggleSelect.EnableTwist)
        {
            SceneRenderSetting._Setting.EnableTwistScreen = toggle.value;
        }
        else if (m_toggle == ToggleSelect.EnableFXAA)
        {
            SceneRenderSetting._Setting.EnableFXAA = toggle.value;
        }
        else if (m_toggle == ToggleSelect.EnableBlur)
        {
            SceneRenderSetting._Setting.EnableGaussBlur = toggle.value;
        }
        else if (m_toggle == ToggleSelect.EnbaleBloom)
        {
            SceneRenderSetting._Setting.EnableBloom = toggle.value;
        }
        else if (m_toggle == ToggleSelect.EnableLutify)
        {
            SceneRenderSetting._Setting.EnableLutify = toggle.value;
        }
        else if (m_toggle == ToggleSelect.EnableUIMAsk)
        {
            SceneRenderSetting._Setting.DebugUIMask = toggle.value;
        }
        else if (m_toggle == ToggleSelect.EnbaleMoveBlur)
        {
            Player.EnableMoveBlur=(toggle.value);

        }
        else if (m_toggle == ToggleSelect.EnableInputBlur)
        {
           SceneRenderSetting._Setting.EnableInputDisturb = toggle.value;
        }
        else if (m_toggle == ToggleSelect.EnableDepth)
        {
            SceneRenderSetting._Setting.ShowDepth = toggle.value;
        }
        else if (m_toggle == ToggleSelect.EnableSubWater)
        {
            Water water = GameObject.Find("SceneObject/Water").GetComponent<Water>();
            water.enableSubWater=toggle.value;
        }
        else if (m_toggle == ToggleSelect.EnableSubgausitic)
        {
            Water water = GameObject.Find("SceneObject/Water").GetComponent<Water>();
            water.enableCausitic = toggle.value;
        }
        else if (m_toggle == ToggleSelect.EnableWaterReflect)
        {
            Water water = GameObject.Find("SceneObject/Water").GetComponent<Water>();
            water.enableRefl = toggle.value;
        }
    }
}
