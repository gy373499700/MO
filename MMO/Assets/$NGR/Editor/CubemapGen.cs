using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public class CubemapGen {
    [MenuItem("Assets/sdFileSystem/TestRT")]
    static void TestRT()
    {
        //6t
        //Material matOrigin = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/$NGR/Materials/matGenCubemap.mat");
        //Material matCopy = new Material(matOrigin);
        //for (int i = 0; i < 6;i++)
        //{
        //    Texture tJpg = AssetDatabase.LoadAssetAtPath<Texture>("Assets/$NGR/Cubemaps/Temp/" + i.ToString() + ".jpg");
        //    matCopy.SetTexture("CubeFace" + i.ToString(), tJpg);
        //}
        //RenderTexture sphere_tex = new RenderTexture(4096, 2048, 0, RenderTextureFormat.ARGB32);
        //Graphics.Blit(null, sphere_tex, matCopy, 0);
        //Texture2D save = new Texture2D(4096, 2048);
        //save.ReadPixels(new Rect(0, 0, save.width, save.height), 0, 0);
        //byte[] data = save.EncodeToJPG();
        //File.WriteAllBytes("Assets/$NGR/Cubemaps/Temp/" + "globalScene" + ".cubemap.jpg", data);
        //AssetDatabase.Refresh();
        //cube
        Material matOrigin = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/$NGR/Materials/matGenCubemap.mat");
        Material matCopy = new Material(matOrigin);
        Cubemap cube = AssetDatabase.LoadAssetAtPath<Cubemap>("Assets/$NGR/Cubemaps/Temp/t.cubemap");
        matCopy.SetTexture("CubeMap", cube);
        RenderTexture sphere_tex = new RenderTexture(4096, 2048, 0, RenderTextureFormat.ARGB32);
        Graphics.Blit(null, sphere_tex, matCopy, 1);

        Texture2D save = new Texture2D(4096, 2048);
        save.ReadPixels(new Rect(0, 0, save.width, save.height), 0, 0);

        byte[] data = save.EncodeToJPG();
        File.WriteAllBytes("Assets/$NGR/Cubemaps/Temp/" + "globalScene1" + ".cubemap.jpg", data);
        AssetDatabase.Refresh();
    }
    [MenuItem("Assets/sdFileSystem/Gen Cubemap1")]
    static void Generate1()
    {
        if(Selection.objects != null && Selection.objects.Length > 0)
        {
            UnityEngine.Object obj = Selection.objects[0];
            GameObject temp = obj as GameObject;
            if(temp != null)
            {
                Vector3 worldPos = temp.transform.position;
                CubemapCreator.Create(worldPos);
                return;
                if (RenderPipeline._instance == null)
                {
                    Debug.LogError("RenderPipeline Instance Not Found.");
                    return;
                }

                if (!Directory.Exists("Assets/$NGR/Cubemaps/Temp"))
                {
                    Directory.CreateDirectory("Assets/$NGR/Cubemaps/Temp");
                }

                int size = 1024;
                Material matOrigin = AssetDatabase.LoadAssetAtPath<Material>("Assets/$NGR/Materials/matGenCubemap.mat");
                Material matCopy = new Material(matOrigin);

                RenderPipeline._instance.transform.position = worldPos;        
                
                GameObject.FindObjectOfType<UICamera>().GetComponent<Camera>().enabled = false;
                Camera mainCam = RenderPipeline._instance.GetComponent<Camera>();
                float originFov = mainCam.fieldOfView;
                Vector3 originPos = RenderPipeline._instance.transform.position;
                Quaternion originRot = RenderPipeline._instance.transform.rotation;

                mainCam.fieldOfView = 90f;
                SceneRenderSetting._Setting.CameraFOV = 90f;
                
                Vector3[] dir = new Vector3[6];
                dir[0] = new Vector3(0, 0, 1);
                dir[1] = new Vector3(1, 0, 0);
                dir[2] = new Vector3(0, 0, -1);
                dir[3] = new Vector3(-1, 0, 0);
                dir[4] = new Vector3(0, 1, 0);
                dir[5] = new Vector3(0, -1, 0);
                RenderTexture[] cache = new RenderTexture[6];
                for (int i = 0; i < 6; i++)
                {
                    if (i < 4)
                    {
                        mainCam.transform.rotation = Quaternion.AngleAxis(i * 90.0f, Vector3.up);
                    }
                    else
                    {
                        mainCam.transform.rotation = Quaternion.FromToRotation(Vector3.forward, dir[i]);
                    }
                    
                    if (cache[i] == null)
                    {
                        int depth = 0;
                        if (i == 0)
                        {
                            depth = 24;
                        }
                        cache[i] = new RenderTexture(size, size, depth, RenderTextureFormat.ARGB32);
                        cache[i].filterMode = FilterMode.Point;
                    }

                    mainCam.SetTargetBuffers(cache[i].colorBuffer, cache[0].depthBuffer);
                    mainCam.Render();
                }
                RenderTexture sphere_tex = new RenderTexture(size * 2, size, 0, RenderTextureFormat.ARGB32);
                //matCopy.SetTexture("CubeFace0", cache[0]);
                //matCopy.SetTexture("CubeFace1", cache[1]);
                //matCopy.SetTexture("CubeFace2", cache[2]);
                //matCopy.SetTexture("CubeFace3", cache[3]);
                //matCopy.SetTexture("CubeFace4", cache[4]);
                //matCopy.SetTexture("CubeFace5", cache[5]);
                
                Cubemap cube = new Cubemap(size, TextureFormat.ARGB32, false);
                Texture2D t = new Texture2D(size, size, TextureFormat.ARGB32,false);
                for(int i=0;i<6;i++)
                {
                    RenderTexture.active = cache[i];
                    t.ReadPixels(new Rect(0, 0, t.width, t.height), 0, 0);
                    File.WriteAllBytes("Assets/$NGR/Cubemaps/Temp/" + i.ToString() + ".cubemap.jpg", t.EncodeToJPG());
                    cube.SetPixels(t.GetPixels(),(CubemapFace)i);
                }
                
                matCopy.SetTexture("CubeMap", cube);
                Graphics.Blit(null, sphere_tex, matCopy, 0);

                Texture2D save = new Texture2D(size * 2, size);
                save.ReadPixels(new Rect(0, 0, save.width, save.height), 0, 0);
                
                byte[] data = save.EncodeToJPG();
                File.WriteAllBytes("Assets/$NGR/Cubemaps/Temp/" + temp.name + ".cubemap.jpg", data);

                GameObject.FindObjectOfType<UICamera>().GetComponent<Camera>().enabled = true;
                mainCam.fieldOfView = originFov;
                RenderPipeline._instance.transform.position = originPos;
                RenderPipeline._instance.transform.rotation = originRot;
                //RenderTexture.active.Release();
                Object.DestroyImmediate(save);
                Object.DestroyImmediate(cube);
                for (int i = 0, iMax = cache.Length; i < iMax; i++)
                {
                    if (cache[i] != null)
                    {
                        cache[i].Release();
                        cache[i] = null;
                    }
                }
                cache = null;
                sphere_tex.Release();
                sphere_tex = null;
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogError("Didn't select correct gameobject.");
            }
            
        }
    }

    // Use this for initialization
    [MenuItem("Assets/sdFileSystem/Gen Cubemap")]
    static void Generate()
    {
        Material matOrigin = AssetDatabase.LoadAssetAtPath<Material>("Assets/$NGR/Materials/matGenCubemap.mat");
        Material matCopy = new Material(matOrigin);
        for (int i = 0; i < Selection.objects.Length; i++)
        {
            UnityEngine.Object obj = Selection.objects[i];
            GameObject temp = obj as GameObject;
            if(temp!=null)
            {
                Vector3 pos = temp.transform.position;

                
                GameObject CameraObj = new GameObject();
                CameraObj.transform.position = pos;
                Camera cam = CameraObj.AddComponent<Camera>();
                if(cam!=null)
                {
                    
                    if (SceneRenderSetting._Setting != null)
                    {
                        cam.clearFlags = CameraClearFlags.SolidColor;
                        Vector4 v = SceneRenderSetting._Setting.AmbientColor * SceneRenderSetting._Setting.AmbientColorScale;
                        v.w = 1;
                        cam.backgroundColor = v;
                    }
                    else
                    {
                        
                    }
                    cam.cullingMask = (1 << 30) - 1;// 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Monster");
                    cam.nearClipPlane = 0.1f;
                    cam.farClipPlane = 100.0f;


                    Cubemap cube = new Cubemap(2048, TextureFormat.ARGB32, true);//256
                    
                    cam.RenderToCubemap(cube);
                    //cube.
                    RenderTexture tempSphereTexture = new RenderTexture(2048, 1024, 0,RenderTextureFormat.ARGB32);//%2
                    matCopy.SetTexture("CubeMap", cube);
                    if (SceneRenderSetting._Setting != null)
                    {
                        matCopy.SetTexture("SkyTexture", SceneRenderSetting._Setting.SkyTexture);
                    }
                    else
                    {
                        matCopy.SetTexture("SkyTexture", Texture2D.blackTexture);
                    }
                    Graphics.Blit(null, tempSphereTexture, matCopy, 1);
                    if(!Directory.Exists("Assets/$NGR/Cubemaps"))
                    {
                        Directory.CreateDirectory("Assets/$NGR/Cubemaps");
                    }
                    Texture2D save = new Texture2D(2048, 1024);
                    save.ReadPixels(new Rect(0, 0, save.width, save.height), 0, 0);
                    byte[] data = save.EncodeToJPG();
                    File.WriteAllBytes("Assets/$NGR/Cubemaps/" + temp.name + ".cubemap.jpg", data);
                    Object.DestroyImmediate(save);
                    Object.DestroyImmediate(cube);
                    RenderTexture.active = null;
                    Object.DestroyImmediate(tempSphereTexture);
                    Debug.Log("Gen Cubemap OK!" + temp.name);
                    
                }
                Object.DestroyImmediate(CameraObj);
            }
        }
        Object.DestroyImmediate(matCopy);
        AssetDatabase.Refresh();
    }
	
}
