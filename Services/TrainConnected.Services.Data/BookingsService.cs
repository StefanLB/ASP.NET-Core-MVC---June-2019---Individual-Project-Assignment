﻿namespace TrainConnected.Services.Data
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

        public BookingsService(IRepository<Workout> workoutsRepository, IRepository<TrainConnectedUser> usersRepository, IWorkoutsService workoutsService, IRepository<Booking> bookingsRepository, IRepository<ITrainConnectedUsersWorkouts> trainConnectedUsersWorkoutsRepository)
        {
            this.workoutsRepository = workoutsRepository;
            this.usersRepository = usersRepository;
            this.workoutsService = workoutsService;
            this.bookingsRepository = bookingsRepository;
            this.trainConnectedUsersWorkoutsRepository = trainConnectedUsersWorkoutsRepository;
        }

        public async Task<BookingDetailsViewModel> CreateAsync(BookingCreateInputModel bookingCreateInputModel, string userId)
        {
            // TODO: coaches should not sign up for their workouts, should not be able to sign up for a fully booked workout, user should not be able to sign up twice
            var workout = await this.workoutsRepository.All()
                .FirstOrDefaultAsync(x => x.Id == bookingCreateInputModel.WorkoutId);

            if (workout == null)
            {
                throw new NullReferenceException();
            }

            var user = await this.usersRepository.All()
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                throw new NullReferenceException();
            }

            // TODO: If user already has a booking which overlaps with this workout, send warning message
            if (user.Bookings.Any(x => x.WorkoutId == workout.Id))
            {
                // TODO: user already has booked this workout, do not allow a second booking, also try to hide the "Sign Up" button from them
                throw new NullReferenceException();
            }

            var booking = new Booking
            {
                TrainConnectedUser = user,
                TrainConnectedUserId = user.Id,
                PaymentMethod = (PaymentMethod)Enum.Parse(typeof(PaymentMethod), bookingCreateInputModel.PaymentMethod),
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

            if (workout.Time > DateTime.UtcNow)
            {
                booking.IsDeleted = true;
                user.Balance += booking.Price;

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

        public async Task<WorkoutDetailsViewModel> GetWorkoutByIdAsync(string id)
        {
            var workout = await this.workoutsService.GetBookingDetailsAsync(id);

            return workout;
        }
    }
}
