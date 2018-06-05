using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

[ExecuteInEditMode]
public class TGAConvert : MonoBehaviour
{
    [MenuItem("Assets/sdFileSystem/PNG Splite")]
    public static void SplitePNG()
    {

        //if(Selection.objects.Length > 2)
        //{
        //    Debug.LogError("must select 2 object");
        //}
        //if (Selection.objects.Length == 1)
        //{
        //    Debug.LogError("must select 2 object");
        //}
        Texture2D obj_src = null;
        Texture2D obj_alpha = null;
        for (int i = 0; i < Selection.objects.Length; i++)
        {
            UnityEngine.Object obj = Selection.objects[i];
            Debug.Log(obj.GetType());
            if (obj is Texture2D)
            {
                obj_src = obj as Texture2D;
                Color[] colors = obj_src.GetPixels();
                for (int j = 0; j < colors.Length; j++)
                {
                    colors[j].r = colors[j].a;
                    colors[j].g = colors[j].a;
                    colors[j].b = colors[j].a;
                    colors[j].a = 1;
                }

                string selectionPath = AssetDatabase.GetAssetPath(obj_src);

                Texture2D newTex = new Texture2D(obj_src.width, obj_src.height);
                newTex.SetPixels(colors);
                byte[] data = newTex.EncodeToPNG();
                File.WriteAllBytes(selectionPath.Replace(".PNG", "_Alpha.PNG").Replace(".png", "_Alpha.PNG"), data);
            }
        }

        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/sdFileSystem/PNG Combine")]
    public static void CombinePNG()
    {

        if (Selection.objects.Length > 2)
        {
            Debug.LogError("must select 2 object");
        }
        if (Selection.objects.Length == 1)
        {
            Debug.LogError("must select 2 object");
        }

        Texture2D obj_src = null;
        Texture2D obj_alpha = null;
        for (int i = 0; i < Selection.objects.Length; i++)
        {
            UnityEngine.Object obj = Selection.objects[i];
            
            if (obj is Texture2D)
            {
                Texture2D tex = obj as Texture2D;
                string path = AssetDatabase.GetAssetPath(tex);
                if (path.Contains("_Alpha.PNG"))
                {
                    obj_alpha = tex;
                }
                else
                {
                    obj_src = tex;
                }

            }
            else
            {
                Debug.LogError(obj.name + " is not a texture");
                return;
            }
        }
        if(obj_alpha.width != obj_src.width || obj_alpha.height != obj_src.height)
        {
            Debug.LogError("texture size is not match!\nwidth=" + obj_alpha.width + " " + obj_src.width +"\nheight=" + obj_alpha.height +" "+ obj_src.height);
            return;
        }
        string alpha_path = AssetDatabase.GetAssetPath(obj_alpha);
        TextureImporter ti = TextureImporter.GetAtPath(alpha_path) as TextureImporter;
        if (!ti.isReadable)
        {
            ti.isReadable = true;
            AssetDatabase.ImportAsset(alpha_path);
            obj_alpha = AssetDatabase.LoadAssetAtPath<Texture2D>(alpha_path);
        }

        Color[] colors_alpha =    obj_alpha.GetPixels();
        Color[] colors_src = obj_src.GetPixels();
        for(int i=0;i< colors_src.Length;i++)
        {
            colors_src[i].a = colors_alpha[i].r;
        }
        //obj_src.SetPixels(colors_src);

        string selectionPath = AssetDatabase.GetAssetPath(obj_src);
        Texture2D newTex = new Texture2D(obj_src.width, obj_src.height);
        newTex.SetPixels(colors_src);
        byte[] data = newTex.EncodeToPNG();
        File.WriteAllBytes(selectionPath, data);

        
        //byte[] data = obj_src.EncodeToPNG();
        //File.WriteAllBytes(selectionPath, data);

        AssetDatabase.ImportAsset(selectionPath);
    }

    void Update()
    {
        Camera[] cams = SceneView.GetAllSceneCameras();
        if(cams==null || cams.Length ==0)
        {
            return;
        }
        Camera sceneCamera = cams[0];
        for (int i = 0; i < Selection.objects.Length; i++)
        {

        }
    }
}
