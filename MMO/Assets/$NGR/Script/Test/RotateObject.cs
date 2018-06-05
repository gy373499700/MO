using UnityEngine;
using System.Collections;

public class RotateObject : MonoBehaviour {
    
    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnGUI()
    {
        if (GUI.RepeatButton(new Rect(500, 0, 200, 200), "<-"))
        {
            Quaternion q = transform.rotation;
            q *= Quaternion.AngleAxis(30 * Time.deltaTime, Vector3.up);
            transform.rotation = q;
        }
        if (GUI.RepeatButton(new Rect(700, 0, 200, 200), "->"))
        {
            Quaternion q = transform.rotation;
            q *= Quaternion.AngleAxis(-30 * Time.deltaTime, Vector3.up);
            transform.rotation = q;
        }
    }
}
