using UnityEngine;
using System.Collections.Generic;

// Helper class to be used to pass coordinates into the hex grid
// http://www.redblobgames.com/grids/#coordinates
// the axial coordinates are being used
	// x = q, z = r
public class Coordinate : IEqualityComparer<Coordinate>
{
	public int q { get; set; }
	public int r { get; set; }

	public Coordinate( int pX, int pZ)
	{
		q = pX;
		r = pZ;
	}

	// All interface implementations for IEqualityComparer below
	public override bool Equals(object obj)
	{
		if (obj == null) return false;
		Coordinate objAsGP = obj as Coordinate;
		if (objAsGP == null) return false;
		return (this.q == objAsGP.q && this.r == objAsGP.r);
	}
	
	public bool equals(Coordinate p)
	{
		return (this.q == p.q && this.r == p.r);
	}
	
	public override int GetHashCode()
	{
		return this.r * 100 + this.r;
	}
	
	public override string ToString ()
	{
		return "(" + q + ", " + r + ")";
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
