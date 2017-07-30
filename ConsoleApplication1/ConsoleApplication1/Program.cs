using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ScrapySharp.Extensions;

namespace ConsoleApplication1
{
	class Program
	{
		static void Main(string[] args)
		{
			TestLoad2();
		}

		public static void TestLoad2()
		{
			string main = "https://www.tripadvisor.com.ph";
			// Things To Do URL
			var url = "https://www.tripadvisor.com.ph/Attractions-g1758900-Activities-Taguig_City_Metro_Manila_Luzon.html#ATTRACTION_SORT_WRAPPER";
			var webGet = new HtmlWeb();
			HtmlDocument document = webGet.Load(url);
			if (document != null)
			{
				// Get Top Things To Do list
				var topPlacesNode = document.DocumentNode
							.Descendants("div")
							.Where(d =>
							   d.Attributes.Contains("class")
							   &&
							   d.Attributes["class"].Value.Contains("listing_title")
							);
				foreach (var topPlace in topPlacesNode)
				{
					Console.WriteLine("ITINERARY: " + topPlace.InnerText);	//Get name
					var link = topPlace.Descendants("a").First().GetAttributeValue("href", null); //Get url
					var reviewLink = main + link;
					Console.WriteLine("URL: " + reviewLink);

					// Get Reviews
					document = webGet.Load(reviewLink);
					var reviewsNode = document.DocumentNode
							.Descendants("p")
							.Where(d =>
							   d.Attributes.Contains("class")
							   &&
							   d.Attributes["class"].Value.Contains("partial_entry")
							);
					foreach (var review in reviewsNode)
					{
						Console.WriteLine("REVIEW: " + review.InnerText); 
					}

					Console.WriteLine(Environment.NewLine);
					
				}
			}
			Console.ReadLine();
		}

		public static void TestLoad()
		{
			var url = "https://www.thewholeworldisaplayground.com/japan-itinerary-2-two-week-14-days-tokyo/";
			var webGet = new HtmlWeb();
			HtmlDocument document = webGet.Load(url);
			if (document != null)
			{
				//var nodes = document.DocumentNode.CssSelect("h2 strong").ToList();
				var nodes = document.DocumentNode.SelectNodes("//*[text()[contains(., 'accommodation')]]");
				foreach (var node in nodes)
				{
					Console.WriteLine("Accommodation: " + node.InnerText);
					//Console.WriteLine("Selling: " + node.CssSelect("h2 a").Single().InnerText);
				}
			}
			Console.ReadLine();
		}
	}
}
