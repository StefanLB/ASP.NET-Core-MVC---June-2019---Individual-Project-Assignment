namespace TrainConnected.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using TrainConnected.Data.Common.Repositories;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Data.Contracts;
    using TrainConnected.Services.Mapping;
    using TrainConnected.Web.InputModels.Bookings;
    using TrainConnected.Web.ViewModels.Bookings;
    using TrainConnected.Web.ViewModels.Workouts;

    public class BookingsService : IBookingsService
    {
        private readonly IRepository<Workout> workoutsRepository;
        private readonly IRepository<TrainConnectedUser> usersRepository;
        private readonly IWorkoutsService workoutsService;
        private readonly IRepository<Booking> bookingsRepository;
        private readonly IRepository<TrainConnectedUsersWorkouts> trainConnectedUsersWorkoutsRepository;
        private readonly IRepository<PaymentMethod> paymentMethodsRepository;

        public BookingsService(IRepository<Workout> workoutsRepository, IRepository<TrainConnectedUser> usersRepository, IWorkoutsService workoutsService, IRepository<Booking> bookingsRepository, IRepository<TrainConnectedUsersWorkouts> trainConnectedUsersWorkoutsRepository, IRepository<PaymentMethod> paymentMethodsRepository)
        {
            this.workoutsRepository = workoutsRepository;
            this.usersRepository = usersRepository;
            this.workoutsService = workoutsService;
            this.bookingsRepository = bookingsRepository;
            this.trainConnectedUsersWorkoutsRepository = trainConnectedUsersWorkoutsRepository;
            this.paymentMethodsRepository = paymentMethodsRepository;
        }

        public async Task<IEnumerable<BookingsAllViewModel>> GetAllAsync(string userId)
        {
            var bookings = await this.bookingsRepository.All()
                .Where(x => x.TrainConnectedUserId == userId)
                .Where(t => t.Workout.Time > DateTime.UtcNow)
                .To<BookingsAllViewModel>()
                .OrderBy(x => x.WorkoutTime)
                .ThenByDescending(x => x.CreatedOn)
                .ToArrayAsync();

            return bookings;
        }

        public async Task<IEnumerable<BookingsAllViewModel>> GetAllHistoryAsync(string userId)
        {
            var bookings = await this.bookingsRepository.All()
                .Where(x => x.TrainConnectedUserId == userId)
                .Where(t => t.Workout.Time <= DateTime.UtcNow)
                .To<BookingsAllViewModel>()
                .OrderByDescending(x => x.WorkoutTime)
                .ThenByDescending(x => x.CreatedOn)
                .ToArrayAsync();

            return bookings;
        }

        public async Task<BookingDetailsViewModel> GetDetailsAsync(string id, string userId)
        {
            var booking = await this.bookingsRepository.All()
                .Where(x => x.Id == id)
                .To<BookingDetailsViewModel>()
                .FirstOrDefaultAsync();

            if (booking == null)
            {
                throw new NullReferenceException(string.Format(ServiceConstants.Booking.NullReferenceBookingId, id));
            }

            var bookedByUserId = await this.bookingsRepository.All()
                .Where(x => x.Id == id)
                .Select(x => x.TrainConnectedUserId)
                .FirstOrDefaultAsync();

            if (bookedByUserId != userId)
            {
                throw new ArgumentException(string.Format(ServiceConstants.Booking.ArgumentUserIdMismatch, userId));
            }

            return booking;
        }

        public async Task<BookingDetailsViewModel> GetDetailsByWorkoutIdAsync(string id, string userId)
        {
            var booking = await this.bookingsRepository.All()
                .Where(x => x.WorkoutId == id)
                .Where(x => x.TrainConnectedUserId == userId)
                .To<BookingDetailsViewModel>()
                .FirstOrDefaultAsync();

            if (booking == null)
            {
                throw new NullReferenceException(string.Format(ServiceConstants.Booking.NullReferenceWorkoutId, id, userId));
            }

            return booking;
        }

        public async Task<BookingDetailsViewModel> CreateAsync(BookingCreateInputModel bookingCreateInputModel, string userId)
        {
            var workout = await this.workoutsRepository.All()
                .FirstOrDefaultAsync(x => x.Id == bookingCreateInputModel.WorkoutId);

            if (workout == null)
            {
                throw new NullReferenceException(string.Format(ServiceConstants.Workout.NullReferenceWorkoutId, bookingCreateInputModel.WorkoutId));
            }

            var workoutBookings = await this.workoutsRepository.All()
                .Where(x => x.Id == bookingCreateInputModel.WorkoutId)
                .To<WorkoutDetailsViewModel>()
                .Select(b => b.BookingsCount)
                .FirstOrDefaultAsync();

            var user = await this.usersRepository.All()
                .FirstOrDefaultAsync(x => x.Id == userId);

            var paymentMethod = await this.paymentMethodsRepository.All()
                .FirstOrDefaultAsync(pm => pm.Name == bookingCreateInputModel.PaymentMethod);

            if (paymentMethod == null)
            {
                throw new NullReferenceException(string.Format(ServiceConstants.PaymentMethod.NullReferencePaymentMethodName, bookingCreateInputModel.PaymentMethod));
            }

            /*
             * Check if:
             * 1. Workout has not begun;
             * 2. User is not the coach;
             * 3. Workout is not fully booked;
             * 4. user has not yet booked the workout.
             */
            if (workout.Time <= DateTime.UtcNow ||
                workout.CoachId == userId ||
                workoutBookings >= workout.MaxParticipants ||
                user.Bookings.Any(x => x.WorkoutId == workout.Id))
            {
                throw new InvalidOperationException(string.Format(ServiceConstants.Booking.BookingCriteriaNotMet));
            }

            var booking = new Booking
            {
                TrainConnectedUser = user,
                TrainConnectedUserId = user.Id,
                PaymentMethod = paymentMethod,
                Price = bookingCreateInputModel.Price,
                WorkoutId = workout.Id,
                Workout = workout,
            };

            await this.trainConnectedUsersWorkoutsRepository.AddAsync(new TrainConnectedUsersWorkouts
            {
                TrainConnectedUserId = user.Id,
                WorkoutId = workout.Id,
            });
            await this.trainConnectedUsersWorkoutsRepository.SaveChangesAsync();

            await this.bookingsRepository.AddAsync(booking);
            await this.bookingsRepository.SaveChangesAsync();

            if (booking.PaymentMethod.PaymentInAdvance)
            {
                var coachUser = await this.usersRepository.All()
                    .FirstOrDefaultAsync(x => x.Id == workout.CoachId);

                coachUser.Balance += booking.Price;
                this.usersRepository.Update(coachUser);
                await this.usersRepository.SaveChangesAsync();
            }

            var bookingDetailsViewModel = AutoMapper.Mapper.Map<BookingDetailsViewModel>(booking);
            return bookingDetailsViewModel;
        }

        public async Task CancelAsync(string id)
        {
            var booking = await this.bookingsRepository.All()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (booking == null)
            {
                throw new NullReferenceException(string.Format(ServiceConstants.Booking.NullReferenceBookingId, id));
            }

            var workout = await this.workoutsRepository.All()
                .FirstOrDefaultAsync(x => x.Id == booking.WorkoutId);

            var user = await this.usersRepository.All()
                .FirstOrDefaultAsync(x => x.Id == booking.TrainConnectedUserId);

            var paymentMethod = await this.paymentMethodsRepository.All()
                .FirstOrDefaultAsync(x => x.Id == booking.PaymentMethodId);

            if (workout.Time > DateTime.UtcNow && !paymentMethod.PaymentInAdvance)
            {
                booking.IsDeleted = true;

                this.bookingsRepository.Update(booking);
                await this.bookingsRepository.SaveChangesAsync();

                this.usersRepository.Update(user);
                await this.usersRepository.SaveChangesAsync();

                var userWorkoutConnection = await this.trainConnectedUsersWorkoutsRepository.All()
                    .Where(u => u.TrainConnectedUserId == user.Id)
                    .Where(w => w.WorkoutId == workout.Id)
                    .FirstOrDefaultAsync();

                this.trainConnectedUsersWorkoutsRepository.Delete(userWorkoutConnection);
                await this.trainConnectedUsersWorkoutsRepository.SaveChangesAsync();
            }
        }
    }
}
