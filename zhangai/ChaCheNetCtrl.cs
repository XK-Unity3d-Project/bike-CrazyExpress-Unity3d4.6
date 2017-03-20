using UnityEngine;
using System.Collections;

public class ChaCheNetCtrl : MonoBehaviour {

	public NetworkPlayer ownerPlayer;
	
	public GameObject CarPath;
	public float CarSpeed = 5.0f;

	private Vector3[] markerPos;
	private int  nextMarker = 0;
	private  bool isStart = false;

	public float minDistance = 3.0f;
	public GameObject StopTrigger;

	private TriggerScript Sts;
	private TriggerScript Ts;

	public GameObject mutou;
	public GameObject StartTrigger;
	public bool IsChache;
	
	bool IsHitClient = false;
	bool isRemoved = false;
	
	private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this
	private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this

	// Use this for initialization
//	void Awake()
//	{
//		
//	}

	public void TriggerEnter()
	{
		Animator animator=GetComponent<Animator>();
		if(animator!=null){
			animator.SetBool("shache", true);
		}

		if(mutou)
		{
			for(int i = 0; i < mutou.transform.childCount; i++)
			{
				Rigidbody r = mutou.transform.GetChild(i).gameObject.GetComponent<Rigidbody>();
				if(r == null)
				{
					r = mutou.transform.GetChild(i).gameObject.AddComponent<Rigidbody>();
				}
				
				r.mass = 100;
				if(IsChache)
				{
					r.velocity = transform.forward * 5.0f;
				}
			}
		}

		isStart = false;
		isRemoved = true;
		//Debug.Log("ssssssssssssssssssssssssss");
	}

	void Start () {
		
		correctPlayerPos = transform.position;
		correctPlayerRot = transform.rotation;

		if(StartTrigger)
		{
			Sts = StartTrigger.GetComponent<TriggerScript>();
			Sts.TriggerEnter += StartTriggerEnter;
		}

		if(StopTrigger)
		{
			Ts = StopTrigger.GetComponent<TriggerScript>();
			Ts.TriggerEnter += TriggerEnter;
		}
		
		InitPathMark();
	}

	public void StartTriggerEnter()
	{
		//Debug.Log("55555555555555555555555");
		isStart = true;
		IsHitClient = true;
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

	void Update ()
	{
		if(PlayerCreatNet.IsDisconnected)
		{
			return;
		}

		if(isRemoved)
		{
			return;
		}

		if(isStart)
		{
			float dis=Vector3.Distance(transform.position,markerPos[nextMarker]);
			//Debug.Log("next="+nextMarker+"dis="+dis+";x="+markerPos[nextMarker].x+";Y="+markerPos[nextMarker].y+";z="+markerPos[nextMarker].z);
			if(dis<minDistance&&nextMarker<CarPath.transform.childCount)
			{
				nextMarker++;
				
			}

			if(nextMarker >= markerPos.Length)
			{
				nextMarker--;

				//remove the chaChe
				removeCar();
			}
			else
			{
				transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(markerPos[nextMarker]-transform.position), Time.deltaTime);
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
