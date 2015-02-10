using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public abstract class ValueManager
{
	Dictionary <UnitType, int> 		unitValues;
	Dictionary <VillageType, int> 	villageValues;
	Dictionary <LandType, int>		landValues;

	private string filePath { get; set; }
	private int value; // TODO: remove?

	public ValueManager()
	{
		unitValues = new Dictionary<UnitType, int>();
		villageValues = new Dictionary<VillageType, int>();
		landValues = new Dictionary<LandType, int>();
	}

	public abstract int getUnitValue(UnitType ut) { return unitValues [ut]; }
	public abstract int getLandValue(LandType lt) { return landValues [lt]; }
	public abstract int getVillageValue(VillageType vt) { return villageValues [vt]; }

	private abstract void loadValuesFromFile(); // Maybe have a generic serializer?

}
