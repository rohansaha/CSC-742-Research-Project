using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;

namespace Sudoku
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		bool _threadFlag = false;
		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();



			_oThread = new Thread(new ThreadStart(GenomeThread));
			_oThread.Start();

			Invalidate();
			
			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		private System.Windows.Forms.StatusBar statusBar1;

		Thread _oThread = null;

		void GenomeThread()
		{
			CalculateGeneration(1000, 100000);
		}

		int ToPercent (float val)
		{
			return (int)(val * 100);
		}

	    Genome _gene = null;
		public void CalculateGeneration(int nPopulation, int nGeneration)
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
					if (  ToPercent(g.CurrentFitness) != _previousFitness)
					{
						Console.WriteLine(g.ToString());
						_gene = g;
						statusBar1.Text = String.Format("Current Fitness = {0}", g.CurrentFitness.ToString("0.00"));
						this.Text = String.Format("Sudoko Grid - Generation {0}", i);
						Invalidate();
						_previousFitness = ToPercent(g.CurrentFitness);
					}

					if (g.CurrentFitness > .9999)
					{
						Console.WriteLine("Final Solution at Generation {0}", i);
						statusBar1.Text = "Finished";
						Console.WriteLine(g.ToString());
						break;
					}
				}

				//				TestPopulation.WriteNextGeneration();
			} 

			
		}


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.statusBar1 = new System.Windows.Forms.StatusBar();
			this.SuspendLayout();
			// 
			// statusBar1
			// 
			this.statusBar1.Location = new System.Drawing.Point(0, 326);
			this.statusBar1.Name = "statusBar1";
			this.statusBar1.Size = new System.Drawing.Size(352, 22);
			this.statusBar1.TabIndex = 0;
			this.statusBar1.Text = "Ready...";
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(352, 348);
			this.Controls.Add(this.statusBar1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.Form1_Closing);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		Pen _penThick = new Pen(Color.Black, 3);
		Font _sudukoFont = new Font("Arial", 16, FontStyle.Regular);

		private void Form1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			// draw square
			Graphics g = e.Graphics;

			Rectangle r = ClientRectangle;
			r.Inflate(-statusBar1.Height -2, -statusBar1.Height - 2);
			g.DrawRectangle(_penThick, r);

			int spacingX = r.Width/9;
			int spacingY = r.Height/9;
			for (int i = 0; i < 9; i++)
			{
				if (i % 3 == 0)
				{
					g.DrawLine(_penThick, r.Left, r.Top + spacingY * i, r.Right, r.Top + spacingY*i);
					g.DrawLine(_penThick, r.Left + spacingX * i, r.Top, r.Left + spacingX * i, r.Bottom);
				}
				else
				{
					g.DrawLine(Pens.Black, r.Left, r.Top + spacingY * i, r.Right, r.Top + spacingY*i);
					g.DrawLine(Pens.Black, r.Left + spacingX * i, r.Top, r.Left + spacingX * i, r.Bottom);
				}
			}

			for (int i = 0; i < 9; i++)
				for (int j = 0; j < 9; j++)
				{
					if (_gene != null)
					{
						int val = (_gene as SudokuGenome)[i, j];
						g.DrawString(val.ToString(), _sudukoFont, Brushes.Black, r.Left + i*spacingX + 5, r.Top + j*spacingY + 5, new StringFormat());
					}
				}

		}

		private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			_threadFlag = true;
		}
	}
}
