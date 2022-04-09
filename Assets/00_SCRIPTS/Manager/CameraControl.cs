using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveParam
{
	public Transform tran;
	public SaveManager.DIR dir;
}

/// <summary>
/// 카메라 조작
/// </summary>
public class CameraControl : MonoBehaviour 
{
	public ButtonObject btnReset, btnPosRot, btnCamType;
	public ButtonObject btnBackTouch;
	
	public Camera cam;
	public GameObject objCam;
	public GameObject objXCam;
	public GameObject objYCam;
	public Transform tranCam;
	public Transform rotXCam;
	public Transform rotYCam;
	public Transform posCam;

	private bool isControlRot = true;

	private System.Action FncHideEditor;
	private System.Action FncUpdateEditor;

	private int camType = 1;
	private bool isZoom;
	private bool isDragRot;
	private bool isDragPos;
	
	private float startZoom;
	private float startZoomCam;
	private Vector2 startDrag;

	private Quaternion qut;
	private float fnum;
	private Vector3 vec3;

	private float maxPosX;
	private float movePosX;
	private float maxPosZ;
	private float movePosZ;

	private bool isCheck;

	private List<MoveParam> listMove;

	[HideInInspector]
	public Vector3 basePos = new Vector3(0f, 2f, 0f);

	public static Vector2 POS_MOUSE;

	private float maxCamPos;
	private bool isMoveTop;
	private bool isMoveBottom;
	private bool isMoveLeft;
	private bool isMoveRight;

	void Start () 
	{
		isMoveTop = false;
		isMoveBottom = false;
		isMoveLeft = false;
		isMoveRight = false;

		isZoom = false;
		isDragRot = false;
		isDragPos = false;

		maxCamPos = GameMgr.WIDTH * 2f;

		tranCam = cam.transform;
		objCam = cam.gameObject;
		objXCam = rotXCam.gameObject;
		objYCam = rotYCam.gameObject;

		listMove = new List<MoveParam>();

		btnReset.btn.onClick.AddListener(PressReset);
		btnPosRot.btn.onClick.AddListener(ClickBtnRotPos);
		btnCamType.btn.onClick.AddListener(ClickBtnCamType);
		
		btnBackTouch.fncPress = OnGroundPress;

		SetMgrGround();
	}

	public void SetMgrGround()
	{
		FncHideEditor = GameMgr.ins.mgrGround.SetHideEdit;
		FncUpdateEditor = GameMgr.ins.mgrGround.UpdateEditor;
		btnBackTouch.btn.onClick.RemoveAllListeners();
		btnBackTouch.btn.onClick.AddListener(GameMgr.ins.mgrGround.SetHideEdit);
	}

	public void Init() 
	{
		//returnDegree = Mathf.FloorToInt(GameMgr.WIDTH / 2);
		//maxPos = (GameMgr.WIDTH * 0.5f) + 1f;
		//movePos = GameMgr.WIDTH + 1;

		//maxPosX = (GameMgr.WIDTH * 1.6f) + 1;
		//movePosX = (GameMgr.WIDTH * 2.1f) + 1;
		maxPosX = (GameMgr.WIDTH) + 1;
		movePosX = (GameMgr.WIDTH) + 1;
		maxPosZ = (GameMgr.WIDTH) + 1;
		movePosZ = (GameMgr.WIDTH) + 1;

		//movePos = (GameMgr.WIDTH / 3) * 2;

		ClickBtnRotPos();
		camType = 2;
		ClickBtnCamType();		
	}

