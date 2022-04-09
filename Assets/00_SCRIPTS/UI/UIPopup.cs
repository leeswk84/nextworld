using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIPopup : MonoBehaviour
{
	public GameObject obj;
	public Transform tran;
	public RectTransform rectBg;
	public ButtonObject btnOK, btnCancel;
	public Text comment;

	private Vector3 vec3;

	private Action callOK;
	private Action callCancel;

	public void Init()
	{
		obj.SetActive(false);

		btnOK.btn.onClick.AddListener(OnClickOK);
		btnCancel.btn.onClick.AddListener(OnClickCancel);

		//Show("AAAAAAAaaaaaaaaaaaaaaaaaaaa\n\n\n\n\n\n\n\nn\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\nAAAAAAAAAAAAAAAAA|AA|BB",null,null);
	}

	public void Show(TxtMgr.TYPE str) {	Show(TxtMgr.ins.GetTxt(str)); }

	public void Show(string str)
	{
		obj.SetActive(true);

		callCancel = null;
		callOK = null;

		comment.text = str;
		btnOK.txt.text = TxtMgr.ins.GetTxt(TxtMgr.TYPE.OK);
		btnCancel.txt.text = TxtMgr.ins.GetTxt(TxtMgr.TYPE.CANCEL);
		
		vec3.z = 0;
		vec3.x = rectBg.sizeDelta.x;//700;
		vec3.y = comment.preferredHeight + 200;
		if (vec3.y < 500) vec3.y = 500;

		rectBg.sizeDelta = vec3;

		SetBtnPosition(false);
	}

	public void ShowLock(TxtMgr.TYPE str)
	{
		Show(str);
		btnOK.obj.SetActive(false);
	}

	private void SetBtnPosition( bool isYesNo = false )
	{
		vec3.y = rectBg.sizeDelta.y;
		vec3.y = ((vec3.y * 0.5f) - 90f) * -1;
		vec3.x = isYesNo ? 120 : 0;
		btnOK.tran.localPosition = vec3;
		vec3.x = -120;
		btnOK.obj.SetActive(true);
		btnCancel.tran.localPosition = vec3;
		btnCancel.obj.SetActive(isYesNo);
	}

	public void Show(string str, System.Action fncOK, System.Action fncCancel = null)
	{
		string[] strs = str.Split('|');
		Show(strs[0]);
		if (strs.Length > 1) btnOK.txt.text = strs[1];
		if (strs.Length > 2) btnCancel.txt.text = strs[2];

		callOK = fncOK;
		callCancel = fncCancel;

		SetBtnPosition(true);
	}

	public void Close()
	{
		obj.SetActive(false);
	}

	private void OnClickOK()
	{
		if (callOK != null) callOK();
		Close();
	}

	private void OnClickCancel()
	{
		if (callCancel != null) callCancel();
		Close();
	}
#if UNITY_EDITOR
	public void Update()
	{
		if (btnOK.obj.activeSelf && Input.GetKeyUp(KeyCode.Z))
		{
			OnClickOK();
		}
	}
#endif
}
