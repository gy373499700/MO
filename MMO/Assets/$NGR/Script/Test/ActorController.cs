using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchInfo
{
    public Vector2 position;
    public Vector2 delta;
    public TouchPhase phase;
    public int fingerId;
}
public class ActorController : MonoBehaviour {
    string debug_string="";
    List<TouchInfo> lstTouch = new List<TouchInfo>();
    Vector2 lastpos = Vector2.zero;
    Vector2 lastdir = Vector2.zero;
    Vector3 saveDirection = Vector3.forward;
    int movefinger = -1;
    int rotatefinger = -1;
    float xrotate = 0;
    float yrotate = 0;
    public Transform ControlActor = null;
    public Transform XRotate = null;
    public Transform CameraDis = null;
    public float movespeed = 5.25f;
    public float rotate_velocity = 1.0f;
    public string idle_anim_name = "warrior_idle_01";
    public string run_anim_name = "warrior_run_01";
    // Use this for initialization
    void Start () {
       
	}
	
	// Update is called once per frame
	void Update () {
        UpdateFinger();
        Vector3 forward = SceneRenderSetting._Setting.transform.TransformDirection(Vector3.forward);
        Vector3 right = SceneRenderSetting._Setting.transform.TransformDirection(Vector3.right);

        //lastdir = lastdir * 2 - Vector2.one;
        Vector3 dir = forward * lastdir.y + right * lastdir.x;
        dir.y = 0;
        dir.Normalize();
        transform.position += dir * movespeed * Time.deltaTime;
        ControlActor.position = transform.position;

        if (dir.magnitude > 0.001f)
        {
            saveDirection   =   dir.normalized;
        }
        ControlActor.rotation = Quaternion.FromToRotation(Vector3.forward, saveDirection);

        Animation anim = ControlActor.GetComponent<Animation>();
        if (dir.magnitude > 0)
        {
            if (!anim.IsPlaying(run_anim_name))
            {
                anim.Play(run_anim_name, PlayMode.StopAll);
            }
        }
        else
        {
            if (!anim.IsPlaying(idle_anim_name))
            {
                anim.Play(idle_anim_name, PlayMode.StopAll);
            }
        }
	}

