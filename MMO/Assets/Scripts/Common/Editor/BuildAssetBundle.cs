using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Text;
using UnityEditor.Callbacks;
/// <summary>
/// 1.所有需要打包到AssetBundle的文件请加$标识
/// 2.多个文件需要打包到同一个Bundle的资源，请放在同一个目录，并且该目录最后一个文件夹包含$
/// 3.建议多个Prefab，多个materials,多个贴图。多个Shader分别放在一个$文件夹内，各自打包成一个bundle，每个bundle包要控制大小，一般不要超过1-3M
/// 4.根目录不要放资源,保证场景相关的资源放在Level下同一个目录，游戏中用到的元素不能和场景元素一样，否则资源会冗余2份。
/// 5.设置不卸载的资源建议单独打包到一个bundle，防止冗余，如果是图集那么程序特殊过滤。需要一个个资源的过。
/// 6.UI建议每个窗口单独打成一个bundle，只有那种零散共存的可以多个打成一个bundle。
/// 7.需要卸载的资源必须单个资源打成一个bundle，然后保证不会被任何资源和场景引用。
/// by gardonguo
/// </summary>
public class BundleItem
{
    public string bundleName;//Level/$ww.unity.unity3d
    public string localPath;//"Assets/Level/$ww.unity"
    public bool isScene;

}

public class BuildAssetBundle : MonoBehaviour {

    #region BuildBundle
    [MenuItem("BundleManager/Test/ClearAssetBundlesNameForTest")]
    static void ClearAssetBundlesName()
    {
        int length = AssetDatabase.GetAllAssetBundleNames().Length;

        string[] oldAssetBundleNames = new string[length];
        for (int i = 0; i < length; ++i)
        {
            oldAssetBundleNames[i] = AssetDatabase.GetAllAssetBundleNames()[i];
        }

        for (int i = 0; i < length; ++i)
        {
            if (oldAssetBundleNames[i] == "snakeskin.unity3d") continue;
            AssetDatabase.RemoveAssetBundleName(oldAssetBundleNames[i], true);
        }

        Debug.Log("ClearAssetBundlesName Finished !"+AssetDatabase.GetAllAssetBundleNames().Length);
    }
    [MenuItem("BundleManager/Tools/BuildAndroidBundle")]
    public static void BuildAndroidBundle()
    {
        BuildBundle(BuildTarget.Android);
    }
    [MenuItem("BundleManager/Tools/BuildPCBundle")]
    public static void BuildPCBundle()
    {
        BuildBundle(BuildTarget.StandaloneWindows);
    }
    public static void BuildBundle(BuildTarget platform)
    {

        string outputPath = Application.dataPath.Replace("Assets","") + "Bundles/";
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }
        Directory.CreateDirectory(outputPath);
        SetAllAssetBundleTag();
      
