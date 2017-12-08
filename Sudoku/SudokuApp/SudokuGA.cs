using System;
using System.Collections;

namespace Sudoku
{
	public class Soduku
	{
    private const int SudokuLength = 9;
    private const int CrossOverAt = SudokuLength / 2;
    private const int InitPopulation = 100;
    private const int PopulationLimit = 75;
    private const int Min = 1;
    private const int Max = 100;
    private const float MutationRate = 0.33f;
    private const float DeathFitness = -1.00f;
    private const float ReproductionFitness = 0.0f;

    private ArrayList _chromosome = new ArrayList();
    private ArrayList _producers = new ArrayList();
    private ArrayList _results = new ArrayList();
    private ArrayList _family = new ArrayList();

    private int _currentPopulation = InitPopulation;
    private int _generation = 1;

    public Soduku()
		{
			for  (int i = 0; i < InitPopulation; i++)
			{
				SudokuGA chromosome = new SudokuGA(SudokuLength, Min, Max);
				chromosome.SetCrossoverPoint(CrossOverAt);
				chromosome.CalculateFitness();
			  _chromosome.Add(chromosome);
			}
		}

		private void Mutate(SudokuGA chromosome)
		{
			if ( SudokuGA.rand.Next(100)< (MutationRate * 100.0)) chromosome.Mutate();
		}

		public void NextGeneration()
		{
		  _generation++; 

			for  (var i = 0; i < _chromosome.Count; i++)
			{
			  if (!((SudokuGA)_chromosome[i]).CanDie(DeathFitness)) continue;
			  _chromosome.RemoveAt(i);
			  i--;
			}

		  _producers.Clear();
		  _results.Clear();

			foreach (SudokuGA chromosome in _chromosome)
			{
			  if (chromosome.CanReproduce(ReproductionFitness))
			  {
			    _producers.Add(chromosome);			
			  }
			}
      Crossover(_producers);

			_chromosome = (ArrayList) _results.Clone();

			foreach (SudokuGA c in _chromosome)
			{
			  Mutate(c);
			}

		  CalculateFamilyFitness(_chromosome);

      // Sort to keep maximum fittest value
		  //_chromosome.Sort();

      if (_chromosome.Count > PopulationLimit)
				_chromosome.RemoveRange(PopulationLimit, _chromosome.Count - PopulationLimit);
			
			_currentPopulation = _chromosome.Count;
		}

    public  void CalculateFamilyFitness(ArrayList chromosome)
		{
			foreach(SudokuGA lg in chromosome)
			{
			  lg.CalculateFitness();
			}
		}

		public void Crossover(ArrayList chromosome)
		{
			ArrayList parent1 = new ArrayList();
			ArrayList parent2 = new ArrayList();

			for (int i = 0; i < chromosome.Count; i++)
			{
				if (SudokuGA.rand.Next(100) % 2 > 0)
				{
				  parent1.Add(chromosome[i]);
				}
				else
				{
				  parent2.Add(chromosome[i]);
				}
			}

			if (parent1.Count > parent2.Count)
			{
				while (parent1.Count > parent2.Count)
				{
				  parent2.Add(parent1[parent1.Count - 1]);
				  parent1.RemoveAt(parent1.Count - 1);
				}

				if (parent2.Count > parent1.Count)
				{
				  parent2.RemoveAt(parent2.Count - 1); 
				}

			}
			else
			{
				while (parent2.Count > parent1.Count)
				{
				  parent1.Add(parent2[parent2.Count - 1]);
				  parent2.RemoveAt(parent2.Count - 1);
				}

				if (parent1.Count > parent2.Count)
				{
				  parent1.RemoveAt(parent1.Count - 1); 
				}
			}

			for (int i = 0; i < parent2.Count; i ++)
			{
				SudokuGA childGene1 = (SudokuGA)((SudokuGA)parent2[i]).Crossover((SudokuGA)parent1[i]);
			  SudokuGA childGene2 = (SudokuGA)((SudokuGA)parent1[i]).Crossover((SudokuGA)parent2[i]);
			
				_family.Clear();
			  _family.Add(parent2[i]);
			  _family.Add(parent1[i]);
			  _family.Add(childGene1);
			  _family.Add(childGene2);
				CalculateFamilyFitness(_family);
			  _family.Sort();

			  _results.Add(_family[0]);
			  _results.Add(_family[1]);
      }

		}

		public SudokuGA GetFittest()
		{
			_chromosome.Sort();
			return (SudokuGA)_chromosome[0];
		}
	}

  public class SudokuGA : IComparable
  {
    private int _length;
    private int _crossoverPoint;
    private int _mutationIndex;
    private float _currentFitness = 0.0f;
    private readonly int[,] _theArray = new int[9, 9];
    public static Random rand = new Random((int)DateTime.Now.Ticks);
    private int min = 0;
    private int max = 100;
    private readonly Hashtable _rowTable = new Hashtable();
    private readonly Hashtable _columnTable = new Hashtable();
    private readonly Hashtable _squaretable = new Hashtable();

    public float CurrentFitness
    {
      get => _currentFitness;
    }

    public int CompareTo(object a)
    {
      SudokuGA gene1 = this;
      SudokuGA gene2 = (SudokuGA)a;
      return Math.Sign(gene2._currentFitness - gene1._currentFitness);
    }

    public void SetCrossoverPoint(int crossoverPoint)
    {
      _crossoverPoint = crossoverPoint;
    }

