using UnityEngine;
using System.Collections;

public class PhysicsTrigger : MonoBehaviour {
    public GameObject[] activeObject;
    public GameObject[] disactiveObject;
    public GameObject[] leaveactiveObject;
    public GameObject[] leavedisactiveObject;
    public  void OnTriggerEnter(Collider obj)
    {
        for(int i=0;i< activeObject.Length; i++)
        {
            activeObject[i].SetActive(true);
        }
        for (int i = 0; i < disactiveObject.Length; i++)
        {
            disactiveObject[i].SetActive(false);
        }

    }

    public  void OnTriggerExit(Collider obj)
    {
        for (int i = 0; i < leaveactiveObject.Length; i++)
        {
            leaveactiveObject[i].SetActive(true);
        }
        for (int i = 0; i < leavedisactiveObject.Length; i++)
        {
            leavedisactiveObject[i].SetActive(false);
        }
    }
}
