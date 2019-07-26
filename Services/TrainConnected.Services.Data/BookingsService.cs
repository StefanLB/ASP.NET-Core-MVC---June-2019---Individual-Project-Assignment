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
        private readonly IRepository<ITrainConnectedUsersWorkouts> trainConnectedUsersWorkoutsRepository;
        private readonly IRepository<PaymentMethod> paymentMethodsRepository;

        public BookingsService(IRepository<Workout> workoutsRepository, IRepository<TrainConnectedUser> usersRepository, IWorkoutsService workoutsService, IRepository<Booking> bookingsRepository, IRepository<ITrainConnectedUsersWorkouts> trainConnectedUsersWorkoutsRepository, IRepository<PaymentMethod> paymentMethodsRepository)
        {
            this.workoutsRepository = workoutsRepository;
            this.usersRepository = usersRepository;
            this.workoutsService = workoutsService;
            this.bookingsRepository = bookingsRepository;
            this.trainConnectedUsersWorkoutsRepository = trainConnectedUsersWorkoutsRepository;
            this.paymentMethodsRepository = paymentMethodsRepository;
        }

        public async Task<BookingDetailsViewModel> CreateAsync(BookingCreateInputModel bookingCreateInputModel, string userId)
        {
            var workout = await this.workoutsRepository.All()
                .FirstOrDefaultAsync(x => x.Id == bookingCreateInputModel.WorkoutId);

            if (workout == null)
            {
                throw new NullReferenceException();
            }

            var workoutBookings = await this.workoutsRepository.All()
                .Where(x => x.Id == bookingCreateInputModel.WorkoutId)
                .To<WorkoutDetailsViewModel>()
                .Select(b => b.BookingsCount)
                .FirstOrDefaultAsync();

            var user = await this.usersRepository.All()
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                throw new NullReferenceException();
            }

            var paymentMethod = await this.paymentMethodsRepository.All()
                .FirstOrDefaultAsync(pm => pm.Name == bookingCreateInputModel.PaymentMethod);

            if (paymentMethod == null)
            {
                throw new NullReferenceException();
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
                throw new InvalidOperationException();
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
                user.Balance += booking.Price;
                this.usersRepository.Update(user);
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
                // TODO: catch exception and redirect appropriately
                throw new NullReferenceException();
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
            }
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

        public async Task<BookingDetailsViewModel> GetDetailsAsync(string id)
        {
            var booking = await this.bookingsRepository.All()
                .Where(x => x.Id == id)
                .To<BookingDetailsViewModel>()
                .FirstOrDefaultAsync();

            if (booking == null)
            {
                throw new InvalidOperationException();
            }

            return booking;
        }
    }
}
