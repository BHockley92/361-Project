using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public abstract class AbstractPlayer
{
	public string username { get; protected set; }
	private PlayerStatus status { get; set; }
	private int wins;
	private int losses;
	public List<AbstractVillage> myVillages { get; set; }
}
