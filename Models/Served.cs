using System.Text.Json.Serialization;
using DiningVsCodeNew.Models;

namespace DiningVsCodeNew
{

    public class Served
    {
        public int Id { get; set; }
        public int paymentMainId { get; set; }
        public PaymentMain paymentMain { get; set; }
        public DateTime Dateserved { get; set; }
        public int ServedBy { get; set; }
        public bool isServed { get; set; }
        public User user { get; set; }
        public int totalAmount{ get; set; }
        public string server{ get; set; }
    }

}