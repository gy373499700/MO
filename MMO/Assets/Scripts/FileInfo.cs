using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


public class TFileInfo
{
    public string pathName;
    public string bundleName;
    public uint bundleCrc;
    public int[] dependcyid;//不包括自己的所有依赖
    public int _ref = 0;//大于0 表示有引用 自己也要计数
    public int length = 0;//每个文件的大小 只能改成page模式才能取
    public static List<TFileInfo> AllFile = new List<TFileInfo>();//allfile
    public static Dictionary<string, int> FileName = new Dictionary<string, int>();//filename,allfile.index
    public static int Size=0;
    #region static function
    public static void LoadFileInfo(string content, Dictionary<string, BundleGlobalItem> AllLoadedBundle)
    {
        FileName.Clear();
        AllFile.Clear();
        if (content == null) {
			return;
		}
        string[] lines = content.Split('\n');//youhua
        if (lines.Length == 0)
            UnityEngine.Debug.Log("lines.Length == 0");
        for (int i = 0; i < lines.Length; i++)
        {
            if (i == 0) continue;
            string s = lines[i];
            if (s.Length == 0)
            {
                UnityEngine.Debug.Log("s.Length == 0");
                continue;                
            }
            string[] eles = s.Split(',');
            TFileInfo info = new TFileInfo();

            
            info.pathName = eles[0];
            info.bundleName = eles[1];
            info.bundleCrc = uint.Parse(eles[2]);
            info.length = int.Parse(eles[3]);
            info.dependcyid = StringToArray(eles[4]);
            FileName[info.pathName] = i-1;//because 0 is description
            AllFile.Add(info);
            if (AllLoadedBundle.ContainsKey(info.bundleName))
            {
                AllLoadedBundle[info.bundleName].selfileLst.Add(info);
            }
            else
            {
                UnityEngine.Debug.Log("RelativeFileToBundle key " + info.bundleName);
            }
        }
    }
    public static void LoadFileInfoFromStreamAssets(string content, Dictionary<string, BundleGlobalItem> AllLoadedBundle)
    {
        FileName.Clear();
        AllFile.Clear();
        if (content == null)
        {
            return;
        }
        string[] lines = content.Split('\n');//youhua
        if (lines.Length == 0)
            UnityEngine.Debug.Log("lines.Length == 0");
        for (int i = 0; i < lines.Length; i++)
        {
            if (i == 0) continue;
            string s = lines[i];
            if (s.Length == 0)
            {
                UnityEngine.Debug.Log("s.Length == 0");
                continue;
            }
            string[] eles = s.Split(',');
            TFileInfo info = new TFileInfo();


            info.pathName = eles[0];
            info.bundleName = eles[1];
            info.bundleCrc = uint.Parse(eles[2]);
            info.length = int.Parse(eles[3]);
            info.dependcyid = StringToArray(eles[4]);
            FileName[info.pathName] = i - 1;//because 0 is description
            AllFile.Add(info);
            if (AllLoadedBundle.ContainsKey(info.bundleName))
            {
                AllLoadedBundle[info.bundleName].selfileLst.Add(info);
            }
            else
            {
                UnityEngine.Debug.Log("RelativeFileToBundle key " + info.bundleName);
            }
        }
    }
    public static void SaveFileInfo(string path, List<TFileInfo> lstRemoteFile)
    {
        FileStream file = new FileStream(path, FileMode.Create);
        if (file != null)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("pathName,bundleName,bundleCrc,fileLength,DependId\n");
            for (int i = 0; i < lstRemoteFile.Count; i++)
            {
                sb.Append(lstRemoteFile[i].pathName);
                sb.Append(",");
                sb.Append(lstRemoteFile[i].bundleName);
                sb.Append(",");
                sb.Append(lstRemoteFile[i].bundleCrc);
                sb.Append(",");
                sb.Append(lstRemoteFile[i].length);
                sb.Append(",");
                for (int j=0;j< lstRemoteFile[i].dependcyid.Length; j++)
                {
                    if (lstRemoteFile[i].dependcyid[j] != -1)
                    {
                        sb.Append(lstRemoteFile[i].dependcyid[j]);
                        sb.Append(";");
                    }
                }
                sb.Append("\n");
            }
            byte[] data = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            file.Write(data, 0, data.Length);
            file.Close();
        }
    }

    public static int[] StringToArray(string s)
    {
        List<int> a = null;
        if (s == null) return null;
        if (s.Length == 0) return null;
        string[] arr = s.Split(';');
      
        if (arr.Length >= 1)
        {
            a = new List<int>();//[arr.Length];
            int index = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i].Length > 0)
                {
                    int id = int.Parse(arr[i]);
                    a.Add(id);
                    index++;
                }
            }
        }
        return a.ToArray();
    }
    public static string AraryToString(int[] arr)
    {
        StringBuilder sb = new StringBuilder();
        if (arr != null)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] > 0)
                {
                    sb.Append(arr[i]);
                    sb.Append(";");
                }
            }
        }
        return sb.ToString();
    }
    public static TFileInfo GetFile(string pathname)
    {
        if (FileName.ContainsKey(pathname))
            return AllFile[FileName[pathname]];
        else
        {
            UnityEngine.Debug.LogError("get file failed !" + pathname);
            return null;
        }
    }
    #endregion

    public TFileInfo[] GetFilesDependcy()
    {
        if (dependcyid != null && dependcyid.Length > 0)
        {
            TFileInfo[] dependcy = new TFileInfo[dependcyid.Length];
            for (int i = 0; i < dependcyid.Length; i++)
            {
                TFileInfo file = AllFile[dependcyid[i]];
                dependcy[i] = file;
            }
            return dependcy;
        }
        return null;
    }
  /*  public string[] GetFileDepency()
    {
        if (dependcyid != null && dependcyid.Length > 0)
        {
            string[] dependcy = new string[dependcyid.Length];
            for (int i = 0; i < dependcyid.Length; i++)
            {
                TFileInfo file = AllFile[dependcyid[i]];
                dependcy[i] = file.pathName;
            }
            return dependcy;
        }
        string[] a = new string[0];
        return a;
    }
        */

    public List<string> GetBundleDependcy()
    {//获取该文件的所有依赖bundle
        List<string> lst = new List<string>();
        if (dependcyid!=null&& dependcyid.Length > 0)
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();
            for(int i=0;i< dependcyid.Length; i++)
            {
                TFileInfo file = AllFile[dependcyid[i]];
                dic[file.bundleName] = 1;//多个file对应一个bundle
            }
            foreach(KeyValuePair<string,int> kv in dic)
            {
                lst.Add(kv.Key);
            }
            return lst;
        }
        return lst;
    }

    public void OnCreate(bool IsScene)
    {//当一个文件被创建的时候，建立其依赖关系
        if (dependcyid != null && dependcyid.Length > 0)
        {
            for (int i = 0; i < dependcyid.Length; i++)
            {
                TFileInfo file = AllFile[dependcyid[i]];
                file.AddRef();
                Size += file.length;
            }
            AddRef();
            Size += length;
        }
    }
    public void OnDestroy(bool IsScene)
    {//当一个文件被销毁时
        if (dependcyid != null && dependcyid.Length > 0)
        {
            for (int i = 0; i < dependcyid.Length; i++)
            {
                TFileInfo file = AllFile[dependcyid[i]];
                Size -= file.length;
                if (IsScene) 
                    file.ClearRef();
                else
                    file.SubRef();
            }
            ClearRef();
            Size -= length;
        }
    }
     int AddRef()
    {
        int ret =  _ref++;
        return ret;
    }

     void SubRef()
    {
         _ref --;
        if (_ref < 0)
        {
            _ref = 0;
#if UNITY_EDITOR
            UnityEngine.Debug.LogError("SubRef error " + pathName);
#endif
        }
    }
    void ClearRef()
    {
        _ref = 0;
    }
    public bool IsFileFree()
    {
        if (_ref > 0)
            return false;
        else
            return true;
    }
    public static void DebugFileRef()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("/////////////////DebugFileRef///////////////////////\n");
        int Size = 0;
        for (int i = 0; i < AllFile.Count; i++)
        {
            if (AllFile[i]._ref > 0)
            {
                Size += AllFile[i].length;
            }
        }
        sb.Append("Memmory Size   "+ Size + "   Calculate Size  " + Size + "B  and about " + (Size / 1024 / 1024) + "M\n");
        for (int i=0;i< AllFile.Count; i++)
        {
            if (AllFile[i]._ref > 0)
            {
                sb.Append(AllFile[i].bundleName);
                sb.Append("                 ");
                sb.Append(AllFile[i].pathName);
                sb.Append("                 ");
                sb.Append(AllFile[i]._ref);
                sb.Append("\n");
            }
        }
        UnityEngine.Debug.Log(sb.ToString());
    }
  
}

