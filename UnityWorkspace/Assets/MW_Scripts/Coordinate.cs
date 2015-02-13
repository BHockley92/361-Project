using UnityEngine;
using System.Collections.Generic;

// Helper class to be used to pass coordinates into the hex grid
// http://www.redblobgames.com/grids/#coordinates
// the axial coordinates are being used
	// x = q, z = r
public class Coordinate : IEqualityComparer<Coordinate>
{
	public int x { get; set; }
	public int z { get; set; }

	public Coordinate( int pX, int pZ)
	{
		x = pX;
		z = pZ;
	}

	// All interface implementations for IEqualityComparer below
	public override bool Equals(object obj)
	{
		if (obj == null) return false;
		Coordinate objAsGP = obj as Coordinate;
		if (objAsGP == null) return false;
		return (this.x == objAsGP.x && this.z == objAsGP.z);
	}
	
	public bool equals(Coordinate p)
	{
		return (this.x == p.x && this.z == p.z);
	}
	
	public override int GetHashCode()
	{
		return this.x * 100 + this.z;
	}
	
	public override string ToString ()
	{
		return "(" + x + ", " + z + ")";
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
