using UnityEngine;
using System.Collections;
using Frederick.ProjectAircraft;
public class backgroundMusic : MonoBehaviour {
	public AudioClip music;
	public bool IsLoop;
	// Use this for initialization
	void Start () {
		AudioManager.Instance.PlayBGM (music, IsLoop);
	}
	
	// Update is called once per frame
//	void Update () {
//	
//	}
}
