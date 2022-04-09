using UnityEngine;
using System.Collections;

/// <summary>
/// 캐릭터 움직임 
/// </summary>
public class MoveCameraControl : MonoBehaviour
{
	/// <summary> 가속도 최대 </summary>
	public const float ADD_SPEED_MAX = 0.2f;
	/// <summary> 컨트롤 최대 </summary>
	private const float CONTROL_MAX = 50f;

	/// <summary> 회전 속도 </summary>
	private float ROT_SPEED = 20f;
	/// <summary> 이동최대 </summary>
	private float MOVE_MAX = 3f;
	
	public PlayerRay playerRay;
	public BlockView blockView;

	private const string STR_CHECK_NET = "Net";
	private const string STR_SAVE_POINT = "savepoint";
	private const string STR_ITEMBOX = "item";
	
	[Header("")]
	private Mini player;
	public GameObject objTouchPos;
	public Transform tranTouchPos;
	public Transform tranJoystic;
	public Transform tranBackground;
	
	public Transform[] transTest;

	private bool _isPress = false;

	private Vector3 vec3 = Vector3.zero;
	private Vector2 vec2;
	//private Vector3 vec32 = Vector3.zero;
	//private Quaternion qut = Quaternion.identity;
	private LTDescr ltween;

	private float tmpf = 0f;
	private float tmpf2 = 0f;
	private int tmpi;

	private int beforeTouchCnt = 0;
	private int moveIdx = 0;
	private int rotIdx = 1;

	private Vector2 vecStartMove;
	//private Vector2 vecStartRot;
	private Vector2 vecMove, vecRot;
	private Vector3 vecMoveDir;
	private Vector3 vecRotDir;

	private Collider colPlane;
	
	private Ray ray;
	private RaycastHit rayHit;

	private Ray rayGround;
	private RaycastHit hitGround;

	private Ray rayBuild;
	private RaycastHit rayHitBuild;

	private Vector3 pos_press_player;

	private float backupY;
	private bool updateCheck = false;
	private bool isHitBuild;
	private Collider[] ColBuild;

	private Quaternion qutFrom, qutTo;

	private float gravity = 0f;

	private int drag_max = 50;
	private int drag_min = 7;

	private float drag_distance;

	public int layer_build;
	public int layer_ground;
	public int layer_eventhit;
	public int layer_groundY;

	private float press_time;

	public Build CurrentSavePoint;

	public void Init()
	{
		layer_build = 1 << LayerMask.NameToLayer(TxtMgr.layer_build) | 1 << LayerMask.NameToLayer(TxtMgr.layer_npc);
		layer_ground = 1 << LayerMask.NameToLayer(TxtMgr.layer_ground);

		layer_groundY = (1 << LayerMask.NameToLayer(TxtMgr.layer_ground)) 
						+ (1 << LayerMask.NameToLayer(TxtMgr.layer_build))
						+ (1 << LayerMask.NameToLayer("BuildObject"));

		layer_eventhit = 1 << LayerMask.NameToLayer("EventHit");

		player = GameMgr.ins.PLAYER_MINI;

		rayHit = new RaycastHit();
		ray = new Ray();
		ray.direction = Vector3.down;

		hitGround = new RaycastHit();
		rayGround = new Ray();
		rayGround.direction = Vector3.down;

		rayHitBuild = new RaycastHit();
		rayBuild = new Ray();
		rayBuild.direction = Vector3.down;

		beforeTouchCnt = 0;
		moveIdx = 0;
		rotIdx = 1;

		vecStartMove.x = -1000;
		vecStartMove.y = -1000;

		vecMove.x = -1000;
		vecMove.y = -1000;

		vecRot.x = -1000;
		vecRot.y = -1000;

		playerRay.Init();
		playerRay.SetOutLine(false);

		blockView.Init();

		objTouchPos.SetActive(false);
	}

