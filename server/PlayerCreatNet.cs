using UnityEngine;
using System.Collections;

public class PlayerCreatNet : MonoBehaviour {

	public Transform WangQiuNetGroup = null;
	public Transform WangQiuNetPrefab = null;

	public Transform DianChiNetGroup = null;
	public Transform DianChiNetPrefab = null;

	public Transform ShouBiaoNetGroup = null;
	public Transform ShouBiaoNetPrefab = null;

	public Transform []ChaCheSpawnPoint = null;
	public Transform []ChaCheNetPrefab = null;
	
	public Transform []spawnTran = null;
	public Transform []playerPrefab = null;

	public Transform []spawnNPCTran = null;
	public Transform []npcPrefab = null;
	public GameObject [] HiddenUI;
	public GameObject MapUI;
	public GameObject ServerWaitUI;
	public GameObject ServerWaitCity;
	public GameObject ServerWaitOutdoor;
	private ArrayList list = new ArrayList();
	static PlayerCreatNet _Instance;
	public static PlayerCreatNet GetInstance()
	{
		return _Instance;
	}

	void Start()
	{
		//ScreenLog.Log("PlayerCreatNet init...");
		_Instance = this;
		if (GlobalData.GetInstance().gameMode == GameMode.SoloMode) {
			return;
		}
		IsDisconnected = false;
		ShowServerWait();

		for(int index = 0; index < ChaCheSpawnPoint.Length; index++)
		{
			ChaCheSpawnPoint[index].gameObject.SetActive(false);
		}

		if(WangQiuNetGroup != null)
		{
			for(int index = 0; index < WangQiuNetGroup.childCount; index++)
			{
				WangQiuNetGroup.GetChild(index).gameObject.SetActive(false);
			}
		}

		if(DianChiNetGroup != null)
		{
			for(int index = 0; index < DianChiNetGroup.childCount; index++)
			{
				DianChiNetGroup.GetChild(index).gameObject.SetActive(false);
			}
		}

		if(ShouBiaoNetGroup != null)
		{
			for(int index = 0; index < ShouBiaoNetGroup.childCount; index++)
			{
				ShouBiaoNetGroup.GetChild(index).gameObject.SetActive(false);
			}
		}

		for(int index = 0; index < spawnTran.Length; index++)
		{
			spawnTran[index].gameObject.SetActive(false);
		}

		if (Network.peerType == NetworkPeerType.Client) {
			SendPlayerIntoGame();
		}

		if (Network.peerType == NetworkPeerType.Server) {
			CheckServerInitGame();
		}
	}

	void SendPlayerIntoGame()
	{
		if (Network.peerType != NetworkPeerType.Client) {
			return;
		}
		NetCtrl.GetInstance().SendPlayerIntoGame();
	}

	public static void ResetClientCount()
	{
		ClientCount = 0;
	}

	public static void OnPlayerIntoGame()
	{
		if (Network.peerType != NetworkPeerType.Server) {
			return;
		}

		ClientCount++;
		if (ClientCount < Network.connections.Length) {
			return;
		}

		if (NetCtrl.GetInstance() != null) {
			NetCtrl.GetInstance().AddPlayerIntoGameCount(ClientCount);
		}

		if (_Instance != null) {
			_Instance.CheckServerInitGame();
		}
	}

	bool IsServerInitGame;
	void CheckServerInitGame()
	{
		if (IsServerInitGame) {
			return;
		}

		if (ClientCount < Network.connections.Length) {
			return;
		}
		IsServerInitGame = true;
		Debug.Log("CheckServerInitGame...");
		OnServerInitGame();
	}

	public void ShowServerWait()
	{
		//ScreenLog.Log("ShowServerWait...");
		switch(Application.loadedLevel)
		{
		case (int)GameLeve.Leve3:
			ServerWaitCity.SetActive( true );
			ServerWaitOutdoor.SetActive( false );
			break;

		case (int)GameLeve.Leve4:
			ServerWaitCity.SetActive( false );
			ServerWaitOutdoor.SetActive( true );
			break;
		}
		
		MapUI.SetActive(false);
		ServerWaitUI.SetActive(true);
	}

	public void HiddenServerWait()
	{
		ScreenLog.Log("HiddenServerWait...");
		ServerWaitUI.SetActive(false);
		ServerWaitCity.SetActive( false );
		ServerWaitOutdoor.SetActive( false );
		if (Network.peerType == NetworkPeerType.Client) {
			MapUI.SetActive(true);
		}
	}

