using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class AbstractGame
{
	public List<AbstractPlayer> participants { get; private set; }
	public AbstractPlayer turnOf { get; private set; }
	public int turnIndex { get; private set; }

	public AbstractGameLogic myGameLogic { get; private set; }
	public Board gameBoard { get; set; }

	protected void initialize(List<AbstractPlayer> p, AbstractGameLogic gl)
	{
		turnIndex = 0;
		participants = p;
		turnOf = participants [turnIndex];
		myGameLogic = gl;
	}

	/**
	 * Sets the turn to the next person in line
	 */
	protected void nextTurn()
	{
		turnOf = participants [++turnIndex % participants.Count];
	}
}
