using UnityEngine;
using System.Collections;

public class XKGameVersionCtrl : MonoBehaviour {
	UILabel VersionLB;
	public static string GameVersion = "Version: 1.3.1_Com_20170330";
	// Use this for initialization
	void Start()
	{
		VersionLB = GetComponent<UILabel>();
		VersionLB.text = GameVersion;
	}
}