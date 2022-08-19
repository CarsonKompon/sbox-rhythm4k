using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;

public partial class Arrow : Image
{
    public Note Note;
    public int Points = 1;
    public bool Missed = false;
    public void SetNote(Note note)
    {
        Note = note;
        SetClass("lane-" + note.Lane.ToString(), true);
    }
}