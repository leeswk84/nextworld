using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class Packet
{

}

public class NetworkManager : MonoBehaviour
{
	public List<Packet> listPacket;
	
	private const string netrooturl = "http://leeswk.ivyro.net/wk_admin/php/nextworld/";
	private Dictionary<PTC, string> protocols;
	public enum PTC
	{
		LOGIN = 0,
		SETNICK,
		MAPEDIT,
		MAPLIST,
		MAPTIME,
		PLANETEDIT,
		PLANETGET,
		MAPEDITTYPE,
		UPDATEDATA,
	}
	[HideInInspector]
	public bool isFirstLoad = false;
	
	private WWWForm netform;
	//private WWW netwww;

	private BaseProtocol basePrt;

	public void Init()
	{
		listPacket = new List<Packet>();
		netform = new WWWForm();
		isFirstLoad = false;

		protocols = new Dictionary<PTC, string>();
		protocols.Add(PTC.LOGIN, "login.php");
		protocols.Add(PTC.SETNICK, "setnick.php");
		protocols.Add(PTC.MAPEDIT, "mapedit.php");
		protocols.Add(PTC.MAPLIST, "maplist.php");
		protocols.Add(PTC.MAPTIME, "maptimelist.php");
		protocols.Add(PTC.PLANETGET, "planettilecount.php");
		protocols.Add(PTC.PLANETEDIT, "planettilecountedit.php");
		protocols.Add(PTC.MAPEDITTYPE, "mapedittype.php");
		protocols.Add(PTC.UPDATEDATA, "updatedata.php");
	}

	public void StartSocket()
	{

	}

	private IEnumerator SendSocket()
	{

		yield break;
	}

	private void Send(PTC protocol, Action<string> valueComplete = null, bool isFinish = false)
	{
		StartCoroutine(DoSend(protocol, valueComplete, isFinish));
	}

	private IEnumerator DoSend(PTC prt, Action<string> valueComplete = null, bool isFinish = false)
	{
		UnityWebRequest netwww= UnityWebRequest.Post(netrooturl + protocols[prt], netform);

		//WWW netwww = new WWW(netrooturl + protocols[prt], netform);
		
		//Debug.Log( JsonFx.Json.JsonWriter.Serialize(netwww.data));
		/*
		while (!netwww.isDone)
		{
			yield return new WaitForSeconds(0.1f);
		}
		*/
		yield return netwww.SendWebRequest();

		if (isFinish) yield break;
		
		//if (string.IsNullOrEmpty(netwww.error) == false)
		if(netwww.isNetworkError)
		{
			Debug.Log("NETWORK ERROR :: " + netwww.error);
			GameMgr.ins.mgrUI.popup.Show("NETWORK ERROR : " + netwww.error);
			netwww.Dispose();
			yield break;
		}
		
#if UNITY_EDITOR
		Debug.Log(netwww.downloadHandler.text);
#endif   
		basePrt = JsonFx.Json.JsonReader.Deserialize<BaseProtocol>(netwww.downloadHandler.text);
		if (!basePrt.result)
		{
			switch (basePrt.error)
			{
				case 1:
					//PNmanager.ins.popup.ShowPopup(PNtextDatas.ins.str.ALERT_ERR_1);
					GameMgr.ins.mgrUI.popup.Show("NETWORK ERROR01");
					break;
				default:
					//PNmanager.ins.popup.ShowPopup(PNtextDatas.ins.str.ALERT_ERR);
					GameMgr.ins.mgrUI.popup.Show("NETWORK ERROR");
					break;
			}
			netwww.Dispose();
			yield break;
		}
		//완료
		if (valueComplete != null) valueComplete(netwww.downloadHandler.text);
		netwww.Dispose();
		yield break;
	}
	private void SetBaseNetForm()
	{
		netform = new WWWForm();
	}

	public void SendLogin(string str)
	{
		//PNmanager.ins.data.id = str;
		SetBaseNetForm();
		//netform.AddField("id", str);
		netform.AddField("userID", str);
		//$userID
		Send(PTC.LOGIN, OnCompleteLogin);
	}

	private void OnCompleteLogin(string data)
	{
		//Debug.Log(data);
		Login prtlogin = JsonFx.Json.JsonReader.Deserialize<Login>(data);

		if (prtlogin.data == null)
		{
			PLAY_DATA.ins.status.Init(prtlogin.id);
			//Debug.Log(JsonFx.Json.JsonWriter.Serialize(PLAY_DATA.ins.status));
			//PLAY_DATA.ins.status.userID = prtlogin.id;
			SendUpdateData();
		}
		else
		{
			PLAY_DATA.ins.status = prtlogin.data;
		}

		if (PLAY_DATA.ins.status.savepoint == null) PLAY_DATA.ins.status.InitSavePoint();
		if (PLAY_DATA.ins.status.destorypoint == null) PLAY_DATA.ins.status.destorypoint = new int[10];
		//PLAY_DATA.ins.status.destorypoint = new int[6];
		//PLAY_DATA.ins.status.nut = 710;
		if (PLAY_DATA.ins.status.dicItembox == null) PLAY_DATA.ins.status.dicItembox = new Dictionary<string, Dictionary<string, Dictionary<string,int>>>();

		//PLAY_DATA.ins.status.item_max = 20; //아이템 획득 최대 갯수
		//PLAY_DATA.ins.status.Init(prtlogin.id); //테스트용 무조건 초기화
		SendUpdateData();
		GameMgr.ins.mgrUI.login.obj.SetActive(false);
		

		GameMgr.ins.mgrUI.FogShow(() => 
		{
			GameMgr.ins.PLAYER.CreateFirstDestroyPoint();
			
			GameMgr.ins.PLAYER.RefreshParts();
			GameMgr.ins.mgrUI.status.EditNut(0);
			GameMgr.ins.mgrUI.status.hotKey.LoadData();

			PLAY_DATA.ins.MOVE_DIRECT = true;
			GameMgr.ins.mgrUI.panelMove.SetActive(true);
			GameMgr.ins.mgrSave.SetBlockIdx(PLAY_DATA.ins.status.savepoint[1], false);
			//등장 연출
			GameMgr.ins.PLAYER.ShowIntro();

			GameMgr.ins.mgrUI.FogReset();
			GameMgr.ins.mgrUI.LockHide(false);

		});
		/*
		GameMgr.ins.mgrSave.ShowLock(() =>
		{
			GameMgr.ins.mgrSave.HideLock(false);
		});
		*/
		//if (prtlogin.no != null) PNmanager.ins.data.no = prtlogin.no;
		//if (prtlogin.nick != null) PNmanager.ins.data.nick = prtlogin.nick;
		//if (prtlogin.grade != null) PNmanager.ins.data.grade = prtlogin.grade;
	}

