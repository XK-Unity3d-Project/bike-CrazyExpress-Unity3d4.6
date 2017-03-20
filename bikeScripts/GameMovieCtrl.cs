using UnityEngine;
using System.Collections;
using Frederick.ProjectAircraft;

public class GameMovieCtrl : MonoBehaviour {
	
	public MovieTexture move;
	public MovieTexture moveServer;
	public MovieTexture moveServerNew;
	static bool isStopMovie = false;
	AudioSource AudioSourceObj;
	static GameMovieCtrl _instance;
//	string AudioCtrl = "_AudioManager";
	GameObject AudioManagerObj;

	// Use this for initialization
	void Start()
	{
		if (_instance == null) {
			_instance = this;
			DontDestroyOnLoad( gameObject );
			PlayMovie();
		}
	}

	public static GameMovieCtrl GetInstance()
	{
		return _instance;
	}

	public void PlayMovie()
	{
		renderer.enabled = true;
		//FreeModeCtrl.IsServer = true; //test.
		if(FreeModeCtrl.IsServer)
		{
			if (FreeModeCtrl.ServerScreenW != 800) {
				//moveServer = move;
				moveServer = moveServerNew;
			}

			renderer.material.mainTexture = moveServer;
			moveServer.loop = true;
			moveServer.Play();
		}
		else
		{
			renderer.material.mainTexture = move;
			move.loop = true;
			move.Play();
		}

		if(AudioSourceObj == null)
		{
			AudioSourceObj = transform.GetComponent<AudioSource>();
		}

		if(FreeModeCtrl.IsServer)
		{
			AudioSourceObj.clip = moveServer.audioClip;
			AudioSourceObj.enabled = false;
			AudioSourceObj.Stop();
		}
		else
		{
			AudioSourceObj.clip = move.audioClip;
			AudioSourceObj.enabled = true;
			AudioSourceObj.Play();
		}
	}

	// Update is called once per frame
	void Update()
	{
		if(AudioSourceObj == null)
		{
			AudioSourceObj = transform.GetComponent<AudioSource>();
		}

		if(AudioSourceObj.enabled && Network.isServer)
		{
			AudioSourceObj.enabled = false;
		}

		if(isStopMovie)
		{
			transform.parent = null;
			if(FreeModeCtrl.IsServer)
			{
				moveServer.Stop();
			}
			else
			{
				move.Stop();
			}

			renderer.enabled = false;
			isStopMovie = false;

			if(AudioSourceObj == null)
			{
				AudioSourceObj = transform.GetComponent<AudioSource>();
			}
			AudioSourceObj.enabled = false;
		}
	}

	public static void stopPlayMovie()
	{
		isStopMovie = true;
	}
}