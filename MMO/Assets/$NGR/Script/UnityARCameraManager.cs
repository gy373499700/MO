using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_5_6
using UnityEngine.XR.iOS;
#endif

public class UnityARCameraManager : MonoBehaviour {

    public Camera m_camera;
#if UNITY_5_6
    private UnityARSessionNativeInterface m_session;
#endif
    private Material savedClearMaterial;

	// Use this for initialization
	public void StartAR () {
#if UNITY_5_6
        if (m_session!=null)
        {
            m_session.Run();
            return;
        }

		m_session = UnityARSessionNativeInterface.GetARSessionNativeInterface();

#if !UNITY_EDITOR
		//Application.targetFrameRate = 60;
        ARKitWorldTackingSessionConfiguration config = new ARKitWorldTackingSessionConfiguration();
        config.planeDetection = UnityARPlaneDetection.Horizontal;
        config.alignment = UnityARAlignment.UnityARAlignmentGravity;
        config.getPointCloudData = true;
        config.enableLightEstimation = true;
        m_session.RunWithConfig(config);

		if (m_camera == null) {
			m_camera = Camera.main;
		}
#else
		//put some defaults so that it doesnt complain
		UnityARCamera scamera = new UnityARCamera ();
		scamera.worldTransform = new UnityARMatrix4x4 (new Vector4 (1, 0, 0, 0), new Vector4 (0, 1, 0, 0), new Vector4 (0, 0, 1, 0), new Vector4 (0, 0, 0, 1));
		Matrix4x4 projMat = Matrix4x4.Perspective (60.0f, 1.33f, 0.1f, 30.0f);
		scamera.projectionMatrix = new UnityARMatrix4x4 (projMat.GetColumn(0),projMat.GetColumn(1),projMat.GetColumn(2),projMat.GetColumn(3));

		UnityARSessionNativeInterface.SetStaticCamera (scamera);

#endif
#endif
    }
    public void StopAR()
    {
#if UNITY_5_6
        if (m_session != null)
        {
            m_session.Pause();
        }
#endif
    }
    public void GetARPosition(ref Vector3 p,ref Quaternion q)
    {
#if UNITY_5_6
        Matrix4x4 matrix = m_session.GetCameraPose();
        p = UnityARMatrixOps.GetPosition(matrix);
        q = UnityARMatrixOps.GetRotation(matrix);
#endif
    }

	public void SetCamera(Camera newCamera)
	{

	}

	private void SetupNewCamera(Camera newCamera)
	{

	}

	// Update is called once per frame

	void Update () {
		

	}

}
