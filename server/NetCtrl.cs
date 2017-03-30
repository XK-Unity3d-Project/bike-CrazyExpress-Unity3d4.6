using UnityEngine;
using System.Collections;

using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

public class NetCtrl : MonoBehaviour {
	public bool IsBackStartLevel = false;
	public bool IsServerPort = false;
	int _selectLinkCount = 0;
	public int selectLinkCount
	{
		set
		{
			_selectLinkCount = value;
		}

		get
		{
			return _selectLinkCount;
		}
	}

	GameLeve selectGameLevel = GameLeve.None;
	public bool isLoadServerLevel = false;
	
	private AsyncOperation status;
	bool IsConnectServer = true;

	public int GetSelectLinkCount()
	{
		return selectLinkCount;
	}

	private static NetCtrl _Instance;
	public static NetCtrl GetInstance()
	{
		return _Instance;
	}

	void Start()
	{
		if (_Instance != null) {
			Destroy(_Instance.gameObject);
		}
		_Instance = this;
		gameObject.name = "_NetCtrl";
		DontDestroyOnLoad(gameObject);

		if (Application.loadedLevel == (int)GameLeve.Movie) {
			if (Network.peerType == NetworkPeerType.Client) {
				StartSenceChangeUI.GetInstance().HanldeClientSelectLink();
			}
		}
	}

	public GameLeve GetSelectGameLevel()
	{
		return selectGameLevel;
	}

	public void handleSelectLevel( int levelGame )
	{
		networkView.RPC("SendSelectLevelInfo", RPCMode.AllBuffered, levelGame);
	}

	[RPC]
	void SendSelectLevelInfo(int levelGame)
	{
		selectGameLevel = (GameLeve)levelGame;
		GlobalData.GetInstance().gameLeve = selectGameLevel;
	}

	public void ClientCallHandleSelectLinkCount(bool isAdd)
	{
		if (!FreeModeCtrl.IsHavePlayerIp || GlobalData.GetInstance().LinkModeState == 1) {
			return;
		}
		StopCoroutine( "handleSelectLinkCount" );
		StartCoroutine( "handleSelectLinkCount", isAdd );
	}

	public IEnumerator handleSelectLinkCount(bool isAdd)
	{
		if (Network.peerType != NetworkPeerType.Server) {
			if (Network.peerType != NetworkPeerType.Client || !Network.isClient) {
				yield return new WaitForSeconds(1.0f);
				yield return handleSelectLinkCount( isAdd );
			}
		}
		yield return new WaitForSeconds(0.5f);
		//reset Client Port isLoadServerLevel
		isLoadServerLevel = false;

		if(isAdd) {
			networkView.RPC("sendAddLinkCount", RPCMode.AllBuffered, Network.player.ipAddress);
		}
		else {
			networkView.RPC("sendSubLinkCount", RPCMode.AllBuffered, Network.player.ipAddress);
		}
		yield break; 
	}

	List<string>IpList;
	[RPC]
	void sendAddLinkCount(string ipInfo)
	{
		if (IpList == null) {
			IpList = new List<string>();
		}

		if (!IpList.Contains(ipInfo)) {
			IpList.Add(ipInfo);
		}

		if (Network.peerType == NetworkPeerType.Server) {
			if (Network.connections.Length < 2) { //至少有2个玩家连接服务器时,可以进行联机游戏.
				return;
			}
		}
		else {
			//选择联机的玩家数量小于2人时,不允许进行联机游戏.
			if (IpList.Count < 2) {
				return;
			}
		}

		selectLinkCount = IpList.Count;
		NetworkServerNet.CountPlayerStatic = selectLinkCount;
		CountLinkPlayer = selectLinkCount;
		ScreenLog.Log("sendAddLinkCount -> selectLinkCount " + selectLinkCount);

		if (Network.peerType == NetworkPeerType.Client) {
			StartSenceChangeUI.GetInstance().ActivePlayerLinkGame();
		}
		else {
			HandleActiveGameSelectObj();
		}
	}

	[RPC]
	void sendSubLinkCount(string ipInfo)
	{
		if (IpList == null) {
			IpList = new List<string>();
		}
		
		if (IpList.Contains(ipInfo)) {
			IpList.Remove(ipInfo);
		}
		selectLinkCount = IpList.Count;
		NetworkServerNet.CountPlayerStatic = selectLinkCount;
		CountLinkPlayer = selectLinkCount;
		ScreenLog.Log("sendSubLinkCount -> selectLinkCount " + selectLinkCount);
	}

	public void handleResetLinkCount()
	{
		networkView.RPC("sendResetLinkCount", RPCMode.AllBuffered);
	}

	[RPC]
	void sendResetLinkCount()
	{
		selectLinkCount = 0;
		selectGameLevel = GameLeve.None;
	}

