using System;
using System.Collections.Generic;
using System.Text;
using NoxRaven.Units;
using static War3Api.Common;

namespace NoxRaven
{
    public abstract class Projectile
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

        public const float REMOVAL_DELAY = 2;

        private static location _tempLoc = Location(0, 0);
        private static trigger _recycleTrig = CreateTrigger();

        public static void InitProjectileLogic()
        {
            region reg = CreateRegion();
            rect rec = GetWorldBounds();
            RegionAddRect(reg, rec);
            TriggerRegisterLeaveRegion(_recycleTrig, reg, Filter(() =>
            {
                return false;
            }));
            RemoveRect(rec);
            Master.s_globalTick.Add(_Update);

        }
        private static void _Update(float delta)
        {

        }

        //public static Dictionary<>

        public unit wc3agent;
        public NUnit owner;
        //? nullables
        public NUnit target = null;
        public float destructiobleHitRadius = DEFAULT_DEST_COLL;
        public float unitHitRadius = DEFAULT_UNIT_COLL;
        public float originalSpeed = 0;
        public float oldSpeed = 0;
        public float zOffset = 0;
        public float timedLife = 0;
        public float timeScale = 1;
        public float removalDelay = REMOVAL_DELAY;
        public bool pauseProj = false;
        public bool allowExpiration = false;
        public bool allowArcAnimReset = false;
        public bool allowTargetHoming = false;
        public float arcSize = 0;
        public float x = 0;//=> GetUnitX(wc3agent);
        public float y = 0;//=> GetUnitY(wc3agent);
        public float z = 0;//=> GetUnitFlyHeight(wc3agent);
        public float velX = 0;
        public float velY = 0;
        public float velZ = 0;
        public float tarX = 0;
        public float tarY = 0;
        public float tarZ = 0;
        public float startX = 0;
        public float startY = 0;
        public float startZ = 0;
        public float time = 0;
        public float accZ = 0;






        public Action onExpire = null;
        public Action onLandCollision = null;
        public Action onUnitCollision = null;
        public Action onDestructibleCollision = null;

        private effect _sfx = null;
        private float _scale = 0;
        private string _path = "";
        private float _speed = 0;

        public virtual void Update()
        {
                //         //  Periodic method: Update projectile movement.
    //         private method periodic takes nothing returns nothing   


    //                 if not this.pauseProj then                    
    //                     if this.target != null and this.allowTargetHoming and UnitAlive(this.target) then
    //                         set TempX = GetUnitX(this.target)
    //                         set TempY = GetUnitY(this.target)
    //                         call MoveLocation(TempL,TempX,TempY)
    //                         set TempZ = GetUnitFlyHeight(this.target) + GetLocationZ(TempL) + this.zOffset
    //                         if this.tarX != TempX or this.tarY != TempY or this.tarZ != TempZ then
    //                             set this.tarX  = TempX
    //                             set this.tarY  = TempY
    //                             set this.tarZ  = TempZ
    //                             set this.update = true
    //                         endif
    //                     endif

    //                     if this.update then
    //                         set TempX     = this.tarX - this.posX
    //                         set TempY     = this.tarY - this.posY
    //                         set TempZ     = this.tarZ - this.posZ
    //                         set TempD     = SquareRoot(TempX * TempX + TempY * TempY + TempZ * TempZ)
    //                         set this.velX = TempX / TempD * this.speed
    //                         set this.velY = TempY / TempD * this.speed
    //                         set this.time = TempD / this.speed * T32_PERIOD
    //                         if this.time <= 0.00 then
    //                             set this.time = T32_PERIOD
    //                         endif
    //                         if this.arcSize != 0.00 then
    //                             set this.accZ = 2.00 * (TempZ / this.time / this.time * T32_PERIOD * T32_PERIOD - (this.velZ * T32_PERIOD) / this.time)
    //                         else
    //                             set this.velZ = TempZ / TempD * this.speed
    //                             set this.accZ = 0.00
    //                         endif
    //                         call SetUnitFacing(this.dummy,Atan2(TempY,TempX) * bj_RADTODEG)
    //                         set this.update = false
    //                     endif

    //                     set TempP     = this.posZ
    //                     set this.velZ = this.velZ + this.accZ * this.timeScale
    //                     set this.posX = this.posX + this.velX * this.timeScale
    //                     set this.posY = this.posY + this.velY * this.timeScale
    //                     set this.posZ = this.posZ + this.velZ * this.timeScale

    //                     call MoveLocation(TempL,this.posX,this.posY)
    //                     set TempZ = GetLocationZ(TempL)

    //                     call SetUnitX(this.dummy,this.posX)
    //                     call SetUnitY(this.dummy,this.posY)
    //                     call SetUnitFlyHeight(this.dummy,this.posZ - TempZ,0.00)
    //                     call SetUnitAnimationByIndex(this.dummy,R2I(bj_RADTODEG * Atan2((this.posZ - TempP),SquareRoot(this.velX * this.velX + this.velY * this.velY) * this.timeScale) + 90.50))

    //                     static if LIBRARY_ProjShadows then
    //                         if this.hasShadow then
    //                             call this.updateShadow()
    //                         endif
    //                     endif

    //                     set this.timedLife = this.timedLife - T32_PERIOD * this.timeScale

    //                     if this.timedLife <= 0.00 then
    //                         if this.allowExpiration then
    //                             set this.stop = true
    //                         endif
    //                         if this.onExpire != 0 then
    //                             call this.onExpire.execute(this)
    //                         endif
    //                     endif

    //                     if this.posZ <= TempZ then
    //                         static if LIBRARY_ProjBounces then
    //                             if this.allowBouncing then
    //                                 call this.bounce()
    //                             endif
    //                         endif
    //                         if this.onLand != 0 then
    //                             call this.onLand.execute(this)
    //                         endif
    //                     endif
    //                 endif
    //             endif
    //         endmethod
            if(onDestructibleCollision != null)
            {
                // TODO
            }
            if(onUnitCollision != null)
            {
                // TODO
            }

        }

        public Projectile(NUnit owner, float x, float y, float z, float angle)
        {
            unit projDummy = CreateUnit(OWNER_ID, DUMMY_ID, x, y, angle);
            UnitAddAbility(projDummy, CROW_FORM);
            UnitRemoveAbility(projDummy, CROW_FORM);
            SetUnitX(projDummy, x);
            SetUnitY(projDummy, y);
            SetUnitFlyHeight(projDummy, z, 0);
            MoveLocation(_tempLoc, x, y);
            this.x = x;
            this.y = y;
            this.z = z + GetLocationZ(_tempLoc);
            startX = x;
            startY = y;
            startZ = z + GetLocationZ(_tempLoc);

        }

        public void Destroy()
        {
            //             static if LIBRARY_ProjShadows then
            //                 if this.hasShadow then
            //                     call this.detachShadow()
            //                 endif
            //             endif
            if (allowArcAnimReset)
            {
                SetUnitAnimationByIndex(wc3agent, 90);
            }
            if (_sfx != null)
            {
                DestroyEffect(_sfx);
            }
            RemoveUnit(wc3agent);
        }

