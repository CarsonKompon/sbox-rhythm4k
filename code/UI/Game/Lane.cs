using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;

public partial class Lane : Panel
{
    public int LaneIndex = 0;
    public List<Arrow> Arrows = new();
    public Receptor Receptor;

    public Lane()
    {
        Receptor = AddChild<Receptor>();
    }

    public void SetLane(int i)
    {
        LaneIndex = i;
        Receptor.SetLane(i);
    }
}