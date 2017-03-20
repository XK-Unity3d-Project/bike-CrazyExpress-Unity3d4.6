using UnityEngine;
using System.Collections;

public class wangqiu : MonoBehaviour {
	public GameObject wq1;
	public GameObject wq2;
	public GameObject wq3;
	private TweenScale scale;
	public GameObject ButtomRight;
	private TweenPosition pos;
	public float duration=0.5f;
	private BufferKind kind;
	private UISprite ui;
	private bool isFinished=false;

	private Transform temp;
	void Awake()
	{
		ui=GetComponent<UISprite>();
		scale=gameObject.GetComponent<TweenScale>();
		pos=gameObject.GetComponent<TweenPosition>();
//		GlobalScript.GetInstance().FireTennisEvent+=FireTennisEvent;
		temp=transform.parent.transform;
		//pos.from=Vector3.zero;
	}
	void FireTennisEvent()
	{
		if(!isFinished)
		{
			AddTennis();
		}
	}

	public void  AddBuffer(BufferKind kind)
	{
		isFinished=false;
		if(!gameObject.activeSelf)
		{
			gameObject.SetActive(true);
		}
		scale.ResetToBeginning();
		pos.ResetToBeginning();
		this.kind=kind;
	
		StartCoroutine("PlayAnimor");
	}

	public IEnumerator PlayAnimor()
	{
	
		ShowImage(1);
		yield return new WaitForSeconds(duration);
		ShowImage(2);
		yield return new WaitForSeconds(duration);
		ShowImage(3);
		yield return new WaitForSeconds(duration);
		if(kind!=BufferKind.Wangqiu)
		{
		gameObject.SetActive(false);
		}
		else
		{
			AddTennis();
		}
	}
	private void  ShowImage(int i)
	{
		//string s=kind.ToString();
//		switch(s)
//		{
//		case BufferKind.Wangqiu:
			updateUI("wq",i);
//			break;
//		case BufferKind.Hanbao:
//			updateUI("hb",i);
//			break;
//		case BufferKind.Jitui:
//			updateUI("jt",i);
//			break;
//		case BufferKind.Shoubiao:
//			updateUI("sb",i);
//			break;
//		case BufferKind.Dianchi:
//			updateUI("dc",i);
//			break;
//		}
	}
	public void updateUI(string str,int i)
	{
		if(ui!=null)
		{
			ui.spriteName=str+i;
		}
	}
	private void AddTennis()
	{
		pos.onFinished.Clear();
		if(GlobalScript.GetInstance().player.TennisCount==0)
		{
			pos.to=temp.InverseTransformPoint( wq1.transform.position);
			EventDelegate.Add(pos.onFinished,delegate {
				GlobalScript.GetInstance().player.Addtennis();
				updataTennisUI();
				gameObject.SetActive(false);
				isFinished=true;
			});
		}
		else if(GlobalScript.GetInstance().player.TennisCount==1)
		{
			pos.to=temp.InverseTransformPoint( wq2.transform.position);
			EventDelegate.Add(pos.onFinished,delegate {
				GlobalScript.GetInstance().player.Addtennis();
				updataTennisUI();
				gameObject.SetActive(false);
				isFinished=true;
			});
		}
		else if(GlobalScript.GetInstance().player.TennisCount==2)
		{
			pos.to=temp.InverseTransformPoint( wq3.transform.position);
			EventDelegate.Add(pos.onFinished,delegate {
				GlobalScript.GetInstance().player.Addtennis();
				updataTennisUI();
				gameObject.SetActive(false);
				isFinished=true;
			});
		}
		else if(GlobalScript.GetInstance().player.TennisCount>2)
		{
			pos.to=temp.InverseTransformPoint( wq3.transform.position);
			EventDelegate.Add(pos.onFinished,delegate {

				updataTennisUI();
				gameObject.SetActive(false);
				isFinished=true;
			});
		}

		scale.PlayForward();
		pos.PlayForward();
	}

	public void updataTennisUI()
	{
//		Debug.Log("remove");
		if(GlobalScript.GetInstance().player.TennisCount==2)
		{
			wq1.SetActive(true);
			wq2.SetActive(true);
			wq3.SetActive(false);
		}
		else if(GlobalScript.GetInstance().player.TennisCount==1)
		{
			wq1.SetActive(true);
			wq2.SetActive(false);
			wq3.SetActive(false);
		}
		else if(GlobalScript.GetInstance().player.TennisCount==0)
		{
//			Debug.Log(GlobalScript.GetInstance().player.TennisCount);
			wq3.SetActive(false);
			wq2.SetActive(false);
			wq1.SetActive(false);
		}
		else
		{
			wq3.SetActive(true);
			wq2.SetActive(true);
			wq1.SetActive(true);
		}
	}
}

public enum BufferKind :int
{
	Hanbao=110,
	Jitui=110,
	Dianchi=100,
	Wangqiu=1,
	Shoubiao=10,
	BaoGuo=400,
	Jiashidian=60
}