	public void handleLoadLevel()
	{
		//ScreenLog.Log("handleLoadLevel********");
		if(isLoadServerLevel)
		{
			return;
		}
		networkView.RPC("SendLoadLevel", RPCMode.AllBuffered);
	}

	void startIntoGame()
	{
		status.allowSceneActivation = true;
		//reset Server Port isLoadServerLevel
		isLoadServerLevel = false;
	}

	public static int CountLinkPlayer;
	[RPC]
	void SendLoadLevel()
	{
		if(isLoadServerLevel)
		{
			return;
		}

		isLoadServerLevel = true;
		//ScreenLog.Log("SendLoadLevel************************test");
		if(Network.isServer)
		{
			ScreenLog.Log("SendLoadLevel -> level " + GlobalData.GetInstance().gameLeve);
			MediaPlayer.stopPlayMove();
			Network.incomingPassword = "";
			IsServerPort = true;
			IsConnectServer = false;

			if(GlobalData.GetInstance().gameLeve == GameLeve.Leve1)
			{
				status = Application.LoadLevelAsync((int)GameLeve.Leve3);
			}
			else
			{
				status = Application.LoadLevelAsync((int)GameLeve.Leve4);
			}
			status.allowSceneActivation = false;
			Invoke("startIntoGame", 1.0f);
		}
	}

	public bool GetIsConnectServer()
	{
		//Debug.Log("GetIsConnectServer " + IsConnectServer);
		return IsConnectServer;
	}

	public void SetIsConnectServer( bool val )
	{
		IsConnectServer = val;
	}

	void OnPlayerDisconnected(NetworkPlayer player)
	{
		if(FreeModeCtrl.IsServer) {
			ScreenLog.Log("OnPlayerDisconnected -> stop send msg! playerIP " + player.ipAddress);
			if (Application.loadedLevel == (int)GameLeve.Movie) {
				ClientCallHandleSelectLinkCount(false);
			}
			Network.SetReceivingEnabled(player, 0, false);
			Network.SetSendingEnabled(player, 0, false);
			CheckIsBackMovieScene();
		}
	}

	int playerLeaveCount = 0;
	public void OnPlayerLeaveGame()
	{
		IsBackStartLevel = true;
		PlayerIntoGameCount = 0; //reset PlayerIntoGameCount
		networkView.RPC("SendOnPlayerLeaveGame", RPCMode.OthersBuffered, Network.player.ipAddress);
	}

	[RPC]
	void SendOnPlayerLeaveGame( string playerIP )
	{
		if(FreeModeCtrl.IsServer)
		{
			foreach(NetworkPlayer playerObj in Network.connections)
			{
				if(playerIP == playerObj.ipAddress)
				{
					ScreenLog.Log("stop send msg! playerIP " + playerIP);
					Network.SetReceivingEnabled(playerObj, 0, false);
					Network.SetSendingEnabled(playerObj, 0, false);
				}
			}
			CheckIsBackMovieScene();
		}
	}

	void CheckIsBackMovieScene()
	{
		if(Application.loadedLevel != (int)GameLeve.Movie)
		{
			playerLeaveCount++;
			//if(FreeModeCtrl.IsServer && playerLeaveCount >= CountLinkPlayer)
			ScreenLog.Log("connections.length "+Network.connections.Length);
			if(FreeModeCtrl.IsServer && Network.connections.Length <= 1)
			{
				ScreenLog.Log("CheckIsBackMovieScene -> loading movie...");
				CountLinkPlayer = 0;
				playerLeaveCount = 0;

				NetworkServerNet netScript = NetworkServerNet.GetInstance();
				if (netScript != null) {
					netScript.SetIsDisconnect();
				}
				XkGameCtrl.IsLoadingLevel = true;
				Application.LoadLevel( (int)GameLeve.Movie );
			}
		}
	}

	public bool IsIntoMoveScense = false;
	public void handleClientIsIntoMoveScense()
	{
		ScreenLog.Log("NetCtrl -> handleClientIsIntoMoveScense...");
		PlayerCreatNet.IsDisconnected = true;
		networkView.RPC("SendToClientSetIsIntoMoveScense", RPCMode.OthersBuffered);
	}

	public void resetIsIntoMoveScense()
	{
		IsIntoMoveScense = false;
	}

	[RPC]
	void SendToClientSetIsIntoMoveScense()
	{
		if(Network.isServer)
		{
			return;
		}

		ScreenLog.Log("SendToClientSetIsIntoMoveScense...");
		IsIntoMoveScense = true;
	}

