using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupSkill : UIPopupBase
{
	override public void Init()
	{
		type = TYPE.Skill;
		base.Init();
	}
}