	public void StartMove()
	{
		playerRay.InitStartMove();
		blockView.StartCheck();
		GameMgr.ins.mgrNpc.Rebirth();
		
		UpdateMove(false);
	}

	public void EndMove()
	{
		playerRay.SetOutLine(false);
		blockView.EndCheck();
	}

	public void OnPress(GameObject obj, bool isPress)
	{
		//Debug.Log(isPress + " , " + Input.touches.Length);
		/*
		if (isPress == false && Input.touches != null && Input.touches.Length > 0)
		{	return;	}
		*/
		/*
		if (_isPress == false && isPress == true && Input.touches != null && Input.touches.Length > 0)
		{   //2번째 터치 시작. 회전
			//vecStartRot = CameraControl.POS_MOUSE;// UICamera.lastTouchPosition;
		}
		*/

		_isPress = isPress;
		
		//Debug.Log("MoveCameraControl.OnPress :: " + isPress);
		if (isPress)
		{   //터치 시작...
			//startVec = Input.touches[0].position;
			// 1번째 터치 시작 움직임.
			//vecStartMove = UICamera.lastTouchPosition;
#if UNITY_EDITOR
#else
			if (Input.touches != null && Input.touches.Length == 1)
#endif
			{
				vecStartMove = Input.mousePosition;
				objTouchPos.SetActive(true);
				vec3 = vecStartMove;
				vec3.x -= GameMgr.ins.mgrCam.cam.pixelWidth * 0.5f;
				vec3.y -= GameMgr.ins.mgrCam.cam.pixelHeight * 0.5f;

				vec3.x /= GameMgr.ins.mgrUI.canvas.scaleFactor;
				vec3.y /= GameMgr.ins.mgrUI.canvas.scaleFactor;

				tranJoystic.localPosition =
				tranTouchPos.localPosition = vec3;
			}
			//Debug.Log("vecStartMove :: "+Input.mousePosition);
			
			pos_press_player = player.trans.position;
			press_time = Time.time;
			GameMgr.ins.mgrUI.status.HideUIRight();
		}
		else
		{
			//if(Input.touches != null && Input.touches.Length > 0 && beforeTouchCnt > 1)
			if ( beforeTouchCnt > 1)
			{	//2번째 터치
				GameMgr.ins.PLAYER.SetTouchAtack();
				_isPress = true;
				return;
			}

			objTouchPos.SetActive(false);

			moveIdx = 0;
			rotIdx = 1;
			vecStartMove.x = -1000;
			vecStartMove.y = -1000;
			vecMove.x = -1000;
			vecMove.y = -1000;
			vecRot.x = -1000;
			vecRot.y = -1000;

			//Debug.Log(Time.time + " : " + press_time);

			if (Time.time - press_time < 0.5f && pos_press_player == player.trans.position)// && Vector3.Distance(vecStartMove, Input.mousePosition) < 1f)
			{
				switch (GameMgr.ins.talk.index_talk)
				{
					case -1: //일반 공격 하는 부분
						GameMgr.ins.PLAYER.SetTouchAtack();
						break;
					default: //건물 인터렉션, 대화
						GameMgr.ins.talk.ClickTalk();
						break;
				}
				//Debug.Log("ACTION");
				//if (GameMgr.ins.talk.index_talk != -1) GameMgr.ins.talk.ClickTalk();
			}
			GameMgr.ins.mgrUI.status.ShowUIRight();
		}
	}

	public void OnPressGround(GameObject obj, bool isPress)
	{
		//Debug.Log("MoveCameraControl.OnPressGround :: " + isPress);
		OnPress(obj, isPress);
	}

