using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;

public partial class Receptor : Panel
{
    public int LaneIndex = 0;
    public Image Sprite;
    public Image GlowSprite;
    public RealTimeSince Timer = 1f;

    public Receptor()
    {
        Sprite = AddChild<Image>();
        Sprite.SetClass("receptor-arrow", true);
        GlowSprite = AddChild<Image>();
        GlowSprite.SetClass("receptor-glow", true);
    }

    [Event.Frame]
    public void OnFrame()
    {
        if(Timer > 0 && GlowSprite.HasClass("show"))
        {
            GlowSprite.RemoveClass("show");
        }
    }

    public void SetLane(int i)
    {
        LaneIndex = i;
        string _class = "lane-" + LaneIndex.ToString();
        AddClass(_class);
        Sprite.AddClass(_class);
        GlowSprite.AddClass(_class);
    }

    public void Glow(Panel panel)
    {
        GlowSprite.Style.FilterHueRotate = panel.Style.FilterHueRotate;
        GlowSprite.Style.FilterSaturate = panel.Style.FilterSaturate;
        GlowSprite.AddClass("show");
        Timer = -0.1f;
    }
}