using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;

public partial class MultiplayerScore : Panel
{
    public Label Position;
    public Label Name;
    public Label ScoreBig;
    public Label ScoreSmall;

    public MultiplayerScore()
    {
        Position = AddChild<Label>();
        Position.AddClass("position");

        Panel container = AddChild<Panel>();
        container.AddClass("container");

        Name = container.AddChild<Label>();
        Name.AddClass("name");

        Panel scoreContainer = container.AddChild<Panel>();
        scoreContainer.AddClass("score-container");

        ScoreBig = scoreContainer.AddChild<Label>();
        ScoreBig.AddClass("score-big");

        ScoreSmall = scoreContainer.AddChild<Label>();
        ScoreSmall.AddClass("score-small");
    }
}