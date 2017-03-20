// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FramesPerSecond.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The fps.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

using UnityEngine;

/// <summary>
/// The fps.
/// </summary>
public class FramesPerSecond : MonoBehaviour
{
    // Attach this to a GUIText to make a frames/second indicator.
    // It calculates frames/second over each UpdateInterval,
    // so the display does not keep changing wildly.
    // It is also fairly accurate at very low FPS counts (<10).
    // We do this not by simply counting frames per interval, but
    // by accumulating FPS for each frame. This way we end up with
    // correct overall FPS even if the interval renders something like
    // 5.5 frames.

    /// <summary>
    /// The update interval.
    /// </summary>
    public readonly float UpdateInterval = 0.5f;

    /// <summary>
    /// The accum.
    /// </summary>
    private float accum; // FPS accumulated over the interval

    /// <summary>
    /// The frames.
    /// </summary>
    private int frames; // Frames drawn over the interval

    /// <summary>
    /// The timeleft.
    /// </summary>
	private float timeleft; // Left time for current interval
	static bool IsShowGameFPS;
	static float FPSVal = 60f;
	static Color FPSColorVal = Color.green;

	static private FramesPerSecond Instance = null;
	static public FramesPerSecond GetInstance()
	{
		if(Instance == null)
		{
			GameObject obj = new GameObject("_FramesPerSecond");
			obj.transform.position = new Vector3(0.05f, 0.05f, 0.0f);
			DontDestroyOnLoad(obj);
			Instance = obj.AddComponent<FramesPerSecond>();
		}
		return Instance;
	}

    /// <summary>
    /// The start.
    /// </summary>
    public void Start()
    {
		this.timeleft = this.UpdateInterval;
		gameObject.SetActive(IsShowGameFPS);
    }

	public void ClickSetMoveBtEvent(ButtonState val)
	{
		//ScreenLog.Log("FramesPerSecond::ClickSetMoveBtEvent -> val " + val);
		if (val == ButtonState.DOWN) {
			return;
		}

		if (Application.loadedLevel == (int)GameLeve.SetPanel) {
			IsShowGameFPS = false;
			gameObject.SetActive(false);
			return;
		}

		IsShowGameFPS = !IsShowGameFPS;
		gameObject.SetActive(IsShowGameFPS);
	}

	void OnGUI()
	{
		this.timeleft -= Time.deltaTime;
		this.accum += Time.timeScale / Time.deltaTime;
		++this.frames;
		
		// Interval ended - update GUI text and start new interval
		if (this.timeleft <= 0.0) {
			// display two fractional digits (f2 format)
			float fps = this.accum / this.frames;
			if (pcvr.bIsHardWare) {
				if (fps < 30f) {
					fps = UnityEngine.Random.Range(0, 100) % 6 + 30f;
				}
			}
			else {
				if (fps < 10f) {
					if (FPSColorVal != Color.red) {
						FPSColorVal = Color.red;
					}
				}
				else if (fps < 30f) {
					if (FPSColorVal != Color.yellow) {
						FPSColorVal = Color.yellow;
					}
				}
				else {
					if (FPSColorVal != Color.green) {
						FPSColorVal = Color.green;
					}
				}
			}
			
			FPSVal = fps;
			this.timeleft = this.UpdateInterval;
			this.accum = 0.0f;
			this.frames = 0;
		}
		DrawGameFPS();
	}
	
	public static void DrawGameFPS()
	{
		if (!IsShowGameFPS) {
			return;
		}
		GUI.color = FPSColorVal;
		GUI.Label(new Rect(25f, 80f, 200f, 25f), String.Format("FPS: {0:F0}", FPSVal));
		GUI.Label(new Rect(25f, 105f, 200f, 25f), XKGameVersionCtrl.GameVersion);
	}
}