        public float scale
        {
            get => _scale;
            set
            {
                _scale = value;
                SetUnitScale(wc3agent, value, 0, 0);
            }
        }
        public string effectPath
        {
            get => _path;
            set
            {
                if (_sfx != null)
                {
                    DestroyEffect(_sfx);
                }
                if (!String.IsNullOrEmpty(value))
                {
                    _sfx = AddSpecialEffectTarget(value, wc3agent, "origin");
                    _path = value;
                }
            }
        }
        public float speed {
            get => _speed / Master.TICK_DELTA;
            set
            {
                float factor = value * Master.TICK_DELTA / _speed;
                if (value > 0)
                {
                    oldSpeed = _speed;
                    _speed = value * Master.TICK_DELTA;
                    time = time / factor;
                    velX = velX * factor;
                    velY = velY * factor;
                    velZ = velZ * factor;
                    accZ = accZ * factor * factor;
                }
            }
        }
        public float distanceMax => SquareRoot((tarX - startX) * (tarX - startX) + (tarY - startY) * (tarY - startY));
        public float distanceLeft => SquareRoot((tarX - x) * (tarX - x) + (tarY - y) * (tarY - y));
        public float distanceDone => SquareRoot((x - startX) * (x - startX) + (y - startY) * (y - startY));
        public float previousSpeed => oldSpeed / Master.TICK_DELTA;
        // public float originalSpeed => _speed / Master.TICK_DELTA;
        public float angle => Atan2((y+velY) - y, (x+velX) - x);

            //         private method setCommon takes real xPos, real yPos, real zPos, real speed, real arc returns nothing
    //             local real tempX   = xPos - this.posX
    //             local real tempY   = yPos - this.posY
    //             local real tempZ   = zPos - this.posZ
    //             local real tempD   = SquareRoot(tempX * tempX + tempY * tempY + tempZ * tempZ)

    //             if speed <= 0.00 or tempD <= 0.00 then
    //                 set this.posX  = xPos
    //                 set this.posY  = yPos
    //                 set this.posZ  = zPos
    //                 set this.time  = 0.00
    //             else
    //                 set this.time  = tempD / speed
    //             endif

    //             set this.speed     = speed * T32_PERIOD
    //             set this.oriSpeed  = this.speed
    //             set this.oldSpeed  = this.speed
    //             set this.arcSize   = arc
    //             set this.tarX      = xPos
    //             set this.tarY      = yPos
    //             set this.tarZ      = zPos
    //             set this.velX      = tempX / tempD * this.speed
    //             set this.velY      = tempY / tempD * this.speed
    //             set this.velZ      = ((tempD * arc) / (this.time / 4.00) + tempZ / this.time ) * T32_PERIOD

