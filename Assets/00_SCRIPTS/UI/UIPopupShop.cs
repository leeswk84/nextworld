using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPopupShop : UIPopupBase
{
	public override void Init()
	{
		type = TYPE.Shop;
		base.Init();
		btnLock.obj.SetActive(false);

	}

	override public void Show()
	{
		base.Show();

	}

}
