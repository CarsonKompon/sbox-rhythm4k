using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;

public partial class Arrow : Image
{
    public int LaneIndex = 0;
    public void SetLane(int i)
    {
        LaneIndex = i;
        SetClass("lane-" + LaneIndex.ToString(), true);
    }
}