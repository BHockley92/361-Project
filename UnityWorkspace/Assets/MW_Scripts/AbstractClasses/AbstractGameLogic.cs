using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public abstract class GameLogic 
{
	public GameLogic();

	public abstract void buildRoad(AbstractUnit u);
	public abstract void upgradeVillage(AbstractVillage v, VillageType newType);
	public abstract void upgradeUnit(AbstractUnit u, UnitType newType);
	public abstract void moveUnit(AbstractUnit u, AbstractTile dest);
	private abstract void destroyVillage(Village v, Unit invader);
	private abstract void divideRegion(IList<AbstractTile> region);
	public abstract void takeoverTile(AbstractTile dest);
}
