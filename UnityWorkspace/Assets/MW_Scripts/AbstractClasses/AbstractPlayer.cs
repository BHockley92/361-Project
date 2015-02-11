using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public abstract class AbstractPlayer
{
	public string username { get; private set; }
	private string password;
	private PlayerStatus status { get; set; }
	private int wins;
	private int losses;
	private IList<AbstractVillage> myVillages { get; set; }
}