	public void MoveAndUpdate()
	{
		UpdateTouch();
		updateCheck = UpdateMove();

		if (updateCheck == false) CheckMoveColObject();
		
		//Debug.Log(Vector3.Distance(player.backPosision, player.trans.position));
		//Debug.Log(Vector3.Angle(player.backForward, player.trans.forward));
		//Debug.Log( Quaternion.Angle(qutFrom, player.trans.rotation));
		/*
		if (Vector3.Distance(player.backPosision, player.trans.position) == 0)
			player.ani.ani.CrossFade( MiniAni.IDLE, 0.3f);
		else
		{
			player.ani.ani[MiniAni.RUN].speed = 2f;
			player.ani.ani.CrossFade(MiniAni.RUN, 0.3f);
		}
		*/

		//player.ani.UpdateAni(Vector3.Distance(player.backPosision, player.trans.position) != 0); //실제 케릭터 이동 여부로 모션
		player.ani.UpdateAni(Vector3.Distance(vecStartMove , vecMove) > 0f); //드래그를 하였다면 이동 모션


		if (player.ani.c_state == MiniAni.STATE.RUN && player.dust_delay < Time.time)//Random.Range(0, 101) < 10)
		{
			player.dust_delay = Time.time + 0.1f;
			GameMgr.ins.mgrEffect.ShowEffect(Effect.EffDust001, ref GameMgr.ins.mgrEffect.tran, false).tran.position = player.trans.position; //ref player.body, false);
		}

		playerRay.UpdateOutLine();

		if (GameMgr.ins.mgrCam.IsCheckMaxPos(ref player.trans))
		{
			//GameMgr.ins.mgrCam.rotXCam.position = player.trans.position;
			//Debug.Log("Change Tile");
			GameMgr.ins.mgrBullet.OnChangeTile();
		}
		
		if (GameMgr.ins.talk.btnTalkNext.obj.activeSelf 
			&& GameMgr.ins.talk.npcTalk != null
			&& GameMgr.ins.talk.npcTalk != GameMgr.ins.PLAYER)
		{	//대화중에는 대상을 보도록
			LookBody(GameMgr.ins.talk.npcTalk.mini.trans.position);
			
			/*
			vec3 = GameMgr.ins.talk.npcTalk.mini.trans.position;
			vec3.y = player.body.position.y;
			qutFrom = player.body.rotation;
			player.body.LookAt(vec3);
			qutTo = player.body.rotation;
			player.body.rotation = Quaternion.Slerp(qutFrom, qutTo, Time.smoothDeltaTime * ROT_SPEED);
			*/
		}
		else if(GameMgr.ins.talk.btnTalk.objMain.activeSelf
				&& GameMgr.ins.talk.interBuild != null)
		{
			LookBody(GameMgr.ins.talk.interBuild.tran.position);
		}
		
		UpdateY();
		UpdateCam();
		GameMgr.ins.mgrUI.skybox.UpdateSkybox(); 
	}

	private void LookBody(Vector3 vec)
	{
		vec3 = vec;
		vec3.y = player.body.position.y;
		qutFrom = player.body.rotation;
		player.body.LookAt(vec3);
		qutTo = player.body.rotation;
		player.body.rotation = Quaternion.Slerp(qutFrom, qutTo, Time.smoothDeltaTime * ROT_SPEED);
	}

