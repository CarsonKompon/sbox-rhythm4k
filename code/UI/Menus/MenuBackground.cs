using Sandbox;
using Sandbox.UI;
using System;

public partial class MenuBackground : Panel
{
    private float Hue = 0f;
    private float HueTo = 0f;
    [Event.Frame]
    public void OnFrame()
    {
        Hue = MathX.Lerp(Hue, HueTo, Time.Delta * 5f);
        var _am = ((-RealTime.Now * 100) % 200);
        Style.BackgroundPositionX = _am;
        Style.BackgroundPositionY = _am;
        Style.FilterHueRotate = Hue;
    }

    public void SetHue(float hue)
    {
        HueTo = hue;
    }
}