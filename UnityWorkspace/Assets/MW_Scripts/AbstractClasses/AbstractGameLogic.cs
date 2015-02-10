using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public abstract class AbstractGameLogic 
{
	public abstract void buildRoad(AbstractUnit u);
	public abstract void upgradeVillage(AbstractVillage v, VillageType newType);
	public abstract void upgradeUnit(AbstractUnit u, UnitType newType);
	public abstract void moveUnit(AbstractUnit u, AbstractTile dest);
	public abstract void destroyVillage(AbstractVillage v, AbstractUnit invader);
	public abstract void divideRegion(IList<AbstractTile> region);
	public abstract void takeoverTile(AbstractTile dest);
}
