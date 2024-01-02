﻿using RogueSharp.Random;
using System;

namespace NamelessRogue.Engine.Utility
{
    public class InternalRandom : IRandom
    {
        private int _seed;
        private long _numberGenerated;
        private Random _random;

        /// <summary>
        /// Constructs a new pseudo-random number generator 
        /// with a seed based on the number of milliseconds elapsed since the system started
        /// </summary>
        public InternalRandom()
           : this(Environment.TickCount)
        {
        }

        /// <summary>
        /// Constructs a new pseudo-random number generator with the specified seed
        /// </summary>
        /// <param name="seed">An integer used to calculate a starting value for the pseudo-random number sequence</param>
        public InternalRandom(int seed)
        {
            _seed = seed;
            _random = new Random(_seed);
        }

        /// <summary>
        /// Gets the next pseudo-random integer between 0 and the specified maxValue inclusive
        /// </summary>
        /// <param name="maxValue">Non-inclusive maximum result</param>
        /// <returns>Returns a pseudo-random integer between 0 and int.MaxValue</returns>
        public int Next()
        {
            return Next(0, int.MaxValue);
        }


        /// <summary>
        /// Gets the next pseudo-random integer between 0 and the specified maxValue inclusive
        /// </summary>
        /// <param name="maxValue">Non-inclusive maximum result</param>
        /// <returns>Returns a pseudo-random integer between 0 and the specified maxValue inclusive</returns>
        public int Next(int maxValue)
        {
            return Next(0, maxValue);
        }

        /// <summary>
        /// Gets the next pseudo-random integer between the specified minValue and maxValue inclusive
        /// </summary>
        /// <param name="minValue">Inclusive minimum result</param>
        /// <param name="maxValue">Non-inclusive maximum result</param>
        /// <returns>Returns a pseudo-random integer between the specified minValue and maxValue inclusive</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if maxValue equals Int32.MaximumValue</exception>
        public int Next(int minValue, int maxValue)
        {
            _numberGenerated++;
            return _random.Next(minValue, maxValue);
        }

        /// <summary>
        /// Saves the current state of the pseudo-random number generator
        /// </summary>
        /// <example>
        /// If you generated three random numbers and then called Save to store the state and 
        /// followed that up by generating 10 more numbers before calling Restore with the previously saved RandomState
        /// the Restore method should return the generator back to the state when Save was first called.
        /// This means that if you went on to generate 10 more numbers they would be the same 10 numbers that were
        /// generated the first time after Save was called.
        /// </example>
        /// <returns>A RandomState class representing the current state of this pseudo-random number generator</returns>
        public RandomState Save()
        {
            return new RandomState
            {
                NumberGenerated = _numberGenerated,
                Seed = new[]
               {
               _seed
            }
            };
        }

        /// <summary>
        /// Restores the state of the pseudo-random number generator based on the specified state parameter
        /// </summary>
        /// <example>
        /// If you generated three random numbers and then called Save to store the state and 
        /// followed that up by generating 10 more numbers before calling Restore with the previously saved RandomState
        /// the Restore method should return the generator back to the state when Save was first called.
        /// This means that if you went on to generate 10 more numbers they would be the same 10 numbers that were
        /// generated the first time after Save was called.
        /// </example>
        /// <param name="state">The state to restore to, usually obtained from calling the Save method</param>
        /// <exception cref="ArgumentNullException">RandomState cannot be null</exception>
        public void Restore(RandomState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state), "RandomState cannot be null");
            }

            _seed = state.Seed[0];
            _random = new Random(_seed);
            for (long i = 0; i < state.NumberGenerated; i++)
            {
                _random.Next();
            }
        }
    }
}
