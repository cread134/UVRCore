using Core.XRFramework.Physics;
using System;

public class PhysicsObjectOverride : IDisposable
{
    private readonly PhysicsObject physicsObject;

    public PhysicsObjectOverride(PhysicsObject physicsObject)
    {
        this.physicsObject = physicsObject;

        physicsObject.CollisionActive = false;
        physicsObject.IsKinematic = true;
    }


    public void Dispose()
    {
        physicsObject.CollisionActive = true;
        physicsObject.IsKinematic = false;
    }
}