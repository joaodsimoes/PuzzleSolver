using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SS_OpenCV
{
	public partial class BrightContrastForm : Form
	{
		public int brightness;
		public double contrast;
		public BrightContrastForm()
		{
			InitializeComponent();
		}

		private void label1_Click(object sender, EventArgs e)
		{

		}

		private void button1_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			brightness = (int)numericUpDown1.Value;
			contrast = (double)numericUpDown2.Value;
			DialogResult = DialogResult.OK;
		}

		private void numericUpDown2_ValueChanged(object sender, EventArgs e)
		{

		}
	}
}
