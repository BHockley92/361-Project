using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public abstract class AbstractPlayer
{
	private string username;
	private string password;
	private PlayerStatus status;
	private int wins;
	private int losses;
	private IList<AbstractVillage> myVillages { get; }
}
