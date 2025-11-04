using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Xml;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using System.Web.Services;
using System.Web.SessionState;
using PhotoShare;

namespace PhotoShare
{
	/// <summary>
	/// PhotoShare image sharing system. By codepoet80
	/// This code is the intellectual propery of codepoet80
	/// Created: September 2003
	/// This Version: November 2003
	/// https://github.com/codepoet80
	/// </summary>
	[WebService(Namespace="https://github.com/codepoet80/photoshare", Description="PhotoSharing Service by codepoet80")]
	public class PhotoShare : System.Web.Services.WebService
	{
		public PhotoShare()
		{
			//CODEGEN: This call is required by the ASP.NET Web Services Designer
			InitializeComponent();
		}

		#region Component Designer generated code
		
		//Required by the Web Services Designer 
		private IContainer components = null;
				
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if(disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);		
		}
		
		#endregion

		[WebMethod]
		public ListItems ListFiles(string Folder, string Filter)
		{
			if (Folder == null || Folder == "")
				Folder = "";
			Folder = urlDecode(Folder);
			if (Filter == null || Filter == "")
				Filter = "*";

			ListItems fileList = new ListItems();
			fileList.Folder = Folder;
			
			try
			{
				string[] directoryEntries =
					System.IO.Directory.GetFileSystemEntries(Server.MapPath("../" + Folder), Filter); 
			
				ListItem[] Items = new ListItem[directoryEntries.Length];            
				int i = 0;
			
				foreach (string str in directoryEntries)
				{
					ListItem currItem = new ListItem();
					if(File.Exists(str))	//check to see if it's a file
					{
						currItem.Path = str.ToString();
						currItem.Name = getFileName(str.ToString());
						FileInfo fi = new FileInfo(str);
						currItem.Type = fi.Extension.ToLower();
						currItem.Type = currItem.Type.Replace(".", "");
						currItem.Size = calculateSize(fi.Length);
						currItem.Date = fi.CreationTime.ToString();
						if (currItem.Type == "jpg" || currItem.Type == "jpeg" || currItem.Type == "gif" || currItem.Type == "tif" || currItem.Type == "tiff" || currItem.Type == "png" || currItem.Type == "bmp")
						{
							Items[i] = new ListItem();
							Items[i] = currItem;
						}
						else
							i--;
					}
					else	//it's a subfolder
					{
						currItem.Path = str.ToString();
						currItem.Name = getFileName(str.ToString());
						currItem.Type = "folder";
						FileInfo fi = new FileInfo(str);
						currItem.Date = fi.CreationTime.ToString();
						Items[i] = new ListItem();
						Items[i] = currItem;
					}
					currItem = null;
					i++;
				}
				fileList.Item = Items;
				fileList.Result = "S_OK";
			}
			catch(Exception ex)
			{
				fileList.Result = ex.Message;
			}
			return fileList;
		}

		
		[WebMethod (true)]
		public void GetImage(string Path, string Size, string Rotate)
		{
			PhotoUtilities ps = new PhotoUtilities();
			Path = urlDecode(Path);
			
			string[] pathParts = Path.Split('[');
			Path = pathParts[0];
			string framePos = "none";
			if (pathParts.Length > 1)
			{
				framePos = pathParts[pathParts.Length - 1];
				framePos = framePos.Replace("]", "");
			}
		
			if (Size == null || Size == "")
				Size = "0";
			int nSize = Convert.ToInt16(Size);

			Context.Response.ContentType = "image/jpeg";
			string strPath = Path;
			
			Image objImage = System.Drawing.Image.FromFile(strPath);
			if (framePos != "none")
			{
				FrameDimension oDimension = new FrameDimension(objImage.FrameDimensionsList[0]);
				int FrameCount = objImage.GetFrameCount(oDimension);
				objImage.SelectActiveFrame(oDimension, Convert.ToInt32(framePos));
			}

			objImage = ps.modifyImage(objImage, nSize, 0, Convert.ToInt32(Rotate));
			objImage.Save(Context.Response.OutputStream, ImageFormat.Jpeg);

			Context.Response.Flush();
			objImage.Dispose();
		}

		
		#region Private Functions
		static string urlDecode(string param)
		{
			string decodeVal;
			decodeVal = param;
			decodeVal = decodeVal.Replace("%25", "%");
			decodeVal = decodeVal.Replace("%26", "&");
			decodeVal = decodeVal.Replace("%20", " ");
			decodeVal = decodeVal.Replace("%23", "#");
			decodeVal = decodeVal.Replace("%2B", "+");
			decodeVal = decodeVal.Replace("%27", "'");
			decodeVal = decodeVal.Replace("%2C", ",");
			decodeVal = decodeVal.Replace("\\", "/");
			return decodeVal;
		}

		static string getFileName(string fpath)
		{
			string[] pathParts = fpath.Split('\\');
			string fileName = pathParts[pathParts.Length-1];
			pathParts = fileName.Split('.');
			if (pathParts.Length > 1)
			{
				fileName = "";
				for (int f=0;f<=pathParts.Length-1;f++)
				{
					if (f<pathParts.Length - 1)
					{
						fileName += pathParts[f];
					}
				}
			}
			return fileName;
		}

		static string getType(string fpath)
		{
			string[] pathParts = fpath.Split('.');
			string fileType = pathParts[pathParts.Length-1];
			return fileType.ToLower();
		}

		static string calculateSize(long fsize)
		{
			string fileSize = fsize.ToString();
			if (fsize < 1024)
			{
				fileSize = fsize.ToString() + " Bytes";
			}
			if (fsize >= 1024 && fsize < 1024000)
			{
				fsize = fsize / 1024;
				fileSize = fsize.ToString() + " KB";
			}
			if (fsize >= 1024000 && fsize < 10240000)
			{
				fsize = (fsize / 1024) / 1024;
				fileSize = fsize.ToString() + " MB";
			}
			if (fsize > 10240000)
			{
				fsize = ((fsize / 1024) / 1024) / 1024;
				fileSize = fsize.ToString() + " GB";
			}
			return fileSize;
		}

		#endregion

	}


	#region Reply Classes
	
	[System.Xml.Serialization.XmlTypeAttribute(Namespace="https://github.com/codepoet80/photoshare")]
	public class ListItems
	{
		[System.Xml.Serialization.XmlElementAttribute("Result")]
		public string Result;
			
		[System.Xml.Serialization.XmlElementAttribute("Item")]
		public ListItem[] Item;

		[System.Xml.Serialization.XmlElementAttribute("Description")]
		public string Description;

		[System.Xml.Serialization.XmlElementAttribute("Date")]
		public string Date;

		[System.Xml.Serialization.XmlElementAttribute("Author")]
		public string Author;

		[System.Xml.Serialization.XmlAttributeAttribute("Folder")]
		public string Folder;

		[System.Xml.Serialization.XmlAttributeAttribute("Filter")]
		public string Filter;
	}

	[System.Xml.Serialization.XmlTypeAttribute(Namespace="https://github.com/codepoet80/PhotoShare/v.0")]
	public class ListItem
	{
		[System.Xml.Serialization.XmlAttributeAttribute("Name")]
		public string Name;

		[System.Xml.Serialization.XmlAttributeAttribute("Path")]
		public string Path;

		[System.Xml.Serialization.XmlAttributeAttribute("Size")]
		public string Size;

		[System.Xml.Serialization.XmlAttributeAttribute("Description")]
		public string Description;
		
		[System.Xml.Serialization.XmlAttributeAttribute("Type")]
		public string Type;

		[System.Xml.Serialization.XmlAttributeAttribute("Date")]
		public string Date;
	}

	#endregion
}
