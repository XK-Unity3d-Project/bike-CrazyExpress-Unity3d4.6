using UnityEngine;
using System.Collections;

public class InputEventCtrl : MonoBehaviour {

	static private InputEventCtrl Instance = null;
	public static float[] PlayerFX = new float[4];
	public static float[] PlayerYM = new float[4];
	public static float[] PlayerTB = new float[4]; //编码器.
	public static float[] PlayerSC = new float[4]; //刹车.

	static public InputEventCtrl GetInstance()
	{
		if(Instance == null)
		{
			GameObject obj = new GameObject("_InputEventCtrl");
			//DontDestroyOnLoad(obj);
			Instance = obj.AddComponent<InputEventCtrl>();
			//InputEventCtrlObj = obj;
		}
		return Instance;
	}

	#region Click Button Envent
	public delegate void EventHandel(ButtonState val);
	public event EventHandel ClickStartBtEvent;
	public void ClickStartBt(ButtonState val)
	{
		if(ClickStartBtEvent != null)
		{
			ClickStartBtEvent( val );
			pcvr.IsOpenStartLight = false;
		}
		pcvr.SetIsPlayerActivePcvr();

		if(ClickFireBtEvent != null
		   && (Application.loadedLevel == (int)GameLeve.Leve1
		    || Application.loadedLevel == (int)GameLeve.Leve2
		    || Application.loadedLevel == (int)GameLeve.Leve3
		    || Application.loadedLevel == (int)GameLeve.Leve4))
		{
			if (!HardwareCheckCtrl.IsTestHardWare) {
				pcvr.FireLightState = LedState.Mie;
			}
			ClickFireBtEvent( val );
		}
	}

	public event EventHandel ClickSetEnterBtEvent;
	public void ClickSetEnterBt(ButtonState val)
	{
//		SetEnterBtSt = val;
		if(ClickSetEnterBtEvent != null)
		{
			ClickSetEnterBtEvent( val );
		}
		
//		if (val == ButtonState.DOWN) {
//			TimeSetEnterMoveBt = Time.time;
//		}
		pcvr.SetIsPlayerActivePcvr();
	}

	public event EventHandel ClickSetMoveBtEvent;
	public void ClickSetMoveBt(ButtonState val)
	{
		if(ClickSetMoveBtEvent != null)
		{
			ClickSetMoveBtEvent( val );
		}
		pcvr.SetIsPlayerActivePcvr();
	}

	public event EventHandel ClickFireBtEvent;
	public void ClickFireBt(ButtonState val)
	{
		if(ClickFireBtEvent != null)
		{
			if (!HardwareCheckCtrl.IsTestHardWare) {
				pcvr.FireLightState = LedState.Mie;
			}
			ClickFireBtEvent( val );
		}
		pcvr.SetIsPlayerActivePcvr();
	}
	
	public event EventHandel ClickStopDongGanBtEvent;
	public void ClickStopDongGanBt(ButtonState val)
	{
		if(ClickStopDongGanBtEvent != null)
		{
			ClickStopDongGanBtEvent( val );
		}

		if (val == ButtonState.DOWN) {
			if (DongGanUICtrl.Instance != null) {
				pcvr.DongGanState = (byte)(pcvr.DongGanState == 1 ? 0 : 1);
				DongGanUICtrl.Instance.ShowDongGanUI(pcvr.DongGanState);
			}
		}
		pcvr.SetIsPlayerActivePcvr();
	}
	#endregion
	
//	float TimeSetEnterMoveBt;
//	ButtonState SetEnterBtSt = ButtonState.UP;
	void Update()
	{
//		if (SetEnterBtSt == ButtonState.DOWN && Time.time - TimeSetEnterMoveBt > 2f) {
//			HardwareCheckCtrl.OnRestartGame();
//		}

		if (pcvr.bIsHardWare && !pcvr.IsTestGetInput) {
			return;
		}

		PlayerFX[0] = Input.GetAxis("Horizontal");
		PlayerYM[0] = Input.GetAxis("Vertical");
		if (!pcvr.IsTestBianMaQi) {
			PlayerTB[0] = Input.GetMouseButton(0) == true ? 1f : 0f;
		}
		
		if(Input.GetKeyUp(KeyCode.T)) {
			GlobalData.GetInstance().Icon++;
		}

		if (Input.GetKeyDown(KeyCode.Space)) {
			InputEventCtrl.PlayerSC[0] = 1f;
		}
		
		if (Input.GetKeyUp(KeyCode.Space)) {
			InputEventCtrl.PlayerSC[0] = 0f;
		}

		//StartBt
		if(Input.GetKeyUp(KeyCode.G))
		{
			ClickStartBt( ButtonState.UP );
		}

		if(Input.GetKeyDown(KeyCode.G))
		{
			ClickStartBt( ButtonState.DOWN );
		}

		//setPanel enter button
		if(Input.GetKeyUp(KeyCode.F4))
		{
			ClickSetEnterBt( ButtonState.UP );
		}
		
		if(Input.GetKeyDown(KeyCode.F4))
		{
			ClickSetEnterBt( ButtonState.DOWN );
		}

		//setPanel move button
		if(Input.GetKeyUp(KeyCode.F5))
		{
			ClickSetMoveBt( ButtonState.UP );
			FramesPerSecond.GetInstance().ClickSetMoveBtEvent( ButtonState.UP );
		}
		
		if(Input.GetKeyDown(KeyCode.F5))
		{
			ClickSetMoveBt( ButtonState.DOWN );
			FramesPerSecond.GetInstance().ClickSetMoveBtEvent( ButtonState.DOWN );
		}

		//Fire button
		if(Input.GetKeyUp(KeyCode.F))
		{
			ClickFireBt( ButtonState.UP );
		}

		if(Input.GetKeyDown(KeyCode.F))
		{
			ClickFireBt( ButtonState.DOWN );
		}

		//OutCard button
		if(Input.GetKeyUp(KeyCode.O))
		{
			ClickStopDongGanBt( ButtonState.UP );
		}
		
		if(Input.GetKeyDown(KeyCode.O))
		{
			ClickStopDongGanBt( ButtonState.DOWN );
		}
	}
}

public enum ButtonState : int
{
	UP = 1,
	DOWN = -1
}