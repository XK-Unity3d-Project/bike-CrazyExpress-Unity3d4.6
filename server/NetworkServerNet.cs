using UnityEngine;
using System.Collections;

public class NetworkServerNet : MonoBehaviour {
	bool IsServer = false;
	public static string ServerPortIP = "192.168.0.2";
	private string ip = "192.168.0.2";
	private int port = 1000;
	NetCtrl NetCtrlScript;
	public static string IpFile = "GameIP.info";
	private static NetworkServerNet _Instance;
	public static NetworkServerNet GetInstance()
	{
		return _Instance;
	}

	void Awake()
	{
		if (_Instance != null) {
			Destroy(_Instance.gameObject);
		}
		_Instance = this;
		transform.parent = null;
		DontDestroyOnLoad(gameObject);
	}

	void Start()
	{
		FreeModeCtrl.CheckIsHavePlayerIp();
		if (FreeModeCtrl.IsServer) {
			bIsLinkServer = true;
		}

		if (pcvr.bIsHardWare && !pcvr.IsTestGetInput) {
			if (ip != ServerPortIP) {
				ip = ServerPortIP;
			}
			//ip = "192.168.0.53"; //test.
		}
		else {
			ip = HandleJson.GetInstance().ReadFromFilePathXml(IpFile, "SERVER_IP");
			if(ip == null) {
				ip = "192.168.0.2";
				HandleJson.GetInstance().WriteToFilePathXml(IpFile, "SERVER_IP", ip);
			}
		}
		//ScreenLog.Log("serverIP is " + ip);

		//Debug.Log("ip "+ip+", ipAddress "+Network.player.ipAddress+", IsServer "+FreeModeCtrl.IsServer);
		if (FreeModeCtrl.IsHavePlayerIp && FreeModeCtrl.IsServer && ip == Network.player.ipAddress) {
			XKMasterServerCtrl.CheckMasterServerIP();
		}
		MasterServer.ipAddress = ip;
		Network.natFacilitatorIP = ip;
	}

	public static string GetServerPortIp()
	{
		string ipVal = HandleJson.GetInstance().ReadFromFilePathXml(IpFile, "SERVER_IP");
		if(ipVal == null) {
			ipVal = "192.168.0.2";
			HandleJson.GetInstance().WriteToFilePathXml(IpFile, "SERVER_IP", ipVal);
		}
		return ipVal;
	}

	void Update()
	{
		if(Application.loadedLevel == (int)GameLeve.Movie && SetPanelCtrl.IsIntoSetPanel)
		{
			SetPanelCtrl.IsIntoSetPanel = false;
		}

		if( Application.loadedLevel == (int)GameLeve.Leve3
		   || Application.loadedLevel == (int)GameLeve.Leve4
		   || Application.loadedLevel == (int)GameLeve.SetPanel
		   || ChangeMode.IsClickModeStart ) {
			CloseTryToLinkServer();
			return;
		}

		if(NetCtrlScript != null && NetCtrlScript.IsServerPort && Network.peerType == NetworkPeerType.Disconnected)
		{
			//ScreenLog.Log("try to create server...");
			bIsLinkServer = true;
		}

		switch(Network.peerType)
		{
		case NetworkPeerType.Disconnected:
			if(isTryLinkServer && Time.frameCount % 50 == 0)
			{
				//ScreenLog.Log("NetworkServerNet -> test isTryLinkServer");
				bIsLinkServer = true;
				if( NetCtrlScript != null )
				{
					NetCtrlScript.IsBackStartLevel = false;
				}
			}

			if( NetCtrlScript != null && NetCtrlScript.IsBackStartLevel )
			{
				return;
			}

			StartCreat();
			break;

		case NetworkPeerType.Server:
			OnServer();
			break;

		case NetworkPeerType.Client:
			OnClient();
			break;

		case NetworkPeerType.Connecting:
			//ScreenLog.Log("link server...");
			break;
		}
	}

	bool isTryLinkServer = false;
	public bool CheckIsLinkedServerPort()
	{
		if(Network.peerType != NetworkPeerType.Disconnected)
		{
			return true;
		}
		return false;
	}

	public void TryToLinkServer()
	{
		if(Network.peerType != NetworkPeerType.Disconnected)
		{
			return;
		}
		
		//ScreenLog.Log("try to link server...");
		if(NetCtrlScript == null)
		{
			NetCtrlScript = NetCtrl.GetInstance();
			if(NetCtrlScript != null)
			{
				NetCtrlScript.IsBackStartLevel = false;
				NetCtrlScript.SetIsConnectServer( true );
			}
		}
		isTryLinkServer = true;
		bIsLinkServer = true;
	}

