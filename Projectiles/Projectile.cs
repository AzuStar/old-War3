using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using static War3Api.Common;

namespace NoxRaven
{
    public abstract class Projectile : IDisposable
    {
        /// <summary>
        /// Get it from map
        /// </summary>
        public static readonly int DUMMY_ID = FourCC("proj");
        public static readonly int CROW_FORM = FourCC("Amrf");

        /// <summary>
        /// Using neutral passive as they have no unit limitation, if that doesn't work, use neutral hostile
        /// </summary>
        public static readonly player OWNER_ID = Player(PLAYER_NEUTRAL_PASSIVE);
        public const float DEFAULT_UNIT_COLL = 24;
        public const float DEFAULT_DEST_COLL = 24;

        public static Dictionary<int, Projectile> projectiles = new Dictionary<int, Projectile>();

        private static trigger _recycleTrig = CreateTrigger();

        public static void InitProjectileLogic()
        {
            region reg = CreateRegion();
            rect rec = GetWorldBounds();
            RegionAddRect(reg, rec);
            TriggerRegisterLeaveRegion(
                _recycleTrig,
                reg,
                Filter(() =>
                {
                    unit u = GetLeavingUnit();
                    if (GetUnitTypeId(u) == DUMMY_ID)
                    {
                        Projectile proj = projectiles[GetHandleId(u)];
                        if (proj != null)
                        {
                            proj.Dispose();
                        }
                    }
                    return false;
                })
            );
            RemoveRect(rec);
            Master.s_globalTick.Add(_Update);
        }

        private static void _Update(float delta)
        {
            foreach (Projectile proj in projectiles.Values)
            {
                proj.Update(delta);
            }
        }

        public Projectile(NAgent owner, Vector2 position, float facingAngle)
        {
            this.owner = owner;
            this.facing = facingAngle;
            this.wc3agent = CreateUnit(OWNER_ID, DUMMY_ID, position.X, position.Y, facingAngle);
            UnitAddAbility(wc3agent, CROW_FORM);
            UnitRemoveAbility(wc3agent, CROW_FORM);
            SetUnitX(wc3agent, position.X);
            SetUnitY(wc3agent, position.Y);
            _sfx = AddSpecialEffectTarget(sfxPath, wc3agent, "origin");
            projectiles.Add(GetHandleId(wc3agent), this);
        }

        public unit wc3agent;
        public NAgent owner;

        public Vector2 backtrackPosition;
        public Vector2 position
        {
            get => new Vector2(GetUnitX(wc3agent), GetUnitY(wc3agent));
            set
            {
                SetUnitX(wc3agent, value.X);
                SetUnitY(wc3agent, value.Y);
            }
        }
        public float velocity;
        public float facing
        {
            get => GetUnitFacing(wc3agent);
            set
            {
                _facing = value;
                BlzSetUnitFacingEx(wc3agent, value);
            }
        }
        private float _facing;

        // how long should the projectile live for (time or distance)
        /// <summary>
        /// Set -1 for infinite
        /// </summary>
        public float distanceLife = -1;

        /// <summary>
        /// Set -1 for infinite
        /// </summary>
        public float timedLife = -1;

        private float _distanceElapsed = 0;
        private float _timeElapsed = 0;

        private effect _sfx = null;

        public virtual void OnLand() { }

        public virtual void OnExpire() { }

        public virtual void OnUnitCollision(NAgent target) { }

        // public virtual void OnDestructibleCollision(widget ds) { }


        protected virtual string sfxPath => "";

        protected virtual void Update(float delta)
        {
            Utils.Debug(
                "LocZ:"
                    + Utils.GetZ(position)
                    + "  DummyZ:"
                    + BlzGetUnitZ(wc3agent)
                    + "/"
                    + BlzGetLocalUnitZ(wc3agent)
            );
            backtrackPosition = position;
            position = Maffs.PolarProjection(position, velocity * delta, facing);
            if (timedLife > 0)
            {
                _timeElapsed += delta;
                if (_timeElapsed >= timedLife)
                {
                    Dispose();
                }
            }
            if (distanceLife > 0)
            {
                _distanceElapsed += Maffs.GetSquaredDistance(backtrackPosition, position);
                if (_distanceElapsed >= distanceLife * distanceLife)
                {
                    Dispose();
                }
            }
            // search units in collision radius
            List<NAgent> units = Utils.GetUnitsInRange(position, DEFAULT_UNIT_COLL);
            if (units.Count > 0)
            {
                foreach (NAgent u in units)
                {
                    if (u != owner)
                    {
                        OnUnitCollision(u);
                    }
                }
            }
        }

        public virtual void Dispose()
        {
            projectiles.Remove(GetHandleId(wc3agent));
            DestroyEffect(_sfx);
            RemoveUnit(wc3agent);
        }
    }
}
