using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIObject : MonoBehaviour
{
	public const string UI_KEY_HP = "UI_KEY_HP";
	public const string UI_KEY_OUTLINE = "UI_KEY_OUTLINE";
	public const string UI_KEY_TXT = "UI_KEY_TXT";
	public const string UI_EVENT = "UI_EVENT";

	private const string STR_DIS = "{0} m";

	[HideInInspector]
	public int index;
	[HideInInspector]
	public string key;

	public GameObject obj;
	public GameObject objHP;
	public GameObject objTarget;
	public RectTransform rectTarget;
	public GameObject objTaretMotion;

	public RectTransform rect;
	public RectTransform rectImg;

	public GameObject objSlider;

	[HideInInspector]
	public string printText;
	public Text txt;
	public Slider slider;
	public Image img;

	[HideInInspector]
	public MiniNPC npc;
	
	public Transform tranTarget;
	public Text txtDistance;

	public bool is_nut;

	private float hide_time;

	public float degreeY;

	public void Init()
	{
		degreeY = 0;
		is_nut = false;
		hide_time = -1;
		obj.SetActive(false);
		if (objHP != null) objHP.SetActive(false);
		if (objTarget != null) objTarget.SetActive(false);
		if (txtDistance != null) txtDistance.text = string.Empty;
	}

	public void Show()
	{
		obj.SetActive(true);
		switch (key)
		{
			case UI_KEY_TXT:
				if (is_nut)
				{
					txt.color = GameMgr.ins.mgrUIObj.colorNut;
					LeanTween.moveLocalX(obj, rect.localPosition.x + Random.Range(-15f, 15f), 0f);
					LeanTween.moveLocalY(obj, rect.localPosition.y + Random.Range(30f, 50f), 0.7f).setEaseOutSine();
					Invoke("Hide", 0.7f);
					is_nut = false;
					return;
				}
				txt.color = Color.white;
				LeanTween.moveLocalY(obj, rect.localPosition.y + Random.Range(50f, 70f), 0.5f).setEaseOutSine();
				LeanTween.moveLocalY(obj, rect.localPosition.y + Random.Range(30f, 40f), 0.2f).setEaseInSine().setDelay(0.5f);
				//LeanTween.alpha(rect, 0f, 0.2f).setDelay(0.5f);
				LeanTween.moveLocalX(obj, rect.localPosition.x + Random.Range(-70f, 70f), 0.7f);//.setEaseOutSine();
				Invoke("Hide", 0.7f);
				break;
			case UI_KEY_HP:
				objHP.SetActive(true);
				break;
		}

	}

	public void UpdateObject()
	{
		if (hide_time != -1 && hide_time < Time.time)
		{
			Hide();
			return;
		}

		switch (key)
		{
			case UI_EVENT:
			case UI_KEY_OUTLINE:
				UpdateTargetPosition();
				if (printText.Length > 1)
				{
					txt.text = printText.Substring(0, Mathf.CeilToInt((Time.time * 2) % (printText.Length)));
				}
				break;
			case UI_KEY_HP:
				UpdateAtkTaretPosition();
				if (objSlider.activeSelf == false) break;
				
				if(Vector3.Distance(tranTarget.position, GameMgr.ins.PLAYER.mini.trans.position) > 11)
				{
					if(objHP.activeSelf == true) objHP.SetActive(false);
					return;
				}
				else if(objHP.activeSelf == false)
				{
					 objHP.SetActive(true);
				}
				UpdateTargetPosition();
				break;
		}
	}

	public void MoveTxt()
	{

	}

	public void ShowTarget()
	{
		if (objTarget.activeSelf == false)
		{
			objTarget.SetActive(true);
			//UpdateTarget(100f);
			LeanTween.cancel(objTaretMotion);
			LeanTween.rotateAroundLocal(objTaretMotion, Vector3.back, 360f, 10f).setLoopClamp();
			LeanTween.value(objTaretMotion, UpdateTarget, 100f, 55f, 0.38f).setEaseOutExpo();
		}
	}

	public void HideTarget()
	{
		if (objTarget.activeSelf == true)
		{
			LeanTween.cancel(objTaretMotion);
			objTarget.SetActive(false);
			txtDistance.text = string.Empty;
		}
	}

	public void UpdateTarget(float value)
	{
		rectTarget.sizeDelta = Vector2.one * value;
	}

	public void UpdateTargetPosition()
	{
		//GameMgr.ins.mgrUIObj.vec3 = GameMgr.ins.mgrUIObj.Change3D2D(ref tranTarget);
		//GameMgr.ins.mgrUIObj.vec3.y += degreeY;
		//rect.localPosition = GameMgr.ins.mgrUIObj.vec3;
		rect.localPosition = GameMgr.ins.mgrUIObj.Change3D2D(ref tranTarget, degreeY);
	}

	public void UpdateAtkTaretPosition()
	{
		if (objTarget == null || npc == null) return;

		if (npc.cur_state == MiniNPC.STATE.HIDE
			|| npc.cur_state == MiniNPC.STATE.NONE
			|| npc.cur_state == MiniNPC.STATE.MAX)
		{
			if (objTarget.activeSelf) HideTarget();
		}
		if (objTarget.activeSelf)
		{
			rectTarget.localPosition = GameMgr.ins.mgrUIObj.Change3D2D(ref npc.tranTarget) - rect.localPosition;
			txtDistance.text = string.Format(STR_DIS, Mathf.RoundToInt(Vector3.Distance(npc.mini.trans.position, GameMgr.ins.PLAYER.mini.trans.position)*10f)*0.1f);
		}
	}
	
	public void Hide()
	{
		obj.SetActive(false);
	}


}