	public bool IsCheckMaxPos(ref Transform dirTran) 
	{
		if (GameMgr.ins.mgrUI.PLAY_MODE != UIManager.MODE.MOVE) return false;
		
		isCheck = false;
		//vec3 = dirTran.position;

		MoveParam param = new MoveParam();
		param.tran = dirTran;

		if (dirTran.position.x < -maxPosX && isMoveLeft == false)
		{
			isMoveLeft = true;
			param.dir = SaveManager.DIR.LEFT;// 2;
			listMove.Add(param);
			isCheck = true;
			GameMgr.ins.mgrSave.MoveIndex(param.dir, false);
		}
		if (dirTran.position.x > maxPosX && isMoveRight == false)
		{
			isMoveRight = true;
			param.dir = SaveManager.DIR.RIGHT;// 2;
			listMove.Add(param);
			isCheck = true;
			GameMgr.ins.mgrSave.MoveIndex(param.dir, false);
		}
		if (dirTran.position.z < -maxPosZ && isMoveBottom == false)
		{
			isMoveBottom = true;
			param.dir = SaveManager.DIR.BOTTOM;
			listMove.Add(param);
			isCheck = true;
			GameMgr.ins.mgrSave.MoveIndex(param.dir, false);
		}
		if (dirTran.position.z > maxPosZ && isMoveTop == false)
		{
			isMoveTop = true;
			param.dir = SaveManager.DIR.TOP;
			listMove.Add(param);
			isCheck = true;
			GameMgr.ins.mgrSave.MoveIndex(param.dir, false);
		}
		
		/*
		if (dirTran.position.x < -maxPos) { vec3.x += movePos; isCheck = true; GameMgr.ins.mgrSave.MoveIndex(SaveManager.DIR.LEFT, false); }
		if (dirTran.position.x > maxPos) { vec3.x -= movePos; isCheck = true; GameMgr.ins.mgrSave.MoveIndex(SaveManager.DIR.RIGHT, false); }
		if (dirTran.position.z < -maxPos) { vec3.z += movePos; isCheck = true; GameMgr.ins.mgrSave.MoveIndex(SaveManager.DIR.BOTTOM, false); }
		if (dirTran.position.z > maxPos) { vec3.z -= movePos; isCheck = true; GameMgr.ins.mgrSave.MoveIndex(SaveManager.DIR.TOP, false); }

		if(isCheck) dirTran.position = vec3;
		*/

		return isCheck;
	}

	public void MoveIndex()
	{
		if(listMove.Count < 1) return;
		vec3 = Vector3.zero;//listMove[0].tran.position;
		switch(listMove[0].dir)
		{
			case SaveManager.DIR.LEFT2:
			case SaveManager.DIR.LEFT: vec3.x += movePosX; isMoveLeft = false; break;
			case SaveManager.DIR.RIGHT2:
			case SaveManager.DIR.RIGHT: vec3.x -= movePosX; isMoveRight = false; break;
			case SaveManager.DIR.BOTTOM: vec3.z += movePosZ; isMoveBottom = false; break;
			case SaveManager.DIR.TOP: vec3.z -= movePosZ; isMoveTop = false; break;
		}

		for (int i = 0; i < GameMgr.ins.netPlayer.Length; i++)
		{
			if (GameMgr.ins.netPlayer[i].mini.obj.activeSelf == false) continue;
			GameMgr.ins.netPlayer[i].UpdateTilePos(vec3);
		}

		GameMgr.ins.mgrEffect.tran.position += vec3;
		GameMgr.ins.mgrBullet.tran.position += vec3;
		listMove[0].tran.position += vec3;
		GameMgr.ins.mgrCam.rotXCam.position = listMove[0].tran.position;// vec3;
		listMove.RemoveAt(0);
	}

	private void SetPositionMouse() 
	{
		if (Input.touches != null && Input.touchCount > 0)
			POS_MOUSE = Input.touches[0].position;
		else
			POS_MOUSE = Input.mousePosition;
	}

