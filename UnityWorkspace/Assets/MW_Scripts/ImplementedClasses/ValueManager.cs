using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public abstract class ValueManager
{
	private Dictionary <UnitType, int> 		unitValues; // cost to buy a unit
	private Dictionary <UnitType, int>		unitMaintenance; // cost every turn to maintain the unit
	private Dictionary <VillageType, int> 	villageValues;
	private Dictionary <LandType, int>		landValues;

	private string filePath { get; set; }
	private int value; // TODO: remove?

	public ValueManager()
	{
		unitValues = new Dictionary<UnitType, int>();
		unitMaintenance = new Dictionary<UnitType, int>();
		villageValues = new Dictionary<VillageType, int>();
		landValues = new Dictionary<LandType, int>();
	}

	public int getUnitValue(UnitType ut) { return unitValues [ut]; }
	public int getMaintenanceCost(UnitType ut) {return unitMaintenance [ut]; }
	public int getLandValue(LandType lt) { return landValues [lt]; }
	public int getVillageValue(VillageType vt) { return villageValues [vt]; }

	public abstract void loadValuesFromFile(); // Maybe have a generic serializer?

}
