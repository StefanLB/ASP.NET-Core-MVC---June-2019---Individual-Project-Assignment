namespace TrainConnected.Web.Areas.Identity.Pages.Account.Manage
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using TrainConnected.Data.Common.Models;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Data.Contracts;

#pragma warning disable SA1649 // File name should match first type name
    public class IndexModel : PageModel
#pragma warning restore SA1649 // File name should match first type name
    {
        private readonly UserManager<TrainConnectedUser> userManager;
        private readonly SignInManager<TrainConnectedUser> signInManager;
        private readonly IEmailSender emailSender;
        private readonly ICloudinaryService cloudinaryService;

        public IndexModel(
            UserManager<TrainConnectedUser> userManager,
            SignInManager<TrainConnectedUser> signInManager,
            IEmailSender emailSender,
            ICloudinaryService cloudinaryService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailSender = emailSender;
            this.cloudinaryService = cloudinaryService;
        }

        public string Username { get; set; }

        public string Email { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");
            }

            var userName = await this.userManager.GetUserNameAsync(user);
            var email = await this.userManager.GetEmailAsync(user);
            var phoneNumber = await this.userManager.GetPhoneNumberAsync(user);

            this.Input = new InputModel
            {
                UserName = userName,
                Email = email,
                PhoneNumber = phoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
            };

            this.IsEmailConfirmed = await this.userManager.IsEmailConfirmedAsync(user);
            this.Username = userName;
            this.Email = email;

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.ModelState.IsValid)
            {
                return this.Page();
            }

            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");
            }


            // If username and email are identical, the user will have to change their username prior to changing their email
            var email = await this.userManager.GetEmailAsync(user);
            if (this.Input.Email != email && email != user.UserName)
            {
                var setEmailResult = await this.userManager.SetEmailAsync(user, this.Input.Email);
                if (!setEmailResult.Succeeded)
                {
                    var userId = await this.userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting email for user with ID '{userId}'.");
                }
            }

            var phoneNumber = await this.userManager.GetPhoneNumberAsync(user);
            if (this.Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await this.userManager.SetPhoneNumberAsync(user, this.Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    var userId = await this.userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting phone number for user with ID '{userId}'.");
                }
            }

            var userName = await this.userManager.GetUserNameAsync(user);
            if (this.Input.UserName != userName && this.Input.UserName != email)
            {
                var userWithSameName = await this.userManager.FindByNameAsync(this.Input.UserName);

                // Username will only be updated if there is no other user with the same name in the database
                if (userWithSameName == null)
                {
                    var setuserNameResult = await this.userManager.SetUserNameAsync(user, this.Input.UserName);
                    if (!setuserNameResult.Succeeded)
                    {
                        var userId = await this.userManager.GetUserIdAsync(user);
                        throw new InvalidOperationException($"Unexpected error occurred setting username for user with ID '{userId}'.");
                    }
                }
            }

            if (this.Input.FirstName != null && this.Input.FirstName.Length >= 3)
            {
                user.FirstName = this.Input.FirstName;
            }

            if (this.Input.LastName != null && this.Input.LastName.Length >= 3)
            {
                user.LastName = this.Input.LastName;
            }

            string pictureUrl = string.Empty;
            if (this.Input.ProfilePicture != null)
            {
                pictureUrl = await this.cloudinaryService.UploadPictureAsync(
                    this.Input.ProfilePicture,
                    this.Input.UserName);

                user.ProfilePicture = pictureUrl;
            }

            await this.userManager.UpdateAsync(user);

            await this.signInManager.RefreshSignInAsync(user);
            this.StatusMessage = "Your profile has been updated";
            return this.RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            if (!this.ModelState.IsValid)
            {
                return this.Page();
            }

            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");
            }

            var userId = await this.userManager.GetUserIdAsync(user);
            var email = await this.userManager.GetEmailAsync(user);
            var code = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = this.Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = userId, code = code },
                protocol: this.Request.Scheme);
            await this.emailSender.SendEmailAsync(
                email,
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            this.StatusMessage = "Verification email sent. Please check your email.";
            return this.RedirectToPage();
        }

        public class InputModel
        {
            [Required]
            [StringLength(ModelConstants.User.NameMaxLength, MinimumLength = ModelConstants.User.NameMinLength, ErrorMessage = ModelConstants.NameLengthError)]
            [Display(Name = ModelConstants.User.UserNameDisplay)]
            public string UserName { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [Phone]
            [RegularExpression(ModelConstants.User.PhoneNumberRegex, ErrorMessage = ModelConstants.User.PhoneNumberRegexError)]
            [Display(Name = ModelConstants.User.PhoneNumberNameDisplay)]
            public string PhoneNumber { get; set; }

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

            [Display(Name = ModelConstants.User.ProfilePictureNameDisplay)]
            public IFormFile ProfilePicture { get; set; }
        }
    }
}
