using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DestinyLoadoutManager.Pages
{
    public class TestFormModel : PageModel
    {
        public string? Message { get; set; }

        public void OnGet()
        {
            Message = null;
        }

        public void OnPost(string? testInput)
        {
            Message = $"POST received successfully! Input: '{testInput}'";
            System.Console.WriteLine($"=== TestForm OnPost called with: {testInput} ===");
        }
    }
}
