using UnityEngine;
using System.Collections;

public class CarNetCtrl : MonoBehaviour {
	
	public NetworkPlayer ownerPlayer;

	public GameObject CarPath;
	public float CarSpeed = 5.0f;

	public  bool isStart = false;
	public float minDistance = 3.0f;

	private Vector3[] markerPos;
	private int  nextMarker = 0;

	bool IsHitClient = false;
	bool isRemoved = false;
	
	private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this
	private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this

	// Use this for initialization
	void Start () {
		
		correctPlayerPos = transform.position;
		correctPlayerRot = transform.rotation;
	}

	public void initCarNetInfo(GameObject path)
	{
		CarPath = path;
		isStart = true;
		IsHitClient = true;

		InitPathMark();
	}

	public void InitPathMark()
	{
		int markerCount = CarPath.transform.childCount;
		markerPos = new Vector3[markerCount];
		//Debug.Log(markerCount);

		Vector3 pos = Vector3.zero;
		for(int index = 0; index < markerCount; index++)
		{
			Transform tran = CarPath.transform.GetChild(index);
			pos.Set(tran.position.x, tran.position.y, tran.position.z);
			markerPos[index] = pos;
		}
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

		if(isStart)
		{
			float dis = Vector3.Distance(transform.position, markerPos[nextMarker]);
			//Debug.Log("next="+nextMarker+"dis="+dis+";x="+markerPos[nextMarker].x+";Y="+markerPos[nextMarker].y+";z="+markerPos[nextMarker].z);
			if(dis < minDistance && nextMarker < CarPath.transform.childCount)
			{
				nextMarker++;
			}

			if(nextMarker >= markerPos.Length)
			{
				nextMarker--;

				//remove the car
				removeCar();
			}
			else
			{
				//transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(markerPos[nextMarker] - transform.position), Time.deltaTime);
				transform.forward = Vector3.Normalize(markerPos[nextMarker] - transform.position);
				transform.Translate(Vector3.forward * Time.deltaTime * CarSpeed);
			}
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

	void removeCar()
	{
		isStart = false;
		isRemoved = true;

		networkView.RPC("sendToServerRemoveCar", RPCMode.OthersBuffered);
		Invoke("delayRemoveNetCar", 0.5f);
	}

	void delayRemoveNetCar()
	{
		Network.RemoveRPCs(networkView.viewID);
		Network.Destroy(gameObject);
	}

	[RPC]
	void sendToServerRemoveCar()
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