    //             if this.timedLife == 0.00 then
    //                 set this.timedLife = this.time
    //             endif
    //         endmethod
        private void RecalculateCommon(float x, float y, float z, float spd, float arc)
        {
            float tempX = x - this.x;
            float tempY = y - this.y;
            float tempZ = z - this.z;
            float tempD = SquareRoot(tempX * tempX + tempY * tempY + tempZ * tempZ);

            if (spd <= 0 || tempD <= 0)
            {
                this.x = x;
                this.y = y;
                this.z = z;
                time = 0;
            }
            else
            {
                time = tempD / spd;
            }

            _speed = spd * Master.TICK_DELTA;
            originalSpeed = _speed;
            oldSpeed = _speed;
            velX = tempX / tempD * _speed;
            velY = tempY / tempD * _speed;
            velZ = ((tempD * arc) / (time / 4) + tempZ / time) * Master.TICK_DELTA;
            accZ = 0;
            tarX = x;
            tarY = y;
            tarZ = z;
        }

    }

    //     //------------------------------------------------------------------------------------------------------------------------\\
    //     //
    //     //  Projectile API:
    //     // ________________
    //     //    • This is a quick over-view of all the methods, functions and members available to use with this system.
    //     //
    //     //    Projectile methods:
    //     //   ____________________
    //     //      • These methods are the main methods used by the system to create, manipulate and destroy projectiles. They
    //     //        include:
    //     //
    //     //        - projectile.create()    - this.addUnit()
    //     //
    //     //        - this.projectNormal()   - this.isUnitAdded()
    //     //
    //     //        - this.projectArcing()   - this.removeUnit()
    //     //
    //     //        - this.setTargPoint()    - this.removeAll()
    //     //
    //     //        - this.setProjPoint()    
    //     //
    //     //        - this.disjoint()        - this.getData()
    //     //
    //     //        - this.refresh()         - this.hasData()
    //     //
    //     //        - this.terminate()       - this.detachData()
    //     //
    //     //    Public Projectile members:
    //     //   ___________________________
    //     //      • These members are publically available to users at any time throughout the lifetime of a projectile. They can be
    //     //        changed and manipulated at any point. They include:
    //     //
    //     //        - this.sourceUnit        - this.currentSpeed      - this.onExpire          - this.allowDestCollisions
    //     //
    //     //        - this.targetUnit        - this.timedLife         - this.onLand            - this.allowTargetHoming
    //     //
    //     //        - this.owningPlayer      - this.damageDealt       - this.onUnit            - this.removalDelay
    //     //
    //     //        - this.unitHitRadius     - this.pauseProj         - this.onDest
    //     //
    //     //        - this.destHitRadius     - this.effectPath        - this.allowDeathSfx 
    //     //
    //     //        - this.scaleSize         - this.onStart           - this.allowExpiration
    //     //
    //     //        - this.timeScale         - this.onLoop            - this.allowArcAnimReset
    //     //
    //     //        - this.zOffset           - this.onFinish          - this.allowUnitCollisions
    //     //
    //     //    Readonly Projectile members:
    //     //   _____________________________
    //     //      • These members are only available for users to read. They cannot be changed or manipulated (unless through the 
    //     //        above methods, i.e. .setTargPoint), they are there for reference only. They include:
    //     //
    //     //        - projetile[unit]        - this.velX              - this.strZ
    //     //
    //     //        - this.dummy             - this.velY              - this.distanceMax
    //     //
    //     //        - this.arcSize           - this.velZ              - this.distanceLeft
    //     //
    //     //        - this.angle             - this.tarX              - this.distanceDone
    //     //
    //     //        - this.isTerminated      - this.tarY              - this.previousSpeed
    //     //
    //     //        - this.posX              - this.tarZ              - this.originalSpeed
    //     //
    //     //        - this.posY              - this.strX
    //     //
    //     //        - this.posZ              - this.strY
    //     //
    //     //------------------------------------------------------------------------------------------------------------------------\\
    //     //
    //     //------------------------------------------------------------------------------------------------------------------------\\
    //     //
    //     //  Detailed Projectile API descriptions:
    //     // ______________________________________
    //     //    Methods:
    //     //   _________
    //     //      • projectile.create(real x, real y, real z, real angle) -> returns projectile
    //     //
    //     //        - Above is the only method for creating projectiles. It takes four simple parameters: The starting x, y and z
    //     //          coordinates of the projectile you want to create, and the angle you want it to face. The angle for the
    //     //          facing direction of the projectile is taken in RADIANS. This method will return the created projectile for you
    //     //          to save into a variable.
    //     //
    //     //      • this.projectNormal(real x, real y, real z, real speed) -> returns boolean   
    //     //
    //     //        - Above is one of the two different methods that can be chosen for launching a projectile. This method is the
    //     //          simpler of the two, which doesn't allow for an arcing movement. It takes the target x, y and z coordinates
    //     //          of the projectile and the speed at which you want the projectile to move. This method will return true if the
    //     //          projectile successfully launched and false if it didn't (possibly because it had already been launched).
    //     //
    //     //      • this.projectArcing(real x, real y, real z, real speed, real arcSize) -> returns boolean
    //     //
    //     //        - Similar to the above method, this method is also used for launching a projectile, however this method allows
    //     //          for the projectile to arc along the z axis. It takes the target x, y and z coordinates of the projectile, the
    //     //          speed at which the projectile will move and the size of the arc you want. Values for the arcSize should range
    //     //          from 0.00 to 1.00 (0.10-0.40 recommended). This method returns the same boolean as this.projectNormal().
    //     //
    //     //      • this.setTargPoint(real x, real y, real z, boolean wantUpdate) -> returns nothing
    //     // 
    //     //        - The above method can be used to change the target coordinates of a projectile, effectively changing where the
    //     //          projectile will move to. The method takes the x, y and z coordinates of the new target destination, which will
    //     //          update the direction of the projectile in the next iteration of the timer. The boolean parameter at the end
    //     //          determines whether or not the system will update the projectile's new velocities according to the new target
    //     //          coordinates (there are some situations in which update and not updating are applicable).
    //     //
    //     //      • this.setProjPoint(real x, real y, real z, boolean wantUpdate) -> returns nothing
    //     //
    //     //        - The above method can be used to change the current position of a projectile, without changing its target
    //     //          coordinates or any other aspect of the projectile. The method takes the x, y and z coordinates of the new
    //     //          position of the projectile. This method does not return anthing (same with the above method). The boolean
    //     //          parameter at the end determines whether or not the system will update the projectile's new velocities 
    //     //          according to the new position coordinates (there are some situations in which update and not updating are
    //     //          applicable).
    //     //
    //     //      • this.disjoint() -> returns nothing
    //     //
    //     //        - This method is basically an easy way for the user to remove the target unit of a projectile and effectively
    //     //          make it target the ground and discontinue its homing capabilities. The method has no parameters and can simply
    //     //          be used whenever the user wants. This works excellently if you want a projectile to disjoint when a unit blinks.
    //     //
    //     //      • this.refresh(boolean fireOnStart) -> returns nothing
    //     //
    //     //        - This method will stop a projectile from moving and enable it to be launched again via the .projectNormal and
    //     //          .projectArcing methods. It allows users to quickly stop and start a projectile with a different arc height as
    //     //          as well as target coordinates and speed. The boolean parameter is used to determine whether or not the 
    //     //          .onStart function interface member should be executed again, or removed from the projectile. Using true as the
    //     //          parameter will allow the .onStart function to be executed again, while false will remove it completely.
    //     //
    //     //      • this.terminate() -> returns nothing
    //     //
    //     //        - The above method is used to terminate (AKA destroy) a projectile. This will in turn remove the projectile from
    //     //          the game as well as any data associated with it. Projectiles will be removed from any projgoups they are in.
    //     //          This method takes no parameters and can be used whenever the user wants.
    //     //
    //     //      • this.addUnit(unit u) -> returns nothing
    //     //
    //     //        - The above method was added for completeness. It will add a unit to the 'already passed through' group of a 
    //     //          projectile without firing the unit impact event. This can be used to quickly add units to the group that you
    //     //          know won't be a target of a projectile. It takes a unit parameter, which is the unit that will be added.
    //     //
    //     //      • this.isUnitAdded(unit u) -> returns boolean
    //     //
    //     //        - The above method was also added for completeness. It will check to see if a given unit is already in the
    //     //          'already passed through' group of a projectile. This can be used in conjunction with this.addUnit() and
    //     //          this.removeUnit() to do some interesting things. It takes a unit parameter.
    //     //
    //     //      • this.removeUnit(unit u) -> returns nothing
    //     //
    //     //        - The above method is the last of the functions added for completeness. This method will remove a given unit
    //     //          from a projectile's 'already passed through' group so that it may fire the unit impact event again (or maybe
    //     //          for the first time if this.addUnit() was used on it). It takes a unit parameter.
    //     //
    //     //      • this.removeAll() -> returns nothing
    //     //
    //     //        - This method can be used to completely clear all units that have been hit by a projectile. Doing so will make
    //     //          all previously hit units available to be hit once more. Can be used with projectiles that travel to a target
    //     //          point and return, so that damage can be dealt twice.
    //     //
    //     //
    //     //    Public members:
    //     //   ________________
    //     //      • this.sourceUnit          -> The unit that can be saved as the source of the projectile (for damage events).
    //     //      • this.targetUnit          -> A unit can be saved as the target unit to enable homing capabilities.
    //     //      • this.owningPlayer        -> Can be set to the owning player of the source unit.
    //     //      • this.damageDealt         -> Damage amounts can be saved directly to a projectile for when it hits a unit.
    //     //      • this.unitHitRadius       -> The radius in which a unit must be for it to be hit by a projectile.
    //     //      • this.destHitRadius       -> The radius in which a destructable must be for it to be hit by a projectile.
    //     //      • this.zOffset             -> Can save an approximate height for projectile impacts on a target unit.
    //     //      • this.timeScale           -> Time scale of a projectile. Can be used to slow down or speed up a projectile.
    //     //      • this.timedLife           -> The life span of a projectile. Can be changed to anything. Defaults to dist / speed.
    //     //      • this.scaleSize           -> Determines the size of the projectile (similar to scale size for units).
    //     //      • this.effectPath          -> The model that will be attached to the dummy unit to create the projectile effect.
    //     //      • this.currentSpeed        -> Determines the movement speed of a projectile.
    //     //      • this.removalDelay        -> Determines how long until the projectile will be removed if .allowDeathSfx is true.
    //     //      • this.pauseProj           -> Whether or not a projectile should be paused or not (collision works while paused).
    //     //      • this.allowDeathSfx       -> Whether or not to show the death effect of a projectile. Defaults to false.
    //     //      • this.allowExpiration     -> If a projectile will terminate once its timed life is up. Defaults to false.
    //     //      • this.allowArcAnimReset   -> Whether or not to reset a projectile's arc animation on death. Helps display effects.
    //     //      • this.allowTargetHoming   -> Whether or not to allow homing on a target unit for a projectile. Defaults to false.
    //     //      • this.allowUnitCollisions -> Whether or not to allow projectile and unit collision. Defaults to false.
    //     //      • this.allowDestCollisions -> Whether or not to allow projectile and destructable collisions. Defaults to false.
    //     //
    //     //      Function interfaces as members:
    //     //     ________________________________
    //     //        - this.onStart           -> Can be set to a function that will be executed when a projectile is launched.
    //     //        - this.onLoop            -> Can be set to a function that will be executed periodically for a projectile.
    //     //        - this.onFinish          -> Can be set to a function that will be executed when a projectile is terminated.
    //     //        - this.onExpire          -> Can be set to a function that will be executed when a projectile's timed life expires.
    //     //        - this.onLand            -> Can be set to a function that will be executed whenever a projectile hits terrain.
    //     //
    //     //          • All of the above will execute a function that a user can specify to create specific event responses for
    //     //            different projectiles. These are higher end members that take a little more knowledge to master. For 
    //     //            example:
    //     //
    //     //              private function OnLandImpact takes projectile whichProj returns nothing
    //     //                  call whichProj.terminate() <--- The function OnLandImpact must take a projectile parameter.
    //     //              endfunction
    //     //            
    //     //              private function Actions takes nothing returns nothing
    //     //                  local projectile p  = 0
    //     //                  local unit       u  = GetTriggerUnit()
    //     //                  local real       ux = GetUnitX(u)
    //     //                  local real       uy = GetUnitY(u)
    //     //                  local real       tx = GetSpellTargetX()
    //     //                  local real       ty = GetSpellTargetY()
    //     //         
    //     //                  set p = projectile.create(ux,uy,50.00,Atan2((ty - uy),(tx - ux)))
    //     //
    //     //                  set p.sourceUnit          = u
    //     //                  set p.owningPlayer        = GetOwningPlayer(u)
    //     //                  set p.effectPath          = "Abilities\\Weapons\\AncientProtectorMissile\\AncientProtectorMissile.mdl"
    //     //                  set p.scaleSize           = 0.75
    //     //                  set p.zOffset             = 0.00
    //     //    
    //     //                  set p.allowUnitCollisions = false
    //     //                  set p.allowProjCollisions = false
    //     //                  set p.allowDestCollisions = false
    //     //    
    //     //                  set p.onLand              = OnLandImpact <--- Notice how p.onLand is set to the function OnLandImpact.
    //     //    
    //     //                  call p.projectNormal(tx,ty,0.00,1024.00)
    //     //
    //     //                  set u = null
    //     //              endfunction
    //     //
    //     //          • These five function interface members can be set to a function that takes a single projectile parameter and
    //     //            returns nothing. This is very important! The functions can be named whatever a user likes, but they must
    //     //            follow this rule. The single projectile parameter for the function refers to a projectile that has just 
    //     //            completed an event (be it .onLand or .onStart), and can therefore be manipulated by the user to do something
    //     //            unique for that event.
    //     //
    //     //        - this.onUnit            -> Can be set to a function that will be executed when a projectile hits a unit.
    //     //        - this.onDest            -> Can be set to a function that will be executed when a projectile hits a destructable.
    //     //
    //     //          • The above are function interfaces that are executed when a projectile hits another object, such as a unit,
    //     //            projectile or destructable. These function in a very similar way to the other function interface members,
    //     //            however they take two parameters, one projectile and one of a specific type that relates to the event. For 
    //     //            example:
    //     //
    //     //              private function OnUnitImpact takes projectile p, unit u returns nothing
    //     //                  call UnitDamageTarget(p.sourceUnit,u,p.damageDealt,false,fasle,null,null,null)
    //     //                  call p.terminate()
    //     //              endfunction
    //     //
    //     //                - As you can see, the above function is for the this.onUnit member. This function must take one
    //     //                  projectile parameter (referring to the projectile hitting the unit) and one unit parameter (referring to
    //     //                  the unit being hit). It must return nothing.
    //     //
    //     //              private function OnDestImpact takes projectile p, destructable d returns nothing
    //     //                  call KillDestructable(d)
    //     //                  call p.terminate()
    //     //              endfunction
    //     //
    //     //                - The above function is for the this.onDest member, used for projectile on destructable collision events.
    //     //                  The function takes one projectile parameter and one destructable parameter, each referring to a specific
    //     //                  object in the collision. This function must return nothing.
    //     //
    //     //          • It is important to note that the functions and their parameters can be named whatever the user wants, but they
    //     //            must remain in the order stated above (projectile parameter always comes first) and the functions must return
    //     //            nothing.
    //     //          • For more examples of how this works, check out the example spells (specifically: Hurl Boulder) in the test
    //     //            map.
    //     //
    //     //    Readonly members:
    //     //   __________________
    //     //      • this.dummy               -> Refers to the dummy unit of a projectile (the actual projectile object).
    //     //      • this.angle               -> Refers to the current facing angle of a projectile.
    //     //      • this.previousSpeed       -> Retrieves the previous speed of a projectile.
    //     //      • this.originalSpeed       -> Retrieves the original speed of a projectile.
    //     //      • this.isTerminated        -> Refers to whether or not a projectile is currently terminated (true if it is).
    //     //      • this.posX/posY/posZ      -> Refers to the current x, y and z coordinates of a projectile.
    //     //      • this.tarX/tarY/tarZ      -> Refers to the current target x, y and z coordinates of a projectile.
    //     //      • this.velX/velY/velZ      -> Refers to the current x, y and z velocities of a projectile.
    //     //      • this.strX/strY/strZ      -> Refers to the starting x, y and z coordinates of a projectile.
    //     //      • this.distanceMax         -> Maximum distance to be travelled by a projectile (start to finish) in a straight line.
    //     //      • this.distanceLeft        -> Distance left to travel for a projectile (current to finish) in a straight line.
    //     //      • this.distanceDone        -> Distance from a projectile's starting point to its current point (straight line).
    //     //      • this.arcSize             -> Retrieves the arc size used for a projectile.
    //     //
    //     //      Static method operator:
    //     //     ________________________
    //     //------------------------------------------------------------------------------------------------------------------------\\
    //     //
    //     //------------------------------------------------------------------------------------------------------------------------\\
    //     //
    //     //           _____           _  _____                       
    //     //          |  __ \         (_)/ ____|                      
    //     //          | |__) | __ ___  _| |  __ _ __ ___  _   _ _ __  
    //     //          |  ___/ '__/ _ \| | | |_ | '__/ _ \| | | | '_ \ 
    //     //          | |   | | | (_) | | |__| | | | (_) | |_| | |_) |
    //     //          |_|   |_|  \___/| |\_____|_|  \___/ \__,_| .__/ 
    //     //                         _/ |                      | |    
    //     //                        |__/                       |_|
    //     //                             By Kenny v0.1.3 (Beta)
    //     //
    //     //------------------------------------------------------------------------------------------------------------------------\\
    //     //
    //     //------------------------------------------------------------------------------------------------------------------------\\
    //     //
    //     //  What is ProjGroup?
    //     // ___________________
    //     //    • ProjGroup is a fully functional projectile group API for the Projectile system, designed to closely match the
    //     //      native WC3 unit groups.
    //     //    • ProjGroup provides common grouping functions for projectiles such as adding and removing projectiles from a group,
    //     //      creating, clearing and destroying groups and enumerating through groups.
    //     //    • ProjGroup was designed to make projectile manipulation easier for users, therefore it has a simplistic interface
    //     //      that is easy to understand. It also comes with a WC3 alternative interface that resembles native WC3 functions.
    //     //
    //     //------------------------------------------------------------------------------------------------------------------------\\
    //     //
    //     //------------------------------------------------------------------------------------------------------------------------\\
    //     //
    //     //  ProjGroup API:
    //     // _______________
    //     //    • This is a quick over-view of all the methods and globals available to users with the projgroup struct.
    //     //
    //     //    ProjGroup methods:
    //     //   ___________________
    //     //      • These methods are the main methods used by the system to create, manipulate and destroy projgroups. They
    //     //        include:
    //     //
    //     //        - projgroup.create()                              - this.forNearby()
    //     //
    //     //        - this.add()                                      - this.forNearbyEx()
    //     //
    //     //        - this.remove()                                   - this.forGroup()
    //     //
    //     //        - this.isInGroup()                                - this.forGroupEx()
    //     //
    //     //        - this.getCount()                                 - this.destroy()
    //     //
    //     //        - this.getFirst()
    //     //
    //     //        - this.clear()
    //     //
    //     //        - this.enumNearby()
    //     //
    //     //    ProjGroup globals:
    //     //   ___________________
    //     //      • These globals are to be used in conjunction with the this.enumNearby and this.forGroup() methods of projgroups
    //     //        to access projectiles. They include:
    //     //
    //     //        - EnumProjectile
    //     //
    //     //        - ParentProjGroup
    //     //
    //     //        - ForProjGroupData
    //     //
    //     //        - GlobalProjGroup
    //     //
    //     //  Alternate ProjGroup API:
    //     // _________________________
    //     //    • This is an alternate interface for projgroups that closely resembles native WC3 group API, for nostalgia's sake.
    //     //
    //     //    ProjGroup functions:
    //     //   _____________________
    //     //      • These functions are the alternate interface for projgroups that encompass both projgroup methods and globals.
    //     //        They include:
    //     //
    //     //        - CreateProjGroup()                 - ForNearbyProjectilesInRange()     - GetGlobalProjGroup()
    //     //
    //     //        - ProjGroupAddProjectile()          - ForNearbyProjectilesInRangeEx()
    //     // 
    //     //        - ProjGroupRemoveProjectile()       - ForProjGroup()
    //     //
    //     //        - IsProjectileInProjGroup()         - ForProjGroupEx()
    //     //
    //     //        - CountProjectilesInProjGroup()     - DestroyProjGroup()
    //     //
    //     //        - FirstOfProjGroup()                - GetEnumProjectile()
    //     //
    //     //        - ProjGroupClear()                  - GetParentProjGroup()
    //     //
    //     //        - GroupEnumProjectilesInRange()     - GetForProjGroupData()  
    //     //
    //     //------------------------------------------------------------------------------------------------------------------------\\
    //     //
    //     //------------------------------------------------------------------------------------------------------------------------\\
    //     //
    //     //  Detailed ProjGroup API descriptions:
    //     // _____________________________________
    //     //    Methods:
    //     //   _________
    //     //      • projgroup.create() -> returns projgroup
    //     //      or
    //     //      • CreateProjGroup() -> returns projgroup
    //     //
    //     //        - The above method enables users to create a projgroup, which is basically a group for projectiles (similar to
    //     //          WC3 native groups for units). This method will returns a projgroup to be saved into a variable.
    //     //        - The alternative WC3 naming for this requires no parameters as well.
    //     //
    //     //      • this.add(projectile p) -> returns boolean
    //     //      or
    //     //      • ProjGroupAddProjectile(projgroup g, projectile p) -> returns boolean
    //     //
    //     //        - The above method adds a projectile to a projgroup (similar to GroupAddUnit()). This method will return true
    //     //          if the projectile was successfully added and false if it wasn't.
    //     //        - The alternative WC3 naming for this requires a projgroup parameter before the projectile parameter.
    //     //
    //     //      • this.remove(projectile p) -> returns boolean
    //     //      or
    //     //      • ProjGroupRemoveProjectile(projgroup g, projectile p) -> returns boolean
    //     //
    //     //        - This method removes a projectile from a projgroup (similar to GroupRemoveUnit()). This method will return true
    //     //          if the projectile was successfully removed and false if it wasn't.
    //     //        - The alternative WC3 naming for this requires a projgroup parameter before the projectile parameter.
    //     //
    //     //      • this.isInGroup(projectile p) -> returns boolean
    //     //      or
    //     //      • IsProjectileInProjGroup(projgroup g, projectile p) -> returns boolean
    //     //
    //     //        - This method checks to see if a specified projectile is in a projgroup (similar to IsUnitInGroup()). This method
    //     //          will return true if the projectile is in the group and false if it isn't.
    //     //        - The alternative WC3 naming for this requires a projgroup parameter before the projectile parameter.
    //     //
    //     //      • this.getCount() -> returns integer
    //     //      or
    //     //      • CountProjectilesInProjGroup(projgroup g) -> returns integer
    //     //
    //     //        - This method will return an integer that represents the number of projectiles currently in a group (similar to
    //     //          CountUnitsInGroup()).
    //     //        - The alternative WC3 naming for this requires a projgroup parameter.
    //     //
    //     //      • this.getFirst() -> returns projectile
    //     //      or
    //     //      • FirstOfProjGroup(projgroup g) -> returns projectile
    //     //
    //     //        - This method attempts to simulate the WC3 native FirstOfGroup(). It will return a projectile that is at the
    //     //          start of the internal projectile array of a projgroup. Useful for first-of-group-loops.
    //     //        - The alternative WC3 naming for this requires a projgroup parameter.
    //     //
    //     //      • this.clear() -> returns nothing
    //     //      or
    //     //      • ProjGroupClear(projgroup g) -> returns nothing
    //     //
    //     //        - This method will clear all projectiles from a projgroup, emptying it of all data completely (similar to
    //     //          GroupClear()). It returns nothing.
    //     //        - The alternative WC3 naming for this requires a projgroup parameter.
    //     //
    //     //      • this.enumNearby(real x, real y, real z, real radius) -> returns nothing
    //     //      or
    //     //      • GroupEnumProjectilesInRange(projgroup g, real x, real y, real z, real radius) -> returns nothing
    //     //
    //     //        - This method will group all projectiles in a specified radius around given coordinates and add them to a
    //     //          projgroup. Before adding projectiles to the projgroup it will clear to projgroup of all existing projectiles,
    //     //          to better simulate the WC3 GroupEnumUnitsInRange(). It takes four parameters, the first three being the x, y and
    //     //          z coordinates around which you want projectiles to be added, and the fourth being the radius in which
    //     //          projectiles need to be to be added to the projgroup.
    //     //        - The alternative WC3 naming for this requires a projgroup parameter before other parameters.
    //     //
    //     //      • this.forGroup(enumFunc e) -> returns nothing
    //     //      or
    //     //      • ForProjGroup(projgroup g, enumFunc e) -> returns nothing
    //     //
    //     //        - This method will perform a function for every projectile in a projgroup. It has one parameter, which is the
    //     //          function that will be executed for all projectiles, it is similar to the function interface members for the
    //     //          projectile struct. The function that is to be passed through this method as an parameter must take nothing and
    //     //          return nothing. For example:
    //     //
    //     //            private function EnumFunctionExample takes nothing and returns nothing
    //     //                call EnumProjectile.terminate()
    //     //            endfunction
    //     //
    //     //        - As you can see above the function takes nothing and returns nothing, and inside this function users can use
    //     //          the EnumProjectile global to access the current projectile the function is being executed for.
    //     //        - The alternative WC3 naming for this requires a projgroup parameter before the enumFunc parameter.
    //     //
    //     //      • this.forGroupEx(enumFunc e, integer i) -> returns nothing
    //     //      or
    //     //      • ForProjGroupEx(projgroup g, enumFunc e, integer i) -> returns nothing
    //     //
    //     //        - This method works in the exact same way as the above-mentioned .forGroup() method, except that it takes one
    //     //          more parameter. This last parameter (that comes after the enumFunc parameter) refers to any temporary data
    //     //          that a user wants to access in the enumFunc provided by the user. It also provides access to the parent
    //     //          projgroup that the method was used for. For example:
    //     //
    //     //            private function EnumFunctionExample takes nothing and returns nothing
    //     //                call Data(ForProjGroupData).destroy() <-- Needs to be typecasted, as with some timer data systems.
    //     //                call EnumProjectile.terminate()
    //     //                call ParentProjGroup.destroy()
    //     //            endfunction
    //     //
    //     //        - As you can see above the function takes nothing and returns nothing, and inside this function users can
    //     //          access not only the projectile the function is being executed for, but also any temporary data and the
    //     //          parent projgroup that the .forGroupEx() method was called for.
    //     //        - The alternative WC3 naming for this requires a projgroup parameter before the other parameters.
    //     //
    //     //      • this.forNearby(real x, real y, real z, real radius, integer i, enumFunc e)
    //     //      or
    //     //      • ForNearbyProjectilesInRange(projgroup p, real x, real y, real z, real r, integer i, enumFunc e)
    //     //
    //     //        - The above method works similarly to .enumNearby(), however instead of just adding the projectiles to the group
    //     //          it will also execute the enumFunc for each projectile added. Think of it as a mix between .enumNearby() and
    //     //          .forGroupEx(). This method also takes an integer parameter before the enumFunc that can be set to temporary
    //     //          data for use.
    //     //        - The alternative WC3 naming for this requires a projgroup parameter before the other parameters.
    //     //
    //     //      • this.forNearbyEx(real x, real y, real z, real radius, integer i, enumFunc e1, enumFunc e2)
    //     //      or 
    //     //      • ForNearbyProjectilesInRangeEx(projgroup p, real x, real y, real z, real r, integer i, enumFunc e1, enumFunc e2)
    //     //
    //     //        - The above method works in the same way as .forNearby(), however it has an extra enumFunc parameter that will
    //     //          be executed when any projectiles that are still in the projgroup are removed from the group before the
    //     //          enumerating begins. The first enumFunc parameter is for when the projectiles get removed, while the second
    //     //          enumFunc parameter is for when a projectile is added to the projgroup. This method can also save and access
    //     //          temporary data and parent projgroups for .forGroupEx().
    //     //        - The alternative WC3 naming for this requires a projgroup parameter before the other parameters.
    //     //
    //     //      • this.destroy() -> returns nothing
    //     //      or
    //     //      • DestroyProjGroup(projgroup g) -> returns nothing
    //     //
    //     //        - This method destroys a projgroup, clearing it of all data (similar to DestroyGroup()). This method returns
    //     //          nothing.
    //     //        - The alternative WC3 naming for this requires a projgroup parameter.
    //     //
    //     //    Globals:
    //     //   _________
    //     //      • EnumProjectile -> returns projectile
    //     //      or
    //     //      • GetEnumProjectile() -> returns projectile
    //     //
    //     //        - This global returns the most recent projectile a function from .forGroup() has been called for. It is to be
    //     //          used like GetEnumUnit() in a normal ForGroup() call.
    //     //        - The alternative WC3 naming for this is a function not a global.
    //     //
    //     //      • ParentProjGroup -> returns projectile
    //     //      or
    //     //      • GetParentProjGroup() -> returns projectile
    //     //
    //     //        - This global returns the most recent projgroup taht .forGroupEx() has been called for. It could come in handy
    //     //          for some situations.
    //     //        - The alternative WC3 naming for this is a function not a global.
    //     //
    //     //      • ForProjGroupData -> returns projectile
    //     //      or
    //     //      • GetForProjGroupData() -> returns projectile
    //     //
    //     //        - This global returns the most recent temporary data that a user wanted to access in the .forGroupEx() method.
    //     //          This can be used to access extra data needed for the function.
    //     //        - The alternative WC3 naming for this is a function not a global.
    //     //
    //     //      • GlobalProjGroup -> returns projgroup
    //     //      or
    //     //      • GetGlobalProjGroup() -> returns projgroup
    //     //
    //     //        - This global is to be used like the temporary group from GroupUtils. It saves the user from creating and 
    //     //          destroying a projgroup if they need to use one .enumNearby() and .forGroup() call.
    //     //        - The alternative WC3 naming for this is a function not a global.
    //     //
    //     //------------------------------------------------------------------------------------------------------------------------\\
    //     //
    //     //------------------------------------------------------------------------------------------------------------------------\\
    //     //
    //     //
    //     //
    //     //  DO NOT TOUCH PAST THIS POINT UNLESS YOU KNOW WHAT YOUR ARE DOING!!!
    //     //                           
    //     globals    
    //         projgroup  GlobalProjGroup  = 0           // Global temporary projgroup for enums.
    //         projgroup  ParentProjGroup  = 0           // Parent projgroup that used .forGroupEx() or .forNearbyEx().
    //         projectile EnumProjectile   = 0           // Used in .forGroup() and .forNearby() methods for projgroups.
    //         integer    ForProjGroupData = 0           // Temporary data that can be used in .forGroupEx() or .forNearbyEx().
    //     endglobals

    //     native UnitAlive takes unit whichUnit returns boolean

    //     globals
    //         private projectile ProjectileList = 0     // Linked list to keep track of projectiles.
    //         private integer    RecycledCount  = 0     // Counter for unit recycling.
    //         private real       WorldMaxX      = 0.00  // Map boundaries for internal BoundSentinel.
    //         private real       WorldMaxY      = 0.00  // Map boundaries for internal BoundSentinel.
    //         private real       WorldMinX      = 0.00  // Map boundaries for internal BoundSentinel.
    //         private real       WorldMinY      = 0.00  // Map boundaries for internal BoundSentinel.
    //         private hashtable  StorageOne     = null  // Hashtable used for trigger and projectile storage.
    //         private hashtable  StorageTwo     = null  // Hashtable needed for removing projectiles from projgroups.
    //         private unit array RecycledUnits          // Array to keep track of recycled units.
    //     endglobals

    //     //------------------------------------------------------------------------------------------------------------------------\\
    //     //  Temporary globals: Needed for the periodic method.
    //     //
    //     globals
    //         private real     TempX = 0.00             // Temporary x coordinate for targets and distances.
    //         private real     TempY = 0.00             // Temporary y coordinate for targets and distances.
    //         private real     TempZ = 0.00             // Temporary z coordinate z heights of xy coordinates.
    //         private real     TempD = 0.00             // Temporary real for distance calculations.
    //         private real     TempP = 0.00             // Temporary real for animation calculations.
    //         private location TempL = null             // Temporary location for z heights of xy coordinates.
    //     endglobals

    //     //------------------------------------------------------------------------------------------------------------------------\\
    //     //  Projectile counter: Easier way to find out how many active projectiles there are.
    //     //
    //     function GetTotalProjectiles takes nothing returns integer
    //         return ProjectileList.size
    //     endfunction

    //     //------------------------------------------------------------------------------------------------------------------------\\
    //     //  Function interfaces: Bonus functions available for greater control.
    //     //
    //     private function interface EnumEvents takes nothing returns nothing
    //     private function interface UserEvents takes projectile instance returns nothing
    //     private function interface UnitImpact takes projectile instance, unit whichUnit returns nothing
    //     private function interface DestImpact takes projectile instance, destructable whichDest returns nothing

    //     //------------------------------------------------------------------------------------------------------------------------\\
    //     //
    //     private module ProjOperators



    //         //------------------------------------------------------------\\
    //         //  Projectile termination boolean.
    //         method operator isTerminated takes nothing returns boolean
    //             return this.stop
    //         endmethod

    //         //------------------------------------------------------------\\
    //         //  Projectile static method operator.
    //         static method operator[] takes unit whichUnit returns thistype
    //             return ProjData[whichUnit].data
    //         endmethod

    //     endmodule

    //     //------------------------------------------------------------------------------------------------------------------------\\
    //     //  ProjMethods: Bonus methods for users. Easier for me to maintain.
    //     //
    //     private module ProjMethods

    //         //------------------------------------------------------------\\
    //         //  Set target coordinates of projectile.
    //         method setTargPoint takes real xPos, real yPos, real zPos, boolean wantUpdate returns nothing
    //             set this.target  = null
    //             set this.arcSize = 0.00
    //             set this.tarX    = xPos
    //             set this.tarY    = yPos
    //             call MoveLocation(TempL,this.tarX,this.tarY)
    //             set this.tarZ    = zPos + GetLocationZ(TempL)
    //             set this.update  = wantUpdate
    //         endmethod

    //         //------------------------------------------------------------\\
    //         //  Set projectile coordinates.
    //         method setProjPoint takes real xPos, real yPos, real zPos, boolean wantUpdate returns nothing
    //             set this.posX   = xPos
    //             set this.posY   = yPos
    //             set this.posZ   = zPos
    //             set this.update = wantUpdate
    //         endmethod

    //         //------------------------------------------------------------\\
    //         //  Disjoint projectile to stop homing and remove target.
    //         method disjoint takes nothing returns nothing
    //             set this.target = null
    //             set this.allowTargetHoming = false
    //         endmethod

    //         //------------------------------------------------------------\\
    //         //  New reset method.
    //         method refresh takes boolean flag returns nothing
    //             call this.stopPeriodic()
    //             set this.launched = false
    //             set this.update = true
    //             if not flag then
    //                 set this.onStart = 0
    //             endif
    //         endmethod

    //         //------------------------------------------------------------\\
    //         //  Terminate projectile instead of destroy.
    //         method terminate takes nothing returns nothing
    //             set this.effectPath = ""
    //             set this.stop = true
    //         endmethod

    //         //------------------------------------------------------------\\
    //         //  Manual damage group methods. Added for completeness.
    //         method addUnit takes unit whichUnit returns nothing
    //             if not IsUnitInGroup(whichUnit,this.dmgGroup) then
    //                 call GroupAddUnit(this.dmgGroup,whichUnit)
    //             endif
    //         endmethod
    //         method isUnitAdded takes unit whichUnit returns boolean
    //             return IsUnitInGroup(whichUnit,this.dmgGroup)
    //         endmethod
    //         method removeUnit takes unit whichUnit returns nothing
    //             if IsUnitInGroup(whichUnit,this.dmgGroup) then
    //                 call GroupRemoveUnit(this.dmgGroup,whichUnit)
    //             endif
    //         endmethod
    //         method removeAll takes nothing returns nothing
    //             call GroupClear(this.dmgGroup)
    //         endmethod


    //     endmodule

    //     //------------------------------------------------------------------------------------------------------------------------\\
    //     //  Projectile: The main struct interface for the system.
    //     //
    //     struct projectile[MAX_PROJECTILES]
    //         implement LinkedList

    //         //------------------------------------------------------------\\

    //         //------------------------------------------------------------\\
    //         //  Static members.
    //         private static rect     destRect = null         // Rect for filtering destructables.
    //         private static group    temGroup = null         // Group needed for unit collision.
    //         private static boolexpr destFilt = null         // The condition used for dest collision.
    //         private static boolexpr unitFilt = null         // The condition used for unit collision.
    //         private static thistype currInst = 0            // Current struct instance for dest collision.

    //         //------------------------------------------------------------\\
    //         //  Implementing required modules.
    //         implement ProjOperators
    //         implement ProjMethods

    //         //------------------------------------------------------------\\
    //         //  Implementing optional modules.
    //         implement optional ProjShadows
    //         implement optional ProjBounces

    //         //------------------------------------------------------------\\
    //         //  Destroy method: Clear all data from projectiles.


    //         //------------------------------------------------------------\\
    //         //  Projectile on destructable collision detection.
    //         private static method destCollisionFilt takes nothing returns boolean
    //             local thistype     this = thistype.currInst
    //             local destructable dest = GetFilterDestructable()
    //             local real         desx = GetDestructableX(dest)
    //             local real         desy = GetDestructableY(dest)

    //             if GetWidgetLife(dest) > 0.405 then
    //                 if (this.posX - desx) * (this.posX - desx) + (this.posY - desy) * (this.posY - desy) <= this.destHitRadius * this.destHitRadius then
    //                     call this.onDest.execute(this,dest)
    //                 endif
    //             endif

    //             set dest = null

    //             return false
    //         endmethod

    //         //------------------------------------------------------------\\
    //         //  Projectile on unit collision detection.
    //         private static method unitCollisionFilt takes nothing returns boolean
    //             local thistype this = thistype.currInst
    //             local unit     filt = GetFilterUnit()

    //             if not IsUnitInGroup(filt,this.dmgGroup) then
    //                 call GroupAddUnit(this.dmgGroup,filt)
    //                 call this.onUnit.execute(this,filt)
    //             endif

    //             set filt = null

    //             return false
    //         endmethod

    //         //------------------------------------------------------------\\

    //         implement T32x

    //         //------------------------------------------------------------\\
    //         //  Common method: Set all common members from project methods.

    //         //------------------------------------------------------------\\
    //         //  Project normal method: Launch without arc.
    //         method projectNormal takes real xPos, real yPos, real zPos, real speed returns boolean
    //             if not this.launched then
    //                 set this.launched = true
    //                 call this.setCommon(xPos,yPos,zPos,speed,0.00)
    //                 call this.onStart.execute(this)
    //                 call this.startPeriodic()
    //                 return true
    //             endif

    //             return false
    //         endmethod

    //         //------------------------------------------------------------\\
    //         //  Project arcing method: Lauch with arc.
    //         method projectArcing takes real xPos, real yPos, real zPos, real speed, real arc returns boolean
    //             if not this.launched then
    //                 set this.launched = true
    //                 call this.setCommon(xPos,yPos,zPos,speed,arc)
    //                 call this.onStart.execute(this)
    //                 call this.startPeriodic()
    //                 return true
    //             endif

    //             return false
    //         endmethod


    //         //------------------------------------------------------------\\
    //         //  OnInit method: Setup static members.
    //         private static method onInit takes nothing returns nothing
    //             set thistype.temGroup = CreateGroup()
    //             set thistype.destRect = Rect(0.00,0.00,0.00,0.00)
    //             set thistype.destFilt = Condition(function thistype.destCollisionFilt)
    //             set thistype.unitFilt = Condition(function thistype.unitCollisionFilt)
    //         endmethod

    //     endstruct

    //     //------------------------------------------------------------------------------------------------------------------------\\
    //     //  ForGroupStack: For recursion safety for projectile groups.
    //     //
    //     private struct ForGroupStack extends array

    //         public integer   callback
    //         public integer   tempData
    //         public projgroup whichGrp

    //         public static thistype top = 0

    //         static method increment takes nothing returns nothing
    //             set thistype.top = thistype(thistype.top + 1)
    //         endmethod

    //         static method decrement takes nothing returns nothing
    //             set thistype.top = thistype(thistype.top - 1)
    //         endmethod

    //     endstruct

    //     //------------------------------------------------------------------------------------------------------------------------\\
    //     //  Projectile groups: More advanced and all-encompassing grouping system.
    //     //
    //     private function ForProjGroupExCallback takes nothing returns nothing
    //         set EnumProjectile   = ProjData[GetEnumUnit()].data
    //         set ParentProjGroup  = ForGroupStack.top.whichGrp
    //         set ForProjGroupData = ForGroupStack.top.tempData
    //         call EnumEvents(ForGroupStack.top.callback).execute()
    //     endfunction

    //     private function ForProjGroupCallback takes nothing returns nothing
    //         set EnumProjectile = ProjData[GetEnumUnit()].data
    //         call EnumEvents(ForGroupStack.top.callback).execute()
    //     endfunction

    //     struct projgroup[MAX_PROJGROUPS]

    //         private integer max = 0
    //         private group   grp

    //         //------------------------------------------------------------\\
    //         //  Destroy method: Clear all data and destroy the struct.
    //         method destroy takes nothing returns nothing
    //             call this.clear()

    //             static if LIBRARY_Recycle then
    //                 call Group.release(this.grp)
    //                 set this.grp = null
    //             elseif LIBRARY_GroupUtils then
    //                 call ReleaseGroup(this.grp)
    //                 set this.grp = null
    //             else
    //                 call GroupClear(this.grp)
    //             endif

    //             call this.deallocate()
    //         endmethod

    //         //------------------------------------------------------------\\
    //         //  ForGroupEx method: Execute function for all projectiles.
    //         method forGroupEx takes EnumEvents whichFunc, integer whichData returns nothing
    //             call ForGroupStack.increment()
    //             set ForGroupStack.top.callback = whichFunc
    //             set ForGroupStack.top.tempData = whichData
    //             set ForGroupStack.top.whichGrp = this
    //             call ForGroup(this.grp,function ForProjGroupExCallback)
    //             call ForGroupStack.decrement()
    //         endmethod

    //         //------------------------------------------------------------\\
    //         //  ForGroup method: Execute function for all projectiles.
    //         method forGroup takes EnumEvents whichFunc returns nothing
    //             call ForGroupStack.increment()
    //             set ForGroupStack.top.callback = whichFunc
    //             call ForGroup(this.grp,function ForProjGroupCallback)
    //             call ForGroupStack.decrement()
    //         endmethod

    //         //------------------------------------------------------------\\
    //         //  Remove method: Remove a projectile from a projgroup.
    //         method remove takes projectile whichProj returns boolean 
    //             local integer i = 0
    //             local integer j = 0

    //             if whichProj != 0 and IsUnitInGroup(whichProj.dummy,this.grp) then
    //                 call GroupRemoveUnit(this.grp,whichProj.dummy)
    //                 set this.max = this.max - 1

    //                 set i = LoadInteger(StorageOne,integer(whichProj),-1) - 1
    //                 set j = LoadInteger(StorageTwo,integer(whichProj),this)
    //                 call SaveInteger(StorageOne,integer(whichProj),-1,i)
    //                 call SaveInteger(StorageOne,integer(whichProj),j,LoadInteger(StorageOne,integer(whichProj),i))
    //                 call SaveInteger(StorageTwo,integer(whichProj),LoadInteger(StorageOne,integer(whichProj),j),j)

    //                 if i < 1 then
    //                     call FlushChildHashtable(StorageOne,integer(whichProj))
    //                     call FlushChildHashtable(StorageTwo,integer(whichProj))
    //                 endif

    //                 return true
    //             endif

    //             return false
    //         endmethod

    //         //------------------------------------------------------------\\
    //         //  Add method: Add a projectile to a projgroup.
    //         method add takes projectile whichProj returns boolean
    //             local integer i = 0

    //             if whichProj != 0 and not IsUnitInGroup(whichProj.dummy,this.grp) then
    //                 call GroupAddUnit(this.grp,whichProj.dummy)
    //                 set this.max = this.max + 1

    //                 if HaveSavedInteger(StorageOne,integer(whichProj),-1) then
    //                     set i = LoadInteger(StorageOne,integer(whichProj),-1)
    //                 endif

    //                 call SaveInteger(StorageOne,integer(whichProj),i,this)
    //                 call SaveInteger(StorageTwo,integer(whichProj),this,i)
    //                 call SaveInteger(StorageOne,integer(whichProj),-1,i + 1)

    //                 return true
    //             endif

    //             return false
    //         endmethod

    //         //------------------------------------------------------------\\
    //         //  IsInGroup method: Check if a projectile is in a projgroup.
    //         method isInGroup takes projectile whichProj returns boolean
    //             return IsUnitInGroup(whichProj.dummy,this.grp)
    //         endmethod

    //         //------------------------------------------------------------\\
    //         //  Extended for nearby Method: Execute functions and grouping.
    //         method forNearbyEx takes real x, real y, real z, real r, integer i, EnumEvents e1, EnumEvents e2 returns nothing
    //             local projectile p = ProjectileList.head

    //             loop
    //                 exitwhen p == 0
    //                 if this.isInGroup(p) then
    //                     call this.remove(p)
    //                     if e1 != 0 then
    //                         set EnumProjectile   = p
    //                         set ParentProjGroup  = this
    //                         set ForProjGroupData = i
    //                         call e1.execute()
    //                     endif
    //                 endif
    //                 if r * r > ((p.posX - x) * (p.posX - x) + (p.posY - y) * (p.posY - y) + (p.posZ - z) * (p.posZ - z)) then
    //                     call this.add(p)
    //                     if e2 != 0 then
    //                         set EnumProjectile   = p
    //                         set ParentProjGroup  = this
    //                         set ForProjGroupData = i
    //                         call e2.execute()
    //                     endif
    //                 endif
    //                 set p = p.next
    //             endloop
    //         endmethod

    //         //------------------------------------------------------------\\
    //         //  For nearby Method: Execute functions and grouping.
    //         method forNearby takes real x, real y, real z, real r, integer i, EnumEvents e returns nothing
    //             local projectile p = ProjectileList.head

    //             call this.clear()

    //             loop
    //                 exitwhen p == 0
    //                 if r * r > ((p.posX - x) * (p.posX - x) + (p.posY - y) * (p.posY - y) + (p.posZ - z) * (p.posZ - z)) then
    //                     call this.add(p)
    //                     if e != 0 then
    //                         set EnumProjectile   = p
    //                         set ParentProjGroup  = this
    //                         set ForProjGroupData = i
    //                         call e.execute()
    //                     endif
    //                 endif
    //                 set p = p.next
    //             endloop
    //         endmethod

    //         //------------------------------------------------------------\\
    //         //  Enum method: Group all nearby projectiles.
    //         method enumNearby takes real x, real y, real z, real r returns nothing
    //             local projectile p = ProjectileList.head

    //             call this.clear()

    //             loop
    //                 exitwhen p == 0
    //                 if r * r > ((p.posX - x) * (p.posX - x) + (p.posY - y) * (p.posY - y) + (p.posZ - z) * (p.posZ - z)) then
    //                     call this.add(p)
    //                 endif
    //                 set p = p.next
    //             endloop
    //         endmethod

    //         //------------------------------------------------------------\\
    //         //  Clear method: Clear all data from a projgroup.
    //         method clear takes nothing returns nothing
    //             local unit u = null

    //             loop
    //                 set u = FirstOfGroup(this.grp)
    //                 exitwhen u == null
    //                 call this.remove(ProjData[u].data)
    //             endloop

    //             call GroupClear(this.grp)
    //             set this.max = 0

    //             set u = null
    //         endmethod

    //         //------------------------------------------------------------\\
    //         //  First of group method: Retrieve first projectile.
    //         method getFirst takes nothing returns projectile
    //             return ProjData[FirstOfGroup(this.grp)].data
    //         endmethod

    //         //------------------------------------------------------------\\
    //         //  Count method: Retrieve number of projectiles in projgroup.
    //         method getCount takes nothing returns integer
    //             return this.max
    //         endmethod

    //         //------------------------------------------------------------\\
    //         //  Projgroup creation.
    //         static method create takes nothing returns thistype
    //             local thistype this = thistype.allocate()

    //             static if LIBRARY_Recycle then
    //                 set this.grp = Group.get()
    //             elseif LIBRARY_GroupUtils then
    //                 set this.grp = NewGroup()
    //             else
    //                 if this.grp == null then
    //                     set this.grp = CreateGroup()
    //                 endif
    //             endif

    //             return this
    //         endmethod

    //     endstruct

}
