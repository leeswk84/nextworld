using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupQuest : UIPopupBase
{
	public ButtonObject[] tabs;

	public Color COL_ACTIVE;
	public Color COL_BASE;

	private int curTab;
	
	public override void Init()
	{
		type = TYPE.Quest;
		base.Init();

		curTab = 0;
		for (num = 0; num < tabs.Length; num++)
		{
			int idx = num;
			tabs[num].btn.onClick.AddListener(() => ClickTab(idx));
		}
		RefreshTab();
	}

	override public void Show()
	{
		base.Show();

	}

	private void ClickTab(int value)
	{
		curTab = value;
		for (num = 0; num < tabs.Length; num++)
		{
			tabs[num].spr.color = num == curTab ? COL_ACTIVE : COL_BASE;
			LeanTween.moveLocalY(tabs[num].obj, curTab == num ? -30f : -35f, 0.25f).setEaseOutBack();
		}
		RefreshTab();
	}

	private void RefreshTab()
	{

	}
}
