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
	public Board(int length, int width)
	{
		board = generateRandomBoard (length, width);
	}

	// Constructor for when a board is provided
	public Board( Tile[,] b )
	{
		board = b;
	}
	
	// Generates a random island that conforms to specs (300 land tiles that aren't water, one big land mass)
	private Tile[,] generateRandomBoard(int boardLength, int boardWidth)
	{
		// TODO
		board = new Tile[ boardLength, boardWidth ];
		return null;
	}

	// returns neighbours of a given tile
	// returns null if the tile is not found
	public List<Tile> getNeighbours(AbstractTile t)
	{
		for(int i = 0; i < board.GetLength(0); i++ )// TODO: is this the right input for the method?
		{
			for( int j = 0; j < board.GetLength(1); j++ ) // TODO: same as above
			{
				if( board[i, j] == t)
				{
					return getNeighbours(new Coordinate(i, j));
				}
			}
		}

		// tile wasnt' found
		return null;
	}
	
	// get the neighbours of a hex tile
	public List<Tile> getNeighbours(Coordinate c)
	{
		List<Tile> ret = new List<Tile> ();

		// Check that the coordinate is within bounds
		if(c.x >= 0 && c.z >= 0)
		{
			if( c.x < board.GetLength(0) && c.z < board.GetLength(1)) // TODO: test that the right dimensions are verified x-> 0 and z -> 1
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

					if( c.z + 1 < board.GetLength(1)) // TODO is this the correct dimension?
					{
						ret.Add(board[ c.x - 1, c.z + 1 ]);
					}
				}

				// Now check the right adjacent spaces
				if( c.x + 1 < board.GetLength(0)) // TODO: is this the correct dimension?
				{
					ret.Add ( board[ c.x + 1, c.z ] );

					// Check for the above and below
					// Check for the above and below
					if( c.z - 1 >= 0 )
					{
						ret.Add(board[c.x + 1, c.z - 1]);
					}
					
					if( c.z + 1 < board.GetLength(1)) // TODO is this the correct dimension?
					{
						ret.Add(board[ c.x + 1, c.z + 1 ]);
					}
				}
			}
		}

		return ret;
	}
}
