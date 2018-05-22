using UnityEngine;

//[ExecuteInEditMode]
public class sdLightAnimation : MonoBehaviour
{
    [System.Serializable]
    public struct Key
    {
        public float time;
        public Color MainLightColor;
        public float MainLightColorScale;
        public Color AmbientColor;
        public float AmbientColorScale;

    }
    [System.Serializable]
    public struct DirKey
    {
        public float time;
        public Vector3 MainLightDirection;
    }

    [System.Serializable]
    public struct AudioKey
    {
        public float time;
    }

    public Key[] keys;
    public DirKey[] dirKeys;
    public AudioKey[] audioKey;

    AudioSource lightingMusic = null;
    public sdAreaRenderSetting areaRender = null;

    public enum AnimType
    {
        Clamp,
        Loop,
    }

    public AnimType cType = AnimType.Loop;
    public float delayTime = 0.0f;
    public float TotalTime = 1.0f;
    public float currentTime = 0.0f;
    //private Color originColor = Color.white;
    //private float originIntensity = 1;
    //private float originRadius = 1;
    //private DeferredLight dlight = null;
    Key originValue = new Key();
    Vector3 originDirection;
    public void Start()
    {
        lightingMusic = GetComponent<AudioSource>();
        if (SceneRenderSetting._Setting == null) return;

        if (areaRender != null)
        {
            originValue.MainLightColor = areaRender.MainLightColor;
            originValue.MainLightColorScale = areaRender.MainLightScale;
            originDirection = areaRender.MainLightDirection;
            originValue.AmbientColor = areaRender.AmbientColor;
            originValue.AmbientColorScale = areaRender.AmbientScale;
        }
        else
        {
            originValue.MainLightColor = SceneRenderSetting._Setting.MainLightColor;
            originValue.MainLightColorScale = SceneRenderSetting._Setting.MainLightColorScale;
            originDirection = SceneRenderSetting._Setting.MainLightDirection;
            originValue.AmbientColor = SceneRenderSetting._Setting.AmbientColor;
            originValue.AmbientColorScale = SceneRenderSetting._Setting.AmbientColorScale;
        }
        
    }

    public void OnEnable()
    {
        //currentTime = -delayTime;

        //dlight = gameObject.GetComponent<DeferredLight>();

        //originColor = dlight.color;
        //originIntensity = dlight.intensity;
        //originRadius = dlight.Radius;

        //Sample(0);

    }
    public void OnDisable()
    {
//         dlight.color = originColor;
//         dlight.intensity = originIntensity;
//         dlight.Radius = originRadius;
    }
    public void Update()
    {
        currentTime += Time.deltaTime;
        float temp = currentTime - delayTime;
        if (cType == AnimType.Clamp)
        {
            if (temp >= TotalTime)
            {
                currentTime = 0.0f;
                changeNum = 0;
                audioNum = 0;
                temp = 0;
                enabled = false;
            }
        }
        else if (cType == AnimType.Loop)
        {
            if (temp >= TotalTime)
            {
                currentTime = 0.0f;
                changeNum = 0;
                audioNum = 0;
                temp = 0;
                if (areaRender == null)
                {
                    SceneRenderSetting._Setting.MainLightDirection = originDirection;
                }
                else
                {
                    areaRender.MainLightDirection = originDirection;
                }
            }
        }

//         float f = currentTime / TotalTime;
//         if (f < 0.0f)
//         {
//             f = 0.0f;
//         }
//         if (cType == AnimType.PingPong)
//         {
//             if (currentTime >= 2 * TotalTime)
//             {
//                 currentTime = 0.0f;
//             }
//             if (currentTime > TotalTime)
//             {
//                 f = (currentTime - TotalTime) / TotalTime;
//                 f = 1 - f;
//             }
//             if (f < 0.0f)
//             {
//                 f = 0.0f;
//             }
//         }
        Sample(temp);
    }

    int changeNum = 0;
    int audioNum = 0;

