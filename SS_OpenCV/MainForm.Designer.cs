namespace SS_OpenCV
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.imageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.colorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.negativeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.grayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.brightnessContrastToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.transformsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.translationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.rotationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.zoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.filtersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.media3x3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.nonUniformToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.autoZoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.edgeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.sobelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.diferentiationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.histogramsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.rGBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.autoresToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.evalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.panel1 = new System.Windows.Forms.Panel();
			this.ImageViewer = new System.Windows.Forms.PictureBox();
			this.allToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.grayToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.medianToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.binarizationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.manualToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.otsuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.redChannelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip1.SuspendLayout();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ImageViewer)).BeginInit();
			this.SuspendLayout();
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.Filter = "Images (*.png, *.bmp, *.jpg)|*.png;*.bmp;*.jpg";
			// 
			// menuStrip1
			// 
			this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.imageToolStripMenuItem,
            this.autoresToolStripMenuItem,
            this.evalToolStripMenuItem,
            this.testToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(792, 24);
			this.menuStrip1.TabIndex = 2;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
			this.openToolStripMenuItem.Text = "Open...";
			this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
			this.saveToolStripMenuItem.Text = "Save As...";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(120, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
			this.exitToolStripMenuItem.Text = "Exit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem});
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
			this.editToolStripMenuItem.Text = "Edit";
			// 
			// undoToolStripMenuItem
			// 
			this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
			this.undoToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
			this.undoToolStripMenuItem.Text = "Undo";
			this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
			// 
			// imageToolStripMenuItem
			// 
			this.imageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.colorToolStripMenuItem,
            this.transformsToolStripMenuItem,
            this.filtersToolStripMenuItem,
            this.autoZoomToolStripMenuItem,
            this.edgeToolStripMenuItem,
            this.histogramsToolStripMenuItem,
            this.binarizationToolStripMenuItem});
			this.imageToolStripMenuItem.Name = "imageToolStripMenuItem";
			this.imageToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
			this.imageToolStripMenuItem.Text = "Image";
			// 
			// colorToolStripMenuItem
			// 
			this.colorToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.negativeToolStripMenuItem,
            this.grayToolStripMenuItem,
            this.brightnessContrastToolStripMenuItem,
            this.redChannelToolStripMenuItem});
			this.colorToolStripMenuItem.Name = "colorToolStripMenuItem";
			this.colorToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.colorToolStripMenuItem.Text = "Color";
			// 
			// negativeToolStripMenuItem
			// 
			this.negativeToolStripMenuItem.Name = "negativeToolStripMenuItem";
			this.negativeToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
			this.negativeToolStripMenuItem.Text = "Negative";
			this.negativeToolStripMenuItem.Click += new System.EventHandler(this.negativeToolStripMenuItem_Click);
			// 
			// grayToolStripMenuItem
			// 
			this.grayToolStripMenuItem.Name = "grayToolStripMenuItem";
			this.grayToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
			this.grayToolStripMenuItem.Text = "Gray";
			this.grayToolStripMenuItem.Click += new System.EventHandler(this.grayToolStripMenuItem_Click);
			// 
			// brightnessContrastToolStripMenuItem
			// 
			this.brightnessContrastToolStripMenuItem.Name = "brightnessContrastToolStripMenuItem";
			this.brightnessContrastToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
			this.brightnessContrastToolStripMenuItem.Text = "Brightness and Contrast";
			this.brightnessContrastToolStripMenuItem.Click += new System.EventHandler(this.brightnessContrastToolStripMenuItem_Click);
			// 
			// transformsToolStripMenuItem
			// 
			this.transformsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.translationToolStripMenuItem,
            this.rotationToolStripMenuItem,
            this.zoomToolStripMenuItem});
			this.transformsToolStripMenuItem.Name = "transformsToolStripMenuItem";
			this.transformsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.transformsToolStripMenuItem.Text = "Transforms";
			// 
			// translationToolStripMenuItem
			// 
			this.translationToolStripMenuItem.Name = "translationToolStripMenuItem";
			this.translationToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
			this.translationToolStripMenuItem.Text = "Translation";
			this.translationToolStripMenuItem.Click += new System.EventHandler(this.translationToolStripMenuItem_Click);
			// 
			// rotationToolStripMenuItem
			// 
			this.rotationToolStripMenuItem.Name = "rotationToolStripMenuItem";
			this.rotationToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
			this.rotationToolStripMenuItem.Text = "Rotation";
			this.rotationToolStripMenuItem.Click += new System.EventHandler(this.rotationToolStripMenuItem_Click);
			// 
			// zoomToolStripMenuItem
			// 
			this.zoomToolStripMenuItem.Name = "zoomToolStripMenuItem";
			this.zoomToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
			this.zoomToolStripMenuItem.Text = "Zoom";
			this.zoomToolStripMenuItem.Click += new System.EventHandler(this.zoomToolStripMenuItem_Click);
			// 
			// filtersToolStripMenuItem
			// 
			this.filtersToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.media3x3ToolStripMenuItem,
            this.nonUniformToolStripMenuItem,
            this.medianToolStripMenuItem});
			this.filtersToolStripMenuItem.Name = "filtersToolStripMenuItem";
			this.filtersToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.filtersToolStripMenuItem.Text = "Filters";
			// 
			// media3x3ToolStripMenuItem
			// 
			this.media3x3ToolStripMenuItem.Name = "media3x3ToolStripMenuItem";
			this.media3x3ToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
			this.media3x3ToolStripMenuItem.Text = "Media3x3";
			this.media3x3ToolStripMenuItem.Click += new System.EventHandler(this.media3x3ToolStripMenuItem_Click);
			// 
			// nonUniformToolStripMenuItem
			// 
			this.nonUniformToolStripMenuItem.Name = "nonUniformToolStripMenuItem";
			this.nonUniformToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
			this.nonUniformToolStripMenuItem.Text = "NonUniform";
			this.nonUniformToolStripMenuItem.Click += new System.EventHandler(this.nonUniformToolStripMenuItem_Click);
			// 
			// autoZoomToolStripMenuItem
			// 
			this.autoZoomToolStripMenuItem.CheckOnClick = true;
			this.autoZoomToolStripMenuItem.Name = "autoZoomToolStripMenuItem";
			this.autoZoomToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.autoZoomToolStripMenuItem.Text = "Auto Zoom";
			this.autoZoomToolStripMenuItem.Click += new System.EventHandler(this.autoZoomToolStripMenuItem_Click);
			// 
			// edgeToolStripMenuItem
			// 
			this.edgeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sobelToolStripMenuItem,
            this.diferentiationToolStripMenuItem});
			this.edgeToolStripMenuItem.Name = "edgeToolStripMenuItem";
			this.edgeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.edgeToolStripMenuItem.Text = "Edge Detection";
			// 
			// sobelToolStripMenuItem
			// 
			this.sobelToolStripMenuItem.Name = "sobelToolStripMenuItem";
			this.sobelToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
			this.sobelToolStripMenuItem.Text = "Sobel";
			this.sobelToolStripMenuItem.Click += new System.EventHandler(this.sobelToolStripMenuItem_Click);
			// 
			// diferentiationToolStripMenuItem
			// 
			this.diferentiationToolStripMenuItem.Name = "diferentiationToolStripMenuItem";
			this.diferentiationToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
			this.diferentiationToolStripMenuItem.Text = "Differentiation";
			this.diferentiationToolStripMenuItem.Click += new System.EventHandler(this.diferentiationToolStripMenuItem_Click);
			// 
			// histogramsToolStripMenuItem
			// 
			this.histogramsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.grayToolStripMenuItem1,
            this.rGBToolStripMenuItem,
            this.allToolStripMenuItem});
			this.histogramsToolStripMenuItem.Name = "histogramsToolStripMenuItem";
			this.histogramsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.histogramsToolStripMenuItem.Text = "Histograms";
			// 
			// rGBToolStripMenuItem
			// 
			this.rGBToolStripMenuItem.Name = "rGBToolStripMenuItem";
			this.rGBToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.rGBToolStripMenuItem.Text = "RGB";
			this.rGBToolStripMenuItem.Click += new System.EventHandler(this.rGBToolStripMenuItem_Click);
			// 
			// autoresToolStripMenuItem
			// 
			this.autoresToolStripMenuItem.Name = "autoresToolStripMenuItem";
			this.autoresToolStripMenuItem.Size = new System.Drawing.Size(69, 20);
			this.autoresToolStripMenuItem.Text = "Autores...";
			this.autoresToolStripMenuItem.Click += new System.EventHandler(this.autoresToolStripMenuItem_Click);
			// 
			// evalToolStripMenuItem
			// 
			this.evalToolStripMenuItem.Name = "evalToolStripMenuItem";
			this.evalToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
			this.evalToolStripMenuItem.Text = "Eval";
			this.evalToolStripMenuItem.Click += new System.EventHandler(this.evalToolStripMenuItem_Click);
			// 
			// testToolStripMenuItem
			// 
			this.testToolStripMenuItem.Name = "testToolStripMenuItem";
			this.testToolStripMenuItem.Size = new System.Drawing.Size(83, 20);
			this.testToolStripMenuItem.Text = "Solve Puzzle";
			this.testToolStripMenuItem.Click += new System.EventHandler(this.testToolStripMenuItem_Click);
			// 
			// panel1
			// 
			this.panel1.AutoScroll = true;
			this.panel1.Controls.Add(this.ImageViewer);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 24);
			this.panel1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(792, 516);
			this.panel1.TabIndex = 6;
			// 
			// ImageViewer
			// 
			this.ImageViewer.Location = new System.Drawing.Point(0, 0);
			this.ImageViewer.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.ImageViewer.Name = "ImageViewer";
			this.ImageViewer.Size = new System.Drawing.Size(576, 427);
			this.ImageViewer.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.ImageViewer.TabIndex = 6;
			this.ImageViewer.TabStop = false;
			this.ImageViewer.Click += new System.EventHandler(this.ImageViewer_Click);
			// 
			// allToolStripMenuItem
			// 
			this.allToolStripMenuItem.Name = "allToolStripMenuItem";
			this.allToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.allToolStripMenuItem.Text = "All";
			this.allToolStripMenuItem.Click += new System.EventHandler(this.allToolStripMenuItem_Click);
			// 
			// grayToolStripMenuItem1
			// 
			this.grayToolStripMenuItem1.Name = "grayToolStripMenuItem1";
			this.grayToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
			this.grayToolStripMenuItem1.Text = "Gray";
			this.grayToolStripMenuItem1.Click += new System.EventHandler(this.grayToolStripMenuItem1_Click);
			// 
			// medianToolStripMenuItem
			// 
			this.medianToolStripMenuItem.Name = "medianToolStripMenuItem";
			this.medianToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.medianToolStripMenuItem.Text = "Median";
			this.medianToolStripMenuItem.Click += new System.EventHandler(this.medianToolStripMenuItem_Click);
			// 
			// binarizationToolStripMenuItem
			// 
			this.binarizationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.manualToolStripMenuItem,
            this.otsuToolStripMenuItem});
			this.binarizationToolStripMenuItem.Name = "binarizationToolStripMenuItem";
			this.binarizationToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.binarizationToolStripMenuItem.Text = "Binarization";
			// 
			// manualToolStripMenuItem
			// 
			this.manualToolStripMenuItem.Name = "manualToolStripMenuItem";
			this.manualToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.manualToolStripMenuItem.Text = "Manual";
			this.manualToolStripMenuItem.Click += new System.EventHandler(this.manualToolStripMenuItem_Click);
			// 
			// otsuToolStripMenuItem
			// 
			this.otsuToolStripMenuItem.Name = "otsuToolStripMenuItem";
			this.otsuToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.otsuToolStripMenuItem.Text = "Otsu";
			this.otsuToolStripMenuItem.Click += new System.EventHandler(this.otsuToolStripMenuItem_Click);
			// 
			// redChannelToolStripMenuItem
			// 
			this.redChannelToolStripMenuItem.Name = "redChannelToolStripMenuItem";
			this.redChannelToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
			this.redChannelToolStripMenuItem.Text = "Red Channel";
			this.redChannelToolStripMenuItem.Click += new System.EventHandler(this.redChannelToolStripMenuItem_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(792, 540);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.Name = "MainForm";
			this.Text = "Sistemas Sensoriais 2018/2019 - Image processing";
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.ImageViewer)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem imageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem colorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem negativeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem transformsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem translationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rotationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filtersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoresToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoZoomToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem evalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem grayToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem media3x3ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem edgeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem sobelToolStripMenuItem;
		private System.Windows.Forms.PictureBox ImageViewer;
		private System.Windows.Forms.ToolStripMenuItem brightnessContrastToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem diferentiationToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem nonUniformToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem histogramsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem rGBToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem grayToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem allToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem medianToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem binarizationToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem manualToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem otsuToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem redChannelToolStripMenuItem;
	}
}

