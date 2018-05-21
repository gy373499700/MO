using UnityEngine;
using System.Collections;

public class BaseWnd : MonoBehaviour {

    public string PathName = "";
    public virtual void OnInit(ResLoadParams kParam)
    {//init
       
    }
   
    public virtual void OnShow(ResLoadParams kParam)
    {
       
    }
    public virtual void OnHide()
    {//隐藏后   一段时间不用  自动销毁

    }


}
