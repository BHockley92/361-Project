enum LandType
{
	Sea,
	Grass,
	Tree,
	Meadow
}

abstract class AbstractTile
{
	LandType myType { get; };
}