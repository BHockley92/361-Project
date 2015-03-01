using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class AbstractGame
{
	public List<AbstractPlayer> participants { get; private set; }
	public AbstractPlayer turnOf { get; private set; }
	private int turnIndex = 0;

	public AbstractGameLogic myGameLogic { get; private set; }
	public Board gameBoard { get; private set; }

	protected void initialize(List<AbstractPlayer> p, Board b, AbstractGameLogic gl)
	{
		participants = p;
		turnOf = participants [turnIndex];
		gameBoard = b;
		myGameLogic = gl;
	}

	/**
	 * Sets the turn to the next person in line
	 */
	protected void nextTurn()
	{
		turnIndex++;
		if(turnIndex >= participants.Count)
		{
			turnIndex = 0;
		}

		turnOf = participants [turnIndex];
	}
}
