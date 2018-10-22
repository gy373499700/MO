using UnityEngine;
using System.Collections;

public class Player :MonoBehaviour {

    public Transform camera = null;
    public GameObject blur = null;
    public static bool EnableMoveBlur=false;
	// Use this for initialization
	IEnumerator Start () {
        yield return null;
        yield return null;
        yield return null;
      //  camera.gameObject.SetActive(false);
        camera.localPosition = new Vector3(0, 0.5f, -3);
        camera.localRotation = Quaternion.identity;
       // camera.gameObject.SetActive(true);
        yield return null;
        while (true)
        {
            if (RenderPipeline._instance != null)
            {
                RenderPipeline._instance.quality.mode = GlobalQualitySetting.Mode.Merge;
                break;
            }
            yield return null;
        }

    }
    public float maxZ = -2;
    public float minZ = -5;
    public float Z = 0f;
    public float sensitive = 1f;
	// Update is called once per frame
	void Update () {

        if (EnableMoveBlur)
        {
            if (Input.GetKey(KeyCode.W))
            {
                if (blur && blur.activeSelf == false)
                    blur.SetActive(true);
            }
            else
            {
                if (blur && blur.activeSelf)
                    blur.SetActive(false);
            }
        }else
        {
            if (blur && blur.activeSelf)
                blur.SetActive(false);
        }


        var scaleFactor = Input.GetAxis("Mouse ScrollWheel");
        if (scaleFactor != 0)
        {
            Z += scaleFactor * sensitive;
            Z = Mathf.Clamp(Z, minZ, maxZ);
            SceneRenderSetting._Setting.transform.localPosition = new Vector3(0, 0.5f, Z);
        }
    }
}
