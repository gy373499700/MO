using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.IO;

public class ExportSceneObject : UnityEditor.AssetModificationProcessor
{
    static public void OnWillSaveAssets(string[] names)
    {
        foreach (string str in names)
        {
            if (str.EndsWith(".unity"))
            {
                Scene scene = SceneManager.GetSceneByPath(str);

                Scene current_Scene = SceneManager.GetActiveScene();
                if (current_Scene == scene)
                {
                  /*  Object[] cull_objs = Resources.FindObjectsOfTypeAll(typeof(BoundingBoxCulling));
                    if(cull_objs!=null)
                    {
                        if(cull_objs.Length>0)
                        {
                            bool bSave = false;
                            if (EditorUtility.DisplayDialog("Message", " Do you want Recalc Scene Culling Data?it will take about 2~3 min", "OK", "Cancel"))
                            {
                                Debug.Log("Save Culling Data");
                                bSave = true;
                            }
                            if (bSave)
                            {

                                for (int i = 0; i < cull_objs.Length; i++)
                                {
                                   BoundingBoxCulling culling = cull_objs[i] as BoundingBoxCulling;
                                    if (culling != null)
                                    {
                                        culling.Save(str);
                                    }

                                }
                                AssetDatabase.Refresh();
                            }

                        }
                    }*/

                    Debug.Log("current scene "+scene.name + " save finished!");
                }
            }
        }
    }
    
    
}