	private void UpdateTouch()
	{
		if (GameMgr.ins.PLAYER.is_intro)
		{
			if (_isPress == true)
			{
				OnPress(null, false);
			}
			return;
		}
		//Debug.Log("MoveCameraControl.MoveAndUpdate");
		//playMini.trans

		//Debug.Log(Input.touches);

		if (_isPress)
		{   //드래그 캐릭터 이동.
			//Debug.Log("cccc");
			//Debug.Log(Input.touches[0].position);
			//Debug.Log(UICamera.lastTouchPosition);

			vecMove = CameraControl.POS_MOUSE;
			/*
			if (Input.touches != null && Input.touches.Length > 1)
			{	//이동과 공격 각도 
				vecMove = Input.touches[moveIdx].position;
				//vecMove.y += 100f;
				//				transTest[0].localPosition = vecMove;

				//두개 터치 해도 하나로만
				vecRot = Input.touches[rotIdx].position;
				
				//vecRot.y += 100f;
				//				transTest[1].localPosition = vecRot;
			}
			else
			{	//이동만
				vecMove = CameraControl.POS_MOUSE; //Input.mousePosition; //UICamera.lastTouchPosition;
				//Debug.Log("vecMove::"+vecMove);

				//vecMove.y += 100f;
				//				transTest[0].localPosition = vecMove;
			}
			*/

			//vec3 = Input.mousePosition;
			vec3 = CameraControl.POS_MOUSE;

			vec3.x -= GameMgr.ins.mgrCam.cam.pixelWidth * 0.5f;
			vec3.y -= GameMgr.ins.mgrCam.cam.pixelHeight * 0.5f;
			vec3.x /= GameMgr.ins.mgrUI.canvas.scaleFactor;
			vec3.y /= GameMgr.ins.mgrUI.canvas.scaleFactor;
			tranJoystic.localPosition = vec3;
			drag_distance = Vector3.Distance(tranTouchPos.localPosition, tranJoystic.localPosition);

			if (drag_distance < drag_min)
			{
				vec3 =
				tranJoystic.localPosition = tranTouchPos.localPosition;
			}

			if (drag_distance > drag_max)
			{
				vec2.x = vec3.x - tranTouchPos.localPosition.x;
				vec2.y = vec3.y - tranTouchPos.localPosition.y;

				vec2.x *= (drag_max / drag_distance);
				vec2.y *= (drag_max / drag_distance);

				vec3.x = tranTouchPos.localPosition.x + vec2.x;
				vec3.y = tranTouchPos.localPosition.y + vec2.y;
			}
			tranJoystic.localPosition = vec3;
		}

		/*
		if (Input.touchCount == 1 && beforeTouchCnt != 1)
		{
			if (Vector2.Distance(vecMove, Input.GetTouch(0).position) > Vector2.Distance(vecRot, Input.GetTouch(0).position))
			{
				rotIdx = 0;
				moveIdx = 1;

				vecMove.x = -1000;
				vecMove.y = -1000;
			}
			else
			{
				moveIdx = 0;
				rotIdx = 1;

				vecRot.x = -1000;
				vecRot.y = -1000;
			}
		}
		*/

		beforeTouchCnt = Input.touchCount;
	}
	private void LookHead(Vector3 vec)
	{
		qutFrom = player.head.rotation;
		vec3 = vec;
		vec3.y = player.head.position.y;
		player.head.LookAt(vec3);
		qutTo = player.head.rotation;
		player.head.rotation = Quaternion.Slerp(qutFrom, qutTo, Time.smoothDeltaTime * ROT_SPEED);
	}
	private bool UpdateMove(bool isCheck = true)
	{
		if (player.moveForce != Vector3.zero)
		{   //밀리는 힘 줄어드는 계산
			player.backPosition = player.trans.position;
			player.trans.position += player.moveForce;
			player.moveForce -= (player.moveForce * 10f * Time.smoothDeltaTime);
			if (Mathf.Abs(player.moveForce.x) < 0.001f) player.moveForce.x = 0f;
			if (Mathf.Abs(player.moveForce.z) < 0.001f) player.moveForce.z = 0f;
		}

		if (GameMgr.ins.PLAYER.lookNPC != null)
		{
			LookHead(GameMgr.ins.PLAYER.lookNPC.mini.trans.position);
			/*
			qutFrom = player.head.rotation;
			vec3 = GameMgr.ins.PLAYER.lookNPC.mini.trans.position;
			vec3.y = player.head.position.y;
			player.head.LookAt(vec3);
			qutTo = player.head.rotation;
			player.head.rotation = Quaternion.Slerp(qutFrom, qutTo, Time.smoothDeltaTime * ROT_SPEED);
			*/
		}
		else if(GameMgr.ins.talk.btnTalk.objMain.activeSelf
				&& GameMgr.ins.talk.interBuild != null)
		{
			LookHead(GameMgr.ins.talk.interBuild.tran.position);
		}


		if (isCheck)
		{
			if (updateCheck) return true;

			player.backPosition = player.trans.position;
			player.backForward = player.trans.forward;
			qutFrom = player.head.rotation;

			updateCheck = true;
			
			if (vecMove.x == -1000) return false;
			if (drag_distance < drag_min) return false;
			if (player.ani.c_state == MiniAni.STATE.DMG) return false;

			//Vector3 backPos = player.trans.position;
			//player.backPosision = player.trans.position;
		}

		backupY = player.trans.position.y;

		vec3 = Vector3.zero;
		tmpf = (vecStartMove.x - vecMove.x);
		SetMaxTmpF(ref tmpf);
		vec3.x -= tmpf;

		tmpf2 = (vecStartMove.y - vecMove.y);
		SetMaxTmpF(ref tmpf2);
		vec3.z -= tmpf2;

		if (vec3.z == 0 && vec3.x == 0) return false;
		
		vec3 =  Vector3.MoveTowards(player.trans.position, vec3, CONTROL_MAX); //캐릭터 이동 범위의 최대값 설정
		vec3 = player.trans.position + vec3;
		vec3.y = player.trans.position.y;

		//몸 이동 방향으로 회전
		vecRotDir = vec3;
		qutFrom = player.body.rotation;
		player.body.LookAt(vecRotDir);
		qutTo = player.body.rotation;
		player.body.rotation = Quaternion.Slerp(qutFrom, qutTo, Time.smoothDeltaTime * ROT_SPEED);
		
		player.trans.Translate(vec3 * Time.smoothDeltaTime * ADD_SPEED_MAX);

		player.trans.position = Vector3.MoveTowards(player.backPosition, player.trans.position, MOVE_MAX * Time.smoothDeltaTime);
		
		return false;
	}

