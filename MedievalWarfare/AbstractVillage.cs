enum VillageType
{
	Hovel,
	Town,
	Fort
}

abstract class AbstractVillage
{
	VillageType myType { get; };
	int gold { get; };
	int wood { get; };
}