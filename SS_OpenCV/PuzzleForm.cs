using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SS_OpenCV
{
	public partial class PuzzleForm : Form
	{
		private List<int[]> pieces;
		private List<int> pieces_angles;
		private Image<Bgr, Byte> originalImg;
		private Image<Bgr, Byte> rotatedImg;
		private Image<Bgr, Byte> finishedImg;
		private byte[] black = { 0, 0, 0 };
		private byte[] white = { 255, 255, 255 };

		public PuzzleForm(Image<Bgr, Byte> img)
		{
			InitializeComponent();
			pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
			Cursor = Cursors.WaitCursor; // clock cursor 

			rotatedImg = img.Copy();
			originalImg = img.Copy();
			ImageClass.puzzleRotateOnly(rotatedImg, rotatedImg.Copy());


			finishedImg = ImageClass.puzzle(img, img.Copy(), out pieces, out pieces_angles, 3);

			paintBlack(rotatedImg, pieces);
			paintBlack(originalImg, pieces);
			Cursor = Cursors.Default; // normal cursor 

			for (int i = 0; i < pieces.Count; i++)
			{
				ListViewItem lvi = new ListViewItem(i.ToString());
				lvi.SubItems.Add(pieces_angles[i].ToString() + " Degrees");
				listView1.Items.Add(lvi);
			}
			listView1.Refresh();

			pictureBox1.Image = originalImg.Bitmap;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			pictureBox1.Image = finishedImg.Bitmap;
			pictureBox1.Size = new System.Drawing.Size(finishedImg.Width, finishedImg.Height);
			pictureBox1.Refresh();
		}

		private void button3_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			pictureBox1.Image = rotatedImg.Bitmap;
			pictureBox1.Size = new System.Drawing.Size(rotatedImg.Width, rotatedImg.Height);
			pictureBox1.Refresh();
		}

		private void button4_Click(object sender, EventArgs e)
		{
			pictureBox1.Image = originalImg.Bitmap;
			pictureBox1.Size = new System.Drawing.Size(originalImg.Width, originalImg.Height);
			pictureBox1.Refresh();
		}

		private void paintBlack(Image<Bgr, Byte> img, List<int[]> pieces)
		{
			int startingX, startingY, tmpWidth, tmpHeigth;
			foreach (int[] piece in pieces)
			{
				startingX = piece[0];
				startingY = piece[1];
				tmpWidth = piece[2] - piece[0] + 2;
				tmpHeigth = piece[3] - piece[1] + 2;
				img.ROI = new Rectangle(startingX, startingY, tmpWidth, tmpHeigth);
				paintRec(img, black);
				img.ROI = Rectangle.Empty;

			}
		}

		private void paintRec(Image<Bgr, Byte> img, byte[] color)
		{
			int width = img.Width;
			int height = img.Height;
			Bgr c = new Bgr(color[0], color[1], color[2]);

			for (int i = 0; i < width - 1; i++)
			{
				img[0, i] = c;
				img[1, i] = c;
				img[height - 1, i] = c;
				img[height - 2, i] = c;
			}
			for (int i = 0; i < height - 1; i++)
			{
				img[i, 0] = c;
				img[i, 1] = c;
				img[i, width - 1] = c;
				img[i, width - 2] = c;
			}
		}

		private int[] getCordsFromAbs(int width, int point)
		{
			int[] cords = new int[2];
			cords[0] = point % width;
			cords[1] = point / width;
			return cords;
		}

		private void listView1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listView1.SelectedIndices.Count <= 0)
				return;

			int startingX, startingY, tmpWidth, tmpHeigth;
			int idss = int.Parse(listView1.SelectedItems[0].SubItems[0].Text);
			paintBlack(originalImg, pieces);
			paintBlack(rotatedImg, pieces);

			int[] piece = pieces[idss];
			startingX = piece[0];
			startingY = piece[1];
			tmpWidth = piece[2] - piece[0] + 2;
			tmpHeigth = piece[3] - piece[1] + 2;
			originalImg.ROI = new Rectangle(startingX, startingY, tmpWidth, tmpHeigth);
			rotatedImg.ROI = new Rectangle(startingX, startingY, tmpWidth, tmpHeigth);
			paintRec(originalImg, white);
			paintRec(rotatedImg, white);

			rotatedImg.ROI = Rectangle.Empty;
			originalImg.ROI = Rectangle.Empty;

			pictureBox1.Refresh();
		}
	}
}