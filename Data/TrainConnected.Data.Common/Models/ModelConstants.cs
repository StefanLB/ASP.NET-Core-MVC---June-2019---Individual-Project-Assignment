namespace TrainConnected.Data.Common.Models
{
    public class ModelConstants
    {
        public const int DefaultPageNumber = 1;
        public const int DefaultPageSize = 6;
        public const string NameRegex = "^[A-Z]\\D+[a-z]$";
        public const string PriceMin = "0";
        public const string PriceMax = "79228162514264337593543950335";

        public const string DescriptionLengthError = "Description must be between {2} and {1} symbols";
        public const string NameLengthError = "Name must be between {2} and {1} symbols";
        public const string NameRegexError = "Name must start with upper case and end with lower case";
        public const string PriceRangeError = "Price must be a positive value";

        public class Achievement
        {
            public const int NameMinLength = 3;
            public const int NameMaxLength = 50;

            public const int DescriptionMaxLength = 500;
            public const int DescriptionMinLength = 10;

            public const string AchievedOnNameDisplay = "Achieved On";
        }

        public class Booking
        {
            public const string PaymentMethodNameDisplay = "Payment Method";
            public const string PaymentMethodPIANameDisplay = "Paid in Advance";
            public const string CreatedOnNameDisplay = "Created On";
            public const string ActivityNameDisplay = "Activity";
            public const string CoachNameDisplay = "Coach";
            public const string TimeNameDisplay = "Start";
            public const string LocationNameDisplay = "Location";
            public const string DurationNameDisplay = "Duration";
            public const string NotesNameDisplay = "Notes";
            public const string SignedUpNameDisplay = "Currently Signed Up";
            public const string MaxParticipantsNameDisplay = "Max Participants";
        }

        public class Certificate
        {
            public const int DescriptionMaxLength = 500;
            public const int DescriptionMinLength = 10;
            public const int IsuedByMinLength = 3;
            public const int IssuedByMaxLength = 50;
            public const string ActivityNameDisplay = "Activity";
            public const string IssuedByNameDisplay = "Issued By";
            public const string IssuedOnNameDisplay = "Issued On";
            public const string ExpiresOnNameDisplay = "Expires On";

            public const string ExpiresOnError = "Certificate expiration date must be a future or current date";
            public const string IssuedByLengthError = "Issuer name must be between {2} and {1} symbols";
            public const string IssuedOnError = "Certificate issue date must be a past or current date";
        }

        public class PaymentMethod
        {
            public const int NameMinLength = 3;
            public const int NameMaxLength = 50;
            public const string PaymentInAdvanceNameDisplay = "Payment In Advance";
        }

        public class User
        {
            public const int NameMinLength = 3;
            public const int NameMaxLength = 30;

            public const int PasswordMinLength = 6;
            public const int PasswordMaxLength = 30;
            public const string PasswordLengthError = "Password must be between {2} and {1} symbols";

            public const string PhoneNumberRegex = "^[\\+]?[\\d ]{3,}$";
            public const string PhoneNumberRegexError = "Phone number can only contain digits and spaces and may start with a digit or \"+\"";

            public const string UserNameDisplay = "Username";
            public const string UserIdNameDisplay = "User Id";
            public const string FirstNameDisplay = "First Name";
            public const string LastNameDisplay = "Last Name";
            public const string WorkoutsCountNameDisplay = "Workouts";
            public const string AchievementsNameDisplay = "Achievements";
            public const string JoinedOnNameDisplay = "Joined On";
            public const string AddedOnNameDisplay = "Buddies Since";
            public const string WorkoutsCoachedNameDisplay = "Workouts Coached";
            public const string PhoneNumberNameDisplay = "Phone Number";
            public const string LockedOutNameDisplay = "Locked Until";
            public const string RolesNameDisplay = "Assigned Roles";
        }

        public class Withdrawal
        {
            public const int AdditionalInstructionsMaxLength = 150;
            public const int ResolutionNotesMaxLength = 150;
            public const string AmountMin = "0.01";
            public const string AmountNameDisplay = "Withdrawal Request Amount";
            public const string AdditionalInstructionsNameDisplay = "Additional Instructions";
            public const string IdNameDisplay = "Transaction Id";
            public const string UserNameDisplay = "User";
            public const string UserIdNameDisplay = "User Id";
            public const string ResolutionNotesNameDisplay = "Resolution Notes";
            public const string CreatedOnNameDisplay = "Created On";
            public const string TrainConnectedUserUserNameDisplay = "Created By";
            public const string CompletedOnNameDisplay = "Completed On";
            public const string ProcessedByUserUserNameDisplay = "Completed By";

            public const string AdditionalInstructionsLengthError = "Additional instructions must be less than {1} symbols";
            public const string ResolutionNotesLengthError = "Resolution notes must be less than {1} symbols";
            public const string AmountError = "Withdrawal request amount cannot be greater than withdrawable amount";
            public const string NegativeAmountError = "Amount must be positive";
        }

        public class Workout
        {
            public const int DurationMin = 10;
            public const int DurationMax = 180;
            public const int LocationNameMinLength = 3;
            public const int LocationNameMaxLength = 100;
            public const int NotesMaxLength = 150;
            public const int ParticipantsMin = 1;
            public const int ParticipantsMax = 99;

            public const string ActivityNameDisplay = "Activity";
            public const string BookingsCountNameDisplay = "Signed Up";
            public const string CoachNameDisplay = "Coach";
            public const string CreatedOnNameDisplay = "Created On";
            public const string MaxParticipantsNameDisplay = "Max Participants";
            public const string PaymentMethodsNameDisplay = "Accepted Payment Methods";
            public const string TimeNameDisplay = "Start";

            public const string DurationRangeError = "Workout duration must be between {1} and {2} minutes";
            public const string LocationRangeError = "Workout location must be between {2} and {1} symbols";
            public const string NotesLengthError = "Notes cannot be longer than {1} symbols";
            public const string ParticipantsRangeError = "Number of participants must be between {1} and {2}";
            public const string PaymentMethodsError = "Please select at least one payment method";
            public const string TimeError = "Beginning of workout must be a future point in time";
        }

        public class WorkoutActivity
        {
            public const int NameMinLength = 3;
            public const int NameMaxLength = 50;

            public const int DescriptionMaxLength = 500;
            public const int DescriptionMinLength = 10;

        }
    }
}
