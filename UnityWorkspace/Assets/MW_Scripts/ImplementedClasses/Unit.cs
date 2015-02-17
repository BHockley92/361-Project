using UnityEngine;
using System.Collections;

public class Unit : AbstractUnit 
{
	public Unit(AbstractVillage vOwner, AbstractTile startLocation)
	{
		myVillage = vOwner;
		myLocation = startLocation;

		if(startLocation.occupyingUnit == null)
		{
			startLocation.occupyingUnit = this;
		}
	}
}
