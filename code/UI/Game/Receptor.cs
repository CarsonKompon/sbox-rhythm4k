using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;

public partial class Receptor : Panel
{
    public int LaneIndex = 0;
    public Image Sprite;
    public Image GlowSprite;

    public Receptor()
    {
        Sprite = AddChild<Image>();
        Sprite.SetClass("receptor-arrow", true);
        GlowSprite = AddChild<Image>();
        GlowSprite.SetClass("receptor-glow", true);
    }

    public void SetLane(int i)
    {
        LaneIndex = i;
        string _class = "lane-" + LaneIndex.ToString();
        AddClass(_class);
        Sprite.AddClass(_class);
        GlowSprite.AddClass(_class);
    }
}