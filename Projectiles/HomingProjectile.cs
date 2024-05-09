using System.Numerics;

namespace NoxRaven
{
    public abstract class HomingProjectile : Projectile
    {
        protected HomingProjectile(NAgent owner, Vector2 position, float facingAngle)
            : base(owner, position, facingAngle) { }
    }
}
