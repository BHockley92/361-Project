using UnityEngine;
using System.Collections;

public class Unit : AbstractUnit 
{
	public Unit(AbstractVillage vOwner, AbstractTile startLocation)
	{
		// TODO: throw exception is try to make a unit?
		myVillage = vOwner;
		myLocation = startLocation;
	}
}
