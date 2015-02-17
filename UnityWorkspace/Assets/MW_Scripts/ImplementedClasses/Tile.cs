using UnityEngine;
using System.Collections;
using GameEnums;

public class Tile : AbstractTile 
{
	// TODO: I'm not sure if keeping an occupying village as a thing is necessary
	// Does moveunit/takeover tile go trough the village directly? If so then we
	// can avoid duplicating state here.

	public Tile( LandType type)
	{
		myType = type;
		myVillage = null;
		occupyingStructure = new Structure(this, StructureType.NONE);
		occupyingUnit = null;
	}
}