        AssetBundleManifest minifest= BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.ChunkBasedCompression, platform);
        BuildDependcy(minifest);//bundle 打包完再生成所有依赖文件
        AssetDatabase.Refresh();

        Debug.Log("Build bundle Success To: " + outputPath);

    }

    static void SetAllAssetBundleTag()
    {
        string[] dirs = Directory.GetDirectories(Application.dataPath);

        List<BundleItem> bundles = new List<BundleItem>();
        foreach (string dir in dirs)
        {
            string filePath = dir.Replace("\\", "/");
            int index = filePath.LastIndexOf("/");
            filePath = filePath.Substring(index + 1);
            //Debug.Log("filePath------>" + filePath);
            string localPath = "";
            if (index > 0)
                localPath = filePath;
            PushXML(localPath, bundles);

        }
        int bundleCount = 0;
        Debug.Log("all bundle count " + bundles.Count);
        for (int i = 0; i < bundles.Count; i++)
        {
         //   Debug.Log(bundles[i].bundleName + "   " + bundles[i].localPath);
            AssetImporter importer = AssetImporter.GetAtPath(bundles[i].localPath);
            if (importer != null)
            {
                bundleCount++;
                if (importer.assetBundleName != bundles[i].bundleName)
                    importer.assetBundleName = bundles[i].bundleName;
                if (importer.assetBundleVariant != "")
                    importer.assetBundleVariant = "";
            }
            else if (bundles[i].localPath.EndsWith("/"))
            {//fold  文件夹bundle对应多个文件夹
                SetFolderBundle( bundles[i].localPath, bundles[i].bundleName);
            }
            else
            {
                Debug.LogError(bundles[i].localPath);
            }
        }
       
        AssetDatabase.SaveAssets();

    }
    static void ClearAssetBundleTag(string path)
    {
        AssetImporter importer = AssetImporter.GetAtPath(path);
        if (importer != null)
        {
            if (importer.assetBundleName != "")
                importer.assetBundleName = "";
            if (importer.assetBundleVariant != "")
                importer.assetBundleVariant = "";
        }else
        {
            Debug.LogError(path);
        }
    }
    static void PushXML(string path, List<BundleItem> objs)
    {
        path = path.Replace("Assets/", "");
        int flagId = path.LastIndexOf("$");

        if (path.Contains("Resources") || path.Contains(".svn") || path.Contains("StreamingAssets"))
        {
            return;
        }
        if (flagId >= 0)
        {
            BundleItem item = new BundleItem();
            item.bundleName = path + ".unity3d";
            item.localPath = "Assets/" + path+"/";
            item.isScene = false;
            objs.Add(item);
        }
        else
        {
            string[] fileEntries = Directory.GetFiles(Application.dataPath + "/" + path);

            foreach (string fileName in fileEntries)
            {
                string filePath = fileName.Replace("\\", "/");
                int index = filePath.LastIndexOf("/");
                filePath = filePath.Substring(index);
                int fileFlagId = filePath.LastIndexOf("$");
                string localPath = "Assets/" + path;
                if (index > 0)
                    localPath += filePath;
                if (fileFlagId >= 0)
                {
                    int dotIndex = filePath.LastIndexOf(".");
                    string ext = filePath.Substring(dotIndex);
                    if (ext == ".meta")
                        continue;

                    BundleItem item = new BundleItem();
                    item.localPath = localPath;

                    if (ext == ".unity")
                    {
                        item.isScene = true;
                    }
                    else
                        item.isScene = false;
                    item.bundleName = path + filePath + ".unity3d";
                    objs.Add(item);
                }
                else
                {
                    if (filePath.EndsWith(".cs") || filePath.EndsWith(".meta"))
                    {

                    }
                    else
                    {
                        ClearAssetBundleTag(localPath);
                    }
                }
            }


            string[] dirs = Directory.GetDirectories(Application.dataPath + "/" + path);
            foreach (string dir in dirs)
            {
                string filePath = dir.Replace("\\", "/");
                int index = filePath.LastIndexOf("/");
                filePath = filePath.Substring(index);
                string localPath = "Assets/" + path;
                if (index > 0)
                    localPath += filePath;
                PushXML(localPath, objs);
            }
        }
    }

    static void SetFolderBundle(string path, string bundleName)
    {
        path = path.Replace("Assets/", "");
        string[] fileEntries = Directory.GetFiles(Application.dataPath + "/"+path);//相对或绝对路径
        foreach (string fileName in fileEntries)
        {
            string filePath = fileName.Replace("\\", "/");

            string localPath = filePath.Replace(Application.dataPath, "Assets");

            int dotIndex = filePath.LastIndexOf(".");
            string ext = filePath.Substring(dotIndex);
            if (ext == ".meta"|| ext == ".cs")
                continue;
            AssetImporter importer = AssetImporter.GetAtPath(localPath);//这个路径只能是相对路径
            if (importer != null)
            {
                if (importer.assetBundleName != bundleName)
                    importer.assetBundleName = bundleName;
                if (importer.assetBundleVariant != "")
                    importer.assetBundleVariant = "";
            }
            else
            {
                Debug.LogError(localPath);
            }
        }
        string[] dirs = Directory.GetDirectories(Application.dataPath + "/"+path);
        foreach (string dir in dirs)
        {
            string filePath = dir.Replace("\\", "/");
            string localPath = filePath.Replace(Application.dataPath, "Assets");
            SetFolderBundle(localPath, bundleName);
        }
    }
    static void CheckBundleMaxSize()
    {
        int size = 5;//mb
    }
    #endregion

    #region BuildDependcy
    static void BuildDependcy(AssetBundleManifest minifest)
    {
        List<FileInfo> lstRemoteFile = new List<FileInfo>();
        string outputPath = Application.dataPath.Replace("Assets", "") + "Bundles/";
        string path = Application.dataPath + "/";
        List<string> lst = new List<string>();//"..$NGR/Shader/BRDF.cg"
        GetAllFiles(path, lst, "$");
        Dictionary<string, int> Namelst = new Dictionary<string, int>();
        Dictionary<string, uint> bundleCrc = new Dictionary<string, uint>();
        for (int j = 0; j < lst.Count; j++)
        {
            lst[j] = lst[j].Replace(Application.dataPath + "/", "");
            FileInfo file = new FileInfo();
            file.pathName = lst[j].ToLower();
            file.bundleName = BundleManager.GetBundleNameFromPath(file.pathName);
            string crcpath = outputPath + file.bundleName;
            if (!bundleCrc.ContainsKey(crcpath))
                 bundleCrc[crcpath] = BundleManager.CalcCRCDataPath(outputPath + file.bundleName);
            file.bundleCrc = bundleCrc[crcpath];
            file.length= BundleManager.CalcDataPathLength(Application.dataPath+"/" + file.pathName);
            lstRemoteFile.Add(file);
            Namelst[file.pathName] = j;
        }
        for (int j = 0; j < lst.Count; j++)
        {
            FileInfo file = lstRemoteFile[j];
            string[] depend = BuildDepengcyItem(file.pathName);
            if (depend.Length > 0)
            {
                int[] depengid = new int[depend.Length];
                for (int i = 0; i < depend.Length; i++)
                {
  
                    if (depend[i] != file.pathName && Namelst.ContainsKey(depend[i]))
                        depengid[i] = (Namelst[depend[i]]);
                    else
                        depengid[i] = -1;
                }
                file.dependcyid = depengid;
            }
  
        }
        FileInfo.SaveFileInfo(outputPath + "fileinfo.txt", lstRemoteFile);
    }
    public static string[] BuildDepengcyItem(string name)
    {
        string[] pathname = { "Assets/" + name };

        string[] dependencies = AssetDatabase.GetDependencies(pathname);
        int[] dependcy = new int[dependencies.Length];
        for (int i = 0; i < dependencies.Length; i++)
        {
            dependencies[i] = dependencies[i].Replace("Assets/", "").ToLower();
        }
        return dependencies;//name
    }

    static void GetAllFiles(string path, List<string> allfiles, string DirectoryKey)
    {

        string[] files = Directory.GetFiles(path);//获取目录 不包含子目录文件
        foreach (string f in files)
        {
            string temp = f.Replace("\\", "/");
            if (temp.EndsWith(".meta"))
            {
                continue;
            }
            if (temp.EndsWith(".cs"))
            {
                continue;
            }
            if (DirectoryKey != null)
            {
                if (temp.Contains(DirectoryKey))
                {
                    allfiles.Add(temp);
                }
            }
            else
            {
                allfiles.Add(temp);
            }
        }
        string[] dirs = Directory.GetDirectories(path);//所有子目录
        foreach (string d in dirs)
        {
            if (d.EndsWith(".svn"))
            {
                continue;
            }
            if (DirectoryKey != null)
            {
                if (d.Contains(DirectoryKey))
                {
                    GetAllFiles(d, allfiles, null);
                }
                else
                {
                    GetAllFiles(d, allfiles, DirectoryKey);
                }
            }
            else
            {
                GetAllFiles(d, allfiles, null);
            }

        }

    }

    #endregion
    #region Version
    [MenuItem("BundleManager/Mode/EditiorMode")]
    public static void EditiorMode()
    {
        string fileSystemTxt = Application.streamingAssetsPath + "/AppVersion.txt";
        StreamReader reader = new StreamReader(Application.streamingAssetsPath + "/AppVersion.txt", System.Text.Encoding.UTF8);
        string originLine, outputStr = "";
        string editorMode = "BundleTest=0";
       // string needUpdate = "  EditorNeedUpdate:0";
        string EditorUpdatePath = "BundlePath=";

        while ((originLine = reader.ReadLine()) != null)
        {
            if (originLine.Contains("BundleTest"))
            {
                originLine = editorMode;
            }
            if (originLine.Contains("BundlePath"))
            {
                originLine = EditorUpdatePath + Application.dataPath.Replace("Assets", "") + "Bundles/";
            }
            outputStr += originLine + "\r\n";
        }
        reader.Close();
        System.IO.File.WriteAllText(fileSystemTxt, string.Empty);
        FileStream steam = new FileStream(fileSystemTxt, FileMode.Open);
        byte[] data = System.Text.Encoding.UTF8.GetBytes(outputStr);
        steam.Write(data, 0, data.Length);
        steam.Flush();
        steam.Close();
    }

    [MenuItem("BundleManager/Mode/BundleMode")]
    public static void BundleMode()
    {
        string fileSystemTxt = Application.streamingAssetsPath + "/AppVersion.txt";
        StreamReader reader = new StreamReader(Application.streamingAssetsPath + "/AppVersion.txt", System.Text.Encoding.UTF8);
        string originLine, outputStr = "";
        string editorMode = "BundleTest=1";
        // string needUpdate = "  EditorNeedUpdate:0";
        string EditorUpdatePath = "BundlePath=";

        while ((originLine = reader.ReadLine()) != null)
        {
            if (originLine.Contains("BundleTest"))
            {
                originLine = editorMode;
            }
            if (originLine.Contains("BundlePath"))
            {
                originLine = EditorUpdatePath + Application.dataPath.Replace("Assets", "") + "Bundles/";
            }
            outputStr += originLine + "\r\n";
        }
        reader.Close();
        System.IO.File.WriteAllText(fileSystemTxt, string.Empty);
        FileStream steam = new FileStream(fileSystemTxt, FileMode.Open);
        byte[] data = System.Text.Encoding.UTF8.GetBytes(outputStr);
        steam.Write(data, 0, data.Length);
        steam.Flush();
        steam.Close();
    }

    #endregion


    #region BuildPlayer

    [MenuItem("BundleManager/Player/BuildPCPlayer")]
    public static void BuildPCPlayer()
    {
        string[] levels = {
            "Assets/Level/login0.unity",
		};
        string output_path = Application.dataPath + "/../bundle-apk/Fighter.exe" ;
        BuildPipeline.BuildPlayer(levels, output_path, BuildTarget.StandaloneWindows, BuildOptions.None);
        Debug.Log("build player succeed and wait for copy res folder!");
        string data = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/AppVersion.txt");
        VersionItem Version = new VersionItem();
        Version.LoadVersion(data);
        DeleteFiles(output_path.Replace("Fighter.exe", "Fighter_Data/Bundles/"));
        CopyFolder(Version.BundlePath, output_path.Replace("Fighter.exe", "Fighter_Data/Bundles/"));
        Debug.Log("Build OK!");
    }

    #endregion


    public static void CopyFolder(string direcSource, string direcTarget)
    {
        if (!Directory.Exists(direcTarget))
            Directory.CreateDirectory(direcTarget);

        DirectoryInfo direcInfo = new DirectoryInfo(direcSource);
        // Files
        System.IO.FileInfo[] files = direcInfo.GetFiles();
        foreach (System.IO.FileInfo file in files)
        {
            if (!file.Name.ToLower().EndsWith(".meta"))
            {
                file.CopyTo(Path.Combine(direcTarget, file.Name).Replace('\\', '/'), true);
            }
        }
        // Sub Directory
        DirectoryInfo[] direcInfoArr = direcInfo.GetDirectories();
        foreach (DirectoryInfo dir in direcInfoArr)
            CopyFolder(Path.Combine(direcSource, dir.Name).Replace('\\', '/'), Path.Combine(direcTarget, dir.Name).Replace('\\', '/'));
    }

    public static void DeleteFiles(string directory)
    {
        if (!Directory.Exists(directory)) return;
        foreach (string d in Directory.GetFileSystemEntries(directory))
        {
            if (File.Exists(d))
            {
                try
                {
                    File.Delete(d);
                }
                catch (System.Exception ex)
                {
                    Debug.Log("DeleteStreamingAssetFiles " + ex);
                }

            }
            else
            {
                try
                {
                    DirectoryInfo di = new DirectoryInfo(d);
                    if (di.GetFiles().Length != 0)
                    {
                        DeleteFiles(di.FullName);
                    }
                    Directory.Delete(d);
                }
                catch (System.Exception ex)
                {
                    Debug.Log("DeleteStreamingAssetFiles " + ex);
                }

            }

        }
    }

}
