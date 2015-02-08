using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public class ValueManager
{
	private string filePath { get; set; }
	private int value; // TODO: remove?

	// Fields that actually contain the values
	private Dictionary <UnitType, int> 		unitValues;
	private Dictionary <VillageType, int> 	villageValues;
	private Dictionary <LandType, int>		landValues;

	public ValueManager(string path)
	{
		filePath = path;
		unitValues = new Dictionary<UnitType, int>();
		villageValues = new Dictionary<VillageType, int>();
		landValues = new Dictionary<LandType, int>();

		loadValuesFromFile ();
	}

	public int getUnitValue(UnitType ut) { return unitValues [ut]; }
	public int getLandValue(LandType lt) { return landValues [lt]; }
	public int getVillageValue(VillageType vt) { return villageValues [vt]; }

	private void loadValuesFromFile()
	{
		// TODO:
			// check that file exists
			// read file, making sure no errors are made
			// construct the hashtables
			// perhaps have a serializer class and allow it to be used for all serializable objects?
	}
}
