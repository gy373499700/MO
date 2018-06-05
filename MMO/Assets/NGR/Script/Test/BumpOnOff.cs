using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BumpOnOff : MonoBehaviour {
    public Renderer[] renderObjs = null;
    List<Material> lstMaterial = new List<Material>();
    public bool bump = true;
	// Use this for initialization
	void Start () {
        for (int i = 0; i < renderObjs.Length; i++)
        {
            Material[] mats = renderObjs[i].materials;
            for (int j = 0; j < mats.Length; j++)
            {
                lstMaterial.Add(mats[j]);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnGUI()
    {
        string txt = "Bump(On)";
        if(!bump)
        {
            txt = "Bump(Off)";
        }
        if(GUI.Button(new Rect(200, 0, 100, 50),txt))
        {
            bump    =  !bump;
            float fbump = 1.0f;
            if (!bump)
            {
                fbump = 0.0f;
            }
            for (int i = 0; i < lstMaterial.Count; i++)
            {
                lstMaterial[i].SetFloat("_BumpScale", fbump);
            }
        }
    }
}
