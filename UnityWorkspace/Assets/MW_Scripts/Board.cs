using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public class Board 
{
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

	// Constructor for when a board is provided
	public Board( Tile[,] b )
	{
		board = b;
	}
	
	// Generates a random island that conforms to specs (300 land tiles that aren't water, one big land mass)
	// TODO: debug
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
				LandType lt;
				int randInt = Random.Range(0, 3);
				if( randInt == 0 ) lt = LandType.Meadow;
				else if( randInt == 1 ) lt = LandType.Grass;
				else lt = LandType.Tree;

				// Now make and place the tile
				board[ i , j ] = new Tile( lt, this );
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
				board[i, j] = new Tile(LandType.Sea, this);
			}
		}
	}

	// returns neighbours of a given tile
	// returns null if the tile is not found
	public List<Tile> getNeighbours(AbstractTile t)
	{
		for(int i = 0; i < board.GetLength(0); i++ )
		{
			for( int j = 0; j < board.GetLength(1); j++ )
			{
				if( board[i, j] == t)
				{
					return getNeighbours(new Coordinate(i, j));
				}
			}
		}

		// tile wasn't found
		return null;
	}
	
	// get the neighbours of a hex tile
	public List<Tile> getNeighbours(Coordinate c)
	{
		List<Tile> ret = new List<Tile> ();

		// Check that the coordinate is within bounds
		if(c.x >= 0 && c.z >= 0)
		{
			if( c.x < board.GetLength(0) && c.z < board.GetLength(0))
			{
				// Now we know the coordinate is valid

				// Check that left adjacent spaces are valid
				if( c.x - 1 >= 0 )
				{
					ret.Add(board[c.x - 1, c.z]);

					// Check for the above and below
					if( c.z - 1 >= 0 )
					{
						ret.Add(board[c.x - 1, c.z - 1]);
					}

					if( c.z + 1 < board.GetLength(1))
					{
						ret.Add(board[ c.x - 1, c.z + 1 ]);
					}
				}

				// Now check the right adjacent spaces
				if( c.x + 1 < board.GetLength(0))
				{
					ret.Add ( board[ c.x + 1, c.z ] );

					// Check for the above and below
					// Check for the above and below
					if( c.z - 1 >= 0 )
					{
						ret.Add(board[c.x + 1, c.z - 1]);
					}
					
					if( c.z + 1 < board.GetLength(1))
					{
						ret.Add(board[ c.x + 1, c.z + 1 ]);
					}
				}
			}
		}
		return ret;
	}
}