	private void CheckMoveColObject()
	{
		/*
		if (Input.touchCount == 2)
		{   //2개 터치 중일때는 하나를 원거리 공격 방향으로 상체 회전

		}
		else
		{
			*/
			if (GameMgr.ins.PLAYER.lookNPC == null && vecRotDir != Vector3.zero)
			{
				qutFrom = player.head.rotation;
				player.head.LookAt(vecRotDir);
				qutTo = player.head.rotation;
				player.head.rotation = Quaternion.Slerp(qutFrom, qutTo, Time.smoothDeltaTime * ROT_SPEED * 0.3f);
			}
		//}

		vecMoveDir = player.trans.position;
		//일반 속도 이동
		CheckRay();
		CheckColBuild();
		//Debug.Log(rayHit.point.y + " , " + tmpf3);
		if (isHitBuild == false) return;// false;
		/*
		//좌측으로 이동.
		player.trans.position = player.backPosision;
		player.trans.LookAt(vecMoveDir);
		player.trans.Translate(Vector3.left * Vector3.Distance(player.backPosision, vecMoveDir) * 0.5f);
		player.trans.rotation = Quaternion.identity;
		CheckRay();
		CheckColBuild();
		if (isHitBuild == false) return false;
		//우측으로이동
		player.trans.position = player.backPosision;
		player.trans.LookAt(vecMoveDir);
		player.trans.Translate(Vector3.right * Vector3.Distance(player.backPosision, vecMoveDir) * 0.5f);
		player.trans.rotation = Quaternion.identity;
		CheckRay();
		CheckColBuild();
		if (isHitBuild == false) return false;
		*/

		//x 조금 이동할 수 있는 경우
		vec3 = vecMoveDir;
		vec3.z = player.backPosition.z;
		if (vecMoveDir.x < player.backPosition.x)
			vec3.x -= (vecMoveDir.x - player.backPosition.x) * 0.1f;
		else vec3.x += (vecMoveDir.x - player.backPosition.x) * 0.1f;

		player.trans.position = vec3;
		CheckRay();
		CheckColBuild();
		
		if (isHitBuild == false) return;// false;
		//z 조금 이동할 수 있는 경우
		vec3 = vecMoveDir;
		vec3.x = player.backPosition.x;
		if (vecMoveDir.z < player.backPosition.z)
			vec3.z -= (vecMoveDir.z - player.backPosition.z) * 0.1f;
		else vec3.z += (vecMoveDir.z - player.backPosition.z) * 0.1f;

		player.trans.position = vec3;
		CheckRay();
		CheckColBuild();
		if (isHitBuild == false) return;// false;

		//이동 전으로 돌아감
		player.trans.position = player.backPosition;
		rayHit.point = player.trans.position;
	}