	bool bIsLinkServer = false;
	void StartCreat()
	{
		if(bIsLinkServer)
		{
			bIsLinkServer = false;
			if(FreeModeCtrl.IsServer) {
				FreeModeCtrl.CheckIsHavePlayerIp();
				//ScreenLog.Log("start create to server...");
				string passwordStr = "Movie";
				if (Application.loadedLevelName != GameLeve.Movie.ToString()) {
					passwordStr = "LinkGame";
				}

				NetworkConnectionError error = NetworkConnectionError.CreateSocketOrThreadFailure;
				if( NetCtrlScript == null || (NetCtrlScript != null && NetCtrlScript.IsServerPort) )
				{
					error = Network.InitializeServer(30, port, true);
					Network.incomingPassword = passwordStr;
				}

				ScreenLog.Log("NetworkServerNet -> current level is " + Application.loadedLevelName
				              + ", password "+passwordStr);
				//ScreenLog.Log("creat server: info is " + error);
				if (error.ToString() != "NoError") {
					bIsLinkServer = true;
				}
			}
			else if(!FreeModeCtrl.IsServer)
			{
				if(Application.loadedLevel < (int)GameLeve.Leve3)
				{
					TimeLinkServer += Time.deltaTime;
					if(TimeLinkServer < 3.0f)
					{
						bIsLinkServer = true;
						return;
					}
					TimeLinkServer = 0.0f;
				}
				FreeModeCtrl.CheckIsHavePlayerIp();

				if( NetCtrlScript == null || (NetCtrlScript != null && NetCtrlScript.GetIsConnectServer()) )
				{
					string passwordStr = "Movie";
					if (Application.loadedLevelName != GameLeve.Movie.ToString()) {
						passwordStr = "LinkGame";
					}
					ScreenLog.Log("start connect to server -> current level is " + Application.loadedLevelName
					              + ", password "+passwordStr+", connectIp "+ip);
					Network.Connect(ip, port, passwordStr);
				}
			}
		}
	}

	float TimeLinkServer = 100.0f;
	void OnConnectedToServer()
	{
		ScreenLog.Log("OnConnectedToServer: ip " + Network.player.ipAddress);
		if(Application.loadedLevel == (int)GameLeve.Movie)
		{
			isTryLinkServer = false;
			bIsLinkServer = false;
			StartSenceChangeUI.GetInstance().HanldeClientSelectLink();
		}
	}

	void CloseTryToLinkServer()
	{
		isTryLinkServer = false;
		bIsLinkServer = false;
	}

	void OnFailedToConnect(NetworkConnectionError error)
	{
		ScreenLog.Log("Could not connect to server: " + error);
		bIsLinkServer = true;
	}
	
	static bool bIsHaveClient = false;
	static public bool getIsHaveClient()
	{
		return bIsHaveClient;
	}

	bool bIsDisconnect = false;
	short mClientCount = 0;
	void OnServer()
	{
		//ScreenLog.Log("creat server succeed, linking, mClientCount " + mClientCount);
		if (Network.connections.Length >= 1) {
			bIsHaveClient = true;
		}

		if(Network.connections.Length != mClientCount) {
	        int length = Network.connections.Length;
			mClientCount = (short)length;
		}

		if(bIsDisconnect) {
			bIsLinkServer = false;
			bIsDisconnect = false;
			Network.Disconnect();
		}
	}

	void OnClient()
	{
		bIsLinkServer = false;
		if(bIsDisconnect) {
			bIsLinkServer = false;
			bIsDisconnect = false;
			Network.Disconnect();
		}
	}

	public Transform NetCtrlPrefab = null;
	void OnServerInitialized()
	{
		//ScreenLog.Log("OnServerInitialized -> init");
		IsServer = true;
		if ( NetCtrlPrefab != null && GameObject.Find(NetCtrlPrefab.name) == null ) {
			int playerID = int.Parse(Network.player.ToString());
			Network.Instantiate(NetCtrlPrefab, NetCtrlPrefab.position, NetCtrlPrefab.rotation, playerID);
			NetCtrlScript = NetCtrl.GetInstance();
			if (NetCtrlScript != null) {
				NetCtrlScript.IsServerPort = true;
			}
		}
	}

	private int clientCount = 0;
	public static int CountPlayerStatic;
	void OnPlayerConnected(NetworkPlayer player)
	{
		clientCount++;
		if(Application.loadedLevel == (int)GameLeve.Movie) {
			int lenVal = Network.connections.Length;
			int indexVal = lenVal - 1;
			NetCtrl.OnPlayerConnectedServer(player, indexVal);

			string ipInfo = Network.connections[indexVal].ipAddress;
			NetCtrl.GetInstance().SendPlayerIndexNameInfo(ipInfo, indexVal);
		}
		//ScreenLog.Log("NetworkServerNet::OnPlayerConnected -> init, clientCount " + clientCount);
	}
	
	void OnPlayerDisconnected(NetworkPlayer player)
	{
		clientCount--;
	}

	public void SetIsDisconnect()
	{
		ScreenLog.Log("SetIsDisconnect...");
		if (NetCtrlScript == null) {
			NetCtrlScript = NetCtrl.GetInstance();
		}
		bIsDisconnect = true;

		switch(Network.peerType) {
		case NetworkPeerType.Server:
			OnServer();
			break;

		case NetworkPeerType.Client:
			OnClient();
			break;
		}

		if (NetCtrl.GetInstance() != null) {
			NetCtrl.GetInstance().DestroyNetCtrl();
		}
		Destroy( gameObject );
	}
	
	public bool GetIsServer()
	{
		return IsServer;
	}

	void OnApplicationQuit()
	{
		if (FreeModeCtrl.IsServer) {
			XKMasterServerCtrl.CloseMasterServer();
		}
	}
}