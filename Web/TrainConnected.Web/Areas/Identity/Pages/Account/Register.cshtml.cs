namespace TrainConnected.Web.Areas.Identity.Pages.Account
{
    using System.ComponentModel.DataAnnotations;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.Extensions.Logging;
    using TrainConnected.Common;
    using TrainConnected.Data.Common.Models;
    using TrainConnected.Data.Models;

    [AllowAnonymous]
#pragma warning disable SA1649 // File name should match first type name
    public class RegisterModel : PageModel
#pragma warning restore SA1649 // File name should match first type name
    {
        private readonly SignInManager<TrainConnectedUser> signInManager;
        private readonly UserManager<TrainConnectedUser> userManager;
        private readonly ILogger<RegisterModel> logger;
        private readonly IEmailSender emailSender;

        public RegisterModel(
            UserManager<TrainConnectedUser> userManager,
            SignInManager<TrainConnectedUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public void OnGet(string returnUrl = null)
        {
            this.ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? this.Url.Content("~/");
            if (this.ModelState.IsValid)
            {
                var user = new TrainConnectedUser { UserName = this.Input.UserName, Email = this.Input.Email,
                                                PhoneNumber = this.Input.PhoneNumber,
                                                FirstName = this.Input.FirstName,
                                                LastName = this.Input.LastName, };

                var result = await this.userManager.CreateAsync(user, this.Input.Password);

                var userToAssignRole = await this.userManager.FindByIdAsync(user.Id);
                
                // All newly registered users are assigned the "TraineeUser" role.
                await this.userManager.AddToRoleAsync(userToAssignRole, GlobalConstants.TraineeRoleName);

                if (result.Succeeded)
                {
                    this.logger.LogInformation("User created a new account with password.");

                    var code = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = this.Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = code },
                        protocol: this.Request.Scheme);

                    await this.emailSender.SendEmailAsync(
                        this.Input.Email,
                        "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    await this.signInManager.SignInAsync(user, isPersistent: false);
                    return this.LocalRedirect(returnUrl);
                }

                foreach (var error in result.Errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return this.Page();
        }

        public class InputModel
        {
            [Required]
            [StringLength(ModelConstants.User.NameMaxLength, MinimumLength = ModelConstants.User.NameMinLength, ErrorMessage = ModelConstants.NameLengthError)]
            [Display(Name = ModelConstants.User.UserNameDisplay)]
            public string UserName { get; set; }

            [Required]
            [RegularExpression(ModelConstants.NameRegex, ErrorMessage = ModelConstants.NameRegexError)]
            [StringLength(ModelConstants.User.NameMaxLength, MinimumLength = ModelConstants.User.NameMinLength, ErrorMessage = ModelConstants.NameLengthError)]
            [Display(Name = ModelConstants.User.FirstNameDisplay)]
            public string FirstName { get; set; }

            [Required]
            [RegularExpression(ModelConstants.NameRegex, ErrorMessage = ModelConstants.NameRegexError)]
            [StringLength(ModelConstants.User.NameMaxLength, MinimumLength = ModelConstants.User.NameMinLength, ErrorMessage = ModelConstants.NameLengthError)]
            [Display(Name = ModelConstants.User.LastNameDisplay)]
            public string LastName { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [RegularExpression(ModelConstants.User.PhoneNumberRegex, ErrorMessage = ModelConstants.User.PhoneNumberRegexError)]
            [Display(Name = ModelConstants.User.PhoneNumberNameDisplay)]
            public string PhoneNumber { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [StringLength(ModelConstants.User.PasswordMaxLength, MinimumLength = ModelConstants.User.PasswordMinLength, ErrorMessage = ModelConstants.User.PasswordLengthError)]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            [StringLength(ModelConstants.User.PasswordMaxLength, MinimumLength = ModelConstants.User.PasswordMinLength, ErrorMessage = ModelConstants.User.PasswordLengthError)]
            public string ConfirmPassword { get; set; }
        }
    }
}
