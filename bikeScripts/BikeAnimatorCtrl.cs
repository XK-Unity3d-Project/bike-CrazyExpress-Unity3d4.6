using UnityEngine;
using System.Collections;

public enum PlayerAniEnum
{
	root,
	run,
	run1,
	run2,
	run3,
	fire,
	failed,
	huanhu,
	Cool,
	Cool2,
	Cool3,
	TurnVal,
	IsTurn,
}

public class BikeAnimatorCtrl : MonoBehaviour {
	public bool IsAiPlayer;
	Animator PlayerAni;
	bike BikeScript;
	bikeNetUnity BikeNetScript;
	// Use this for initialization
	void Awake()
	{
		BikeNetScript = GetComponent<bikeNetUnity>();
		BikeScript = GetComponent<bike>();
		PlayerAni = GetComponent<Animator>();
		PlayRootAni();
	}

	void Update()
	{
		if (Network.peerType == NetworkPeerType.Server) {
			return;
		}

		switch (Network.peerType) {
		case NetworkPeerType.Client:
			if (BikeNetScript == null || !BikeNetScript.CheckPlayerClient()) {
				return;
			}
			break;
		case NetworkPeerType.Disconnected:
			if (BikeScript == null || BikeScript.GetIsAiNPC()) {
				return;
			}
			break;
		}

		float playerSpeed = GlobalScript.GetInstance().player.Speed;
		if (playerSpeed <= 3) {
			if (PlayerAni.GetBool("root") && PlayerAni.speed != 1f) {
				PlayerAni.speed = 1f;
			}
			return;
		}

		float minSpeed = 0f;
		float maxSpeed = 40f;

		float minVal = BikeGameCtrl.GetInstance().AniRunSpeedMin;
		float maxVal = BikeGameCtrl.GetInstance().AniRunSpeedMax;

		float aniSpeed = 1f;
		if (playerSpeed <= maxSpeed) {
			float key = (maxVal - minVal) / (maxSpeed - minSpeed);
			aniSpeed = minVal + key * (playerSpeed - minSpeed);
		}

		if (PlayerAni.speed != aniSpeed) {
			PlayerAni.speed = aniSpeed;
		}
	}

	public void ResetRunActionInfo()
	{
		BikeScript.ResetRunActionInfo();
	}

	void PlayRootAni()
	{
		if (!IsAiPlayer) {
			PlayerAni.SetBool(PlayerAniEnum.fire.ToString(), false);
			PlayerAni.SetBool(PlayerAniEnum.failed.ToString(), false);
			PlayerAni.SetBool(PlayerAniEnum.huanhu.ToString(), false);
			PlayerAni.SetBool(PlayerAniEnum.Cool.ToString(), false);
			PlayerAni.SetBool(PlayerAniEnum.Cool2.ToString(), false);
			PlayerAni.SetBool(PlayerAniEnum.Cool3.ToString(), false);
			PlayerAni.SetFloat(PlayerAniEnum.TurnVal.ToString(), 0.5f);
		}
		PlayerAni.SetBool(PlayerAniEnum.root.ToString(), true);
		PlayerAni.SetBool(PlayerAniEnum.run1.ToString(), false);
		PlayerAni.SetBool(PlayerAniEnum.run2.ToString(), false);
		PlayerAni.SetBool(PlayerAniEnum.run3.ToString(), false);
	}

	void ResetPlayerAni()
	{
		if (!IsAiPlayer) {
			PlayerAni.SetBool(PlayerAniEnum.fire.ToString(), false);
			PlayerAni.SetBool(PlayerAniEnum.failed.ToString(), false);
			PlayerAni.SetBool(PlayerAniEnum.huanhu.ToString(), false);
			PlayerAni.SetBool(PlayerAniEnum.Cool.ToString(), false);
			PlayerAni.SetBool(PlayerAniEnum.Cool2.ToString(), false);
			PlayerAni.SetBool(PlayerAniEnum.Cool3.ToString(), false);
		}
		PlayerAni.SetBool(PlayerAniEnum.root.ToString(), false);
		PlayerAni.SetBool(PlayerAniEnum.run.ToString(), false);
		PlayerAni.SetBool(PlayerAniEnum.run1.ToString(), false);
		PlayerAni.SetBool(PlayerAniEnum.run2.ToString(), false);
		PlayerAni.SetBool(PlayerAniEnum.run3.ToString(), false);
	}

	public void PlayAnimation(PlayerAniEnum ani, float val)
	{
		if (PlayerAni == null) {
			return;
		}

		switch (ani) {
		case PlayerAniEnum.Cool:
			break;
		default:
			if (BikeScript != null && BikeScript.getIsPlayCool(1)) {
				return;
			}

			if (BikeNetScript != null && BikeNetScript.getIsPlayCool(1)) {
				return;
			}
			break;
		}
		
		ResetPlayerAni();
		if (PlayerAni.GetBool(ani.ToString()) == true) {
			return;
		}
		PlayerAni.SetBool(ani.ToString(), true);
		PlayerAni.speed = val;
	}

	public bool GetAniBool(PlayerAniEnum ani)
	{
		if (PlayerAni == null) {
			return false;
		}
		return PlayerAni.GetBool(ani.ToString());
	}

	public void SetAnimatorFloat(PlayerAniEnum ani, float val)
	{
		if (PlayerAni == null) {
			return;
		}
		PlayerAni.SetFloat(ani.ToString(), val);
	}

	public void SetAnimatorSpeed(float val)
	{
		PlayerAni.speed = val;
	}

	void OnFinishCoolAni(int val)
	{
		if (BikeNetScript != null) {
			BikeNetScript.HandleAnimationEventCool(val);
		}
		else if (BikeScript != null) {
			switch (val) {
			case 1:
			case 2:
				break;
				
			default:
				ResetPlayerAni();
				break;
			}
			BikeScript.resetPlayCoolState(val);
		}
	}

	public void SetAniBool(PlayerAniEnum ani, bool isActive)
	{
		if (PlayerAni == null) {
			return;
		}
		PlayerAni.SetBool(ani.ToString(), isActive);
	}

	public void CloseAnimator()
	{
		PlayerAni.enabled = false;
	}
}