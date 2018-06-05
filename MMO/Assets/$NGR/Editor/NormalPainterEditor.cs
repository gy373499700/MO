using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;


[CustomEditor(typeof(NormalPainter))]
public class NormalPainterEditor : Editor
{

    public Color[] temp_color;
    public Texture2D temp;

    bool Mesh_RayCast(Ray kRay, Vector3[] akPosition, Vector2[] texcoord,int[] aiIndex,ref Vector3 kPoint, ref Vector2 uv,float fMax)
    {//CAN'T USE IN FILESYSTEM
        kRay.direction.Normalize();

        //Vector3[] akPosition = vertices;
        //int[] aiIndex = temp_navdata.indices;

        if (akPosition == null || akPosition.Length == 0)
        {
            Debug.LogError("Mesh_RayCast error");
            return false;
        }

        float fMaxDistance = fMax;
        bool bHit = false;
        Vector2 temp_uv = Vector2.zero;
        for (int i = 0; i < aiIndex.Length; i += 3)
        {
            /*if (Hexagon.Manager.Ray_Triangle(kRay, akPosition[aiIndex[i]], akPosition[aiIndex[i + 1]], akPosition[aiIndex[i + 2]], ref fMaxDistance,ref temp_uv))
            {
                kPoint = kRay.GetPoint(fMaxDistance);
                Vector3 p0 = Vector3.zero;
                Vector3 p1 = akPosition[aiIndex[i + 1]] - akPosition[aiIndex[i]];
                Vector3 p2 = akPosition[aiIndex[i + 2]] - akPosition[aiIndex[i]];
                Vector3 p3 = kPoint - akPosition[aiIndex[i]];

                Vector3 dir2 = p2.normalized;
                float dot = Vector3.Dot(p1, dir2);
                Vector3 intersect1 = dir2 * dot;
                Vector3 p1_inter = p1 - intersect1;
                float dot3 = Vector3.Dot(p1_inter.normalized, p3- intersect1);
                float v = dot3 / p1_inter.magnitude;
                Vector3 dir3 = p3 - p1;
                Vector3 dst = dir3.normalized * dir3.magnitude / (1 - v) + p1;
                float u = dst.magnitude / p2.magnitude;

                Vector2 uv0 = texcoord[aiIndex[i]];
                Vector2 uv1 = texcoord[aiIndex[i+1]];
                Vector2 uv2 = texcoord[aiIndex[i+2]];


                uv = Vector2.Lerp(uv0, uv2, u);
                uv = Vector2.Lerp(uv, uv1, v);

                bHit = true;
            }*/
        }

        return bHit;
    }
#if UNITY_EDITOR
    void OnSceneGUI()
    {

        NormalPainter normal_obj = target as NormalPainter;
        if(normal_obj == null)
        {
            return;
        }
        if(!normal_obj.enabled)
        {
            return;
        }
        if (!normal_obj.Editing)
        {
            HandleUtility.AddDefaultControl(0);
            return;
        }
        Event e = Event.current;
        if (e.isKey)
        {
            if(e.keyCode == KeyCode.LeftBracket)
            {
                normal_obj.paint_size--;
            }
            if (e.keyCode == KeyCode.RightBracket)
            {
                normal_obj.paint_size++;
            }
            if(normal_obj.paint_size<1)
            {
                normal_obj.paint_size = 1;
            }

            
            
        }
        if (e.keyCode == KeyCode.P)
        {
            if (e.type == EventType.KeyDown)
            {
                normal_obj.Erase = true;
            }
            else if (e.type == EventType.keyUp)
            {
                normal_obj.Erase = false;
            }
        }

        int control_id = GUIUtility.GetControlID(FocusType.Passive);
        HandleUtility.AddDefaultControl(control_id);

        Camera[] cams = SceneView.GetAllSceneCameras();
        if (cams == null || cams.Length == 0)
        {
            return;
        }
        Camera sceneCamera = cams[0];

        
        //Debug.Log(e.mousePosition);

        if(e.type == EventType.mouseDown && e.button == 0)
        {
            if (normal_obj.color_control_tex == null)
            {
                Texture2D temp_tex = new Texture2D(normal_obj.paint_tex.width, normal_obj.paint_tex.height, TextureFormat.ARGB32, false);
                string path = AssetDatabase.GetAssetPath(normal_obj.paint_tex);

                Color c = new Color(0.5f, 0.5f, 0.0f, 0.5f);
                Color[] colors = temp_tex.GetPixels();
                for (int i = 0; i < colors.Length; i++)
                {
                    colors[i] = c;
                }
                temp_tex.SetPixels(colors);
                byte[] temp_data = temp_tex.EncodeToPNG();
                path = path.Replace(".PNG", ".png").Replace(".png", "_sp.png");
                File.WriteAllBytes(path, temp_data);

                AssetDatabase.ImportAsset(path);
                TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
                if (ti != null)
                {
                    Debug.Log("modify readable texture format");
                    ti.isReadable = true;
                    ti.textureFormat = TextureImporterFormat.ARGB32;
                }
                AssetDatabase.ImportAsset(path);
                Texture2D control_tex = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
                normal_obj.SetColorControl(control_tex);
            }
            else
            {
                string path = AssetDatabase.GetAssetPath(normal_obj.color_control_tex);
                TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
                if (ti != null)
                {
                    if(!ti.isReadable)
                    {
                        ti.isReadable = true;
                        AssetDatabase.ImportAsset(path);
                    }
                }
            }
            temp_color  = normal_obj.color_control_tex.GetPixels();
            
        }

        if(e.type == EventType.mouseUp && e.button == 0)
        {
            if (temp == null)
            {
                temp = new Texture2D(normal_obj.color_control_tex.width, normal_obj.color_control_tex.height, TextureFormat.ARGB32, false);
            }
            string path = AssetDatabase.GetAssetPath(normal_obj.color_control_tex);
            temp.SetPixels(temp_color);
            byte[] temp_data = temp.EncodeToPNG();
            File.WriteAllBytes(path, temp_data);
            AssetDatabase.ImportAsset(path);
        }
        if (e.type == EventType.mouseDrag && e.button == 0)
        {
            Ray r = HandleUtility.GUIPointToWorldRay(e.mousePosition);

            bool bHit = false;

            r.origin = normal_obj.transform.InverseTransformPoint(r.origin);
            r.direction = normal_obj.transform.InverseTransformDirection(r.direction);

            Vector3[] _pos = normal_obj.mesh.vertices;
            Vector2[] _uvs = normal_obj.mesh.uv;
            int[] index = normal_obj.mesh.triangles;
            Vector3 p = Vector3.zero;
            Vector2 intersect_uv = Vector2.zero;
            if (Mesh_RayCast(r, _pos, _uvs, index, ref p, ref intersect_uv, 10000.0f))
            {
                normal_obj.intersect_point = normal_obj.transform.TransformPoint(p);
                bHit = true;
            }

                RaycastHit hit;
            //if (t4m.mc.Raycast(r, out hit, 10000.0f))
            if(bHit)
            {
                //t4m.intersect_point = hit.point;
                Vector2 uv = intersect_uv;// hit.textureCoord;
                int x = (int)(uv.x * normal_obj.color_control_tex.width);
                int y = (int)(uv.y * normal_obj.color_control_tex.height);

                int w = normal_obj.color_control_tex.width;

                NormalPaintType pt = normal_obj.paint_type;
                //Color paint_color = new Color(0.5f, 0.5f, 0.0f);// normal_obj.paint_power;
                if(normal_obj.Erase)
                {
                    pt = NormalPaintType.Flat;
                    //paint_color = 0.5f;
                }
                float val = 1.0f;
                if (pt == NormalPaintType.Concave)
                {
                    val = -1.0f;
                }
                else if (pt == NormalPaintType.Convex)
                {
                    val = 1;
                }
                else if (pt == NormalPaintType.Flat)
                {
                   val = 0.5f;
                }
                else if (pt == NormalPaintType.Roughness)
                {
                    val = normal_obj.paint_power;
                }                

                Vector2 center = new Vector2(x, y);
                int size = normal_obj.paint_size;
                for (int i = x - size; i>=0&&i < x + size && i < normal_obj.color_control_tex.width; i++)
                {
                    for (int j = y - size; j>=0&&j < y + size && j < normal_obj.color_control_tex.height; j++)
                    {
                        Vector2 pos = new Vector2(i, j);
                        float power = (pos - center).magnitude / (float)size;
                        if(power > 1)
                        {
                            power = 1;  
                        }
                        power = (1 - power);
                        if (pt != NormalPaintType.Flat)
                        {
                            power = power * 0.1f * normal_obj.paint_power;
                        }
                        int idx = j * w + i;
                        Color origin_color = temp_color[idx];
                        Color c = origin_color;
                        if (pt == NormalPaintType.Roughness)
                        {
                            c.b = (c.b * (1 - power) + val * power);
                            c.r = origin_color.r;
                            c.g = origin_color.g;
                            c.a = origin_color.a;
                        }
                        else
                        {
                            Vector2 offset = (pos - center);/// size;
                            if (offset.magnitude <= size)
                            {
                                float lenSize = offset.magnitude / size;
                                float z = (1 - lenSize);
                                if (z < 0)
                                {
                                    z = 0;
                                }
                                z *= val;
                                c.a = (origin_color.a * (1 - power) + val * power);
                                if(c.a<0.0f)
                                {
                                    c.a = 0.0f;
                                }
                                if(c.a>1.0f)
                                {
                                    c.a = 1.0f;
                                }
                            }
                        }

                        temp_color[idx] = c;
                    }
                }

                for (int i = x - size; i >= 0 && i < x + size && i < normal_obj.color_control_tex.width; i++)
                {
                    for (int j = y - size; j >= 0 && j < y + size && j < normal_obj.color_control_tex.height; j++)
                    {
                        int idx = j * w + i;
                        Color origin_color = temp_color[idx];
                        int right = j * w + i+1;
                        if (w + 1 == normal_obj.color_control_tex.width)
                        {
                            right = idx;
                        }
                        Color right_color = temp_color[right];
                        int down = j * (w+1) + i;
                        if(w+1 == normal_obj.color_control_tex.height)
                        {
                            down = idx;
                        }
                        Color down_color = temp_color[idx];

                        float x_val = (origin_color.a-right_color.a) * 2.0f;
                        if (x_val > 1.0f)
                        {
                            x_val = 1.0f;
                        }
                        if(x_val<-1.0f)
                        {
                            x_val = -1.0f;
                        }
                        float y_val = (down_color.a - origin_color.a) * 2.0f;
                        if (y_val > 1.0f)
                        {
                            y_val = 1.0f;
                        }
                        if (y_val < -1.0f)
                        {
                            y_val = -1.0f;
                        }
                        Vector2 v = new Vector2(x_val, y_val);
                        
                        if(v.magnitude > 1.0f)
                        {
                            v.Normalize();

                            origin_color.r = v.x * 0.5f + 0.5f;
                            origin_color.g = v.y * 0.5f + 0.5f;
                        }
                        else
                        {
                            Vector3 v3 = new Vector3(x_val, y_val, 0);
                            v3.z = Mathf.Sqrt(1 - v.sqrMagnitude);
                            origin_color.r = v3.x * 0.5f + 0.5f;
                            origin_color.g = v3.y * 0.5f + 0.5f;
                        }
                        temp_color[idx] = origin_color;
                    }
                }
                normal_obj.color_control_tex.SetPixels(temp_color);
                normal_obj.color_control_tex.Apply();
            }

            
        }
        else
        {
            //Handles.DrawSolidDisc(t4m.intersect_point, Vector3.up, 0.5f);
        }
        //if (e.type == EventType.mouseDown && e.button == 0)
        //{
        //    Ray r = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        //    r.origin = t4m.transform.InverseTransformPoint(r.origin);
        //    r.direction = t4m.transform.InverseTransformDirection(r.direction);
        //
        //    Vector3[] pos = t4m.mesh.vertices;
        //    Vector2[] uv = t4m.mesh.uv;
        //    int[] index = t4m.mesh.triangles;
        //    Vector3 p = Vector3.zero;
        //    Vector2 intersect_uv = Vector2.zero;
        //    if (Mesh_RayCast(r, pos, uv, index, ref p, ref intersect_uv, 10000.0f))
        //    {
        //        t4m.intersect_point = t4m.transform.TransformPoint(p);
        //        //if (e.type == EventType.mouseDrag && e.button == 1)
        //        {
        //            Debug.Log(t4m.intersect_point +" "+ intersect_uv.x + " "+ intersect_uv.y);
        //        }
        //    }
        //}
        //Ray terrain = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        //MeshCollider mc = null; ;
        //if(mc.Raycast(r,)

    }

#endif

}