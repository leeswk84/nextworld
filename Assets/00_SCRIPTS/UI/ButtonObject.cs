using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	public GameObject objMain;
	public GameObject obj;
	public Transform tran;

	public UnityEngine.UI.Button btn;
	public UnityEngine.UI.Text txt;
	public UnityEngine.UI.Image spr;

	public System.Action<bool> fncPress;

	public bool isAni = true;
	
	private static Vector3 vecScale = new Vector3(0.9f, 0.9f, 1f);
	
	void Start () 
	{
		//btn.onClick.AddListener(Click);
		//vec3.x = 
	}
	
	public void OnPointerDown(PointerEventData evt)
	{
		if (isAni) LeanTween.scale(obj, vecScale, 0.3f).setEase(LeanTweenType.easeOutExpo);
			
		if (fncPress != null) fncPress(true);
	}

	public void OnPointerUp(PointerEventData evt)
	{
		if (isAni) LeanTween.scale(obj, Vector3.one, 0.3f).setEase(LeanTweenType.easeOutExpo);
		if (fncPress != null) fncPress(false);
	}
	
}
