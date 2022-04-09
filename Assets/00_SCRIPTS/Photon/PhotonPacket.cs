using System.Collections;
using System.Collections.Generic;

public class MSG
{
	//public int idx;
	public string name;
}

public class MSG_POS : MSG
{
	public int t;
	public int x;
	public int y;
	public int z;
}

public class MSG_STAT : MSG
{
	public int[] equip;
	//public Dictionary<int, EQUIP_DATA> dicEquip;
}