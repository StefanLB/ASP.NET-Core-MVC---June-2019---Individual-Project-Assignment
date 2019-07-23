﻿namespace TrainConnected.Web.Areas.Administration.InputModels.WorkoutActivities
{
    using System.ComponentModel.DataAnnotations;
    using TrainConnected.Data.Models;
    using TrainConnected.Services.Mapping;

    public class WorkoutActivityCreateInputModel : IMapFrom<WorkoutActivity>
    {
        [Required]
        [StringLength(100, ErrorMessage = "Field cannot contain more than 100 characters")]
        public string Name { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Field cannot contain more than 100 characters")]
        public string Description { get; set; }
    }
}