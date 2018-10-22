using UnityEngine;
using System.Collections;


    /// <summary>
    /// 相机分为两层    
    /// 1.transform (public)是相机的基础功能层
    /// 2.rootTransform (private) 处理位移旋转碰撞等
    /// </summary>

    public class CameraManager : Singleton<CameraManager>
    {
        public delegate void RoationFinish();
        public RoationFinish roationFinish = null;
        public Camera m_pCamera = null;
        public Camera m_UICamera = null;
        // public Transform transform;  // camera
        public Transform targetTransform; // player
        private Transform rootTransform; // root of camera  parent

        public Vector3 targetPosOffset = new Vector3(0.03f, 1.42f, 0);//player offser

        public enum CameraState
        {
            NORMAL = 0,
            BATTLE = 1,
            AUTO_FOLLOW = 2,
            DRAG_CAMERA = 3,
        }

        public enum CameraAction
        {
            ENTER_BATTLE = 0,
            LEAVE_BATTLE = 1,
            LEAVE_BATTLE_TO_AUTO_FOLLOW = 2,
            ENTER_AUTO_FOLLOW = 3,
            LEAVE_AUTO_FOLLOW = 4,
            ENTER_DRAG_CAMERA_IN_AUTO_FOLLOW = 5,
            LEAVE_DRAG_CAMERA_IN_AUTO_FOLLOW = 6,
            ENTER_DRAG_CAMERA_IN_BATTLE = 7,
            LEAVE_DRAG_CAMERA_IN_BATTLE = 8,
            ENTER_DRAG_CAMERA_IN_NORMAL = 9,
            LEAVE_DRAG_CAMERA_IN_NORMAL = 10,
        }

        private float scaleSpeed = 0.02f;
        private float minDistance = 5.0f;
        private float maxDistance = 30.0f;
        private float _currDistance = 10.0f;
        private CameraState _currentCameraState = CameraState.NORMAL;
        public float CurrDistance
        {
            get
            {
                return _currDistance;
            }
            set
            {
                _currDistance = value;
            }
        }

        public CameraState CurrentCameraState
        {
            get
            {
                return _currentCameraState;
            }
            set
            {
                _currentCameraState = value;
            }
        }
        public float _ActDistance = 0f;
        public float CameraActDistance
        {//实际玩家到摄像机的距离
            get
            {
                return _ActDistance;
            }
        }
        public Vector3 CamOffset = new Vector3(0, -0.03f, -1);

        private RaycastHit hit;
        private LayerMask wallMask;
        private LayerMask groundMask;
        private LayerMask waterMask;

        private float _senstiveX = 0.3f;
        private float _senstiveY = 0.2f;
        private float _angleX = 0;
        private float _angleY = 1;
        public float AngleX
        {
            get
            {
                return _angleX;
            }
            set
            {
                if (float.IsNaN(value))
                {
     
                    return;
                }
                _angleX = value;
            }
        }
        public float AngleY
        {
            get
            {
                return _angleY;
            }
            set
            {
                if (float.IsNaN(value))
                {
                 
                    return;
                }
                _angleY = value;
            }
        }
        private float _minAngleY = -30;
        private float _maxAngleY = 80;
        private float _fov = 60;
        private float _minInterviewDis = 5;
        private Vector2 _targetInterviewRotation = Vector2.zero;//from sheet
        private float _targetInterviewDis = 5;//from sheet
        private float _targetInterviewOffset = 0f;

        public bool EnableScaleDistance = true;
        public bool EnableRotation = true;

        private bool _enableAutoFollow = true;
        //private bool _autoFollowing = false;
        public float _autoFollowSpeedStart = 0.005f;//自动跟随第一段平滑速度
        public float _autoFollowSpeed = 0.05f;//自动跟随第一段之后的平滑速度
        public Vector3 _autotargetfollow = new Vector3(0, 15f, 7f);

        private bool _stopBattleToAutoFollow = false;
        private CameraState _lastCameraState = CameraState.NORMAL;


        //自动旋转镜头
        public bool AutoRotation
        {
            get
            {
                return _autoRotation;
            }
        }
        private bool _autoRotation = false;//自动旋转中...
        private Vector3 _oldpos = Vector3.zero;
        private Vector3 _targetpos = Vector3.zero;
        private Vector3 _deltapos = Vector3.zero;
//         private float _targetDeltaAngleX = 0;//自动旋转每帧需要转动的大小
//         private float _targetDeltaAngleY = 0;
//         private float _targetDeltaDistance = 0;


        private bool _ShakeCamera = false;
        Vector3 _ShakeCamerPos = Vector3.zero;

        public bool IsCamerShake()
        {
            return _ShakeCamera;
        }
        public Quaternion currentQuaternion
        {
            get { return Quaternion.Euler(_angleY, _angleX, 0); }
        }

        public float SetMaxDistance
        {//给外部设置镜头拖拉远近的 最大距离
            get
            {
                return maxDistance;
            }
            set
            {
                maxDistance = value;//revert
            }
        }

        public void ClearMainCamera()
        {
            if(null != m_pCamera)
            {
                var parent = m_pCamera.transform.parent;
                if (null != parent)
                    GameObject.Destroy(parent.gameObject);
                m_pCamera = null;
            }
        }
      
       
        public Camera GetMainCamera()
        {
            return m_pCamera;
        }
        public Camera GetMainUICamera()
        {
            if (m_UICamera == null)
            {
                if(GameObject.Find("UIRoot/UICamera")!=null)
                    m_UICamera = GameObject.Find("UIRoot/UICamera").GetComponent<Camera>();
            }
            
            return m_UICamera;
        }

        public Vector3 GetForward()
        {
            return (m_pCamera != null) ? transform.forward : Vector3.zero;
        }


        #region calculate
     

        public void SetFollowTransform(Transform followTransform)
        {
            targetTransform = followTransform;  
        }

        public void SetRootTranform(Transform pRootTransform)
        {
            rootTransform = pRootTransform;
        }
        void UpdateShakeCamera(Vector3 newpos)
        {
            if (_ShakeCamera)
            {//相当于人在振动，相机跟着计算位置刷新。否则相机单独计算位置可能会逻辑冲突
           

            }
        }
        float followtimer = 0f;//follow的平滑时间
        float b_startimer = 2f;//follow状态第一段平滑时间更长，第一段需要平滑1s以上，并且平滑到同步后，才开始2段平滑
        float _dragcameratimer = 5f;//在autofollow时拖动相机，需要几秒钟来回归原位
        bool _back_to_phase_1 = false;//是否回归到第一个平滑阶段
        public float delta_angle_x = 0.0f;
        private void UpdateAutoFollow()
        {
            if (_enableAutoFollow && _currentCameraState == CameraState.AUTO_FOLLOW)
            {
                float mainCharY = targetTransform.transform.localEulerAngles.y;

                float x = _autotargetfollow.x + mainCharY;
                float y = _autotargetfollow.y;
                float d = _autotargetfollow.z;
                if (x - _angleX > 180)
                {
                    x = x - 360;
                }
                else if (x - _angleX < -180)
                {
                    x = x + 360;
                }
                if (b_startimer >0)
                {//平滑第一阶段 慢平滑
                    //还没反过来
                    b_startimer -= Time.deltaTime;
                    b_startimer = Mathf.Clamp(b_startimer, 1, 2);
                    followtimer += Time.deltaTime * _autoFollowSpeedStart;
                }
                else
                {//平滑第2阶段 快平滑
                    followtimer += Time.deltaTime * _autoFollowSpeed;
                    delta_angle_x = Mathf.Abs(x - _angleX);
                    if (Mathf.Abs(x - _angleX) > 10.0f) //如果角度变换太快，就强制回到第一阶段
                    {
                        _back_to_phase_1 = true;
                    }
                }
                _currDistance = Mathf.Lerp(_currDistance, d, followtimer);
                _angleX = Mathf.Lerp(_angleX, x, followtimer);
                _angleY = Mathf.Lerp(_angleY, y, followtimer);
                
                if (b_startimer <= 1 && Mathf.Abs(_angleX - x) < 1f) //第一阶段平滑结束
                {
                    followtimer = 0f;
                    b_startimer = 0f;
                }

                if (_back_to_phase_1 && Mathf.Abs(_angleX - x) < 1f) //变换太快，第二阶段强制结束，返回第一阶段
                {
                    b_startimer = 2f;
                    followtimer = 0;
                    _back_to_phase_1 = false;
                }
            }
            else
            {
                b_startimer = 2f;
                followtimer = 0;
                if (_currentCameraState== CameraState.DRAG_CAMERA)
                {
                    _dragcameratimer -= Time.deltaTime;
                    if (_dragcameratimer < 0)
                    {
                     //   Transit(CameraAction.LEAVE_DRAG_CAMERA_IN_AUTO_FOLLOW);
                    }
                }
            }
        }

        public void Update()
        {//根据不同的camera状态切换不同的camera的update方法
          //  UpdateAutoRotation();
            UpdateAutoFollow();
        }

        public float smoothspeeed = 0.5f;//开始移动瞬间的平滑速度插值,值越小越平滑
        public float SensitiveShake = 0.3f;//左右晃动过程的平滑速度插值乘量，越小越平滑  越小也越容易失去焦点 玩家跑出镜头
        public float SensitiveSqrtY = 0.00001f;//y方向的上下坎高度的平方
        private bool _FirstSmooth = false;//是否处于刚开始移动的平滑逻辑
        private float smoothShake = 1f;//玩家乱动时的平滑乘值
        private float _smoothIndex =0f;//静止计数
        private float _speedtimer = 0;//timer平滑时间
   
        private  Vector3 _lastPos = Vector3.zero;
        private Vector3 _latsDir = Vector3.zero;
        //上一帧的数据
        private float _lastDirY = 0;

        public Vector3 GetCameraPostion()
        {
            return rootTransform.position;
        }

        public void LateUpdate()
        {//LateUpdate主要做了相机防抖动、相机碰撞等后处理工作，然后计算出相机真正的位置
            if (null == targetTransform || null == rootTransform)
            {
                return; 
            }
            _angleX = Mathf.Repeat(_angleX, 360);
            _angleY = Mathf.Clamp(_angleY, _minAngleY, _maxAngleY);
            _currDistance = Mathf.Clamp(_currDistance, minDistance, maxDistance);

/*            Debug.Log(_angleX + "  " + _angleY+"   "+ _currDistance);*/
            var dist = CamOffset * _currDistance;
            var rotation = Quaternion.Euler(_angleY, _angleX, 0);
            Vector3 targetPos = UpdateSmooth();
            if (_ShakeCamera)
            {
               // Vector3 newpos = targetTransform.position + targetPosOffset + rotation * dist;
                Vector3 newpos = targetPos + targetPosOffset + rotation * dist;
                UpdateShakeCamera(newpos);
                rootTransform.position = _ShakeCamerPos;
                rootTransform.rotation = rotation;
            }
            else
            {//防抖动的是玩家坐标
               
             //   Vector3 targetPos =  UpdateSmooth();//calculate player smooth position
                Vector3 newpos = targetPos + targetPosOffset + rotation * dist;
                _cameraspeedtimer0 += (Time.smoothDeltaTime * camerasmooth);//smooth Barrier to camera
                Vector3 LerpPos = Vector3.Lerp(rootTransform.position, newpos, _cameraspeedtimer0);//相机要从障碍物处平滑恢复

                rootTransform.position = LerpPos;
                rootTransform.rotation = rotation;
            }
            _ActDistance = _currDistance;


        }
        public void SetCameraSmooth()
        {//需要平滑的地方调用一下，每次调用可以在段时间内使用平滑
            _smoothIndex = 0;
            _speedtimer = 0;
            smoothShake = 1;
            _FirstSmooth = true;
        }
        public void DelaySetCameraSmooth(float t)
        {
            StartCoroutine(waitSmooth(t));
        }
        IEnumerator waitSmooth(float t)
        {
            yield return new WaitForSeconds(t);
            SetCameraSmooth();
        }
        public void SetCameraSmoothEnd()
        {

        }
        private Vector3 UpdateSmooth()
        {//smooth player
            _speedtimer += (Time.smoothDeltaTime * smoothspeeed* smoothShake);
    

            Vector3 targetPos = Vector3.Lerp(_lastPos, targetTransform.position, _speedtimer);

            if (targetTransform.position == targetPos && _lastPos == targetPos)
            {//smooth logic  解决从静止开始移动的瞬间抖动
                _smoothIndex++;
                if (_smoothIndex > 5)
                {//静止超过一定时间5fps
                    _smoothIndex = 0;
                    _speedtimer = 0;
                    smoothShake = 1;
                    _FirstSmooth = true;
                }
            }
            else
            {
                _smoothIndex = 0;
                //解决运动过程中反复来回抖动问题
               // Debug.Log((targetPos - _lastPos).magnitude+"    "+ (targetTransform.position - _lastPos).magnitude);
                Vector3 Dir= (targetPos - _lastPos).normalized;
                float D = Vector3.Dot(Dir, _latsDir);//horizontal
                float DirY = targetPos.y - _lastPos.y;
                float V = _lastDirY * DirY;//vertical
                if (_FirstSmooth == false&& (D <0||(V< -SensitiveSqrtY)))
                {      //移动中反向要平滑镜头
                    smoothShake=SensitiveShake;
                    _speedtimer = 0;
                    // Debug.LogError("reset" +D+"  "+V);
                }
                if (_speedtimer > 1)
                {//平滑结束后恢复
                    smoothShake = 1;
                    _FirstSmooth = false;
                }
                _latsDir = Dir;
                _lastDirY = DirY;
            }
  
            _lastPos = targetPos;


            return targetPos;
        }
   
        private bool _birthcamera=false;
        public void ApplyBirthCameraConfig(float X,float Y,float D)
        {
            _angleX = X;
            _angleY = Y;
            _currDistance = D;
            _birthcamera = true;
        }

        private bool HideMainChar1 = false;
        private bool HideMainChar2 = false;
        private bool HideMainChar3 = false;
        public float camerasmooth = 3f;//移动到障碍物处的相机平滑速度
        private float _cameraspeedtimer0 = 0;//障碍我恢复时间timer
        private float _cameraspeedtimer1 = 0;//timer平滑时间
        private float _cameraspeedtimer2 = 0;
        private float _cameraspeedtimer3 = 0;
 
        #endregion



     

    }

