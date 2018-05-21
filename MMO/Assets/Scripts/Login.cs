using UnityEngine;
using System.Collections;

public class Login : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameObject obj = GameObject.Find("button");
        if (obj == null)
        {
            obj = new GameObject();
            obj.name = "button";
          //  obj.AddComponent<ButtonClick>();
        }
        gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnClick);

    }
	
	// Update is called once per frame
	void OnClick () {
        //if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("load level");
            ResourceMgr.Instance.LoadLevel("Level/lumina_5/$lumina_5F.unity.unity3d", "$lumina_5F");
           
        }
	}
}
