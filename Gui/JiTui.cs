using UnityEngine;
using System.Collections;
using System.Linq;
public class JiTui : MonoBehaviour {
	public GameObject DianChiPrefab;
	public GameObject ShouBiaoPrefab;

    private GameObject jitui;
	private bool isStart;
//	public GameObject target;
	public GameObject UICam;
	//public GameObject JICam;
//	public float TTime;
//	public Tmoder mode;
	private float Vspeed;
	private float Hspeed;
	private Vector3 TargetLoaclPosition;
	private float a;
	private int frameCount=0;
	private float x=0;
	private float y=0;
	private float MaxX;
	public NMB[] Targets;
//	private HighlightingEffect effectScriptP = null;
//	private HighlightingEffect effectScript = null;
	private Tmoder moder;
	private BufferKind kind;

	public void StartTransform(GameObject jituiObj, BufferKind kind)
	{
		if(GlobalData.GetInstance().gameMode == GameMode.OnlineMode)
		{
			//ScreenLog.Log("delete daoJu -> name " + jituiObj.name);
			//			DaoJuNetCtrl DaoJuNetCtrlScript = jitui.GetComponent<DaoJuNetCtrl>();
			//			if(DaoJuNetCtrlScript != null)
			//			{
			//				ScreenLog.Log("**********delete daoJu -> name " + jitui.name);
			//				DaoJuNetCtrlScript.closeDaoJuServer();
			//			}
			//			else
			//			{
			//				ScreenLog.LogWarning("DaoJuNetCtrlScript is null, daoJu -> name " + jitui.name);
			//			}
			
			GameObject newDaoJu = null;
			switch(kind)
			{
			case BufferKind.Dianchi:
				newDaoJu = (GameObject)Instantiate(DianChiPrefab);
				break;
				
			case BufferKind.Shoubiao:
				newDaoJu = (GameObject)Instantiate(ShouBiaoPrefab);
				break;
			}
			
			if(newDaoJu != null)
			{
				newDaoJu.transform.parent = transform;
				newDaoJu.transform.localPosition = Vector3.zero;
			}
			
			isStart=false;
			return;
		}

		this.jitui = jituiObj;
		x=0;
		y=0;

		var s=from q in Targets where q.kind==kind select q;
		if(s.Count()>0)
		{
			//ScreenLog.Log("*******************test kind " + kind.ToString());
			NMB nmb=s.First();
			moder=nmb.Moder;
			this.kind=kind;
			Vector3 v1=UICam.camera.WorldToScreenPoint(nmb.Tareget.transform.position);
			Vector3 v2=gameObject.camera.ScreenToWorldPoint(v1);
			Vector3 TargetLoaclPosition=gameObject.transform.InverseTransformPoint(v2);
			Hspeed=(TargetLoaclPosition.x-jitui.transform.localPosition.x)/nmb.time;
			Vspeed=(TargetLoaclPosition.y-jitui.transform.localPosition.y)/nmb.time;
			MaxX=TargetLoaclPosition.x;
			if(nmb.Moder==Tmoder.Parabola)
			{
				a=(TargetLoaclPosition.y-jitui.transform.localPosition.y)*2*Time.fixedDeltaTime*Time.fixedDeltaTime/(nmb.time*nmb.time);
			}
		}

		isStart=true;
	}

	// Update is called once per frame
//	void Update () {
//	
//	
//	}
	void FixedUpdate()
	{
		if(isStart)
		{
			if(Mathf.Abs(MaxX) < Mathf.Abs(jitui.transform.localPosition.x))
			{
				Destroy(jitui);

				GlobalScript.GetInstance().player.AddBuffer(kind);
				isStart=false;
				return;
			}
			x=Hspeed*Time.deltaTime;
			
			if(moder==Tmoder.Parabola&&a!=0)
			{
				frameCount++;
				y=y+a;
				//ScreenLog.Log("yyyyyyyyy"+y);
			}
			else 
			{
				y=Vspeed*Time.deltaTime;
			}
			jitui.transform.localPosition = jitui.transform.localPosition + new Vector3(x,y,0);
		}
	}
}

[System.Serializable]
public class NMB
{

	public GameObject Tareget;
	public Tmoder Moder;
	public BufferKind kind;
	public float time;
}

public enum Tmoder
{
	Parabola,
	Line
}
