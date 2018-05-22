using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class NoRotate : MonoBehaviour {
    public Transform bindNode;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate () {
        if (bindNode != null)
        {
            transform.position = bindNode.transform.position;
        }
	}
}
