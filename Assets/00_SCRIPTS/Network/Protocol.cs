using System.Collections.Generic;

public class BaseProtocol
{
	public bool result;
	public int error;
}

public class Login : BaseProtocol
{
	public string id;
	public int no;
	public string nick;
	public int grade;
	public STATUS_DATA data;
}

public class Planet : BaseProtocol
{
	public int planet;
	public int count;
	public int width;
	public int height;
}

public class MapData
{
	public int idx;
	public int dt;
	public string dg;
	public string dr;
	public string db;
}

public class MapList : BaseProtocol
{
	public int planet;
	public int count;
	public int width;
	public int height;
	public Dictionary<string, MapData> tiles;	

}
