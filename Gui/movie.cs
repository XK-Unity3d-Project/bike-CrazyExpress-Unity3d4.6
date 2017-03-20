using UnityEngine;
using System.Collections;

public class movie : MonoBehaviour {

	public MovieTexture movieTexture;
	// Use this for initialization
	void Start () 
	{
		renderer.material.mainTexture=movieTexture;
		movieTexture.loop=true;
		movieTexture.Play();
	}
}
