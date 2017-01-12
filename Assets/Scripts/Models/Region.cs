using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Region
{
    public List<Tile> regionTiles = new List<Tile>();
    public List<Connector> connectorTiles = new List<Connector>();
    public int index;

    public bool PartOfMain
    {
        get
        {
            return connectorTiles == null || index == 1;
            // HACK: This works only beacuse we are re-setting connector tiles in Dungeon.cs Line 163
        }
    }

    public Region(int index)
    {
        this.index = index;
    }
}
