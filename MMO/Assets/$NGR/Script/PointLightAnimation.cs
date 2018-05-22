using UnityEngine;

[ExecuteInEditMode]
public class PointLightAnimation : MonoBehaviour
{
    public enum ValueType
    {
        Color,
        Intensity,
        Radius
    }
    public enum AnimType
    {
        Clamp,
        Loop,
        PingPong
    }

    public float[] keys;
    public Vector4[] values;
    public ValueType vType = ValueType.Intensity;
    public AnimType cType = AnimType.Loop;
    public float delayTime = 0.0f;
    public float TotalTime = 1.0f;
    private float currentTime = 0.0f;
    private Vector4 originValue = Vector4.zero;

    private DeferredLight dlight = null;
    public void OnEnable()
    {
        currentTime = -delayTime;

        dlight = gameObject.GetComponent<DeferredLight>();
        if (vType == ValueType.Color)
        {
            originValue = dlight.color;
        }
        else if (vType == ValueType.Intensity)
        {
            originValue.x = dlight.intensity;
        }
        else if (vType == ValueType.Radius)
        {
            originValue.x = dlight.Radius;
        }
        
        Sample(dlight, 0);

    }
    public void OnDisable()
    {
        //dlight.color = originColor;
        //dlight.intensity= originIntensity;
        //dlight.Radius= originRadius;
        if (vType == ValueType.Color)
        {
            dlight.color = originValue;// = dlight.color;
        }
        else if (vType == ValueType.Intensity)
        {
            dlight.intensity = originValue.x;// = dlight.intensity;
        }
        else if (vType == ValueType.Radius)
        {
            dlight.Radius = originValue.x;// = dlight.Radius;
        }
    }
    public void Update()
    {
        if (Application.isPlaying)
        {
            currentTime += Time.deltaTime;
        }
        else
        {
            currentTime += 0.1f;
        }
        if(cType == AnimType.Clamp)
        {
            if(currentTime >= TotalTime)
            {
                currentTime = TotalTime;
            }
        }
        else if (cType == AnimType.Loop)
        {
            if (currentTime >= TotalTime)
            {
                currentTime = 0.0f;
            }
        }
        
        float f = currentTime / TotalTime;
        if(f<0.0f)
        {
            f = 0.0f;
        }
        if (cType == AnimType.PingPong)
        {
            if (currentTime >= 2*TotalTime)
            {
                currentTime = 0.0f;
            }
            if (currentTime > TotalTime)
            {
                f = (currentTime - TotalTime) / TotalTime;
                f = 1 - f;
            }
            if(f < 0.0f)
            {
                f = 0.0f;
            }
        }
        Sample(dlight, f);
    }
    void Sample(DeferredLight l,float f)
    {
        if (keys == null)
            return;
        if (keys.Length == 0)
            return;

        if (f <= keys[0])
        {
            if (vType == ValueType.Color)
            {
                l.color = values[0];// = dlight.color;
            }
            else if (vType == ValueType.Intensity)
            {
                l.intensity = values[0].x;// = dlight.intensity;
            }
            else if (vType == ValueType.Radius)
            {
                l.Radius = values[0].x;// = dlight.Radius;
            }
            return;
        }
        for (int i = 1; i <keys.Length;i++)
        {
            float last = keys[i - 1];
            float next = keys[i];
            if (f> last && f <= next)
            {
                float lerp_val = (f - last) / (next - last);
                if (vType == ValueType.Color)
                {
                    l.color = Color.Lerp(values[i - 1], values[i], lerp_val);
                }
                else if (vType == ValueType.Intensity)
                {
                    l.intensity = values[i - 1].x * (1 - lerp_val) + values[i].x * lerp_val;
                }
                else if (vType == ValueType.Radius)
                {
                    l.Radius = values[i - 1].x * (1 - lerp_val) + values[i].x * lerp_val;
                }
                return;
            }
        }
        
        
    }
}