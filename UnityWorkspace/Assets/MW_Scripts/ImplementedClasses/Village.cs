using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public class Village : AbstractVillage 
{
	public Village(List<AbstractTile> region, AbstractPlayer player)
	{
		myType = VillageType.Hovel;
		gold = 7;
		wood = 0;
		
		controlledRegion = region;
		supportedUnits = new List<AbstractUnit> ();
		myPlayer = player;
	}

	public override void swapControl(AbstractTile myTile, AbstractVillage other)
	{
		myTile.myVillage.controlledRegion.Remove (myTile);
		myTile.myVillage = other;
		myTile.myType = LandType.Meadow; //specs say "the tile is converted to a meadow of the invading player’s color"
		other.controlledRegion.Add (myTile);
	}
}
