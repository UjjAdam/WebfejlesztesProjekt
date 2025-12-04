using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace DestinyLoadoutManager.Areas.Identity.Pages.Account
{
    public class TestModel : PageModel
    {
        public string? Message { get; set; }

        public void OnGet()
        {
            Message = "Test page loaded";
        }

        public IActionResult OnPost(string? testInput)
        {
            Message = $"POST received! Input: {testInput}";
            return Page();
        }
    }
}
