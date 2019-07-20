namespace TrainConnected.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using TrainConnected.Data.Common.Models;

    public class Achievement : BaseDeletableModel<string>, IAchievement
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime AchievedOn { get; set; }

        [Required]
        [MaxLength(100)]
        public string Description { get; set; }

        public string TrainConnectedUserId { get; set; }
        public TrainConnectedUser TrainConnectedUser { get; set; }

    }
}
