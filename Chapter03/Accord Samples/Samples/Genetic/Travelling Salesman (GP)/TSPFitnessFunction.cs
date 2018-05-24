// AForge Framework
// Traveling Salesman Problem using Genetic Algorithms
//
// Copyright © Andrew Kirillov, 2006-2008
// andrew.kirillov@gmail.com
//

using System;
using Accord.Genetic;

namespace SampleApp
{
    using ReflectSoftware.Insight;

    /// <summary>
    /// Fitness function for TSP task (Travaling Salasman Problem)
    /// </summary>
    public class TSPFitnessFunction : IFitnessFunction
    {
        // map
        private double[,] map = null;

        // Constructor
        public TSPFitnessFunction(double[,] map)
        {
            this.map = map;
        }

        /// <summary>
        /// Evaluate chromosome - calculates its fitness value
        /// </summary>
        public double Evaluate(IChromosome chromosome)
        {
            return 1 / (PathLength(chromosome) + 1);
        }

        /// <summary>
        /// Translate genotype to phenotype 
        /// </summary>
        public object Translate(IChromosome chromosome)
        {
            return chromosome.ToString();
        }

        /// <summary>
        /// Calculate path length represented by the specified chromosome 
        /// </summary>
        private double PathLength(IChromosome chromosome)
        {
            double pathLength = 0.0D;

            // salesman path
            RILogManager.Default.SendTrace("Calculating Salesman's Path");
            ushort[] path = ((PermutationChromosome)chromosome)?.Value;

            // check path size
            if (path != null && (map != null && path.Length != map.GetLength(0)))
            {
                throw new ArgumentException("Invalid path specified - not all houses are visited");
            }

            // path length
            if (path == null)
            {
                return pathLength;
            }

            int prevCity = path[0];
            int currCity = path[path.Length - 1];

            // calculate distance between the last and the first city
            RILogManager.Default.SendTrace("Calculating distance between cities");


            double dx = map[currCity, 0] - map[prevCity, 0];
            double dy = map[currCity, 1] - map[prevCity, 1];
            pathLength = Math.Sqrt(dx * dx + dy * dy);
            if (map == null)
            {
                return pathLength;
            }

            // calculate the path length from the first city to the last
            RILogManager.Default.SendTrace("Calculating city path");
            for (int i = 1, n = path.Length; i < n; i++)
            {
                currCity = path[i];

                // calculate distance
                dx = map[currCity, 0] - map[prevCity, 0];
                dy = map[currCity, 1] - map[prevCity, 1];
                pathLength += Math.Sqrt(dx * dx + dy * dy);
                prevCity = currCity;
            }
            return pathLength;
        }
    }
}
