using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRT_TOOL : MonoBehaviour
{
	[Header("게임 메인씬으로 플레이 여부")]
	public bool IS_PLAY_MAIN;

	public static CRT_TOOL ins;

	public Mini PLAYER;
	
	private Parts tmpPart;

	public Camera cam;
	public Camera uicam;
	public Canvas canvas;
	
	public UIVertexPoint uiVertex;

	[HideInInspector]
	public bool IS_SET_CAMERA;

	public ButtonObject btnPlay;

	public void Start()
	{
		if (IS_PLAY_MAIN == true)
		{	//게임 메인으로 플레이
			UnityEngine.SceneManagement.SceneManager.LoadScene(0);
			return;
		}

		if (ins == null) ins = this;
		else
		{
			GameObject.Destroy(gameObject);
			return;
		}

		IS_SET_CAMERA = true;
		uiVertex.Init();
		PLAYER.Init();
		btnPlay.btn.onClick.AddListener(ClickPlay);

		GameMgr.ins.mgrAtlas.Init();
		GameMgr.ins.mgrParts.Init();

		GameMgr.ins.mgrParts.SetPart(BONE.Spine, ref PLAYER, 0 , true);
		GameMgr.ins.mgrParts.SetPart(BONE.Spine1, ref PLAYER, 0, true);
		//GameMgr.ins.mgrParts.SetPart(BONE.Head, ref PLAYER, 0, true);

		GameMgr.ins.mgrParts.SetPart(BONE.LUpperArm, ref PLAYER, 0, true);
		GameMgr.ins.mgrParts.SetPart(BONE.LForeArm, ref PLAYER, 0, true);
		//GameMgr.ins.mgrParts.SetPart(BONE.LHand, ref PLAYER, 0, true);
		GameMgr.ins.mgrParts.SetPart(BONE.RUpperArm, ref PLAYER, 0, true);
		GameMgr.ins.mgrParts.SetPart(BONE.RForeArm, ref PLAYER, 0, true);
		//GameMgr.ins.mgrParts.SetPart(BONE.RHand, ref PLAYER, 0, true);

		GameMgr.ins.mgrParts.SetPart(BONE.RThigh, ref PLAYER, 1, true);
		GameMgr.ins.mgrParts.SetPart(BONE.RCalf, ref PLAYER, 1, true);
		//SetPart(BONE.RFoot, ref PLAYER);
		GameMgr.ins.mgrParts.SetPart(BONE.LThigh, ref PLAYER, 1, true);
		GameMgr.ins.mgrParts.SetPart(BONE.LCalf, ref PLAYER, 1, true);
		//SetPart(BONE.LFoot);
	}
#if UNITY_EDITOR
	public void Update()
	{
		if (IS_SET_CAMERA && UnityEditor.SceneView.lastActiveSceneView != null)
		{
			CRT_TOOL.ins.cam.transform.position = UnityEditor.SceneView.lastActiveSceneView.camera.transform.position;
			CRT_TOOL.ins.cam.transform.rotation = UnityEditor.SceneView.lastActiveSceneView.camera.transform.rotation;
		}
	}
#endif
	private void ClickPlay()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(0);
	}

}
