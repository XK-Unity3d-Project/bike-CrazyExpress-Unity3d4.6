using UnityEngine;
using System.Collections;

public class rainNetCtrl : MonoBehaviour {

	public NetworkPlayer ownerPlayer;
	
	public Transform RainObj;

	bool isStopPlayRain = false;
	bool isPlayRain = false;
	bool isPlayRainState = false;

	[RPC]
	void sendToClientPlayRain()
	{
		isPlayRain = true;
	}

	void delaySetIsPlayRain()
	{
		isPlayRain = true;
		networkView.RPC("sendToClientPlayRain", RPCMode.OthersBuffered);
	}

	// Update is called once per frame
	void Update () {

		if(Application.loadedLevel != (int)GameLeve.Leve3)
		{
			return;
		}

		if(ownerPlayer != null & Network.player == ownerPlayer)
		{
			if(Network.isServer)
			{
				//Debug.Log("rainNetCtrl ************ server 2");
				if(!isPlayRainState)
				{
					isPlayRainState = true;
					float time = Random.Range(15.0f, 20.0f);
					Invoke("delaySetIsPlayRain", time);
				}
			}
		}

		if(isPlayRain && !RainObj.gameObject.activeSelf)
		{
			//Debug.Log("play rain");
			RainObj.gameObject.SetActive(true);
			if(Network.isServer)
			{
				float time = Random.Range(20.0f, 30.0f);
				Invoke("setStopPlayRain", time);
			}
		}

		if(isStopPlayRain && RainObj.gameObject.activeSelf)
		{
			//Debug.Log("stop play rain");
			isPlayRain = false;
			isStopPlayRain = false;
			RainObj.gameObject.SetActive(false);
		}
	}

	void setStopPlayRain()
	{
		isStopPlayRain = true;
		networkView.RPC("sendToClientStopPlayRain", RPCMode.OthersBuffered);
	}

	[RPC]
	void sendToClientStopPlayRain()
	{
		isStopPlayRain = true;
	}
}
