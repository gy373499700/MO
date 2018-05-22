using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SceneCameraSync : MonoBehaviour {

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnRenderObject()
    {
        if (Camera.current!=null)
        {
            if (Camera.current.name.Equals("SceneCamera"))
            {
                //Debug.Log(Camera.current.name);
                SceneRenderSetting._Setting.SceneCamera_Pos = Camera.current.transform.position;
                SceneRenderSetting._Setting.SceneCamera_Rot = Camera.current.transform.rotation;
                SceneRenderSetting._Setting.transform.position = Camera.current.transform.position;
                SceneRenderSetting._Setting.transform.rotation = Camera.current.transform.rotation;
            }
        }
    }
}
