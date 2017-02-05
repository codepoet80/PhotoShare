using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;

namespace PhotoShare
{
	/// <summary>
	/// Summary description for PhotoShare.
	/// </summary>
	public class PhotoUtilities
	{
		public PhotoUtilities()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public Image modifyImage(Image imgPhoto, int Width, int Height, int Rotate)
		{
			int sourceWidth = imgPhoto.Width;
			int sourceHeight = imgPhoto.Height;
			
			int destWidth = Width;
			float nPercentW = ((float)destWidth/(float)sourceWidth);
			int destHeight;
			if (Height == 0)
				destHeight = System.Convert.ToInt16(sourceHeight*nPercentW);
			else
				destHeight = Height;

			Bitmap bmPhoto = new Bitmap(destWidth, destHeight, PixelFormat.Format24bppRgb);
			bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

			Graphics grPhoto = Graphics.FromImage(bmPhoto);
			grPhoto.Clear(Color.FromName("White"));
			grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

			grPhoto.DrawImage(imgPhoto, 
				new Rectangle(0,0,destWidth,destHeight),
				new Rectangle(0,0,sourceWidth,sourceHeight),
				GraphicsUnit.Pixel);

			for (int i = 0; i < Rotate; i++)
			{
				bmPhoto.RotateFlip(RotateFlipType.Rotate90FlipNone);
			}

			imgPhoto.Dispose();
			grPhoto.Dispose();
			return bmPhoto;
		}
	}
}
