using UnityEngine;
using System.Collections;

public class RotateSelf : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
    public float speed = 30;
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.up, Time.deltaTime* speed);
	}
}
