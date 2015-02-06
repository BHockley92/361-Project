using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public class ValueManager
{
	Dictionary <UnitType, int> 		unitValues;
	Dictionary <VillageType, int> 	villageValues;
	Dictionary <LandType, int>		landValues;

	private string filePath { get; set; }
	private int value;

	public ValueManager();

	public int getUnitValue(UnitType ut) { return unitValues [ut]; }
	public int getLandValue(LandType lt) { return landValues [lt]; }
	public int getVillageValue(VillageType vt) { return villageValues [vt]; }

	private void loadValuesFromFile();

}
