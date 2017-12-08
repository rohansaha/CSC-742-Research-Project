using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Sudoku;

namespace SudokuApp
{
  class Program
  {
    static void Main(string[] args)
    {
      ComputeGeneticAlgorithm(100, 10000);
    }

    public static void ComputeGeneticAlgorithm(int nPopulation, int nGeneration)
    {
      bool isOptimal = false;
      float previousFitness = 0;
      Soduku popoulation = new Soduku();
      SudokuGA optimalSudoku = new SudokuGA(9,1,100);
      for (int i = 0; i < nGeneration; i++)
      {
        popoulation.NextGeneration();
        SudokuGA chromosome = popoulation.GetFittest();
        if (i % 100 == 0)
        {
          Console.WriteLine("Generation #{0}", i);
          if (chromosome.CurrentFitness != previousFitness)
          {
            previousFitness = chromosome.CurrentFitness;
            Console.WriteLine(chromosome.ToString());
          }

          if (chromosome.CurrentFitness >= 1)
          {
            isOptimal = true;
            Console.WriteLine("Final Solution at Generation {0}", i);
            Console.WriteLine(chromosome.ToString());
            break;
          }
        }
        optimalSudoku = chromosome;
      }
      if (!isOptimal)
      {
        Console.WriteLine("Max Optimal Solution reached");
        Console.WriteLine(optimalSudoku.ToString());
      }
      Console.ReadLine();
    }
  }
}
