using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System.IO;
public class NGUI_Sprite_Convert
{
    [MenuItem("Editor/NGUI_FaceExpression_Convert")]
    public static void Convert()
    {
        UnityEngine.Object[] objs = Selection.objects;
        for(int i=0;i< objs.Length;i++)
        {
            string path = AssetDatabase.GetAssetPath(objs[i]);
            TextAsset text = objs[i] as TextAsset;
            if(text==null)
            {
                continue;
            }
            Hashtable decodedHash =  NGUIJson.jsonDecode(text.text) as Hashtable;
            Hashtable frames = (Hashtable)decodedHash["frames"];

            string csv_content = "name,x,y,w,h\n";

            foreach (DictionaryEntry de in frames)
            {
                string[] keys = de.Key.ToString().Split('.');
                string key_name = keys[0];

                Hashtable sprite_table = (Hashtable)de.Value;
                Hashtable frame = (Hashtable)sprite_table["frame"];

                int frameX = int.Parse(frame["x"].ToString());
                int frameY = int.Parse(frame["y"].ToString());
                int frameW = int.Parse(frame["w"].ToString());
                int frameH = int.Parse(frame["h"].ToString());
                csv_content += key_name + "," + frameX + "," + frameY + "," + frameW + "," + frameH+"\n";
            }

            File.WriteAllText(path + ".csv", csv_content);
        }
    }
}