	private void UpdateY() 
	{
		vec3 = player.trans.position;
		vec3.y = rayHit.point.y;

		//Debug.Log(Mathf.Abs(rayHit.point.y - backupY));

		if (vec3.y < GameMgr.ins.mgrField.GetWaterY() - 1f)
		{	//일정 높이 이하로 내려가지 못하도록
			//Debug.Log("Min Y : " + player.backPosition + "," + (GameMgr.ins.mgrField.GetWaterY() - 1f) + "," + vec3.y);
			//player.backPosision.y = GameMgr.ins.mgrField.GetWaterY() - 1f;
			//vec3.y = GameMgr.ins.mgrField.GetWaterY() - 1f;
			player.backPosition.y = vec3.y;
			player.trans.position = player.backPosition;
			//rayHit.point = player.trans.position;
			//backupY = player.trans.position.y;
			return;
		}
		
		/*
		if (rayHit.point.y > 0f && rayHit.point.y > backupY && rayHit.point.y - backupY > 0.05f)
		{	//일정 높이 간격 이상 올라가지 못하도록
			player.trans.position = player.backPosition;
			return;
		}
		*/

		//아래로 떨어지기
		if (backupY - rayHit.point.y > 0.1f)
		{
			gravity += 1f;
			vec3.y = player.trans.position.y - (gravity * Time.smoothDeltaTime);
		}
		else 
		{
			gravity = 3f;
		}

		/*
		if (vec3.y < GameMgr.ins.mgrField.GetWaterY() - 0.3f)
		{	//물 위에서 헤엄..
			vec3.y = GameMgr.ins.mgrField.GetWaterY() - 0.3f;
		}
		*/
		
		player.trans.position = vec3;

		vec3.y = 100;
		rayGround.origin = vec3;
		Physics.Raycast(rayGround, out hitGround, 200f, layer_ground);
		
		if (hitGround.point.y > player.trans.position.y)
		{
			vec3 = player.trans.position;
			vec3.y = hitGround.point.y;
			player.trans.position = vec3;
		}
		
	}

	private void CheckColBuild() 
	{
		vec3 = player.trans.position;
		vec3.y += 0.25f;

		isHitBuild = Physics.CheckSphere(vec3, 0.25f, layer_build);
		
		//Debug.Log(vec3.y);

		/*
		ColBuild = Physics.OverlapSphere(vec3, 0.25f, layer_build);
		isHitBuild = false;
		for (tmpi = 0; tmpi < ColBuild.Length; tmpi++)
		{
			if (ColBuild[tmpi].tag == BUILD)
			{
				isHitBuild = true;
				//Debug.Log(ColBuild[tmpi].gameObject.GetComponent<Build>().GetIdx());
				break;
			}
		}
		*/
		//if (rayHit.point.y - backupY > 0.2f) isHitBuild = true;
	}

