using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mini : MonoBehaviour 
{
	public ABILITY ability = new ABILITY();

	private Mini mini;

	public GameObject obj;
	public Transform trans;
	public Transform head;
	public Transform body;
	public CapsuleCollider cap;
	
	public Transform tranModelLeg, tranModelHead;

	public Transform[] tranBones;
	public GameObject objBoneBody;
	public Transform tranTarget;
	public Transform tranHitCheck;

	public MiniAni ani;
	[HideInInspector]
	public bool is_show_outline;

	[HideInInspector]
	public Vector3 backPosition;
	[HideInInspector]
	public Vector3 backForward;
	[HideInInspector]
	public float dust_delay = 0f;
	
	private List<Parts> listOutline;

	private Parts tmpPart;
	
	private int num;
	private Vector3 vec3;

	private BONE bone;

	public Dictionary<BONE, Parts> dicPart;

	public Vector3 moveForce; //밀리는 힘

	public float rader;
	public Dictionary<ITEM_DATA.TYPE_2, EQUIP_DATA> dicEquip;

	private ITEM_DATA.TYPE_2[] arrEtype;

	public void Init ()
	{
		mini = this;
		moveForce = Vector3.zero;

		ability.Init();
		listOutline = new List<Parts>();

		dicPart = new Dictionary<BONE, Parts>();
		dicEquip = new Dictionary<ITEM_DATA.TYPE_2, EQUIP_DATA>();

		backPosition = trans.position;
		ani.Init();

		arrEtype = new ITEM_DATA.TYPE_2[]
		{
			ITEM_DATA.TYPE_2.BODY,
			ITEM_DATA.TYPE_2.FOOT,
			ITEM_DATA.TYPE_2.ARML,
			ITEM_DATA.TYPE_2.ARMR
		};

	}

	/// <summary>
	/// 파츠 장비
	/// </summary>
	/// <param name="value"></param>
	public void EquipPart(EQUIP_DATA value, ITEM_DATA.TYPE_2 type2)
	{
		//Debug.Log(type2 + ":" + value.id + ":" + PLAY_DATA.ins.dataEquip.dic[value.id].model.Length);
		if(dicEquip.ContainsKey(type2) == false) dicEquip.Add(type2, new EQUIP_DATA());

		dicEquip[type2] = value;

		for (num = 0; num < value.model.Length; num++)
		{
			//switch (PLAY_DATA.ins.dataItem.dic[value.id].type2)
			switch (type2)
			{
			case ITEM_DATA.TYPE_2.BODY:
				bone = BONE.Spine1;
				//레이더 정보
				rader = PLAY_DATA.ins.dataSkill.dic[value.skill].values[SKILL_DATA.VTYPE.lengthlook];
				break;
			case ITEM_DATA.TYPE_2.FOOT:
				switch (num)
				{
					case 0: bone = BONE.Spine; break;
					case 1: bone = BONE.RThigh; break;
					case 2: bone = BONE.RCalf; break;
					case 3: bone = BONE.RFoot; break;
					//case 4: bone = BONE.LThigh; break;
					//case 5: bone = BONE.LCalf; break;
					//case 6: bone = BONE.LFoot; break;
					
				}
				break;
			case ITEM_DATA.TYPE_2.ARML:
				switch (num)
				{
					case 0: bone = BONE.LUpperArm; break;
					case 1: bone = BONE.LForeArm; break;
					case 2: bone = BONE.LHand; break;
				}
				break;
			case ITEM_DATA.TYPE_2.ARMR:
				switch (num)
				{
					case 0: bone = BONE.RUpperArm; break;
					case 1: bone = BONE.RForeArm; break;
					case 2: bone = BONE.RHand; break;
				}
				break;
			}
			//Debug.Log(type2+":" +value.id+":"+num+":"+bone+":" +PLAY_DATA.ins.dataEquip.dic[value.id].model[num]);
			GameMgr.ins.mgrParts.SetPart(bone, ref mini, value.model[num]);
			if (type2 == ITEM_DATA.TYPE_2.FOOT)
			{
				switch (num)
				{
					case 1: GameMgr.ins.mgrParts.SetPart(BONE.LThigh, ref mini, value.model[num]); break;
					case 2: GameMgr.ins.mgrParts.SetPart(BONE.LCalf, ref mini, value.model[num]); break;
					case 3: GameMgr.ins.mgrParts.SetPart(BONE.LFoot, ref mini, value.model[num]); break;
				}
			}
		}
	}

	public bool RemovePart(BONE value_bone, string str)
	{
		if (dicPart.ContainsKey(value_bone))
		{
			if (dicPart[value_bone].obj.name == str) return false;
			dicPart[value_bone].outLine.ui.rect.SetParent(GameMgr.ins.mgrUIObj.tran);
			dicPart[value_bone].outLine.ui.obj.SetActive(false);
			listOutline.Remove(dicPart[value_bone]);
			GameObject.Destroy(dicPart[value_bone].obj);
			dicPart.Remove(value_bone);
		}
		return true;
	}

	public void SetPart(BONE value_bone, ref GameObject prefab, bool isPrefab)
	{
		//Debug.Log(value_bone);
		if(RemovePart(value_bone, prefab.name) == false) return;
		
		tmpPart = ((GameObject)GameObject.Instantiate(prefab)).GetComponent<Parts>();
		tmpPart.obj.name = prefab.name;

		dicPart.Add(value_bone, tmpPart);

		vec3 = tmpPart.tran.localScale;
		tmpPart.obj.SetActive(true);
		tmpPart.tran.SetParent(tranBones[(int)value_bone]);
		tmpPart.tran.localScale = vec3;
		tmpPart.tran.localPosition = Vector3.zero;
		tmpPart.tran.localRotation = Quaternion.identity;

		//value.objOutline.GetComponent<ParticleSystemRenderer>().material = GameMgr.ins.mgrParts.matOutline;
		//value.particle.gameObject.GetComponent<ParticleSystemRenderer>().maxParticleSize = 0.1f;

		tmpPart.outLine = GameObject.Instantiate(GameMgr.ins.mgrParts.prefabOutLine.obj).GetComponent<PartsOutLine>();
		tmpPart.outLine.tran.parent = tmpPart.objOutline.transform.parent;
		tmpPart.outLine.tran.localScale = Vector3.one;
		tmpPart.outLine.tran.localPosition = Vector3.zero;

		GameObject.Destroy(tmpPart.objOutline);
		tmpPart.objOutline = tmpPart.outLine.obj;
		tmpPart.particle = tmpPart.outLine.particle;
		tmpPart.objOutline.SetActive(false);

		if (GameMgr.ins.mgrUIObj != null)
		{
			tmpPart.outLine.ui = GameMgr.ins.mgrUIObj.GetObject(UIObject.UI_KEY_OUTLINE);
			tmpPart.outLine.ui.tranTarget = tmpPart.tran;
			tmpPart.outLine.ui.obj.SetActive(true);
		}

		if (value_bone == BONE.Spine1)
		{
			vec3.y = vec3.x = 12f;
			vec3.z = 1f;
			tmpPart.outLine.ui.rectImg.sizeDelta = vec3;
		}
		//value.UpdateMapping();
		//dicPart.Add(value_bone, value);
		listOutline.Add(tmpPart);
		
		/*
#if UNITY_EDITOR
		if (isPrefab) UnityEditor.PrefabUtility.ConnectGameObjectToPrefab(tmpPart.obj, prefab);
#endif
		*/

	}

	public void SetOutLine(bool value)
	{
		is_show_outline = value;
		for (num = 0; num < listOutline.Count; num++)
		{
			if (listOutline[num] == null || listOutline[num].objOutline == null) continue;
			if (listOutline[num].outLine == null) continue;
			listOutline[num].outLine.ui.obj.SetActive(value);
			/*
			listOutline[num].objOutline.SetActive(value);
			if(value) listOutline[num].particle.Play();
			if(!value) listOutline[num].particle.Stop();
			*/
		}
	}
	/// <summary>
	/// 스킬 능력치
	/// </summary>
	/// <param name="type2">파츠 부위</param>
	/// <param name="vtype">능력치 종류</param>
	/// <returns></returns>
	public float GetSkillValue(ITEM_DATA.TYPE_2 type2, SKILL_DATA.VTYPE vtype)
	{
		return PLAY_DATA.ins.dataSkill.dic[mini.dicEquip[type2].skill].GetValue(vtype);
	}

	/// <summary>
	/// 스킬 타입
	/// </summary>
	/// <param name="type2">파츠 부위</param>
	/// <returns></returns>
	public SKILL_DATA.ATYPE GetSkillType(ITEM_DATA.TYPE_2 type2)
	{
		return PLAY_DATA.ins.dataSkill.dic[mini.dicEquip[type2].skill].atype;
	}

	public SKILL_DATA GetSkillData(ITEM_DATA.TYPE_2 type2)
	{
		return PLAY_DATA.ins.dataSkill.dic[mini.dicEquip[type2].skill];
	}

	public void RefreshMaxAbility(bool isupdateui = true)
	{
		ability.HPmax = 0;
		for (int i = 0; i < arrEtype.Length; i++)
		{
			if (dicEquip.ContainsKey(arrEtype[i]) == false) continue;
			if (dicEquip[arrEtype[i]] == null) continue;
			//Debug.Log(PLAY_DATA.ins.dataEquip.dic[dicEquip[arrEtype[i]]].hp);
			ability.HPmax += dicEquip[arrEtype[i]].hp;
			ability.HPmax += GetSkillValue(arrEtype[i], SKILL_DATA.VTYPE.addhp);
		}

		ability.MPmax = GetSkillValue(ITEM_DATA.TYPE_2.BODY, SKILL_DATA.VTYPE.mpmax);

		if (ability.HP > ability.HPmax) ability.HP = ability.HPmax;
		if (ability.MP > ability.MPmax) ability.MP = ability.MPmax;

		if (isupdateui && ani.player != null)
		{
			GameMgr.ins.mgrUI.status.EditHP(0);
			GameMgr.ins.mgrUI.status.EditMP(0);
		}
	}

}
