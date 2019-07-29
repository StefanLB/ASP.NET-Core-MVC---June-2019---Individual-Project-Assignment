﻿namespace TrainConnected.Data.Common.Models
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
        }

        public class Certificate
        {
            public const int DescriptionMaxLength = 500;
            public const int DescriptionMinLength = 10;
            public const int IsuedByMinLength = 3;
            public const int IssuedByMaxLength = 50;

            public const string ExpiresOnError = "Certificate expiration date must be a future or current date";
            public const string IssuedByLengthError = "Issuer name must be between {2} and {1} symbols";
            public const string IssuedOnError = "Certificate issue date must be a past or current date";
        }

        public class PaymentMethod
        {
            public const int NameMinLength = 3;
            public const int NameMaxLength = 50;
        }

        public class Withdrawal
        {
            public const int AdditionalInstructionsMaxLength = 500;

            public const string AdditionalInstructionsLengthError = "Additional instructions must be less than {1} symbols";
        }

        public class Workout
        {
            public const int DurationMin = 10;
            public const int DurationMax = 180;
            public const int ParticipantsMin = 1;
            public const int ParticipantsMax = 99;

            public const string DurationRangeError = "Workout duration must be between {2} and {1} minutes";
            public const string ParticipantsRangeError = "Maximum number of participants must be between {2} and {1}";
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
