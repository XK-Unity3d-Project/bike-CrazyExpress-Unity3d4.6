using UnityEngine;
using System.Collections;

public class LinkGameInfo : MonoBehaviour {

	public GameObject ServerInfo;
	public GameObject ClientInfo;

	private static LinkGameInfo _instance;
	public static LinkGameInfo GetInstance()
	{
		return _instance;
	}

	// Use this for initialization
	void Start()
	{
		_instance = this;
		NetCtrl.InitPlayerIpArray();
		NetworkServerNet.CountPlayerStatic = 0;
		PlayerCreatNet.ResetClientCount();
	}

	public void SetVisibleServer(bool isVisible)
	{
		if(isVisible)
		{
			if(!IsInvoking("ShowServerUI"))
			{
				Invoke("ShowServerUI", 20.0f);
			}
		}
		else
		{
			CancelInvoke("ShowServerUI");
			ServerInfo.SetActive(false);
		}
	}
	
	public void SetVisibleClient(bool isVisible)
	{
		if(isVisible)
		{
			if(!IsInvoking("ShowClientUI"))
			{
				Invoke("ShowClientUI", 30.0f);
			}
		}
		else
		{
			CancelInvoke("ShowClientUI");
			ClientInfo.SetActive(false);
		}
	}

	void ShowServerUI()
	{
		ServerInfo.SetActive(true);
	}

	void ShowClientUI()
	{
		ClientInfo.SetActive(true);
	}
}
