using System;
using System.Collections;

namespace Sudoku
{

	public class Population
	{

		protected const int sudoLength = 9;
		protected const int crossOverAt = sudoLength/2;
		protected const int initPopulation = 100;
		protected const int PopulationLimit = 50;
		protected const int kMin = 1;
		protected const int kMax = 100;
		protected const float  mutationRate = 0.33f;
		protected const float  kDeathFitness = -1.00f;
        protected const float kReproductionFitness = 0.0f;

        protected ArrayList Genomes = new ArrayList();//create new genome
		protected ArrayList GenomeReproducers  = new ArrayList();
		protected ArrayList GenomeResults = new ArrayList();
		protected ArrayList GenomeFamily = new ArrayList();

		protected int		  CurrentPopulation = initPopulation;
		protected int		  Generation = 1;
		protected bool	  select2 = true; //to select the best two

		public Population()
		{
			
			for  (int i = 0; i < initPopulation; i++)
			{
				SudokuChromosome chromosome = new SudokuChromosome(sudoLength, kMin, kMax);
				chromosome.SetCrossoverPoint(crossOverAt);
				chromosome.CalculateFitness();
				Genomes.Add(chromosome);
			}

		}

		private void Mutate(Chromosome aGene)
		{
			if ( SudokuChromosome.TheSeed.Next(100)< (mutationRate * 100.0))
            {
			  	aGene.Mutate();
			}
		}

		public void NextGeneration()
		{
			// add generation counter;
			Generation++; 


			//
			for  (int i = 0; i < Genomes.Count; i++)
			{
				if  (((Chromosome)Genomes[i]).CanDie(kDeathFitness))
				{
					Genomes.RemoveAt(i);
					i--;
				}
			}


			// check who can reproduce
			GenomeReproducers.Clear();
			GenomeResults.Clear();

			for  (int i = 0; i < Genomes.Count; i++)
			{
				if (((Chromosome)Genomes[i]).CanReproduce(kReproductionFitness))
				{
					GenomeReproducers.Add(Genomes[i]);			
				}
			}
			
			// crossover genes and add them to the population
			 DoCrossover(GenomeReproducers);

			Genomes = (ArrayList)GenomeResults.Clone();

			// mutate few genes in the new population
			for  (int i = 0; i < Genomes.Count; i++)
			{
				Mutate((Chromosome)Genomes[i]);
			}

			// calculate fitness of all the genes
			for  (int i = 0; i < Genomes.Count; i++)
			{
				((Chromosome)Genomes[i]).CalculateFitness();
			}


            Genomes.Sort(); 

            // kill genes above the population limit
            if (Genomes.Count > PopulationLimit)
				Genomes.RemoveRange(PopulationLimit, Genomes.Count - PopulationLimit);
			
			CurrentPopulation = Genomes.Count;
			
		}

		public  void CalculateFitnessForAll(ArrayList genes)
		{
			foreach(SudokuChromosome lg in genes)
			{
			  lg.CalculateFitness();
			}
		}

		public void DoCrossover(ArrayList genes)
		{
			ArrayList GeneMoms = new ArrayList();
			ArrayList GeneDads = new ArrayList();

			for (int i = 0; i < genes.Count; i++)
			{
				// randomly pick the parents
				if (SudokuChromosome.TheSeed.Next(100) % 2 > 0)
				{
					GeneMoms.Add(genes[i]);
				}
				else
				{
					GeneDads.Add(genes[i]);
				}
			}

            //to balance the number of different parents
			if (GeneMoms.Count > GeneDads.Count)
			{
				while (GeneMoms.Count > GeneDads.Count)
				{
					GeneDads.Add(GeneMoms[GeneMoms.Count - 1]);
					GeneMoms.RemoveAt(GeneMoms.Count - 1);
				}

				if (GeneDads.Count > GeneMoms.Count)
				{
					GeneDads.RemoveAt(GeneDads.Count - 1); // make sure they are equal
				}

			}
			else
			{
				while (GeneDads.Count > GeneMoms.Count)
				{
					GeneMoms.Add(GeneDads[GeneDads.Count - 1]);
					GeneDads.RemoveAt(GeneDads.Count - 1);
				}

				if (GeneMoms.Count > GeneDads.Count)
				{
					GeneMoms.RemoveAt(GeneMoms.Count - 1); // make sure they are equal
				}
			}

			// cross them over and add them according to fitness
			for (int i = 0; i < GeneDads.Count; i ++)
			{
				// select the best two from parent and children
				SudokuChromosome babyGene1 = (SudokuChromosome)((SudokuChromosome)GeneDads[i]).Crossover((SudokuChromosome)GeneMoms[i]);
			    SudokuChromosome babyGene2 = (SudokuChromosome)((SudokuChromosome)GeneMoms[i]).Crossover((SudokuChromosome)GeneDads[i]);
			
				GenomeFamily.Clear();
				GenomeFamily.Add(GeneDads[i]);
				GenomeFamily.Add(GeneMoms[i]);
				GenomeFamily.Add(babyGene1);
				GenomeFamily.Add(babyGene2);
				CalculateFitnessForAll(GenomeFamily);
				GenomeFamily.Sort();

				if (select2 == true)
				{
					// if select2 is true, add top fitness genes
					GenomeResults.Add(GenomeFamily[0]);					
					GenomeResults.Add(GenomeFamily[1]);					

				}
				else
				{
					GenomeResults.Add(babyGene1);
					GenomeResults.Add(babyGene2);
				}
			}

		}

		public Chromosome GetHighestScoreGenome()
		{
			Genomes.Sort();
			return (Chromosome)Genomes[0];
		}

		public virtual void WriteNextGeneration()
		{
			// write the top 20
			Console.WriteLine("Generation {0}\n", Generation);
			if (Generation % 1  == 0) 
			{
				Genomes.Sort();
				for  (int i = 0; i <  CurrentPopulation ; i++)
				{
					Console.WriteLine(((Chromosome)Genomes[i]).ToString());
				}

				Console.WriteLine("Hit the enter key to continue...\n");
				Console.ReadLine();
			}
		}
	}
}
