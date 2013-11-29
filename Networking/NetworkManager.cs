//NetworkManager
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour {

	public string PlayerName;
	public static NetworkManager instance;
	public List<Player> PlayerList = new List<Player>();
	public Player MyPlayer;
	public GameObject SpawnPlayer;
	public Level CurLevel;
	public List<Level> ListOfLevels = new List<Level>();

	public int police = 0;
	public bool MatchStarted;

	void Awake()
	{
		instance = this;
		PlayerName = PlayerPrefs.GetString("Username");
	}

	void Start()
	{
		DontDestroyOnLoad(gameObject);
	}

	void Update()
	{

	}

	public void StartServer(string ServerName, int MaxPlayers)
	{
		Network.InitializeSecurity();
		Network.InitializeServer(MaxPlayers, 25565, true);
		MasterServer.RegisterHost("Racing", ServerName, "");
		Debug.Log("StartServer");
	}

	void OnPlayerConnected(NetworkPlayer id)
	{
		foreach(Player pl in PlayerList)
		{
			networkView.RPC("getLevel", id, CurLevel.LoadName, MatchStarted);
			networkView.RPC("Client_PlayerJoined", id, pl.PlayerName, pl.OnlinePlayer);
		}
	}

	void OnConnectedToServer()
	{
		networkView.RPC("Server_PlayerJoined", RPCMode.Server, PlayerName, 0, Network.player);
	}

	void OnServerInitialized()
	{
		Server_PlayerJoined(PlayerName, 0, Network.player);
	}

	void OnPlayerDisconnected(NetworkPlayer id)
	{
		networkView.RPC("RemovePlayer", RPCMode.All, id);
		Network.Destroy(getPlayer(id).manager.gameObject);
		Network.RemoveRPCs(id);
	}

	void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		foreach(Player pl in PlayerList)
		{
			Network.Destroy(pl.manager.gameObject);
		}

		PlayerList.Clear();
		Application.LoadLevel(0);
	}

	[RPC]
	public void Server_PlayerJoined(string Username, int Team, NetworkPlayer id)
	{
		networkView.RPC("Client_PlayerJoined", RPCMode.All, Username, Team, id);
	}

	[RPC]
	public void Client_PlayerJoined(string Username, int Team, NetworkPlayer id)
	{
		Player temp = new Player();
		temp.PlayerName = Username;
		temp.OnlinePlayer = id;
		PlayerList.Add(temp);
		if(Network.player == id)
		{
			MyPlayer = temp;
			GameObject LastPlayer = Network.Instantiate(SpawnPlayer, Vector3.zero, Quaternion.identity, 0) as GameObject;
			LastPlayer.networkView.RPC("RequestPlayer", RPCMode.AllBuffered, Username);
		}
	}

	[RPC]
	public void RemovePlayer(NetworkPlayer id)
	{
		Player temp = new Player();
		foreach(Player pl in PlayerList)
		{
			if(pl.OnlinePlayer == id)
			{
				temp = pl;
			}
		}

		if(temp != null)
		{
			PlayerList.Remove (temp);
		}
	}

	[RPC]
	public void RemoveAllPlayers()
	{
		foreach(Player pl in PlayerList)
		{
			PlayerList.Remove(pl);
			print("Player Removed");
			Network.Destroy(pl.manager.gameObject);
			MasterServer.UnregisterHost();
			Application.LoadLevel(0);
		}
	}

	[RPC]
	public void LoadLevel(string loadName)
	{
		Application.LoadLevel(loadName);

	}

	[RPC]
	public Level getLevel(string LoadName, bool isStarted)
	{
		foreach(Level lvl in ListOfLevels)
			{
				if(LoadName == lvl.LoadName)
				{
					CurLevel = lvl;
					return lvl;
				}
			}

		if(isStarted)
		{
			LoadLevel(LoadName);
		}

		return null;
	}

	public static Player getPlayer(NetworkPlayer id)
	{
		foreach(Player pl in instance.PlayerList)
		{
			if(pl.OnlinePlayer == id)
			{
				return pl;
			}
		}

		return null;
	}

	public static bool HasPlayer(string n)
	{
		foreach(Player pl in instance.PlayerList)
		{
			if(pl.PlayerName == n)
			{
				return true;
			}
		}

		return false;
	}

	public static Player getPlayer(string id)
	{
		foreach(Player pl in instance.PlayerList)
		{
			if(pl.PlayerName == id)
			{
				return pl;
			}
		}

		return null;
	}

	void OnGUI()
	{

	}

}

[System.Serializable]
public class Player {

	public string PlayerName;
	public NetworkPlayer OnlinePlayer;
	public PlayerController manager;
	public bool isCop;
	public int Score; //drifting :D
    public bool isSpawned;

}

[System.Serializable]
public class Level {

	public string LoadName;
	public string PlayName;

}
