using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupBag : UIPopupBase
{
	public ButtonObject[] tabs;

	public Color COL_ACTIVE;
	public Color COL_BASE;

	public ButtonObject btnPageUp, btnPageDown;
	public Text txtPage;

	public RectTransform rectContent;
	public ScrollRect scroll;
	public RectTransform rectDrag;

	public UISlot prefab;
	private List<UISlot> list;

	private List<string> showData;

	private int snum;
	private int scroll_index;
	private int max;
	
	[HideInInspector]
	public int curTab;
	
	public override void Init()
	{
		type = TYPE.Bag;
		base.Init();

		btnPageDown.btn.onClick.AddListener(ClickPageDown);
		btnPageUp.btn.onClick.AddListener(ClickPageUp);
		
		showData = new List<string>();
		list = new List<UISlot>();
		scroll_index = 0;
		curTab = 0;

		for (num = 0; num < tabs.Length; num++)
		{
			int idx = num;
			tabs[num].btn.onClick.AddListener(() => ClickTab(idx));
		}

		scroll.onValueChanged.AddListener(ScrollDrag);

		vec3 = Vector3.zero;
		for (num = 0; num < 20; num++)
		{
			tmpSlot = GameObject.Instantiate(prefab.obj).GetComponent<UISlot>();
			tmpSlot.rect.SetParent(rectDrag);
			tmpSlot.rect.localScale = Vector3.one;
			vec3.x = (num % 5) * 64f;
			vec3.x += 35f;
			vec3.y = Mathf.RoundToInt(num / 5) * -65f;
			vec3.y -= 30f;
			tmpSlot.rect.localPosition = vec3;
			tmpSlot.Init();
			tmpSlot.icon.ActiveDrag = true;
			tmpSlot.popup = this;
			list.Add(tmpSlot);
		}
		prefab.obj.SetActive(false);
	}

	override public void Show()
	{
		base.Show();
		ClickTab(curTab);
	}

	public void RefreshTab()
	{
		ClickTab(curTab);
	}

	private void ClickTab(int value)
	{
		curTab = value;
		for (num = 0; num < tabs.Length; num++)
		{
			tabs[num].spr.color = num == curTab ? COL_ACTIVE : COL_BASE;
			LeanTween.moveLocalY(tabs[num].obj, curTab == num ? -30f : -35f, 0.25f).setEaseOutBack();
		}
		vec3 = rectContent.sizeDelta;
		vec3.y = Mathf.CeilToInt(max / 5) * 65;
		vec3.y += 65;
		rectContent.sizeDelta = vec3;

		showData.Clear();
		
		//for (num = 0; num < PLAY_DATA.ins.status.item.Count; num++)
		foreach (string str in PLAY_DATA.ins.status.item.Keys)
		{
			if ((curTab == 0 || PLAY_DATA.ins.dataItem.dic[PLAY_DATA.ins.status.item[str].idx].type1 == (ITEM_DATA.TYPE_1)(curTab))  //tab이 전체 이거나, type이 같거나
				&& PLAY_DATA.ins.status.item[str].cnt > 0) //갯수 0개 이상만
			{
				//showData.Add(PLAY_DATA.ins.status.item[num].idx);
				showData.Add(str);
			}
		}

		max = showData.Count;
		if(curTab == 0) max = PLAY_DATA.ins.status.item_max;
		
		RefreshList();
	}

	private void ClickPageUp()
	{
		scroll_index -= 4;
		if (scroll_index < 0) scroll_index = 0;
		RefreshList();
	}

	private void ClickPageDown()
	{
		scroll_index += 4;
		snum = Mathf.FloorToInt((max - 1) / 20) * 4;
		//snum = Mathf.CeilToInt(max / 20) * 4;
		if (scroll_index >= snum) scroll_index = snum;
		RefreshList();
	}

	private void ScrollDrag(Vector2 value)
	{
		num = Mathf.FloorToInt(rectContent.localPosition.y / 65f);
		//Debug.Log(num);
		if (num < 0) num = 0;
		if (scroll_index == num) return;
		scroll_index = num;
		
		RefreshList();
	}

	private void PrintPage()
	{
		snum = Mathf.CeilToInt((max - 1) / 20);
		
		txtPage.text = "";
		for (num = 0; num <= snum; num++)
		{
			if (num == Mathf.CeilToInt(scroll_index/4)) txtPage.text += "●";
			else txtPage.text += "○";
		}
	}

	private void RefreshList()
	{
		PrintPage();
		/*
		vec3 = rectDrag.localPosition;
		vec3.y = scroll_index * -65;
		rectDrag.localPosition = vec3;
		*/
		
		for (num = 0; num < list.Count; num++)
		{
			snum = num + (scroll_index * 5);
			if (max <= snum)
			{
				list[num].obj.SetActive(false);
				continue;
			}
			if(list[num].obj.activeSelf == false) list[num].obj.SetActive(true);

			if (PLAY_DATA.ins.status.item.Count <= snum || showData.Count <= snum)
			{
				list[num].txtE.text = "";
				list[num].icon.obj.SetActive(false);
				continue;
			}
			list[num].SetData(PLAY_DATA.ins.dataItem.dic[PLAY_DATA.ins.status.item[showData[snum]].idx], showData[snum]);
			//showData
			//list[num].SetData(PLAY_DATA.ins.dataItem.dic[PLAY_DATA.ins.status.item[snum].idx], snum);
		}
	}
}
