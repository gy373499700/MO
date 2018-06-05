using UnityEngine;
using System;


public class Frustum
{
    public Camera camera;
    Plane[] clipplane = new Plane[6];
    public void Buildclipplane(Camera cam)
    {
        camera = cam;

        Vector3 pos = cam.transform.position;
        Vector3 _dir = cam.transform.TransformDirection(Vector3.forward);
        Vector3 _up = cam.transform.TransformDirection(Vector3.up);
        float _near = cam.nearClipPlane;
        float _far = cam.farClipPlane;
        float _angle = 3.14159265f * cam.fieldOfView / 180.0f;
        float _aspect = cam.aspect;

        // Vector3 dir;
        Vector3 Z = new Vector3(-_dir.x, -_dir.y, -_dir.z);
        Z.Normalize();

        Vector3 X = Vector3.Cross(_up, Z);
        X.Normalize();

        Vector3 Y = Vector3.Cross(Z, X);

        Vector3 nc = pos - Z * _near;
        Vector3 fc = pos - Z * _far;

        //Vector3 ntl, ntr, nbl, nbr, ftl, ftr, fbl, fbr;
        float nw, nh, fw, fh;

        float tang = (float)Mathf.Tan(_angle * 0.5f);
        nh = _near * tang;
        nw = nh * _aspect;
        fh = _far * tang;
        fw = fh * _aspect;

        Vector3 ntl = nc + Y * nh - X * nw;
        Vector3 ntr = nc + Y * nh + X * nw;
        Vector3 nbl = nc - Y * nh - X * nw;
        Vector3 nbr = nc - Y * nh + X * nw;

        Vector3 ftl = fc + Y * fh - X * fw;
        Vector3 ftr = fc + Y * fh + X * fw;
        Vector3 fbl = fc - Y * fh - X * fw;
        Vector3 fbr = fc - Y * fh + X * fw;

        //clipplane[0] = new Plane(ntr, ntl, ftl);//enFS_Top
        //clipplane[1] = new Plane(nbl, nbr, fbr);//enFS_Bottom
        //clipplane[2] = new Plane(ntl, nbl, fbl);//enFS_Left
        //clipplane[3] = new Plane(nbr, ntr, fbr);//enFS_Right
        //clipplane[4] = new Plane(ntl, ntr, nbr);//enFS_Near
        //clipplane[5] = new Plane(ftr, ftl, fbl);//enFS_Far

        //ÓÅ»¯ÊÓ×¶²Ã¼ôË³Ðò
        //clipplane[3] = new Plane(ntr, ntl, ftl);//enFS_Top
        //clipplane[4] = new Plane(nbl, nbr, fbr);//enFS_Bottom
        //clipplane[1] = new Plane(ntl, nbl, fbl);//enFS_Left
        //clipplane[2] = new Plane(nbr, ntr, fbr);//enFS_Right
        //clipplane[0] = new Plane(ntl, ntr, nbr);//enFS_Near
        //clipplane[5] = new Plane(ftr, ftl, fbl);//enFS_Far


        clipplane[3].Set3Points(ntr, ntl, ftl);//enFS_Top
        clipplane[4].Set3Points(nbl, nbr, fbr);//enFS_Bottom
        clipplane[1].Set3Points(ntl, nbl, fbl);//enFS_Left
        clipplane[2].Set3Points(nbr, ntr, fbr);//enFS_Right
        clipplane[0].Set3Points(ntl, ntr, nbr);//enFS_Near
        clipplane[5].Set3Points(ftr, ftl, fbl);//enFS_Far
    }
    public bool IsVisiable(Vector3 p)
    {

        for (int i = 0; i < 6; i++)
        {
            float val = Vector3.Dot(p, clipplane[i].normal) + clipplane[i].distance;
            if (val < 0.0f)
            {
                return false;
            }
        }
        return true;
    }
    public bool IsVisiable(Vector3 Center, float radius)
    {

        for (int i = 0; i < 6; i++)
        {
            float val = Vector3.Dot(Center, clipplane[i].normal) + clipplane[i].distance;
            if (val < -radius)
            {
                return false;
            }
        }
        return true;
    }
    public bool IsVisiable(Vector3 Center, float radius,int mask)
    {

        for (int i = 0; i < 6; i++)
        {
            if (((1 << i) & mask) != 0)
            {
                float val = Vector3.Dot(Center, clipplane[i].normal) + clipplane[i].distance;
                if (val < -radius)
                {
                    return false;
                }
            }
        }
        return true;
    }
    public bool IsVisiable(Vector3[] bound)
    {
        
        for (int i = 0; i < 6; i++)
        {
            int ivisiable = 0;
            for (int j = 0; j < 8; j++)
            {
                float val = Vector3.Dot(bound[j], clipplane[i].normal) + clipplane[i].distance;
                if (val < 0.0f)
                {
                    ivisiable++;
                }
            }
            if(ivisiable==8)
            {
                return false;
            }
        }
        return true;
    }
}