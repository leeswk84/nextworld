using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStatus : MonoBehaviour
{
	public GameObject objHP, objNut, objMP;
	//public RectTransform rectHP, rectHP_delay;
	public Slider sliderHp, sliderHp_delay;
	public Slider sliderMp, sliderMp_delay;
	public Text txtHP, txtNut, txtMP;

	public Text txtFPS;
	public UINetUser[] netuser;

	[Header("")]
	public GameObject objRight;
	public ButtonObject btnStatus;
	public ButtonObject btnBag;
	public ButtonObject btnSkill;
	public ButtonObject btnQuest;

	[Header("")]
	public RectTransform rectPopup;
	public UIPopupStatus popupStatus;
	public UIPopupBag popupBag;
	public UIPopupSkill popupSkill;
	public UIPopupQuest popupQuest;
	public UIPopupHotKey hotKey;
	public UIPopupItemDetail popupItemDetail;
	public UIPopupShop popupShop;
	public UIPopupMessage popupMessage;

	//private float maxwHP;
	private float printHP;
	private float dirHP;
	
	private long printNut;
	private float dirnut;

	private float printMP;
	private float dirMP;

	private Vector2 vec2;

	private List<UIPopupBase> listReOpen;

	private float deltaTime = 0;

	private float time_mp;
	private float time_gap_mp;
	private int add_mp;
	
	private int num;
	private const string STR_NUT = "{0:###,###,###,##0}";
	private const string STR_HP = "{0} / {1}";
	public void Init()
	{
		listReOpen = new List<UIPopupBase>();
		
		add_mp = 5;
		time_gap_mp = 0.5f;
		time_mp = Time.time;

		printHP = GameMgr.ins.PLAYER_MINI.ability.HP;
		txtHP.text = string.Format(STR_HP, GameMgr.ins.PLAYER_MINI.ability.HP , GameMgr.ins.PLAYER_MINI.ability.HPmax);
		printNut = PLAY_DATA.ins.status.nut;
		txtNut.text = string.Format(STR_NUT, PLAY_DATA.ins.status.nut);
		printMP = GameMgr.ins.PLAYER_MINI.ability.MP;
		txtMP.text = string.Format(STR_HP, GameMgr.ins.PLAYER_MINI.ability.MP, GameMgr.ins.PLAYER_MINI.ability.MPmax);

		popupStatus.Init();
		popupBag.Init();
		popupSkill.Init();
		popupQuest.Init();
		hotKey.Init();
		popupItemDetail.Init();
		popupShop.Init();
		popupMessage.Init();

		btnStatus.btn.onClick.AddListener(popupStatus.Toggle);
		btnBag.btn.onClick.AddListener(popupBag.Toggle);
		btnSkill.btn.onClick.AddListener(popupSkill.Toggle);
		btnQuest.btn.onClick.AddListener(popupQuest.Toggle);
		
		txtFPS.text = "";
	}
	
	public void UpdateStatus()
	{
		//Application.targetFrameRate = 60;
		//float deltaTime = 0;
		AddMp();

		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
		txtFPS.text = "FPS:" + Mathf.Ceil(1.0f / deltaTime).ToString();
	}
	/// <summary>
	/// HP 수치 업데이트
	/// </summary>
	public void EditHP(int value)
	{
		if (value < 0 && GameMgr.ins.PLAYER_MINI.ability.HP < 1) return; //게임 오버

		/*
		if (GameMgr.ins.PLAYER_MINI.ability.HP + value > GameMgr.ins.PLAYER_MINI.ability.HPmax)
		{	//최대치
			value = Mathf.FloorToInt(GameMgr.ins.PLAYER_MINI.ability.HPmax - GameMgr.ins.PLAYER_MINI.ability.HP);
		}
		*/

		dirHP = GameMgr.ins.PLAYER_MINI.ability.HP + value;

		if (dirHP <= 0)
		{
			dirHP = 0;
			if (GameMgr.ins.PLAYER.is_intro == false)
			{
				GameMgr.ins.PLAYER.DestroyIntro();
			}
		}
		
		if(dirHP > GameMgr.ins.PLAYER_MINI.ability.HPmax) dirHP = GameMgr.ins.PLAYER_MINI.ability.HPmax;

		LeanTween.cancel(objHP);
		LeanTween.value(objHP, UpdateHP, printHP,  dirHP, 0.5f);

		//LeanTween.value(objHP, UpdateHPBar, rectHP.sizeDelta.x, dirHP / (float)(GameMgr.ins.PLAYER_MINI.ability.HPmax) * maxwHP, 0.5f);
		//LeanTween.value(objHP, UpdateHPDelay, rectHP_delay.sizeDelta.x, dirHP / (float)(GameMgr.ins.PLAYER_MINI.ability.HPmax) * maxwHP, 0.3f).setDelay(0.7f);
		LeanTween.value(objHP, UpdateHPBar, sliderHp.value, dirHP / (float)(GameMgr.ins.PLAYER_MINI.ability.HPmax), 0.5f);
		LeanTween.value(objHP, UpdateHPDelay, sliderHp_delay.value, dirHP / (float)(GameMgr.ins.PLAYER_MINI.ability.HPmax), 0.3f).setDelay(0.7f);


		//GameMgr.ins.PLAYER_MINI.ability.HP += value;
		GameMgr.ins.PLAYER_MINI.ability.HP = dirHP;
	}

	private void UpdateHP(float value)
	{
		printHP = Mathf.RoundToInt(value);
		txtHP.text = string.Format(STR_HP, printHP , GameMgr.ins.PLAYER_MINI.ability.HPmax);
	}
	private void UpdateHPBar(float value)
	{
		sliderHp.value = value;
		//vec2 = rectHP.sizeDelta;
		//vec2.x = value;
		//rectHP.sizeDelta = vec2;
	}
	private void UpdateHPDelay(float value)
	{
		sliderHp_delay.value = value;
		//vec2 = rectHP_delay.sizeDelta;
		//vec2.x = value;
		//rectHP_delay.sizeDelta = vec2;
	}

	private void AddMp()
	{
		if (time_mp < Time.time)
		{
			EditMP(add_mp);
			time_mp = Time.time + time_gap_mp;
		}
	}

	public bool EditMP(int value)
	{
		if (value < 0) time_mp = Time.time + (time_gap_mp * 4);
		if (value < 0 && GameMgr.ins.PLAYER_MINI.ability.MP + value < 0) return false; //스킬 사용 불가
		if (value > 0 && GameMgr.ins.PLAYER_MINI.ability.MP == GameMgr.ins.PLAYER_MINI.ability.MPmax) return false;

		dirMP = GameMgr.ins.PLAYER_MINI.ability.MP + value;
		if (dirMP > GameMgr.ins.PLAYER_MINI.ability.MPmax) dirMP = GameMgr.ins.PLAYER_MINI.ability.MPmax;

		LeanTween.cancel(objMP);
		LeanTween.value(objMP, UpdateMP, printMP, dirMP, 0.5f);
		LeanTween.value(objMP, UpdateMPBar, sliderMp.value, dirMP / (float)(GameMgr.ins.PLAYER_MINI.ability.MPmax), 0.5f);
		LeanTween.value(objMP, UpdateMPDelay, sliderMp_delay.value, dirMP / (float)(GameMgr.ins.PLAYER_MINI.ability.MPmax), 0.3f).setDelay(0.7f);
		
		GameMgr.ins.PLAYER_MINI.ability.MP = dirMP;

		return true;

	}
	
	private void UpdateMP(float value)
	{
		printMP = Mathf.RoundToInt(value);
		txtMP.text = string.Format(STR_HP, printMP, GameMgr.ins.PLAYER_MINI.ability.MPmax);
	}

	private void UpdateMPBar(float value) {	sliderMp.value = value;	}
	private void UpdateMPDelay(float value)	{ sliderMp_delay.value = value; }
	
	public void EditNut(int value)
	{
		if (PLAY_DATA.ins.status.nut < 0) return;

		dirnut = PLAY_DATA.ins.status.nut + value;
		LeanTween.cancel(objNut);
		LeanTween.value(objNut, UpdateNut, printNut, dirnut, 0.5f);

		PLAY_DATA.ins.status.nut += value;
	}

	private void UpdateNut(float value)
	{
		printNut = Mathf.RoundToInt(value);
		txtNut.text = string.Format(STR_NUT, printNut);
	}

	public void ShowUIRight()
	{
		//objRight.SetActive(true);
		LeanTween.moveLocalX(objRight, 0, 0.3f).setEaseOutQuart();
		//for(num = 0; num < listReOpen.Count; num++) listReOpen[num].Show();
	}

	public void HideUIRight()
	{
		//objRight.SetActive(false);
		LeanTween.moveLocalX(objRight, 110, 0.3f).setEaseOutQuart();

		listReOpen.Clear();
		if (popupStatus.obj.activeSelf == true) listReOpen.Add(popupStatus);
		if (popupBag.obj.activeSelf == true) listReOpen.Add(popupBag);
		if (popupSkill.obj.activeSelf == true) listReOpen.Add(popupSkill);
		if (popupQuest.obj.activeSelf == true) listReOpen.Add(popupQuest);
		//if (popupNetwork.obj.activeSelf == true) listReOpen.Add(popupNetwork);

		if (string.IsNullOrEmpty(popupStatus.btnLock.txt.text) == true) popupStatus.Hide();
		if (string.IsNullOrEmpty(popupBag.btnLock.txt.text) == true) popupBag.Hide();
		if (string.IsNullOrEmpty(popupSkill.btnLock.txt.text) == true) popupSkill.Hide();
		if (string.IsNullOrEmpty(popupQuest.btnLock.txt.text) == true) popupQuest.Hide();
		popupMessage.Hide();

		popupShop.Hide();
		popupItemDetail.Hide();
	}
}
