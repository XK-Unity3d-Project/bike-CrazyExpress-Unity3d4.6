using UnityEngine;
using System.Collections;

public class GuidanceImg : MonoBehaviour {
	public float KeepTime;
	public int frameRate;
	public GameObject DianchiGuidance;
	private UISprite dianchiUI;
	public GameObject WangqiuGuidance;
	private UISprite wangqiuUI;
	// Use this for initialization
	void Start () {
		GlobalScript.GetInstance().player.showDianchiGuidanceEvent+=showDianchiGuidanceEvent;
		GlobalScript.GetInstance().player.showWangqiuGuidanceEvent+=showWangqiuGuidanceEvent;
		wangqiuUI=WangqiuGuidance.GetComponent<UISprite>();
		dianchiUI=DianchiGuidance.GetComponent<UISprite>();
	}
	private void showDianchiGuidanceEvent()
	{
	 
		dianchiUI.enabled=true;
		StartCoroutine(Keep("dianchi"));
	}
	public IEnumerator Keep(string name)
	{
		yield return new WaitForSeconds(2);
		if(name=="dianchi")
		dianchiUI.enabled=false;
		else
			wangqiuUI.enabled=false;
	}
	private void showWangqiuGuidanceEvent()
	{
		wangqiuUI.enabled=true;
		StartCoroutine(Keep("wangqiu"));
	}
}
