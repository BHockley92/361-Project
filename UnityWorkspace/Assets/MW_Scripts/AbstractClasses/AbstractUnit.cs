using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public abstract class AbstractUnit 
{
	public bool isCannon = false;
	public UnitType myType { get; set; }
	public ActionType currentAction { get; set; }
	
	public AbstractVillage myVillage { get; set; }
	public AbstractTile myLocation { get; set; }

	public AbstractPlayer getPlayer()
	{
		return myVillage.myPlayer;
	}
}
