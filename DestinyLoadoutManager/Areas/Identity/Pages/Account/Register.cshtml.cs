using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using DestinyLoadoutManager.Models;

namespace DestinyLoadoutManager.Areas.Identity.Pages.Account
{
    [IgnoreAntiforgeryToken]
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            Input = new InputModel();
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(50, ErrorMessage = "A felhasználónév legalább {2} és maximum {1} karakter hosszú lehet.", MinimumLength = 3)]
            [Display(Name = "Felhasználónév")]
            public string UserName { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            [Required]
            [StringLength(100, ErrorMessage = "A jelszó legalább {2} és maximum {1} karakter hosszú lehet.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Jelszó")]
            public string Password { get; set; } = string.Empty;
        }

        public async Task OnGetAsync()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation($"Registration attempt for user: {Input.UserName}, email: {Input.Email}");
            
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = Input.UserName, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"User {Input.UserName} created successfully in database.");

                    // Add User role by default
                    await _userManager.AddToRoleAsync(user, "User");
                    _logger.LogInformation($"User role added to {Input.UserName}.");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation($"User {Input.UserName} signed in automatically after registration.");
                    
                    return Redirect("/Loadout/Index");
                }
                else
                {
                    _logger.LogError($"Failed to create user {Input.UserName}. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            else
            {
                _logger.LogWarning($"ModelState invalid for registration: {Input.UserName}");
            }

            return Page();
        }
    }
}
