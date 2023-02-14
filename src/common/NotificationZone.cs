using Godot;
using System;

public partial class NotificationZone : Node3D
{
    [Export] private Control _popNotification;

    private void OnEnterNotificationArea(Node body)
    {
        {
            if (body.IsInGroup("Player"))
            {
                _popNotification.Visible = true;
            }
        }
    }

    private void OnExitNotificationArea(Node body)
    {
        {
            if (body.IsInGroup("Player"))
            {
                _popNotification.Visible = false;
            }
        }
    }
}