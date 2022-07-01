using System;
using UnityEngine;
using UnityEditor;
using System.Linq;

#if (UNITY_EDITOR)
[CustomEditor(typeof(Moveable))]
class MoveableInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Moveable moveable = target as Moveable;
        //specify type
        if (moveable == null)
        {
            return;
        }

        if (moveable.PrefabName == "")
            moveable.PrefabName = moveable.name;
    }
}

[CustomEditor(typeof(Room))]
class RoomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Room room = target as Room;
        //specify type
        if (room == null)
        {
            return;
        }

        if (room.PrefabName == "")
            room.PrefabName = room.name;
    }
}
#endif