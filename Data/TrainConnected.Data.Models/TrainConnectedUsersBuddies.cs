namespace TrainConnected.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using TrainConnected.Data.Models.Contracts;

    public class TrainConnectedUsersBuddies : ITrainConnectedUsersBuddies
    {
        [Required]
        public string TrainConnectedUserId { get; set; }

        public TrainConnectedUser TrainConnectedUser { get; set; }

        [Required]
        public string TrainConnectedBuddyId { get; set; }

        public TrainConnectedUser TrainConnectedBuddy { get; set; }

        [Required]
        public DateTime AddedOn { get; set; }
    }
}
