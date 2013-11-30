//PlayerController
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

    public Transform clientCar;
    public Transform networkCar;
	public Transform wheel1;
	public Quaternion wheel1rot;
	public Transform wheel2;
	public Quaternion wheel2rot;
	public Transform wheel3;
	public Quaternion wheel3rot;
	public Transform wheel4;
	public Quaternion wheel4rot;
	public Transform networkWheel1;
	public Transform networkWheel2;
	public Transform networkWheel3;
	public Transform networkWheel4;
    public Vector3 CurPos;
    public Quaternion CurRot;
    public List<Level> ListOfLevels = new List<Level>();
    public bool isCop;
    public Player MyPlayer;
	
	public int currentCar;
	public Transform[] clientCars;
	public Transform[] networkCars;

    void Awake()
    {

    }

    void Start()
    {
        if (networkView.isMine)
        {
            MyPlayer = NetworkManager.getPlayer(networkView.owner);
            MyPlayer.manager = this;
        }
        clientCar.gameObject.SetActive(false);
        networkCar.gameObject.SetActive(false);
        DontDestroyOnLoad(gameObject);
		MyPlayer.isSpawned = false;
    }

    void Update()
    {

    }

    void FixedUpdate()
    {

    }
	
	public void updateMyCar(int carID)
	{
		clientCar = clientCars[carID].transform;
		networkCar = networkCars[carID].transform;
		currentCar = carID;
	}

    [RPC]
    public void RequestPlayer(string Nameee)
    {
        networkView.RPC("GiveMyPlayer", RPCMode.OthersBuffered, Nameee);
    }

    [RPC]
    public void GiveMyPlayer(string n)
    {
        StartCoroutine(GivePlayer(n));
    }

    IEnumerator GivePlayer(string nn)
    {
        while (!NetworkManager.HasPlayer(nn))
        {
            yield return new WaitForEndOfFrame();
        }
        MyPlayer = NetworkManager.getPlayer(nn);
        MyPlayer.manager = this;
    }

    //Sniper add car racing code here ;-; unless i find a better way to do it
    //Will be added at this line here

    public void Client_PlaySound(string GunName, Vector3 soundPoint)
    {
        networkView.RPC("Server_PlaySound", RPCMode.Others, GunName, soundPoint);
    }

    [RPC]
    public void Server_PlaySound(string GunName, Vector3 soundPoint)
    {
        //AudioSource.PlayClipAtPoint(WeaponManager.FindWeapon(GunName).gameObject.audio.clip, soundPoint);
    }

    [RPC]
    void Spawn()
    {
        MyPlayer.isSpawned = true;
        if (networkView.isMine)
        {
            clientCar.gameObject.SetActive(true);
            networkCar.gameObject.SetActive(false);
        }
        else
        {
            clientCar.gameObject.SetActive(false);
            networkCar.gameObject.SetActive(true);
        }
    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        if (stream.isWriting)
        {
            CurPos = clientCar.position;
            CurRot = clientCar.rotation;
			wheel1rot = wheel1.rotation;
			wheel2rot = wheel2.rotation;
			wheel3rot = wheel3.rotation;
			wheel4rot = wheel4.rotation;
            stream.Serialize(ref CurPos);
            stream.Serialize(ref CurRot);
			stream.Serialize(ref wheel1rot);
            stream.Serialize(ref wheel2rot);
			stream.Serialize(ref wheel3rot);
            stream.Serialize(ref wheel4rot);
        }
        else
        {
            stream.Serialize(ref CurPos);
            stream.Serialize(ref CurRot);
			stream.Serialize(ref wheel1rot);
            stream.Serialize(ref wheel2rot);
			stream.Serialize(ref wheel3rot);
            stream.Serialize(ref wheel4rot);
            networkCar.position = CurPos;
            networkCar.rotation = CurRot;
			networkWheel1.rotation = wheel1rot;
            networkWheel2.rotation = wheel2rot;
			networkWheel3.rotation = wheel3rot;
            networkWheel4.rotation = wheel4rot;
        }
    }
}