using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class PhotonZone : MonoBehaviourPunCallbacks
{
	public bool is_init = false;

	//private string str_send;
	//private bool is_send;
	//private int send_idx;

	private bool is_connect_room;
	
	private const string STR_ReciveMSG_Pos = "ReciveMSG_Pos";
	private const string STR_SendMSG_Pos = "SendMSG_Pos";
	private MSG_POS pSendMSG_Pos;
	private MSG_POS pRecieveMSG_Pos;
	
	private const string STR_ReciveMSG_Stat = "ReciveMSG_Stat";
	private MSG_STAT pSendMSG_Stat;
	private MSG_STAT pRecieveMSG_Stat;

	private Dictionary<string, NetPlayer> dicNetPlayer;

	private Vector3 vec3 ;

	private void Init()
	{
		//send_idx = 0;
		//is_send = false;
		PhotonNetwork.AutomaticallySyncScene = true;
		is_init = true;
		is_connect_room = false;
		vec3 = Vector3.zero;


		pSendMSG_Pos = new MSG_POS();
		pRecieveMSG_Pos = new MSG_POS();
		pSendMSG_Stat = new MSG_STAT();
		pRecieveMSG_Stat = new MSG_STAT();
		dicNetPlayer = new Dictionary<string, NetPlayer>();
	}
	
	public void ConnetMatser(string valueName)
	{
		PhotonNetwork.NickName = valueName;
		if (is_init == false) Init();
		is_connect_room = true;

		if (PhotonNetwork.IsConnected == true)
		{
			ClickConnect();
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
		//PhotonNetwork.NickName = input.text;
		PhotonNetwork.JoinRandomRoom();
		//PhotonNetwork.JoinLobby();
	}

	public void ClickExit()
	{
		Debug.Log("나가기");
		for (int i = 0; i < GameMgr.ins.mgrUI.status.netuser.Length; i++)
		{
			GameMgr.ins.mgrUI.status.netuser[i].obj.SetActive(false);
			GameMgr.ins.netPlayer[i].mini.obj.SetActive(false);
		}
		PhotonNetwork.LeaveRoom();
	}
	/// <summary> 포톤 접속 중인지 여부 반환 </summary>
	/// <returns></returns>
	public bool CheckInRoom()
	{
		
		if (is_init == true 
			&& PhotonNetwork.IsConnected == true 
			&& PhotonNetwork.CurrentRoom != null)
		{
			return true;
		}
		return false;
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
		if (is_connect_room) ClickConnect();
		else
		{
			//PhotonNetwork.LeaveLobby();
			PhotonNetwork.Disconnect();
		}
		is_connect_room = false;
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

		//string str = "유저 목록\n";
		int cnt = 0;
		for (cnt = 0; cnt < GameMgr.ins.mgrUI.status.netuser.Length; cnt++)
		{
			GameMgr.ins.mgrUI.status.netuser[cnt].obj.SetActive(false);
		}
		cnt = 0;
		foreach (int key in PhotonNetwork.CurrentRoom.Players.Keys)
		{
			if (GameMgr.ins.mgrUI.status.netuser.Length <= cnt) continue;

			GameMgr.ins.mgrUI.status.netuser[cnt].obj.SetActive(true);
			GameMgr.ins.mgrUI.status.netuser[cnt].txtName.text = PhotonNetwork.CurrentRoom.Players[key].NickName;
			cnt++;
			//str += PhotonNetwork.CurrentRoom.Players[key].NickName + "\n";
		}

		for (cnt = 0; cnt < GameMgr.ins.mgrUI.status.netuser.Length; cnt++)
		{
			if (GameMgr.ins.mgrUI.status.netuser[cnt].txtName.text == PhotonNetwork.NickName) continue;

			if (GameMgr.ins.mgrUI.status.netuser[cnt].obj.activeSelf == false)
			{
				if (dicNetPlayer.ContainsKey(GameMgr.ins.mgrUI.status.netuser[cnt].txtName.text) == true)
				{
					dicNetPlayer[GameMgr.ins.mgrUI.status.netuser[cnt].txtName.text].mini.obj.SetActive(false);
					dicNetPlayer.Remove(GameMgr.ins.mgrUI.status.netuser[cnt].txtName.text);
				}
			}

		}
		SendMSG_Stat();

		//txtUsers.text = str;
	}


	public override void OnLeftRoom()
	{
		base.OnLeftRoom();
		//objConnect.SetActive(true);
		//objRoom.SetActive(false);
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
		//PhotonNetwork.CreateRoom(input.text, new RoomOptions { MaxPlayers = 4, IsVisible = true });
		PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 3, IsVisible = true });
	}

	public override void OnJoinedRoom()
	{
		Debug.Log("방 진입");
		//objConnect.SetActive(false);
		//objRoom.SetActive(true);
		//txtRoomName.text = "방이름:" + PhotonNetwork.CurrentRoom.Name;
		UpdateUIPlayers();
		GameMgr.ins.mgrUI.status.popupMessage.OnConnectRoom();

		SendMSG_Stat();
		SendMSG_Pos();
	}
	/*
	public int w_pos;
	public int h_pos;
	public int tile_idx;
	public string check_str;
	*/
	/// <summary>
	/// 위치 정보 보내기
	/// </summary>
	public void SendMSG_Pos()
	{
		if (CheckInRoom() == false) return;
		//Debug.Log("Click");
		//is_send = true;
		//send_idx++;
		//sendMSG.idx = send_idx;
		pSendMSG_Pos.name = PhotonNetwork.NickName;
		pSendMSG_Pos.x = Mathf.RoundToInt(GameMgr.ins.PLAYER.mini.trans.position.x * 100);
		pSendMSG_Pos.y = Mathf.RoundToInt(GameMgr.ins.PLAYER.mini.trans.position.y * 100);
		pSendMSG_Pos.z = Mathf.RoundToInt(GameMgr.ins.PLAYER.mini.trans.position.z * 100);
		pSendMSG_Pos.t = GameMgr.ins.mgrSave.editIndex;

		photonView.RPC(STR_ReciveMSG_Pos, RpcTarget.All, JsonFx.Json.JsonWriter.Serialize(pSendMSG_Pos));
		//photonView.RPC("SendBtn", RpcTarget.All, PhotonNetwork.NickName + ".send_idx::" + send_idx.ToString());
		/*
		MSG_POS data = pSendMSG_Pos;
		tile_idx = data.t;
		int[] w_check = new[] { 0, 1, 3, 4 };
		int[] h_check = new[] { 0, 5, 15, 20 };
		int n, nn;

		check_str = string.Empty;
		//if (GameMgr.ins.mgrSave.editIndex != data.t)
		{
			//Debug.Log(SaveManager.PLANET_WIDTH)
			//GameMgr.ins.mgrSave.editIndex
			w_pos = -1;
			for (n = 0; n < w_check.Length; n++)
			{
				for (nn = 0; nn < 5; nn++)
				{
					check_str += GameMgr.ins.mgrBlock.posBlocks[w_check[n] + (nn * 5)].block_idx + ",";
					if (data.t == GameMgr.ins.mgrBlock.posBlocks[w_check[n] + (nn * 5)].block_idx)
					{
						w_pos = n;
						break;
					}
				}
				check_str += "|";
				if (w_pos != -1) break;
			}
			
			h_pos = -1;
			for (n = 0; n < h_check.Length; n++)
			{
				for (nn = 0; nn < 5; nn++)
				{
					if (data.t == GameMgr.ins.mgrBlock.posBlocks[h_check[n] + nn].block_idx)
					{
						h_pos = n;
						break;
					}
				}
				if (h_pos != -1) break;
			}
		}
		*/
	}

	/// <summary>
	/// 위치 정보 서버 통신 내용 받아 옴
	/// </summary>
	/// <param name="value"></param>
	[PunRPC]
	public void ReciveMSG_Pos(string value)
	{
		//Debug.Log(value);
		pRecieveMSG_Pos = JsonFx.Json.JsonReader.Deserialize<MSG_POS>(value);
		
		if (pRecieveMSG_Pos.name == PhotonNetwork.NickName) Invoke(STR_SendMSG_Pos, 0.5f); //0.5초 후 자신의 정보 다시 보냄
		else
		{
			//vec3.x = pRecieveMSG_Pos.x * 0.01f;
			//vec3.y = pRecieveMSG_Pos.y * 0.01f;
			//vec3.z = pRecieveMSG_Pos.z * 0.01f;
			//GameMgr.ins.netPlayer[0].mini.trans.position = vec3;
			if (dicNetPlayer.ContainsKey(pRecieveMSG_Pos.name))
			{
				dicNetPlayer[pRecieveMSG_Pos.name].SetMove(pRecieveMSG_Pos);
			}
		}
	}


	/// <summary>
	/// 파츠 정보 보내기
	/// </summary>
	public void SendMSG_Stat()
	{
		if (CheckInRoom() == false) return;

		pSendMSG_Stat.name = PhotonNetwork.NickName;
		pSendMSG_Stat.equip = new int[(int)ITEM_DATA.TYPE_2.MAX];
		pSendMSG_Stat.equip[(int)ITEM_DATA.TYPE_2.BODY] = PLAY_DATA.ins.status.item[PLAY_DATA.ins.status.equip[(int)ITEM_DATA.TYPE_2.BODY]].idx;
		pSendMSG_Stat.equip[(int)ITEM_DATA.TYPE_2.FOOT] = PLAY_DATA.ins.status.item[PLAY_DATA.ins.status.equip[(int)ITEM_DATA.TYPE_2.FOOT]].idx;
		pSendMSG_Stat.equip[(int)ITEM_DATA.TYPE_2.ARML] = PLAY_DATA.ins.status.item[PLAY_DATA.ins.status.equip[(int)ITEM_DATA.TYPE_2.ARML]].idx;
		pSendMSG_Stat.equip[(int)ITEM_DATA.TYPE_2.ARMR] = PLAY_DATA.ins.status.item[PLAY_DATA.ins.status.equip[(int)ITEM_DATA.TYPE_2.ARMR]].idx;
		photonView.RPC(STR_ReciveMSG_Stat, RpcTarget.All, JsonFx.Json.JsonWriter.Serialize(pSendMSG_Stat));
	}

	/// <summary>
	/// 파츠 정보 서버 통신 내용 받아 옴
	/// </summary>
	/// <param name="value"></param>
	[PunRPC]
	public void ReciveMSG_Stat(string value)
	{
		//Debug.Log(value);
		pRecieveMSG_Stat = JsonFx.Json.JsonReader.Deserialize<MSG_STAT>(value);

		for (int i = 0; i < GameMgr.ins.mgrUI.status.netuser.Length; i++)
		{
			if (pRecieveMSG_Stat.name == GameMgr.ins.mgrUI.status.netuser[i].txtName.text)
			{
				GameMgr.ins.mgrUI.status.netuser[i].imgBody.sprite = GameMgr.ins.uiatlas.GetSprite(PLAY_DATA.ins.dataItem.dic[pRecieveMSG_Stat.equip[(int)ITEM_DATA.TYPE_2.BODY]].icon);
				break;
			}
		}
		
		if (pRecieveMSG_Stat.name == PhotonNetwork.NickName) return;

		if (dicNetPlayer.ContainsKey(pRecieveMSG_Stat.name) == false)
		{
			for (int i = 0; i < GameMgr.ins.netPlayer.Length; i++)
			{
				if (GameMgr.ins.netPlayer[i].mini.obj.activeSelf == true) continue;
				dicNetPlayer.Add(pRecieveMSG_Stat.name, GameMgr.ins.netPlayer[i]);
				break;
			}
		}
		dicNetPlayer[pRecieveMSG_Stat.name].SetParts(pRecieveMSG_Stat.name, pRecieveMSG_Stat.equip);
	}


}
