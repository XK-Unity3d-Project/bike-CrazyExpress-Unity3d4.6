using UnityEngine;
using System.Collections;

public class KuangCtrl : MonoBehaviour {

	private GameObject duiHaoObj = null;
	private UILabel uiLabelScript = null;

	// Use this for initialization
	void Awake () {
		if(transform.childCount > 0)
		{
			duiHaoObj = transform.GetChild(0).gameObject;
			if(duiHaoObj)
			{
				duiHaoObj.SetActive(false);
			}
		}

		uiLabelScript = GetComponent<UILabel>();
		if(uiLabelScript != null)
		{
			uiLabelScript.depth = 0;
		}
	}
}
