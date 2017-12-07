using System;
using System.Collections;

namespace Sudoku
{
	/// <summary>
	/// Summary description for Genome.
	/// </summary>
	public abstract class Chromosome : IComparable
	{
		public long Length;
		public int  CrossoverPoint;
		public int  MutationIndex;
		public float CurrentFitness = 0.0f;

		abstract public void Initialize();
		abstract public void Mutate();
		abstract public Chromosome Crossover(Chromosome g);
		abstract public object GenerateGeneValue(object min, object max);
		abstract public void SetCrossoverPoint(int crossoverPoint);
		abstract public float CalculateFitness();
		abstract public bool  CanReproduce(float fitness);
		abstract public bool  CanDie(float fitness);
		abstract public string ToString();
		abstract public void	CopyGeneInfo(Chromosome g);

		
		abstract public int CompareTo(object a);

	}
}
