﻿namespace NamelessRogue.Engine.Components.WorldBoardComponents.Combat
{


    public enum MovementType
    {
        Ground,
        Flying,
        Naval
    }

    public enum UnitAttackType
    {
        Melee,
        Ranged
    }


    public class Unit : Components.Component
    {
        private int _currentHp;

        public int MaxNumberOfTroops { get; set; }
        public int NumberOfTroops { get; set; }

        public int AttackPower { get; set; }
        public int Defence { get; set; }

        public int SingleTrooperHp { get; set; }

        public int AttackRange { get; set; }

        public int MovementSpeed { get; set; }

        public int SupplyCostPerTurn { get; set; }

        public int ManaCostPerTurn { get; set; }

        public int MaxHp
        {
            get { return MaxNumberOfTroops * SingleTrooperHp; }
        }

        public int CurrentHp
        {
            get => _currentHp;
            set { _currentHp = value;
                NumberOfTroops = _currentHp / SingleTrooperHp;
            }
        }



        public MovementType MovementType { get; set; }
        public UnitAttackType AttackType { get; set; }

        public override IComponent Clone()
        {
            return new Unit()
            {
                MovementType = MovementType,
                SingleTrooperHp = SingleTrooperHp,
                NumberOfTroops = NumberOfTroops,
                AttackType = AttackType,
                Defence = Defence,
                AttackPower = AttackPower,
                AttackRange = AttackRange,
                CurrentHp = CurrentHp,
                ManaCostPerTurn = ManaCostPerTurn,
                MaxNumberOfTroops = MaxNumberOfTroops,
                MovementSpeed = MovementSpeed,
                SupplyCostPerTurn = SupplyCostPerTurn
            };
        }
    }
}
