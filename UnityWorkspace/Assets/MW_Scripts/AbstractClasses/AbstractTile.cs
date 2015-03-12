using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public abstract class AbstractTile 
{
	public Vector2 boardPosition { get; set; }
	public LandType myType { get; set; }
	public AbstractStructure occupyingStructure { get; set; }
	public AbstractVillage myVillage{ get; set; }
	public AbstractUnit occupyingUnit { get; set; }
	public abstract List<AbstractTile> getNeighbours();
}
