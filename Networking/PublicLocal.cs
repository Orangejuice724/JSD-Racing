using UnityEngine;
using System.Collections;

public class PublicLocal : MonoBehaviour {
	
	public Vector3 Pos;
	public Quaternion Rot;
	public NetworkStatess NetworkType = NetworkStatess.Local;
	
	void Start () {
	
	}
	
	void Update () {
		if(NetworkType == NetworkStatess.Public)
		{
		if(networkView.isMine)
		{
			Pos = transform.position;
			Rot = transform.rotation;
		}
		else
		{
			transform.position = Pos;
			transform.rotation = Rot;
		}
		}
	}
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		if(NetworkType == NetworkStatess.Public)
		{
			if(stream.isWriting)
			{
				Pos = transform.position;
				Rot = transform.rotation;
				stream.Serialize(ref Pos);
				stream.Serialize(ref Rot);
			}
			else
			{
				stream.Serialize(ref Pos);
				stream.Serialize(ref Rot);
				transform.position = Pos;
				transform.rotation = Rot;
			}
		}
	}
}

public enum NetworkStatess
{
	Local,
	Public
}