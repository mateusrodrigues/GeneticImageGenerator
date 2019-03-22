using Shipwreck.Phash;
using Shipwreck.Phash.Bitmaps;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace GeneticImageGenerator
{
    public class Dna
    {
        public Bitmap Genes { get; private set; }
        public double Fitness { get; private set; }

        public Dna(int width, int height, bool generateRandomImage = true)
        {
            Random randomGenerator = new Random((int) DateTime.Now.Ticks);

            Genes = new Bitmap(width, height);
            if (generateRandomImage)
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        // var currentColor = Color.FromKnownColor((KnownColor) randomGenerator.Next(1, 168));
                        //var currentColor = Color.FromArgb(randomGenerator.Next(0, 256),
                        //    randomGenerator.Next(0, 256), randomGenerator.Next(0, 256));
                        //int grayscale = (int)((currentColor.R * 0.3) + (currentColor.G * 0.59)
                        //    + (currentColor.B * 0.11));
                        //var grayscaleColor = Color.FromArgb(grayscale, grayscale, grayscale);
                        var number = randomGenerator.Next(0, 2);
                        if (number % 2 == 0)
                        {
                            Genes.SetPixel(i, j, Color.White);
                        }
                        else
                        {
                            Genes.SetPixel(i, j, Color.Black);
                        }
                    }
                }
            }
        }

        public void CalculateFitness(Bitmap goal)
        {
            //double score = 0;
            //var currentImageHash = ImagePhash.ComputeDigest(Genes.ToLuminanceImage());
            //var goalHash = ImagePhash.ComputeDigest(goal.ToLuminanceImage());

            //score = ImagePhash.GetCrossCorrelation(currentImageHash, goalHash);

            //Fitness = score;

            int pixelsInPlace = 0;
            int totalPixels = Genes.Width * Genes.Height;
            for (int i = 0; i < Genes.Width; i++)
            {
                for (int j = 0; j < Genes.Height; j++)
                {
                    if (goal.GetPixel(i, j) == Genes.GetPixel(i, j))
                    {
                        pixelsInPlace++;
                    }
                }
            }
            Fitness = ((double)pixelsInPlace) / totalPixels;
        }

        public Dna Crossover(Dna partner)
        {
            // The crossover strategy consists of creating a new image
            // by intercalating the pixels coming from each of the partners.

            // 1. Check if both bitmaps are of the same size
            if (partner.Genes.Width != Genes.Width ||
                partner.Genes.Height != Genes.Height)
            {
                throw new NotSupportedException("The bitmaps are of different sizes." +
                    " Crossover operation not supported.");
            }

            // 2. Create the child object
            var child = new Dna(Genes.Width, Genes.Height, generateRandomImage: false);

            // 3. Iterate the pixels and intercalate origin
            int midpoint = (Genes.Width * Genes.Height) / 2;
            for (int i = 0; i < Genes.Width; i++)
            {
                for (int j = 0; j < Genes.Height; j++)
                {
                    var currentPixel = (i * Genes.Width) + (j + 1);
                    if (currentPixel < midpoint)
                    {
                       child.Genes.SetPixel(i, j, Genes.GetPixel(i, j));
                    }
                    else
                    {
                       child.Genes.SetPixel(i, j, partner.Genes.GetPixel(i, j));
                    }
                    // if (j % 2 == 0)
                    // {
                    //     // Genetic information comes from current image
                    //     child.Genes.SetPixel(i, j, Genes.GetPixel(i, j));
                    // }
                    // else
                    // {
                    //     // Genetic information comes from partner image
                    //     child.Genes.SetPixel(i, j, partner.Genes.GetPixel(i, j));
                    // }
                }
            }

            // 4. Return the resulting object from the crossover operation
            return child;
        }

        public void Mutate(double mutationRate)
        {
            // If the random value generated is less than or equal to
            // the mutation rate probability, then mutate a random pixel
            // in the gene pool to a random color.
            var randomNumberGenerator = new Random((int) DateTime.Now.Ticks);
            var value = randomNumberGenerator.NextDouble();

            if (value <= mutationRate)
            {
                Genes.SetPixel(randomNumberGenerator.Next(0, Genes.Width),
                    randomNumberGenerator.Next(0, Genes.Height),
                    /*Color.FromKnownColor((KnownColor)randomNumberGenerator.Next(0, 168))*/
                    Color.Black);
            }
        }

        public override string ToString()
        {
            return("Genetic Information\n" +
                $"\t- Gene Count: {Genes.Width * Genes.Height}\n" +
                $"\t- Fitness: {(Fitness * 100)}%");
        }
    }
}
