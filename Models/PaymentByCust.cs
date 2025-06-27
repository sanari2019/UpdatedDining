using DiningVsCodeNew.Models;
using Sieve.Attributes;

namespace DiningVsCodeNew
{

    public class PaymentByCust
    {
        [Sieve(CanSort = true, CanFilter = true)]
        public string enteredBy { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string custCode { get; set; }
        public float totalAmount { get; set; }

        [Sieve(CanSort = true, CanFilter = true)]
        public string EnteredbyName { get; set; }

        public int RemainingAmountUnserved { get; set; }

        public bool Freeze { get; set; }
    }

}