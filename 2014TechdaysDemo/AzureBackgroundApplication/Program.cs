using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
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

		}

		public static void ProcessQueueMessage([QueueTrigger("webjobsqueue")] string input,
			[Blob("tagboa/youtube")] Stream writer)
		{
			using (HttpClient client = new HttpClient())
			using (HttpResponseMessage response = client.GetAsync(getScreen(input)).Result)
			using (HttpContent content = response.Content)
			{

				var messageBytes = content.ReadAsByteArrayAsync().Result;
				writer.Write(messageBytes, 0, messageBytes.Length);
			}

			//output = input;
			//writer.WriteLine(input);
			//var quantizer = new WuQuantizer();
			//using (var bitmap = new Bitmap(input))
			//{
			//	using (var quantized = quantizer.QuantizeImage(bitmap))
			//	{
			//		quantized.Save(output, ImageFormat.Png);
			//	}
			//}
		}

		protected static string getScreen(string code)
		{
			var url = "http://img.youtube.com/vi/" + code + "/0.jpg";
			Console.WriteLine(url);
			return url;
		}
	}
}
