using UnityEngine;
using System.Collections;

public class RockNet : MonoBehaviour {

	public NetworkPlayer ownerPlayer;

	public float LifeTime=3.0f;
	public float Force=10;
	public float probability;
	
	private GameObject rockParticle = null;
	void Start()
	{
		//Debug.Log("transform.childCount " + transform.childCount);
		correctPlayerPos = transform.position;
		correctPlayerRot = transform.rotation;

		rockParticle = transform.GetChild(0).gameObject;
		if(rockParticle == null)
		{
			ScreenLog.Log("rockParticle is null! name " + transform.name);
		}
	}

	[RPC]
	void ShowRockParticle()
	{
		rockParticle = transform.GetChild(0).gameObject;
		if(rockParticle != null)
		{
			rockParticle.SetActive(true);
		}
	}
	
	public void AddForce()
	{
		//Debug.Log("2222222222222222222222222");
		if(transform)
		{
			if(rockParticle == null)
			{
				Start();
			}
			
			if(rockParticle != null)
			{
				rockParticle.SetActive(true);
			}
			networkView.RPC("ShowRockParticle", RPCMode.OthersBuffered);

			IsHitClient = true;
			transform.collider.enabled = true;
			transform.GetComponent<Rigidbody>().isKinematic = false;
			
			// Debug.Log("+++++++++++++++"+(new Vector3(599f,46f,768f)-transform.position).x+(new Vector3(599f,46f,768f)-transform.position).z);
			transform.GetComponent<Rigidbody>().AddRelativeForce(Vector3.right*Force,ForceMode.Impulse);
		}
	}
	
	public void BenginDestoryColne()
	{
		StartCoroutine("DestoryColne");
	}

	public IEnumerator DestoryColne()
	{
		yield return new WaitForSeconds(LifeTime);
		//Destroy(gameObject);

		//remove the rock
		removeRock();
	}
	
	void OnDrawGizmosSelected() 
	{
		Gizmos.color = Color.red;
		Vector3 direction = transform.TransformDirection(Vector3.right) * 5;
		Gizmos.DrawRay(transform.position, direction);
	}
	
	bool IsHitClient = false;
	bool isRemoved = false;
	
	private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this
	private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this

	//Update is called once per frame
	void Update()
	{
		if(Application.loadedLevel < (int)GameLeve.Leve3)
		{
			Destroy(gameObject);
			return;
		}

		if(isRemoved)
		{
			return;
		}

		if(PlayerCreatNet.IsDisconnected)
		{
			return;
		}

		if(ownerPlayer != null & Network.player == ownerPlayer)
		{
			if(Network.isServer)
			{
//				if(Time.frameCount % 200 == 0)
//				{
//					Debug.Log("test carNet server**********************");
//				}
				
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
//				if(Time.frameCount % 200 == 0)
//				{
//					Debug.Log("test carNet client**********************");
//				}
				
				float dis = Vector3.Distance(transform.position, correctPlayerPos);
				if(dis > 0.1f && IsHitClient)
				{
					networkView.RPC ("sendDaoJuTranInfoToServer", RPCMode.Server,
					                 transform.position, transform.rotation);
				}
			}
		}
	}

	void removeRock()
	{
		//isStart = false;
		isRemoved = true;
		
		networkView.RPC("sendToServerRemoveRock", RPCMode.OthersBuffered);
		Invoke("delayRemoveNetRock", 2.5f);
	}
	
	void delayRemoveNetRock()
	{
		Network.RemoveRPCs(networkView.viewID);
		Network.Destroy(gameObject);
	}
	
	[RPC]
	void sendToServerRemoveRock()
	{
		isRemoved = true;
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
	
	[RPC]
	void sendDaoJuTranInfoToServer(Vector3 pos, Quaternion rot)
	{
		correctPlayerPos = pos;
		correctPlayerRot = rot;
	}
}
