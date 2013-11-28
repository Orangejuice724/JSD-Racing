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

	void Awake()
	{
		
	}
}
