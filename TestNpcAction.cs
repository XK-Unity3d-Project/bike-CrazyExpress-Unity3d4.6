using UnityEngine;
using System.Collections;

public class TestNpcAction : MonoBehaviour {

	// Use this for initialization
	void Start () {
		player = gameObject;
		if (!IsTestNewAction) {
			AnimationState run3 = player.animation["run3"];
			Debug.Log("run3.layer "+run3.layer);
			
			turnLeft1 = player.animation["turnLeft1"];
			turnLeft1.layer = 1;
			turnLeft1.blendMode = AnimationBlendMode.Additive;
			//turnLeft1.wrapMode = WrapMode.Once;
			//turnLeft1.enabled = true;
			turnLeft1.weight  = 1.0f;
		}
		else {
			AniCom = GetComponent<Animator>();
		}
	}

	GameObject player;
	bool IsTurnLeft;
	AnimationState turnLeft1;
	Animator AniCom;
	bool IsTestNewAction = true;
	[Range(0f, 1f)]public float TimeTest = 0f;
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.A) && !IsTurnLeft)
		{
			IsTurnLeft = true;
		
			if (!IsTestNewAction) {
				//turnLeft1.weight  = 1f;
				player.animation.CrossFade("turnLeft1", 1f);
			}
			else {
				AniCom.SetBool("turnLeft1", true);
			}
		}

		if (Input.GetKeyUp(KeyCode.A) && IsTurnLeft) {
			IsTurnLeft = false;
			if (!IsTestNewAction) {
				//AnimationState turnLeft1 = player.animation["turnLeft1"];
				//player.animation.Stop("turnLeft1");
				//turnLeft1.normalizedTime = 0f;
				//turnLeft1.enabled = false;
				//turnLeft1.weight  = 0f;
			}
			else {
				AniCom.SetBool("turnLeft1", false);
			}
		}

		if (AniCom != null) {
			AniCom.SetFloat("TestTurn", TimeTest);
		}
	}
}
