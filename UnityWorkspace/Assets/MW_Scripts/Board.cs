using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public class Board {

	// board representation -- use this: http://www.redblobgames.com/grids/hexagons/#map-storage
	// Rhombus is being used as the shape of the board.
	public Tile[,] board { get; private set; }
	public int border { get; private set; }

	/**
	 * Constructs a board randomly.
	 * 
	 * Length and width are length and width of te board.
	 * Waterboarder is how many tiles of water should surround the island.
	 * Assumes valid inputs.
	 */
	public Board(int length, int width, int waterBorder)
	{
		border = waterBorder;
		generateRandomBoard (length, width);
	}

	// provide a ready made board
	public Board( Tile[,] b, int waterBorder)
	{
		board = b;
		border = waterBorder; //might need to specify waterBorder
	}

	// Generates a random island that conforms to specs (300 land tiles that aren't water, one big land mass)
	private void generateRandomBoard(int boardLength, int boardWidth)
	{
		board = new Tile[ boardLength, boardWidth ];
		fillBoardWithWater ();
		// Now make the island while taking into account for the border (don't put land there)
		for(int i = 0 + border; i < board.GetLength(0) - border; i++)
		{
			for(int j = 0 + border; j < board.GetLength(1) - border; j++)
			{
				// Generate a random land type
				LandType lt = LandType.Grass;
				int randInt = Random.Range(0, 10);
				switch (randInt) {
					case 0:
					case 1:
						lt = LandType.Tree;
						break;
					case 2:
						lt = LandType.Meadow;
						break;
					default:
						break;
				}
				// Now make and place the tile
				board[ i , j ] = new Tile( lt, this, i, j);
			}
		}
	}

	// Fills the board with water tiles
	private void fillBoardWithWater()
	{
		for(int i = 0; i < board.GetLength(0); i++)
		{
			for(int j = 0; j < board.GetLength(1); j++)
			{
				board[i, j] = new Tile(LandType.Sea, this, i ,j);
			}
		}
	}

	/*
	 * returns neighbours of a given tile
	 * returns null if the tile is not found
	 * No sea tiles are returned
	 */ 
	public List<Tile> getNeighbours(AbstractTile t)
	{
		for(int i = 0; i < board.GetLength(0); i++ )
		{
			for( int j = 0; j < board.GetLength(1); j++ )
			{
				if( board[i, j] == t)
				{
					return getNeighbours(i, j);
				}
			}
		}

		// tile wasn't found
		return null;
	}

	/**
	 * Returns a List of tiles adjacent to the unit.
	 * Returns an empty list if the coordinates are out of bounds.
	 * No sea tiles are returned
	 */
	public List<Tile> getNeighbours(int x, int z)
	{
		List<Tile> ret = new List<Tile> ();

		// Check that the coordinate is within bounds
		if(x >= 0 && z >= 0)
		{
			if( x < board.GetLength(0) && z < board.GetLength(0))
			{
				// Now we know the coordinate is valid

				// Check that left adjacent spaces are valid
				if( x - 1 >= 0 )
				{
					if( board[x - 1, z].myType != LandType.Sea)
						ret.Add(board[x - 1, z]);

					// Check for the above and below
					if( z - 1 >= 0 && board[x - 1, z - 1].myType != LandType.Sea)
					{
						ret.Add(board[x - 1, z - 1]);
					}

					if( z + 1 < board.GetLength(1) && board[x - 1, z + 1].myType != LandType.Sea)
					{
						ret.Add(board[ x - 1, z + 1 ]);
					}
				}

				// Now check the right adjacent spaces
				if( x + 1 < board.GetLength(0))
				{
					if( board[ x + 1, z ].myType != LandType.Sea )
						ret.Add ( board[ x + 1, z ] );

					// Check for the above and below
					// Check for the above and below
					if( z - 1 >= 0 && board[x + 1, z - 1].myType != LandType.Sea)
					{
						ret.Add(board[x + 1, z - 1]);
					}
					
					if( z + 1 < board.GetLength(1) && board[ x + 1, z + 1 ].myType != LandType.Sea)
					{
						ret.Add(board[ x + 1, z + 1 ]);
					}
				}
			}
		}

		return ret;
	}
}
