using UnityEngine;
using System.Collections;

public class Fire : MonoBehaviour {
	public GameObject SightBead;
	private TweenScale scale;
	public GameObject camera1; //main camera
	public GameObject camera2; //ui camera
	private Vector3 zhunXingPos;

	// Use this for initialization
	void Awake()
	{
		scale = transform.GetComponentInChildren<TweenScale>();
	}

	void Start () {
		GlobalScript.GetInstance().player.CanFireChange+=CanFireChange;
		//UICamera uica=camera.GetComponent<UICamera>();
	
	}

	public void playTween()
	{
		scale.ResetToBeginning();
		scale.PlayForward();
	    EventDelegate.Add(scale.onFinished,delegate {
			GlobalScript.GetInstance().player.CanFire=true;
		});
	}
	
	public void CanFireChange()
	{
		if(gameObject.activeSelf)
		{
			if(GlobalScript.GetInstance().player.CanFire)
			{
				pcvr.FireLightState = LedState.Shan;
				transform.GetChild(0).GetComponent<UISprite>().spriteName="ZhunXingLv";
			}
			else
			{
				pcvr.FireLightState = LedState.Mie;
				//Debug.Log("2222222222222222");
				transform.GetChild(0).GetComponent<UISprite>().spriteName="ZhunXingHong_1";
			}
		}
	}

	void FixedUpdate()
	{
		zhunXingPos = GlobalScript.GetInstance().player.AimPossion;
		zhunXingPos = camera1.camera.WorldToScreenPoint(zhunXingPos);

		zhunXingPos.z = 0;
		zhunXingPos = camera2.camera.ScreenToWorldPoint(zhunXingPos);

		transform.position = zhunXingPos;
	}
}
