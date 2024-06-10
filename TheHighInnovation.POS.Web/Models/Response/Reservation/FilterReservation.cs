using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheHighInnovation.POS.Model.Response.Reservation
{
    public class FilterReservation
    {
        public DateTime ArrivalDate { get; set; } = DateTime.Now;
        public DateTime DepartureDate { get; set; } = DateTime.Now.AddMonths(1);
        public string SearchCustomer { get; set; } = "";
    }
}
