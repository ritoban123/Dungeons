using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Connector
{
    public Tile Tile { get; protected set; }
    public Region r1;
    public Region r2;

    public Connector(Tile tile, Region r1, Region r2)
    {
        Tile = tile;
        this.r1 = r1;
        this.r2 = r2;
    }

    public void CheckMainRegion(Region mainRegion)
    {
        if (r1.PartOfMain)
            r1 = mainRegion;
        if (r2.PartOfMain)
            r2 = mainRegion;
    }

    /// <summary>
    /// Given A Region r, will return the region that this connector connectors to, that is not r.
    /// Returns null if r is not a region this connector connects to
    /// </summary>
    /// <param name="r">The region</param>
    /// <returns></returns>
    public Region GetOtherRegion(Region r, bool rIsMain)
    {
        if (rIsMain == false)
        {
            if (r == r1)
                return r2;
            else if (r == r2)
                return r1;
            else
                return null;
        }
        else
        {
            if (r1.PartOfMain)
                return r2;
            else if (r2.PartOfMain)
                return r1;
            else
                return null;
        }
    }
}

