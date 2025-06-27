// Define a model class representing the data expected from the frontend
using DiningVsCodeNew;
using DiningVsCodeNew.Models;

public class OnlinePaymentRequest
{
    public OnlinePayment payment { get; set; }
    public PaymentMain pymt { get; set; }
    public OrderedMeal[] orderedMeals { get; set; }
    public PaymentMain[] pymnt { get; set; }

    public User user { get; set; }
}
