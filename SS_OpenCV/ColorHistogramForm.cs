using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SS_OpenCV
{
	public partial class ColorHistogramForm : Form
	{
		public ColorHistogramForm(int[,] array, int depth)
		{
			InitializeComponent();
			if(depth==4)
			{
				chart1.Series.Clear();
				int i, j;
				for (i = 0; i < depth; i++)
				{
					Series series = new Series();
					series.ChartType = SeriesChartType.Line;
					DataPointCollection points = series.Points;
					for (j = 0; j < 256; j++)
						points.AddXY(j, array[i, j]);

					chart1.Series.Add(series);
				}
				chart1.Series[0].Color = Color.Gray;
				chart1.Series[1].Color = Color.Blue;
				chart1.Series[2].Color = Color.Green;
				chart1.Series[3].Color = Color.Red;
				chart1.Series[0].Name = "Gray";
				chart1.Series[1].Name = "Blue";
				chart1.Series[2].Name = "Green";
				chart1.Series[3].Name = "Red";
				chart1.ChartAreas[0].AxisX.Maximum = 255;
				chart1.ChartAreas[0].AxisX.Minimum = 0;
				chart1.ChartAreas[0].AxisX.Title = "Intensidade";
				chart1.ChartAreas[0].AxisY.Title = "Numero Pixeis";
				chart1.ResumeLayout();
			}
			else if (depth == 3)
			{
				chart1.Series.Clear();
				int i, j;
				for (i = 0; i < depth; i++)
				{
					Series series = new Series();
					series.ChartType = SeriesChartType.Line;
					DataPointCollection points = series.Points;
					for (j = 0; j < 256; j++)
						points.AddXY(j, array[i, j]);

					chart1.Series.Add(series);
				}
				chart1.Series[0].Color = Color.Blue;
				chart1.Series[1].Color = Color.Green;
				chart1.Series[2].Color = Color.Red;
				chart1.Series[0].Name = "Blue";
				chart1.Series[1].Name = "Green";
				chart1.Series[2].Name = "Red";
				chart1.ChartAreas[0].AxisX.Maximum = 255;
				chart1.ChartAreas[0].AxisX.Minimum = 0;
				chart1.ChartAreas[0].AxisX.Title = "Intensidade";
				chart1.ChartAreas[0].AxisY.Title = "Numero Pixeis";
				chart1.ResumeLayout();
			}

		}

		private void chart1_Click(object sender, EventArgs e)
		{

		}
	}
}
