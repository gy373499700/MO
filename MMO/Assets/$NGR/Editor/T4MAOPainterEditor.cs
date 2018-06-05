using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;


[CustomEditor(typeof(T4MAOPainter))]
public class T4MAOPainterEditor : Editor
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
          /*  if (Hexagon.Manager.Ray_Triangle(kRay, akPosition[aiIndex[i]], akPosition[aiIndex[i + 1]], akPosition[aiIndex[i + 2]], ref fMaxDistance,ref temp_uv))
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

        T4MAOPainter t4m = target as T4MAOPainter;
        if(t4m == null)
        {
            return;
        }
        if(!t4m.enabled)
        {
            return;
        }
        if (!t4m.Editing)
        {
            HandleUtility.AddDefaultControl(0);
            return;
        }
        Event e = Event.current;
        if (e.isKey)
        {
            if(e.keyCode == KeyCode.LeftBracket)
            {
                t4m.paint_size--;
            }
            if (e.keyCode == KeyCode.RightBracket)
            {
                t4m.paint_size++;
            }
            if(t4m.paint_size<1)
            {
                t4m.paint_size = 1;
            }

            
            
        }
        if (e.keyCode == KeyCode.P)
        {
            if (e.type == EventType.KeyDown)
            {
                t4m.Erase = true;
            }
            else if (e.type == EventType.keyUp)
            {
                t4m.Erase = false;
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
            if(t4m.color_control_tex==null)
            {
                Texture2D temp_tex = new Texture2D(t4m.paint_tex.width, t4m.paint_tex.height, TextureFormat.ARGB32, false);
                string path = AssetDatabase.GetAssetPath(t4m.paint_tex);

                Color c = new Color(0.5f, 0.0f, 0.0f, 1.0f);
                Color[] colors = temp_tex.GetPixels();
                for(int i=0;i< colors.Length;i++)
                {
                    colors[i] = c;
                }
                temp_tex.SetPixels(colors);
                byte[] temp_data = temp_tex.EncodeToPNG();
                path = path.Replace(".PNG", ".png").Replace(".png", "_AO.png");
                File.WriteAllBytes(path, temp_data);
                
                AssetDatabase.ImportAsset(path);
                TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
                if(ti!=null)
                {
                    Debug.Log("modify readable texture format");
                    ti.isReadable = true;
                    ti.textureFormat = TextureImporterFormat.ARGB32;
                }
                AssetDatabase.ImportAsset(path);
                Texture2D control_tex = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
                t4m.SetColorControl(control_tex);
            }
            temp_color  =   t4m.color_control_tex.GetPixels();
            
        }

        if(e.type == EventType.mouseUp && e.button == 0)
        {
            if (temp == null)
            {
                temp = new Texture2D(t4m.color_control_tex.width, t4m.color_control_tex.height, TextureFormat.ARGB32, false);
            }
            string path = AssetDatabase.GetAssetPath(t4m.color_control_tex);
            temp.SetPixels(temp_color);
            byte[] temp_data = temp.EncodeToPNG();
            File.WriteAllBytes(path, temp_data);
            AssetDatabase.ImportAsset(path);
        }
        if (e.type == EventType.mouseDrag && e.button == 0)
        {
            Ray r = HandleUtility.GUIPointToWorldRay(e.mousePosition);

            bool bHit = false;

            r.origin = t4m.transform.InverseTransformPoint(r.origin);
            r.direction = t4m.transform.InverseTransformDirection(r.direction);

            Vector3[] _pos = t4m.mesh.vertices;
            Vector2[] _uvs = t4m.mesh.uv;
            int[] index = t4m.mesh.triangles;
            Vector3 p = Vector3.zero;
            Vector2 intersect_uv = Vector2.zero;
            if (Mesh_RayCast(r, _pos, _uvs, index, ref p, ref intersect_uv, 10000.0f))
            {
                t4m.intersect_point = t4m.transform.TransformPoint(p);
                bHit = true;
            }

                RaycastHit hit;
            //if (t4m.mc.Raycast(r, out hit, 10000.0f))
            if(bHit)
            {
                //t4m.intersect_point = hit.point;
                Vector2 uv = intersect_uv;// hit.textureCoord;
                int x = (int)(uv.x * t4m.color_control_tex.width);
                int y = (int)(uv.y * t4m.color_control_tex.height);

                int w = t4m.color_control_tex.width;

                PaintType pt = t4m.paint_type;
                Color paint_color = t4m.paint_color;
                float paint_power = paint_color.a;
                paint_color.a = 1.0f;
                if (t4m.Erase)
                {
                    if (pt == PaintType.Dark || pt == PaintType.Light || pt == PaintType.Fixed)
                    {
                        pt = PaintType.Fixed;
                        paint_color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
                    }
                    else if (pt == PaintType.Wet || pt == PaintType.Dry || pt == PaintType.FixedWetness)
                    {
                        pt = PaintType.FixedWetness;
                        paint_color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
                    }
                }


                Vector2 center = new Vector2(x, y);
                int size = t4m.paint_size;
                for (int i = x - size; i>=0&&i < x + size && i < t4m.color_control_tex.width; i++)
                {
                    for (int j = y - size; j>=0&&j < y + size && j < t4m.color_control_tex.height; j++)
                    {
                        Vector2 pos = new Vector2(i, j);
                        float power = (pos - center).magnitude / (float)size;
                        if(power > 1)
                        {
                            continue;
                        }
                        power = (1 - power)*0.1f* paint_color.a;
                       
                        int idx = j * w + i;
                        Color c = temp_color[idx];
                        if (pt == PaintType.Fixed)
                        {
                            c.r = (c.r* (1-power) + paint_color.r * power) ;
                        }
                        if (pt == PaintType.FixedWetness)
                        {
                            c.g = (c.g * (1 - power) + paint_color.r * power);
                        }
                        else if (pt == PaintType.Dark)
                        {
                            c.r *= (1 - power);
                        }
                        else if (pt == PaintType.Light)
                        {
                            c.r *= (1 + power);
                        }
                        else if (pt == PaintType.Wet)
                        {
                            c.g += (power * 0.4f);
                        }
                        else if (pt == PaintType.Dry)
                        {
                            c.g += (- power*0.4f );
                        }

                        temp_color[idx] = c;
                    }
                }
                t4m.color_control_tex.SetPixels(temp_color);
                t4m.color_control_tex.Apply();
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