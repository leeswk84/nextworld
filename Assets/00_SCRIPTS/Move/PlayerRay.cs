using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRay : MonoBehaviour
{
	//public HighlightsFX outline;
	public Camera cam_outline;
	public Camera cam_main;

	private Ray ray;
	private RaycastHit hit;

	private Vector3 vec_ray_pos;

	private int layer_ray;
	private bool show_outline;

	public void Init()
	{
		show_outline = false;
		//layer_ray = (-1) - ((1 << LayerMask.NameToLayer("10번레이어이름")) | (1 << LayerMask.NameToLayer("11번레이어이름")));
		layer_ray = (1 << LayerMask.NameToLayer(TxtMgr.layer_bullet)) + (1 << LayerMask.NameToLayer("EventHit"));// & (1 << LayerMask.NameToLayer(TxtMgr.layer_player));
		layer_ray = ~layer_ray;
		vec_ray_pos = new Vector3(0.5f, 0.53f, 0f);
		//
		//layer_ray = 1 << LayerMask.NameToLayer(TxtMgr.layer_ground);

		//UpdateOutLine();
		//outline.Init();
	}

	public void InitStartMove()
	{

	}

	public void UpdateOutLine()
	{
		//ray = Camera.main.ViewportPointToRay(vec_ray_pos);
		ray = cam_main.ViewportPointToRay(vec_ray_pos);
		if (Physics.Raycast(ray, out hit, 1000f, layer_ray) == false 
			|| hit.collider.name == GameMgr.ins.PLAYER_MINI.obj.name)
		{
			SetOutLine(false, true);
		}
		else SetOutLine(true, true);
	}
	
	public void SetOutLine(bool value, bool isray = false)
	{
		//if (value == cam_outline.enabled) return;
		if (GameMgr.ins.PLAYER.is_intro)
		{
			GameMgr.ins.PLAYER_MINI.SetOutLine(false);
			show_outline = false;
			return;
		}

		if (isray == true && value == show_outline) return;
		show_outline = value;
		
		//cam_outline.enabled = value;
		GameMgr.ins.PLAYER_MINI.SetOutLine(value);
		//outline.HideOutLine();
	}
	
}
