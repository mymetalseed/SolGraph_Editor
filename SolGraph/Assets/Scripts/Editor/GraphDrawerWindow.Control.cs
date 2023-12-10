using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GraphDrawerWindow 
{
    private float dragThreshold = 1f;
    public Vector2 panOffset { get { return _panOffset; } set { _panOffset = value; Repaint(); } }
    private Vector2 _panOffset;
    public static bool isPanning { get; private set; }
    private Vector2 lastMousePos = new Vector2(0,0);
    private bool clickDirty = false;
    public void Control()
    {
        Event e = Event.current;
        switch (e.type)
        {
            case EventType.MouseDown:
                Repaint();
                if(e.button == 0)
                {
                    lastMousePos = e.mousePosition;
                    clickDirty = true;
                }
                break;
            case EventType.MouseDrag:
                if(e.button==1 || e.button == 2)
                {
                    if (e.delta.magnitude > dragThreshold)
                    {
                        panOffset += e.delta/1.4f;
                        isPanning = true;
                    }
                }
                break;
            case EventType.MouseUp:
                if (e.button==1 || e.button == 2)
                {
                    isPanning = false;
                }
                break;
        }
    }

    private bool CheckTouched(Vector2 nodePos)
    {
        Vector2 finalMousePos = WindowToGridPos(lastMousePos);
        if(finalMousePos.x > nodePos.x 
            && finalMousePos.x < nodePos.x + 208
            && finalMousePos.y > nodePos.y
            && finalMousePos.y < nodePos.y + 80
            )
        {
            return true;
        }
        return false;
    }

    private Vector2 WindowToGridPos(Vector2 pos)
    {
        return pos - _panOffset;
    }
}
