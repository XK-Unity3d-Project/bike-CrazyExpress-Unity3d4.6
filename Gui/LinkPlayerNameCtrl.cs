using UnityEngine;
using System.Collections;

public class LinkPlayerNameCtrl : MonoBehaviour {
	public TweenAlpha[] NameAlphaArray;
	public static int IndexPlayerVal = -1;
	static LinkPlayerNameCtrl _Instance;
	public static LinkPlayerNameCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Awake()
	{
		_Instance = this;
		int max = NameAlphaArray.Length;
		for (int i = 0; i < max; i++) {
			NameAlphaArray[i].enabled = false;
		}
		IndexPlayerVal = -1;
		gameObject.SetActive(false);
	}

	public void ActivePlayerInfo()
	{
		gameObject.SetActive(true);
		if (IndexPlayerVal != -1) {
			ActivePlayerNameAlpha(IndexPlayerVal);
		}
	}
	
	public void HiddenPlayerInfo()
	{
		gameObject.SetActive(false);
	}

	public void ActivePlayerNameAlpha(int indexVal)
	{
		if (!gameObject.activeSelf) {
			IndexPlayerVal = indexVal;
			return;
		}
		NameAlphaArray[indexVal].enabled = true;
		NameAlphaArray[indexVal].ResetToBeginning();
		NameAlphaArray[indexVal].PlayForward();
	}
}