using UnityEngine;
using System.Collections;

public class wangQiuAmmoNet : MonoBehaviour {

	public NetworkPlayer ownerPlayer;

	public string firePlayerName;
	public bool IsChangeClient = false;

	bool isRemoved = false;
	
	private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this
	private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this
	
	// Use this for initialization
	void Start () {
		correctPlayerPos = transform.position;
		correctPlayerRot = transform.rotation;
	}
	
	public void DelayRemoveAmmo()
	{
		Invoke("removeAmmo", 3f);
	}

	void removeAmmo()
	{
		//Debug.Log("removeAmmo ******************** test");
		closeDaoJuServer();
	}


	// Update is called once per frame
	void Update () {

		if(Application.loadedLevel < (int)GameLeve.Leve3)
		{
			Destroy(gameObject);
			return;
		}

		if(isRemoved)
		{
			return;
		}

		if(ownerPlayer != null & Network.player == ownerPlayer)
		{
			if(Network.isServer && !isRemoved)
			{
				float dis = Vector3.Distance(transform.position, correctPlayerPos);
				if(dis > 0.1f)
				{
					//Debug.Log("Server *********** test dis " + dis);
					transform.position = Vector3.Lerp(transform.position, correctPlayerPos, 0.5f);
					transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, 0.5f);
					
					networkView.RPC("sendDaoJuTranInfoToClient", RPCMode.OthersBuffered,
					                correctPlayerPos, correctPlayerRot);
				}
			}
		}
		else
		{
			if(Network.isClient)
			{
				float dis = Vector3.Distance(transform.position, correctPlayerPos);
				if(dis > 0.1f && IsChangeClient)
				{
					//Debug.Log("Client *********** test");
					networkView.RPC ("sendDaoJuTranInfoToServer", RPCMode.OthersBuffered,
					                 transform.position, transform.rotation);
				}
			}
		}
	}

	[RPC]
	void setAimName( string nameFire)
	{
		firePlayerName = nameFire;
	}

	public void setAmmoInfo( string nameFire )
	{
		firePlayerName = nameFire;
		networkView.RPC("setAimName", RPCMode.OthersBuffered, nameFire);

		if(Network.isClient)
		{
			IsChangeClient = true;
			//Debug.Log("setAmmoInfo ************** test");
			
			Invoke("removeAmmo", 3f);
		}
	}

	[RPC]
	void sendToServerSetIsRemoved()
	{
		//Debug.Log("sendToServerSetIsRemoved ************ test");
		isRemoved = true;
		IsChangeClient = false;
	}

	void delayRemoveAmmo()
	{
		Network.RemoveRPCs(networkView.viewID);
		Network.Destroy(gameObject);
	}

	public void closeDaoJuServer()
	{
		if(!enabled)
		{
			return;
		}
		networkView.RPC("sendToServerSetIsRemoved", RPCMode.AllBuffered);
		enabled = false;

		if(!networkView.isMine)
		{
			return;
		}
		Debug.Log("closeDaoJuServer... wanQiu");
		Invoke("delayRemoveAmmo", 2.0f);
	}
	
	[RPC]
	void sendToOtherCloseDaoJu()
	{
		if(Network.isServer)
		{
			isRemoved = true;
		}
		
		if(!enabled)
		{
			return;
		}
		enabled = false;

		if(!networkView.isMine)
		{
			return;
		}

//		if(IsChangeClient)
//		{
			Debug.Log("sendToOtherCloseDaoJu -> Remove Ammo");
			IsChangeClient = false;
			Network.RemoveRPCs(networkView.viewID);
			Network.Destroy(gameObject);
//		}
	}
	
	[RPC]
	void sendDaoJuTranInfoToServer(Vector3 pos, Quaternion rot)
	{
		correctPlayerPos = pos;
		correctPlayerRot = rot;
	}
	
	[RPC]
	void sendDaoJuTranInfoToClient(Vector3 pos, Quaternion rot)
	{
		if(IsChangeClient)
		{
			return;
		}
		
		transform.position = pos;
		transform.rotation = rot;
	}
}
