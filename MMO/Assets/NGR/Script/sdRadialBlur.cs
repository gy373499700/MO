using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class sdRadialBlur : MonoBehaviour {
    public static List<sdRadialBlur> lstRadialBlur = new List<sdRadialBlur>();
    public float Intensity = 0.0f;
    public float Radius = 0.5f;
    // Use this for initialization
    void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnEnable()
    {
        lstRadialBlur.Add(this);
    }
    void OnDisable()
    {
        lstRadialBlur.Remove(this);
    }
    public static sdRadialBlur Find(Camera mainCamera)
    {
        
        sdRadialBlur obj = null;
        float maxVal = 0.0f;
        for (int i=0;i< lstRadialBlur.Count;i++)
        {
            sdRadialBlur temp = lstRadialBlur[i];
            if(temp.Intensity < 0.001f)
            {
                continue;
            }
            if (temp.Radius < 0.001f)
            {
                continue;
            }
            Vector3 vp = mainCamera.WorldToViewportPoint(temp.transform.position);
            if (vp.x > 1.0f || vp.x < 0.0f || vp.y > 1.0f || vp.x < 0.0f)
            {
                continue;
            }
            float val = temp.Radius;// Vector3.Distance(temp.transform.position, mainCamera.transdorm.position);
            if(val > maxVal)
            {
                obj = temp;
                maxVal = val;
            }
        }
        return obj;
    }
}
