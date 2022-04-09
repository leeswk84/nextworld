using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

/// <summary>
/// Photon.scene 테스트용 코드
/// </summary>
public class PhotonConnect : MonoBehaviourPunCallbacks//, IPunObservable
{
	public GameObject objConnect;
	public GameObject objRoom;

	public UnityEngine.UI.InputField input;
	public UnityEngine.UI.Text txtRoomName;
	public UnityEngine.UI.Text txtUsers;

	public ButtonObject btnConnect;
	public ButtonObject btnSend;
	public ButtonObject btnExit;

	private string str_send;
	//private bool is_send;
	private int send_idx;

	private class MSG
	{
		//public int idx;
		//public string name;
	}

	private void Awake()
	{
		PhotonNetwork.AutomaticallySyncScene = true;
	}
	
	void Start()
    {
		send_idx = 0;
		//is_send = false;

		objConnect.SetActive(true);
		objRoom.SetActive(false);

		btnSend.btn.onClick.AddListener(ClickSend);
		btnConnect.btn.onClick.AddListener(ClickConnect);
		btnExit.btn.onClick.AddListener(ClickExit);

		if (PhotonNetwork.IsConnected == true)
		{

		}
		else
		{
			PhotonNetwork.GameVersion = "1";
			AppSettings setting = new AppSettings();
			PhotonNetwork.ConnectUsingSettings();
		}

	}

	private void ClickConnect()
	{
		PhotonNetwork.NickName = input.text;
		PhotonNetwork.JoinRandomRoom();
		//PhotonNetwork.JoinLobby();
	}

	private void ClickExit()
	{
		Debug.Log("나가기");
		PhotonNetwork.LeaveRoom();
	}

	public void ClickSend()
	{
		//Debug.Log("Click");
		//is_send = true;
		send_idx++;
		photonView.RPC("SendBtn", RpcTarget.All, PhotonNetwork.NickName+ ".send_idx::" + send_idx.ToString());
	}

	[PunRPC]
	public void SendBtn(string value)
	{
		Debug.Log(value);
	}
	
	/*
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			if (is_send == true)
			{
				// We own this player: send the others our data
				Debug.Log("메세지 보내기");
				stream.SendNext(PhotonNetwork.NickName + ".send_idx::" + send_idx.ToString());
				is_send = false;
			}
		}
		else
		{
			// Network player, receive data
			str_send = (string)stream.ReceiveNext();
			Debug.Log("받은 내용 :: " + str_send);
		}
	}
	*/

	public override void OnConnectedToMaster()
	{
		base.OnConnectedToMaster();
		Debug.Log("PUN 접속");
		
	}

	public override void OnRoomListUpdate(List<RoomInfo> roomList)
	{
		base.OnRoomListUpdate(roomList);
		Debug.Log("방 갯수 :: " + roomList.Count);
		if (roomList.Count > 0)
		{
			
			string str = "";
			str += "materClientId::" + roomList[0].masterClientId + "|";
			str += "IsOpen::" + roomList[0].IsOpen + "|";
			str += "IsVisible::" + roomList[0].IsVisible + "|";
			str += "Name::" + roomList[0].Name + "|";
			str += "PlayerCount::" + roomList[0].PlayerCount + "|";
			str += "MaxPlayers::" + roomList[0].MaxPlayers + "|";
			str += "RemovedFromList::" + roomList[0].RemovedFromList + "|";
			Debug.Log(str);
			
			//Debug.Log(roomList[0].ToStringFull());
		}

	}

	public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
	{
		base.OnRoomPropertiesUpdate(propertiesThatChanged);
		UpdateUIPlayers();
	}

	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		UpdateUIPlayers();
	}

	public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
		UpdateUIPlayers();
	}

	private void UpdateUIPlayers()
	{
		Debug.Log("방에 유저수:" + PhotonNetwork.CurrentRoom.Players.Count);

		string str = "유저 목록\n";
		foreach (int key in PhotonNetwork.CurrentRoom.Players.Keys)
		{
			str += PhotonNetwork.CurrentRoom.Players[key].NickName + "\n";
		}
		txtUsers.text = str;
	}


	public override void OnLeftRoom()
	{
		base.OnLeftRoom();
		objConnect.SetActive(true);
		objRoom.SetActive(false);
	}

	public override void OnJoinedLobby()
	{
		base.OnJoinedLobby();
		Debug.Log("로비에 들어옴");
		Debug.Log("PlayerList::" + PhotonNetwork.PlayerList.Length);

	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		base.OnDisconnected(cause);
		Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
	}


	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		Debug.Log("임의 방 진입 실패. 방 생성");

		// #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
		PhotonNetwork.CreateRoom(input.text, new RoomOptions { MaxPlayers = 4, IsVisible = true });
	}

	public override void OnJoinedRoom()
	{
		Debug.Log("방 진입");
		objConnect.SetActive(false);
		objRoom.SetActive(true);

		txtRoomName.text = "방이름:" + PhotonNetwork.CurrentRoom.Name;
		UpdateUIPlayers();
	}
}
