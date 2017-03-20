using UnityEngine;
using System.Collections;

public class EndPage : MonoBehaviour {
	public bool isStart;
	private Vector3 dre;
	public float duation=1;
	private Vector3 roa;

	void Awake()
	{
		GlobalScript.GetInstance ().ShowEndPageEvent += ShowEndPageEvent;
	}

	void ShowEndPageEvent()
	{
		isStart = true;
	}


	
	// Update is called once per frame
	void Update () {
		if(isStart)
		{
			dre = new Vector3 (0, 0, -622) - transform.localPosition;
			roa = Vector3.zero-transform.localEulerAngles;
		
			transform.localPosition+=(dre * Time.deltaTime/duation);
			transform.localEulerAngles+=(roa* Time.deltaTime/duation);
			//Debug.Log(dre.x+"y"+dre.y+"z"+dre.z);
			if(dre.magnitude<1)
			{
				isStart=false;
				GlobalScript.GetInstance().ShowCaipiao();
				//GlobalScript.GetInstance().ShowFinalRank();
			}
			//isStart=false;
		}
	}
}
