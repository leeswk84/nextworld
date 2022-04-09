public class TxtMgr 
{

	public const string layer_player = "Player";
	public const string layer_npc = "NPC";
	public const string layer_bullet = "Bullet";
	public const string layer_user_bullet = "BulletUser";
	public const string layer_ground = "Ground";
	public const string layer_build = "Build";
	public const string layer_UI = "UI";

	private static TxtMgr _ins;
	
	public static TxtMgr ins
	{
		get
		{
			if (_ins == null) _ins = new TxtMgr();
			return _ins;
		}
	}


	private System.Collections.Generic.Dictionary<TYPE, string> dic = new System.Collections.Generic.Dictionary<TYPE, string>();

	public enum TYPE 
	{
		YES = 0,
		NO,
		CANCEL,
		DAY,
		START,
		EDIT,
		OK,
		COMBO,
		CONNECT,
		DISCONNECT,
		ABILITY,
		PAUSE,
		JOIN,
		TUTORIAL,
		TUTORIAL_END,
		NEWVERSION,
		POPUP_TUTORIAL,
		POPUP_TUTORIAL_COMMENT,
		POPUP_TUTORIAL_END,
		OPTION,
		SNDON,
		SNDOFF,
		BGM,
		SND,
		POPUP_DISCONNECT,
		CREDIT,
		GOOGLECONNECT,
		SELLNG,
		CURRENTLNG,

		CAM_RESET,
		CAM_ROT,
		CAM_POS,
		GROUND_RESET,
		GROUND_UP,
		GROUND_DOWN,
		GROUND_UP_SAM,
		GROUND_DOWN_SAM,
		CAM_TYPE1,
		CAM_TYPE2,
		CAM_TYPE3,
		CAM_TYPE4,

		ROAD_MAKE,
		ROAD_DEL,
		ROAD_START,
		ROAD_END,

		GROUND_TYPE_DEFAULT,

		MOVE,
		DELETE,
		ROTATION,
		TRADE,

		MODE_BUILD,
		MODE_MOVE,

		COMMENT_NOT_BUILD,
		COMMENT_NOT_BUILD_ROAD,
		
		POPUP_SAVE_DOING,
		POPUP_SAVE_COMPLETE,

		CONNECTING,

		TUTORIAL_STEP = 1000,

	}

	public string lan = "";
	
	public int tutorialCnt = 0;
	
	public TxtMgr() 
	{
		SetKorean();
	}

	public string GetTxt(TYPE valueType)
	{
		if (!dic.ContainsKey(valueType)) return "";
		return dic[valueType];
	}
	/*
	private void AddTutorial(string str) 
	{
		tutorialCnt++;
		dic.Add(TYPE.TUTORIAL_STEP + tutorialCnt, str);
	}
	*/
	public void SetKorean() 
	{
		lan = "kor";

		dic[TYPE.POPUP_SAVE_DOING] = "저장 중입니다.";
		dic[TYPE.POPUP_SAVE_COMPLETE] = "저장 완료 했습니다.";

		dic[TYPE.COMMENT_NOT_BUILD] = "이 위치에는 건물을 지을 수 없습니다.";
		dic[TYPE.COMMENT_NOT_BUILD_ROAD] = "이 위치에는 도로가 있어서 건물을 지을 수 없습니다.";

		dic[TYPE.YES] = "예";
		dic[TYPE.NO] = "아니오";
		dic[TYPE.CANCEL] = "취소";
		dic[TYPE.DAY] = "일";
		dic[TYPE.START] = "시작";
		dic[TYPE.EDIT] = "수정";
		dic[TYPE.OK] = "확인";
		dic[TYPE.COMBO] = "연속";
		dic[TYPE.CONNECT] = "구글\n접속";
		dic[TYPE.DISCONNECT] = "접속\n해제";
		dic[TYPE.ABILITY] = "능력치";
		dic[TYPE.PAUSE] = "일시\n정지";
		dic[TYPE.JOIN] = "등록";
		dic[TYPE.TUTORIAL] = "튜토리얼";
		dic[TYPE.TUTORIAL_END] = "튜토리얼\n종료";
		dic[TYPE.NEWVERSION] = "새로운 버전이 준비 되었습니다.\n업데이트 받으러 가실까요?";

		dic[TYPE.POPUP_TUTORIAL] = "튜토리얼을 시작 하시겠습니까?";
		dic[TYPE.POPUP_TUTORIAL_COMMENT] = "[73d7d5][" + "" + "][-]에서 언제든지 튜토리얼 확인이 가능합니다.";
		dic[TYPE.POPUP_TUTORIAL_END] = "튜토리얼을 종료 하시겠습니까?";

		dic[TYPE.OPTION] = "설정";
		dic[TYPE.SNDON] = "들림";
		dic[TYPE.SNDOFF] = "안들림";
		dic[TYPE.BGM] = "배경음악";
		dic[TYPE.SND] = "효과음";
		dic[TYPE.POPUP_DISCONNECT] = "구글 접속을 해제 하시겠습니까?";
		dic[TYPE.CREDIT] = "[73d7d5]만든사람\n\n이상욱";
		dic[TYPE.GOOGLECONNECT] = "구글 계정 접속 중";
		
		dic[TYPE.SELLNG] = "언어 선택";
		dic[TYPE.CURRENTLNG] = "한글";

		//AddTutorial("튜토리얼을 시작 하겠습니다.");

		dic[TYPE.CAM_RESET] = "카메라\n초기화";
		dic[TYPE.CAM_ROT] = "카메라\n회전";
		dic[TYPE.CAM_POS] = "카메라\n이동";
		dic[TYPE.GROUND_RESET] = "잔디\n초기화";
		dic[TYPE.GROUND_UP] = "올림";
		dic[TYPE.GROUND_DOWN] = "내림";
		dic[TYPE.GROUND_UP_SAM] = "상단\n평행";
		dic[TYPE.GROUND_DOWN_SAM] = "하단\n평행";

		dic[TYPE.CAM_TYPE1] = "카메라\n버튼형";
		dic[TYPE.CAM_TYPE2] = "카메라\n잔디형";
		dic[TYPE.CAM_TYPE3] = "카메라\n상하형";
		dic[TYPE.CAM_TYPE4] = "카메라\n좌우형";

		dic[TYPE.ROAD_MAKE] = "도로\n생성";
		dic[TYPE.ROAD_DEL] = "도로\n제거";
		dic[TYPE.ROAD_START] = "도로\n시작";
		dic[TYPE.ROAD_END] = "도로\n끝";

		dic[TYPE.GROUND_TYPE_DEFAULT] = "기본";

		dic[TYPE.MOVE] = "이동";
		dic[TYPE.DELETE] = "제거";
		dic[TYPE.ROTATION] = "회전";
		dic[TYPE.TRADE] = "교체";

		dic[TYPE.MODE_BUILD] = "건설\n형태";
		dic[TYPE.MODE_MOVE] = "이동\n형태";

		dic[TYPE.CONNECTING] = "접속 중입니다...";
	}

	public void SetEnglish()
	{
		lan = "eng";

		dic[TYPE.YES] = "YES";
		dic[TYPE.NO] = "NO";
		dic[TYPE.CANCEL] = "CANCEL";
		dic[TYPE.DAY] = "DAY";
		dic[TYPE.START] = "START";
		dic[TYPE.EDIT] = "EDIT";
		dic[TYPE.OK] = "OK";
		dic[TYPE.COMBO] = "COMBO";
		dic[TYPE.CONNECT] = "CONNECT";
		dic[TYPE.DISCONNECT] = "DISCONNECT";
		dic[TYPE.ABILITY] = "ABILITY";
		dic[TYPE.PAUSE] = "PAUSE";
		dic[TYPE.JOIN] = "JOIN";
		dic[TYPE.TUTORIAL] = "TUTORIAL";
		dic[TYPE.TUTORIAL_END] = "TUTORIAL\nEND";

		dic[TYPE.OPTION] = "OPTION";// "설정";
		dic[TYPE.SNDON] = "ON";// "들림";
		dic[TYPE.SNDOFF] = "OFF";// "안들림";
		dic[TYPE.BGM] = "BGM";//"배경음악";
		dic[TYPE.SND] = "SOUND";//"효과음";
		dic[TYPE.POPUP_DISCONNECT] = "??";// "구글 접속을 해제 하시겠습니까?";
		dic[TYPE.CREDIT] = "[73D7D5]CREDIT\nSANG WK LEE";// "[73d7d5]만든사람\n\n이상욱";
		dic[TYPE.GOOGLECONNECT] = "GOOGLE CONNECTING..";// "구글 계정 접속 중";

		dic[TYPE.SELLNG] = "SELECT LANGUAGE";
		dic[TYPE.CURRENTLNG] = "ENGLESH";

		dic[TYPE.CONNECTING] = "CONNECTING...";
	}

}
