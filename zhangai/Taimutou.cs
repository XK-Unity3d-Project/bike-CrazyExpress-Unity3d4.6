using UnityEngine;
using System.Collections;

public class Taimutou : MonoBehaviour {
	public GameObject CarPath;
	public float CarSpeed=5.0f;
	private Vector3[] markerPos;
	private int  nextMarker=0;
	private  bool isStart=false;
	public float minDistance=3.0f;
	//public GameObject mutou;
	public GameObject zhuangmutouTrigger;
	private Animator animator;
//	private AnimatorStateInfo stateInfo;
	public GameObject StartTrigger;
	//public GameObject player;
	// Use this for initialization
//	void Awake()
//	{
//		
//	}

	/// <summary>
	/// 撞木头后触发
	/// </summary>
	public void TriggerEnter()
	{
		animator.SetBool("zhuang",true);
		zhuangmutouTrigger.transform.parent = null;
		zhuangmutouTrigger.GetComponent<Rigidbody>().isKinematic=false;
		zhuangmutouTrigger.GetComponent<BoxCollider>().isTrigger=false;
		zhuangmutouTrigger.layer = LayerMask.NameToLayer("Default");
		//gameObject.layer = LayerMask.NameToLayer("Default");
		Invoke("delayChangeLayer", 0.5f);

		CarSpeed*=2;
	}

	void delayChangeLayer()
	{
		gameObject.layer = LayerMask.NameToLayer("Default");
	}

	//must freeze position and rotation
	void Start()
	{
		if(StartTrigger)
		{
			var	Sts=StartTrigger.GetComponent<TriggerScript>();
			Sts.TriggerEnter+=StartTriggerEnter;
		}

//		if(zhuangmutouTrigger)
//		{
//		    TriggerScript Ts;
//			Ts=zhuangmutouTrigger.GetComponent<TriggerScript>();
//			Ts.TriggerEnter+=TriggerEnter;
//		}

		//获取animator
		animator= GetComponent<Animator>();
//		if(animator!=null)
//		{
//			stateInfo= animator.GetCurrentAnimatorStateInfo(0);
//		}
		InitPathMark();
	}

	public void StartTriggerEnter()
	{
		isStart=true;
	}

	bool isFallState = false;
	void resetIsFallState()
	{
		isFallState = false;
	}

	/// <summary>
	/// 撞人后触发
	/// </summary>
	void OnCollisionEnter(Collision collision) {
		if(GlobalData.GetInstance().gameMode == GameMode.OnlineMode)
		{
			return;
		}

		bike bikeScript = collision.transform.GetComponent<bike>();
		if(bikeScript != null && !bikeScript.GetIsAiNPC())
		{
			isFallState = true;
			Invoke("resetIsFallState", 1.0f);

			Vector3 v=	transform.InverseTransformPoint(collision.transform.position);
			if(v.x>0)
			{
				//Debug.Log("you");
				animator.SetBool("zuoshuai",true);
			}
			else if(v.x<0)
			{
				//Debug.Log("zuo");
				animator.SetBool("youshuai",true);
			}

			BoxCollider boxCol = GetComponent<BoxCollider>();
			boxCol.isTrigger = true;

			zhuangmutouTrigger.transform.parent = null;
			zhuangmutouTrigger.GetComponent<Rigidbody>().isKinematic=false;
			zhuangmutouTrigger.GetComponent<BoxCollider>().isTrigger=false;
		}
	}

	public void InitPathMark()	
	{
		int markerCount=CarPath.transform.childCount;
		markerPos=new Vector3[markerCount];
		//Debug.Log("InitPathMark -> markerCount " + markerCount);

		Vector3 pos=Vector3.zero;
		for(int index=0;index<markerCount;index++)
		{
			Transform tran=CarPath.transform.GetChild(index);
			pos.Set(tran.position.x,tran.position.y,tran.position.z);
			markerPos[index]=pos;
		}
		
	}
	void Update ()
	{
		if(isFallState)
		{
			return;
		}

		if(isStart)
		{
			float dis=Vector3.Distance(transform.position,markerPos[nextMarker]);
		    //Debug.Log("next="+nextMarker+"dis="+dis+";x="+markerPos[nextMarker].x+";Y="+markerPos[nextMarker].y+";z="+markerPos[nextMarker].z);
			if(dis < minDistance && nextMarker < CarPath.transform.childCount)
			{
				nextMarker++;
				
			}

			if(nextMarker >= markerPos.Length)
			{
				//Debug.Log("delete taiMuTou npc...");
				nextMarker--;
				zhuangmutouTrigger.transform.parent = transform;
				Destroy(gameObject);
			}
			else
			{
				//transform.rotation= Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(markerPos[nextMarker]-transform.position),Time.deltaTime);
				transform.forward = Vector3.Normalize(markerPos[nextMarker] - transform.position);
				transform.Translate(Vector3.forward*Time.deltaTime*CarSpeed);
			}
		}
	}
}
