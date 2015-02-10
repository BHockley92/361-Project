using UnityEngine;
using System.Collections;

namespace GameEnums
{
	public enum PlayerStatus { Offline, Online };
	public enum LandType { Sea, Grass, Tree, Meadow };
	public enum UnitType { Peasant, Infantry, Soldier, Knight };
	public enum VillageType { Hovel, Town, Fort };
	public enum ActionType { ReadyForOrders, Moved, BuildingRoad, 
		ChoppingTree, ClearingTombStone, UpgradingCombining, 
		StartCultivating, FinishCultivating };

	public enum StructureType { NONE, Tombstone, Road, Tower };
}
