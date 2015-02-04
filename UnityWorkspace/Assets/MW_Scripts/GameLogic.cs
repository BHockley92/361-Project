using UnityEngine;
using System.Collections;

public class GameLogic 
{
	public GameLogic();

	public void buildRoad(Unit u);
	public void upgradeVillage(Village v, VillageType newType);
	public void upgradeUnit(Unit u, UnitType newType);
	public void moveUnit(Unit u, Tile dest);
	private void destroyVillage(Village v, Unit invader);
	private void divideRegion(Sequence<Tile> region);
	public void takeoverTile(Tile dest);
}
