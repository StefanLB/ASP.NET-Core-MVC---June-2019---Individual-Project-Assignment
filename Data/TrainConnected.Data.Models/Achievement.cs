namespace TrainConnected.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using TrainConnected.Data.Common.Models;

    public class Achievement : BaseDeletableModel<string>, IAchievement
    {
        [Required]
        public string Name { get; set; }

        public DateTime? FirstAchievedOn { get; set; }

        public DateTime? LastAchievedOn { get; set; }

        public int TimesAchieved { get; set; }

        [Required]
        [MaxLength(100)]
        public string Description { get; set; }

        public string TrainConnectedUserId { get; set; }
        public TrainConnectedUser TrainConnectedUser { get; set; }

    }
}
