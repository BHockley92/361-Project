enum UnitType
{
	Peasant, 
	Infantry, 
	Soldier, 
	Knight
}

enum ActionType
{
	ReadyForOrders,
	Moved,
	BuildingRoad,
	ChoppingTree,
	ClearingTombstone,
	UpgradingCombining,
	StartCultivating,
	FinishCultivating
}

abstract class AbstractUnit
{
	UnitType myType { get; };
	ActionType currentAction { get; };
}