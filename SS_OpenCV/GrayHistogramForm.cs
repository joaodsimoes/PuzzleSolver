using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SS_OpenCV
{
	public partial class GrayHistogramForm : Form
	{
		public GrayHistogramForm(int[] array)
		{
			InitializeComponent();

			DataPointCollection list1 = chart1.Series[0].Points;
			for (int i = 0; i < 256; i++)
			{
				list1.AddXY(i, array[i]);
			}
			chart1.Series[0].Color = Color.Gray;
			chart1.ChartAreas[0].AxisX.Maximum = 255;
			chart1.ChartAreas[0].AxisX.Minimum = 0;
			chart1.ChartAreas[0].AxisX.Title = "Intensidade";
			chart1.ChartAreas[0].AxisY.Title = "Numero Pixeis";
			chart1.ResumeLayout();
		}

		private void chart1_Click(object sender, EventArgs e)
		{

		}
	}
}
