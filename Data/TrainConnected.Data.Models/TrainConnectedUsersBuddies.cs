using System;
using System.Collections.Generic;
using System.Text;

namespace TrainConnected.Data.Models
{
    public class TrainConnectedUsersBuddies : ITrainConnectedUsersBuddies
    {
        public string TrainConnectedUserId { get; set; }
        public TrainConnectedUser TrainConnectedUser { get; set; }

        public string TrainConnectedBuddyId { get; set; }
        public TrainConnectedUser TrainConnectedBuddy { get; set; }

        public DateTime AddedOn { get; set; }
    }
}
