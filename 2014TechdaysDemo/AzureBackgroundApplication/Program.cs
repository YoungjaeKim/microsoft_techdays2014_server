using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using nQuant;

namespace AzureBackgroundApplication
{
	public class Program
	{
		private static void Main(string[] args)
		{
			JobHost host = new JobHost();
			host.RunAndBlock();
			//ProcessQueueMessage(new YoutubeLink(), "", new MemoryStream());
		}

		public static void ProcessQueueMessage([QueueTrigger("webjobsqueue")] YoutubeLink input, string BlobFilename,
			[Blob("tagboa/{BlobFilename}", FileAccess.Write)] Stream writer)
		{
			string[] links = new[] { getScreen("http://www.youtube.com/watch?v=1fBFm4OD2W0"), getScreen("http://www.youtube.com/watch?v=pg1onIAwpaQ") };
			var messageBytes = ImageToByte(mergeImages(links));

			writer.Write(messageBytes, 0, messageBytes.Length);


			//using (HttpClient client = new HttpClient())
			//{
			//	var url = getScreen(input.Link);
			//	using (HttpResponseMessage response = await client.GetAsync(url))
			//	{
			//		if (response.IsSuccessStatusCode)
			//		{
			//			using (HttpContent content = response.Content)
			//			{
			//				var messageBytes = content.ReadAsByteArrayAsync().Result;
			//				writer.Write(messageBytes, 0, messageBytes.Length);
			//			}
			//		}
			//		else
			//		{
			//			Console.WriteLine("cannot get from " + url);
			//		}
			//	}
			//}

			//byte[] errorOutput = Encoding.UTF8.GetBytes("error");
			//await writer.WriteAsync(errorOutput, 0, errorOutput.Length);
		}
		public static byte[] ImageToByte(Image img)
		{
			ImageConverter converter = new ImageConverter();
			return (byte[])converter.ConvertTo(img, typeof(byte[]));
		}
		public static Bitmap mergeImages(string[] urls)
		{
			if (urls.Length != 2)
				throw new ArgumentException("urls requires only two array length", "urls");

			Bitmap a, b;
			using (var ms = new MemoryStream(getBitmapStream(urls[0])))
				a = new Bitmap(ms);
			using (var ms = new MemoryStream(getBitmapStream(urls[1])))
				b = new Bitmap(ms);

			Graphics g = Graphics.FromImage(a);
			g.DrawImage(b, new Rectangle(b.Width / 2, 0, b.Width / 2 - 1, b.Height), new Rectangle(b.Width / 2, 0, b.Width / 2 - 1, b.Height), GraphicsUnit.Pixel);
			g.Dispose();
			return a;
		}

		/// <summary>
		/// url에서 스트림으로 반환
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		private static byte[] getBitmapStream(string url)
		{
			using (HttpClient client = new HttpClient())
			{
				using (HttpResponseMessage response = client.GetAsync(url).Result)
				{
					if (response.IsSuccessStatusCode)
					{
						using (HttpContent content = response.Content)
						{
							return content.ReadAsByteArrayAsync().Result;
						}
					}
				}
			}
			return null;
		}

		public static Bitmap cropAtRect(Bitmap b, Rectangle r)
		{
			Bitmap nb = new Bitmap(r.Width, r.Height);
			Graphics g = Graphics.FromImage(nb);
			g.DrawImage(b, -r.X, -r.Y);
			return nb;
		}

		protected static string getScreen(string url)
		{
			var code = Regex.Match(url, "[\\?&]v=([^&#]*)");
			Console.WriteLine("{0} is extracted from {1}.", code, url);

			var jpegUrl = "http://img.youtube.com/vi/" + code.ToString().Substring(3) + "/0.jpg";
			return jpegUrl;
		}
	}
}
