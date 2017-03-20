using UnityEngine;
using System.Collections;

public class SetPanelCtrl : MonoBehaviour {

	static private SetPanelCtrl Instance = null;

	static public SetPanelCtrl GetInstance()
	{
		if(Instance == null)
		{
			GameObject obj = new GameObject("_SetPanelCtrl");
			//DontDestroyOnLoad(obj);
			Instance = obj.AddComponent<SetPanelCtrl>();

			if (!FreeModeCtrl.IsServer) {
				pcvr.GetInstance();
			}
			FramesPerSecond.GetInstance();
			ScreenLog.init();
		}
		return Instance;
	}

	void Start()
	{
		InputEventCtrl.GetInstance().ClickSetEnterBtEvent += ClickSetEnterBtEvent;
	}

	void ClickSetEnterBtEvent(ButtonState val)
	{
		if(val == ButtonState.DOWN)
		{
			return;
		}

		//link mode, donot into setPanel
		if(Application.loadedLevel == (int)GameLeve.Leve3
		   || Application.loadedLevel == (int)GameLeve.Leve4
		   || FreeModeCtrl.IsServer
		   || Network.peerType == NetworkPeerType.Client)
		{
			return;
		}

		if(Application.loadedLevel != (int)GameLeve.SetPanel)
		{
			if(!ChangeLeve.IsCanActiveSetPanel)
			{
				if(Application.loadedLevel > (int)GameLeve.Movie)
				{
					ChangeLeve.IsCanActiveSetPanel = true;
				}
				else
				{
					return;
				}
			}
			
			//ScreenLog.Log("start open setPanel...");
			if(Application.loadedLevel == (int)GameLeve.Leve1 || Application.loadedLevel == (int)GameLeve.Leve2)
			{
				bike.resetBikeStaticInfo();
			}
			loadLevelSetPanel();
		}
	}

	public static bool IsIntoSetPanel = false;
	public GameObject SetPanelPrefab;
	public static GameObject SetPanelObj;
	void loadLevelSetPanel()
	{
		//ScreenLog.Log("SetPanelCtrl -> loadLevelSetPanel...");
		XkGameCtrl.IsLoadingLevel = true;
		System.GC.Collect();
		Application.LoadLevel( (int)GameLeve.SetPanel );
	}

	void loadMoveLevel()
	{
		//ScreenLog.Log("SetPanelCtrl -> loadMoveLevel");
		XkGameCtrl.IsLoadingLevel = true;
		System.GC.Collect();
		Application.LoadLevel( (int)GameLeve.Movie );
	}
}