    public SudokuGA(int length, object min, object max)
    {
      _length = length;
      this.min = (int)min;
      this.max = (int)max;
      for (int i = 0; i < 9; i++)
      {
        int offset = rand.Next(9);
        for (int j = 0; j < 9; j++)
        {
          _theArray[j, i] = (int)GeneratRandomValue(1, 9);
        }
      }
    }

    public bool CanDie(float fitness)
    {
      if (_currentFitness <= fitness)
      {
        return true;
      }

      return false;
    }

    public bool CanReproduce(float fitness)
    {
      if (rand.Next(100) >= (int)(fitness * 100))
      {
        return true;
      }

      return false;
    }

    public object GeneratRandomValue(object min, object max)
    {
      return rand.Next((int)min, (int)max + 1);
    }

    public void Mutate()
    {
      int mutationIndex1 = rand.Next((int)9);
      int mutationIndex2 = rand.Next((int)9);
      int mutationIndex3 = rand.Next((int)9);

      if (rand.Next(2) == 1)
      {
        _theArray[mutationIndex1, mutationIndex2] = mutationIndex3 + 1;
      }
      else
      {
        int temp = 0;
        if (rand.Next(2) == 1)
        {
          temp = _theArray[mutationIndex1, mutationIndex2];
          _theArray[mutationIndex1, mutationIndex2] = _theArray[mutationIndex3, mutationIndex2];
          _theArray[mutationIndex3, mutationIndex2] = temp;

        }
        else
        {
          temp = _theArray[mutationIndex2, mutationIndex1];
          _theArray[mutationIndex2, mutationIndex1] = _theArray[mutationIndex2, mutationIndex3];
          _theArray[mutationIndex2, mutationIndex3] = temp;
        }
      }

    }

    private void CalculateSudokuFitness()
    {
      float fitnessColumns = 0;
      float fitnessRows = 0;
      float fitnessSquares = 0;

      for (int i = 0; i < 9; i++)
      {
        _columnTable.Clear();
        for (int j = 0; j < 9; j++)
        {
          if (_columnTable[_theArray[i, j]] == null)
          {
            _columnTable[_theArray[i, j]] = 0;
          }

          _columnTable[_theArray[i, j]] = ((int)_columnTable[_theArray[i, j]]) + 1;
        }

        fitnessColumns += (float)(1.0f / (10 - _columnTable.Count)) / 9.0f;
      }

      for (int i = 0; i < 9; i++)
      {
        _rowTable.Clear();
        for (int j = 0; j < 9; j++)
        {
          if (_rowTable[_theArray[j, i]] == null)
          {
            _rowTable[_theArray[j, i]] = 0;
          }

          _rowTable[_theArray[j, i]] = ((int)_rowTable[_theArray[j, i]]) + 1;
        }

        fitnessRows += (float)(1.0f / (10 - _rowTable.Count)) / 9.0f;
      }

      for (int l = 0; l < 3; l++)
      {
        for (int k = 0; k < 3; k++)
        {
          _squaretable.Clear();
          for (int i = 0; i < 3; i++)
          {
            for (int j = 0; j < 3; j++)
            {
              if (_squaretable[_theArray[i + k * 3, j + l * 3]] == null)
              {
                _squaretable[_theArray[i + k * 3, j + l * 3]] = 0;
              }
              _squaretable[_theArray[i + k * 3, j + l * 3]] = ((int)_squaretable[_theArray[i + k * 3, j + l * 3]]) + 1;
            }
          }

          fitnessSquares += (float)(1.0f / (10 - _squaretable.Count)) / 9.0f;
        }

      }
      _currentFitness = fitnessColumns * fitnessRows * fitnessSquares;
    }

    public float CalculateFitness()
    {
      CalculateSudokuFitness();
      return _currentFitness;
    }

    public override string ToString()
    {
      string strResult = "";
      for (int j = 0; j < _length; j++)
      {
        for (int i = 0; i < _length; i++)
        {
          strResult = strResult + ((int)_theArray[i, j]).ToString() + " ";
        }
        strResult += "\r\n";
      }

      strResult += "Current Fitness -->" + _currentFitness.ToString();

      return strResult;
    }

    public SudokuGA Crossover(SudokuGA chromosome)
    {
      var aChild1 = new SudokuGA(_length, min, max);
      var aChild2 = new SudokuGA(_length, min, max);

      var crossingGene = (SudokuGA)chromosome;
      if (rand.Next(2) == 1)
      {
        for (int j = 0; j < 9; j++)
        {
          _crossoverPoint = rand.Next(8) + 1;
          for (int k = 0; k < _crossoverPoint; k++)
          {
            aChild1._theArray[k, j] = crossingGene._theArray[k, j];
            aChild2._theArray[k, j] = _theArray[k, j];
          }

          for (int k = _crossoverPoint; k < 9; k++)
          {
            aChild2._theArray[k, j] = crossingGene._theArray[k, j];
            aChild1._theArray[k, j] = _theArray[k, j];
          }
        }
      }
      else
      {
        for (int j = 0; j < 9; j++)
        {
          _crossoverPoint = rand.Next(8) + 1;
          for (int k = 0; k < _crossoverPoint; k++)
          {
            aChild1._theArray[j, k] = crossingGene._theArray[j, k];
            aChild2._theArray[j, k] = _theArray[j, k];
          }

          for (int k = _crossoverPoint; k < 9; k++)
          {
            aChild2._theArray[j, k] = crossingGene._theArray[j, k];
            aChild1._theArray[j, k] = _theArray[j, k];
          }
        }
      }

      SudokuGA aChromosome = null;
      if (rand.Next(2) == 1)
      {
        aChromosome = aChild1;
      }
      else
      {
        aChromosome = aChild2;
      }

      return aChromosome;
    }

  }
}
