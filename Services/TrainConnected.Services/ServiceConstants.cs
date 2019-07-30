namespace TrainConnected.Services
{
    public class ServiceConstants
    {
        public class Achievement
        {
            public const string FirstWorkoutAchievementName = "First Workout";
            public const string GettingStartedAchievementName = "Getting Started";
            public const string VeteranAchievementName = "Veteran";
            public const string FirstResponderAchievementName = "First Responder";
            public const string CuttingItCloseAchievementName = "Cutting it Close";
            public const string BigSpenderAchievementName = "Big Spender";
            public const string AdventurerAchievementName = "Adventurer";
            public const string DoubleTroubleAchievementName = "Double Trouble";
            public const string CantGetEnoughAchievementName = "Can't Get Enough";
            public const string MedicAchievementName = "MEDIC!!!";
            public const string EarlyBirdAchievementName = "Early Bird";
            public const string NightOwlAchievementName = "Night Owl";

            public const string FirstWorkoutAchievementDescription = "Your first ever workout in TrainConnected!";
            public const string GettingStartedAchievementDescription = "Attend 10 workouts, regardless of activity type!";
            public const string VeteranAchievementDescription = "Attend 100 workouts, regardless of activity type!";
            public const string FirstResponderAchievementDescription = "Be the first person to sign up for 10 different workouts!";
            public const string CuttingItCloseAchievementDescription = "Sign up for the last spot for 10 different workouts!";
            public const string BigSpenderAchievementDescription = "Spend over BGN 1000.00 on workouts!";
            public const string AdventurerAchievementDescription = "Sign up for 10 different types of workouts!";
            public const string DoubleTroubleAchievementDescription = "Attend two workouts in one day!";
            public const string CantGetEnoughAchievementDescription = "Attend three workouts in one day!";
            public const string MedicAchievementDescription = "Attend four workouts in one day!";
            public const string EarlyBirdAchievementDescription = "Attend a workout before 08:00 a.m.!";
            public const string NightOwlAchievementDescription = "Attend a workout after 08:00 p.m.!";

            public const string NullReferenceAchievementId = "Achievement with id {0} not found.";
            public const string ArgumentUserIdMismatch = "Achievement does not belong to userId {0}.";
        }

        public class Booking
        {
            public const string ArgumentUserIdMismatch = "Booking does not belong to userId {0}.";

            public const string NullReferenceBookingId = "Booking with id {0} not found.";
            public const string NullReferenceWorkoutId = "No Booking found based on provided Workout {0} and UserId {1} parameters.";

            public const string BookingCriteriaNotMet = "User does not meet the booking requirements.";
        }

        public class Certificate
        {
            public const string ArgumentUserIdMismatch = "Certificate does not belong to userId {0}.";

            public const string NullReferenceCertificateId = "Certificate with id {0} not found.";
            public const string NullReferenceWorkoutActivityName = "Workout Activity with name {0} does not exist.";
        }

        public class PaymentMethod
        {
            public const string NullReferencePaymentMethodName = "Payment method with name {0} not found.";
            public const string NullReferencePaymentMethodId = "Payment method with id {0} not found.";

            public const string PaymentMethodNameAlreadyExists = "Payment method with name {0} already exists.";
        }

        public class User
        {
            public const string ArgumentUserBuddyMismatch = "UserId {0} is not in userId {1}'s buddy list.";

            public const string NullReferenceUserId = "User with id {0} not found.";
            public const string NullReferenceCoachName = "Coach with Username {0} not found.";
            public const string NullReferenceRoleName = "Role with name {0} does not exist.";

            public const string BefriendingCriteriaNotMet = "User - Buddy connection request is invalid.";
        }

        public class Withdrawal
        {
            public const string NullReferenceWithdrawalId = "Withdrawal with id {0} does not exist.";

            public const string RequestedAmountGreaterThanWithdrawable = "Withdrawal request amount is greater than withdrawable amount.";
        }

        public class Workout
        {
            public const string ArgumentUserIdMismatch = "Workout not created by userId {0}.";

            public const string NullReferenceWorkoutId = "Workout with id {0} not found.";
            public const string NullReferenceActivityName = "Workout Activity with name {0} not found.";
            public const string NullReferencePaymentMethodName = "No payment methods with the specified names found.";

            public const string WorkoutCancelCriteriaNotMet = "Workout cannot be canceled as bookings have already been made.";
        }

        public class WorkoutActivity
        {
            public const string NullReferenceActivityId = "Workout Activity with id {0} not found.";

            public const string SameNameActivityExists = "Activity with the same name already exists.";
        }
    }
}
