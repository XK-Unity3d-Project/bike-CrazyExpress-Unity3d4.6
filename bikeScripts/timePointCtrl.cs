using UnityEngine;
using System.Collections;

public class timePointCtrl : MonoBehaviour {

	private string [] playerName = null;

	public bool checkPlayerName(string name)
	{
		for(int i = 0; i < 8; i++)
		{
			if(playerName[i] == null)
			{
				playerName[i] = name;
				break;
			}
			else if(playerName[i] == name)
			{
				return false;
			}
		}

		return true;
	}

	// Use this for initialization
	void Start ()
	{
		if(playerName == null || playerName.Length == 0)
		{
//			Debug.Log("init timePointCtrl...");
			playerName = new string[8];
			timePointCtrl script = gameObject.GetComponent<timePointCtrl>();
			script.enabled = false;
		}
	}
}
