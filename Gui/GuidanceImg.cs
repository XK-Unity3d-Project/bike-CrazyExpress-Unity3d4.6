using UnityEngine;
using System.Collections;

public class GuidanceImg : MonoBehaviour {
	public float KeepTime;
	public int frameRate;
	public GameObject DianchiGuidance;
	private UISprite dianchiUI;
	public GameObject WangqiuGuidance;
	private UISprite wangqiuUI;
	static GuidanceImg _Instance;
	// Use this for initialization
	void Start () {
		_Instance = this;
		wangqiuUI=WangqiuGuidance.GetComponent<UISprite>();
		wangqiuUI.enabled = false;
		dianchiUI=DianchiGuidance.GetComponent<UISprite>();
		dianchiUI.enabled = false;
		GlobalScript.GetInstance().player.showDianchiGuidanceEvent+=showDianchiGuidanceEvent;
		GlobalScript.GetInstance().player.showWangqiuGuidanceEvent+=showWangqiuGuidanceEvent;
	}
	private void showDianchiGuidanceEvent()
	{
	 
		dianchiUI.enabled=true;
		StartCoroutine(Keep("dianchi"));
	}
	public IEnumerator Keep(string name)
	{
		yield return new WaitForSeconds(2);
		if(name=="dianchi") {
			do {
				yield return new WaitForSeconds(0.2f);
			} while (GlobalScript.GetInstance().player.Energy > 0f);
			dianchiUI.enabled=false;
		}
		else {
			wangqiuUI.enabled=false;
		}
	}
	private void showWangqiuGuidanceEvent()
	{
		wangqiuUI.enabled=true;
		StartCoroutine(Keep("wangqiu"));
	}

	public static void OpenYouMenUI()
	{
		_Instance.showDianchiGuidanceEvent();
	}
}
