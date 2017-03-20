using UnityEngine;
using System.Collections;

public class AddTime1 : MonoBehaviour {
	public GameObject Shiwei;
	public GameObject Gewei;
	public float Duration;
	public float KeepTime;
	private UISprite shiUI;
	private UISprite geUI;

	void Awake()
	{
		shiUI=Shiwei.GetComponent<UISprite>();
		geUI=Gewei.GetComponent<UISprite>();

	}
//	void Start()
//	{
//		AddTimel();
//	}
	public void AddTimel(int time)
	{
		//Debug.Log("addddddddddddddddddddddddddd");
		intToImg(time);
		AddTween(true);
		StopCoroutine("Keep");
		StartCoroutine("Keep");
	}
	private void intToImg(int time)
	{
      if(shiUI!=null)
		{
			shiUI.spriteName="jdjs"+time/10;
		}
		if(geUI!=null)
		{
			geUI.spriteName="jdjs"+time%10;
		}
	}
	private IEnumerator Keep()
	{
		yield return new WaitForSeconds(KeepTime+Duration);
		AddTween(false);

	}
	private void AddTween(bool isadd)
	{
		for(int i=0;i<transform.childCount;i++)
		{
			TweenAlpha 	alpha=transform.GetChild(i).GetComponent<TweenAlpha>();
			//TweenPosition positon=transform;
			if(isadd)
			{
		   
				alpha.duration=Duration;
				alpha.from=0;
				alpha.to=1;
				alpha.ResetToBeginning();
				alpha.PlayForward();
			}
		   else
			{
			
				alpha.duration=Duration;
				alpha.from=1;
				alpha.to=0;
				alpha.ResetToBeginning();
				alpha.PlayForward();
			}
		}
	}
	// Update is called once per frame
//	void Update () {
//	
//	}
}
