using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TILE_MOVE_TEST : MonoBehaviour 
{
	public Button btnUp;
	public Button btnDown;
	public Button btnLeft;
	public Button btnRight;

	public Text[] txtTiles;
	private int[] aryIdxs;

	public int editIndex;
	public int PLANET_WIDTH;
	public int PLANET_HEIGHT;
	private int PLANET_MAX;

	private int for_i;

	void Start () 
	{
		PLANET_MAX = PLANET_WIDTH * PLANET_HEIGHT;

		aryIdxs = new int[txtTiles.Length];

		btnUp.onClick.AddListener(OnClickUp);
		btnDown.onClick.AddListener(OnClickDown);
		btnLeft.onClick.AddListener(OnClickLeft);
		btnRight.onClick.AddListener(OnClickRight);

		SetIndex();
	}

	private void OnClickUp() { editIndex=		aryIdxs[7]; SetIndex(); }
	private void OnClickDown() { editIndex =	aryIdxs[17]; SetIndex(); }
	private void OnClickLeft() { editIndex =	aryIdxs[11]; SetIndex(); }
	private void OnClickRight() { editIndex = aryIdxs[13]; SetIndex(); }
	
	/*
	width 5
	  0  1  2  3  4
	  5  6  7  8  9
	 10 11 12 13 14
	 15 16 17 18 19
	 20 21 22 23 24
	 */

	int check_num;
	int line_first;
	int line_center;

	public void SetIndex() 
	{
		//center
		SetLine(editIndex, 12);

		//up
		line_center = editIndex - PLANET_WIDTH;
		if (line_center < 0) line_center = PLANET_MAX + line_center;
		SetLine(line_center, 7);

		//up up
		line_center = editIndex - (PLANET_WIDTH * 2);
		if (line_center < 0) line_center = PLANET_MAX + line_center;
		SetLine(line_center, 2);

		//down
		line_center = editIndex + (PLANET_WIDTH);
		if (line_center > (PLANET_MAX - 1)) line_center = line_center - PLANET_MAX;
		SetLine(line_center, 17);

		//down down
		line_center = editIndex + (PLANET_WIDTH * 2);
		if (line_center > (PLANET_MAX - 1)) line_center = line_center - PLANET_MAX;
		SetLine(line_center, 22);

		for (for_i = 0; for_i < aryIdxs.Length; for_i++)
		{	txtTiles[for_i].text = aryIdxs[for_i].ToString(); }
	}
	
	private void SetLine(int valueIdx, int valueCur) 
	{
		aryIdxs[valueCur] = valueIdx;

		line_first = Mathf.FloorToInt(valueIdx / PLANET_WIDTH) * PLANET_WIDTH; //지금 위치 좌측 처음 수
		//left
		check_num = valueIdx - 1;
		if (check_num < line_first) check_num = (line_first + PLANET_WIDTH) - (line_first - check_num);
		aryIdxs[valueCur-1] = check_num;

		//left left
		check_num = valueIdx - 2;
		if (check_num < line_first) check_num = (line_first + PLANET_WIDTH) - (line_first - check_num);
		aryIdxs[valueCur-2] = check_num;

		//right
		check_num = valueIdx + 1;
		if (check_num > line_first + PLANET_WIDTH - 1) check_num = line_first + (check_num - (line_first + PLANET_WIDTH));
		aryIdxs[valueCur+1] = check_num;

		//right right
		check_num = valueIdx + 2;
		if (check_num > line_first + PLANET_WIDTH - 1) check_num = line_first + (check_num - (line_first + PLANET_WIDTH));
		aryIdxs[valueCur+2] = check_num;
	}

}
