using UnityEngine;
using System.Collections;
using GameEnums;

public class Structure : AbstractStructure 
{
	public Structure( AbstractTile loc, StructureType t)
	{
		myLocation = loc;
		myType = t;
	}
}
