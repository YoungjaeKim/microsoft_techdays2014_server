using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Azure.WebJobs;

namespace AzureBackgroundApplication
{
	public class Program
	{
		private static void Main(string[] args)
		{
			JobHost host = new JobHost();
			host.RunAndBlock();
		}

		public static void ProcessSimpleQueueMessage([QueueTrigger("webjobsqueue")] string inputText,
			[Blob("techdays/techdays.txt")]TextWriter writer)
		{
			writer.WriteLine(inputText);
		}


		/// <summary>
		/// YouTube 동영상 주소로부터 이미지를 취득. 링크가 2개일 경우 좌우병합.
		/// </summary>
		/// <param name="input"></param>
		/// <param name="BlobFilename"></param>
		/// <param name="writer"></param>
		//public static void ProcessQueueMessage(
		//	[QueueTrigger("webjobsqueue")] YoutubeLink input,
		//	string BlobFilename,
		//	[Blob("techdays/{BlobFilename}", FileAccess.Write)] Stream writer)
		//{
		//	if (input.Links.Length == 1)
		//	{
		//		using (HttpClient client = new HttpClient())
		//		{
		//			var url = getVideoCode(input.Links[0]);
		//			using (HttpResponseMessage response = client.GetAsync(url).Result)
		//			{
		//				if (response.IsSuccessStatusCode)
		//				{
		//					using (HttpContent content = response.Content)
		//					{
		//						var image = content.ReadAsByteArrayAsync().Result;
		//						writer.Write(image, 0, image.Length);
		//					}
		//				}
		//				else
		//					Trace.WriteLine("cannot get from " + url);
		//			}
		//		}
		//	}
		//	else if (input.Links.Length == 2)
		//	{
		//		string[] links = input.Links.Select(getVideoCode).ToArray();
		//		var messageBytes = ImageToByte(mergeImages(links));

		//		writer.Write(messageBytes, 0, messageBytes.Length);
		//	}
		//	else
		//	{
		//		byte[] errorOutput = Encoding.UTF8.GetBytes("error");
		//		writer.WriteAsync(errorOutput, 0, errorOutput.Length).Wait();
		//	}
		//	Trace.WriteLine("완료 :)");
		//}


		/// <summary>
		/// 이미지 좌우 병합
		/// </summary>
		/// <param name="urls"></param>
		/// <returns></returns>
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

		
		public static byte[] ImageToByte(Image img)
		{
			ImageConverter converter = new ImageConverter();
			return (byte[])converter.ConvertTo(img, typeof(byte[]));
		}

		/// <summary>
		/// YouTube Thumbnail 주소 처리
		/// <para>주의: 반드시 <![CDATA['v=']]>이 들어가는 주소여야 함.</para>
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		protected static string getVideoCode(string url)
		{
			// 참고: http://jquery-howto.blogspot.kr/2009/02/how-to-get-youtube-video-screenshot.html
			var code = Regex.Match(url, "[\\?&]v=([^&#]*)");
			Console.WriteLine("{0} is extracted from {1}.", code, url);

			var jpegUrl = "http://img.youtube.com/vi/" + code.ToString().Substring(3) + "/0.jpg";
			return jpegUrl;
		}
	}
}
