namespace TrainConnected.Data.Models
{
    using System.Collections.Generic;

    public interface ICoach
    {
        ICollection<Certificate> Certificates { get; set; }

        decimal Balance { get; set; }
    }
}