	public void UpdateCameraMove () 
	{
		SetPositionMouse();
        
		if (GameMgr.ins.mgrUI.PLAY_MODE == UIManager.MODE.MOVE)
		{	//캐릭터 조작 중 카메라 조작
			GameMgr.ins.mgrMoveCam.MoveAndUpdate();
			return;
		}
		
        if ((isZoom || isDragPos || isDragRot) &&  GameMgr.ins.mgrUI.select.getType() == UISelect.TYPE.BLOCK_MOVE)
        {
            if (GameMgr.ins.mgrUI.select.obj.activeSelf) GameMgr.ins.mgrUI.select.obj.SetActive(false);
        }

		//Debug.Log("lastTouchPosition :: "+posMouse);
		if (isZoom) 
		{
			//lblTest.text = "Touch Distance::" + Vector2.Distance(Input.touches[0].position, Input.touches[1].position).ToString();
			vec3 = tranCam.localPosition;
			vec3.z = startZoomCam - ((startZoom - Vector2.Distance(Input.touches[0].position, Input.touches[1].position)) * 0.05f);
			
			if (vec3.z > -5f) vec3.z = -5f;
			if (vec3.z < -50f) vec3.z = -50f;

			tranCam.localPosition = vec3;
			
			FncUpdateEditor();
		}
		else if (isDragRot) 
		{
			//Debug.Log("update::"+posMouse);
			//Debug.Log("x:" + (startDrag.x - posMouse.x));
			//Debug.Log("y:" + (startDrag.y - posMouse.y));

			fnum = (startDrag.x - POS_MOUSE.x) * -1f;
			if (Mathf.Abs(fnum) > 0f)
			{	//속도 최대 최소
				//if (fnum > 100) fnum = 100;
				//if (fnum < -100) fnum = -100;
				
				qut = rotXCam.localRotation;
				rotXCam.Rotate(Vector3.up, fnum);
#if UNITY_EDITOR
				rotXCam.localRotation = Quaternion.Slerp(qut, rotXCam.localRotation, Time.smoothDeltaTime * 35f);
#else
				rotXCam.localRotation = Quaternion.Slerp(qut, rotXCam.localRotation, Time.smoothDeltaTime * 15f);
#endif
				startDrag.x = POS_MOUSE.x;
			}

			fnum = (startDrag.y - POS_MOUSE.y) * -1f;
			
			if (Mathf.Abs(fnum) > 0f)
			{	//속도 최대 최소
				//if (fnum > 100) fnum = 100;
				//if (fnum < -100) fnum = -100;
				
				qut = rotYCam.localRotation;
				rotYCam.Rotate(Vector3.left, fnum);
				
#if UNITY_EDITOR
				rotYCam.localRotation = Quaternion.Slerp(qut, rotYCam.localRotation, Time.smoothDeltaTime * 30f);
#else
				rotYCam.localRotation = Quaternion.Slerp(qut, rotYCam.localRotation , Time.smoothDeltaTime * 10f);
#endif

                if (rotYCam.localRotation.x < 0.05f)
				{	//최소, 최대 값 설정..
					qut = rotYCam.localRotation;
					qut.x = 0.05f;
					rotYCam.localRotation = qut;
				}
				if (rotYCam.localRotation.x > 0.6f)
				{
					qut = rotYCam.localRotation;
					qut.x = 0.6f;
					rotYCam.localRotation = qut;
				}
				startDrag.y = POS_MOUSE.y;
			}
			FncUpdateEditor();
		}
		else if (isDragPos)
		{	//위치 이동...
			fnum = (startDrag.x - POS_MOUSE.x) * (tranCam.localPosition.z * -0.15f);
			if (Mathf.Abs(fnum) > 0f)
			{	//좌, 우
				vec3 = posCam.localPosition;
				vec3.x += fnum;
#if UNITY_EDITOR
				posCam.localPosition = Vector3.Slerp(posCam.localPosition, vec3, Time.smoothDeltaTime * 2f);
#else
				posCam.localPosition = Vector3.Slerp(posCam.localPosition, vec3, Time.smoothDeltaTime * 0.5f);
#endif
				vec3 = posCam.position;
				
				if (vec3.x < -maxCamPos) vec3.x = -maxCamPos;
				if (vec3.x > maxCamPos) vec3.x = maxCamPos;
				if (vec3.z < -maxCamPos) vec3.z = -maxCamPos;
				if (vec3.z > maxCamPos) vec3.z = maxCamPos;

				rotXCam.position = vec3;
				posCam.localPosition = Vector3.zero;
				startDrag.x = POS_MOUSE.x;

			}
			fnum = (startDrag.y - POS_MOUSE.y) * (tranCam.localPosition.z * -0.15f);
			if (Mathf.Abs(fnum) > 0f)
			{	//앞뒤..
				vec3 = posCam.localPosition;
				vec3.z += fnum;
#if UNITY_EDITOR
				posCam.localPosition = Vector3.Slerp(posCam.localPosition, vec3, Time.smoothDeltaTime * 2f);
#else
				posCam.localPosition = Vector3.Slerp(posCam.localPosition, vec3, Time.smoothDeltaTime * 0.5f);
#endif
				rotXCam.position = posCam.position;
				posCam.localPosition = Vector3.zero;
				startDrag.y = POS_MOUSE.y;
			}
			IsCheckMaxPos(ref rotXCam);

			/*
			vec3 = rotXCam.localPosition;
			if (rotXCam.localPosition.x < -maxPos) vec3.x += GameMgr.WIDTH;
			if (rotXCam.localPosition.x > maxPos) vec3.x -= GameMgr.WIDTH;
			if (rotXCam.localPosition.z < -maxPos) vec3.z += GameMgr.WIDTH;
			if (rotXCam.localPosition.z > maxPos) vec3.z -= GameMgr.WIDTH;
			rotXCam.localPosition = vec3;
			*/
			
			//Debug.Log(rotXCam.localPosition);
			//GameMgr.WIDTH / 2 -1
			
			FncUpdateEditor();
		}
#if UNITY_EDITOR
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
		{
			vec3 = tranCam.localPosition;
			vec3.z -= 1;
			if (vec3.z < -50f) vec3.z = -50f;

			tranCam.localPosition = vec3;
			
			FncUpdateEditor();
		}
		else if (Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			vec3 = tranCam.localPosition;
			vec3.z += 1;
			if (vec3.z > -5f) vec3.z = -5f;

			tranCam.localPosition = vec3;
			
			FncUpdateEditor();
		}
#endif
        //Vector3.Distance

    }

