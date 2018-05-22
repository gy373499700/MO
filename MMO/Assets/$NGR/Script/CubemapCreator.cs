using UnityEngine;
using System.Collections;
using System.IO;

public class CubemapCreator : MonoBehaviour {

    public static void Create(Vector3 worldPos)
    {
        GameObject obj = new GameObject("__CubemapCreator");
        CubemapCreator cc = obj.AddComponent<CubemapCreator>();
        cc.worldPos = worldPos;
    }

    Vector3 worldPos = Vector3.zero;
	void Start () {
        StartCoroutine(Excute());
    }

    IEnumerator Excute()
    {
#if UNITY_EDITOR
        if (RenderPipeline._instance == null)
        {
            Debug.LogError("RenderPipeline Instance Not Found.");
            yield break; ;
        }

        if (!Directory.Exists("Assets/$NGR/Cubemaps/Temp"))
        {
            Directory.CreateDirectory("Assets/$NGR/Cubemaps/Temp");
        }

        int size = 1024;
        Material matOrigin = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/$NGR/Materials/matGenCubemap.mat");
        Material matCopy = new Material(matOrigin);
        GameObject.FindObjectOfType<UICamera>().GetComponent<Camera>().enabled = false;
        Camera mainCam = RenderPipeline._instance.GetComponent<Camera>();
        float originFov = mainCam.fieldOfView;
        Vector3 originPos = RenderPipeline._instance.transform.position;
        Quaternion originRot = RenderPipeline._instance.transform.rotation;

        RenderPipeline._instance.transform.position = worldPos;
        mainCam.fieldOfView = 90f;
        SceneRenderSetting._Setting.CameraFOV = 90f;
        Time.timeScale = 0;
        yield return null;
        Vector3[] dir = new Vector3[6];
        dir[0] = new Vector3(0, 0, 1);
        dir[1] = new Vector3(1, 0, 0);
        dir[2] = new Vector3(0, 0, -1);
        dir[3] = new Vector3(-1, 0, 0);
        dir[4] = new Vector3(0, 1, 0);
        dir[5] = new Vector3(0, -1, 0);
        Texture2D[] cache = new Texture2D[6];
        
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

            yield return null;
            yield return null;
            yield return null;
            yield return new WaitForEndOfFrame();
            cache[i] = new Texture2D(size, size, TextureFormat.ARGB32, false);
            cache[i].ReadPixels(new Rect(0, 0, cache[i].width, cache[i].height), 0, 0);
            byte[] d = cache[i].EncodeToJPG();
            File.WriteAllBytes("Assets/$NGR/Cubemaps/Temp/" + i.ToString() + ".jpg", d);
        }
        RenderTexture sphere_tex = new RenderTexture(size * 2, size, 0, RenderTextureFormat.ARGB32);

        Cubemap cube = new Cubemap(size, TextureFormat.ARGB32, false);
        cube.filterMode = FilterMode.Point;
        Texture2D t = new Texture2D(size, size, TextureFormat.ARGB32, false);
        //3,1,4,5,0,2
        cube.SetPixels(cache[3].GetPixels(), CubemapFace.PositiveX);
        cube.SetPixels(cache[1].GetPixels(), CubemapFace.NegativeX);
        cube.SetPixels(cache[4].GetPixels(), CubemapFace.PositiveY);
        cube.SetPixels(cache[5].GetPixels(), CubemapFace.NegativeY);
        cube.SetPixels(cache[0].GetPixels(), CubemapFace.PositiveZ);
        cube.SetPixels(cache[2].GetPixels(), CubemapFace.NegativeZ);
        cube.Apply();
        matCopy.SetTexture("CubeMap", cube);
        Graphics.Blit(null, sphere_tex, matCopy, 1);

        Texture2D save = new Texture2D(size * 2, size);
        save.ReadPixels(new Rect(0, 0, save.width, save.height), 0, 0);

        byte[] data = save.EncodeToJPG();
        File.WriteAllBytes("Assets/$NGR/Cubemaps/Temp/" + "globalSceneall" + ".cubemap.jpg", data);

        GameObject.FindObjectOfType<UICamera>().GetComponent<Camera>().enabled = true;
        mainCam.fieldOfView = originFov;
        RenderPipeline._instance.transform.position = originPos;
        RenderPipeline._instance.transform.rotation = originRot;
        RenderTexture.active = null;
        Object.DestroyImmediate(save);
        Object.DestroyImmediate(cube);
        for (int i = 0, iMax = cache.Length; i < iMax; i++)
        {
            if (cache[i] != null)
            {
                Object.DestroyImmediate(cache[i]);
                cache[i] = null;
            }
        }
        cache = null;
        sphere_tex.Release();
        sphere_tex = null;
        UnityEditor.AssetDatabase.Refresh();
        Time.timeScale = 1;
        MonoBehaviour.DestroyImmediate(this.gameObject);
        /*RenderPipeline._instance.transform.forward = Vector3.forward;
        Texture2D save = new Texture2D(RenderPipeline._instance.GetCurrentWidth(), RenderPipeline._instance.GetCurrentHeight());
        yield return new WaitForEndOfFrame();
        save.ReadPixels(new Rect(0, 0, save.width, save.height), 0, 0);
        byte[] data = save.EncodeToPNG();
        File.WriteAllBytes("Assets/$NGR/Cubemaps/Temp/forward.png", data);

        RenderPipeline._instance.transform.forward = Vector3.back;
        yield return new WaitForEndOfFrame();
        save.ReadPixels(new Rect(0, 0, save.width, save.height), 0, 0);
        data = save.EncodeToPNG();
        File.WriteAllBytes("Assets/$NGR/Cubemaps/Temp/back.png", data);

        RenderPipeline._instance.transform.forward = Vector3.left;
        yield return new WaitForEndOfFrame();
        save.ReadPixels(new Rect(0, 0, save.width, save.height), 0, 0);
        data = save.EncodeToPNG();
        File.WriteAllBytes("Assets/$NGR/Cubemaps/Temp/left.png", data);
        

        RenderPipeline._instance.transform.forward = Vector3.right;
        yield return new WaitForEndOfFrame();
        save.ReadPixels(new Rect(0, 0, save.width, save.height), 0, 0);
        data = save.EncodeToPNG();
        File.WriteAllBytes("Assets/$NGR/Cubemaps/Temp/right.png", data);
        

        RenderPipeline._instance.transform.forward = Vector3.up;
        yield return new WaitForEndOfFrame();
        save.ReadPixels(new Rect(0, 0, save.width, save.height), 0, 0);
        data = save.EncodeToPNG();
        File.WriteAllBytes("Assets/$NGR/Cubemaps/Temp/up.png", data);
        

        RenderPipeline._instance.transform.forward = Vector3.down;
        yield return new WaitForEndOfFrame();
        save.ReadPixels(new Rect(0, 0, save.width, save.height), 0, 0);
        data = save.EncodeToPNG();
        File.WriteAllBytes("Assets/$NGR/Cubemaps/Temp/down.png", data);
        yield return new WaitForEndOfFrame();

        MonoBehaviour.DestroyImmediate(save);
        MonoBehaviour.DestroyImmediate(this.gameObject);
        uicam.gameObject.GetComponent<Camera>().enabled = true;*/
#endif
        yield break;
    }
}
