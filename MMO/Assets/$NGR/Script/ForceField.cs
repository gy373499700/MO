using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ForceField : MonoBehaviour {
    public static List<ForceField> FFList = new List<ForceField>();

    public float    radius = 1.0f;
    public float    forcepower = 1.0f;
    public bool     velocityIsPower = false;
    private float   velocity = 0.0f;
    private float   velocityPower = 0.0f;
    private Vector3 LastPosition = Vector3.zero;
	// Use this for initialization
    void Awake()
    {
        LastPosition = transform.position;
    }
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        float len = (transform.position - LastPosition).magnitude;
        float t = Time.deltaTime;
        if(t < 0.016f)
        {
            t = 0.016f;
        }
        float v = len /t;
        if (v > 0.01f)
        {
            velocity += v;
            if(velocity > 5.0f)
            {
                velocity = 5.0f;
            }
        }
        else
        {
            velocity -= t *2.0f;
        }
        LastPosition = transform.position;
    }
    void OnEnable()
    {
        FFList.Add(this);
    }
    void OnDisable()
    {
        FFList.Remove(this);
    }

    static Matrix4x4 matrix0 = new Matrix4x4();
    static Matrix4x4 matrix1 = new Matrix4x4();
    static Matrix4x4 matrix2 = new Matrix4x4();
    static Vector4 v = Vector4.zero;
    public static void SubmitShaderParam(Vector3 center,Frustum mainCameraFrustum)
    {
        int iCount = 0;
        for (int i=0;i< FFList.Count;i++)
        {
            ForceField ff = FFList[i];
            Vector3 pos = ff.transform.position;
            if(ff.radius < 0.01f)
            {
                continue;
            }
            float force = ff.forcepower;
            if (ff.velocityIsPower)
            {
                float fv = ff.velocity;
                if(fv > 1.5f)
                {
                    fv = 1.5f;
                }
                force *= fv;
            }
            if(force<=0.01f)
            {
                continue;
            }
            if((center- pos).magnitude > 50.0f)
            {
                continue;
            }
            if(!mainCameraFrustum.IsVisiable(pos,ff.radius))
            {
                continue;
            }
            //Vector4 v = new Vector4(pos.x, pos.z, ff.radius, force);
            v.x = pos.x;
            v.y = pos.z;
            v.z = ff.radius;
            v.w = force;
            if(iCount < 4)
            {
                matrix0.SetRow(iCount % 4, v);
            }
            else if (iCount < 8)
            {
                matrix1.SetRow(iCount % 4, v);
            }
            else
            {
                matrix2.SetRow(iCount % 4, v);
            }
            iCount++;
            if(iCount>=12)
            {
                break;
            }
        }
        Shader.SetGlobalMatrix("ForceField0", matrix0);
        Shader.SetGlobalMatrix("ForceField1", matrix1);
        Shader.SetGlobalMatrix("ForceField2", matrix2);
    }
    void OnDrawGizmos()
    {

        Gizmos.DrawIcon(transform.position, "ForceField.png");
    }
    void OnDrawGizmosSelected()
    {

        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
