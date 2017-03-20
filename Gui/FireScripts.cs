using UnityEngine;
using System.Collections;
using System.Linq;
public class FireScripts : MonoBehaviour {
	//private float time;
	public  GameObject wangqiu;
	private  Transform hand;
	static private GameObject npc;
	private Vector3 target;
	float wangqiuspeed = 200;

	public static FireScripts _Instance;
	public static FireScripts GetInstance()
	{
		return _Instance;
	}

	static BikeAnimatorCtrl AniScript;
	static public void wangQiuTranEvent(GameObject obj)
	{
		npc = obj;
		AniScript = obj.GetComponent<BikeAnimatorCtrl>();
	}

	void Start()
	{
		_Instance = this;
		InputEventCtrl.GetInstance().ClickFireBtEvent += ClickFireBtEvent;
	}

	void ClickFireBtEvent(ButtonState val)
	{
		//ScreenLog.Log("FireScripts::ClickStartBtEvent -> val " + val);
		if(val == ButtonState.DOWN)
		{
			return;
		}

		if(GlobalScript.GetInstance().player.wangQiuTran != null)
		{
			hand = GlobalScript.GetInstance().player.wangQiuTran;
		}
		
		if(hand == null)
		{
			ScreenLog.LogWarning("ClickFireBtEvent -> mei you shou!");
			return;
		}
		
		if(isFire)
		{
			return;
		}
		
		//if(true) //test
		if(GlobalScript.GetInstance().player.CanFire)
		{
			if(GlobalData.GetInstance().gameMode == GameMode.OnlineMode)
			{
				bikeNetUnity bikeNet = npc.GetComponent<bikeNetUnity>();
				if(bikeNet != null)
				{
					bikeNet.playFireAction();
				}
			}
			else {
				AniScript.PlayAnimation(PlayerAniEnum.fire, 1f);
			}
			StartCoroutine(Fire());
			GlobalScript.GetInstance().player.RemoveTennis();
			isFire = true;
		}
	}

	//public Transform TestAimPos;

	wangQiuAmmoNet wangQiuAmmoNetScript;
	bool isFire = false;
	public IEnumerator Fire()
	{
		GameObject newwangqiu=null;
		yield return new  WaitForSeconds(0.5f);
		if(wangqiu)
		{
			isFire = false;
//			GlobalScript.GetInstance().FireTennis();

			if(GlobalData.GetInstance().gameMode == GameMode.OnlineMode)
			{
				//Debug.Log("test ********* spawn wangQiu");
				int playerID = int.Parse(Network.player.ToString());
				newwangqiu = (GameObject)Network.Instantiate(wangqiu, hand.position, hand.rotation, playerID);

				wangQiuAmmoNetScript = newwangqiu.GetComponent<wangQiuAmmoNet>();
				if(wangQiuAmmoNetScript != null)
				{
					wangQiuAmmoNetScript.setAmmoInfo( npc.name );
					wangQiuAmmoNetScript.DelayRemoveAmmo();
				}
			}
			else
			{
				newwangqiu = (GameObject)Instantiate(wangqiu, hand.position, hand.rotation);
				wangQiuAmmoCtrl ammoScript = newwangqiu.GetComponent<wangQiuAmmoCtrl>();
				ammoScript.DelayRemoveAmmo();
				AniScript.ResetRunActionInfo();
			}

			newwangqiu.rigidbody.useGravity = false;
			newwangqiu.transform.position = hand.transform.position;
			Vector3 v = GlobalScript.GetInstance().player.AimPossion - hand.position;
			newwangqiu.rigidbody.AddForce(v.normalized*wangqiuspeed, ForceMode.Impulse);
		}
	}
}
