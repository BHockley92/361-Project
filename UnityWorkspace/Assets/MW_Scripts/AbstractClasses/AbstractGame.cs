using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class AbstractGame
{
	public List<AbstractPlayer> participants { get; private set; }
	public AbstractPlayer turnOf { get; private set; }

	public AbstractGameLogic myGameLogic { get; private set; }
	public Board gameBoard { get; private set; }
	
	// TODO: board representation -- use this: http://www.redblobgames.com/grids/hexagons/#map-storage
}
