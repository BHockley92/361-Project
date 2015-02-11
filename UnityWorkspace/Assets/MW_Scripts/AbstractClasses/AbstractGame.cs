using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class AbstractGame
{
	List<AbstractPlayer> participants;
	public AbstractPlayer turnOf { get; private set; }

	public AbstractGameLogic myGameLogic { get; private set; }
	public AbstractTile[,] board { get; private set; }
	// TODO: board representation -- use this: http://www.redblobgames.com/grids/hexagons/#map-storage
}
