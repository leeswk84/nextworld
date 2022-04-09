using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPrefab : MonoBehaviour
{
	/*
	public enum TYPE
	{
		PARTS,
		BUILD,
		EFFECT,
	}
	public TYPE curType = TYPE.PARTS;
	*/
	public GameObject obj;
	public Transform tran;
	
	//[HideInInspector]
	//public ObjectPrefab prefab;
	[HideInInspector]
	public GameObject prefab;
}
