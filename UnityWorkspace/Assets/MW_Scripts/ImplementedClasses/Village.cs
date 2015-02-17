using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public class Village : AbstractVillage 
{
	public Village(List<AbstractTile> region, AbstractPlayer player)
	{
		myType = VillageType.Hovel;
		gold = 0;
		wood = 0;
		
		controlledRegion = region;
		supportedUnits = new List<AbstractUnit> ();
		myPlayer = player;
	}

	public override void swapControl(AbstractTile myTile, AbstractVillage other)
	{
		myTile.myVillage.controlledRegion.Remove (myTile);
		myTile.myVillage = other;
		other.controlledRegion.Add (myTile);
	}
}
