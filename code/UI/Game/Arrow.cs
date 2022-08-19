using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;

public partial class Arrow : Image
{
    public Note Note;
    public void SetNote(Note note)
    {
        Note = note;
        SetClass("lane-" + note.Lane.ToString(), true);
    }
}