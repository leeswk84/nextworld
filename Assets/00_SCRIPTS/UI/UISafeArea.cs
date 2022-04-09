using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISafeArea : MonoBehaviour
{
	//RectTransform rectTransform;
	//Rect safeArea;
	//Vector2 minAnchor;
	//Vector2 maxAnchor;

	private void Awake()
	{
		// safeArea를 받아서 min앵커와 max앵커에 position을 부여
		// 단, 필셀로 반환되니 앵커에 집어 넣을땐 비율로 변환 필요
		RectTransform rectTransform = GetComponent<RectTransform>();
		Rect safeArea = Screen.safeArea;
		Vector2 minAnchor = safeArea.position;
		Vector2 maxAnchor = minAnchor + safeArea.size;

		//인스펙터 프로퍼티에 집어 넣을수 있게 비율로 변환 및 할당
		minAnchor.x /= Screen.width;
		minAnchor.y /= Screen.height;
		maxAnchor.x /= Screen.width;
		maxAnchor.y /= Screen.height;

		rectTransform.anchorMin = minAnchor;
		rectTransform.anchorMax = maxAnchor;
	}
}
