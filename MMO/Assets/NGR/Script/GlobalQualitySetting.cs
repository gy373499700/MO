using UnityEngine;
using System.Collections;
using System.Threading;
[System.Serializable]
public class GlobalQualitySetting
{
    public enum AutoForceMode
    {
        None,
        IdleMode,
        CancelIdleMode,
        UIMode,
        CancelUIMode
    }
    public enum Mode
    {
        Manual,
        AutoFPS,
        Fast,
        Perfect,
        Merge,
    }
    public enum Quality
    {

//可以用宏区分不同平台下的质量
//#if UNITY_IPHONE 
        Shadow_Off,
        //不能自动关闭的效果
        MIN,
        Fog_On,
        Grass_10,
        ParticleLow,
        PointLight_Near,
        Grass_20,
        PointLight_Middle,
        HalfShadow,
        Resolution_Low,
        Shadow_OneSample,
        Grass_40,
        SSAO_NoAmbient,
        RimLight_On, 
        Grass_Full,
        Resolution_Middle,
        Shadow_1x1,
        SpotLight_Off,
        WaterRealTimeWave,
        ParticleMiddle,
        PointLight_NoShadow,
        PointLight_StaticShadow,
        PointLight_AllShadow,
        Shadow_2x2,
        Shadow_3x3,
        Shadow_Rotate8,
        SSAO_Middle,
        Dof_On,
        LightShaft_On,
        PointLightSpecular_1,
        PointLightSpecular_2,
        PointLightSpecular_3,
        PointLightSpecular_4,
        SpotLight_ShadowOn,
        SpotLight_VolumeOn,
        Resolution_High,
        ParticleHigh,
        SSAO_High,
        Tonemapping_On,
        FXAA_On,
        WaterRealTimeReflect,
        MAX,
        //不能自动开启的效果
        XRayFull,
        SSAO_VeryHigh,
        None,
//#endif
    }


    [Range(0, (int)Quality.MAX)]
    public int Level = (int)Quality.MAX;
    [Range(0, 4)]
    public int ShadowLevel = 4;
    public int ParticleLevel = 2;
    [Range(0, 3)]
    public int AOLevel = 2;
    [Range(0, 2)]
    public int PointLightLevel = 2;
    [Range(0, 4)]
    public int PointLightSpecularLevel = 4;
    [Range(0, 2)]
    public int WaterLevel = 1;
    [Range(0, 1)]
    public int DOFLevel = 1;
    [Range(0, 2)]
    public int LightShaftLevels = 2;
    [Range(0, 1)]
    public int ToneMappingLevel = 1;
    [Range(0, 1)]
    public int FXAALevel = 0;
    [Range(0, 3)]
    public int SpotLightLevel = 3;
    [Range(0, 1)]
    public int FogLevel = 1;
    [Range(0, 1)]
    public int RimLightLevel = 1;
    [Range(0, 1)]
    public int HalfShadowResolution = 0;
    [Range(0, 2)]
    public int PointLightDisappearLevel = 2;
    [Range(0, 1)]
    public float CullLevel = 1;
    [Range(0, 1)]
    public int XRayLevel = 1;
    [Range(0,4)]
    public int GrassLevel = 4;

    public int ResolutionLevel = 2;
    public int OldResolutionLevel = 2;

    public Mode mode = Mode.AutoFPS;
    public Quality ForceQuality = Quality.None;
    public AutoForceMode m_forceMode = AutoForceMode.None;
    public int OldLevel = 0;
    public float fps = 30.0f;
    protected System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
    protected bool isSwitchScene = false;
    protected long last_time = 0;
    protected long frame_count = 0;
    protected int Origin_Screen_Width = 0;
    protected int Origin_Screen_Height = 0;
    protected int High_Width = 0;
    protected int High_Height = 0;
    protected int Middle_Width = 0;
    protected int Middle_Height = 0;
    protected int Low_Width = 0;
    protected int Low_Height = 0;
    public int CurrentWidth = 0;
    public int CurrentHeight = 0;

