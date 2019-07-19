using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TrainConnected.Data.Models;
using TrainConnected.Services.Mapping;

namespace TrainConnected.Web.ViewModels.Withdrawals
{
    public class WithdrawalsAllViewModel : IMapFrom<Withdrawal>
    {
        [Display(Name = "Transaction Id")]
        public string Id { get; set; }

        public decimal Amount { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; }
    }
}
