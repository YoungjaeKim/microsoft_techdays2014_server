using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using nQuant;

namespace AzureBackgroundApplication
{
	public class Program
	{
		static void Main(string[] args)
		{
			JobHost host = new JobHost();
			host.RunAndBlock();

		}

		public static void ProcessQueueMessage([QueueTrigger("webjobsqueue")] string input, [Blob("tagboa/out3")] out string output)
		{
			output = input;
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
	}
}
