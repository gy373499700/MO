using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class UIWndMask : MonoBehaviour {

    public static List<UIWndMask> m_Mask = new List<UIWndMask>();
	// Use this for initialization
	void OnEnable ()
    {
        m_Mask.Add(this);
    }
	
	// Update is called once per frame
	void OnDisable ()
    {
        m_Mask.Remove(this);
    }
//     void Update()
//     {
//         if (targetUI != null)
//         {
//             m_Bounds = targetUI.bounds;
//         }
//     }
    public BoxCollider targetUI = null;

    public Bounds m_Bounds = new Bounds();
    public Vector3 P1//左下
    {
        get
        {
            return m_Bounds.center - m_Bounds.extents*1.01f; 
        }
    }
    public Vector3 P2//左上
    {
        get
        {
            return m_Bounds.center + new Vector3(-m_Bounds.extents.x, m_Bounds.extents.y, m_Bounds.extents.z) * 1.01f;
        }
    }
    public Vector3 P3
    {
        get
        {
            return m_Bounds.center + m_Bounds.extents * 1.01f;
        }
    }
    public Vector3 P4
    {
        get
        {
            return m_Bounds.center + new Vector3(m_Bounds.extents.x, -m_Bounds.extents.y, m_Bounds.extents.z) * 1.01f;
        }
    }
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (targetUI != null)
        {
            m_Bounds = targetUI.bounds;
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(P1, P2);
        Gizmos.DrawLine(P2, P3);
        Gizmos.DrawLine(P3, P4);
        Gizmos.DrawLine(P4, P1);

    }
#endif
}
