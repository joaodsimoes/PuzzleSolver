using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace SS_OpenCV
{
	internal class ImageClass
	{

		private const int XCoordinate = 0;
		private const int YCoordinate = 1;
		private const int TopLeft = 0;
		private const int TopRight = 1;
		private const int BottomLeft = 2;
		private const int BottomRight = 3;
		private const double Rad2Deg = 180.0 / Math.PI;
		private const double Deg2Rad = Math.PI / 180.0;

		/// <summary>
		/// Image Negative using EmguCV library
		/// Super fast
		/// </summary>
		/// <param name="img">Image</param>
		public static unsafe void Negative(Image<Bgr, byte> img)
		{

			MIplImage m = img.MIplImage;
			// simple way
			long* dataPtrBase = (long*)m.imageData.ToPointer(); // Pointer to the image

			int nChan = m.nChannels; // number of channels - 3
			int padding = m.widthStep - nChan * m.width; // alinhament bytes (padding)
			int height = m.height;
			//int total8 = m.widthStep / 8 * height;
			int total4 = m.widthStep / 4 * height;

			long* dataPtr8 = dataPtrBase;
			long* finish = dataPtrBase + total4 / 2;
			while (dataPtr8 < finish)
				*dataPtr8 = ~*dataPtr8++;

			int* dataPtr4 = (int*)m.imageData.ToPointer() + (total4 / 2) * 2;
			int* finish4 = (int*)dataPtrBase + total4;
			while (dataPtr4 < finish4)
				*dataPtr4 = ~*dataPtr4++;
		}

		private static int Clamp(int value)
		{
			return (value >= 255) ? 255 : (value <= 0) ? 0 : value;
		}

		public static int Clamp(int value, int min, int max)
		{
			return (value >= max) ? max : (value <= min) ? min : value;
		}


		public static unsafe void BrightContrast(Image<Bgr, byte> img, int bright, double contrast)
		{
			MIplImage m = img.MIplImage;
			byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image

			int nChan = m.nChannels; // number of channels - 3
			int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)


			byte[] lookup = new byte[256];
			double v;
			for (int i = 0; i < 256; i++)
			{
				v = Math.Round(i * contrast + bright);
				if (v < 0)
					lookup[i] = 0;
				else if (v > 255)
					lookup[i] = 255;
				else
					lookup[i] = (byte)v;
			}

			int realWidth = img.Width * nChan;
			byte* lastPixel = dataPtr + m.widthStep * img.Height - padding;
			byte* c;
			for (; dataPtr < lastPixel; dataPtr += padding)
			{
				for (c = dataPtr + realWidth; dataPtr < c; dataPtr += 3)
				{
					dataPtr[0] = lookup[dataPtr[0]];
					dataPtr[1] = lookup[dataPtr[1]];
					dataPtr[2] = lookup[dataPtr[2]];
				}
			}
		}


		public static unsafe void RedChannel(Image<Bgr, byte> img)
		{
			MIplImage m = img.MIplImage;
			byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image

			int width = img.Width;
			int height = img.Height;
			int nChan = m.nChannels; // number of channels - 3
			int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
			int x, y;
			for (y = 0; y < height; y++)
			{
				for (x = 0; x < width; x++)
				{
					dataPtr[0] = dataPtr[1] = dataPtr[2];

					// advance the pointer to the next pixel
					dataPtr += nChan;
				}

				//at the end of the line advance the pointer by the aligment bytes (padding)
				dataPtr += padding;
			}
		}

		/// <summary>
		/// Convert to gray
		/// Direct access to memory - faster method
		/// </summary>
		/// <param name="img">image</param>
		public static void ConvertToGray(Image<Bgr, byte> img)
		{
			unsafe
			{
				// direct access to the image memory(sequencial)
				// direcion top left -> bottom right

				MIplImage m = img.MIplImage;
				byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
				byte blue, green, red, gray;

				int width = img.Width;
				int height = img.Height;
				int nChan = m.nChannels; // number of channels - 3
				int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
				int x, y;

				if (nChan == 3) // image in RGB
				{
					for (y = 0; y < height; y++)
					{
						for (x = 0; x < width; x++)
						{
							//retrive 3 colour components
							blue = dataPtr[0];
							green = dataPtr[1];
							red = dataPtr[2];

							// convert to gray
							gray = (byte)Math.Round((blue + green + red) / 3.0);

							// store in the image
							dataPtr[0] = gray;
							dataPtr[1] = gray;
							dataPtr[2] = gray;

							// advance the pointer to the next pixel
							dataPtr += nChan;
						}

						//at the end of the line advance the pointer by the aligment bytes (padding)
						dataPtr += padding;
					}
				}
			}
		}

		public static unsafe void Translation(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, int dx, int dy)
		{
			MIplImage m = img.MIplImage;
			MIplImage mCopy = imgCopy.MIplImage;
			byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
			byte* dataPtrCopy = (byte*)mCopy.imageData.ToPointer(); // Pointer to the image copy

			int nChan = m.nChannels; // number of channels - 3
			int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
			int line = m.widthStep;

			int startY = Math.Max(dy, 0) * line;
			int startX = Math.Max(dx, 0) * nChan;

			int width = Clamp((img.Width + dx), 0, img.Width) * nChan;
			int height = Clamp((img.Height + dy), 0, img.Height) * line;
			int imgWidth = img.Width * nChan;
			int imgHeight = img.Height * line;

			dataPtrCopy -= (dy * line + dx * nChan);

			int x, y;
			byte* original, newPixel;

			//paint all black if img is outside
			if (Math.Abs(dx) >= img.Width || Math.Abs(dy) >= img.Height)
			{
				int total8 = m.widthStep / 8 * img.Height;
				int total4 = m.widthStep / 4 * img.Height;

				long* dataPtr8 = (long*)dataPtr;
				long* finish = (long*)dataPtr + total8;
				while (dataPtr8 < finish)
					*dataPtr8++ = 0;

				int* dataPtr4 = (int*)m.imageData.ToPointer() + total8 * 2;
				int* finish4 = (int*)dataPtr + total4;
				while (dataPtr4 < finish4)
					*dataPtr4++ = 0;
			}
			//if its inside do it normally
			else
			{
				//Image
				for (y = startY; y < height; y += line)
				{
					for (x = startX; x < width; x += 3)
					{
						original = dataPtr + y + x;
						newPixel = dataPtrCopy + y + x;
						(original)[0] = (newPixel)[0];
						(original)[1] = (newPixel)[1];
						(original)[2] = (newPixel)[2];
					}
				}

				//Black bars
				if (dy < 0)
				{
					startY = img.Height * line + dy * line;
					imgHeight = img.Height * line;
				}
				else
				{
					startY = 0;
					imgHeight = dy * line;
				}
				if (dx < 0)
				{
					startX = img.Width * nChan + dx * nChan;
					imgWidth = img.Width * nChan;
				}
				else
				{
					startX = 0;
					imgWidth = dx * nChan;

				}

				//horizontal
				int imgWidthEnd = img.Width * nChan;
				for (y = startY; y < imgHeight; y += line)
				{
					for (x = 0; x < imgWidthEnd; x += 3)
					{
						original = dataPtr + y + x;
						(original)[0] = 0;
						(original)[1] = 0;
						(original)[2] = 0;
					}
				}
				//Vertical
				imgHeight = img.Height * line;
				for (y = 0; y < imgHeight; y += line)
				{
					for (x = startX; x < imgWidth; x += 3)
					{
						original = dataPtr + y + x;
						(original)[0] = 0;
						(original)[1] = 0;
						(original)[2] = 0;
					}
				}


			}
		}

		public static unsafe void Rotation(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float angle)
		{
			angle = -angle;
			MIplImage m = img.MIplImage;
			MIplImage mCopy = imgCopy.MIplImage;
			byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
			byte* dataPtrCopy = (byte*)mCopy.imageData.ToPointer(); // Pointer to the image copy

			int nChan = m.nChannels; // number of channels - 3
			int line = m.widthStep;
			int padding = m.widthStep - m.nChannels * m.width;

			int width = img.Width;
			int height = img.Height;

			double Yp = (double)height / 2;
			double Xp = (double)width / 2;
			double cos = Math.Cos(angle);
			double sin = Math.Sin(angle);
			double nxI = Xp - Xp * cos + Yp * sin;
			double nyI = Yp - Xp * sin - Yp * cos;
			double nx, ny;

			int newX, newY;
			byte* newPixel;
			int realWidth = width * nChan;
			byte* lastPixel = dataPtr + m.widthStep * height - padding;
			byte* c;
			for (; dataPtr < lastPixel; dataPtr += padding)
			{
				nx = nxI;
				ny = nyI;
				for (c = dataPtr + realWidth; dataPtr < c; dataPtr += 3)
				{
					newX = (int)Math.Round(nx);
					newY = (int)Math.Round(ny);
					if (newX >= 0 && newX < width && newY >= 0 && newY < height)
					{
						newPixel = dataPtrCopy + newY * line + newX * nChan;
						*dataPtr = *newPixel;
						*(dataPtr + 1) = *(newPixel + 1);
						*(dataPtr + 2) = *(newPixel + 2);

					}
					else
					{
						*(dataPtr) = 0;
						*(dataPtr + 1) = 0;
						*(dataPtr + 2) = 0;
					}
					nx += cos;
					ny += sin;
				}
				nxI -= sin;
				nyI += cos;
			}
		}

		

		public static unsafe void RotationCustomBackground(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float angle, byte[] background, out int[] border)
		{
			angle = -angle;
			MIplImage m = img.MIplImage;
			MIplImage mCopy = imgCopy.MIplImage;
			byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
			byte* dataPtrCopy = (byte*)mCopy.imageData.ToPointer(); // Pointer to the image copy

			int nChan = m.nChannels; // number of channels - 3
			int line = m.widthStep;
			int padding = m.widthStep - m.nChannels * m.width;

			int width = img.Width;
			int height = img.Height;

			double Yp = (double)height / 2;
			double Xp = (double)width / 2;
			double cos = Math.Cos(angle);
			double sin = Math.Sin(angle);
			double nxI = Xp - Xp * cos + Yp * sin;
			double nyI = Yp - Xp * sin - Yp * cos;
			double nx, ny;

			int newX, newY;
			byte* newPixel;
			border = new int[] { int.MaxValue, -1, int.MaxValue, -1 };
			int x, y;
			for (y = 0; y < height; y++)
			{
				nx = nxI;
				ny = nyI;
				for (x = 0; x < width; x++)
				{
					newX = (int)Math.Round(nx);
					newY = (int)Math.Round(ny);
					if (newX >= 0 && newX < width && newY >= 0 && newY < height)
					{
						newPixel = dataPtrCopy + newY * line + newX * nChan;
						if (newPixel[0] == background[0] && newPixel[1] == background[1] && newPixel[2] == background[2])
						{
							*dataPtr = newPixel[0];
							*(dataPtr + 1) = newPixel[1];
							*(dataPtr + 2) = newPixel[2];
						}
						else
						{
							*dataPtr = *newPixel;
							*(dataPtr + 1) = *(newPixel + 1);
							*(dataPtr + 2) = *(newPixel + 2);
						}


						if (*newPixel != background[0] && newPixel[1] != background[1] && newPixel[2] != background[2])
						{
							if (x < border[0])
								border[0] = x;
							if (x > border[1])
								border[1] = x;
							if (y < border[2])
								border[2] = y;
							if (y > border[3])
								border[3] = y;
						}
					}
					else
					{
						*(dataPtr) = background[0];
						*(dataPtr + 1) = background[1];
						*(dataPtr + 2) = background[2];

					}
					nx += cos;
					ny += sin;
					dataPtr += 3;
				}
				nxI -= sin;
				nyI += cos;
				dataPtr += padding;
			}
		}

		public static unsafe void Rotation_Poin_IgnoreBackground(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float angle, double Xp, double Yp)
		{
			angle = -angle;
			MIplImage m = img.MIplImage;
			MIplImage mCopy = imgCopy.MIplImage;
			byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
			byte* dataPtrCopy = (byte*)mCopy.imageData.ToPointer(); // Pointer to the image copy

			int nChan = m.nChannels; // number of channels - 3
			int line = m.widthStep;
			int padding = m.widthStep - m.nChannels * m.width;

			int width = img.Width;
			int height = img.Height;

			//double Yp = xP;
			//double Xp = yP;
			double cos = Math.Cos(angle);
			double sin = Math.Sin(angle);
			double nxI = Xp - Xp * cos + Yp * sin;
			double nyI = Yp - Xp * sin - Yp * cos;
			double nx, ny;

			int newX, newY;
			byte* newPixel;
			int realWidth = width * nChan;
			byte* lastPixel = dataPtr + m.widthStep * height - padding;
			byte* c;
			for (; dataPtr < lastPixel; dataPtr += padding)
			{
				nx = nxI;
				ny = nyI;
				for (c = dataPtr + realWidth; dataPtr < c; dataPtr += 3)
				{
					newX = (int)Math.Round(nx);
					newY = (int)Math.Round(ny);
					if (newX >= 0 && newX < width && newY >= 0 && newY < height)
					{
						newPixel = dataPtrCopy + newY * line + newX * nChan;
						*dataPtr = *newPixel;
						*(dataPtr + 1) = *(newPixel + 1);
						*(dataPtr + 2) = *(newPixel + 2);

					}
					else
					{
						*(dataPtr) = 0;
						*(dataPtr + 1) = 0;
						*(dataPtr + 2) = 0;
					}
					nx += cos;
					ny += sin;
				}
				nxI -= sin;
				nyI += cos;
			}
		}

		public static unsafe void Scale(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float scaleFactor)
		{
			if (scaleFactor == 0)
				scaleFactor = 1f;
			MIplImage m = img.MIplImage;
			MIplImage mCopy = imgCopy.MIplImage;
			byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
			byte* dataPtrCopy = (byte*)mCopy.imageData.ToPointer(); // Pointer to the image copy
			byte* pBack = dataPtr;

			int nChan = m.nChannels; // number of channels - 3
			int line = m.widthStep;
			int padding = m.widthStep - m.nChannels * m.width;

			int realWidth = img.Width;
			int realHeight = img.Height;
			int width = Clamp((int)Math.Ceiling(img.Width * scaleFactor), 0, img.Width);
			int height = Clamp((int)Math.Ceiling(img.Height * scaleFactor), 0, img.Height);

			int maxWH = Math.Max(width, height);
			int[] lookup = new int[maxWH];
			for (int i = 0; i < maxWH; i++)
			{
				lookup[i] = (int)Math.Round(i / scaleFactor);
			}

			int x, y, newY;
			byte* newPixel;
			int linePadding = line - width * nChan;
			for (y = 0; y < height; y++)
			{
				newY = lookup[y] * line;
				for (x = 0; x < width; x++)
				{
					newPixel = dataPtrCopy + newY + lookup[x] * nChan;
					dataPtr[0] = (newPixel)[0];
					dataPtr[1] = (newPixel)[1];
					dataPtr[2] = (newPixel)[2];
					dataPtr += nChan;
				}
				dataPtr += linePadding;
			}
			if (scaleFactor < 1)
			{
				dataPtr = pBack;
				byte* c;
				byte* lastPixel = dataPtr + line * img.Height - padding;

				realWidth *= nChan;
				width *= nChan;
				line = width + padding;
				dataPtr += width;
				int blackSpacing = realWidth - width;
				//horizontal
				for (; dataPtr < lastPixel; dataPtr += line)
				{
					for (c = dataPtr + blackSpacing; dataPtr < c; dataPtr += 3)
					{
						dataPtr[0] = 0;
						dataPtr[1] = 0;
						dataPtr[2] = 0;
					}
				}
				dataPtr = pBack + height * m.widthStep;
				line = blackSpacing + padding;
				//vertical
				for (; dataPtr < lastPixel; dataPtr += line)
				{
					for (c = dataPtr + width; dataPtr < c; dataPtr += 3)
					{
						dataPtr[0] = 0;
						dataPtr[1] = 0;
						dataPtr[2] = 0;
					}
				}
			}


		}

		public static unsafe void Scale_point_xy(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float scaleFactor, int centerX, int centerY)
		{
			if (scaleFactor == 0)
				scaleFactor = 1;
			MIplImage m = img.MIplImage;
			MIplImage mCopy = imgCopy.MIplImage;
			byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
			byte* dataPtrCopy = (byte*)mCopy.imageData.ToPointer(); // Pointer to the image copy
			byte* pBack = dataPtr;

			int nChan = m.nChannels; // number of channels - 3
			int line = m.widthStep;
			int padding = m.widthStep - m.nChannels * m.width;

			int width = img.Width;
			int height = img.Height;

			float scale = (1.0f / scaleFactor);

			float u = centerX - (width / 2 / scaleFactor);

			int[] lookupX = new int[width];
			bool[] lookupCheck = new bool[width];
			int tmp;
			for (int i = 0; i < width; i++)
			{
				tmp = (int)Math.Round(u + i * scale);
				lookupX[i] = tmp * nChan;
				lookupCheck[i] = tmp >= 0 && tmp < width;
			}


			int x, y;
			byte* newPixel;
			int linePadding = line - width * nChan;
			float v = centerY - (height / 2 / scaleFactor);

			int[] lookupY = new int[height];
			bool[] lookupCheckY = new bool[height];
			for (int i = 0; i < height; i++)
			{
				tmp = (int)Math.Round(v + i * scale);
				lookupY[i] = tmp * line;
				lookupCheckY[i] = tmp >= 0 && tmp < height;
			}

			for (y = 0; y < height; y++)
			{
				if (lookupCheckY[y])
				{
					for (x = 0; x < width; x++)
					{
						if (lookupCheck[x])
						{
							newPixel = dataPtrCopy + lookupY[y] + lookupX[x];
							(dataPtr)[0] = (newPixel)[0];
							(dataPtr)[1] = (newPixel)[1];
							(dataPtr)[2] = (newPixel)[2];
						}
						else
						{
							(dataPtr)[0] = 0;
							(dataPtr)[1] = 0;
							(dataPtr)[2] = 0;
						}
						dataPtr += 3;
					}

				}
				else
				{
					for (x = 0; x < width; x++)
					{
						(dataPtr)[0] = 0;
						(dataPtr)[1] = 0;
						(dataPtr)[2] = 0;
						dataPtr += 3;
					}
				}
				dataPtr += padding;
			}
		}

		private static unsafe int[,,] PadBorder(Image<Bgr, byte> img, int size)
		{
			MIplImage m = img.MIplImage;
			byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
			byte* dataPtrBase = dataPtr;
			int nChan = m.nChannels;
			int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
			int width = m.width;
			int height = m.height;
			int line = (width + size * 2) * nChan;
			int padding2 = size * 2 * nChan;

			int[,,] matrix = new int[height + size * 2, width + size * 2, m.nChannels];
			int x = size, y = size;

			fixed (int* p = &matrix[size, size, 0])
			{
				int* pTemp = p;
				for (y = 0; y < height; y++)
				{
					for (x = 0; x < width * 3; x++)
					{
						*(pTemp++) = *(dataPtr++);
					}
					dataPtr += padding;
					pTemp += padding2;
				}
				byte* dataPtrLast = dataPtrBase + nChan + line * height; ;
				for (int s = 1; s <= size; s++)
				{
					int sLines = s * line;
					int untilWidth = (width + s - 1) * nChan;
					int untilHeight = (height + s - 1) * line;
					int sChannels = s * nChan;
					for (x = -(s - 1); x < width + (s - 1); x++)
					{
						int xCol = x * nChan;
						int* tempPlus = p + xCol;
						int* temp = tempPlus - sLines;

						int* temp2 = (p + untilHeight + xCol);
						int* temp2Plus = (p + untilHeight + xCol) - sLines;
						for (int channel = 0; channel < nChan; channel++)
						{
							*(temp++) = *(tempPlus++);
							*(temp2++) = *(temp2Plus++);
						}
					}
					for (y = -s; y < height + s; y++)
					{
						int yline = y * line;

						int* temp = p - sChannels + yline;
						int* tempPlus = temp + nChan;

						int* temp2 = p + untilWidth + yline;
						int* temp2Plus = temp2 - nChan;
						for (int channel = 0; channel < nChan; channel++)
						{
							*(temp++) = *(tempPlus++);
							*(temp2++) = *(temp2Plus++);
						}
					}
				}
			}
			return matrix;
		}

		private static unsafe int[,,] PadBorder(int[,,] img, int width, int height, int nChan, int size)
		{
			int[,,] matrix = new int[height + size * 2, width + size * 2, nChan];
			int line = (width + size * 2) * nChan;
			// x   y  chan
			//[500,50,3]
			width *= nChan;
			int y, channel;

			fixed (int* p = &matrix[size, size, 0], imgPtr = &img[0, 0, 0])
			{
				int* dataPtr = imgPtr;
				int* final = p;
				int* end = final;
				int padding = 2 * size * nChan;
				for (y = 0; y < height; y++)
				{
					for (end = final + width; final < end; final++)
					{
						*(final) = *(dataPtr++);
					}
					final += padding;
				}
				int* finalLast;
				int* finalEnd;
				for (int s = 1; s <= size; s++)
				{
					//horizontal
					final = p - s * line - (s - 1) * nChan;
					finalLast = p + (height + s - 1) * line - (s - 1) * nChan;
					finalEnd = p + width + (s - 1) * nChan - s * line;
					while (final < finalEnd)
					{
						for (channel = 0; channel < nChan; channel++)
						{
							*(final) = *(final + line);
							*(finalLast) = *(finalLast - line);
							final++;
							finalLast++;
						}
					}
					//vertical
					final = p - s * (nChan + line);
					finalLast = finalEnd;
					finalEnd = p + (height + s - 1) * line - s * nChan;
					while (final < finalEnd)
					{
						for (channel = 0; channel < nChan; channel++)
						{
							*(final) = *(final + nChan);
							*(finalLast) = *(finalLast - nChan);
							final++;
							finalLast++;
						}

						final += line - nChan;
						finalLast += line - nChan;
					}
				}

			}
			return matrix;
		}

		public static unsafe void test(Image<Bgr, Byte> img)
		{
			/*byte[] bArray = { 3, 3, 3, 1, 0, 0, 255, 255, 255, 2, 4, 3 };
			fixed (byte* pointerB = &bArray[0])
			{
				int backColor = 0;
				backColor += 3 << 16;
				backColor += 4 << 8;
				backColor += 2;
				int t, a, b, c, d;
				int test;
				bool bTest;
				String aa, tt;
				int* pointer = (int*)pointerB;
				t = pointer[0];
				a = t & 0x000000ff;
				b = (t & 0x0000ff00) >> 8;
				c = (t & 0x00ff0000) >> 16;
				test = t & 0x00ffffff;
				bTest = test == backColor;
				aa = GetIntBinaryString(a);
				aa = GetIntBinaryString(t);

				pointer = (int*)(pointerB + 3);
				t = pointer[0];
				a = t & 0x000000ff;
				b = (t & 0x0000ff00) >> 8;
				c = (t & 0x00ff0000) >> 16;
				test = t & 0x00ffffff;
				bTest = test == backColor;
				aa = GetIntBinaryString(a);
				aa = GetIntBinaryString(t);

				pointer = (int*)(pointerB + 6);
				t = pointer[0];
				a = t & 0x000000ff;
				b = (t & 0x0000ff00) >> 8;
				c = (t & 0x00ff0000) >> 16;
				test = t & 0x00ffffff;
				bTest = test == backColor;
				aa = GetIntBinaryString(a);
				aa = GetIntBinaryString(t);

				pointer = (int*)(pointerB + 9);
				t = pointer[0];
				a = t & 0x000000ff;
				b = (t & 0x0000ff00) >> 8;
				c = (t & 0x00ff0000) >> 16;
				test = t & 0x00ffffff;
				bTest = test == backColor;
				aa = GetIntBinaryString(a);
				aa = GetIntBinaryString(backColor);
				tt = GetIntBinaryString(t);


			}*/


			//Console.WriteLine(var);
			//ConvertToBW_Otsu(img);
			//Image<Gray, Byte> result = new Image<Gray, byte>(img.Bitmap);
			Image<Gray, int> connected = ConnectedComponenets(img);

			//GetComponenets(img);
			new Image<Bgr, Byte>(connected.Bitmap).CopyTo(img);
			//img = new Image<Bgr, byte>(500, 500, new Bgr(122, 122, 122));


		}

		public static unsafe void MeanSolutionA(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
		{
			MIplImage m = img.MIplImage;
			MIplImage mCopy = imgCopy.MIplImage;

			byte* dataPtrBase = (byte*)m.imageData.ToPointer(); // Pointer to the image
			byte* dataPtrCopyBase = (byte*)mCopy.imageData.ToPointer();

			int nChan = m.nChannels; // number of channels - 3
			int padding = m.widthStep - m.nChannels * (m.width - 2); // alinhament bytes (padding)
			int line = m.widthStep;
			int width = m.width - 1;
			int height = m.height - 1;

			byte* copyPixel;
			int x, y;
			int b, g, r;

			byte[] lookup = new byte[2304];
			for (int i = 0; i < 2304; i++)
			{
				lookup[i] = (byte)Math.Round(i / 9.0);
			}

			byte* dataPtrCopy = dataPtrCopyBase + nChan + line;
			byte* dataPtr = dataPtrBase + nChan + line;

			//center
			for (y = 1; y < height; y++)
			{
				for (x = 1; x < width; x++)
				{
					*(dataPtr++) = lookup[*(dataPtrCopy - 3) + *(dataPtrCopy - 3 - line) + *dataPtrCopy + *(dataPtrCopy + 3 - line) + *(dataPtrCopy - line) + *(dataPtrCopy - 3 + line) + *(dataPtrCopy + 3) + *(dataPtrCopy + line) + *(dataPtrCopy + 3 + line)];
					*(dataPtr++) = lookup[*(dataPtrCopy - 2) + *(dataPtrCopy - 2 - line) + *(dataPtrCopy + 1) + *(dataPtrCopy + 4 - line) + *(dataPtrCopy + 1 - line) + *(dataPtrCopy + 4) + *(dataPtrCopy - 2 + line) + *(dataPtrCopy + 1 + line) + *(dataPtrCopy + 4 + line)];
					*(dataPtr++) = lookup[*(dataPtrCopy - 1) + *(dataPtrCopy - 1 - line) + *(dataPtrCopy + 2) + *(dataPtrCopy + 5 - line) + *(dataPtrCopy + 2 - line) + *(dataPtrCopy + 2 + line) + *(dataPtrCopy + 5 + line) + *(dataPtrCopy + 5) + *(dataPtrCopy - 1 + line)];

					dataPtrCopy += nChan;
				}
				dataPtr += padding;
				dataPtrCopy += padding;
			}

			//top
			dataPtrCopy = dataPtrCopyBase + nChan;
			dataPtr = dataPtrBase + nChan;
			for (x = 1; x < width; x++)
			{
				copyPixel = dataPtrCopy;

				*(dataPtr++) = lookup[(*(copyPixel - nChan) << 1) + (*(copyPixel + nChan) << 1) + (*(copyPixel) << 1) + *(copyPixel - nChan + line) + *(copyPixel + line) + *(copyPixel + nChan + line)];
				*(dataPtr++) = lookup[(*(copyPixel - 2) << 1) + (*(copyPixel + 1) << 1) + (*(copyPixel + 4) << 1) + *(copyPixel - 2 + line) + *(copyPixel + 1 + line) + *(copyPixel + 4 + line)];
				*(dataPtr++) = lookup[(*(copyPixel - 1) << 1) + (*(copyPixel + 2) << 1) + (*(copyPixel + 5) << 1) + *(copyPixel - 1 + line) + *(copyPixel + 2 + line) + *(copyPixel + 5 + line)];

				dataPtrCopy += nChan;
			}

			//bottom
			dataPtrCopy = dataPtrCopyBase + (height) * line + nChan;
			dataPtr = dataPtrBase + (height) * line + nChan;
			for (x = 1; x < width; x++)
			{
				copyPixel = dataPtrCopy;

				b = *(copyPixel - nChan) << 1;
				g = *(copyPixel - 2) << 1;
				r = *(copyPixel - 1) << 1;

				b += *(copyPixel) << 1;
				g += *(copyPixel + 1) << 1;
				r += *(copyPixel + 2) << 1;

				b += *(copyPixel + nChan) << 1;
				g += *(copyPixel + 4) << 1;
				r += *(copyPixel + 5) << 1;

				//second
				copyPixel = dataPtrCopy - line;

				b += *(copyPixel - nChan);
				g += *(copyPixel - 2);
				r += *(copyPixel - 1);

				b += *(copyPixel);
				g += *(copyPixel + 1);
				r += *(copyPixel + 2);

				b += *(copyPixel + nChan);
				g += *(copyPixel + 4);
				r += *(copyPixel + 5);

				*(dataPtr++) = lookup[b];
				*(dataPtr++) = lookup[g];
				*(dataPtr++) = lookup[r];

				dataPtrCopy += nChan;
			}

			//left
			dataPtrCopy = dataPtrCopyBase + line;
			dataPtr = dataPtrBase + line;
			for (y = 1; y < height; y++)
			{
				copyPixel = dataPtrCopy;

				b = *(copyPixel) << 1;
				g = *(copyPixel + 1) << 1;
				r = *(copyPixel + 2) << 1;

				b += *(copyPixel + nChan);
				g += *(copyPixel + 4);
				r += *(copyPixel + 5);

				//second
				copyPixel = dataPtrCopy - line;

				b += *(copyPixel) << 1;
				g += *(copyPixel + 1) << 1;
				r += *(copyPixel + 2) << 1;

				b += *(copyPixel + nChan);
				g += *(copyPixel + 4);
				r += *(copyPixel + 5);

				//second
				copyPixel = dataPtrCopy + line;

				b += *(copyPixel) << 1;
				g += *(copyPixel + 1) << 1;
				r += *(copyPixel + 2) << 1;

				b += *(copyPixel + nChan);
				g += *(copyPixel + 4);
				r += *(copyPixel + 5);

				*(dataPtr) = lookup[b];
				*(dataPtr + 1) = lookup[g];
				*(dataPtr + 2) = lookup[r];

				dataPtr += line;
				dataPtrCopy += line;
			}

			//right
			dataPtrCopy = dataPtrCopyBase + width * nChan + line;
			dataPtr = dataPtrBase + width * nChan + line;
			for (y = 1; y < height; y++)
			{
				copyPixel = dataPtrCopy;

				b = *(copyPixel) << 1;
				g = *(copyPixel + 1) << 1;
				r = *(copyPixel + 2) << 1;

				b += *(copyPixel - nChan);
				g += *(copyPixel - 2);
				r += *(copyPixel - 1);

				//second
				copyPixel = dataPtrCopy - line;

				b += *(copyPixel) << 1;
				g += *(copyPixel + 1) << 1;
				r += *(copyPixel + 2) << 1;

				b += *(copyPixel - nChan);
				g += *(copyPixel - 2);
				r += *(copyPixel - 1);

				//second
				copyPixel = dataPtrCopy + line;

				b += *(copyPixel) << 1;
				g += *(copyPixel + 1) << 1;
				r += *(copyPixel + 2) << 1;

				b += *(copyPixel - nChan);
				g += *(copyPixel - 2);
				r += *(copyPixel - 1);

				*(dataPtr) = lookup[b];
				*(dataPtr + 1) = lookup[g];
				*(dataPtr + 2) = lookup[r];

				dataPtr += line;
				dataPtrCopy += line;
			}

			//cantos
			PixelMediaFilter(dataPtrBase, dataPtrCopyBase, 0, 0, width, height, line, nChan, 1);
			PixelMediaFilter(dataPtrBase, dataPtrCopyBase, width, 0, width, height, line, nChan, 1);
			PixelMediaFilter(dataPtrBase, dataPtrCopyBase, 0, height, width, height, line, nChan, 1);
			PixelMediaFilter(dataPtrBase, dataPtrCopyBase, width, height, width, height, line, nChan, 1);
		}

		private static unsafe void PixelMediaFilter(byte* imgPointer, byte* imgCopyPointer, int x, int y, int maxWidth, int maxHeight, int line, int nChan, int padding)
		{
			int i, j, newX, newY;
			int from = -padding;

			int b = 0;
			int g = 0;
			int r = 0;
			for (i = from; i <= padding; i++)
			{
				newY = Clamp(y + i, 0, maxHeight) * line;
				for (j = from; j <= padding; j++)
				{
					newX = Clamp(x + j, 0, maxWidth);
					b += *(imgCopyPointer + newY + newX * nChan);
					g += *(imgCopyPointer + newY + newX * nChan + 1);
					r += *(imgCopyPointer + newY + newX * nChan + 2);
				}
			}

			int size = padding * 2 + 1;
			b = (int)Math.Round(b / (double)(size * size));
			g = (int)Math.Round(g / (double)(size * size));
			r = (int)Math.Round(r / (double)(size * size));
			*(imgPointer + y * line + x * nChan) = (byte)b;
			*(imgPointer + y * line + x * nChan + 1) = (byte)g;
			*(imgPointer + y * line + x * nChan + 2) = (byte)r;
		}

		public static unsafe void Mean2Blur(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
		{
			MIplImage m = img.MIplImage;
			MIplImage mCopy = imgCopy.MIplImage;

			byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
			byte* dataPtrCopy = (byte*)mCopy.imageData.ToPointer();

			int nChan = m.nChannels; // number of channels - 3
			int padding = m.widthStep - m.nChannels * (m.width - 2); // alinhament bytes (padding)
			int line = m.widthStep;
			int width = m.width - 1;
			int height = m.height - 1;

			byte* copyPixel;
			int x, y;
			int b, g, r;

			dataPtrCopy += nChan + line;
			dataPtr += nChan + line;
			for (y = 1; y < height; y++)
			{
				for (x = 1; x < width; x++)
				{
					copyPixel = dataPtrCopy;

					b = *(copyPixel - nChan);
					g = *(copyPixel - 2);
					r = *(copyPixel - 1);

					b += *(copyPixel);
					g += *(copyPixel + 1);
					r += *(copyPixel + 2);

					b += *(copyPixel + nChan);
					g += *(copyPixel + 4);
					r += *(copyPixel + 5);


					copyPixel = dataPtrCopy - line;
					b += *(copyPixel);
					g += *(copyPixel + 1);
					r += *(copyPixel + 2);

					copyPixel = dataPtrCopy + line;
					b += *(copyPixel);
					g += *(copyPixel + 1);
					r += *(copyPixel + 2);

					*(dataPtr++) = (byte)(b / 5.0);
					*(dataPtr++) = (byte)(g / 5.0);
					*(dataPtr++) = (byte)(r / 5.0);

					dataPtrCopy += nChan;
				}
				dataPtr += padding;
				dataPtrCopy += padding;
			}


		}

		public static unsafe void Mean(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
		{
			MeanSolutionA(img, imgCopy);
		}

		public static unsafe void MeanSolutionC(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
		{
			MIplImage m = img.MIplImage;
			MIplImage mCopy = imgCopy.MIplImage;
			byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
			byte* dataPtrCopy = (byte*)mCopy.imageData.ToPointer(); // Pointer to the image copy
			int[,,] matrix = new int[img.Height, img.Width, 3];
			int nChan = m.nChannels; // number of channels - 3
			int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
			int line = m.widthStep;
			int width = m.width;
			int height = m.height;

			byte* currentPixel;
			byte* prevPivot, NextPivot, prevPivot1, NextPivot1;
			int x, y;
			int mWidth = width * 3;

			//(0,0)
			fixed (int* pMatrix = &matrix[0, 0, 0])
			{
				pMatrix[0] = dataPtrCopy[0] * 4 + ((dataPtrCopy + nChan)[0] << 1) + ((dataPtrCopy + line)[0] << 1) + (dataPtrCopy + nChan + line)[0];
				pMatrix[1] = dataPtrCopy[1] * 4 + ((dataPtrCopy + nChan)[1] << 1) + ((dataPtrCopy + line)[1] << 1) + (dataPtrCopy + nChan + line)[1];
				pMatrix[2] = dataPtrCopy[2] * 4 + ((dataPtrCopy + nChan)[2] << 1) + ((dataPtrCopy + line)[2] << 1) + (dataPtrCopy + nChan + line)[2];

				dataPtr[0] = (byte)Math.Round(*(pMatrix) / 9.0);
				dataPtr[1] = (byte)Math.Round(*(pMatrix + 1) / 9.0);
				dataPtr[2] = (byte)Math.Round(*(pMatrix + 2) / 9.0);

				int yM, xM;
				//first line
				for (x = 1; x < width; x++)
				{
					xM = x - 1;
					prevPivot = dataPtrCopy + Math.Max(x - 2, 0) * nChan;
					prevPivot1 = dataPtrCopy + Math.Max(x - 2, 0) * nChan + line;
					NextPivot = dataPtrCopy + Math.Min(x + 1, width - 1) * nChan;
					NextPivot1 = dataPtrCopy + Math.Min(x + 1, width - 1) * nChan + line;

					(pMatrix + x * nChan)[0] = (pMatrix + xM * nChan)[0] - (prevPivot[0] << 1) - prevPivot1[0] + (NextPivot[0] << 1) + NextPivot1[0];
					(pMatrix + x * nChan)[1] = (pMatrix + xM * nChan)[1] - (prevPivot[1] << 1) - prevPivot1[1] + (NextPivot[1] << 1) + NextPivot1[1];
					(pMatrix + x * nChan)[2] = (pMatrix + xM * nChan)[2] - (prevPivot[2] << 1) - prevPivot1[2] + (NextPivot[2] << 1) + NextPivot1[2];

					currentPixel = dataPtr + x * nChan;
					currentPixel[0] = (byte)Math.Round(*(pMatrix + x * nChan) / 9.0);
					currentPixel[1] = (byte)Math.Round(*(pMatrix + x * nChan + 1) / 9.0);
					currentPixel[2] = (byte)Math.Round(*(pMatrix + x * nChan + 2) / 9.0);
				}
				//first col
				for (y = 1; y < height; y++)
				{
					yM = y - 1;
					prevPivot = dataPtrCopy + Math.Max(y - 2, 0) * line;
					prevPivot1 = dataPtrCopy + Math.Max(y - 2, 0) * line + nChan;
					NextPivot = dataPtrCopy + Math.Min(y + 1, height - 1) * line;
					NextPivot1 = dataPtrCopy + Math.Min(y + 1, height - 1) * line + nChan;

					*(pMatrix + y * mWidth) = *(pMatrix + yM * mWidth) - (prevPivot[0] << 1) - prevPivot1[0] + (NextPivot[0] << 1) + NextPivot1[0];
					*(pMatrix + y * mWidth + 1) = *(pMatrix + yM * mWidth + 1) - (prevPivot[1] << 1) - prevPivot1[1] + (NextPivot[1] << 1) + NextPivot1[1];
					*(pMatrix + y * mWidth + 2) = *(pMatrix + yM * mWidth + 2) - (prevPivot[2] << 1) - prevPivot1[2] + (NextPivot[2] << 1) + NextPivot1[2];

					currentPixel = dataPtr + y * line;
					currentPixel[0] = (byte)Math.Round(*(pMatrix + y * mWidth) / 9.0);
					currentPixel[1] = (byte)Math.Round(*(pMatrix + y * mWidth + 1) / 9.0);
					currentPixel[2] = (byte)Math.Round(*(pMatrix + y * mWidth + 2) / 9.0);
				}
				byte* a, b, c, d;
				int aa, bb, cc, dd;

				int v1, v2, v3;
				int* pHelper;
				int* pHelperYM;
				dataPtr += line + nChan;
				for (y = 1; y < height; y++)
				{
					aa = Math.Max(y - 2, 0) * line;
					dd = Math.Min(y + 1, height - 1) * line;
					yM = y - 1;
					for (x = 1; x < width; x++)
					{
						xM = x - 1;
						bb = Math.Max(x - 2, 0) * nChan;
						cc = Math.Min(x + 1, width - 1) * nChan;
						a = dataPtrCopy + aa + bb;
						b = dataPtrCopy + aa + cc;
						c = dataPtrCopy + dd + bb;
						d = dataPtrCopy + dd + cc;

						pHelper = pMatrix + y * mWidth + x * nChan;
						pHelperYM = pHelper - mWidth;

						v1 = *(pHelperYM) - *(pHelperYM - nChan) + *(pHelper - nChan) + *a - *b - *c + *d;
						v2 = *(pHelperYM + 1) - *(pHelperYM - 2) + *(pHelper - 2) + *(a + 1) - *(b + 1) - *(c + 1) + *(d + 1);
						v3 = *(pHelperYM + 2) - *(pHelperYM - 1) + *(pHelper - 1) + *(a + 2) - *(b + 2) - *(c + 2) + *(d + 2);

						*(pHelper) = v1;
						*(pHelper + 1) = v2;
						*(pHelper + 2) = v3;

						dataPtr[0] = (byte)Math.Round(v1 / 9.0);
						dataPtr[1] = (byte)Math.Round(v2 / 9.0);
						dataPtr[2] = (byte)Math.Round(v3 / 9.0);
						dataPtr += nChan;
					}
					dataPtr += padding + nChan;
				}
			}
		}

		public static unsafe void MeanSeparableFilter(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
		{
			MIplImage m = img.MIplImage;
			MIplImage mCopy = imgCopy.MIplImage;

			byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
			byte* dataPtrCopy = (byte*)mCopy.imageData.ToPointer();

			int nChan = m.nChannels; // number of channels - 3
			int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
			int width = m.width;
			int height = m.height;
			int[,,] matrix = PadBorder(img, 1);
			int[,,] mid = new int[height, width, nChan];

			int x, y;
			fixed (int* pMid = &mid[0, 0, 0])
			{
				fixed (int* pMat = &matrix[0, 1, 0])
				{
					int line = (width + 2) * nChan;
					int* m1 = pMid, m2 = pMid + 1, m3 = pMid + 2;
					int* o1 = pMat, o2 = pMat + 1, o3 = pMat + 2;
					for (y = 0; y < height; y++)
					{
						for (x = 0; x < width; x++)
						{

							*m1 += *o1;
							*m1 += *(o1 + line);
							*m1 += *(o1 + (line << 1));
							m1 += nChan;
							o1 += nChan;


							*m2 += *o2;
							*m2 += *(o2 + line);
							*m2 += *(o2 + (line << 1));
							m2 += nChan;
							o2 += nChan;

							*m3 += *o3;
							*m3 += *(o3 + line);
							*m3 += *(o3 + (line << 1));
							o3 += nChan;
							m3 += nChan;
						}
						o1 += nChan << 1;
						o2 += nChan << 1;
						o3 += nChan << 1;
					}
				}

				mid = PadBorder(mid, width, height, nChan, 1);
				int b, g, r;
				for (y = 0; y < height; y++)
				{
					for (x = 0; x < width; x++)
					{
						b = g = r = 0;
						b += mid[y + 1, x, 0];
						g += mid[y + 1, x, 1];
						r += mid[y + 1, x, 2];

						b += mid[y + 1, x + 1, 0];
						g += mid[y + 1, x + 1, 1];
						r += mid[y + 1, x + 1, 2];

						b += mid[y + 1, x + 2, 0];
						g += mid[y + 1, x + 2, 1];
						r += mid[y + 1, x + 2, 2];

						dataPtr[0] = (byte)Math.Round(b / 9.0);
						dataPtr[1] = (byte)Math.Round(g / 9.0);
						dataPtr[2] = (byte)Math.Round(r / 9.0);

						dataPtr += nChan;
					}
					dataPtr += padding;
				}
			}
		}
		public static unsafe Image<Gray, int> ConnectedComponenets(Image<Bgr, Byte> img)
		{


			Image<Gray, int> connectedImg = new Image<Gray, int>(img.Width, img.Height, new Gray(int.MaxValue));
			MIplImage m = img.MIplImage;
			MIplImage mConnected = connectedImg.MIplImage;
			byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
			int* dataPtrConnected = (int*)connectedImg.MIplImage.imageData.ToPointer(); // Pointer to the connected components img

			int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
			int line = m.widthStep;
			int width = m.width;
			int height = m.height;
			int line2 = mConnected.widthStep / 4;

			//int backColor = *(int*)dataPtr & 0x00ffffff;

			byte[] back = { dataPtr[0], dataPtr[1], dataPtr[2] };

			Dictionary<int, int> collisions = new Dictionary<int, int>();
			int nLabel = 0;
			int x, y, value = 0, value2 = 0;
			int* newPixel, left, top;

			int yAdd = 0;
			int neighboursCount = 0;
			for (y = 0; y < height; y++)
			{
				if (y > 0)
					yAdd = -line2;
				for (x = 0; x < width; x++)
				{
					//f ((*(int*)(dataPtr) & 0x00ffffff) == backColor)
					if (*dataPtr == back[0] && dataPtr[1] == back[1] && dataPtr[2] == back[2])
					{
						dataPtr += 3;
						continue;
					}
					dataPtr += 3;
					newPixel = dataPtrConnected + x;

					if (x > 0)
						left = newPixel - 1;
					else
						left = newPixel;

					top = newPixel + yAdd;
					if (*left != int.MaxValue)
					{
						value = *left;
						neighboursCount++;
					}
					if (*top != int.MaxValue)
					{
						value = *top;
						neighboursCount++;
					}

					if (neighboursCount == 2 && *top != *left)
					{
						if (collisions[*top] < collisions[*left])
						{
							value2 = collisions[*top];
							collisions[*left] = value2;
							collisions[value] = value2;
						}
						else
						{
							value2 = collisions[*left];
							collisions[*top] = value2;
							collisions[value] = value2;
						}
						*newPixel = value2;
					}
					else if (neighboursCount == 0)
					{
						*newPixel = nLabel;
						collisions[nLabel] = nLabel++;
					}
					else
					{
						*newPixel = value;
					}
					neighboursCount = 0;
				}
				dataPtr += padding;
				dataPtrConnected += line2;
			}
			//collisions[3] = 256 * 256;
			//second pass
			dataPtrConnected = (int*)connectedImg.MIplImage.imageData.ToPointer(); // Pointer to the connected components img
			for (y = 0; y < height; y++)
			{
				for (x = 0; x < width; x++)
				{
					newPixel = dataPtrConnected + x;
					if (*newPixel != int.MaxValue)
						*newPixel = collisions[*newPixel];
				}
				dataPtrConnected += line2;
			}


			//List<int> unique = collisions.Values.Distinct().ToList();
			return connectedImg;
		}

		public static unsafe void NonUniform(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float[,] matrix, float matrixWeight)
		{
			if (matrixWeight == 0)
				matrixWeight = 1f;
			MIplImage m = img.MIplImage;
			MIplImage mCopy = imgCopy.MIplImage;

			byte* dataPtrBase = (byte*)m.imageData.ToPointer(); // Pointer to the image
			byte* dataPtrCopyBase = (byte*)mCopy.imageData.ToPointer();

			int nChan = m.nChannels; // number of channels - 3
			int padding = m.widthStep - m.nChannels * (m.width - 2); // alinhament bytes (padding)
			int line = m.widthStep;
			int width = m.width - 1;
			int height = m.height - 1;

			byte* copyPixel;
			int x, y;
			float b, g, r;
			float[] lookup;
			fixed (float* kernel = &matrix[0, 0])
			{
				lookup = new float[9 * 256];
				for (int i = 0; i < 9; i++)
				{
					int temp = i * 256;
					for (int j = 0; j < 256; j++)
					{
						lookup[temp + j] = j * kernel[i] / matrixWeight;
					}
				}
			}

			float* lookup0, lookup1, lookup2, lookup3, lookup4, lookup5, lookup6, lookup7, lookup8;
			fixed (float* pLookup = &lookup[0])
			{
				lookup0 = pLookup;
				lookup1 = pLookup + 256;
				lookup2 = pLookup + 256 * 2;
				lookup3 = pLookup + 256 * 3;
				lookup4 = pLookup + 256 * 4;
				lookup5 = pLookup + 256 * 5;
				lookup6 = pLookup + 256 * 6;
				lookup7 = pLookup + 256 * 7;
				lookup8 = pLookup + 256 * 8;
			}

			byte* dataPtrCopy = dataPtrCopyBase + nChan + line;
			byte* dataPtr = dataPtrBase + nChan + line;
			//center
			for (y = 1; y < height; y++)
			{
				for (x = 1; x < width; x++)
				{
					copyPixel = dataPtrCopy;

					b = lookup3[*(copyPixel - 3)] + lookup4[*copyPixel] + lookup2[*(copyPixel + 3 - line)] + lookup1[*(copyPixel - line)] + lookup5[*(copyPixel + 3)] + lookup6[*(copyPixel - 3 + line)] + lookup7[*(copyPixel + line)] + lookup8[*(copyPixel + 3 + line)] + lookup0[*(copyPixel - 3 - line)];
					g = lookup3[*(copyPixel - 2)] + lookup4[*(copyPixel + 1)] + lookup2[*(copyPixel + 4 - line)] + lookup1[*(copyPixel + 1 - line)] + lookup5[*(copyPixel + 4)] + lookup6[*(copyPixel - 2 + line)] + lookup7[*(copyPixel + 1 + line)] + lookup8[*(copyPixel + 4 + line)] + lookup0[*(copyPixel - 2 - line)];
					r = lookup3[*(copyPixel - 1)] + lookup4[*(copyPixel + 2)] + lookup2[*(copyPixel + 5 - line)] + lookup1[*(copyPixel + 2 - line)] + lookup5[*(copyPixel + 5)] + lookup6[*(copyPixel - 1 + line)] + lookup7[*(copyPixel + 2 + line)] + lookup8[*(copyPixel + 5 + line)] + lookup0[*(copyPixel - 1 - line)];

					*(dataPtr++) = (byte)((b >= 255) ? 255 : (b <= 0) ? 0 : Math.Round(b));
					*(dataPtr++) = (byte)((g >= 255) ? 255 : (g <= 0) ? 0 : Math.Round(g));
					*(dataPtr++) = (byte)((r >= 255) ? 255 : (r <= 0) ? 0 : Math.Round(r));

					dataPtrCopy += 3;

				}
				dataPtr += padding;
				dataPtrCopy += padding;
			}

			//top
			dataPtrCopy = dataPtrCopyBase + nChan;
			dataPtr = dataPtrBase + nChan;
			for (x = 1; x < width; x++)
			{
				copyPixel = dataPtrCopy;

				b = lookup3[*(copyPixel - nChan)];
				g = lookup3[*(copyPixel - 2)];
				r = lookup3[*(copyPixel - 1)];

				b += lookup4[*copyPixel];
				g += lookup4[*(copyPixel + 1)];
				r += lookup4[*(copyPixel + 2)];

				b += lookup5[*(copyPixel + nChan)];
				g += lookup5[*(copyPixel + 4)];
				r += lookup5[*(copyPixel + 5)];

				//second
				copyPixel = dataPtrCopy + line;

				b += lookup6[*(copyPixel - nChan)];
				g += lookup6[*(copyPixel - 2)];
				r += lookup6[*(copyPixel - 1)];

				b += lookup7[*copyPixel];
				g += lookup7[*(copyPixel + 1)];
				r += lookup7[*(copyPixel + 2)];

				b += lookup8[*(copyPixel + nChan)];
				g += lookup8[*(copyPixel + 4)];
				r += lookup8[*(copyPixel + 5)];

				//third
				copyPixel = dataPtrCopy;

				b += lookup0[*(copyPixel - nChan)];
				g += lookup0[*(copyPixel - 2)];
				r += lookup0[*(copyPixel - 1)];

				b += lookup1[*copyPixel];
				g += lookup1[*(copyPixel + 1)];
				r += lookup1[*(copyPixel + 2)];

				b += lookup2[*(copyPixel + nChan)];
				g += lookup2[*(copyPixel + 4)];
				r += lookup2[*(copyPixel + 5)];

				*(dataPtr++) = (byte)((b >= 255) ? 255 : (b <= 0) ? 0 : Math.Round(b));
				*(dataPtr++) = (byte)((g >= 255) ? 255 : (g <= 0) ? 0 : Math.Round(g));
				*(dataPtr++) = (byte)((r >= 255) ? 255 : (r <= 0) ? 0 : Math.Round(r));

				dataPtrCopy += nChan;
			}

			//bottom
			dataPtrCopy = dataPtrCopyBase + (height) * line + nChan;
			dataPtr = dataPtrBase + (height) * line + nChan;
			for (x = 1; x < width; x++)
			{
				copyPixel = dataPtrCopy;

				b = lookup3[*(copyPixel - nChan)];
				g = lookup3[*(copyPixel - 2)];
				r = lookup3[*(copyPixel - 1)];

				b += lookup4[*copyPixel];
				g += lookup4[*(copyPixel + 1)];
				r += lookup4[*(copyPixel + 2)];

				b += lookup5[*(copyPixel + nChan)];
				g += lookup5[*(copyPixel + 4)];
				r += lookup5[*(copyPixel + 5)];

				//second
				copyPixel = dataPtrCopy;

				b += lookup6[*(copyPixel - nChan)];
				g += lookup6[*(copyPixel - 2)];
				r += lookup6[*(copyPixel - 1)];

				b += lookup7[*copyPixel];
				g += lookup7[*(copyPixel + 1)];
				r += lookup7[*(copyPixel + 2)];

				b += lookup8[*(copyPixel + nChan)];
				g += lookup8[*(copyPixel + 4)];
				r += lookup8[*(copyPixel + 5)];

				//third
				copyPixel = dataPtrCopy - line;

				b += lookup0[*(copyPixel - nChan)];
				g += lookup0[*(copyPixel - 2)];
				r += lookup0[*(copyPixel - 1)];

				b += lookup1[*copyPixel];
				g += lookup1[*(copyPixel + 1)];
				r += lookup1[*(copyPixel + 2)];

				b += lookup2[*(copyPixel + nChan)];
				g += lookup2[*(copyPixel + 4)];
				r += lookup2[*(copyPixel + 5)];

				*(dataPtr++) = (byte)((b >= 255) ? 255 : (b <= 0) ? 0 : Math.Round(b));
				*(dataPtr++) = (byte)((g >= 255) ? 255 : (g <= 0) ? 0 : Math.Round(g));
				*(dataPtr++) = (byte)((r >= 255) ? 255 : (r <= 0) ? 0 : Math.Round(r));

				dataPtrCopy += nChan;
			}

			//left
			dataPtrCopy = dataPtrCopyBase + line;
			dataPtr = dataPtrBase + line;
			for (y = 1; y < height; y++)
			{
				copyPixel = dataPtrCopy;

				b = lookup3[*copyPixel];
				g = lookup3[*(copyPixel + 1)];
				r = lookup3[*(copyPixel + 2)];

				b += lookup4[*copyPixel];
				g += lookup4[*(copyPixel + 1)];
				r += lookup4[*(copyPixel + 2)];

				b += lookup5[*(copyPixel + nChan)];
				g += lookup5[*(copyPixel + 4)];
				r += lookup5[*(copyPixel + 5)];

				//second
				copyPixel = dataPtrCopy + line;

				b += lookup6[*copyPixel];
				g += lookup6[*(copyPixel + 1)];
				r += lookup6[*(copyPixel + 2)];

				b += lookup7[*copyPixel];
				g += lookup7[*(copyPixel + 1)];
				r += lookup7[*(copyPixel + 2)];

				b += lookup8[*(copyPixel + nChan)];
				g += lookup8[*(copyPixel + 4)];
				r += lookup8[*(copyPixel + 5)];

				//third
				copyPixel = dataPtrCopy - line;

				b += lookup0[*copyPixel];
				g += lookup0[*(copyPixel + 1)];
				r += lookup0[*(copyPixel + 2)];

				b += lookup1[*copyPixel];
				g += lookup1[*(copyPixel + 1)];
				r += lookup1[*(copyPixel + 2)];

				b += lookup2[*(copyPixel + nChan)];
				g += lookup2[*(copyPixel + 4)];
				r += lookup2[*(copyPixel + 5)];

				*(dataPtr) = (byte)((b >= 255) ? 255 : (b <= 0) ? 0 : Math.Round(b));
				*(dataPtr + 1) = (byte)((g >= 255) ? 255 : (g <= 0) ? 0 : Math.Round(g));
				*(dataPtr + 2) = (byte)((r >= 255) ? 255 : (r <= 0) ? 0 : Math.Round(r));

				dataPtr += line;
				dataPtrCopy += line;
			}

			//right
			dataPtrCopy = dataPtrCopyBase + width * nChan + line;
			dataPtr = dataPtrBase + width * nChan + line;
			for (y = 1; y < height; y++)
			{
				copyPixel = dataPtrCopy;

				b = lookup3[*(copyPixel - nChan)];
				g = lookup3[*(copyPixel - 2)];
				r = lookup3[*(copyPixel - 1)];

				b += lookup4[*copyPixel];
				g += lookup4[*(copyPixel + 1)];
				r += lookup4[*(copyPixel + 2)];

				b += lookup5[*copyPixel];
				g += lookup5[*(copyPixel + 1)];
				r += lookup5[*(copyPixel + 2)];

				//second
				copyPixel = dataPtrCopy + line;

				b += lookup6[*(copyPixel - nChan)];
				g += lookup6[*(copyPixel - 2)];
				r += lookup6[*(copyPixel - 1)];

				b += lookup7[*copyPixel];
				g += lookup7[*(copyPixel + 1)];
				r += lookup7[*(copyPixel + 2)];

				b += lookup8[*copyPixel];
				g += lookup8[*(copyPixel + 1)];
				r += lookup8[*(copyPixel + 2)];

				//third
				copyPixel = dataPtrCopy - line;

				b += lookup0[*(copyPixel - nChan)];
				g += lookup0[*(copyPixel - 2)];
				r += lookup0[*(copyPixel - 1)];

				b += lookup1[*copyPixel];
				g += lookup1[*(copyPixel + 1)];
				r += lookup1[*(copyPixel + 2)];

				b += lookup2[*copyPixel];
				g += lookup2[*(copyPixel + 1)];
				r += lookup2[*(copyPixel + 2)];

				*(dataPtr) = (byte)((b >= 255) ? 255 : (b <= 0) ? 0 : Math.Round(b));
				*(dataPtr + 1) = (byte)((g >= 255) ? 255 : (g <= 0) ? 0 : Math.Round(g));
				*(dataPtr + 2) = (byte)((r >= 255) ? 255 : (r <= 0) ? 0 : Math.Round(r));

				dataPtr += line;
				dataPtrCopy += line;
			}
			//cantos
			PixelNonUniformFilter(dataPtrBase, dataPtrCopyBase, 0, 0, width, height, line, nChan, 1, matrix, matrixWeight);
			PixelNonUniformFilter(dataPtrBase, dataPtrCopyBase, width, 0, width, height, line, nChan, 1, matrix, matrixWeight);
			PixelNonUniformFilter(dataPtrBase, dataPtrCopyBase, 0, height, width, height, line, nChan, 1, matrix, matrixWeight);
			PixelNonUniformFilter(dataPtrBase, dataPtrCopyBase, width, height, width, height, line, nChan, 1, matrix, matrixWeight);
		}

		private static unsafe void PixelNonUniformFilter(byte* imgPointer, byte* imgCopyPointer, int x, int y, int maxWidth, int maxHeight, int line, int nChan, int padding, float[,] matrix, float weight)
		{
			int i, j, newX, newY;
			int from = -padding;

			float b = 0;
			float g = 0;
			float r = 0;
			int counter = 0;
			fixed (float* kernel = &matrix[0, 0])
			{
				for (i = from; i <= padding; i++)
				{
					newY = Clamp(y + i, 0, maxHeight) * line;
					for (j = from; j <= padding; j++)
					{
						newX = Clamp(x + j, 0, maxWidth);
						b += *(imgCopyPointer + newY + newX * nChan) * kernel[counter];
						g += *(imgCopyPointer + newY + newX * nChan + 1) * kernel[counter];
						r += *(imgCopyPointer + newY + newX * nChan + 2) * kernel[counter];
						counter++;
					}
				}
			}

			b = Clamp((int)Math.Round(b / weight));
			g = Clamp((int)Math.Round(g / weight));
			r = Clamp((int)Math.Round(r / weight));
			*(imgPointer + y * line + x * nChan) = (byte)b;
			*(imgPointer + y * line + x * nChan + 1) = (byte)g;
			*(imgPointer + y * line + x * nChan + 2) = (byte)r;
		}

		public static unsafe void Sobel(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
		{
			MIplImage m = img.MIplImage;
			MIplImage mCopy = imgCopy.MIplImage;

			byte* dataPtrBase = (byte*)m.imageData.ToPointer(); // Pointer to the image
			byte* dataPtrCopyBase = (byte*)mCopy.imageData.ToPointer();

			int nChan = m.nChannels; // number of channels - 3
			int padding = m.widthStep - m.nChannels * (m.width - 2); // alinhament bytes (padding)
			int line = m.widthStep;
			int width = (m.width - 1);
			int height = m.height - 1;

			int x, y;
			int value;

			int[] lookup = new int[2048];
			int v;
			for (int i = 0; i < 2048; i++)
			{
				v = i - 1024;
				lookup[i] = Math.Min(255, Math.Abs(v));
			}

			fixed (int* pLookup = &lookup[1024])
			{

				byte* dataPtrCopy = dataPtrCopyBase + nChan + line;
				byte* dataPtr = dataPtrBase + nChan + line;
				//center
				for (y = 1; y < height; y++)
				{
					for (x = 1; x < width; x++)
					{
						*(dataPtr++) = (byte)pLookup[
							pLookup[dataPtrCopy[-line - nChan] + dataPtrCopy[-line + nChan] - dataPtrCopy[line - nChan] - dataPtrCopy[line + nChan] + ((dataPtrCopy[-line] - dataPtrCopy[line]) << 1)] +
							pLookup[dataPtrCopy[-line + nChan] + dataPtrCopy[line + nChan] - dataPtrCopy[-line - nChan] - dataPtrCopy[line - nChan] + ((dataPtrCopy[nChan] - dataPtrCopy[-nChan]) << 1)]];

						dataPtrCopy++;

						*(dataPtr++) = (byte)pLookup[
						pLookup[dataPtrCopy[-line - nChan] + dataPtrCopy[-line + nChan] - dataPtrCopy[line - nChan] - dataPtrCopy[line + nChan] + ((dataPtrCopy[-line] - dataPtrCopy[line]) << 1)] +
						pLookup[dataPtrCopy[-line + nChan] + dataPtrCopy[line + nChan] - dataPtrCopy[-line - nChan] - dataPtrCopy[line - nChan] + ((dataPtrCopy[nChan] - dataPtrCopy[-nChan]) << 1)]];

						dataPtrCopy++;

						*(dataPtr++) = (byte)pLookup[
							pLookup[dataPtrCopy[-line - nChan] + dataPtrCopy[-line + nChan] - dataPtrCopy[line - nChan] - dataPtrCopy[line + nChan] + ((dataPtrCopy[-line] - dataPtrCopy[line]) << 1)] +
							pLookup[dataPtrCopy[-line + nChan] + dataPtrCopy[line + nChan] - dataPtrCopy[-line - nChan] - dataPtrCopy[line - nChan] + ((dataPtrCopy[nChan] - dataPtrCopy[-nChan]) << 1)]];

						dataPtrCopy++;
					}
					dataPtr += padding;
					dataPtrCopy += padding;
				}

				//top
				dataPtrCopy = dataPtrCopyBase + nChan;
				dataPtr = dataPtrBase + nChan;
				for (x = 1; x < width; x++)
				{
					*(dataPtr++) = (byte)pLookup[
							pLookup[dataPtrCopy[-nChan] + dataPtrCopy[nChan] - dataPtrCopy[line - nChan] - dataPtrCopy[line + nChan] + 2 * (dataPtrCopy[0] - dataPtrCopy[line])] +
							pLookup[dataPtrCopy[nChan] + dataPtrCopy[line + nChan] - dataPtrCopy[-nChan] - dataPtrCopy[line - nChan] + 2 * (dataPtrCopy[nChan] - dataPtrCopy[-nChan])]];

					dataPtrCopy++;
					*(dataPtr++) = (byte)pLookup[
							pLookup[dataPtrCopy[-nChan] + dataPtrCopy[nChan] - dataPtrCopy[line - nChan] - dataPtrCopy[line + nChan] + 2 * (dataPtrCopy[0] - dataPtrCopy[line])] +
							pLookup[dataPtrCopy[nChan] + dataPtrCopy[line + nChan] - dataPtrCopy[-nChan] - dataPtrCopy[line - nChan] + 2 * (dataPtrCopy[nChan] - dataPtrCopy[-nChan])]];

					dataPtrCopy++;
					*(dataPtr++) = (byte)pLookup[
							pLookup[dataPtrCopy[-nChan] + dataPtrCopy[nChan] - dataPtrCopy[line - nChan] - dataPtrCopy[line + nChan] + 2 * (dataPtrCopy[0] - dataPtrCopy[line])] +
							pLookup[dataPtrCopy[nChan] + dataPtrCopy[line + nChan] - dataPtrCopy[-nChan] - dataPtrCopy[line - nChan] + 2 * (dataPtrCopy[nChan] - dataPtrCopy[-nChan])]];

					dataPtrCopy++;
				}

				//bottom
				dataPtrCopy = dataPtrCopyBase + (height) * line + nChan;
				dataPtr = dataPtrBase + (height) * line + nChan;
				for (x = 1; x < width; x++)
				{
					value = pLookup[
							pLookup[dataPtrCopy[-line - nChan] + dataPtrCopy[-line + nChan] - dataPtrCopy[-nChan] - dataPtrCopy[nChan] + 2 * (dataPtrCopy[-line] - dataPtrCopy[0])] +
							pLookup[dataPtrCopy[-line + nChan] + dataPtrCopy[nChan] - dataPtrCopy[-line - nChan] - dataPtrCopy[-nChan] + 2 * (dataPtrCopy[nChan] - dataPtrCopy[-nChan])]];

					*(dataPtr++) = (byte)value;
					dataPtrCopy++;
					value = pLookup[
							pLookup[dataPtrCopy[-line - nChan] + dataPtrCopy[-line + nChan] - dataPtrCopy[-nChan] - dataPtrCopy[nChan] + 2 * (dataPtrCopy[-line] - dataPtrCopy[0])] +
							pLookup[dataPtrCopy[-line + nChan] + dataPtrCopy[nChan] - dataPtrCopy[-line - nChan] - dataPtrCopy[-nChan] + 2 * (dataPtrCopy[nChan] - dataPtrCopy[-nChan])]];

					*(dataPtr++) = (byte)value;
					dataPtrCopy++;
					value = pLookup[
							pLookup[dataPtrCopy[-line - nChan] + dataPtrCopy[-line + nChan] - dataPtrCopy[-nChan] - dataPtrCopy[nChan] + 2 * (dataPtrCopy[-line] - dataPtrCopy[0])] +
							pLookup[dataPtrCopy[-line + nChan] + dataPtrCopy[nChan] - dataPtrCopy[-line - nChan] - dataPtrCopy[-nChan] + 2 * (dataPtrCopy[nChan] - dataPtrCopy[-nChan])]];

					*(dataPtr++) = (byte)value;
					dataPtrCopy++;
				}

				//left
				dataPtrCopy = dataPtrCopyBase + line;
				dataPtr = dataPtrBase + line;
				for (y = 1; y < height; y++)
				{
					value = pLookup[
						pLookup[dataPtrCopy[-line] + dataPtrCopy[-line + nChan] - dataPtrCopy[line] - dataPtrCopy[line + nChan] + 2 * (dataPtrCopy[-line] - dataPtrCopy[line])] +
						pLookup[dataPtrCopy[-line + nChan] + dataPtrCopy[line + nChan] - dataPtrCopy[-line] - dataPtrCopy[line] + 2 * (dataPtrCopy[nChan] - dataPtrCopy[0])]];

					*(dataPtr) = (byte)value;
					dataPtrCopy++;

					value = pLookup[
						pLookup[dataPtrCopy[-line] + dataPtrCopy[-line + nChan] - dataPtrCopy[line] - dataPtrCopy[line + nChan] + 2 * (dataPtrCopy[-line] - dataPtrCopy[line])] +
						pLookup[dataPtrCopy[-line + nChan] + dataPtrCopy[line + nChan] - dataPtrCopy[-line] - dataPtrCopy[line] + 2 * (dataPtrCopy[nChan] - dataPtrCopy[0])]];

					*(dataPtr + 1) = (byte)value;
					dataPtrCopy++;
					value = pLookup[
						pLookup[dataPtrCopy[-line] + dataPtrCopy[-line + nChan] - dataPtrCopy[line] - dataPtrCopy[line + nChan] + 2 * (dataPtrCopy[-line] - dataPtrCopy[line])] +
						pLookup[dataPtrCopy[-line + nChan] + dataPtrCopy[line + nChan] - dataPtrCopy[-line] - dataPtrCopy[line] + 2 * (dataPtrCopy[nChan] - dataPtrCopy[0])]];

					*(dataPtr + 2) = (byte)value;
					dataPtrCopy++;

					dataPtr += line;
					dataPtrCopy += line - nChan;
				}

				//right
				dataPtrCopy = dataPtrCopyBase + (m.width - 1) * nChan + line;
				dataPtr = dataPtrBase + (m.width - 1) * nChan + line;
				for (y = 1; y < height; y++)
				{

					value = pLookup[
						pLookup[dataPtrCopy[-line - nChan] + dataPtrCopy[-line] - dataPtrCopy[line - nChan] - dataPtrCopy[line] + 2 * (dataPtrCopy[-line] - dataPtrCopy[line])] +
						pLookup[dataPtrCopy[-line] + dataPtrCopy[line] - dataPtrCopy[-line - nChan] - dataPtrCopy[line - nChan] + 2 * (dataPtrCopy[0] - dataPtrCopy[-nChan])]];

					*(dataPtr) = (byte)value;
					dataPtrCopy++;
					value = pLookup[
						pLookup[dataPtrCopy[-line - nChan] + dataPtrCopy[-line] - dataPtrCopy[line - nChan] - dataPtrCopy[line] + 2 * (dataPtrCopy[-line] - dataPtrCopy[line])] +
						pLookup[dataPtrCopy[-line] + dataPtrCopy[line] - dataPtrCopy[-line - nChan] - dataPtrCopy[line - nChan] + 2 * (dataPtrCopy[0] - dataPtrCopy[-nChan])]];

					*(dataPtr + 1) = (byte)value;
					dataPtrCopy++;
					value = pLookup[
						pLookup[dataPtrCopy[-line - nChan] + dataPtrCopy[-line] - dataPtrCopy[line - nChan] - dataPtrCopy[line] + 2 * (dataPtrCopy[-line] - dataPtrCopy[line])] +
						pLookup[dataPtrCopy[-line] + dataPtrCopy[line] - dataPtrCopy[-line - nChan] - dataPtrCopy[line - nChan] + 2 * (dataPtrCopy[0] - dataPtrCopy[-nChan])]];

					*(dataPtr + 2) = (byte)value;
					dataPtrCopy++;

					dataPtr += line;
					dataPtrCopy += line - nChan;
				}
			}

			//cantos
			PixelCornerSobel(dataPtrBase, dataPtrCopyBase, 0, 0, width, height, line, nChan);
			PixelCornerSobel(dataPtrBase, dataPtrCopyBase, width, 0, width, height, line, nChan);
			PixelCornerSobel(dataPtrBase, dataPtrCopyBase, 0, height, width, height, line, nChan);
			PixelCornerSobel(dataPtrBase, dataPtrCopyBase, width, height, width, height, line, nChan);
		}

		private static unsafe void PixelCornerSobel(byte* imgPointer, byte* imgCopyPointer, int x, int y, int maxWidth, int maxHeight, int line, int nChan)
		{
			byte* dataPtrCopy = imgCopyPointer + y * line + x * nChan;
			byte* dataPtr = imgPointer + y * line + x * nChan;
			int value, c;


			if (x == 0 && y == 0)
			{
				for (c = 0; c < 3; c++)
				{
					value = Math.Min(255,
						Math.Abs(dataPtrCopy[0] + dataPtrCopy[nChan] - dataPtrCopy[line] - dataPtrCopy[line + nChan] + 2 * (dataPtrCopy[0] - dataPtrCopy[line])) +
						Math.Abs(dataPtrCopy[nChan] + dataPtrCopy[line + nChan] - dataPtrCopy[0] - dataPtrCopy[line] + 2 * (dataPtrCopy[nChan] - dataPtrCopy[0])));

					*(dataPtr + c) = (byte)value;
					dataPtrCopy++;
				}
			}
			else if (x == 0)
			{
				for (c = 0; c < 3; c++)
				{
					value = Math.Min(255,
						Math.Abs(dataPtrCopy[-line] + dataPtrCopy[-line + nChan] - dataPtrCopy[0] - dataPtrCopy[nChan] + 2 * (dataPtrCopy[-line] - dataPtrCopy[0])) +
						Math.Abs(dataPtrCopy[-line + nChan] + dataPtrCopy[nChan] - dataPtrCopy[-line] - dataPtrCopy[0] + 2 * (dataPtrCopy[nChan] - dataPtrCopy[0])));

					*(dataPtr + c) = (byte)value;
					dataPtrCopy++;
				}
			}
			else if (y == 0)
			{
				for (c = 0; c < 3; c++)
				{
					value = Math.Min(255,
						Math.Abs(dataPtrCopy[-nChan] + dataPtrCopy[0] - dataPtrCopy[line - nChan] - dataPtrCopy[line] + 2 * (dataPtrCopy[0] - dataPtrCopy[line])) +
						Math.Abs(dataPtrCopy[0] + dataPtrCopy[line] - dataPtrCopy[-nChan] - dataPtrCopy[line - nChan] + 2 * (dataPtrCopy[0] - dataPtrCopy[-nChan])));

					*(dataPtr + c) = (byte)value;
					dataPtrCopy++;
				}
			}
			else
			{
				for (c = 0; c < 3; c++)
				{
					value = Math.Min(255,
						Math.Abs(dataPtrCopy[-line - nChan] + dataPtrCopy[-line] - dataPtrCopy[-nChan] - dataPtrCopy[0] + 2 * (dataPtrCopy[-line] - dataPtrCopy[0])) +
						Math.Abs(dataPtrCopy[-line] + dataPtrCopy[0] - dataPtrCopy[-line - nChan] - dataPtrCopy[-nChan] + 2 * (dataPtrCopy[0] - dataPtrCopy[-nChan])));

					*(dataPtr + c) = (byte)value;
					dataPtrCopy++;
				}
			}


		}

		public static unsafe void Diferentiation(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
		{
			MIplImage m = img.MIplImage;
			MIplImage mCopy = imgCopy.MIplImage;

			byte* dataPtrBase = (byte*)m.imageData.ToPointer(); // Pointer to the image
			byte* dataPtrCopyBase = (byte*)mCopy.imageData.ToPointer();

			int nChan = m.nChannels; // number of channels - 3
			int padding = m.widthStep - m.nChannels * (m.width - 1); // alinhament bytes (padding)
			int line = m.widthStep;
			int width = m.width - 1;
			int height = m.height - 1;

			int x, y;

			int[] lookup = new int[1024];
			int v;
			for (int i = 0; i < 1024; i++)
			{
				v = i - 512;
				lookup[i] = Math.Min(255, Math.Abs(v));
			}
			fixed (int* pLookup = &lookup[512])
			{
				//center & top & left
				byte* dataPtrCopy = dataPtrCopyBase;
				byte* dataPtr = dataPtrBase;
				for (y = 0; y < height; y++)
				{
					for (x = 0; x < width; x++)
					{
						*(dataPtr++) = (byte)pLookup[
							pLookup[dataPtrCopy[0] - dataPtrCopy[line]] +
							pLookup[dataPtrCopy[0] - dataPtrCopy[nChan]]
							];
						dataPtrCopy++;

						*(dataPtr++) = (byte)pLookup[
							pLookup[dataPtrCopy[0] - dataPtrCopy[line]] +
							pLookup[dataPtrCopy[0] - dataPtrCopy[nChan]]
							];
						dataPtrCopy++;

						*(dataPtr++) = (byte)pLookup[
								pLookup[dataPtrCopy[0] - dataPtrCopy[line]] +
								pLookup[dataPtrCopy[0] - dataPtrCopy[nChan]]
								];
						dataPtrCopy++;
					}
					dataPtr += padding;
					dataPtrCopy += padding;
				}

				//right
				dataPtrCopy = dataPtrCopyBase + (width) * nChan;
				dataPtr = dataPtrBase + (width) * nChan;
				for (y = 0; y < height; y++)
				{
					*(dataPtr++) = (byte)pLookup[dataPtrCopy[0] - dataPtrCopy[line]];
					dataPtrCopy++;

					*(dataPtr++) = (byte)pLookup[dataPtrCopy[0] - dataPtrCopy[line]];
					dataPtrCopy++;

					*(dataPtr) = (byte)pLookup[dataPtrCopy[0] - dataPtrCopy[line]];

					dataPtr += line - 2;
					dataPtrCopy += line - 2;
				}

				//bottom
				dataPtrCopy = dataPtrCopyBase + (height) * line;
				dataPtr = dataPtrBase + (height) * line;
				for (x = 0; x < width; x++)
				{
					*(dataPtr++) = (byte)pLookup[dataPtrCopy[0] - dataPtrCopy[nChan]];
					dataPtrCopy++;

					*(dataPtr++) = (byte)pLookup[dataPtrCopy[0] - dataPtrCopy[nChan]];
					dataPtrCopy++;

					*(dataPtr++) = (byte)pLookup[dataPtrCopy[0] - dataPtrCopy[nChan]];
					dataPtrCopy++;
				}

				//canto bottom-right
				dataPtr = dataPtrBase + (height) * line + width * nChan;
				*(dataPtr++) = 0;
				*(dataPtr++) = 0;
				*(dataPtr++) = 0;
			}



		}

		public static unsafe void Median(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
		{
			imgCopy.SmoothMedian(3).CopyTo(img);
		}

		public static unsafe int[] Histogram_Gray(Emgu.CV.Image<Bgr, byte> img)
		{
			MIplImage m = img.MIplImage;
			byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image

			int nChan = m.nChannels; // number of channels - 3
			int padding = m.widthStep - nChan * m.width; // alinhament bytes (padding)

			int[] histogram = new int[256];

			int[] lookup = new int[768];
			for (int i = 0; i < 768; i++)
			{
				lookup[i] = (int)Math.Round(i / 3.0);
			}

			int realWidth = img.Width * nChan;
			byte* lastPixel = dataPtr + m.widthStep * img.Height - padding;
			byte* c;
			for (; dataPtr < lastPixel; dataPtr += padding)
			{
				for (c = dataPtr + realWidth; dataPtr < c; dataPtr += nChan)
				{
					histogram[lookup[*dataPtr + dataPtr[1] + dataPtr[2]]]++;
				}
			}
			return histogram;
		}

		public static unsafe int[,] Histogram_RGB(Emgu.CV.Image<Bgr, byte> img)
		{
			MIplImage m = img.MIplImage;
			byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image

			int nChan = m.nChannels; // number of channels - 3
			int padding = m.widthStep - nChan * m.width; // alinhament bytes (padding)

			int[,] histogram = new int[3, 256];

			int realWidth = img.Width * nChan;
			byte* lastPixel = dataPtr + m.widthStep * img.Height - padding;
			byte* c;
			fixed (int* his = &histogram[0, 0])
			{
				int* his1 = his + 256;
				int* his2 = his + 512;
				for (; dataPtr < lastPixel; dataPtr += padding)
				{
					for (c = dataPtr + realWidth; dataPtr < c; dataPtr += nChan)
					{
						his[dataPtr[0]]++;
						his1[dataPtr[1]]++;
						his2[dataPtr[2]]++;
					}
				}
			}

			return histogram;
		}

		public static unsafe int[,] Histogram_All(Emgu.CV.Image<Bgr, byte> img)
		{
			MIplImage m = img.MIplImage;
			byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image

			int nChan = m.nChannels; // number of channels - 3
			int padding = m.widthStep - nChan * m.width; // alinhament bytes (padding)

			int[,] histogram = new int[4, 256];

			int[] lookup = new int[768];
			for (int i = 0; i < 768; i++)
			{
				lookup[i] = (int)Math.Round(i / 3.0);
			}

			int realWidth = img.Width * nChan;
			byte* lastPixel = dataPtr + m.widthStep * img.Height - padding;
			byte* c;
			fixed (int* his = &histogram[0, 0])
			{
				int* his1 = his + 256;
				int* his2 = his + 512;
				int* his3 = his + 768;
				for (; dataPtr < lastPixel; dataPtr += padding)
				{
					for (c = dataPtr + realWidth; dataPtr < c; dataPtr += nChan)
					{
						his[lookup[*dataPtr + dataPtr[1] + dataPtr[2]]]++;
						his1[*dataPtr]++;
						his2[dataPtr[1]]++;
						his3[dataPtr[2]]++;
					}
				}
			}

			return histogram;
		}

		public static unsafe void ConvertToBW(Emgu.CV.Image<Bgr, byte> img, int threshold)
		{
			MIplImage m = img.MIplImage;
			// simple way
			//Image<Bgr, byte> imgBorders = new Image<Bgr, Byte>(img.Width + 2, img.Height + 2);
			//CvInvoke.cvCopyMakeBorder(img, imgBorders,new System.Drawing.Point(1,1), Emgu.CV.CvEnum.BORDER_TYPE.REPLICATE,new MCvScalar(0));
			byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image

			bool[] lookup = new bool[768];
			for (int i = 0; i < 768; i++)
			{
				lookup[i] = Math.Round(i / 3.0) > threshold ? true : false;
			}

			int nChan = m.nChannels; // number of channels - 3
			int padding = m.widthStep - nChan * m.width; // alinhament bytes (padding)
			int realWidth = img.Width * nChan;
			byte* lastPixel = dataPtr + m.widthStep * img.Height - padding;
			byte* c;
			for (; dataPtr < lastPixel; dataPtr += padding)
			{
				for (c = dataPtr + realWidth; dataPtr < c; dataPtr += nChan)
				{
					if (lookup[*dataPtr + dataPtr[1] + dataPtr[2]])
					{
						*dataPtr = 255;
						dataPtr[1] = 255;
						dataPtr[2] = 255;
					}
					else
					{
						*dataPtr = 0;
						dataPtr[1] = 0;
						dataPtr[2] = 0;
					}

				}
			}
		}

		public static unsafe void ConvertToBW_Otsu(Emgu.CV.Image<Bgr, byte> img)
		{
			int[] histogram = Histogram_Gray(img);
			double[] probHistogram = new double[256];

			int n = img.Width * img.Height;
			if (n == 0)
				return;
			double imgMean = 0;
			for (int i = 0; i < 256; i++)
			{
				probHistogram[i] = histogram[i] / (double)n;
				imgMean += probHistogram[i] * i;
			}

			int bestThreshold = 0;
			double bestVar = 0;

			double q1 = 0, q2 = 1;
			double u1 = 0, u2 = 0, u1Init = 0;
			double tmpVariance;
			double u1MinusU2 = 0;

			for (int threshold = 0; threshold < 256; threshold++)
			{
				u1 = u1Init;
				u2 = (imgMean - (u1 * q1)) / q2;

				u1MinusU2 = u1 - u2;
				tmpVariance = q1 * q2 * (u1MinusU2 * u1MinusU2);

				if (tmpVariance > bestVar)
				{
					if (threshold > 0)
						bestThreshold = threshold - 1;
					else
						bestThreshold = threshold;
					bestVar = tmpVariance;
				}

				u1Init *= q1;

				q1 += probHistogram[threshold];
				q2 -= probHistogram[threshold];
				u1Init += threshold * probHistogram[threshold];

				if (q1 != 0)
					u1Init /= q1;

				if (q2 <= 0)
					break;

			}

			ConvertToBW(img, bestThreshold);
		}
		public static void puzzleRotateOnly(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
		{
			HashSet<int> labels;
			Dictionary<int, int[][]> pieces_vector;
			Dictionary<int, double[]> pieces_angles;

			labels = PiecesInfo(ConnectedComponenets(img), out pieces_vector, out pieces_angles);
			foreach (int l in pieces_vector.Keys)
			{
				if (pieces_angles[l][0] != 0)
					RotateImage(img, pieces_vector[l], (float)pieces_angles[l][0]);
			}
		}

		public static Image<Bgr, byte> puzzle(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, out List<int[]> Pieces_positions, out List<int> Pieces_angle, int level)
		{
			//Image<Bgr, byte> dummyImg = img.Copy();
			Pieces_positions = new List<int[]>();
			//int[] piece_vector = new int[4];
			//Pieces_positions.Add(piece_vector);
			Pieces_angle = new List<int>();
			//Pieces_angle.Add(0); // angle
			//int[][][] pieces = GetPieces(img);

			Dictionary<int, int[][]> pieces_vector;
			Dictionary<int, double[]> pieces_angles;
			HashSet<int> labels;
			DateTime tI = DateTime.Now;
			if (level > 1)
			{
				labels = PiecesInfo(ConnectedComponenets(img), out pieces_vector, out pieces_angles);
				foreach (int[][] pieceCorner in pieces_vector.Values)
				{
					int[] piece_coord = new int[4];
					piece_coord[0] = pieceCorner[TopLeft][XCoordinate];
					piece_coord[1] = pieceCorner[TopRight][YCoordinate];
					piece_coord[2] = piece_coord[0] + pieceCorner[BottomRight][XCoordinate] - pieceCorner[TopLeft][XCoordinate];
					piece_coord[3] = piece_coord[1] + pieceCorner[BottomLeft][YCoordinate] - pieceCorner[TopRight][YCoordinate];
					Pieces_positions.Add(piece_coord);
				}
				foreach (int l in pieces_vector.Keys)
				{
					Pieces_angle.Add(Convert.ToInt32(Math.Round(ConvertRadiansToDegrees(pieces_angles[l][0]))));
					if (pieces_angles[l][0] != 0)
						RotateImage(img, pieces_vector[l], (float)pieces_angles[l][0]);
				}
			}
			else // level 1
			{
				Pieces_angle.Add(0);
				Pieces_angle.Add(0);
			}

			TimeSpan tF = DateTime.Now - tI;
			Console.WriteLine(tF.ToString());
			labels = PiecesInfoNoRot(ConnectedComponenets(img), out pieces_vector);

			if(Pieces_positions.Count==0)
			{
				foreach (int[][] pieceCorner in pieces_vector.Values)
				{
					int[] piece_coord = new int[4];
					piece_coord[0] = pieceCorner[TopLeft][XCoordinate];
					piece_coord[1] = pieceCorner[TopRight][YCoordinate];
					piece_coord[2] = piece_coord[0] + pieceCorner[BottomRight][XCoordinate] - pieceCorner[TopLeft][XCoordinate];
					piece_coord[3] = piece_coord[1] + pieceCorner[BottomLeft][YCoordinate] - pieceCorner[TopRight][YCoordinate];
					Pieces_positions.Add(piece_coord);
				}
			}
			
			List<Image<Bgr, byte>> imgs = ExtractImages(img, labels, pieces_vector);
			int i, j;
			Image<Bgr, byte> imgA, imgB, final;
			while (imgs.Count > 1)
			{
				int lowestDiff = int.MaxValue;
				int imgAIdx = -1;
				int imgBIdx = -1;
				int finalPost = 0;
				float finalScale = 1;
				int pos;
				int diff;
				for (i = 0; i < imgs.Count; i++)
				{
					for (j = 0; j < imgs.Count; j++)
					{
						if (i == j)
							continue;
						float NScale = 1f;
						if (i < j)
						{
							pos = ImageBorderDiff(imgs[i], imgs[j], true, out NScale, out diff);
						}
						else
						{
							pos = ImageBorderDiff(imgs[i], imgs[j], false, out NScale, out diff);
						}
						if (lowestDiff > diff)
						{
							lowestDiff = diff;
							finalPost = pos;
							finalScale = NScale;
							imgAIdx = i;
							imgBIdx = j;
						}
					}
				}
				if (finalScale != 1)
					imgA = GetScaledImg(imgs[imgAIdx], finalScale);
				else
					imgA = imgs[imgAIdx];
				imgB = imgs[imgBIdx];
				switch (finalPost)
				{
					case 0:
						//A em cima de I
						final = GlueImages(imgA, imgB, true);
						imgs.Add(final);
						break;
					case 1:
						//I a esqueda de I
						final = GlueImages(imgA, imgB, false);
						imgs.Add(final);
						break;
					case 2:
						//j em cima de I
						final = GlueImages(imgB, imgA, true);
						imgs.Add(final);
						break;
					case 3:
						//I a esqueda de J
						final = GlueImages(imgB, imgA, false);
						imgs.Add(final);
						break;
				}
				if (imgAIdx > imgBIdx)
				{
					imgs.RemoveAt(imgAIdx);
					imgs.RemoveAt(imgBIdx);
				}
				else
				{
					imgs.RemoveAt(imgBIdx);
					imgs.RemoveAt(imgAIdx);
				}
			}
			return imgs[0];
		}

		private static unsafe HashSet<int> PiecesInfoNoRot(Image<Gray, int> connectedImg, out Dictionary<int, int[][]> pieces_vector)
		{
			MIplImage m = connectedImg.MIplImage;
			int* dataPtr = (int*)m.imageData.ToPointer(); // Pointer to the image
			int width = m.width;
			int height = m.height;

			int line = m.widthStep;
			int padding = m.widthStep / 4 - m.width;

			pieces_vector = new Dictionary<int, int[][]>();

			HashSet<int> labels = new HashSet<int>();

			int x, y;
			int[][] vec;
			for (y = 0; y < height; y++)
			{
				for (x = 0; x < width; x++)
				{
					int label = *dataPtr;
					if (label < int.MaxValue)
					{
						if (!labels.Contains(label))
						{
							labels.Add(label);
							pieces_vector.Add(label, new int[4][]);
							vec = pieces_vector[label];
							vec[0] = new int[] { x, y };
							vec[1] = new int[] { x, y };
							vec[2] = new int[] { x, y };
							vec[3] = new int[] { x, y };
						}
						else
						{
							vec = pieces_vector[label];
							if (x < vec[TopLeft][XCoordinate])// novo top left
							{
								vec[TopLeft][XCoordinate] = x;
								vec[TopLeft][YCoordinate] = y;
							}
							else
							   if (y > vec[BottomLeft][YCoordinate])//novo bottom left
							{
								vec[BottomLeft][XCoordinate] = x;
								vec[BottomLeft][YCoordinate] = y;
							}
							else
							   if (y <= vec[TopRight][YCoordinate])//novo top right
							{
								vec[TopRight][XCoordinate] = x;
								vec[TopRight][YCoordinate] = y;

							}
							else
								if (x >= vec[BottomRight][XCoordinate]) //novo bottom right
							{
								vec[BottomRight][XCoordinate] = x;
								vec[BottomRight][YCoordinate] = y;
							}

						}

					}

					dataPtr++;
				}
				dataPtr += padding;
			}

			return labels;
		}

		private static List<Image<Bgr, byte>> ExtractImages(Image<Bgr, byte> img, HashSet<int> labels, Dictionary<int, int[][]> pieces_vector)
		{
			List<Image<Bgr, byte>> imgs = new List<Image<Bgr, byte>>();
			int width, height, startingX, startingY;
			int[][] piece;
			foreach (int l in labels)
			{
				piece = pieces_vector[l];
				width = piece[BottomRight][XCoordinate] - piece[TopLeft][XCoordinate] + 1;
				height = piece[BottomLeft][YCoordinate] - piece[TopRight][YCoordinate] + 1;
				startingX = piece[TopLeft][XCoordinate];
				startingY = piece[TopRight][YCoordinate];
				img.ROI = new Rectangle(startingX, startingY, width, height);
				imgs.Add(img.Copy());
			}
			return imgs;
		}

		private static Image<Bgr, Byte> GlueImages(Image<Bgr, Byte> image1, Image<Bgr, Byte> image2, bool vertical)
		{
			if (vertical)
			{
				int ImageWidth = Math.Max(image1.Width, image2.Width);
				int ImageHeight = image1.Height + image2.Height;

				//declare new image (large image).
				Image<Bgr, Byte> imageResult = new Image<Bgr, Byte>(ImageWidth, ImageHeight);


				imageResult.ROI = new Rectangle(0, 0, image1.Width, image1.Height);
				image1.CopyTo(imageResult);
				imageResult.ROI = new Rectangle(0, image1.Height, image2.Width, image2.Height);
				image2.CopyTo(imageResult);
				imageResult.ROI = Rectangle.Empty;
				return imageResult;
			}
			else
			{
				int ImageWidth = image1.Width + image2.Width;
				int ImageHeight = Math.Max(image1.Height, image2.Height);

				//declare new image (large image).
				Image<Bgr, Byte> imageResult = new Image<Bgr, Byte>(ImageWidth, ImageHeight);


				imageResult.ROI = new Rectangle(0, 0, image1.Width, image1.Height);
				image1.CopyTo(imageResult);
				imageResult.ROI = new Rectangle(image1.Width, 0, image2.Width, image2.Height);
				image2.CopyTo(imageResult);
				imageResult.ROI = Rectangle.Empty;
				return imageResult;
			}


		}


		public static unsafe int ImageBorderDiff(Image<Bgr, byte> img1, Image<Bgr, byte> img2, bool topBottom, out float needScale, out int diff)
		{
			MIplImage m1 = img1.MIplImage;
			MIplImage m2 = img2.MIplImage;

			int topDiff = 0;
			int botDiff = 0;
			needScale = 1f;
			if (topBottom)
			{
				int width1 = m1.width;
				int width2 = m2.width;

				needScale = (float)width2 / width1;
				if (needScale != 1)
					img1 = GetScaledImg(img1, needScale);

				m1 = img1.MIplImage;
				byte* img1Ptr = (byte*)m1.imageData.ToPointer();
				byte* img1PtrBot = (byte*)m1.imageData.ToPointer() + (m1.height - 1) * m1.widthStep;
				m2 = img2.MIplImage;
				byte* img2Ptr = (byte*)m2.imageData.ToPointer();
				byte* img2PtrBot = (byte*)m2.imageData.ToPointer() + (m2.height - 1) * m2.widthStep;

				for (int i = 1; i < width2 - 1; i++)
				{
					topDiff += (int)Math.Sqrt(Math.Pow(img1PtrBot[0] - img2Ptr[0], 2) + Math.Pow(img1PtrBot[0] - img2Ptr[0], 2) + Math.Pow(img1PtrBot[0] - img2Ptr[0], 2));
					botDiff += (int)Math.Sqrt(Math.Pow(img1Ptr[0] - img2PtrBot[0], 2) + Math.Pow(img1Ptr[1] - img2PtrBot[1], 2) + Math.Pow(img1Ptr[2] - img2PtrBot[2], 2));

					img1Ptr += 3;
					img1PtrBot += 3;
					img2Ptr += 3;
					img2PtrBot += 3;
				}
				topDiff /= width2;
				botDiff /= width2;

				if (topDiff <= botDiff)
				{
					diff = topDiff;
					return 0;
				}
				else
				{
					diff = botDiff;
					return 2;
				}
			}
			else
			{
				int height1 = m1.height;
				int height2 = m2.height;


				needScale = (float)height2 / height1;
				img1 = GetScaledImg(img1, needScale);

				m1 = img1.MIplImage;
				byte* img1Ptr = (byte*)m1.imageData.ToPointer();
				byte* img1PtrRight = img1Ptr + (m1.width - 1) * 3;
				m2 = img2.MIplImage;
				byte* img2Ptr = (byte*)m2.imageData.ToPointer();
				byte* img2PtrRight = img2Ptr + (m2.width - 1) * 3;

				int line1 = m1.widthStep;
				int line2 = m2.widthStep;
				for (int i = 0; i < height2; i++)
				{
					topDiff += (int)Math.Sqrt(Math.Pow(img1Ptr[0] - img2PtrRight[0], 2) + Math.Pow(img1Ptr[1] - img2PtrRight[1], 2) + Math.Pow(img1Ptr[2] - img2PtrRight[2], 2));
					botDiff += (int)Math.Sqrt(Math.Pow(img2Ptr[0] - img1PtrRight[0], 2) + Math.Pow(img2Ptr[1] - img1PtrRight[1], 2) + Math.Pow(img2Ptr[2] - img1PtrRight[2], 2));

					img1Ptr += line1;
					img1PtrRight += line1;
					img2Ptr += line2;
					img2PtrRight += line2;
				}
				topDiff /= height2;
				botDiff /= height2;

				if (topDiff <= botDiff)
				{
					diff = topDiff;
					return 3;
				}
				else
				{
					diff = botDiff;
					return 1;
				}
			}
		}

		public static unsafe Image<Bgr, byte> GetScaledImg(Image<Bgr, byte> img, float scaleFactor)
		{
			//return img.Resize((int)Math.Round(img.Width * scaleFactor), (int)Math.Round(img.Height * scaleFactor), Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
			if (scaleFactor <= 0)
				scaleFactor = 1f;
			MIplImage m = img.MIplImage;
			byte* dataPtrCopy = (byte*)m.imageData.ToPointer(); // Pointer to the image copy
			int maxWidth = m.width;
			int maxHeight = m.height;
			// Pointer to the image


			int nChan = m.nChannels; // number of channels - 3
			int padding = m.widthStep - m.nChannels * m.width;

			int realWidth = img.Width;
			int realHeight = img.Height;
			Image<Bgr, byte> resizedImg = new Image<Bgr, byte>((int)Math.Round(realWidth * scaleFactor), (int)Math.Round(realHeight * scaleFactor));
			MIplImage mCopy = resizedImg.MIplImage;
			byte* dataPtr = (byte*)mCopy.imageData.ToPointer();


			int width = resizedImg.Width;
			int height = resizedImg.Height;

			int[] lookupX = new int[width];
			for (int i = 0; i < width; i++)
			{
				lookupX[i] = (int)Math.Min(Math.Round(i / scaleFactor), maxWidth - 1);
			}

			int[] lookupY = new int[height];
			for (int i = 0; i < height; i++)
			{
				lookupY[i] = (int)Math.Min(Math.Round(i / scaleFactor), maxHeight - 1);
			}

			int x, y, newY;
			byte* newPixel;
			int line = mCopy.widthStep;
			int lineOrg = m.widthStep;
			int linePadding = line - width * nChan;
			for (y = 0; y < height; y++)
			{
				newY = lookupY[y] * lineOrg;
				for (x = 0; x < width; x++)
				{
					newPixel = dataPtrCopy + newY + lookupX[x] * nChan;
					dataPtr[0] = (newPixel)[0];
					dataPtr[1] = (newPixel)[1];
					dataPtr[2] = (newPixel)[2];
					dataPtr += nChan;
				}
				dataPtr += linePadding;
			}

			return resizedImg;

		}

		public static Image<Bgr, byte> puzzle2(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, out List<int[]> Pieces_positions, out List<int> Pieces_angle, int level)
		{
			unsafe
			{
				int[][][] pecas;
				List<int> angulo_pecas = new List<int>();
				List<int[]> posicao_pecas = new List<int[]>();
				int[] posicao_peca0 = new int[8];
				int[] posicao_peca1 = new int[8];
				Image<Bgr, byte> completePuzzle = null;

				int widthAfterRotation, heightAfterRotation;

				Pieces_positions = null;
				Pieces_angle = null;


				pecas = GetPieces(img);

				posicao_peca0[0] = pecas[0][TopLeft][XCoordinate];
				posicao_peca0[1] = pecas[0][TopLeft][YCoordinate];
				posicao_peca0[2] = pecas[0][TopRight][XCoordinate];
				posicao_peca0[3] = pecas[0][TopRight][YCoordinate];
				posicao_peca0[4] = pecas[0][BottomLeft][XCoordinate];
				posicao_peca0[5] = pecas[0][BottomLeft][YCoordinate];
				posicao_peca0[6] = pecas[0][BottomRight][XCoordinate];
				posicao_peca0[7] = pecas[0][BottomRight][YCoordinate];

				posicao_peca1[0] = pecas[1][TopLeft][XCoordinate];
				posicao_peca1[1] = pecas[1][TopLeft][YCoordinate];
				posicao_peca1[2] = pecas[1][TopRight][XCoordinate];
				posicao_peca1[3] = pecas[1][TopRight][YCoordinate];
				posicao_peca1[4] = pecas[1][BottomLeft][XCoordinate];
				posicao_peca1[5] = pecas[1][BottomLeft][YCoordinate];
				posicao_peca1[6] = pecas[1][BottomRight][XCoordinate];
				posicao_peca1[7] = pecas[1][BottomRight][YCoordinate];

				posicao_pecas.Add(posicao_peca0);
				posicao_pecas.Add(posicao_peca1);

				Pieces_positions = posicao_pecas;

				if (level == 1)
				{

					completePuzzle = GetNewImage(img, pecas);

					angulo_pecas.Add(0);
					angulo_pecas.Add(0);
					Pieces_angle = angulo_pecas;

				}
				else if (level == 2)
				{
					// o angulo passado para
					angulo_pecas.Add((int)getPieceAngleDegree(posicao_peca0[0], posicao_peca0[1], posicao_peca0[2], posicao_peca0[3]));
					angulo_pecas.Add((int)getPieceAngleDegree(posicao_peca1[0], posicao_peca1[1], posicao_peca1[2], posicao_peca1[3]));
					Pieces_angle = angulo_pecas;


					GetRotatedImage(img, pecas, getPieceAngle(posicao_peca0[0], posicao_peca0[1], posicao_peca0[2], posicao_peca0[3]), getPieceAngle(posicao_peca1[0], posicao_peca1[1], posicao_peca1[2], posicao_peca1[3]));


					pecas = GetPieces(img);
					widthAfterRotation = Math.Min(pecas[0][TopRight][XCoordinate] - pecas[0][TopLeft][XCoordinate], pecas[1][TopRight][XCoordinate] - pecas[1][TopLeft][XCoordinate]);
					int widthDiff = Math.Abs((pecas[0][TopRight][XCoordinate] - pecas[0][TopLeft][XCoordinate]) - (pecas[1][TopRight][XCoordinate] - pecas[1][TopLeft][XCoordinate]));

					heightAfterRotation = Math.Min(pecas[0][BottomRight][YCoordinate] - pecas[0][TopRight][YCoordinate], pecas[1][BottomRight][YCoordinate] - pecas[1][TopRight][YCoordinate]);
					int heightDiff = Math.Abs((pecas[0][BottomRight][YCoordinate] - pecas[0][TopRight][YCoordinate]) - (pecas[1][BottomRight][YCoordinate] - pecas[1][TopRight][YCoordinate]));

					Console.WriteLine("width do 0 " + (pecas[0][TopRight][XCoordinate] - pecas[0][TopLeft][XCoordinate]));
					Console.WriteLine("width do 1 " + (pecas[1][TopRight][XCoordinate] - pecas[1][TopLeft][XCoordinate]));

					Console.WriteLine(pecas[0][TopLeft][XCoordinate]);

					Console.WriteLine(pecas[1][TopLeft][XCoordinate]);
					Console.WriteLine(pecas[1][TopRight][XCoordinate]);
					Console.WriteLine(pecas[1][BottomLeft][XCoordinate]); // o get pieces do de baixo nao esta a funcionar
					Console.WriteLine(pecas[1][BottomRight][XCoordinate]);

					if (widthDiff < 20)
					{
						pecas[0][TopRight][XCoordinate] = pecas[0][TopLeft][XCoordinate] + widthAfterRotation;
						pecas[0][BottomRight][XCoordinate] = pecas[0][BottomLeft][XCoordinate] + widthAfterRotation;

						pecas[1][TopRight][XCoordinate] = pecas[1][TopLeft][XCoordinate] + widthAfterRotation;
						pecas[1][BottomRight][XCoordinate] = pecas[1][BottomLeft][XCoordinate] + widthAfterRotation;

					}


					if (heightDiff < 20)
					{

						pecas[0][BottomLeft][YCoordinate] = pecas[0][TopLeft][YCoordinate] + heightAfterRotation;
						pecas[0][BottomRight][YCoordinate] = pecas[0][TopRight][YCoordinate] + heightAfterRotation;

						pecas[1][BottomLeft][YCoordinate] = pecas[1][TopLeft][YCoordinate] + heightAfterRotation;
						pecas[1][BottomRight][YCoordinate] = pecas[1][TopRight][YCoordinate] + heightAfterRotation;


					}





					completePuzzle = GetNewImage(img, pecas);
				}



				return completePuzzle;
			}

		}


		public static unsafe Image<Bgr, byte> GetNewImage(Image<Bgr, byte> img, int[][][] imgLimits)
		{
			Console.WriteLine("Tou no GetNewImage");
			MIplImage m = img.MIplImage;
			byte* imgPtr = (byte*)img.MIplImage.imageData.ToPointer(); // Pointer to the connected components img

			int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
															   //int line = mConnected.widthStep / 4;
			int width = m.width;
			int height = m.height;
			int widthStep = m.widthStep;
			int nChan = m.nChannels;
			int x0, y0, x1, y1;

			int y0sInf = imgLimits[0][BottomLeft][YCoordinate];
			int y0sSup = imgLimits[0][TopLeft][YCoordinate];
			int y1sSup = imgLimits[1][TopLeft][YCoordinate];
			int y1sInf = imgLimits[1][BottomLeft][YCoordinate];
			int x0sLeft = imgLimits[0][TopLeft][XCoordinate];
			int x0sRight = imgLimits[0][TopRight][XCoordinate];
			int x1sRight = imgLimits[1][TopRight][XCoordinate];
			int x1sLeft = imgLimits[1][TopLeft][XCoordinate];

			int width0 = imgLimits[0][BottomRight][XCoordinate] - imgLimits[0][BottomLeft][XCoordinate];
			int width1 = imgLimits[1][BottomRight][XCoordinate] - imgLimits[1][BottomLeft][XCoordinate];
			int height0 = imgLimits[0][BottomLeft][YCoordinate] - imgLimits[0][TopLeft][YCoordinate];
			int height1 = imgLimits[1][BottomLeft][YCoordinate] - imgLimits[1][TopLeft][YCoordinate];

			//int differencesMean;
			int result1 = 0, result2 = 0;
			int sumDifferences1 = 0;
			int sumDifferences2 = 0;
			if (width0 == width1)   //comparar arestas superiores e inferiores entre si para 2 pecas obviamente
			{
				//comparar inferior dos 0s a superior dos 1s
				y0 = y0sInf;
				y1 = y1sSup;
				x1 = x1sLeft;
				for (x0 = x0sLeft; x0 < x0sRight; x0++)
				{
					result1 = (int)(Math.Round(((imgPtr + y0 * widthStep + x0 * nChan)[0]
													+ (imgPtr + y0 * widthStep + x0 * nChan)[1]
													+ (imgPtr + y0 * widthStep + x0 * nChan)[2]) / 3.0));
					result2 = (int)(Math.Round(((imgPtr + y1 * widthStep + x1 * nChan)[0]
													+ (imgPtr + y1 * widthStep + x1 * nChan)[1]
													+ (imgPtr + y1 * widthStep + x1 * nChan)[2]) / 3.0));
					x1++;
					sumDifferences1 += (Math.Abs(result1 - result2));
				}
				//comparar superior dos 0s a inferior dos 1s
				y0 = y0sSup;
				y1 = y1sInf;
				x1 = x1sLeft;
				for (x0 = x0sLeft; x0 < x0sRight; x0++)
				{
					result1 = (int)(Math.Round(((imgPtr + y0 * widthStep + x0 * nChan)[0]
													+ (imgPtr + y0 * widthStep + x0 * nChan)[1]
													+ (imgPtr + y0 * widthStep + x0 * nChan)[2]) / 3.0));
					result2 = (int)(Math.Round(((imgPtr + y1 * widthStep + x1 * nChan)[0]
													+ (imgPtr + y1 * widthStep + x1 * nChan)[1]
													+ (imgPtr + y1 * widthStep + x1 * nChan)[2]) / 3.0));
					x1++;
					sumDifferences2 += (Math.Abs(result1 - result2));
				}
				Console.WriteLine("WIDTH 0 " + width0);
				Console.WriteLine("HEIGHT 0 " + height0);
				Console.WriteLine("HEIGHT 1 " + height1);
				Image<Bgr, byte> newImg = new Image<Bgr, byte>(width0, height0 + height1); //criar nova imagem com dimensoes necessarias
				MIplImage mNew = newImg.MIplImage;
				int newWidthStep = mNew.widthStep;
				byte* newImgPtr = (byte*)newImg.MIplImage.imageData.ToPointer();


				if (sumDifferences1 > sumDifferences2) //por peca 1s em cima dos 0s
				{
					Console.WriteLine("BAIXO EM CIMA");
					addPiece(imgPtr, newImgPtr, imgLimits[1], height1, width1, widthStep, nChan, 0, 0, newWidthStep);
					addPiece(imgPtr, newImgPtr, imgLimits[0], height0, width0, widthStep, nChan, height1, 0, newWidthStep);
				}
				else //por peca 0s em cima dos 1s
				{
					Console.WriteLine("CIMA EM CIMA");
					addPiece(imgPtr, newImgPtr, imgLimits[0], height0, width0, widthStep, nChan, 0, 0, newWidthStep);
					addPiece(imgPtr, newImgPtr, imgLimits[1], height0 + height1, width1, widthStep, nChan, height0, 0, newWidthStep);
				}

				return newImg;
			}
			else if (height0 == height1) //comparar arestas laterais
			{
				//comparar direita dos 0s a esquerda dos 1s
				//y0 = y0sSup;
				y1 = y1sSup;
				x0 = x0sRight;
				x1 = x1sLeft;
				for (y0 = y0sSup; y0 < y0sInf; y0++)
				{
					result1 = (int)(Math.Round(((imgPtr + y0 * widthStep + x0 * nChan)[0]
													+ (imgPtr + y0 * widthStep + x0 * nChan)[1]
													+ (imgPtr + y0 * widthStep + x0 * nChan)[2]) / 3.0));
					result2 = (int)(Math.Round(((imgPtr + y1 * widthStep + x1 * nChan)[0]
													+ (imgPtr + y1 * widthStep + x1 * nChan)[1]
													+ (imgPtr + y1 * widthStep + x1 * nChan)[2]) / 3.0));
					y1++;
					sumDifferences1 += (Math.Abs(result1 - result2));
				}
				//comparar esquerda dos 0s a direita dos 1s
				//y0 = y0sSup;
				y1 = y1sSup;
				x0 = x0sLeft;
				x1 = x1sRight;
				for (y0 = y0sSup; y0 < y0sInf; y0++)
				{
					result1 = (int)(Math.Round(((imgPtr + y0 * widthStep + x0 * nChan)[0]
													+ (imgPtr + y0 * widthStep + x0 * nChan)[1]
													+ (imgPtr + y0 * widthStep + x0 * nChan)[2]) / 3.0));
					result2 = (int)(Math.Round(((imgPtr + y1 * widthStep + x1 * nChan)[0]
													+ (imgPtr + y1 * widthStep + x1 * nChan)[1]
													+ (imgPtr + y1 * widthStep + x1 * nChan)[2]) / 3.0));
					y1++;
					sumDifferences2 += (Math.Abs(result1 - result2));
				}
				Console.WriteLine("WIDTH 0 " + width0);
				Console.WriteLine("WIDTH 1 " + width1);
				Console.WriteLine("HEIGHT 1 " + height1);
				Image<Bgr, byte> newImg = new Image<Bgr, byte>(width0 + width1, height0); //criar nova imagem com dimensoes necessarias
				MIplImage mNew = newImg.MIplImage;
				int newWidthStep = mNew.widthStep;
				byte* newImgPtr = (byte*)newImg.MIplImage.imageData.ToPointer();


				if (sumDifferences1 > sumDifferences2) //por a 1s -direita- primeiro
				{
					Console.WriteLine("DIREITA PRIMEIRO");
					addPiece(imgPtr, newImgPtr, imgLimits[1], height1, width1, widthStep, nChan, 0, 0, newWidthStep);
					addPiece(imgPtr, newImgPtr, imgLimits[0], height0, width0 + width1, widthStep, nChan, 0, width1, newWidthStep);

					//addPiece(imgPtr, newImgPtr, imgLimits[0], height0, width0, widthStep, nChan, 0, 0, newWidthStep);
					//addPiece(imgPtr, newImgPtr, imgLimits[1], height1, width1 + width0, widthStep, nChan, 0, width0, newWidthStep);
				}
				else //por a 0s - esquerda- depois
				{
					Console.WriteLine("ESQUERDA PRIMEIRO");
					addPiece(imgPtr, newImgPtr, imgLimits[0], height0, width0, widthStep, nChan, 0, 0, newWidthStep);
					addPiece(imgPtr, newImgPtr, imgLimits[1], height1, width1, widthStep, nChan, 0, width0, newWidthStep);
				}

				return newImg;
			}
			Console.WriteLine("NEM WIDTH NEM HEIGHT SAO IGUAIS");
			return img;
		}

		//adds a piece
		public static unsafe void addPiece(byte* originalPtr, byte* newImagePtr, int[][] limits, int height, int width, int widthStep, int nChan, int startingHeight, int startingWidth, int newWidthStep)
		{
			int x, y;
			int startY = limits[TopLeft][YCoordinate];
			int startX = limits[TopLeft][XCoordinate];
			Console.WriteLine("STARTING HEIGHT " + startingHeight);
			Console.WriteLine("STARTING WIDTH " + startingWidth);
			Console.WriteLine("HEIGHT " + height);
			Console.WriteLine("WIDTH " + width);
			for (y = startingHeight; y < height; y++)
			{
				for (x = startingWidth; x < width; x++)
				{
					(newImagePtr + y * newWidthStep + x * nChan)[0] = (originalPtr + startY * widthStep + startX * nChan)[0];
					(newImagePtr + y * newWidthStep + x * nChan)[1] = (originalPtr + startY * widthStep + startX * nChan)[1];
					(newImagePtr + y * newWidthStep + x * nChan)[2] = (originalPtr + startY * widthStep + startX * nChan)[2];

					startX++;
				}
				startY++;
				startX = limits[TopLeft][XCoordinate]; //returns to the start of the line
			}
		}


		private static unsafe void RotateImage(Image<Bgr, Byte> img, int[][] pieces, float rotation)
		{
			MIplImage m = img.MIplImage;

			byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
			byte[] background = { dataPtr[0], dataPtr[1], dataPtr[2] };
			int width, height, startingX, startingY;
			Image<Bgr, Byte> toBeRotated;


			width = pieces[BottomRight][XCoordinate] - pieces[TopLeft][XCoordinate] + 2;
			height = pieces[BottomLeft][YCoordinate] - pieces[TopRight][YCoordinate] + 2;
			startingX = pieces[TopLeft][XCoordinate] - 1;
			startingY = pieces[TopRight][YCoordinate] - 1;
			int endingX = pieces[TopRight][XCoordinate];
			int endingY = pieces[BottomRight][YCoordinate];

			toBeRotated = img.Copy(new Rectangle(startingX, startingY, width, height));
			int[] border;
			RotationCustomBackground(toBeRotated, toBeRotated.Copy(), rotation, background, out border);
			int extra = 0;
			border[0] += extra;
			border[1] -= extra;
			border[2] += extra;
			border[3] -= extra;

			MIplImage mr = toBeRotated.MIplImage;
			byte* dataPtrRotated = (byte*)mr.imageData.ToPointer(); // Pointer to the image
			int widthstep = mr.widthStep;


			//eliminacao das edges
			byte* pixel;
			int j, i;
			for (j = 0; j < height; j++)
			{
				for (i = 0; i < width; i++)
				{
					if (i <= border[0] || j <= border[2] || i >= border[1] || j >= border[3])
					{
						pixel = (dataPtrRotated + j * widthstep + i * 3);
						pixel[0] = background[0];
						pixel[1] = background[1];
						pixel[2] = background[2];

					}
				}
			}

			img.ROI = new Rectangle(startingX, startingY, width, height);
			toBeRotated.CopyTo(img);
			img.ROI = Rectangle.Empty;
		}

		private static unsafe void TrimImage(Image<Bgr, Byte> img, int[][] pieces)
		{
			MIplImage m = img.MIplImage;

			byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
			byte[] background = { dataPtr[0], dataPtr[1], dataPtr[2] };
			int width, height, startingX, startingY;
			Image<Bgr, Byte> toBeRotated;


			width = pieces[BottomRight][XCoordinate] - pieces[TopLeft][XCoordinate] + 1;
			height = pieces[BottomLeft][YCoordinate] - pieces[TopRight][YCoordinate] + 1;
			startingX = pieces[TopLeft][XCoordinate];
			startingY = pieces[TopRight][YCoordinate];

			toBeRotated = img.Copy(new Rectangle(startingX, startingY, width, height));
			MIplImage mr = toBeRotated.MIplImage;
			byte* dataPtrRotated = (byte*)mr.imageData.ToPointer(); // Pointer to the image
			int widthstep = mr.widthStep;


			//eliminacao das edges
			byte* pixel;
			int j, i;
			for (j = 0; j < height; j++)
			{
				for (i = 0; i < width; i++)
				{
					if (i <= 0 || j <= 0 || i >= width - 1 || j >= height - 1)
					{
						pixel = (dataPtrRotated + j * widthstep + i * 3);
						pixel[0] = background[0];
						pixel[1] = background[1];
						pixel[2] = background[2];

					}
				}
			}

			img.ROI = new Rectangle(startingX, startingY, width, height);
			toBeRotated.CopyTo(img);
			img.ROI = Rectangle.Empty;
		}


		public static unsafe Image<Bgr, Byte> GetRotatedImage(Image<Bgr, Byte> img, int[][][] pieces, float rotation0, float rotation1)
		{
			MIplImage m = img.MIplImage;
			byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
			byte[] background = { dataPtr[0], dataPtr[1], dataPtr[2] };


			if (rotation0 != 0)
			{
				RotateImage(img, pieces[0], rotation0);
			}
			if (rotation1 != 0)
			{
				RotateImage(img, pieces[1], rotation1);
			}

			return img;

		}


		public static unsafe void PuzzleRotation(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float angle, byte* background)
		{
			unsafe
			{


				MIplImage m = img.MIplImage;
				MIplImage mc = imgCopy.MIplImage;

				byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image that will be translated.
				byte* copyDataPtr = (byte*)mc.imageData.ToPointer(); //pointer to copy image
				byte* tempPtr;
				byte* copyTempPtr;

				int width = img.Width;
				int height = img.Height;
				int xOrigem, yOrigem;
				int nChan = m.nChannels; // number of channels
				int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)


				double widthCentered = width / 2.0;
				double heightCentered = height / 2.0;

				double sin = Math.Sin(angle);
				double cos = Math.Cos(angle);

				if (nChan == 3) // image in RGB
				{
					for (int y = 0; y < height; y++)
					{

						for (int x = 0; x < width; x++)
						{
							xOrigem = (int)Math.Round(((x - widthCentered) * cos) - ((heightCentered - y) * sin) + widthCentered);
							yOrigem = (int)Math.Round(heightCentered - ((x - widthCentered) * sin) - ((heightCentered - y) * cos));

							tempPtr = dataPtr + (y * m.widthStep) + (x * nChan);


							if (xOrigem >= 0 && xOrigem < width && yOrigem >= 0 && yOrigem < height)
							{
								copyTempPtr = copyDataPtr + (yOrigem * m.widthStep) + (xOrigem * nChan);

								tempPtr[0] = copyTempPtr[0];
								tempPtr[1] = copyTempPtr[1];
								tempPtr[2] = copyTempPtr[2];
							}
							else
							{
								tempPtr[0] = background[0];
								tempPtr[1] = background[1];
								tempPtr[2] = background[2];


							}
						}

					}
				}
			}
		}


		//retorna em radianos
		public static float getPieceAngleDegree(int x1, int y1, int x2, int y2)
		{
			return Convert.ToSingle(Math.Atan2(y1 - y2, x2 - x1));
		}

		//retorna em radianos
		public static float getPieceAngle(int x1, int y1, int x2, int y2)
		{
			return Convert.ToSingle(Math.Atan2(y1 - y2, x2 - x1) * Rad2Deg * Math.PI / 180.0);
		}

		public static unsafe int[][][] GetPieces(Image<Bgr, Byte> img)
		{

			int x, y;

			int[][][] pieces = new int[2][][];  // inicializacao do jagged array de 3 dimensoes, a primeira dimensao representa uma peca de puzzle

			pieces[0] = new int[4][];
			pieces[1] = new int[4][];           //a segunda qual dos quatro cantos da peca se esta a falar

			for (y = 0; y < 2; y++)
			{
				for (x = 0; x < 4; x++)
				{
					pieces[y][x] = new int[2]; //a terceira dimensao e para as coordenadas x e y do tal canto da tal peca
				}

			}

			MIplImage m = img.MIplImage;
			Image<Gray, int> connectedImg = ConnectedComponenets(img);
			MIplImage mConnected = connectedImg.MIplImage;
			int* dataPtrConnected = (int*)connectedImg.MIplImage.imageData.ToPointer(); // Pointer to the connected components img

			int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
			int line = mConnected.widthStep / 4;
			int width = m.width;
			int height = m.height;
			bool first0 = true, first1 = true;

			int* newPixel;




			for (y = 0; y < height; y++)
			{
				for (x = 0; x < width; x++)
				{
					newPixel = dataPtrConnected + x;

					if (*newPixel == 0 || *newPixel == 1)   //estamos na presenca de um objecto
					{
						if (first0 && *newPixel == 0)
						{
							for (int z = 0; z < 4; z++)
							{
								pieces[0][z][XCoordinate] = x;
								pieces[0][z][YCoordinate] = y;
							}
							first0 = false;
						}
						else if (first1 && *newPixel == 1)
						{
							for (int z = 0; z < 4; z++)
							{
								pieces[1][z][XCoordinate] = x;
								pieces[1][z][YCoordinate] = y;
							}
							first1 = false;

						}
						else
						{
							if (x < pieces[*newPixel][TopLeft][XCoordinate])// novo top left
							{
								pieces[*newPixel][TopLeft][XCoordinate] = x;
								pieces[*newPixel][TopLeft][YCoordinate] = y;
							}
							else
							   if (y > pieces[*newPixel][BottomLeft][YCoordinate])//novo bottom left
							{
								pieces[*newPixel][BottomLeft][XCoordinate] = x;
								pieces[*newPixel][BottomLeft][YCoordinate] = y;
							}
							else
							   if (y <= pieces[*newPixel][TopRight][YCoordinate])//novo top right
							{
								pieces[*newPixel][TopRight][XCoordinate] = x;
								pieces[*newPixel][TopRight][YCoordinate] = y;

							}
							else
								if (x >= pieces[*newPixel][BottomRight][XCoordinate]) //novo bottom right
							{
								pieces[*newPixel][BottomRight][XCoordinate] = x;
								pieces[*newPixel][BottomRight][YCoordinate] = y;
							}
						}

					}

				}
				dataPtrConnected += line;
			}


			for (y = 0; y < 2; y++)           // apenas para debug
			{
				for (x = 0; x < 4; x++)     // apenas para debug
				{

					Console.WriteLine("Piece: " + y + " corner: " + x);
					Console.Write(pieces[y][x][0] + " ");                     // apenas para debug
					Console.WriteLine(pieces[y][x][1]);
				}
			}



			Console.WriteLine(getPieceAngle(pieces[1][0][0], pieces[1][0][1], pieces[1][1][0], pieces[1][1][1]));

			if (pieces[0][TopLeft][YCoordinate] != pieces[0][TopRight][YCoordinate])
			{       // necessario quando a imagem de cima esta rodada
					// pieces[0][BottomLeft][XCoordinate] -= 1;
				pieces[0][BottomLeft][YCoordinate] += 1;

				pieces[0][BottomRight][XCoordinate] += 1;
				// pieces[0][BottomRight][YCoordinate] += 1;
			}


			return pieces;
		}

		private static unsafe HashSet<int> PiecesInfo(Image<Gray, int> connectedImg, out Dictionary<int, int[][]> pieces_vector, out Dictionary<int, double[]> angles)
		{
			MIplImage m = connectedImg.MIplImage;
			int* dataPtr = (int*)m.imageData.ToPointer(); // Pointer to the image
			int width = m.width;
			int height = m.height;

			int line = m.widthStep;
			int padding = m.widthStep / 4 - m.width;

			angles = new Dictionary<int, double[]>();


			//	int sx = 0, sy = 1, sxx = 2, syy = 3, sxy = 4, area = 5;
			Dictionary<int, long[]> calcs = new Dictionary<int, long[]>();

			pieces_vector = new Dictionary<int, int[][]>();

			HashSet<int> labels = new HashSet<int>();

			int x, y;
			int[][] vec;
			long[] tmp;
			for (y = 0; y < height; y++)
			{
				for (x = 0; x < width; x++)
				{
					int label = *dataPtr;
					if (label < int.MaxValue)
					{
						if (!labels.Contains(label))
						{
							calcs.Add(label, new long[6]);
							tmp = calcs[label];
							tmp[0] += x;
							tmp[1] += y;
							tmp[2] += x * x;
							tmp[3] += y * y;
							tmp[4] += x * y;
							tmp[5]++;
							labels.Add(label);
							pieces_vector.Add(label, new int[4][]);
							vec = pieces_vector[label];
							vec[0] = new int[] { x, y };
							vec[1] = new int[] { x, y };
							vec[2] = new int[] { x, y };
							vec[3] = new int[] { x, y };
						}
						else
						{
							tmp = calcs[label];
							tmp[0] += x;
							tmp[1] += y;
							tmp[2] += x * x;
							tmp[3] += y * y;
							tmp[4] += x * y;
							tmp[5]++;
							vec = pieces_vector[label];
							if (x < vec[TopLeft][XCoordinate])// novo top left
							{
								vec[TopLeft][XCoordinate] = x;
								vec[TopLeft][YCoordinate] = y;
							}
							else if (y > vec[BottomLeft][YCoordinate])//novo bottom left
							{
								vec[BottomLeft][XCoordinate] = x;
								vec[BottomLeft][YCoordinate] = y;
							}
							else if (y <= vec[TopRight][YCoordinate])//novo top right
							{
								vec[TopRight][XCoordinate] = x;
								vec[TopRight][YCoordinate] = y;

							}
							else if (x >= vec[BottomRight][XCoordinate]) //novo bottom right
							{
								vec[BottomRight][XCoordinate] = x;
								vec[BottomRight][YCoordinate] = y;
							}

						}

					}

					dataPtr++;
				}
				dataPtr += padding;
			}

			foreach (int l in labels)
			{
				angles[l] = new double[3];

				tmp = calcs[l];

				long xs = tmp[0];
				long ys = tmp[1];
				long a = tmp[5];
				double mxx = tmp[2] - (Math.Pow(xs, 2) / a);
				double myy = tmp[3] - (Math.Pow(ys, 2) / a);
				double mxy = tmp[4] - (xs * ys / a);

				double pwr = Math.Pow(mxx - myy, 2);

				double sqr = Math.Sqrt(pwr + 4 * Math.Pow(mxy, 2));
				double bot = 2 * mxy;
				if (bot == 0)
					angles[l][0] = 0;
				else
				{
					angles[l][0] = Math.Atan((mxx - myy + sqr) / bot);
					if (angles[l][0] < 0)
						angles[l][0] += 1.5708;

				}

				angles[l][1] = xs / a;
				angles[l][2] = ys / a;
			}

			return labels;
		}

		public static double ConvertRadiansToDegrees(double radians)
		{
			double degrees = (180 / Math.PI) * radians;
			return (degrees);
		}

		public static double ConvertDegreesToRadians(double degrees)
		{
			double rads = (Math.PI / 180) * degrees;
			return rads;
		}

	}
}