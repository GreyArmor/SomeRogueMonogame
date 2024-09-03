using NamelessRogue.Engine.Components.Physical;

namespace NamelessRogue.Engine.Components.WorldBoardComponents.Combat
{
    public static class CombatCalculator
    {

        public enum FightSituation
        {
             RightFight, LeftFight, BothFight = RightFight & LeftFight,
        }

        public static void Fight(Unit right, Unit left, Position rightPosition, Position leftPosition)
        {

            int distance = (int)(rightPosition.Point - leftPosition.Point).Length();

            var attacks = WhoIsAttacking(right, left, distance);

            var damageRight = right.AttackPower - left.Defence;
            var damageLeft = left.AttackPower - right.Defence;


            if (damageRight < 0)
            {
                damageRight = 0;
            }

            if (damageLeft < 0)
            {
                damageLeft = 0;
            }

            if (attacks == FightSituation.RightFight)
            {
                right.CurrentHp = right.CurrentHp - damageLeft;
            }

            if (attacks == FightSituation.LeftFight)
            {
                left.CurrentHp = left.CurrentHp - damageRight;
            }


        }

        public static FightSituation WhoIsAttacking(Unit right, Unit left, int distance)
        {
            var rightAttacksLeft = CanAttack(right, left, distance);
            var leftAttacksRight = CanAttack(left, right, distance);


            if (rightAttacksLeft && leftAttacksRight)
            {
                return FightSituation.BothFight;
            }
            else if (rightAttacksLeft)
            {
                return FightSituation.RightFight;
            }
            else
            {
                return FightSituation.LeftFight;
            }
        }

        /// <summary>
        /// determines if right can attack left
        /// </summary>
        /// <param name="right"></param>
        /// <param name="left"></param>
        /// <returns></returns>
        private static bool CanAttack(Unit right, Unit left, int distance)
        {
            if (right.MovementType == MovementType.Flying)
            {
                if (distance <= right.AttackRange)
                {
                    return true;
                }
            }

            if (right.MovementType == left.MovementType){
                if (distance <= right.AttackRange)
                {
                    return true;
                }
            }

            if (left.MovementType == MovementType.Flying)
            {
                if (right.AttackType == UnitAttackType.Ranged)
                {
                    if (distance <= right.AttackRange)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
