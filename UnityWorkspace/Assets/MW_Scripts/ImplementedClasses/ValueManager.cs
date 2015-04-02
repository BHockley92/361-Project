using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public class ValueManager
{
	private Dictionary <UnitType, int> 		unitValues; // cost to buy a unit in gold
	private Dictionary <UnitType, int>		unitMaintenance; // cost every turn to maintain the unit
	private Dictionary <VillageType, int> 	villageValues; //cost in wood to upgrade
	private Dictionary <LandType, int>		landValues; //gold generated depending on landType

	private string filePath { get; set; }
	private int value; // TODO: remove?

	public ValueManager()
	{
		//I'm going to hard code this instead of loading from a file, at least for now
		unitValues = new Dictionary<UnitType, int>();
		unitValues.Add(GameEnums.UnitType.Peasant, 10);
		unitValues.Add(GameEnums.UnitType.Infantry, 20);
		unitValues.Add(GameEnums.UnitType.Soldier, 30);
		unitValues.Add(GameEnums.UnitType.Knight, 40);

		unitMaintenance = new Dictionary<UnitType, int>();
		unitMaintenance.Add (GameEnums.UnitType.Peasant, 2);
		unitMaintenance.Add (GameEnums.UnitType.Infantry, 6);
		unitMaintenance.Add (GameEnums.UnitType.Soldier, 18);
		unitMaintenance.Add (GameEnums.UnitType.Knight, 54); //might lower, why so $$

		villageValues = new Dictionary<VillageType, int>();
		villageValues.Add (GameEnums.VillageType.Hovel, 0);
		villageValues.Add (GameEnums.VillageType.Town, 8);
		villageValues.Add (GameEnums.VillageType.Fort, 8); //8 wood and a built town
		villageValues.Add (GameEnums.VillageType.Castle, 12); //12 wood and a built fort AND upkeep of 80 which nick coded in gameLogic

		landValues = new Dictionary<LandType, int>();
		landValues.Add (GameEnums.LandType.Grass, 1);
		landValues.Add (GameEnums.LandType.Tree, 0);
		landValues.Add (GameEnums.LandType.Meadow, 2);
		landValues.Add (GameEnums.LandType.Sea, 0); //just incase
	}

	public int getUnitValue(UnitType ut) { return unitValues [ut]; }
	public int getMaintenanceCost(UnitType ut) {return unitMaintenance [ut]; }
	public int getLandValue(LandType lt) { return landValues [lt]; }
	public int getVillageValue(VillageType vt) { return villageValues [vt]; }

	//public abstract void loadValuesFromFile(); // Maybe have a generic serializer?

}
