using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScreenSpaceReflect : MonoBehaviour {
    public static List<ScreenSpaceReflect> RefletList = new List<ScreenSpaceReflect>();
    public Renderer render_obj;
	// Use this for initialization
	void Start () {
        //renderer.Render(0);
	}
	
    void OnEnable()
    {
        RefletList.Add(this);
    }
    void OnDisable()
    {
        RefletList.Remove(this);
    }
    public void Draw(Material replace_mat)
    {
#if UNITY_5

#else
        //Material mat = render_obj.materials[0];
        //render_obj.materials[0] = replace_mat;
        render_obj.Render(0);
        //render_obj.materials[0] = mat;
#endif
    }
}
