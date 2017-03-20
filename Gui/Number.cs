using UnityEngine;
using System.Collections;

public class Number : MonoBehaviour {
	public GameObject qian;
	public GameObject bai;
	public GameObject shi;
	public GameObject ge;
	[HideInInspector]
	public string PR="df";
	private  UISprite qianUi;

	private UISprite baiUi;

	private UISprite shiUi;

	private UISprite geUi;
	void Awake()
	{
		qianUi=qian.GetComponent<UISprite>();
		//Debug.Log(qianUi.spriteName+"ssssss2");
		baiUi=bai.GetComponent<UISprite>();
		shiUi=shi.GetComponent<UISprite>();
		geUi=ge.GetComponent<UISprite>();
	}
	public void intToImage(int d)
	{
		int q,b,s,g;
		q=d/1000;
		b=(d%1000)/100;
		s=(d%100)/10;
		g=(d%10);
		if(q>0)
		{
			UpdateUI(qian,qianUi,true,PR,q);
		}
		else
		{
			
			UpdateUI(qian,qianUi,false,PR,q);
		}
		if(b>0||q>0)
		{
			UpdateUI(bai,baiUi,true,PR,b);
		}
		else if(q==0)
		{
			UpdateUI(bai,baiUi,false,PR,b);
		}
		if(s>0||b>0||q>0)
		{
			UpdateUI(shi,shiUi,true,PR,s);
		}
		else if(q==0&&b==0)
		{
			UpdateUI(shi,shiUi,false,PR,s);
		}
		
		UpdateUI(ge,geUi,true,PR,g);
		
	}
	private void UpdateUI(GameObject go, UISprite ui,bool enable,string s,int num)
	{
		//Debug.Log(s+"sssssssssssssssss");
		if (enable) {
			if (go != null)
				go.SetActive (true);
			if (ui != null)
				ui.spriteName = s + num;
			//Debug.Log(ui.spriteName+"3333333");
		} 
		else 
		{
			if(go!=null)
			{
				go.SetActive(false);
			}
		}

	}

}
