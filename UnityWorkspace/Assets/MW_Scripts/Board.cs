using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public class Board 
{
	// TODO: board representation -- use this: http://www.redblobgames.com/grids/hexagons/#map-storage with solution 3 (slide to the left)
	// Axial coordinate system will be used with a rhombus shaped board
		// q = x coordinate
		// r = z coordinate
	public Tile[,] board;

	// Constructor for when a random board is generated
	public Board(int qlength, int rwidth)
	{
		board = generateRandomBoard (qlength, rwidth);
	}

	// Constructor for when a board is provided
	public Board(AbstractTile[,] b)
	{
		board = b;
	}
	
	// Generates a random island that conforms to specs (300 land tiles that aren't water, one big land mass)
	private void generateRandomBoard(int qboardLength, int rboardWidth)
	{
		// TODO
		board = new AbstractTile[ rboardWidth, qboardLength ];
	}
	
	// get the neighbours of a hex tile
	// list is ordered from the upper left neighbor and moving clockwise
	public List<Tile> getNeighbors(Coordinate c)
	{
		// TODO: check that indices are valid -- don't forget that not all indices in the array are used!
		List<Tile> ret = new List<Tile> ();
		ret.Add (board [c.q, c.r - 1]);
		ret.Add (board [c.q + 1, c.r - 1 ]);
		ret.Add (board [c.q + 1, c.r ]);
		ret.Add (board [c.q, c.r + 1]);
		ret.Add (board [c.q - 1, c.r + 1 ]);
		ret.Add (board [c.q - 1, c.r]);

		return ret;
	}
}
