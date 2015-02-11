using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public abstract class AbstractVillage 
{
	public VillageType myType { get; private set; }
	public int gold { get; private set; }
	public int wood { get; private set; }
	public IList<AbstractTile> controlledRegion { get; private set; }
	public IList<AbstractUnit> supportedUnits { get; private set; }
	public AbstractTile location { get; private set; }
	public AbstractPlayer myPlayer { get; private set; }
	
	public AbstractVillage(IList<AbstractTile> region, AbstractPlayer player)
	{
		myType = VillageType.Hovel;
		gold = 0;
		wood = 0;
		
		controlledRegion = region;
		supportedUnits = new List<AbstractUnit> ();
		myPlayer = player;
	}
	
	public abstract void swapControl(AbstractTile myTile, AbstractVillage other);
}
