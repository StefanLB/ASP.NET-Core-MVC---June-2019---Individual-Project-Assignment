namespace TrainConnected.Data.Models
{
    using TrainConnected.Data.Common.Models;
    using TrainConnected.Data.Models.Contracts;

    public class WorkoutActivity : BaseDeletableModel<string>, IWorkoutActivity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Icon { get; set; }
    }
}
