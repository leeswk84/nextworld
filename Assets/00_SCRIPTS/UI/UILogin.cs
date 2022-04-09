using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILogin : MonoBehaviour
{
	public GameObject obj;
	public ButtonObject btnLogin;
	public UnityEngine.UI.InputField inputAccount;


	public void Init()
	{
		btnLogin.btn.onClick.AddListener(ClickLogin);

	}

	private void ClickLogin()
	{
		if (string.IsNullOrEmpty(inputAccount.text))
		{
			GameMgr.ins.mgrUI.popup.Show("Not Empty.");
			return;
		}

		GameMgr.ins.mgrNetwork.SendLogin(inputAccount.text);
		//Debug.Log("ClickLogin");
	}
}
