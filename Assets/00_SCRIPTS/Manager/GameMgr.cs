using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CODE_VALUE
{
	public int value;
	public int stack;
}

public class GameMgr : MonoBehaviour 
{
	[Space(-11)]
	[Header("숨김 테스트 클래스")]
	public CODE_VALUE codeValue;
	
	public const int WIDTH = 8; //tile 가로 갯수
	public const int TILE_CENTER = 12;//4;
	public const int TILE_MAX = 25;//9;
	public const int TILE_WIDTH = 5;
	[HideInInspector]
	public static GameMgr ins;
	
	public Mini PLAYER_MINI;
	public Player PLAYER;
	public NetPlayer[] netPlayer;

	public NetworkManager mgrNetwork;
	public PhotonZone photon;

	[Space(-11)]
	[Header("땅 전체 제어")]
	public GroundManager mgrGround;
	public RoadManager mgrRoad;
	public BuildManager mgrBuild;

	public CameraControl mgrCam;
	public UIManager mgrUI;
	public FieldManager mgrField;

	public SaveManager mgrSave;
	public MoveCameraControl mgrMoveCam;

	public GroundSideManager mgrSide;

	public TotalBlockManager mgrBlock;

	public NpcManager mgrNpc;
	
	public TextureAtlas mgrAtlas;

	public BulletManager mgrBullet;
	public EffectManager mgrEffect;
	public BuildListManager mgrBuildList;
	public PartsManager mgrParts;

	public UIObjectManager mgrUIObj;

	public UnityEngine.U2D.SpriteAtlas uiatlas;

	public UITalkSmall talk;
	//public NPC_DATA_LIST dataNPC;

	public StarwarsGraphics.LowBufferCam LowBuffer;

	private bool isInit = false;

	private int col_i;
	private int l;

	void Awake() 
	{
		if (ins != null)
		{
			GameObject.Destroy(this);
		}
		ins = this;
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 60;

	}

	void Start() 
	{
		isInit = false;

		if (mgrAtlas != null) mgrAtlas.Init();
		if (mgrParts != null) mgrParts.Init();

		if (mgrUI != null) mgrUI.objLockPlane.SetActive(true);
	}

	public void StartInit()
	{
		if (mgrNetwork != null) mgrNetwork.Init();
		if (mgrBuildList != null) mgrBuildList.Init();

		if (mgrNetwork != null) mgrNetwork.SendPlanetGet(PLAY_DATA.ins.START_PLANET);
	}

	public void Init(ref Planet planet)
	{
		//dataNPC = new NPC_DATA_LIST();
		//dataNPC.Init();
		
		mgrBullet.Init();
		mgrEffect.Init();
		mgrUIObj.Init();
		talk.Init();
		
		PLAYER.Init();
		for (int i = 0; i < netPlayer.Length; i++)
		{
			netPlayer[i].Init();
		}
		/*
		//Debug.Log(PLAY_DATA.ins.status.equip[(int)ITEM_DATA.TYPE_2.BODY]);

		//BODY
		GameMgr.ins.mgrParts.SetPart(BONE.Spine1, ref PLAYER_MINI, 1);
		GameMgr.ins.mgrParts.SetPart(BONE.Spine1, ref PLAYER_MINI, 3);
		GameMgr.ins.mgrParts.SetPart(BONE.Spine1, ref PLAYER_MINI, 3);
		//GameMgr.ins.mgrParts.SetPart(BONE.Head, ref PLAYER);
		//ARML
		GameMgr.ins.mgrParts.SetPart(BONE.LUpperArm, ref PLAYER_MINI, 1);
		GameMgr.ins.mgrParts.SetPart(BONE.LForeArm, ref PLAYER_MINI, 1);
		//GameMgr.ins.mgrParts.SetPart(BONE.LHand, ref PLAYER);

		//ARMR
		GameMgr.ins.mgrParts.SetPart(BONE.RUpperArm, ref PLAYER_MINI, 1);
		GameMgr.ins.mgrParts.SetPart(BONE.RForeArm, ref PLAYER_MINI, 1);
		//GameMgr.ins.mgrParts.SetPart(BONE.RHand, ref PLAYER);

		//LEG
		GameMgr.ins.mgrParts.SetPart(BONE.Spine, ref PLAYER_MINI, 3);

		GameMgr.ins.mgrParts.SetPart(BONE.RThigh, ref PLAYER_MINI, 1);
		GameMgr.ins.mgrParts.SetPart(BONE.RCalf, ref PLAYER_MINI, 1);
		
		GameMgr.ins.mgrParts.SetPart(BONE.LThigh, ref PLAYER_MINI, 1);
		GameMgr.ins.mgrParts.SetPart(BONE.LCalf, ref PLAYER_MINI, 1);
		*/
		mgrNpc.Init();
		
		mgrUI.Init();
		
		mgrSave.SetPlanet(ref planet);

		mgrSave.Init();
		mgrField.Init();
		mgrSide.Init();

		mgrBlock.Init();
		
		mgrCam.Init();
		mgrMoveCam.Init();

		mgrSave.AddListBackAll();
		mgrSave.OnLoad();
		
		isInit = true;
	}

	public void GameStart()
	{
		//이동모드로 시작.
		GameMgr.ins.mgrUI.SetChangeModeMove();
	}
	
	void Update() 
	{
		if (isInit == false) return;

		talk.UpdateBefore();

		mgrCam.UpdateCameraMove();
		mgrField.UpdateWaterMove();

#if UNITY_EDITOR
		mgrUI.CheckKey();
#endif

		if (GameMgr.ins.mgrUI.PLAY_MODE != UIManager.MODE.MOVE) return;

		mgrUI.status.UpdateStatus();
		PLAYER.UpdataPlayer();
		for (l = 0; l < netPlayer.Length; l++) { netPlayer[l].UpdateNetPlayer(); }
		mgrNpc.UpdateNpc();
		mgrBullet.UpdateBullet();
		mgrEffect.UpdateEffect();
		mgrUIObj.UpdateObject();
		talk.UpdateTalk();
	}
	
	public void OnApplicationQuit()
	{
		if (mgrNetwork == null || PLAY_DATA.ins == null) return;
#if UNITY_EDITOR == false
		//mgrSave.OnSave(true);
#endif
		if (string.IsNullOrEmpty(PLAY_DATA.ins.status.userID) == false) mgrNetwork.SendUpdateData();
	}

	public void OnApplicationPause(bool pause)
	{
		if (mgrNetwork == null || PLAY_DATA.ins == null) return;
#if UNITY_EDITOR == false
		//if(pause) mgrSave.OnSave(true);
#endif
		if (string.IsNullOrEmpty(PLAY_DATA.ins.status.userID) == false) mgrNetwork.SendUpdateData();
	}
}