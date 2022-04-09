using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UISelect : MonoBehaviour 
{
	public enum TYPE 
	{
		DEFAULT,
		BUILD,
		BUILD_EDIT,
		BUILD_MOVE,
		BLOCK_MOVE,
	}

	private TYPE m_type = TYPE.DEFAULT;
	
	public GameObject obj;
	public Transform tran;
	
	public ButtonObject btnUp, btnDown, btnMakeRoad, btnOK, btnCacel, btnMoveBlock;
	public Canvas canvasFront;
	public GameObject pnlFocus;

	public GameObject objInput;
	public InputField inputRot, inputY, inputScale, inputNpc;

	public Text txtEtcValue;
		
	private Vector3 vec3;

	private int selectBuildListIdx;

	public TYPE getType(){	return m_type;	}

	private int tradeIdx = -1;
	
	[HideInInspector]
	public int movetile = -1;
	[HideInInspector]
	public int movetileedit = -1;

	private Vector3 vec3Load;

	public void Init () 
	{
		btnOK.obj.SetActive(false);
		btnCacel.obj.SetActive(false);
		btnMoveBlock.obj.SetActive(false);

		btnOK.btn.onClick.AddListener(ClickOK);
		btnCacel.btn.onClick.AddListener(OnClickCancel);

		btnUp.btn.onClick.AddListener(ClickUp);
		btnDown.btn.onClick.AddListener(ClickDown);
		btnMakeRoad.btn.onClick.AddListener(ClickMakeRoad);

		btnMoveBlock.btn.onClick.AddListener(OnClickMoveBlock);

		inputRot.onValueChanged.AddListener(OnEditInputRot);
		inputY.onValueChanged.AddListener(OnEditInputY);
		inputScale.onValueChanged.AddListener(OnEditInputScale);
		inputNpc.onValueChanged.AddListener(OnEditInputNpc);
	}

	public void Hide() 
	{
		//GameMgr.ins.mgrGround.pnlFocus.SetActive(false);
		GameMgr.ins.mgrField.pnlFocus.SetActive(false);
		obj.SetActive(false);
	}

	public void UpdatePosition(ref Vector3 value, bool isCheckType = true) 
	{
		if (obj.activeInHierarchy == false) return;
		if (isCheckType && m_type == TYPE.BLOCK_MOVE)
		{
			obj.SetActive(false);
			return;
		}
		value.x += -0.5f;
		value.z += -0.5f;
		//vec3 = GameMgr.ins.mgrGround.cam.WorldToScreenPoint(value);
		vec3 = GameMgr.ins.mgrCam.cam.WorldToScreenPoint(value);

		vec3.x -= (GameMgr.ins.mgrCam.cam.pixelWidth * 0.5f);
		vec3.y -= (GameMgr.ins.mgrCam.cam.pixelHeight * 0.5f);

		vec3.x /= canvasFront.scaleFactor;
		vec3.y /= canvasFront.scaleFactor;

		tran.localPosition = vec3;

		vec3 = btnUp.tran.localPosition;
		vec3.x = 150 * ((tran.localPosition.x < 0) ? 1 : -1);
		btnUp.tran.localPosition = vec3;
		vec3 = btnDown.tran.localPosition;
		vec3.x = btnUp.tran.localPosition.x;
		btnDown.tran.localPosition = vec3;
	}

	/// <summary>
	/// 화면에 보여짐
	/// </summary>
	/// <param name="valueType"> 형태 </param>
	/// <param name="value"></param>
	public void Show(TYPE valueType, int value = -1)
	{
		obj.SetActive(true);

		btnOK.obj.SetActive(false);
		btnCacel.obj.SetActive(false);
		btnUp.obj.SetActive(false);
		btnDown.obj.SetActive(false);
		btnMakeRoad.obj.SetActive(false);
		btnMoveBlock.obj.SetActive(false);
		objInput.SetActive(false);

		switch (valueType)
		{
			case TYPE.DEFAULT:
				btnUp.obj.SetActive(true);
				btnDown.obj.SetActive(true);
				btnMakeRoad.obj.SetActive(true);
				break;
			case TYPE.BUILD:
				if (m_type == TYPE.BUILD_MOVE) valueType = TYPE.BUILD_MOVE;
				tradeIdx = -1;
				btnCacel.obj.SetActive(true);
				btnOK.obj.SetActive(true);
				
				btnOK.txt.text = TxtMgr.ins.GetTxt(TxtMgr.TYPE.OK);
				btnCacel.txt.text = TxtMgr.ins.GetTxt(TxtMgr.TYPE.CANCEL);
				break;
			case TYPE.BUILD_EDIT:
				selectBuildListIdx = value;
				btnCacel.obj.SetActive(true);
				btnOK.obj.SetActive(true);
				btnUp.obj.SetActive(true);
				btnDown.obj.SetActive(true);

				btnOK.txt.text = TxtMgr.ins.GetTxt(TxtMgr.TYPE.MOVE);
				btnCacel.txt.text = TxtMgr.ins.GetTxt(TxtMgr.TYPE.DELETE);
				GameMgr.ins.mgrField.EditorSet(GameMgr.TILE_CENTER, true);

				objInput.SetActive(true);
				inputRot.text = GameMgr.ins.mgrBuild.GetBuildDataListIndex(value).rot.ToString();
				inputY.text = GameMgr.ins.mgrBuild.GetBuildDataListIndex(value).move_y.ToString();
				inputScale.text = GameMgr.ins.mgrBuild.GetBuildDataListIndex(value).scale.ToString();
				inputNpc.text = GameMgr.ins.mgrBuild.GetBuildDataListIndex(value).index_npc.ToString();

				if(GameMgr.ins.mgrBuild.GetBuild(value).npc != null )
				{
					txtEtcValue.text = "npc";
					inputNpc.gameObject.SetActive(true);
				}
				else if(GameMgr.ins.mgrBuild.GetBuild(value).buildType == Build.BTYPE.ItemBox)
				{
					txtEtcValue.text = "idx";
					inputNpc.gameObject.SetActive(true);
				}
				else
				{
					inputNpc.gameObject.SetActive(false);
				}

				//btnMakeRoad.obj.SetActive(true);
				//btnMakeRoad.txt.text = TxtMgr.ins.GetTxt(TxtMgr.TYPE.ROTATION);		
				break;
			case TYPE.BLOCK_MOVE:
				btnMoveBlock.obj.SetActive(true);
				break;
		}
		m_type = valueType;
	}
	
	public void SetTradeBuild(int value) 
	{
		GameMgr.ins.mgrBuild.SelectBuildComplete(GameMgr.ins.mgrBuild.GetBuild(value).pos);
		tradeIdx = value;
		btnOK.txt.text = TxtMgr.ins.GetTxt(TxtMgr.TYPE.TRADE);
	}
	
	private void ClickUp() 
	{
		GameMgr.ins.mgrGround.GroundUp();
		GameMgr.ins.mgrBuild.SetUpdateBuild(GameMgr.ins.mgrGround.getCurIdx());
	}

	private void ClickDown() 
	{
		GameMgr.ins.mgrGround.GroundDown();
		GameMgr.ins.mgrBuild.SetUpdateBuild(GameMgr.ins.mgrGround.getCurIdx());
	}

	private void ClickMakeRoad() 
	{
		switch (m_type)
		{ 
			//case TYPE.BUILD_EDIT:
				//GameMgr.ins.mgrBuild.BuildRotation(selectBuildListIdx);
				//break;
			default:
				GameMgr.ins.mgrGround.MakeRoad();
				break;
		}
	}



	public void ClickOK() 
	{
		switch (m_type)
		{ 
			case TYPE.BUILD:	 //생성 완료
			case TYPE.BUILD_MOVE://이동 완료
				GameMgr.ins.mgrBuild.SetFocusBuildMaterial(false);
				if (m_type == TYPE.BUILD_MOVE && tradeIdx != -1)
				{	//교체 상황
					GameMgr.ins.mgrBuild.MoveBuild(GameMgr.ins.mgrBuild.GetFocusBuild().listIdx,
														GameMgr.ins.mgrBuild.GetBuild(tradeIdx).pos);
					GameMgr.ins.mgrBuild.MoveBuild(tradeIdx, GameMgr.ins.mgrBuild.GetBackMove());

				}
				else 
				{	//이동, 생성 완료
					GameMgr.ins.mgrBuild.CreateComplete();
				}
				GameMgr.ins.mgrUI.build.ClickBuildBack();
				GameMgr.ins.mgrGround.setCurIdx(-1);
				GameMgr.ins.mgrBuild.EditBuildComplete();
				tradeIdx = -1;
				GameMgr.ins.mgrGround.SetGroundColor(true);
				break;
			case TYPE.BUILD_EDIT: //이동 시작
				m_type = TYPE.BUILD_MOVE;
				GameMgr.ins.mgrBuild.StartMoveBuild(selectBuildListIdx, GameMgr.ins.mgrGround.getCurIdx());

				GameMgr.ins.mgrBuild.SetFocusBuildMaterial(true);
				
				break;
			//case TYPE.BUILD_MOVE:
			//	break;
		}
		
		Hide();
	}

	public void ClickCancel() {	SetCancel();	}
	public void OnClickCancel() { SetCancel(true); }
	public void SetCancel(bool isBtn = false) 
	{
		switch (m_type)
		{
			case TYPE.BUILD:	//생성취소
				GameMgr.ins.mgrBuild.SetFocusBuildMaterial(false);
				GameMgr.ins.mgrBuild.CreateCancel();
				GameMgr.ins.mgrUI.build.ClickBuildBack();
				break;
			case TYPE.BUILD_EDIT: //삭제
				if (isBtn) GameMgr.ins.mgrBuild.DeleteBuild(selectBuildListIdx);
				//Debug.Log("BUILD_EDIT_DELTE");
				break;
			case TYPE.BUILD_MOVE: //이동 취소
				GameMgr.ins.mgrBuild.SetFocusBuildMaterial(false);
				GameMgr.ins.mgrBuild.MoveCancelBuild();
				GameMgr.ins.mgrUI.build.ClickBuildBack();
				break;
		}
		if(isBtn) GameMgr.ins.mgrGround.SetGroundColor(true);
		Hide();
		m_type = TYPE.DEFAULT;
	}

	public void OnClickMoveBlock() 
	{
		GameMgr.ins.mgrSave.OnSave(); //타일 이동하기전 해당 타일 내용 저장.

		//Debug.Log("Move Block. BLOCK::" + movetile + ", EDIT::" + movetileedit);
		vec3Load = GameMgr.ins.mgrCam.rotXCam.localPosition;
		vec3Load.x -= GameMgr.ins.mgrBlock.posBlocks[movetile].mgrGround.posTile.x * GameMgr.WIDTH + GameMgr.ins.mgrBlock.posBlocks[movetile].mgrGround.posTile.x;
		vec3Load.z -= GameMgr.ins.mgrBlock.posBlocks[movetile].mgrGround.posTile.y * GameMgr.WIDTH + GameMgr.ins.mgrBlock.posBlocks[movetile].mgrGround.posTile.y;
		
		GameMgr.ins.mgrSave.SetBlockIdx(SaveManager.aryIdxs[movetile]);
	}

	public void SetBlockMove()
	{
		GameMgr.ins.mgrCam.rotXCam.localPosition = vec3Load;

		vec3.y = vec3Load.y;
		vec3.x = GameMgr.ins.mgrBlock.posBlocks[GameMgr.TILE_CENTER].mgrGround.vertices[movetileedit].x;
		vec3.z = GameMgr.ins.mgrBlock.posBlocks[GameMgr.TILE_CENTER].mgrGround.vertices[movetileedit].z;

		LeanTween.moveLocal(GameMgr.ins.mgrCam.rotXCam.gameObject, vec3, 0.3f).setEase(LeanTweenType.easeInQuad);
	}
	private float f;
	private int index_npc;
	public void OnEditInputRot(string value)
	{
		if (float.TryParse(inputRot.text, out f) == false) return;
		GameMgr.ins.mgrBuild.BuildRotation(selectBuildListIdx, f);
	}

	public void OnEditInputY(string value)
	{
		if (float.TryParse(inputY.text, out f) == false) return;
		GameMgr.ins.mgrBuild.BuildMoveY(selectBuildListIdx, f);
	}
	public void OnEditInputScale(string value)
	{
		if (float.TryParse(inputScale.text, out f) == false) return;
		GameMgr.ins.mgrBuild.BuildScale(selectBuildListIdx, f);
	}
	public void OnEditInputNpc(string value)
	{
		if (int.TryParse(inputNpc.text, out index_npc) == false) return;
		GameMgr.ins.mgrBuild.BuildNpcIndex( selectBuildListIdx, index_npc);


	}
}
