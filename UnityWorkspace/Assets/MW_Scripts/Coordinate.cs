using UnityEngine;
using System.Collections.Generic;

// Helper class to be used to pass x and y coordinates
public class Coordinate : IEqualityComparer<GridPoint>
{
	public int x { get; set; }
	public int y { get; set; }

	public Coordinate( int pX, int pY)
	{
		x = pX;
		y = pY;
	}

	// All interface implementations for IEqualityComparer below
	public override bool Equals(object obj)
	{
		if (obj == null) return false;
		Coordinate objAsGP = obj as Coordinate;
		if (objAsGP == null) return false;
		return (this.x == objAsGP.x && this.y == objAsGP.y);
	}
	
	public bool equals(Coordinate p)
	{
		return (this.x == p.x && this.y == p.y);
	}
	
	public override int GetHashCode()
	{
		return this.x * 100 + this.y;
	}
	
	public override string ToString ()
	{
		return "(" + x + ", " + y + ")";
	}
	
	public bool Equals(Coordinate p1, Coordinate p2)
	{
		return p1.equals(p2);
	}
	
	public int GetHashCode(Coordinate p)
	{
		return p.GetHashCode ();
	}
}
