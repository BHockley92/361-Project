using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public abstract class AbstractUnit 
{
	public UnitType myType { get; protected set; }
	public ActionType currentAction { get; set; }
	
	public AbstractVillage myVillage { get; protected set; }
	public AbstractTile myLocation { get; protected set; }

	public AbstractPlayer getPlayer()
	{
		return myVillage.myPlayer;
	}
}
