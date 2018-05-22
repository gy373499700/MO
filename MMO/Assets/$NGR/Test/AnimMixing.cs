using UnityEngine;
using System.Collections;

public class AnimMixing : MonoBehaviour {
    public Animation anim;
    public AnimationState as_idle;
    public AnimationState as_shoot;
	public string run_anim_name = "$run_Left01";
	public string shoot_anim_name = "attack01";
    public Transform rotatebonedown = null;
	public Transform rotateboneup = null;
	public Transform rotatebonechest = null;
    public float x = 0;
	public float y = 0;
	public float z = 0;
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animation>();
        as_idle = anim[run_anim_name];
		as_shoot = anim[shoot_anim_name];

        as_idle.layer = -1;
        as_idle.wrapMode = WrapMode.Loop;
		Transform t = transform.FindChild("Bip01/Bip01 Spine");
        as_shoot.AddMixingTransform(t, true);
        as_shoot.layer = 10;
        as_shoot.wrapMode = WrapMode.Once;

        anim.Play(run_anim_name);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void LateUpdate()
    {
		rotatebonedown.rotation = Quaternion.AngleAxis( x, Vector3.up)*rotatebonedown.rotation;
		rotateboneup.rotation = Quaternion.AngleAxis(y, Vector3.up)*rotateboneup.rotation;
		rotatebonechest.rotation = Quaternion.AngleAxis(z, Vector3.up)*rotatebonechest.rotation;
    }
    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 100, 100), "attack"))
        {
			anim.Play(shoot_anim_name, PlayMode.StopSameLayer);
        }
        //x = 0.0f;
        if (GUI.RepeatButton(new Rect(0, 100, 100, 100), "left"))
        {
            x += Time.deltaTime;

        }
        if (GUI.RepeatButton(new Rect(0, 200, 100, 100), "right"))
        {
            x += -Time.deltaTime;
        }
    }
}