	public void SendUpdateData()
	{
		SetBaseNetForm();
		netform.AddField("userID", PLAY_DATA.ins.status.userID);
		netform.AddField("data", JsonFx.Json.JsonWriter.Serialize(PLAY_DATA.ins.status));
		Send(PTC.UPDATEDATA, CompleteUpdateData);
	}

	public void CompleteUpdateData(string data)
	{

	}

	public void SendSetName(string str, Action valuefncComplete)
	{
		//PNmanager.ins.data.nick = str;
		SetBaseNetForm();
		//netform.AddField("no", PNmanager.ins.data.no);
		netform.AddField("nick", str);
		Send(PTC.SETNICK, CompleteSetName);
	}

	public void CompleteSetName(string data)
	{

	}

	public void SendPlanetGet(int planet)
	{
		SetBaseNetForm();
		netform.AddField("planet", planet);
#if UNITY_EDITOR
		Debug.Log("planet:" + planet);
#endif
		Send(PTC.PLANETGET, CompletePlanetGet);
	}
	public void CompletePlanetGet(string data)
	{
		Planet planet = JsonFx.Json.JsonReader.Deserialize<Planet>(data);
		GameMgr.ins.Init(ref planet);
	}

	public void SendMapTimeList(int planet)
	{

	}

	public void SendMapList(int planet, string tiles)
	{
		SetBaseNetForm();
		netform.AddField("planet", planet);
		netform.AddField("tiles", tiles);
#if UNITY_EDITOR
		//Debug.Log("planet:" + planet +",tiles:"+tiles);
#endif
		Send(PTC.MAPLIST, CompleteMapList);
	}
	public void CompleteMapList(string data)
	{
		MapList list = JsonFx.Json.JsonReader.Deserialize<MapList>(data);
		GameMgr.ins.mgrSave.SetTiles(ref list);
		isFirstLoad = true;
	}

	public void SendMapEdit(int planet, int tile, int tiletype)
	{ SendMapEdit(planet, tile, tiletype); }
	public void SendMapEdit(int planet, int tile, string dataground = "", string dataroad = "", string databuild = "", bool isFinish = false)
	{	SendMapEdit(planet, tile, -1, dataground, dataroad, databuild, isFinish); }
	public void SendMapEdit(int planet, int tile, int maptype, string dataground = "", string dataroad = "", string databuild = "", bool isFinish = false)
	{
		if (isFirstLoad == false) return; //서버의 최초 데이터를 가져오지 않으면 저장하지 않음.

		SetBaseNetForm();
		netform.AddField("planet", planet);
		netform.AddField("tile", tile);
		if (maptype != -1) netform.AddField("maptype", maptype);
		if (dataground != "") netform.AddField("dataground", dataground);
		dataroad = dataroad.Replace(";0", ";");
		if (dataroad != "") netform.AddField("dataroad", dataroad);
		databuild = databuild.Replace(";0", ";");
		//Debug.Log(databuild);
		if (databuild != "") netform.AddField("databuild", databuild);
		
		Send(PTC.MAPEDIT, CompleteMapEdit, isFinish);
	}

	private void CompleteMapEdit(string data)
	{
		Debug.Log("SAVE" + data);
		if(GameMgr.ins.mgrUI.popup.obj.activeSelf) GameMgr.ins.mgrUI.popup.Show(TxtMgr.TYPE.POPUP_SAVE_COMPLETE);
	}
	/// <summary>
	/// 여러 tile type 한번에 수정
	/// </summary>
	/// <param name="planet"></param>
	/// <param name="tiles"></param>
	/// <param name="maptype"></param>
	public void SendMapEdittype(string tiles, int maptype)
	{
		if (isFirstLoad == false) return;
		SetBaseNetForm();
		netform.AddField("planet", GameMgr.ins.mgrSave.PLANET_IDX);
		netform.AddField("tiles", tiles);
		netform.AddField("maptype", maptype);
		Send(PTC.MAPEDITTYPE, CompleteMapEdittype, false);
	}
	private void CompleteMapEdittype(string data)
	{
		Debug.Log(data);
	}

#if UNITY_EDITOR
	//유니티 에디터용 통신
	public void SendMapCountEdit(int width, int height)
	{
		SetBaseNetForm();
		netform.AddField("planet", GameMgr.ins.mgrSave.PLANET_IDX);
		netform.AddField("w_count", width);
		netform.AddField("h_count", height);
		Send(PTC.PLANETEDIT, CompleteMapCountEdit, false);
	}

	private void CompleteMapCountEdit(string data)
	{
		Debug.Log(data);
		SendPlanetGet(PLAY_DATA.ins.START_PLANET);
	}

#endif

}
