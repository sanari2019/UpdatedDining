// Define a model class representing the data expected from the frontend
using DiningVsCodeNew;
using DiningVsCodeNew.Models;

public class UpdateServedReq
{
    public Served[] served { get; set; }
    public ServedEmail servedEmail { get; set; }
    public int loggedinuser { get; set; }
    public User user { get; set; }

}
