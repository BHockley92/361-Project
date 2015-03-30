using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public class Tile : AbstractTile 
{
	// TODO: I'm not sure if keeping an occupying village as a thing is necessary
	// Does moveunit/takeover tile go trough the village directly? If so then we
	// can avoid duplicating state here.

	public bool visited { get; set; } // for BFS algorithm
	public bool isVisited{ get; set; } //for assignRegions BFS algorithm
	public bool hasVillage{ get; set; } //for assignRegions BFS algorithm
	public Board myBoard { get; private set; }
	public bool iteratedOver{ get; set; } //for bfs

	public Tile( LandType type, Board b, float i, float j)
	{
		myType = type;
		myVillage = null;
		occupyingStructure = new Structure(this, StructureType.NONE);
		occupyingUnit = null;
		visited = false;
		boardPosition = new Vector2(i,j);

		if (b == null) throw new System.ArgumentException ("Board passed to tile is null");

		myBoard = b;
	}

	public override List<AbstractTile> getNeighbours()
	{
		List<AbstractTile> ret = new List<AbstractTile>();
		foreach(Tile t in myBoard.getNeighbours(this))
		{
			ret.Add(t);
		}
		return ret;
	}
}
