using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;


public class RotateKernelGen : MonoBehaviour {

    // Use this for initialization
    [MenuItem("Editor/RotateKernel4X4")]
    public static void Gen4x4 () {
       Color[] temp = new Color[16];
        Color half = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        for (int i=0;i<16;i++)
        {
            float angle =  ((float)i / 16.0f)*3.1415925f*2.0f;
            temp[i] = new Color(Mathf.Cos(angle), -Mathf.Sin(angle), Mathf.Sin(angle), Mathf.Cos(angle));
            temp[i] *= 0.5f;
            temp[i] += half;
        }
        Color[] c = new Color[16];

        c[0] = temp[0];  c[1] = temp[4];  c[2] = temp[1];   c[3] = temp[5];
        c[4] = temp[12]; c[5] = temp[8];  c[6] = temp[13];   c[7] = temp[9];
        c[8] = temp[3];  c[9] = temp[7];  c[10] = temp[2];  c[11] = temp[6];
        c[12] = temp[15]; c[13] = temp[11]; c[14] = temp[14]; c[15] = temp[10];

        Texture2D tex = new Texture2D(4, 4, TextureFormat.ARGB32, false);
        tex.SetPixels(c);
        byte[] data = tex.EncodeToPNG();
        File.WriteAllBytes("Assets/rotate4x4.png",data);
        //Resources.UnloadAsset(tex);
        AssetDatabase.ImportAsset("Assets/rotate4x4.png");
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
