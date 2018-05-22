using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFloated : MonoBehaviour {

    public bool isAdvanceMode = false;
    public Water destWater;
    public Transform destTran;
    public float yOffset = 0f;

    public Vector2 sampleLeftFront = new Vector2(-1f, 2f);
    public Vector2 sampleRightFront = new Vector2(1f, 2f);
    public Vector2 sampleLeftBack = new Vector2(-1, -2f);
    public Vector2 sampleRightBack = new Vector2(1f, -2f);

    Vector3 originPos;
    Vector3 originOffs;
    Transform m_trans;
    Vector3[] sampleWorldPos;
    Vector3[] sampleWavePos;

    private static Water _gWater;
    private static Water globalWater
    {
        get
        {
            if(_gWater == null)
            {
                if(Water.WaterList != null)
                {
                    for (int i = 0; i < Water.WaterList.Count; i++)
                    {
                        if(Water.WaterList[i].type == Water.WaterType.ocean)
                        {
                            _gWater = Water.WaterList[i];
                            break;
                        }
                    }
                }
                if (_gWater == null)
                    Debug.LogError("Find none ocean.");
            }
            return _gWater;
        }
    }

    void Start()
    {
        m_trans = transform;
        originPos = m_trans.position;
        sampleWorldPos = new Vector3[4];
        sampleWavePos = new Vector3[4];
        if (destTran == null)
            destTran = m_trans.GetChild(0);
        originOffs = destTran.position - m_trans.position;
    }

	void Update () {
        Water w = destWater != null ? destWater : globalWater;

        if (w == null)
            return;

        if (!isAdvanceMode)
        {
            Vector3 _p;
            Vector3 _n;
            w.GetGerstnerWavePos(originPos, out _p, out _n);
            _p.y += yOffset;
            m_trans.position = _p;
            m_trans.rotation = Quaternion.FromToRotation(Vector3.up, _n);
            return;
        }

        if (destTran == null)
            return;

        Matrix4x4 m = Matrix4x4.TRS(m_trans.position + originOffs, m_trans.rotation, m_trans.lossyScale);

        sampleWorldPos[0] = m.MultiplyPoint3x4(new Vector3(sampleLeftFront.x, 0, sampleLeftFront.y));
        sampleWorldPos[1] = m.MultiplyPoint3x4(new Vector3(sampleRightFront.x, 0, sampleRightFront.y));
        sampleWorldPos[2] = m.MultiplyPoint3x4(new Vector3(sampleLeftBack.x, 0, sampleLeftBack.y));
        sampleWorldPos[3] = m.MultiplyPoint3x4(new Vector3(sampleRightBack.x, 0, sampleRightBack.y));
        
        Vector3 wavePos;
        Vector3 waveNrml;
        Vector3 averWorldPos = Vector3.zero;
        for (int i = 0; i < 4; i++)
        {
            w.GetGerstnerWavePos(sampleWorldPos[i], out wavePos, out waveNrml);
            sampleWavePos[i] = wavePos;
            averWorldPos += wavePos;
        }
        averWorldPos /= 4f;
        averWorldPos.y += yOffset;
        Vector3 averNormal = Vector3.zero;
        Plane _p1 = new Plane(sampleWavePos[0], sampleWavePos[1], sampleWavePos[2]);
        Plane _p2 = new Plane(sampleWavePos[1], sampleWavePos[3], sampleWavePos[2]);

        averNormal = m_trans.InverseTransformDirection((_p1.normal + _p2.normal) / 2f);

        destTran.position = averWorldPos;
        destTran.localRotation = Quaternion.FromToRotation(Vector3.up, averNormal);
    }

    void OnDrawGizmos()
    {
        if (sampleWorldPos == null || sampleWavePos == null || !Application.isPlaying)
            return;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(sampleWorldPos[0], sampleWorldPos[1]);
        Gizmos.DrawLine(sampleWorldPos[0], sampleWorldPos[2]);
        Gizmos.DrawLine(sampleWorldPos[3], sampleWorldPos[1]);
        Gizmos.DrawLine(sampleWorldPos[3], sampleWorldPos[2]);
        Gizmos.DrawLine(sampleWorldPos[2], sampleWorldPos[1]);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(sampleWavePos[0], sampleWavePos[1]);
        Gizmos.DrawLine(sampleWavePos[0], sampleWavePos[2]);
        Gizmos.DrawLine(sampleWavePos[3], sampleWavePos[1]);
        Gizmos.DrawLine(sampleWavePos[3], sampleWavePos[2]);
        Gizmos.DrawLine(sampleWavePos[2], sampleWavePos[1]);
    }
}
