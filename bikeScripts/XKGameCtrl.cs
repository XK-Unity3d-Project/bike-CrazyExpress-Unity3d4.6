using UnityEngine;
using System.Collections;

public class XKGameCtrl : MonoBehaviour {
	bool IsActiveGameInfo;
	// Update is called once per frame
	void Update()
	{
		if (!pcvr.bIsHardWare || pcvr.IsTestGetInput) {
			if (Input.GetKeyUp(KeyCode.S)) {
				IsActiveGameInfo = !IsActiveGameInfo;
			}
		}
	}

	void OnGUI()
	{
		if (!IsActiveGameInfo) {
			return;
		}

		string infoA = "QN-1 " + pcvr.QiNangArray[0]
						+ ", QN-2 " + pcvr.QiNangArray[1]
						+ ", QN-3 " + pcvr.QiNangArray[2]
						+ ", QN-4 " + pcvr.QiNangArray[3];
		GUI.Box(new Rect(0f, 0f, Screen.width * 0.4f, 20f), infoA);

		string infoB = "ZuLiInfo " + pcvr.mBikeZuLiInfo;
		GUI.Box(new Rect(0f, 20f, Screen.width * 0.4f, 20f), infoB);
	}
}