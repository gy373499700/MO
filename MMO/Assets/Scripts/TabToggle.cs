using UnityEngine;
using System.Collections;

public class TabToggle : MonoBehaviour {

    // Use this for initialization
    public GameObject toggle1 = null;
    public GameObject toggle2 = null;
    public GameObject toggle3 = null;
    IEnumerator Start()
    {
        yield return null;

        toggle1.SetActive(true);
        toggle2.SetActive(false);
        toggle3.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
