using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
	public class TripAdvisorScraper
	{
		private string _cityUrl;
		private string _mainUrl = "https://www.tripadvisor.com.ph";

		public string CityUrl
		{
			get { return _cityUrl; }
			set { _cityUrl = value; }
		}

		public string MainUrl
		{
			get { return _mainUrl; }
		}
		public TripAdvisorScraper(string url)
		{
			CityUrl = url;
		}
		public void Process()
		{
			string thingsToDoURL = GetThingsToDoURL(CityUrl);

			var webGet = new HtmlWeb();
			HtmlDocument document = webGet.Load(thingsToDoURL);
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
					Console.WriteLine("ITINERARY: " + topPlace.InnerText);  //Get name
					var link = topPlace.Descendants("a").First().GetAttributeValue("href", null); //Get url
					var reviewLink = MainUrl + link;
					Console.WriteLine("URL: " + reviewLink);

					// Get Reviews
					document = webGet.Load(reviewLink);

					// Get full Review
					var newReviewNode = document.DocumentNode.Descendants("a").Where(d => d.Attributes.Contains("id") && d.Attributes["href"].Value.Contains("ShowUserReviews"));
					if (newReviewNode.Count() > 0) // if there are user reviews
					{
						var freviewLink = newReviewNode.First().GetAttributeValue("href", null); //get link of first review
						var fullreviewLink = MainUrl + freviewLink;
						document = webGet.Load(fullreviewLink);

						var fullReviewNodes = document.DocumentNode.Descendants("div").Where(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("innerBubble"));
						foreach (var review in fullReviewNodes)
						{
							var p = review.Descendants("p").Where(d => d.Attributes.Contains("id"));
							Console.WriteLine("REVIEW: " + p.First().InnerText);
						}

						Console.WriteLine(Environment.NewLine);
					}

					Console.WriteLine(Environment.NewLine);
				}
			}
			Console.ReadLine();
		}

		private string GetThingsToDoURL(string cityUrl)
		{
			string thingsToDoUrl = string.Empty;

			var webGet = new HtmlWeb();
			HtmlDocument document = webGet.Load(cityUrl);
			if (document != null)
			{
				var newReviewNode2 = document.DocumentNode.Descendants("li").Where(d => d.Attributes.Contains("data-element") && d.Attributes["data-element"].Value.Contains(".masthead-dropdown-attractions"));
				if (newReviewNode2.Count() > 0)
				{
					string tmp = newReviewNode2.First().FirstChild.GetAttributeValue("href", null);
					thingsToDoUrl = MainUrl + tmp;
				}
			}

			return thingsToDoUrl;
		}
	}
}
