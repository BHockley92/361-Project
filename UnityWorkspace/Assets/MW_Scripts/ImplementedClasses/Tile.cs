using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public class Tile : AbstractTile 
{
	// TODO: I'm not sure if keeping an occupying village as a thing is necessary
	// Does moveunit/takeover tile go trough the village directly? If so then we
	// can avoid duplicating state here.

	public bool visited { get; set; } // for BFS algorithm
	public Board myBoard { get; private set; }

	public Tile( LandType type)
	{
		myType = type;
		myVillage = null;
		occupyingStructure = new Structure(this, StructureType.NONE);
		occupyingUnit = null;

		visited = false;
	}
}
