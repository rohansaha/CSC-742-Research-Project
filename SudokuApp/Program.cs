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
        static bool _threadFlag = false;
        static void Main(string[] args)
        {
            Thread _oThread = new Thread(new ThreadStart(GenomeThread));
            _oThread.Start();
        }
        static void GenomeThread()
        {
            CalculateGeneration(100, 10000);
           
        }

        static int ToPercent(float val)
        {
            return (int)(val * 100);
        }

        static Genome _gene = null;
        public static void CalculateGeneration(int nPopulation, int nGeneration)
        {
            int _previousFitness = 0;
            Population TestPopulation = new Population();
            //			TestPopulation.WriteNextGeneration();
            for (int i = 0; i < nGeneration; i++)
            {
                if (_threadFlag)
                    break;
                TestPopulation.NextGeneration();
                Genome g = TestPopulation.GetHighestScoreGenome();

                if (i % 100 == 0)
                {
                    Console.WriteLine("Generation #{0}", i);
                    if (ToPercent(g.CurrentFitness) != _previousFitness)
                    {
                        Console.WriteLine(g.ToString());
                        _gene = g;
                        ////CheckForIllegalCrossThreadCalls = false; statusBar1.Text = String.Format("Current Fitness = {0}", g.CurrentFitness.ToString("0.00"));
                        //this.Text = String.Format("Sudoko Grid - Generation {0}", i);
                        //Invalidate();
                        _previousFitness = ToPercent(g.CurrentFitness);
                    }

                    if (g.CurrentFitness > .9999)
                    {
                        Console.WriteLine("Final Solution at Generation {0}", i);
                        //statusBar1.Text = "Finished";
                        Console.WriteLine(g.ToString());
                        Console.ReadLine();
                        break;
                    }
                }

                //				TestPopulation.WriteNextGeneration();
            }


        }
    }
}
