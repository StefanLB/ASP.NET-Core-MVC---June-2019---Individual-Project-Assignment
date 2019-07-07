namespace TrainConnected.Data.Models
{
    using System.Collections.Generic;

    public class Coach : Trainee, ICoach
    {
        public Coach()
            : base()
        {
            this.Certificates = new HashSet<Certificate>();
        }

        public ICollection<Certificate> Certificates { get; set; }

        public decimal Balance { get; set; }
    }
}
