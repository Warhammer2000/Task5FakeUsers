using System.Globalization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebAppFakeUsers.Models;
using WebAppFakeUsers.Services;

namespace WebAppFakeUsers.Pages
{
	public class IndexModel : PageModel
	{
		private readonly FakeUserDataService _dataService;
		public double Errors { get; set; }
		public string Seed { get; set; }
		public IList<FakeUser> FakeUsers { get; set; }
		[BindProperty] public string SelectedRegion { get; set; } = "USA";



		public IndexModel( FakeUserDataService dataService)
		{
			_dataService = dataService;
		}
		public void OnPost(string region, string seed, string errors)
		{
			SelectedRegion = region;
			
			Seed = seed;

			if (string.IsNullOrEmpty(seed))
			{
				seed = Guid.NewGuid().ToString();
			}
			
			int pageNumber = 1;
			Errors = double.Parse(errors, CultureInfo.InvariantCulture);


			FakeUsers = _dataService.GenerateFakeUsers(20, pageNumber, seed, Errors, region);
		}

		public void OnGet()
		{
			if (string.IsNullOrEmpty(Seed))
			{
				Seed = GenerateRandomSeed();
			}
			
			FakeUsers = _dataService.GenerateFakeUsers(20, 1, Seed, Errors, SelectedRegion);
		}

		public IActionResult OnGetMoreData(int pageNumber, int pageSize, string seed, string region, double errors)
		{
			if (string.IsNullOrEmpty(seed))
			{
				return BadRequest("Seed is required for data consistency.");
			}
			try
			{
				var startNumber = (pageNumber - 1) * pageSize;
				var users = _dataService.GenerateFakeUsers(pageSize, startNumber, seed, errors, region);

				return new JsonResult(users);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Internal server error: " + ex.Message);
			}
		}


		public HtmlString GetRegionsSelectHtml()
		{
			var tagBuilder = new TagBuilder("select");
			tagBuilder.Attributes.Add("id", "region");
			tagBuilder.Attributes.Add("name", "Region");
			tagBuilder.Attributes.Add("class", "form-control");

			var regions = new Dictionary<string, string>
			{
				{ "USA", "USA (English)" },
				{ "Poland", "Poland (Polish)" },
				{ "Uzbekistan", "Uzbekistan (Uzbek)" }
			};

			foreach (var region in regions)
			{
				var option = new TagBuilder("option");
				option.Attributes.Add("value", region.Key);
				if (SelectedRegion == region.Key)
				{
					option.Attributes.Add("selected", "selected");
				}
				option.InnerHtml.Append(region.Value);
				tagBuilder.InnerHtml.AppendHtml(option);
			}

			using (var writer = new StringWriter())
			{
				tagBuilder.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
				return new HtmlString(writer.ToString());
			}
		}

		private string GenerateRandomSeed()
		{
			return Guid.NewGuid().ToString().GetHashCode().ToString();
		}
	}
}