	public void CheckRay() 
	{
		vec3 = player.trans.position;
		vec3.y += 0.5f;
		ray.origin = vec3;
		//Physics.Raycast(ray, out rayHit, 8 << 9);
		Physics.Raycast(ray, out rayHit, 5f, layer_groundY);

		if (player.obj.activeSelf == false) return;

		vec3 = player.trans.position;
		vec3.y += 1f;
		rayBuild.origin = vec3;
		Physics.Raycast(rayBuild, out rayHitBuild, 5f, layer_eventhit);

		if (rayHitBuild.collider != null)
		{
			GameMgr.ins.talk.ShowTalkBtn(-1, null, rayHitBuild.collider.GetComponent<EventCollider>());
			/*
			//Debug.Log(rayHitBuild.collider.name);
			switch (rayHitBuild.collider.name)
			{
				case STR_CHECK_NET:
					GameMgr.ins.talk.ShowTalkBtn(-2, null);
					break;
				case STR_ITEMBOX:
					GameMgr.ins.talk.ShowTalkBtn(-5, null, rayHitBuild.collider.GetComponent<EventCollider>());
					break;
				case STR_SAVE_POINT:
					GameMgr.ins.talk.ShowTalkBtn(-4, null, rayHitBuild.collider.GetComponent<EventCollider>());
					break;
			}
			*/
		}

		//Debug.Log(rayHit.collider.name);
	}

	private void SetMaxTmpF(ref float value)
	{
		if (value > CONTROL_MAX) value = CONTROL_MAX;
		if (value < -CONTROL_MAX) value = -CONTROL_MAX;
	}

	/// <summary> 카메라가 캐릭터 위치 따라 가기.. </summary>
	private void UpdateCam()
	{
		/*
		//살짝의 딜레이
		vec3 = GameMgr.ins.mgrCam.rotXCam.position;
		vec3.x -= (vec3.x - player.trans.position.x) * 0.2f;
		vec3.y = player.trans.position.y + 2;
		vec3.z -= (vec3.z - player.trans.position.z) * 0.2f;
		GameMgr.ins.mgrCam.rotXCam.position = vec3;
		*/

		//바로 카메라 이동
		vec3 = player.trans.position;
		//vec3.y += 2;
		GameMgr.ins.mgrCam.rotXCam.position = vec3;
		

		//뒷쪽 배경 위치 이동..
		/*
		vec3.x = player.trans.position.x - (GameMgr.WIDTH * 0.5f);
		vec3.y = tranBackground.position.y;
		vec3.z = player.trans.position.z - (GameMgr.WIDTH * 0.5f);
		tranBackground.position = vec3;
		*/
	}

	/// <summary>  이동 화면으로 카메라 각도 변경 </summary>
	public void IntroMoveCam(float rotX, float rotY, float moveZ = -0.1245f)
	{
		player.tranModelLeg.localRotation = Quaternion.identity;
		player.tranModelLeg.Rotate(Vector3.up, rotX);
		player.tranModelHead.localRotation = Quaternion.identity;
		player.tranModelHead.Rotate(Vector3.up, rotX);
		player.trans.localRotation = Quaternion.identity;
		player.trans.Rotate(Vector3.up, rotX);

		RotateY(rotY);
		RotateX(rotX);
		
		if (moveZ == -0.1245f) return;

		MoveZ(moveZ);
	}

	public void RotateY(float value) 
	{
		LeanTween.rotateX(GameMgr.ins.mgrCam.objYCam, value, 0.3f).setEase(LeanTweenType.easeOutExpo);
	}
	public void RotateX(float value) 
	{
		LeanTween.rotateY(GameMgr.ins.mgrCam.objXCam, value, 0.3f).setEase(LeanTweenType.easeOutExpo);
	}
	public void Move(float valuex, float valuez, float valuey = 2f) 
	{
		vec3.x = valuex;
		vec3.y = valuey;
		vec3.z = valuez;

		LeanTween.moveLocal(GameMgr.ins.mgrCam.objXCam, vec3, 0.3f).setEase(LeanTweenType.easeOutExpo);
	}
	public void MoveZ(float value) 
	{
		LeanTween.moveLocalZ(GameMgr.ins.mgrCam.objCam, value, 0.3f).setEase(LeanTweenType.easeOutExpo);
	}
}