	void OnServerInitGame()
	{
		ScreenLog.Log("playerCreatNet : OnServerInitGame -> init");
		ScreenLog.Log("playerCreatNet : OnServerInitGame -> PlayerCount "+NetCtrl.CountLinkPlayer);
		switch(Application.loadedLevel)
		{
		case (int)GameLeve.Leve3:
			if(ServerWaitCity != null)
			{
				ServerWaitCity.SetActive( true );
				ServerWaitOutdoor.SetActive( false );
			}
			break;
			
		case (int)GameLeve.Leve4:
			if(ServerWaitOutdoor != null)
			{
				ServerWaitCity.SetActive( false );
				ServerWaitOutdoor.SetActive( true );
			}
			break;
		}
		
		AudioListener al = gameObject.GetComponent<AudioListener>();
		if(al != null)
		{
			al.enabled = true;
		}
		
		int playerID = int.Parse(Network.player.ToString());
		for(int i = 0; i < HiddenUI.Length; i++)
		{
			HiddenUI[i].SetActive(false);
		}
		
		if(ShouBiaoNetGroup != null)
		{
			for(int index = 0; index < ShouBiaoNetGroup.childCount; index++)
			{
				Transform daoJuTran = (Transform)Network.Instantiate(ShouBiaoNetPrefab, ShouBiaoNetGroup.GetChild(index).position,
				                                                     ShouBiaoNetGroup.GetChild(index).rotation, playerID);
				
				daoJuTran.parent = ShouBiaoNetGroup.parent;
			}
		}
		
		if(DianChiNetGroup != null)
		{
			for(int index = 0; index < DianChiNetGroup.childCount; index++)
			{
				Transform daoJuTran = (Transform)Network.Instantiate(DianChiNetPrefab, DianChiNetGroup.GetChild(index).position,
				                                                     DianChiNetGroup.GetChild(index).rotation, playerID);
				
				daoJuTran.parent = DianChiNetGroup.parent;
			}
		}
		
		if(WangQiuNetGroup)
		{
			for(int index = 0; index < WangQiuNetGroup.childCount; index++)
			{
				Transform daoJuTran = (Transform)Network.Instantiate(WangQiuNetPrefab, WangQiuNetGroup.GetChild(index).position,
				                                                     WangQiuNetGroup.GetChild(index).rotation, playerID);
				
				daoJuTran.parent = WangQiuNetGroup.parent;
			}
		}
		
		for(int index = 0; index < ChaCheSpawnPoint.Length; index++)
		{
			Network.Instantiate(ChaCheNetPrefab[index], ChaCheSpawnPoint[index].position,
			                    ChaCheSpawnPoint[index].rotation, playerID);
		}

		Transform npcTran = null;
		//当玩家选择联机,在切回一次单机,之后玩家进入联机游戏时,在服务器产生npc时就会多产生一个的bug.
		//int max = spawnNPCTran.Length - NetCtrl.CountLinkPlayer + 1;
		int max = spawnNPCTran.Length - Network.connections.Length + 1;
		for(int index = 0; index < max; index++)
		{
			npcTran = (Transform)Network.Instantiate(npcPrefab[index], spawnNPCTran[index].position,
			                   			spawnNPCTran[index].rotation, playerID);
			npcTran.name = npcPrefab[index].name;
		}

		OnPlayerConnectedInfo();
	}

	static int ClientCount = 0;
	void OnPlayerConnectedInfo()
	{
		int max = Network.connections.Length;
		ScreenLog.Log("OnPlayerConnected -> init, clientCount " + max);
		for (int i = 0; i < max; i++) {
			CreatLinkPlayer(Network.connections[i]);
		}
		NetworkServerNet.CountPlayerStatic = 0;
		ServerWaitUI.SetActive(false);
		NetCtrl.GetInstance().SendCheckIsPlayGame();
	}

	void CreatLinkPlayer(NetworkPlayer player)
	{
		int playerID = int.Parse(player.ToString());
		bool isFindPlayer = false;
		int num = 0;
		int max = Network.connections.Length;
		for (int i = 0; i < max; i++) {
			if (player.ipAddress == NetCtrl.PlayerIpArray[i]
			    && player.port == NetCtrl.PlayerPortArray[i]) {
				ScreenLog.Log("ipAddress "+player.ipAddress+", index "+i);
				num = i;
				isFindPlayer = true;
				break;
			}
		}

		if (FreeModeCtrl.IsHavePlayerIp && !isFindPlayer) {
			ScreenLog.LogWarning("creatNPC -> not find player, ipAddress is "+player.ipAddress);
		}
		//num = PlayerPrefs.GetInt("PlayerIndex"); //test
		//num = 3; //test
		//spawnNum = num;

		string nameStr = playerPrefab[num].name;
		ScreenLog.Log("creatNPC -> name " + nameStr);
		Transform playerTrans = (Transform)Network.Instantiate(playerPrefab[num], spawnTran[num].position,
		                                                       spawnTran[num].rotation, playerID);

		playerTrans.parent = transform.parent;
		NetworkView playerObjNetworkview = playerTrans.networkView;
		bikeNetUnity bikeScript = playerTrans.GetComponent<bikeNetUnity>();
		list.Add(bikeScript);
		
		playerObjNetworkview.RPC("SetPlayer", RPCMode.AllBuffered, player);
		playerObjNetworkview.RPC("SetClientPlayerName", RPCMode.OthersBuffered, playerPrefab[num].name);
		playerTrans.name = playerPrefab[num].name;
		//ScreenLog.Log("spawn player: " + playerTrans.name + ", test " + playerPrefab[num].name);
	}

    void OnPlayerDisconnected(NetworkPlayer player)
	{
		foreach(bikeNetUnity script in list)
		{
			if(player == script.ownerPlayer)
			{
				ScreenLog.Log("Clean up after player " + player + ", name " + script.gameObject.name);
				Network.RemoveRPCs(script.gameObject.networkView.viewID);
				Network.Destroy(script.gameObject);
				list.Remove(script);
				break;
			}
		}
		
		int playerNumber = int.Parse(player + "");
		Network.RemoveRPCs(Network.player, playerNumber);
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}

	bool IsLoadLevel = true;
	public static bool IsDisconnected = false;
	public static bool IsBackMoveScence = false;
	void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		if(!IsLoadLevel)
		{
			return;
		}

		if(IsBackMoveScence)
		{
			ScreenLog.LogWarning("PlayerCreatNet::OnDisconnectedFromServer -> IsBackMoveScence is true!");
			return;
		}

		if(SetPanelCtrl.IsIntoSetPanel)
		{
			ScreenLog.LogWarning("OnDisconnectedFromServer -> Donot load " + Application.loadedLevelName + ", IsIntoSetPanel is true!");
			return;
		}
		ScreenLog.Log("OnDisconnectedFromServer -> info " + info + ", level " + Application.loadedLevelName);
		XkGameCtrl.IsLoadingLevel = true;
		Application.LoadLevel(Application.loadedLevel);	
	}
}