	private void PressReset() 
	{
		vec3 = Vector3.zero; vec3.y = 30;
		rotXCam.localEulerAngles = vec3;
		
		vec3 = Vector3.zero; vec3.x = 35;
		rotYCam.localEulerAngles = vec3;
		
		vec3 = Vector3.zero; vec3.z = -15;
		tranCam.localPosition = vec3;

		vec3 = Vector3.zero; vec3.y = 2;
		rotXCam.localPosition = vec3;

		FncUpdateEditor();
	}
	
	//public void OnPointUp() { OnPress(null, false, true); }
	//public void OnPointDown() {	OnPress(null, true, true); }

	public void OnGroundPress(bool value) { OnPress(null, value, true); }
	public void OnPointClick() { FncHideEditor(); }

	public void OnPress(GameObject obj, bool isPress, bool isGround = false) 
	{
		if (GameMgr.ins.mgrUI.PLAY_MODE == UIManager.MODE.MOVE)
		{
			GameMgr.ins.mgrMoveCam.OnPress(obj, isPress);
			return;
		}
		//Debug.Log( "BACK PRESS :: " + isPress );
		//UICamera.currentCamera.ScreenPointToRay(posMouse)
		//UICamera.lastHit.
		//Debug.Log("TouchCount :: "+UICamera.touchCount);
		//lblTest.text = "BACK CLICK TouchCount :: " + UICamera.touchCount;
		
		if (isPress)
		{
			if (Input.touchCount > 1)
			{	//줌인 아웃..
				isZoom = true;
				isDragRot = false;
				isDragPos = false;
				startZoom = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
				startZoomCam = tranCam.localPosition.z;
			}
			else
			{
				SetPositionMouse();

				if( (camType == 1 && isControlRot)
					|| (camType == 2 && isGround)
					|| (camType == 3 && POS_MOUSE.y > cam.pixelHeight * 0.5f)
					|| (camType == 4 && POS_MOUSE.x < cam.pixelWidth * 0.5f))
				{	//밖을 터치하면 회전
					isDragRot = true;
					isDragPos = false;
				}
				else 
				{	//땅을 터치하면 위치 드래그
					isDragRot = false;
					isDragPos = true;
				}

				startDrag = POS_MOUSE; //Input.touches[0].position;
				//Debug.Log(startDrag);
			}
		}

		if (!isPress)
		{
			isZoom = false;
			isDragRot = false;
			isDragPos = false;
			//if (obj.name == "backTouch") FncHideEditor();
		}

		/*
		lblTest.text = "";
		for (int i = 0; i < Input.touchCount; i++) 
		{
			if (i != 0) lblTest.text += "\n";
			lblTest.text += "BACK Press"+i+" :: " + Input.touches[i].position;
		}
		Debug.Log("Input.touchCount::"+Input.touchCount);
		*/


		//UICamera.GetTouch(0).pos
		//UICamera.currentTouch.
	}
	private void ClickBtnRotPos() 
	{
		isControlRot = !isControlRot;

		if (isControlRot) btnPosRot.txt.text = TxtMgr.ins.GetTxt(TxtMgr.TYPE.CAM_ROT);
		else btnPosRot.txt.text = TxtMgr.ins.GetTxt(TxtMgr.TYPE.CAM_POS);
	}
	private void ClickBtnCamType() 
	{
		camType++;
		if (camType > 4) camType = 1;
		btnPosRot.obj.SetActive(camType == 1);

		switch (camType)
		{
			case 1: btnCamType.txt.text = TxtMgr.ins.GetTxt(TxtMgr.TYPE.CAM_TYPE1); break;
			case 2: btnCamType.txt.text = TxtMgr.ins.GetTxt(TxtMgr.TYPE.CAM_TYPE2); break;
			case 3: btnCamType.txt.text = TxtMgr.ins.GetTxt(TxtMgr.TYPE.CAM_TYPE3); break;
			case 4: btnCamType.txt.text = TxtMgr.ins.GetTxt(TxtMgr.TYPE.CAM_TYPE4); break;
		}
	}
}
