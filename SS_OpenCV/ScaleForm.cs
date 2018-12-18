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
	public partial class ScaleForm : Form
	{
		public int xCenter, yCenter;
		public float scale;
		public bool useCenter;
		public ScaleForm()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void label5_Click(object sender, EventArgs e)
		{

		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			numericUpDown1.Enabled = !numericUpDown1.Enabled;
			numericUpDown2.Enabled = !numericUpDown2.Enabled;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			xCenter = (int)numericUpDown1.Value;
			yCenter = (int)numericUpDown2.Value;
			scale = (float)numericUpDown3.Value;
			useCenter = checkBox1.Checked;
			DialogResult = DialogResult.OK;
		}
	}
}