	void Update()
	{
		if(Time.frameCount % 30 == 0)
		{
			if(selectGameLevel != GameLeve.None && (Application.loadedLevel == (int)GameLeve.Leve3 || Application.loadedLevel == (int)GameLeve.Leve4))
			{
				selectGameLevel = GameLeve.None;
			}

			if( selectLinkCount != 0 && (Application.loadedLevel == (int)GameLeve.Leve3 || Application.loadedLevel == (int)GameLeve.Leve4) )
			{
				selectLinkCount = 0;
			}
		}
	}

	public int PlayerIntoGameCount = 0;
	public void AddPlayerIntoGameCount(int playerNum)
	{
		networkView.RPC("SendToAddPlayerIntoGameCount", RPCMode.OthersBuffered, playerNum);
	}

	[RPC]
	void SendToAddPlayerIntoGameCount(int playerNum)
	{
		PlayerIntoGameCount = playerNum;
	}
	
	void OnDisconnectedFromServer(NetworkDisconnection info)
	{
	}

	public void SendPlayerIndexNameInfo(string ipInfo, int indexVal)
	{
		if (networkView == null) {
			return;
		}
		networkView.RPC("NetCtrlSendPlayerIndexNameInfo", RPCMode.OthersBuffered, ipInfo, indexVal);
	}

	[RPC]
	void NetCtrlSendPlayerIndexNameInfo(string ipInfo, int indexVal)
	{
		if (Network.player.ipAddress != ipInfo) {
			return;
		}
		
		ScreenLog.Log("NetCtrlSendPlayerIndexNameInfo -> ipInfo "+ipInfo+", indexVal "+indexVal);
		ScreenLog.Log("NetCtrlSendPlayerIndexNameInfo -> ipAddress "+Network.player.ipAddress);
		LinkPlayerNameCtrl.IndexPlayerVal = indexVal;
		if (LinkPlayerNameCtrl.GetInstance() != null) {
			LinkPlayerNameCtrl.GetInstance().ActivePlayerInfo();
		}
	}

	public void SendPlayerIntoGame()
	{
		if (networkView == null) {
			return;
		}
		networkView.RPC("NetCtrlSendPlayerIntoGame", RPCMode.Server);
	}
	
	[RPC]
	void NetCtrlSendPlayerIntoGame()
	{
		PlayerCreatNet.OnPlayerIntoGame();
	}

	public static string[] PlayerIpArray = new string[8];
	public static int[] PlayerPortArray = new int[8];
	public static void InitPlayerIpArray()
	{
		PlayerIpArray = new string[8];
		PlayerPortArray = new int[8];
		for (int i = 0; i < 8; i++) {
			PlayerIpArray[i] = "";
			PlayerPortArray[i] = -1;
		}
		ScreenLog.Log("InitPlayerIpArray...");
	}

	public static void OnPlayerConnectedServer(NetworkPlayer player, int clientCount)
	{
		if(Application.loadedLevel == (int)GameLeve.Movie) {
			int lenVal = Network.connections.Length;
			int indexVal = lenVal - 1;
			if (PlayerIpArray[indexVal] != player.ipAddress) {
				PlayerIpArray[indexVal] = player.ipAddress;
				PlayerPortArray[indexVal] = player.port;
				ScreenLog.Log("OnPlayerConnectedServer -> ipAddress "+player.ipAddress
				              +", port "+player.port+", index "+indexVal);
			}
		}
	}

	public void DestroyNetCtrl()
	{
		Destroy(gameObject);
	}
	
	public void SendCheckIsPlayGame()
	{
		if (networkView == null || Network.peerType != NetworkPeerType.Server) {
			return;
		}
		networkView.RPC("NetCtrlSendCheckIsPlayGame", RPCMode.OthersBuffered);
	}
	
	[RPC]
	void NetCtrlSendCheckIsPlayGame()
	{
		Go.GetInstance().checkIsPlayGame();
	}

	public void HandleActiveGameSelectObj()
	{
		if (!FreeModeCtrl.IsHavePlayerIp || networkView == null) {
			return;
		}

		if (Network.peerType != NetworkPeerType.Server) {
			return;
		}

		//if (Network.connections.Length < 1) { //至少有1个玩家连接服务器时,可以进行联机游戏.
		if (Network.connections.Length < 2) { //至少有2个玩家连接服务器时,可以进行联机游戏.
			return;
		}
		string ipInfo = Network.connections[0].ipAddress;
		int port = Network.connections[0].port;
		networkView.RPC("NetCtrlSendHandleActiveGameSelectObj", RPCMode.OthersBuffered, ipInfo, port);
	}
	
	[RPC]
	void NetCtrlSendHandleActiveGameSelectObj(string ipInfo, int port)
	{
		StartSenceChangeUI.GetInstance().ActiveGameSelectObj(ipInfo, port);
	}
}