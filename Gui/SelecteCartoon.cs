using UnityEngine;
using System.Collections;

public class SelecteCartoon : MonoBehaviour {

	public UISprite SelecteSprite;
	int count = 0;

	void Start()
	{
		InvokeRepeating("PlayCartoon", 0.0f, 0.5f);
	}
	
	void PlayCartoon()
	{
		count++;
		if(count >= 5)
		{
			count = 1;
		}
		SelecteSprite.spriteName = count.ToString();
	}
}
