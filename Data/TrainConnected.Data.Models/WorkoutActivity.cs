namespace TrainConnected.Data.Models
{
    using Microsoft.AspNetCore.Http;
    using TrainConnected.Data.Common.Models;

    public class WorkoutActivity : BaseDeletableModel<string>, IWorkoutActivity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Picture { get; set; }
    }
}
