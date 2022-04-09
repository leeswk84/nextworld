using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkybox : MonoBehaviour
{
	public RectTransform rectSkybox;
	private float dir;
	private float beforex = 0;
	private Vector3 vec3 = Vector3.zero;

	public void UpdateSkybox()
	{
		//Debug.Log(GameMgr.ins.PLAYER_MINI.trans.position.y);
		//515, 650
		
		// 0 = -75
		// 1.31 = -145
		
		vec3 = rectSkybox.localPosition;
		//dir = (GameMgr.ins.PLAYER_MINI.trans.position.y + 1.5f) * -50;//-50;
		dir = (GameMgr.ins.PLAYER_MINI.trans.position.y * -53.4f) + -75f;//-50;
		vec3.y = dir;
		//if (dir < -85) dir = -85;
		//vec3.y -= (vec3.y - dir) *0.2f;

		/*
		dir = (GameMgr.ins.PLAYER_MINI.trans.position.x - beforex) * 10;
		if (dir > 5 || dir < -5) dir = 0; 
		vec3.x -= dir;
		if (vec3.x < -700) vec3.x = 80;
		if (vec3.x > 140) vec3.x = -640;
		*/

		rectSkybox.localPosition = vec3;
		beforex = GameMgr.ins.PLAYER_MINI.trans.position.x;

		vec3 = GameMgr.ins.LowBuffer.rectBgPlane.localPosition;
		vec3.y = dir + 654f;
		GameMgr.ins.LowBuffer.rectBgPlane.localPosition = vec3;
	}	

}
