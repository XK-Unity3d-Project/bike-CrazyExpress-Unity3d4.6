using UnityEngine;
using System.Collections;

using Frederick.ProjectAircraft;

public class GameOver : MonoBehaviour {
	public AudioClip GameOverAudio = null;
	public GameObject go;
	public float time;
	// Use this for initialization
	void Start () {
		//go.SetActive(true);
		GameoverToCaipiao();

		AudioSource AudioSourceObj = AudioManager.Instance.audio;
		AudioSourceObj.clip = GameOverAudio;
		AudioSourceObj.Play();
	}

	public void GameoverToCaipiao()
	{
		Invoke("Show",time);
	}

	public void Show()
	{
		go.SetActive(false);
		GlobalScript.GetInstance().player.IsPass=true;
		pcvr.ResetBikeZuLiInfo();
		Rank.GetInstance().HiddenRankList();

		pcvr.GetInstance().HandleBikeHeadQiFu(BikeHeadMoveState.PLANE, 0.0f, 0.0f);
		//ScreenLog.Log("GameOver ******************** test");
	}
}
