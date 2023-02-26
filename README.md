# NoxRaven
C# primitives collection for map building.<br />
No license, use as you please.<br />
Credits for contribution, these are the terms!<br /><br /><br />

## How to use
```csharp
// Add 6 human players who are playing right now to global number of playing players
for (int i = 0; i < 6; i++)
    if (IsPlayerSlotState(Player(i), PLAYER_SLOT_STATE_PLAYING) && GetPlayerController(Player(i)) == MAP_CONTROL_USER)
        Globals.Players.Add(new SurvivorPlayer(i));
// Add enemy force and friendly force
Globals.UndeadForce = new EnemyForce(20);
Globals.LastLiving = new NoxPlayer(6);
// Unit Indexing after players known
// Register any custom Type indexes
// Dont forget to add it to all others!
NUnit.AddCustomType(FourCC("H000"), typeof(Marine)); // Makes unit "H000" hit public Marine(unit u) function
NUnit.AddCustomType(FourCC("n00A"), typeof(EnemyArmyUnit));
NUnit.AddCustomType(FourCC("u000"), typeof(EnemyArmyUnit));
NUnit.AddCustomType(FourCC("n00B"), typeof(EnemyArmyUnit));
// Custom items
// When picking up "I003" unit will recieve 10 base damage, when drop/sell/pawn loose 10 base damage
// the magic is that AddBaseDamage hits bottom inheritance, so Marine from example will get +15 and loose -15 on pickup/drop
NoxItem.RegisterItem(FourCC("I003"), (target) =>
{
    target.AddBaseDamage(10);
}, (target) =>
{
    target.AddBaseDamage(-10);
}, null);
// Once units and items are assigned run this
// Run this after all static data init.
Master.RunAfterExtensionsReady();

// More of your script (do anything you want here)

// This makes sure map was loaded correctly
Master.RunAtEndOfMain();
```
Marine.cs
```csharp
public class Marine : PlayerHero
    {
      public Marine(unit u) : base(u)
      {
      // will be hit when unit created or was present in the map when map started
      }

      public override void AddBaseDamage(int val)
      {
          base.AddBaseDamage(R2I(val*1.5f));
      }
    }
```
Make sure you grab this basemap.
http://media.wolfrealm.org/NoxRavenBasemap.w3x.zip <br />
Otherwise some stuff might not work (like all unit modifications - agi, str, green armour, green damage, red armour...)

Thx to Drake53 for C# Compiler.<br />
