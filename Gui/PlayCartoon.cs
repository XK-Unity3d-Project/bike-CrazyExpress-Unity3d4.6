using UnityEngine;
using System.Collections;

public class PlayCartoon : MonoBehaviour {
	//public GameObject carttoon;

	public void StartPlay()
	{
		StartCoroutine("Play");
		GlobalData.GetInstance().playCartoonEnd=false;
	}


}
