using System;
using System.Windows.Forms;

namespace SS_OpenCV
{
	public partial class NonUniformForm : Form
	{
		public float[,] matrix;
		public float weight;
		public NonUniformForm()
		{
			InitializeComponent();
			matrix = new float[,] { { 1f, 1f, 1f }, { 1f, 1f, 1f }, { 1f, 1f, 1f } };
			weight = 9f;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (comboBox1.SelectedIndex == 0)
			{
				numericUpDown1.Value = 1;
				numericUpDown2.Value = 1;
				numericUpDown3.Value = 1;

				numericUpDown4.Value = 1;
				numericUpDown5.Value = 1;
				numericUpDown6.Value = 1;

				numericUpDown7.Value = 1;
				numericUpDown8.Value = 1;
				numericUpDown9.Value = 1;

				numericUpDown10.Value = 9;
			}
		}

		private void numericUpDown10_ValueChanged(object sender, EventArgs e)
		{

		}

		private void button2_Click(object sender, EventArgs e)
		{
			matrix = new float[,]{
				{ (float)numericUpDown1.Value, (float)numericUpDown2.Value, (float)numericUpDown3.Value },
				{ (float)numericUpDown4.Value, (float)numericUpDown5.Value, (float)numericUpDown6.Value },
				{ (float)numericUpDown7.Value, (float)numericUpDown8.Value, (float)numericUpDown9.Value }
			};

			weight = (float)numericUpDown10.Value;

			DialogResult = DialogResult.OK;
		}

		private void numericUpDown9_ValueChanged(object sender, EventArgs e)
		{

		}
	}
}
