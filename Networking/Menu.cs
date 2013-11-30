using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {
	
	public static Menu instance;
	private string CurMenu;
	public string PlayerName;
	public string MatchName; // Delete in future as servers will be dedicated and not user created..
	public int MaxPlayers;
	public string ipToConnect;
	
	void Start () {
		instance = this;
		CurMenu = "Main";
		PlayerName = PlayerPrefs.GetString("Username");
		MaxPlayers = 8;
		MatchName = "JSD Racing Server " + Random.Range(0, 100);
		ipToConnect = "127.0.0.1";
	}
	
	void Update () {
		
	}
	
	void ToMenu(string menu)
	{
		CurMenu = menu;
	}
	
	void OnGUI()
	{
		if(CurMenu == "Main")
			Main();
		if(CurMenu == "Host")
			Host();
		if(CurMenu == "Lobby")
			Lobby();
	}
	
	private void Main()
	{
		if(GUI.Button(new Rect(0, 0, 128, 32), "Host Game")){
			ToMenu("Host");
		}
		PlayerName = GUI.TextField(new Rect(130, 0, 128, 32), PlayerName);
		if(GUI.Button(new Rect(258, 0, 128, 32), "Set Name")){
			PlayerPrefs.SetString("Username", PlayerName);
			NetworkManager.instance.PlayerName = PlayerName;
		}
		ipToConnect = GUI.TextField(new Rect(130, 33, 128, 32), ipToConnect);
		if(GUI.Button(new Rect(258, 33, 128, 32), "Connect")){
			Network.Connect(ipToConnect, 25565);
		}
	}
	
	private void Host()
	{
		if(GUI.Button(new Rect(0, 0, 128, 32), "Start Game")){
			NetworkManager.instance.StartServer(MatchName, MaxPlayers);
			ToMenu("Lobby");
		}
		
		if(GUI.Button(new Rect(0, 33, 128, 32), "Main Menu")){
				ToMenu("Main");
		}
		MatchName = GUI.TextField(new Rect(130, 0, 128, 32), MatchName);
		GUI.Label (new Rect(260, 0, 128, 32), "Match Name");
		GUI.Label (new Rect(130, 33, 128, 32), "Max Players");
		MaxPlayers = Mathf.Clamp (MaxPlayers, 0, 32);
		if(GUI.Button (new Rect(210, 33, 32, 32), "+"))
			MaxPlayers += 2;
		if(GUI.Button (new Rect(274, 33, 32, 32), "-"))
			MaxPlayers -= 2;
		GUI.Label (new Rect(254, 33, 64, 32), MaxPlayers.ToString());
	}
	
	private void Lobby()
	{
		if(Network.isServer)
		{
			if(GUI.Button(new Rect(Screen.width - 128, Screen.height - 64, 128, 32), "Start Match")){
				NetworkManager.instance.networkView.RPC ("LoadLevel", RPCMode.All, "test");
			}
		}
		
		if(GUI.Button(new Rect(Screen.width - 128, Screen.height - 32, 128, 32), "Disconnect")){
			Network.Disconnect();
			ToMenu("Main");
		}
		
		GUILayout.BeginArea(new Rect(0, 0, 128, Screen.height), "Players");
		GUILayout.Space(20);
		foreach(Player pl in NetworkManager.instance.PlayerList)
		{
			GUILayout.BeginHorizontal();
			GUI.color = Color.blue;
			GUILayout.Box(pl.PlayerName);
			GUILayout.EndHorizontal();
		}
		GUILayout.EndArea();
	}
	
	void OnServerInitialized ()
	{
		ToMenu ("Lobby");
	}
	
	void OnConnectedToServer ()
	{
		ToMenu ("Lobby");
	}
		
	void OnDisconnectedFromServer (NetworkDisconnection info)
	{
		ToMenu ("Main");
	}
}
