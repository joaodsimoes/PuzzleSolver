using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SS_OpenCV
{
	public partial class MainForm : Form
	{
		private Image<Bgr, Byte> img = null; // working image
		private Image<Bgr, Byte> imgUndo = null; // undo backup image - UNDO
		private string title_bak = "";

		public MainForm()
		{
			InitializeComponent();
			title_bak = Text;
		}

		/// <summary>
		/// Opens a new image
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				img = new Image<Bgr, byte>(openFileDialog1.FileName);
				Text = title_bak + " [" +
						openFileDialog1.FileName.Substring(openFileDialog1.FileName.LastIndexOf("\\") + 1) +
						"]";
				imgUndo = img.Copy();
				ImageViewer.Image = img.Bitmap;
				ImageViewer.Refresh();
			}
		}

		/// <summary>
		/// Saves an image with a new name
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				ImageViewer.Image.Save(saveFileDialog1.FileName);
			}
		}

		/// <summary>
		/// Closes the application
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		/// <summary>
		/// restore last undo copy of the working image
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void undoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (imgUndo == null) // verify if the image is already opened
				return;
			Cursor = Cursors.WaitCursor;
			img = imgUndo.Copy();

			ImageViewer.Image = img.Bitmap;
			ImageViewer.Refresh(); // refresh image on the screen

			Cursor = Cursors.Default; // normal cursor 
		}

		/// <summary>
		/// Change visualization mode
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void autoZoomToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// zoom
			if (autoZoomToolStripMenuItem.Checked)
			{
				ImageViewer.SizeMode = PictureBoxSizeMode.Zoom;
				ImageViewer.Dock = DockStyle.Fill;
			}
			else // with scroll bars
			{
				ImageViewer.Dock = DockStyle.None;
				ImageViewer.SizeMode = PictureBoxSizeMode.AutoSize;
			}
		}

		/// <summary>
		/// Show authors form
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void autoresToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AuthorsForm form = new AuthorsForm();
			form.ShowDialog();
		}

		/// <summary>
		/// Calculate the image negative
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void negativeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (img == null) // verify if the image is already opened
				return;
			Cursor = Cursors.WaitCursor; // clock cursor 

			//copy Undo Image
			imgUndo = img.Copy();

			ImageClass.Negative(img);

			ImageViewer.Image = img.Bitmap;
			ImageViewer.Refresh(); // refresh image on the screen

			Cursor = Cursors.Default; // normal cursor 
		}

		/// <summary>
		/// Call automated image processing check
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void evalToolStripMenuItem_Click(object sender, EventArgs e)
		{
			EvalForm eval = new EvalForm();
			eval.WindowState = FormWindowState.Maximized;
			eval.ShowDialog();
		}

		/// <summary>
		/// Call image convertion to gray scale
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void grayToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (img == null) // verify if the image is already opened
				return;
			Cursor = Cursors.WaitCursor; // clock cursor 

			//copy Undo Image
			imgUndo = img.Copy();

			ImageClass.ConvertToGray(img);

			ImageViewer.Image = img.Bitmap;
			ImageViewer.Refresh(); // refresh image on the screen

			Cursor = Cursors.Default; // normal cursor 
		}

		private void testToolStripMenuItem_Click(object sender, EventArgs e)
		{

			if (img == null) // verify if the image is already opened
				return;

			using (PuzzleForm form = new PuzzleForm(img.Copy()))
			{
				form.ShowDialog();
			}

			
			

		}

		private void media3x3ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (img == null) // verify if the image is already opened
				return;
			Cursor = Cursors.WaitCursor; // clock cursor 

			//copy Undo Image
			imgUndo = img.Copy();

			ImageClass.Mean(img, imgUndo);

			ImageViewer.Image = img.Bitmap;
			ImageViewer.Refresh(); // refresh image on the screen

			Cursor = Cursors.Default; // normal cursor 
		}

		private void sobelToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (img == null) // verify if the image is already opened
				return;
			Cursor = Cursors.WaitCursor; // clock cursor 

			//copy Undo Image
			imgUndo = img.Copy();

			ImageClass.Sobel(img, imgUndo);

			ImageViewer.Image = img.Bitmap;
			ImageViewer.Refresh(); // refresh image on the screen

			Cursor = Cursors.Default; // normal cursor 
		}

		private void diferentiationToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (img == null) // verify if the image is already opened
				return;
			Cursor = Cursors.WaitCursor; // clock cursor 

			//copy Undo Image
			imgUndo = img.Copy();

			ImageClass.Diferentiation(img, imgUndo);

			ImageViewer.Image = img.Bitmap;
			ImageViewer.Refresh(); // refresh image on the screen

			Cursor = Cursors.Default; // normal cursor 
		}

		private void translationToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (img == null) // verify if the image is already opened
				return;

			using (TranslationForm form = new TranslationForm())
			{
				if (form.ShowDialog() == DialogResult.OK)
				{
					Cursor = Cursors.WaitCursor; // clock cursor 
												 //copy Undo Image
					imgUndo = img.Copy();

					ImageClass.Translation(img, imgUndo, form.dx, form.dy);

					ImageViewer.Image = img.Bitmap;
					ImageViewer.Refresh(); // refresh image on the screen

					Cursor = Cursors.Default; // normal cursor 
				}
			}

		}

		private void rotationToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (img == null) // verify if the image is already opened
				return;
			Cursor = Cursors.WaitCursor; // clock cursor 

			InputBox form = new InputBox("Input angle (Deg)");
			form.ShowDialog();
			float angle = (float)ImageClass.ConvertDegreesToRadians(double.Parse(form.ValueTextBox.Text));

			//copy Undo Image
			imgUndo = img.Copy();

			ImageClass.Rotation(img, imgUndo, angle);

			ImageViewer.Image = img.Bitmap;
			ImageViewer.Refresh(); // refresh image on the screen

			Cursor = Cursors.Default; // normal cursor 
		}

		private void zoomToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (img == null) // verify if the image is already opened
				return;

			using (ScaleForm form = new ScaleForm())
			{
				if (form.ShowDialog() == DialogResult.OK)
				{
					Cursor = Cursors.WaitCursor; // clock cursor 
												 //copy Undo Image
					imgUndo = img.Copy();
					if (form.useCenter)
						ImageClass.Scale_point_xy(img, imgUndo, form.scale, form.xCenter, form.yCenter);
					else
						ImageClass.Scale(img, imgUndo, form.scale);

					ImageViewer.Image = img.Bitmap;
					ImageViewer.Refresh(); // refresh image on the screen

					Cursor = Cursors.Default; // normal cursor 
				}
			}
		}

		private void brightnessContrastToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (img == null) // verify if the image is already opened
				return;

			using (BrightContrastForm form = new BrightContrastForm())
			{
				if (form.ShowDialog() == DialogResult.OK)
				{
					Cursor = Cursors.WaitCursor; // clock cursor 
												 //copy Undo Image
					imgUndo = img.Copy();

					ImageClass.BrightContrast(img, form.brightness, form.contrast);

					ImageViewer.Image = img.Bitmap;
					ImageViewer.Refresh(); // refresh image on the screen

					Cursor = Cursors.Default; // normal cursor 
				}
			}
		}

		private void nonUniformToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (img == null) // verify if the image is already opened
				return;

			using (NonUniformForm form = new NonUniformForm())
			{
				if (form.ShowDialog() == DialogResult.OK)
				{
					Cursor = Cursors.WaitCursor; // clock cursor 
												 //copy Undo Image
					imgUndo = img.Copy();

					ImageClass.NonUniform(img,imgUndo, form.matrix, form.weight);

					ImageViewer.Image = img.Bitmap;
					ImageViewer.Refresh(); // refresh image on the screen

					Cursor = Cursors.Default; // normal cursor 
				}
			}
		}

		private void rGBToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (img == null) // verify if the image is already opened
				return;

			using (ColorHistogramForm form = new ColorHistogramForm(ImageClass.Histogram_RGB(img), 3))
			{
				form.ShowDialog();
			}
		}

		private void grayToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			if (img == null) // verify if the image is already opened
				return;

			using (GrayHistogramForm form = new GrayHistogramForm(ImageClass.Histogram_Gray(img)))
			{
				form.ShowDialog();
			}
		}

		private void allToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (img == null) // verify if the image is already opened
				return;

			using (ColorHistogramForm form = new ColorHistogramForm(ImageClass.Histogram_All(img), 4))
			{
				form.ShowDialog();
			}
		}

		private void medianToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (img == null) // verify if the image is already opened
				return;
			Cursor = Cursors.WaitCursor; // clock cursor 

			//copy Undo Image
			imgUndo = img.Copy();

			ImageClass.Median(img, imgUndo);

			ImageViewer.Image = img.Bitmap;
			ImageViewer.Refresh(); // refresh image on the screen

			Cursor = Cursors.Default; // normal cursor 
		}

		private void otsuToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (img == null) // verify if the image is already opened
				return;
			Cursor = Cursors.WaitCursor; // clock cursor 

			//copy Undo Image
			imgUndo = img.Copy();

			ImageClass.ConvertToBW_Otsu(img);

			ImageViewer.Image = img.Bitmap;
			ImageViewer.Refresh(); // refresh image on the screen

			Cursor = Cursors.Default; // normal cursor 
		}

		private void manualToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (img == null) // verify if the image is already opened
				return;
			Cursor = Cursors.WaitCursor; // clock cursor 

			InputBox form = new InputBox("Input treshold (0-255)");
			form.ShowDialog();
			int threshold = int.Parse(form.ValueTextBox.Text);
			if (threshold < 0)
				threshold = 0;
			else if (threshold > 255)
				threshold = 255;

			//copy Undo Image
			imgUndo = img.Copy();

			ImageClass.ConvertToBW(img,threshold);

			ImageViewer.Image = img.Bitmap;
			ImageViewer.Refresh(); // refresh image on the screen

			Cursor = Cursors.Default; // normal cursor 
		}

		private void redChannelToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (img == null) // verify if the image is already opened
				return;
			Cursor = Cursors.WaitCursor; // clock cursor 

			//copy Undo Image
			imgUndo = img.Copy();

			ImageClass.RedChannel(img);

			ImageViewer.Image = img.Bitmap;
			ImageViewer.Refresh(); // refresh image on the screen

			Cursor = Cursors.Default; // normal cursor 
		}

		private void ImageViewer_Click(object sender, EventArgs e)
		{

		}
	}




}