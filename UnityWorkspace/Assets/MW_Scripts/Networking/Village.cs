using UnityEngine;
using System.Collections.Generic;
using GameEnums;

public class Village 
{
	private VillageType myType { get; set; }
	private int gold { get; set; }
	private int wood { get; set; }
	private IList<Tile> controlledRegion { get; }
	private IList<Unit> supportedUnits { get; }
	private Tile location { get; } // TODO: initialize
	Player myPlayer { get; set; } // TODO: initialize -- preferable to NOT have a setter and assign on construction?

	public Village(IList<Tile> region) // Is there a true sequence interface/class?
	{
		myType = VillageType.Hovel;
		gold = 0;
		wood = 0;

		controlledRegion = region;
		supportedUnits = new List<Unit> ();
	}

	public void swapControl(Tile myTile, Village other);
}
