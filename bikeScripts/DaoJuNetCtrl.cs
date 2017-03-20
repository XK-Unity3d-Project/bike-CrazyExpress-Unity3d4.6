using UnityEngine;
using System.Collections;

public class DaoJuNetCtrl : MonoBehaviour {
	
	public NetworkPlayer ownerPlayer;
	
	public bool IsHitClient = false;

	private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this
	private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this

	// Use this for initialization
	void Start () {

		IsHitClient = false;
		correctPlayerPos = transform.position;
		correctPlayerRot = transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
		if(ownerPlayer != null & Network.player == ownerPlayer)
		{
			if(Network.isServer)
			{
				float dis = Vector3.Distance(transform.position, correctPlayerPos);
				if(dis > 0.1f)
				{
					transform.position = Vector3.Lerp(transform.position, correctPlayerPos, 0.1f);
					transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, 0.1f);

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
				if(dis > 0.1f && IsHitClient)
				{
					networkView.RPC ("sendDaoJuTranInfoToServer", RPCMode.Server,
					                 transform.position, transform.rotation);
				}
			}
		}
	}

	public void closeDaoJuServer()
	{
		ScreenLog.Log("closeDaoJuServer -> daoJuName " + gameObject.name);
//		if(Network.peerType == NetworkPeerType.Connecting)
//		{
			networkView.RPC("sendToOtherCloseDaoJu", RPCMode.OthersBuffered);
//		}
		enabled = false;

		if(gameObject != null)
		{
//			Network.RemoveRPCs(networkView.viewID);
//			Network.Destroy(gameObject);
			gameObject.SetActive(false);
		}
	}

	[RPC]
	void sendToOtherCloseDaoJu()
	{
		ScreenLog.Log("sendToOtherCloseDaoJu -> daoJuName " + gameObject.name);
		enabled = false;
		gameObject.SetActive(false);
//		Network.RemoveRPCs(networkView.viewID);
//		Network.Destroy(gameObject);
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
		if(IsHitClient)
		{
			return;
		}

		transform.position = pos;
		transform.rotation = rot;
	}

//	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
//	{
//	}

	public void closeMeshRender()
	{
		MeshRenderer meshRend = transform.GetComponentInChildren<MeshRenderer>();
		meshRend.enabled = false;
//		if(Network.peerType == NetworkPeerType.Connecting)
//		{
			networkView.RPC("sendToServerCloseMeshRender", RPCMode.OthersBuffered);
//		}
		gameObject.SetActive(false);
	}

	[RPC]
	void sendToServerCloseMeshRender()
	{
		//ScreenLog.Log("sendToServerCloseMeshRender............daoJu is " + gameObject.name);
		MeshRenderer meshRend = transform.GetComponentInChildren<MeshRenderer>();
		meshRend.enabled = false;

		Transform particleObj = meshRend.transform.GetChild(0);
		if(!particleObj.gameObject.activeSelf)
		{
			particleObj.gameObject.SetActive(true);
		}

		Invoke("HiddenDaoJu", 0.05f);
	}

	void HiddenDaoJu()
	{
		gameObject.SetActive(false);
	}
}
