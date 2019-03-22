using Shipwreck.Phash;
using Shipwreck.Phash.Bitmaps;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace GeneticImageGenerator
{
    class Program
    {
        private const double MUTATION_RATE = 0.05;
        private const int POPULATION_SIZE = 50;

        public static Dna[] Population { get; set; }
        public static List<Dna> MatingPool { get; set; }
        public static Bitmap Goal { get; set; }

        static void Main(string[] args)
        {
            // Initialize global elements
            Population = new Dna[POPULATION_SIZE];
            MatingPool = new List<Dna>();

            // Read from goal file
            var path = Path.Combine("Images", "slow.bmp");
            Goal = (Bitmap)Image.FromFile(path);

            // --- STEP 1: Initialize Population ---
            // Initialize the population with random elements
            for (int i = 0; i < POPULATION_SIZE; i++)
            {
                Population[i] = new Dna(Goal.Width, Goal.Height, generateRandomImage: true);
            }

            int generation = 0;
            while (true)
            {
                Console.WriteLine($"--- Generation {++generation} ---");

                // --- STEP 2: Selection ---
                // 2a. Calculate the fitness for each element in population
                double bestFitnessInGeneration = 0.0;
                Bitmap bestImageInGeneration = default(Bitmap);
                for (int i = 0; i < POPULATION_SIZE; i++)
                {
                    Population[i].CalculateFitness(Goal);
                    if (Population[i].Fitness > bestFitnessInGeneration)
                    {
                        bestFitnessInGeneration = Population[i].Fitness;
                        bestImageInGeneration = Population[i].Genes;
                    }
                }
                Console.WriteLine($"\tBest Fitness: {bestFitnessInGeneration}");
                if (bestFitnessInGeneration > 0.3)
                {
                    var bestImagePath = Path.Combine("Results", $"Generation-{generation}.bmp");
                    bestImageInGeneration.Save(bestImagePath);
                }

                // 2b. Build mating pool
                MatingPool.Clear();
                for (int i = 0; i < POPULATION_SIZE; i++)
                {
                    int occurences = (int)(Population[i].Fitness * 100);
                    for (int j = 0; j < occurences; j++)
                    {
                        MatingPool.Add(Population[i]);
                    }
                }

                // --- STEP 3: Reproduction ---
                var randomNumberGenerator = new Random((int)DateTime.Now.Ticks);
                for (int i = 0; i < POPULATION_SIZE; i++)
                {
                    var dadIndex = randomNumberGenerator.Next(0, MatingPool.Count);
                    var momIndex = randomNumberGenerator.Next(0, MatingPool.Count);

                    var dad = MatingPool[dadIndex];
                    var mom = MatingPool[momIndex];

                    var child = dad.Crossover(mom);
                    child.Mutate(MUTATION_RATE);

                    Population[i] = child;
                }
                GC.Collect();
            }
        }
    }
}
