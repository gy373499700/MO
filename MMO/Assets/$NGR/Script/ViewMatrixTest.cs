using UnityEngine;
using System.Collections;

public class ViewMatrixTest : MonoBehaviour {
    public GameObject obj = null;
    public Vector3 view;
    public Vector3 shadow_view;
    public Vector3 shadow_view2;
    public Vector3 shadow_proj;
    public Vector3 shadow_proj2;
    public Vector3 ptemp_view;
    public Vector3 proj_pos;
    public Camera shadow_cam;
    //public Camera mainCamera
	// Use this for initialization
	void Start () {
        Vector3[] samples = new Vector3[15];
        Vector3 baseVec = new Vector3(0,0,1);
        string str = "";
        float angle = 360.0f/7.0f;
        for (int i = 0; i < 7; i++)
        {
            Quaternion q = Quaternion.AngleAxis(angle * i, Vector3.up);
            samples[i] = q * baseVec;
            str += samples[i].x + "," + samples[i].z+ "\n";
        }
        Debug.Log(str);
	}
    public static Vector3 TransformPoint(Vector4[] mat, Vector3 p)
    {
        Vector4 v = new Vector4(p.x, p.y, p.z, 1);
        Vector3 p3;
        p3.x = Vector4.Dot(v, mat[0]);
        p3.y = Vector4.Dot(v, mat[1]);
        p3.z = Vector4.Dot(v, mat[2]);
        return p3;
    }
    public static Vector3 CameraFarCorner(Camera cam)
    {
        Vector3 corner = Vector3.one;
        float w_h = Screen.width / (float)Screen.height;
        corner.y = cam.farClipPlane * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        corner.z = cam.farClipPlane;
        corner.x = w_h * corner.y;
        return corner;
    }
	// Update is called once per frame
	void Update () {

        Matrix4x4 world_camera = Matrix4x4.identity;
        RenderPipeline.ViewMatrix(Camera.main, ref world_camera);
        Vector4[] world_camera2 = new Vector4[4];
        RenderPipeline.ViewMatrix(Camera.main, ref world_camera2);
        Matrix4x4 camera_world = world_camera.inverse;
        Vector4[] camera_world2 = new Vector4[4];
        camera_world2[0] = camera_world.GetRow(0);
        camera_world2[1] = camera_world.GetRow(1);
        camera_world2[2] = camera_world.GetRow(2);
        camera_world2[3] = camera_world.GetRow(3);
        Vector3 p = transform.position;

        view = world_camera.MultiplyPoint(p);
        Vector3 v_trans = camera_world.MultiplyPoint(view);


        Matrix4x4 shadow_cam_world = Matrix4x4.identity;
        RenderPipeline.ViewMatrix(shadow_cam, ref shadow_cam_world);

        Matrix4x4 mainview_shadow_view = shadow_cam_world*camera_world ;
        shadow_view = mainview_shadow_view.MultiplyPoint(view);
        shadow_view2 = shadow_cam_world.MultiplyPoint(p);
        shadow_proj = GL.GetGPUProjectionMatrix(shadow_cam.projectionMatrix, true).MultiplyPoint(shadow_view);
        Matrix4x4 matProj = Matrix4x4.identity;
        RenderPipeline.ProjMatrix(shadow_cam, ref matProj);
        shadow_proj2 = matProj.MultiplyPoint(shadow_view);

        Vector3 p2 = TransformPoint(world_camera2, p);
        Vector3 p_proj = GL.GetGPUProjectionMatrix(Camera.main.projectionMatrix,true).MultiplyPoint(p2);
        Vector3 farCorner = CameraFarCorner(Camera.main);

        float dis = p2.z / Camera.main.farClipPlane;
        Vector3 viewdir = new Vector3(-p_proj.x * farCorner.x, p_proj.y * farCorner.y, farCorner.z);
        ptemp_view = viewdir * dis;

        Vector3 p3 = TransformPoint(camera_world2, ptemp_view);
        

        obj.transform.position = p3;

        proj_pos = p_proj;
	}
}