    public bool AutoEnterIdle = true;
    protected bool IsIdle = false;
    protected float input_busy_time = 300.0f;

    protected int LastLevel = -1;
    protected int CheckTime = 1000;
    protected int FPS5_Last = 0;
    protected int FPS5_Current = 0;
    protected int FPS5_Count = 0;
    public void Init()
    {
        if(Origin_Screen_Width == 0)
        {
            Origin_Screen_Width = Screen.width;
            Origin_Screen_Height = Screen.height;
            CurrentWidth = Origin_Screen_Width;
            CurrentHeight = Origin_Screen_Height;

            if (Application.platform == RuntimePlatform.Android)
            {
                if (    Origin_Screen_Width == 1920 && 
                        Origin_Screen_Height == 1080)
                {
                    High_Width = 1280;
                    High_Height = 720;
                    Middle_Width = 1152;
                    Middle_Height = 648;
                    Low_Width = 960;
                    Low_Height = 540;
                }
                else if (   Origin_Screen_Width == 2048 && 
                            Origin_Screen_Height == 1536)
                {
                    High_Width = 1280;
                    High_Height = 960;
                    Middle_Width = 1152;
                    Middle_Height = 864;
                    Low_Width = 1024;
                    Low_Height = 768;

                }
                else if (   Origin_Screen_Width == 2560 && 
                            Origin_Screen_Height == 1440)
                {
                    High_Width = 1280;
                    High_Height = 720;
                    Middle_Width = 1152;
                    Middle_Height = 648;
                    Low_Width = 960;
                    Low_Height = 540;
                }
                else if (   Origin_Screen_Width == 2560 && 
                            Origin_Screen_Height == 1600)
                {
                    High_Width = 1280;
                    High_Height = 800;
                    Middle_Width = 1152;
                    Middle_Height = 720;
                    Low_Width = 960;
                    Low_Height = 600;
                }
                else if (   Origin_Screen_Width == 3840 && 
                            Origin_Screen_Height == 2160)
                {
                    High_Width = 1280;
                    High_Height = 720;
                    Middle_Width = 1152;
                    Middle_Height = 648;
                    Low_Width = 960;
                    Low_Height = 540;
                }
                else if (   Origin_Screen_Width == 1600 &&
                            Origin_Screen_Height == 1200)
                {
                    High_Width = 1200;
                    High_Height = 800;
                    Middle_Width = 1024;
                    Middle_Height = 768;
                    Low_Width = 960;
                    Low_Height = 600;
                }
                else if (Origin_Screen_Width == 1280 &&
                            Origin_Screen_Height == 720)
                {
                    High_Width = 1280;
                    High_Height = 720;
                    Middle_Width = 1152;
                    Middle_Height = 648;
                    Low_Width = 960;
                    Low_Height = 540;
                }
                else if (Origin_Screen_Width == 1280 &&
                            Origin_Screen_Height == 800)
                {
                    High_Width = 1280;
                    High_Height = 800;
                    Middle_Width = 1152;
                    Middle_Height = 720;
                    Low_Width = 960;
                    Low_Height = 600;
                }
                else
                {
                    High_Width = Origin_Screen_Width;
                    High_Height = Origin_Screen_Height;
                    Middle_Width = High_Width * 2 / 3;
                    Middle_Height = High_Height * 2 / 3;
                    Low_Width = High_Width/2;
                    Low_Height = High_Height / 2;
                }

                if (High_Width != Origin_Screen_Width ||
                    High_Height != Origin_Screen_Height)
                {
                    SetResolution(High_Width, High_Height, true);
                    CurrentWidth = High_Width;
                    CurrentHeight = High_Height;
                }

            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (Origin_Screen_Width == 1920 && 
                    Origin_Screen_Height == 1080)//iphone plus
                {
                    High_Width = 1280;
                    High_Height = 720;
                    Middle_Width = 1152;
                    Middle_Height = 648;
                    Low_Width = 960;
                    Low_Height = 540;
                }
                else if (   Origin_Screen_Width == 2048 && 
                            Origin_Screen_Height == 1536)//ipad&ipad mini
                {
                    High_Width = 1280;
                    High_Height = 960;
                    Middle_Width = 1152;
                    Middle_Height = 864;
                    Low_Width = 1024;
                    Low_Height = 768;
                }
                else if (   Origin_Screen_Width == 2723 && 
                            Origin_Screen_Height == 2048)//ipad pro
                {
                    High_Width = 1361;
                    High_Height = 1024;
                    Middle_Width = 1200;
                    Middle_Height = 902;
                    Low_Width = 1021;
                    Low_Height = 768;
                }
                else if (Origin_Screen_Width == 1136 &&
                            Origin_Screen_Height == 640)//iphone 5/5S
                {
                    High_Width = 1136;
                    High_Height = 640;
                    Middle_Width = 1136;
                    Middle_Height = 640;
                    Low_Width = 960;
                    Low_Height = 540;
                }
                else if (Origin_Screen_Width == 1334 &&
                            Origin_Screen_Height == 750)//iphone 6
                {
                    High_Width = 1334;
                    High_Height = 750;
                    Middle_Width = 1136;
                    Middle_Height = 640;
                    Low_Width = 960;
                    Low_Height = 540;
                }
                else
                {
                    High_Width = Origin_Screen_Width;
                    High_Height = Origin_Screen_Height;
                    Middle_Width = Origin_Screen_Width*2/3;
                    Middle_Height = Origin_Screen_Height * 2 / 3;
                    Low_Width = High_Width /2;
                    Low_Height = High_Height/2;
                }

                if (High_Width != Origin_Screen_Width ||
                    High_Height != Origin_Screen_Height)
                {
                    SetResolution(High_Width, High_Height, true);
                    CurrentWidth = High_Width;
                    CurrentHeight = High_Height;
                }
            }
            else
            {
                if (Origin_Screen_Width == 1280 &&
                    Origin_Screen_Height == 720)
                {
                    High_Width = 1280;
                    High_Height = 720;
                    Middle_Width = 1152;
                    Middle_Height = 648;
                    Low_Width = 960;
                    Low_Height = 540;
                    CurrentWidth = High_Width;
                    CurrentHeight = High_Height;
                }
                else
                {
                    High_Width = Origin_Screen_Width;
                    High_Height = Origin_Screen_Height;
                    Middle_Width = High_Width * 2 / 3;
                    Middle_Height = High_Height * 2 / 3;
                    Low_Width = High_Width / 2;
                    Low_Height = High_Height / 2;
                    CurrentWidth = High_Width;
                    CurrentHeight = High_Height;
                }
            }

            
        }
        watch.Reset();
        watch.Start();
        last_time = 0;
        frame_count = Time.renderedFrameCount;

       /* if(Application.isPlaying)
        {
            string Video_level = sdConfDataMgr.Instance().GetSetting("CFG_VideoLevel");
            float temp_val = 1.0f;
            if(float.TryParse(Video_level, out temp_val))
            {
                int temp = (int)(temp_val * ((int)Quality.MAX - (int)Quality.MIN) )+ (int)Quality.MIN;
                Level = temp;
            }
            else
            {
                Level = (int)Quality.MAX;
            }

            string Res_Level = sdConfDataMgr.Instance().GetSetting("CFG_Resolution");
            if(Res_Level == "0")
            {
                ResolutionLevel = 0;
            }
            else if (Res_Level == "1")
            {
                ResolutionLevel = 1;
            }
            else if (Res_Level == "2")
            {
                ResolutionLevel = 2;
            }
            else
            {
                ResolutionLevel = 2;
            }
            string auto_fps = sdConfDataMgr.Instance().GetSetting("CFG_FPS");
            if(auto_fps == "1")
            {
                mode = Mode.AutoFPS;
            }
            else if (auto_fps == "0")
            {
                mode = Mode.Manual;
            }
            else if (auto_fps == "2")
            {
                mode = Mode.Fast;
            }
            else if(auto_fps == "3")
            {
                mode = Mode.Perfect;
            }
            if (UnityEngine.SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2)
            {
                mode = GlobalQualitySetting.Mode.Fast;
               
                Debug.Log("OpenGLES2 GlobalQualitySetting.Mode.Fast");
            }
            if(mode == Mode.Fast)
            {
                Level = (int)Quality.MIN;
            }
            if(mode == Mode.Perfect)
            {
                Level = (int)Quality.MAX;
            }
        }*/
    }