    void UpdateFinger()
    {
        if (Input.touchCount > 0)
        {
            Touch[] ts = Input.touches;
            for (int i = 0; i < ts.Length; i++)
            {
                Touch t = ts[i];
                if (t.phase == TouchPhase.Stationary)
                {
                    _OnFingerStay(t);
                }
                else if (t.phase == TouchPhase.Began)
                {
                    _OnFingerBegin(t);
                }
                else if (t.phase == TouchPhase.Moved)
                {
                    _OnFingerMove(t);
                }
                else if (t.phase == TouchPhase.Ended)
                {
                    _OnFingerEnd(t);
                }
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            Remove(0);
            TouchInfo info = AddMouseTouch(Input.mousePosition);
            OnFingerDown(info);
        }
        if (Input.GetMouseButtonUp(0))
        {
//            Debug.Log("0 up");
            TouchInfo info = Find(0);

            OnFingerUp(info);
        }
        if (Input.GetMouseButton(0))
        {
//            Debug.Log("0 move");
            TouchInfo info = Find(0);
            Vector2 mouse = new Vector2(Input.mousePosition.x,Input.mousePosition.y);
            if (Vector2.Distance(mouse, info.position) > 0.1f)
            {
                info.delta = mouse-info.position;
                info.position = mouse;
                if (info.phase == TouchPhase.Began || info.phase == TouchPhase.Stationary)
                {
                    info.phase = TouchPhase.Moved;
                    OnFingerBeginMove(info);
                }
                else if (info.phase == TouchPhase.Moved)
                {
                    OnFingerMove(info);
                }
            }
            else
            {
                info.delta = Vector2.zero;
                info.position = mouse;
                if (info.phase == TouchPhase.Moved)
                {
                    info.phase = TouchPhase.Stationary;
                    OnFingerEndMove(info);
                }
            }
        }
    }
    
    TouchInfo Find(Touch t)
    {
        for (int i = 0; i < lstTouch.Count; i++)
        {
            if (lstTouch[i].fingerId == t.fingerId)
            {
                return lstTouch[i];
            }
        }
        return null;
    }
    TouchInfo Find(int id)
    {
        for (int i = 0; i < lstTouch.Count; i++)
        {
            if (lstTouch[i].fingerId == id)
            {
                return lstTouch[i];
            }
        }
        return null;
    }
    void Remove(int id)
    {
        for (int i = 0; i < lstTouch.Count; i++)
        {
            if (lstTouch[i].fingerId == id)
            {
                lstTouch.RemoveAt(i);
                return;
            }
        }
    }
    TouchInfo Add(Touch t)
    {
        TouchInfo info = new TouchInfo();
        info.position = t.position;
        info.delta = Vector2.zero;
        info.phase = t.phase;
        info.fingerId = t.fingerId;
        lstTouch.Add(info);
        return info;
    }
    TouchInfo AddMouseTouch(Vector2 position)
    {
        TouchInfo info = new TouchInfo();
        info.position = position;
        info.delta = Vector2.zero;
        info.phase = TouchPhase.Began;
        info.fingerId = 0;
        lstTouch.Add(info);
        return info;
    }
    void _OnFingerBegin(Touch t)
    {
        Remove(t.fingerId);
        TouchInfo info = Add(t);
        OnFingerDown(info);
    }
    void _OnFingerEnd(Touch t)
    {
        TouchInfo info = Find(t);
        if (info != null)
        {
            if (info.phase == TouchPhase.Moved)
            {
                OnFingerEndMove(info);
            }
            OnFingerUp(info);
            info.phase = TouchPhase.Moved;
        }
        Remove(t.fingerId);
    }
    void _OnFingerMove(Touch t)
    {
        TouchInfo info = Find(t);
        if (info != null)
        {
            if (info.phase == TouchPhase.Began || info.phase == TouchPhase.Stationary)
            {
                OnFingerBeginMove(info);
            }
            else if (info.phase == TouchPhase.Moved)
            {
                OnFingerMove(info);
            }
            info.phase = t.phase;
        }
    }
    void _OnFingerStay(Touch t)
    {
        TouchInfo info = Find(t);
        if (info != null)
        {
            if (info.phase == TouchPhase.Moved)
            {
                OnFingerEndMove(info);
            }
            info.phase = t.phase;
        }
    }

    void OnFingerDown(TouchInfo t)
    {
        DebugPrint(t.fingerId + " " + t.phase);
        //lastpos = t.position;
    }
    void OnFingerBeginMove(TouchInfo t)
    {
        DebugPrint(t.fingerId + " " + t.phase);
        if (t.fingerId != movefinger && rotatefinger == -1 && t.position.x > Screen.width * 0.5f)
        {
            rotatefinger    = t.fingerId;
        }

        if (t.fingerId != rotatefinger && movefinger == -1 && t.position.x < Screen.width * 0.5f)
        {
            movefinger = t.fingerId;
            lastpos = t.position;
        }

    }
    void OnFingerMove(TouchInfo t)
    {
        DebugPrint(t.fingerId + " " + t.phase);
        if (movefinger == t.fingerId)
        {
            lastdir = (t.position - lastpos).normalized;
        }
        else if (rotatefinger == t.fingerId)
        {
            float angle = t.delta.x * Time.deltaTime* rotate_velocity;
            transform.Rotate(Vector3.up, angle);
            xrotate += -t.delta.y* Time.deltaTime* rotate_velocity;
            if (xrotate > 89)
            {
                xrotate = 89;
            }
            if (xrotate < -10)
            {
                xrotate = -10;
            }
            XRotate.localRotation = Quaternion.AngleAxis(xrotate, Vector3.right);

        }
    }
    void OnFingerEndMove(TouchInfo t)
    {
        DebugPrint(t.fingerId + " " + t.phase);
        if (movefinger == t.fingerId)
        {
            lastdir = (t.position - lastpos).normalized;
        }
    }
    void OnFingerUp(TouchInfo t)
    {
        if(t==null)
        {
            return;
        }
        DebugPrint(t.fingerId + " " + t.phase);
        if (movefinger == t.fingerId)
        {
            movefinger = -1;
            lastdir = Vector2.zero;
        }
        if (rotatefinger == t.fingerId)
        {
            rotatefinger = -1;
        }
    }

    void DebugPrint(string content)
    {
        debug_string = content;
//        Debug.Log(debug_string);
    }
    void OnGUI()
    {
        Vector3 p = CameraDis.localPosition;
        if (GUI.RepeatButton(new Rect(600, 0, 100, 100), "+"))
        {
            p.z += -1.0f*Time.deltaTime;
        }
        if(GUI.RepeatButton(new Rect(600, 100, 100, 100), "-"))
        {
            p.z += 1.0f*Time.deltaTime;
        }
        if(p.z> -2)
        {
            p.z = -2;
        }
        if(p.z< -20)
        {
            p.z = -20;
        }
        CameraDis.localPosition = p;
    }
    void OnEnable()
    {
        CameraDis.localPosition = new Vector3(0, 0, -5);
        CameraDis.localRotation = Quaternion.identity;
    }
}
