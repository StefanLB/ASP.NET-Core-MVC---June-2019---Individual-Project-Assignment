using Microsoft.AspNetCore.Http;

namespace TrainConnected.Data.Models
{
    public interface IWorkoutActivity
    {
        string Name { get; set; }

        string Description { get; set; }

        string ActivityIcon { get; set; }
    }
}