    public void SetQualityForceMode(GlobalQualitySetting.AutoForceMode mode)
    {
        if (m_forceMode == mode) return;

        if(m_forceMode == AutoForceMode.UIMode)
        {
            if (mode == AutoForceMode.IdleMode || mode == AutoForceMode.CancelIdleMode)
            {
                return;
            }
        }

        if (m_forceMode == AutoForceMode.IdleMode || m_forceMode == AutoForceMode.UIMode)
        {
            if (mode == AutoForceMode.IdleMode || mode == AutoForceMode.UIMode)
            {
                return;
            }
        }
        if (m_forceMode == AutoForceMode.CancelIdleMode || m_forceMode == AutoForceMode.CancelUIMode)
        {
            if (mode == AutoForceMode.CancelIdleMode || mode == AutoForceMode.CancelUIMode)
            {
                return;
            }
        }
        m_forceMode = mode;
        if (mode == AutoForceMode.IdleMode || mode == AutoForceMode.UIMode)
        {
            SetForceQuality(Quality.MIN);
        }
        else
        {
            SetForceQuality(Quality.None);
        }
    }
    public void SetForceQuality(Quality quality)
    {
        ForceQuality = quality;
        if(ForceQuality != Quality.None)
        {
            OldResolutionLevel = ResolutionLevel;
            OldLevel = Level;
            Level = (int)ForceQuality;
            CalcLevel();
        }
        else
        {
            if (OldLevel > 0)
            {
                ResolutionLevel = OldResolutionLevel;
                Level = OldLevel;
                CalcLevel();
            }
        }
    }
    public void Update()
    {
        //if(Application.isEditor)
        //{
        //    Origin_Screen_Width = Screen.width;
        //    Origin_Screen_Height = Screen.height;
        //
        //
        //    High_Width = Origin_Screen_Width;
        //    High_Height = Origin_Screen_Height;
        //    Middle_Width = High_Width * 2 / 3;
        //    Middle_Height = High_Height * 2 / 3;
        //    Low_Width = High_Width / 2;
        //    Low_Height = High_Height / 2;
        //
        //    CurrentWidth = High_Width;
        //    CurrentHeight = High_Height;
        //}
        //if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (!isSwitchScene && AutoEnterIdle)
            {
                if (Input.touchCount > 0|| Input.anyKey)
                {
                    input_busy_time = 300.0f;
                    if(IsIdle)
                    {
                        Debug.Log("Check User Input, Enter Busy");
                        Application.targetFrameRate = 30;
                    }
                    IsIdle = false;
                }
                else
                {
                    
                    if(input_busy_time < 0.0f)
                    {
                        //玩家超过15秒 没有输入 开始进入低功耗模式
                        if(!IsIdle)
                        {
                            Debug.Log("No Input, Enter Idle");
                            Application.targetFrameRate = 10;
                        }
                        IsIdle = true;
                       
                    }
                    else
                    {
                        input_busy_time -= Time.deltaTime;
                    }
                }
            }
        }
        if (Application.isPlaying)
        {
            long current = watch.ElapsedMilliseconds;
            long offset = current - last_time;
            if (offset > CheckTime)
            {
                last_time = current;

                long current_count = Time.renderedFrameCount;
                double t = (double)(offset) * 0.001;
                fps = (float)((double)(current_count - frame_count) / t);
                FPS5_Current += (int)fps;
                FPS5_Count++;
                if(FPS5_Count>=5)
                {
                    FPS5_Last = FPS5_Current;
                    FPS5_Current = 0;
                    FPS5_Count = 0;
                }

                frame_count = current_count;
                if (!IsIdle && ForceQuality == Quality.None && mode == Mode.AutoFPS && !isSwitchScene)
                {
                    if (fps < 24.0f)
                    {
                        Level--;
                        if (Level < (int)Quality.MIN)
                        {
                            Level = (int)Quality.MIN;
                        }
                    }
                    else if (FPS5_Last>140 &&fps>29.0f)
                    {
                        Level++;
                        //123123
                        if (Level > (int)Quality.MAX)
                        {
                            Level = (int)Quality.MAX;
                        }
                    }
                }
            }
        }
        else
        {
            Level = (int)Quality.MAX;
        }
        CalcLevel();
        if (IsIdle)
        {
            SetQualityForceMode(AutoForceMode.IdleMode);

        }
        else
        {
            SetQualityForceMode(AutoForceMode.CancelIdleMode);
        }

