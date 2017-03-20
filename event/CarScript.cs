using UnityEngine;
using System.Collections;

public class CarScript : MonoBehaviour {
	//public GameObject EnterTrigger;
	public GameObject CarPath;
	public float CarSpeed=5.0f;
	private Vector3[] markerPos;
	private int  nextMarker=0;
	public  bool isStart=false;
	public float minDistance=3.0f;
	// Use this for initialization

	void Start () {
		InitPathMark();
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

	// Update is called once per frame
	void FixedUpdate () {
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
