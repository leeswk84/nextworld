using UnityEngine;
using System.Collections;

public class BaseBlockManager : MonoBehaviour 
{
	[HideInInspector]
	public int pos_idx;
	[HideInInspector]
	public int block_idx;
	[HideInInspector]
	public BlockManager mgr;

	protected GroundManager getGround()
	{
		return mgr.mgrGround;
	}
	protected RoadManager getRoad()
	{
		return mgr.mgrRoad;
	}
	protected BuildManager getBuild()
	{
		return mgr.mgrBuild;
	}
}
