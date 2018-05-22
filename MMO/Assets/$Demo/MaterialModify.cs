using UnityEngine;
using System.Collections;

public class MaterialModify : MonoBehaviour {

    public Material mat = null;
    public string propertyName = "";
    UISlider slider = null;
	// Use this for initialization
	void Start () {
        slider = transform.GetComponent<UISlider>();
        EventDelegate.Add(slider.onChange, OnDragChange);
        slider.value= mat.GetFloat(propertyName);
    }
     void OnEnable()
    {
        if(slider)
            slider.value = mat.GetFloat(propertyName);
    }
    void OnDragChange()
    {
        float t = slider.value;
        mat.SetFloat(propertyName, t);
    }
	// Update is called once per frame
	void Update () {
	
	}
}