    void Sample(float f)
    {
        if (keys == null)
            return;
        if (keys.Length == 0)
            return;

        if (keys.Length == 1)
        {
            if (f > keys[0].time)
            {
                if (areaRender == null)
                {
                    SceneRenderSetting._Setting.MainLightColor = originValue.MainLightColor;
                    SceneRenderSetting._Setting.MainLightColorScale = originValue.MainLightColorScale;
                    SceneRenderSetting._Setting.AmbientColor = originValue.AmbientColor;
                    SceneRenderSetting._Setting.AmbientColorScale = originValue.AmbientColorScale;
                }
                else
                {
                    areaRender.MainLightColor = originValue.MainLightColor;
                    areaRender.MainLightScale = originValue.MainLightColorScale;
                    areaRender.AmbientColor = originValue.AmbientColor;
                    areaRender.AmbientScale = originValue.AmbientColorScale;
                }
            }
            else
            {
                float lerp_val = f / keys[0].time;

                if (areaRender == null)
                {
                    SceneRenderSetting._Setting.MainLightColor = Color.Lerp(originValue.MainLightColor, keys[0].MainLightColor, lerp_val);
                    SceneRenderSetting._Setting.MainLightColorScale = originValue.MainLightColorScale * (1 - lerp_val) + keys[0].MainLightColorScale * lerp_val;
                    SceneRenderSetting._Setting.AmbientColor = Color.Lerp(originValue.AmbientColor, keys[0].AmbientColor, lerp_val);
                    SceneRenderSetting._Setting.AmbientColorScale = originValue.AmbientColorScale * (1 - lerp_val) + keys[0].AmbientColorScale * lerp_val;

                }
                else
                {
                    areaRender.MainLightColor = Color.Lerp(originValue.MainLightColor, keys[0].MainLightColor, lerp_val);
                    areaRender.MainLightScale = originValue.MainLightColorScale * (1 - lerp_val) + keys[0].MainLightColorScale * lerp_val;
                    areaRender.AmbientColor = Color.Lerp(originValue.AmbientColor, keys[0].AmbientColor, lerp_val);
                    areaRender.AmbientScale = originValue.AmbientColorScale * (1 - lerp_val) + keys[0].AmbientColorScale * lerp_val;

                }
            } 
        }
        else
        {
            for (int i = 0; i < keys.Length; i++)
            {
                if (i == keys.Length - 1)
                {
                    float last = keys[i].time;
                    if (f >= last)
                    {
                        if (areaRender == null)
                        {
                            SceneRenderSetting._Setting.MainLightColor = keys[i].MainLightColor;
                            SceneRenderSetting._Setting.MainLightColorScale = keys[i].MainLightColorScale;
                            SceneRenderSetting._Setting.AmbientColor = keys[i].AmbientColor;
                            SceneRenderSetting._Setting.AmbientColorScale = keys[i].AmbientColorScale;
                        }
                        else
                        {
                            areaRender.MainLightColor = keys[i].MainLightColor;
                            areaRender.MainLightScale = keys[i].MainLightColorScale;
                            areaRender.AmbientColor = keys[i].AmbientColor;
                            areaRender.AmbientScale = keys[i].AmbientColorScale;
                        }
                            break;
                    }
                }
                else
                {
                    float last = keys[i].time;
                    float next = keys[i + 1].time;

                    if (f > last && f <= next)
                    {
                        float lerp_val = (f - last) / (next - last);

                        if (areaRender == null)
                        {
                            SceneRenderSetting._Setting.MainLightColor = Color.Lerp(keys[i].MainLightColor, keys[i + 1].MainLightColor, lerp_val);
                            SceneRenderSetting._Setting.MainLightColorScale = keys[i].MainLightColorScale * (1 - lerp_val) + keys[i + 1].MainLightColorScale * lerp_val;

                            SceneRenderSetting._Setting.AmbientColor = Color.Lerp(keys[i].AmbientColor, keys[i + 1].AmbientColor, lerp_val);
                            SceneRenderSetting._Setting.AmbientColorScale = keys[i].AmbientColorScale * (1 - lerp_val) + keys[i + 1].AmbientColorScale * lerp_val;

                        }
                        else
                        {
                            areaRender.MainLightColor = Color.Lerp(keys[i].MainLightColor, keys[i + 1].MainLightColor, lerp_val);
                            areaRender.MainLightScale = keys[i].MainLightColorScale * (1 - lerp_val) + keys[i + 1].MainLightColorScale * lerp_val;

                            areaRender.AmbientColor = Color.Lerp(keys[i].AmbientColor, keys[i + 1].AmbientColor, lerp_val);
                            areaRender.AmbientScale = keys[i].AmbientColorScale * (1 - lerp_val) + keys[i + 1].AmbientColorScale * lerp_val;

                        }
                        break;
                    }
                }
            }
        }

        if (dirKeys == null || dirKeys.Length == 0) return;

        
        for (int i = 0; i < dirKeys.Length; i++)
        {
            float temp = dirKeys[i].time;

            if (f >= temp && changeNum == i)
            {
                if (areaRender==null)
                {
                    SceneRenderSetting._Setting.MainLightDirection = dirKeys[i].MainLightDirection;
                }
                else
                {
                    areaRender.MainLightDirection = dirKeys[i].MainLightDirection;
                }
                changeNum++;
                break;
            }
        }

        if (audioKey == null || audioKey.Length == 0) return;

        for (int i = 0; i < audioKey.Length; i++)
        {
            float temp = audioKey[i].time;

            if (f >= temp && audioNum == i)
            {
                lightingMusic.Play();
                audioNum++;
                break;
            }
        }
    }
}


