using UnityEngine;
using System.Collections;

public class HalfPixelOffset : MonoBehaviour {
    //public bool offset = false;
    public Camera contrrolCamera;
    public float fScale = 1.0f;
    public RenderPipeline pipeline;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        //if (SceneRenderSetting._Setting.EnableAA)
        //{
        //    int width  = GlobalQualitySetting.Instance.CurrentWidth;
        //    int height = GlobalQualitySetting.Instance.CurrentHeight;
        //
        //    if (Time.frameCount % 2 == 1)
        //    {
        //        Vector3 up = contrrolCamera.transform.TransformDirection(Vector3.up);
        //        Vector3 right = contrrolCamera.transform.TransformDirection(Vector3.right);
        //        //Shader.SetGlobalVector("rs_offset", Vector4.zero);
        //        //camera.nearClipPlane
        //        transform.localPosition = (right * (float)width + up * (float)height) * 0.5f * fScale / (float)Screen.width;
        //    }
        //    else
        //    {
        //        Vector4 v = new Vector4(fScale / (float)width, fScale / (float)height, 0, 0);
        //        //Shader.SetGlobalVector("rs_offset", v);
        //        transform.localPosition = Vector3.zero;
        //    }
        //}
	}
}
