using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public abstract class AbstractGameLogic 
{
	public ValueManager myValueManager { get; protected set; }

	public abstract void hireVillager(AbstractUnit u, AbstractVillage commandingVillage, AbstractTile spawnedTile);
	public abstract void buildRoad (AbstractUnit u);
	public abstract bool buildTower(Tile t);

	public abstract bool upgradeVillage (AbstractVillage v, VillageType newType);
	public abstract bool upgradeUnit (AbstractUnit u, UnitType newType);
	public abstract bool combineUnits (AbstractUnit upgrader, AbstractUnit sacrificed);

	public abstract bool moveUnit(AbstractUnit u, AbstractTile dest);
	public abstract void destroyVillage(AbstractVillage v, AbstractUnit invader);
	public abstract void divideRegion(List<AbstractTile> region);
	public abstract void takeoverTile(AbstractTile dest);

	protected abstract void connectRegions (List<AbstractVillage> v);

	public abstract void beginTurn (AbstractPlayer p, AbstractGame g);
	protected abstract void treeGrowthPhase( Board b );
	protected abstract void tombStonePhase (AbstractVillage myVillage);
	protected abstract void peasantBuild (AbstractTile myTile);
	protected abstract void buildPhase (AbstractVillage myVillage);
	protected abstract void incomePhase (AbstractVillage myVillage);
	protected abstract void paymentPhase (AbstractVillage myVillage);

	protected abstract void perishVillagers (AbstractVillage myVillage );
}
