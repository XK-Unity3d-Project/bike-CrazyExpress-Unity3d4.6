using UnityEngine;
using System.Collections;

public class Chache : MonoBehaviour {
	public GameObject CarPath;
	public float CarSpeed=5.0f;
	private Vector3[] markerPos;
	private int  nextMarker=0;
	private  bool isStart=false;
	public float minDistance=3.0f;
	public GameObject StopTrigger;
	private TriggerScript Sts;
	private TriggerScript Ts;
	public GameObject mutou;
	public GameObject StartTrigger;
	public bool IsChache;
	// Use this for initialization
	void Awake()
	{

	}
	public void TriggerEnter()
	{
	  Animator animator=GetComponent<Animator>();
		if(animator!=null){
			animator.SetBool("shache",true);
		
		}
		if(mutou)
		{
			for(int i=0;i<mutou.transform.childCount;i++)
			{
				Rigidbody r = mutou.transform.GetChild(i).gameObject.GetComponent<Rigidbody>();
				if(r == null)
				{
					r = mutou.transform.GetChild(i).gameObject.AddComponent<Rigidbody>();
				}

				r.mass=100;
				if(IsChache)
				{
					r.velocity=transform.forward*5;
				}
			}
		}
		isStart=false;
		//Debug.Log("ssssssssssssssssssssssssss");
	}
	void Start () {
		if(StartTrigger)
		{
			Sts=StartTrigger.GetComponent<TriggerScript>();
			Sts.TriggerEnter+=StartTriggerEnter;
		}
		if(StopTrigger)
		{
			Ts=StopTrigger.GetComponent<TriggerScript>();
			Ts.TriggerEnter+=TriggerEnter;
		}
	
		InitPathMark();
	//	animation.Play("Run");
		//state.enabled=true;
		//gameObject.animation.CrossFade("Run");
		//	    TriggerScript ts=(TriggerScript)EnterTrigger.GetComponent("TriggerScript");	
		//        ts.TriggerEnter+=CarTriggered;
	}
	public void StartTriggerEnter()
	{
		//Debug.Log("55555555555555555555555");
		isStart=true;
	}
	public void InitPathMark()
		
	{
		int markerCount=CarPath.transform.childCount;
		markerPos=new Vector3[markerCount];
		//Debug.Log(markerCount);
		Vector3 pos=Vector3.zero;
		for(int index=0;index<markerCount;index++)
		{
			Transform tran=CarPath.transform.GetChild(index);
			pos.Set(tran.position.x,tran.position.y,tran.position.z);
			markerPos[index]=pos;
		}
		
	}
	void Update () {
		if(isStart)
		{
			float dis=Vector3.Distance(transform.position,markerPos[nextMarker]);
			//Debug.Log("next="+nextMarker+"dis="+dis+";x="+markerPos[nextMarker].x+";Y="+markerPos[nextMarker].y+";z="+markerPos[nextMarker].z);
			if(dis<minDistance&&nextMarker<CarPath.transform.childCount)
			{
				nextMarker++;
				
			}
			if(nextMarker>=markerPos.Length)
			{
				nextMarker--;
				//Destroy(gameObject);
			}
			else
			{
				transform.rotation= Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(markerPos[nextMarker]-transform.position),Time.deltaTime);
				transform.Translate(Vector3.forward*Time.deltaTime*CarSpeed);
			}
		}
	}
}
