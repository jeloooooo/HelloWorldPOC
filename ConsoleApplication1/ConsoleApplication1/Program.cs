using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using System.Text.RegularExpressions;
using System.Web;

namespace ConsoleApplication1
{
	class Program
	{
		static void Main(string[] args)
		{
			MostFrequentWords();
			//TestLoad2();
		}

		public static void MostFrequentWords()
		{
			var url = "http://www.wheninmanila.com/5-things-to-do-in-bonifacio-global-city/";
			var webGet = new HtmlWeb();
			HtmlDocument document = webGet.Load(url);

			if (document != null)
			{ 
				var obj = Regex.Split(RemoveUnwantedTags(document).ToLower(), @"\W+")
					.Where(s => s.Length > 3)
					.GroupBy(s => s)
					.OrderByDescending(g => g.Count());

				foreach (var item in obj)
				{
					if (item.Count() > 2)
						Console.WriteLine("Word: " + item.Key + ", Count: " + item.Count());
				}

				Console.ReadLine();
			}
		}

		public static string ExtractText(string html)
		{
			if (html == null)
			{
				throw new ArgumentNullException("html");
			}

			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(html);

			var chunks = new List<string>();

			foreach (var item in doc.DocumentNode.DescendantsAndSelf())
			{
				if (item.NodeType == HtmlNodeType.Text)
				{
					if (item.InnerText.Trim() != "")
					{
						chunks.Add(item.InnerText.Trim());
					}
				}
			}
			return String.Join(" ", chunks);
		}

		public static string RemoveHTMLTags(string content)
		{
			var cleaned = string.Empty;
			try
			{
				string textOnly = string.Empty;
				Regex tagRemove = new Regex(@"<[^>]*(>|$)");
				Regex compressSpaces = new Regex(@"[\s\r\n]+");
				textOnly = tagRemove.Replace(content, string.Empty);
				textOnly = compressSpaces.Replace(textOnly, " ");
				cleaned = textOnly;
			}
			catch
			{
				//A tag is probably not closed. fallback to regex string clean.

			}

			return HttpUtility.HtmlDecode(cleaned);
		}

		internal static string RemoveUnwantedTags(HtmlDocument data)
		{
			//if (string.IsNullOrEmpty(data)) return string.Empty;

			//var document = new HtmlDocument();
			//document.LoadHtml(data);

			// remove scripts and styles
			data.DocumentNode.Descendants()
				.Where(n => n.Name == "script" || n.Name == "style" || n.Name == "#script")
				.ToList()
				.ForEach(n => n.Remove());

			return RemoveHTMLTags(data.DocumentNode.InnerText);

			//var acceptableTags = new String[] { }; //"strong", "em", "u"

			//var nodes = new Queue<HtmlNode>(data.DocumentNode.SelectNodes("./*|./text()"));
			//while (nodes.Count > 0)
			//{
			//	var node = nodes.Dequeue();
			//	var parentNode = node.ParentNode;

			//	if (!acceptableTags.Contains(node.Name) && node.Name != "#text")
			//	{
			//		var childNodes = node.SelectNodes("./*|./text()");

			//		if (childNodes != null)
			//		{
			//			foreach (var child in childNodes)
			//			{
			//				nodes.Enqueue(child);
			//				parentNode.InsertBefore(child, node);
			//			}
			//		}

			//		parentNode.RemoveChild(node);

			//	}
			//}

			//return data.DocumentNode.InnerHtml;
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
