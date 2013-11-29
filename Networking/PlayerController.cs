//PlayerController
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

    public Transform clientCar;
    public Transform networkCar;
    public Vector3 CurPos;
    public Quaternion CurRot;
    public List<Level> ListOfLevels = new List<Level>();
    public bool isCop;
    public Player MyPlayer;

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
    }

    void Update()
    {

    }

    void FixedUpdate()
    {

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
            stream.Serialize(ref CurPos);
            stream.Serialize(ref CurRot);
        }
        else
        {
            stream.Serialize(ref CurPos);
            stream.Serialize(ref CurRot);
            networkCar.position = CurPos;
            networkCar.rotation = CurRot;
        }
    }
}