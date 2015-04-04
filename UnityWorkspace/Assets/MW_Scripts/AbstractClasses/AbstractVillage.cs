using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public abstract class AbstractVillage 
{
	public int damageTaken = 0;
	public VillageType myType { get; set; }
	public int gold { get; set; }
	public int wood { get; set; }
	public List<AbstractTile> controlledRegion { get; set; } //need it unprotected for serialization 
	public List<AbstractUnit> supportedUnits { get; set; }
	public AbstractTile location { get; set; }
	public AbstractPlayer myPlayer { get; set; }

	public abstract void swapControl(AbstractTile myTile, AbstractVillage other);
}