        if (fps < 15&& isSwitchScene==false&&IsIdle==false)
        {
            lowFPSTimeTime -= Time.unscaledDeltaTime;
            if (lowFPSTimeTime < 0)
            {
                if (Mode.Perfect == mode)
                {
                    //sdUICharacter.Instance.ShowMsgLine("当前画面卡顿，请在设置里边选择均衡或流畅模式！");
                }
                else
                {
                   // sdUICharacter.Instance.ShowMsgLine("当前画面卡顿，请在设置里边选择流畅模式！");
                }
                lowFPSTimeTime = 15f;
            }
        }
        else
        {
            lowFPSTimeTime = 15f;
        }
    }
    float lowFPSTimeTime = 15f;
   public void OnLevelBegin(object userdata)
   {
       isSwitchScene = false;
        watch.Reset();
        watch.Start();
        last_time = 0;
        frame_count = Time.renderedFrameCount;
    }
   public void OnLevelEnd(object userdata)
   {
       isSwitchScene = true;

   }
    public void ChangeResolutionLevel(int level)
    {
        //if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            ResolutionLevel = level;
            if (ResolutionLevel < 0)
            {
                ResolutionLevel = 0;
            }
            else if (ResolutionLevel > 2)
            {
                ResolutionLevel = 2;
            }
            int w = 0;
            int h = 0;
            if (ResolutionLevel == 2)
            {
                w = High_Width;
                h = High_Height;
            }
            else if (ResolutionLevel == 1)
            {
                w = Middle_Width;
                h = Middle_Height;
            }
            else
            {
                w = Low_Width;
                h = Low_Height;
            }
            if (w != CurrentWidth || h != CurrentHeight)
            {
                //SetResolution(w, h, true);
                CurrentWidth = w;
                CurrentHeight = h;
            }
        }
    }

    void CalcResolution()
    {
        if (mode == Mode.AutoFPS && ForceQuality == Quality.None)
        {
            //分辨率只进行 自动降低 不自动升高
            if (Level >= (int)Quality.Resolution_High)
            {
                if (ResolutionLevel >= 2)
                {
                    ResolutionLevel = 2;
                }
                //只有等级达到MAX之后 才能提高分辨率。。。
                else if ( ResolutionLevel < 2 && Level>= (int)Quality.MAX)
                {
                    ResolutionLevel = 2;
                }

            }
            else if (Level >= (int)Quality.Resolution_Middle)
            {
                if (ResolutionLevel >= 1)
                {
                    ResolutionLevel = 1;
                }
            }
            else
            {
                if (ResolutionLevel >= 0)
                {
                    ResolutionLevel = 0;
                }
            }

            
        }
        else
        {

            //未开启自动画质时 不对分辨率做调整
            //if (Level >= (int)Quality.Resolution_High)
            //{
            //        ResolutionLevel = 2;
            //}
            //else if (Level >= (int)Quality.Resolution_Middle)
            //{
            //        ResolutionLevel = 1;
            //}
            //else
            //{
            //        ResolutionLevel = 0;
            //}
        }

        int w = 0;
        int h = 0;
        if (ResolutionLevel == 2)
        {
            w = High_Width;
            h = High_Height;
        }
        else if (ResolutionLevel == 1)
        {
            w = Middle_Width;
            h = Middle_Height;
        }
        else
        {
            w = Low_Width;
            h = Low_Height;
        }


        if (w != CurrentWidth || h != CurrentHeight)
        {
            //SetResolution(w, h, true);
            CurrentWidth = w;
            CurrentHeight = h;
        }
    }
    void SetResolution(int w,int h,bool fullscreen)
    {
        Screen.SetResolution(w, h, fullscreen);
    }
    protected void CalcLevel()
    {
        CalcResolution();
        if (LastLevel!=Level)
        {
            LastLevel = Level;
        }
        else
        {
            return;
        }
        if (Level >= (int)Quality.Shadow_Rotate8)
        {
            ShadowLevel = 4;
        }
        else if (Level >= (int)Quality.Shadow_3x3)
        {
            ShadowLevel = 3;
        }
        else if (Level >= (int)Quality.Shadow_2x2)
        {
            ShadowLevel = 2;
        }
        else if (Level >= (int)Quality.Shadow_OneSample)
        {
            ShadowLevel = 2;
        }
        else if(Level >= (int)Quality.MIN)
        {
            ShadowLevel = 1;
        }
        else
        {
            ShadowLevel = 0;
        }

        if (Level >= (int)Quality.SSAO_VeryHigh)
        {
            AOLevel = 3;
        }
        else if (Level >= (int)Quality.SSAO_High)
        {
            AOLevel = 2;
        }
        else if (Level >= (int)Quality.SSAO_Middle)
        {
            AOLevel = 1;
        }
        else if (Level >= (int)Quality.SSAO_NoAmbient)
        {
            AOLevel = 0;
        }
        else
        {
            AOLevel = 4;
        }

        if (Level >= (int)Quality.PointLight_AllShadow)
        {
            PointLightLevel = 2;
        }
        else if (Level >= (int)Quality.PointLight_StaticShadow)
        {
            PointLightLevel = 1;
        }
        else
        {
            PointLightLevel = 0;
        }


        if (Level >= (int)Quality.PointLightSpecular_4)
        {
            PointLightSpecularLevel = 4;
        }
        else if (Level >= (int)Quality.PointLightSpecular_3)
        {
            PointLightSpecularLevel = 3;
        }
        else if (Level >= (int)Quality.PointLightSpecular_2)
        {
            PointLightSpecularLevel = 2;
        }
        else if (Level >= (int)Quality.PointLightSpecular_1)
        {
            PointLightSpecularLevel = 1;
        }
        else
        {
            PointLightSpecularLevel = 0;
        }

        if(Level >= (int)Quality.WaterRealTimeReflect)
        {
            WaterLevel = 2;
        }
        else if (Level >= (int)Quality.WaterRealTimeWave)
        {
            WaterLevel = 1;
        }
        else
        {
            WaterLevel = 0;
        }

        if (Level >= (int)Quality.Dof_On)
        {
            DOFLevel = 1;
        }
        else
        {
            DOFLevel = 0;
        }
        if (Level >= (int)Quality.LightShaft_On)
        {
            LightShaftLevels = 2;
        }
        else
        {
            LightShaftLevels = 0;
        }
        if (Level >= (int)Quality.Tonemapping_On)
        {
            ToneMappingLevel = 1;
        }
        else
        {
            ToneMappingLevel = 0;
        }
        if (Level >= (int)Quality.FXAA_On)
        {
            FXAALevel = 1;
        }
        else
        {
            FXAALevel = 0;
        }
        if (Level >= (int)Quality.SpotLight_VolumeOn)
        {
            SpotLightLevel = 3;
        }
        else if (Level >= (int)Quality.SpotLight_ShadowOn)
        {
            SpotLightLevel = 2;
        }
        else if(Level >= (int)Quality.SpotLight_Off)
        {
            SpotLightLevel = 1;
        }
        else
        {
            SpotLightLevel = 0;
        }

        if (Level >= (int)Quality.Fog_On)
        {
            FogLevel = 1;
        }
        else
        {
            FogLevel = 0;
        }

        if (Level >= (int)Quality.RimLight_On)
        {
            RimLightLevel = 1;
        }
        else
        {
            RimLightLevel = 0;
        }

        if(Level < (int)Quality.HalfShadow)
        {
            HalfShadowResolution = 1;
        }
        else if(Level >= (int)Quality.Shadow_OneSample)
        {
            HalfShadowResolution = 0;
        }

        if (Level >= (int)Quality.PointLight_Middle)
        {
            PointLightDisappearLevel = 2;
        }
        else if (Level >= (int)Quality.PointLight_Near)
        {
            PointLightDisappearLevel = 1;
        }
        else
        {
            PointLightDisappearLevel = 0;
        }

        int LevelRange = (int)Quality.MAX - (int)Quality.MIN;

        CullLevel = (float)(Level - (int)Quality.MIN) / (float)LevelRange;

        if(Level >= (int)Quality.XRayFull)
        {
            XRayLevel = 1;
        }
        else
        {
            XRayLevel = 0;
        }

        if(Level >= (int)Quality.ParticleHigh)
        {
            ParticleLevel = 2;
        }
        else if (Level >= (int)Quality.ParticleMiddle)
        {
            ParticleLevel = 1;
        }
        else
        {
            ParticleLevel = 0;
        }

        if(Level >= (int)Quality.Grass_Full)
        {
            GrassLevel = 4;
        }
        else if (Level >= (int)Quality.Grass_40)
        {
            GrassLevel = 3;
        }
        else if (Level >= (int)Quality.Grass_20)
        {
            GrassLevel = 2;
        }
        else if (Level >= (int)Quality.Grass_10)
        {
            GrassLevel = 1;
        }
        else
        {
            GrassLevel = 0;
        }
    }
    public bool Command(string key,int val)
    {
        string low_string = key.ToLower();
        if (low_string == "mode")
        {
            mode = (Mode)val;
        }
        else if (low_string == "quality")
        {
            Level = val;
        }
        else if (low_string == "res")
        {
            ResolutionLevel = val;
        }
        else if (low_string == "fog")
        {
            FogLevel = val;
        }
        else if (low_string == "spotlight")
        {
            SpotLightLevel = val;
        }
        else if (low_string == "fxaa")
        {
            FXAALevel = val;
        }
        else if (low_string == "tonemapping")
        {
            ToneMappingLevel = val;
        }
        else if (low_string == "lightshaft")
        {
            LightShaftLevels = val;
        }
        else if (low_string == "dof")
        {
            DOFLevel = val;
        }
        else if (low_string == "water")
        {
            WaterLevel = val;
        }
        else if (low_string == "pointlightspec")
        {
            PointLightSpecularLevel = val;
        }
        else if (low_string == "pointlight")
        {
            PointLightLevel = val;
        }
        else if (low_string == "ao")
        {
            AOLevel = val;
        }
        else if (low_string == "shadow")
        {
            ShadowLevel = val;
        }
        else if (low_string == "autoenteridle")
        {
            AutoEnterIdle = val==1;
        }
        else if (low_string == "checktime")
        {
            CheckTime = val;
            if(CheckTime<1000)
            {
                CheckTime = 1000;
            }
        }
        else
        {
            return false;
        }
        return true;
    }
}
