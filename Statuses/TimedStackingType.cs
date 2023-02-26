// using System;
// using System.Collections.Generic;
// using System.Text;
// using static War3Api.Common;
// using static War3Api.Blizzard;
// using NoxRaven.Units;

// namespace NoxRaven.Statuses
// {
//     public class TimedStackingType : TimedType
//     {
        
//         public TimedStackingType(StatusFunction apply, StatusFunction reset, StatusFunction onRemove, string specialEffectPath, string specialEffectAttachmentPoint) : base(apply, reset, onRemove, specialEffectPath, specialEffectAttachmentPoint)
//         {
//         }

//         public Status ApplyStatus(NUnit source, NUnit target, int level, float duration, int initialStacks = 1, int bonusLevel = 0, float bonusDuration = 0, int bonusStacks = 1)
//         {
//             if (!target.ContainsStatus(Id))
//                 // create new status and add it to unit
//                 return target.AddStatus(Id, new Status(Id, this, source, target, level, duration));
//             return target.GetStatus(Id).Reapply(bonusDuration, bonusLevel, bonusStacks);
//         }

//         [Obsolete]
// #pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
//         private new Status ApplyStatus(NUnit source, NUnit target, int level, float duration, int bonusLevel = 0, float bonusDuration = 0)
// #pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
//         {
//             if (!target.ContainsStatus(Id))
//                 // create new status and add it to unit
//                 return target.AddStatus(Id, new Status(Id, this, source, target, level, duration));
//             return target.GetStatus(Id).Reapply(bonusDuration, bonusLevel, 0);
//         }

//     }
// }