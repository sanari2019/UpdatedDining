using Microsoft.AspNetCore.Mvc;
using DiningVsCodeNew;


namespace DiningVsCodeNew.Controllers;

[ApiController]
[Route("[controller]")]
public class ServedController : ControllerBase
{

    ServedRepository repserv;
    private EmailConfiguration _emailConfig;
    private UserRepository _userRepository;
    private PaymentMainRepository _paymentmainRepository;
    int idvalue = 0;
    public ServedController(ServedRepository repserv, EmailConfiguration emailConfig, UserRepository userRepository, PaymentMainRepository paymentmainRepository)
    {
        this.repserv = repserv;
        this._emailConfig = emailConfig;
        this._userRepository = userRepository; 
        this._paymentmainRepository = paymentmainRepository;
    }
    // GET: api/Cities
    [HttpGet]
    public async Task<ActionResult> GetServed()
    {
        return new OkObjectResult(repserv.GetServeds());
    }
    [HttpPost("getServedbyCustomer")]
    public async Task<ActionResult> GetServed([FromBody] User us)
    {
        return new OkObjectResult(repserv.GetServedbyCustomer(us.id));
    }
    [HttpPost("getServedemail")]
    public async Task<ActionResult<ServedEmail>> servedEmail([FromBody] ServedEmail served)
    {
        Email em = new Email();
        string logourl = "";//"https://evercaregroup.com/wp-content/uploads/2020/12/EVERCARE_LOGO_03_LEKKI_PRI_FC_RGB.png";
        string applink = "https://cafeteria.evercare.ng";
        EmailSender _emailSender = new EmailSender(this._emailConfig);
        string salutation = "Dear " + served.customerFirstName + ",";
        string emailcontent = "Meal worth: " + "NGN" + served.amount.ToString("N") + " has been served by: " + served.serversName + ". Thanks for visiting Evercare's cafeteria";
        string narration1 = " ";
        string econtent = em.HtmlMail("Served Meal Details", applink, salutation, emailcontent, narration1, logourl);
        var message = new Message(new string[] { served.customerUserName }, "Cafeteria Application", econtent);
        await _emailSender.SendEmailAsync(message);
        return served;

    }
    // GET: api/Cities/5
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetServed(int id)
    {
        return new OkObjectResult(repserv.GetServed(id));

    }

    // [HttpGet("{getTopNServed")]
    // public async Task<ActionResult<IEnumerable<Served>>> GetTopNServed([FromBody] User us)
    // {
    //     return repserv.GetTopNServed(us.id);
    // }
    [HttpGet("getHistoryRecords/{ServedBy}")]
    public IActionResult GetHistoryRecords(int ServedBy)
    {
        try
        {
            var historyRecords = repserv.GetHistoryRecords(ServedBy);

            if (historyRecords == null)
            {
                return NotFound(); // Return a 404 Not Found response if no records are found
            }

            return Ok(historyRecords); // Return a 200 OK response with the history records
        }
        catch (Exception ex)
        {
            // Handle any exceptions or errors here
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }


    [HttpPost("{id}")]
    public async Task<IActionResult> PutServed(int id, Served serv)
    {
        if (id != serv.Id)
        {
            return BadRequest();
        }
        // _context.Entry(state).State = EntityState.Modified;

        repserv.updateServed(serv);
        return new OkObjectResult(serv);

    }
//     [HttpPost("updateServed")]
// public async Task<IActionResult> PutPymtMain([FromBody] Served[] serv)
// {
//     if (serv == null || serv.Length == 0)
//     {
//         return BadRequest("Invalid request body");
//     }

//     var updatedServes = new List<Served>();

//     foreach (var serve in serv)
//     {
//         serve.isServed = true; // Set isServed=true
//         serve.ServedBy = 0; // Set ServedBy to user ID
//         serve.server = serv[0].server; // Set server to the first serve's server name

//         // Update the serve object in the repository
//         repserv.updateServed(serve);

//         updatedServes.Add(serve);
//     }

//     var user = _userRepository.GetUser(serv[0].user.id);
//     user.freeze=true;
//     // Update the user in the repository
//     _userRepository.updateUser(user);

//     // Send asynchronous email notifications
//     Task.Run(() => SendServedEmailAsync(updatedServes));

//     return Ok(updatedServes);
// }



[HttpPost("updateServed")]
public async Task<IActionResult> PutPymtMain([FromBody] Served[] serv)
{
    if (serv == null || serv.Length == 0)
    {
        return BadRequest("Invalid request body");
    }

    // var pymt = _paymentmainRepository.GetPymt()

    float totalAmount = 0;
    // var server= _userRepository.GetUser(serv[0].ServedBy);

    foreach (var serve in serv)
    {
        // var pymt = _paymentmainRepository.GetPymt(serve.paymentMainId);
        // Update serve properties
       serve.isServed = true; // Set isServed=true
        serve.ServedBy = serv[0].ServedBy; // Set ServedBy to user ID
        serve.server = serv[0].user.firstName;  // Set server from the first item in the array

        // Calculate total amount for this serve
        totalAmount = totalAmount + serve.paymentMain.Amount;
          // Update the serve objects in the repository
        repserv.updateServed(serve);
    }

    // Update the serve objects in the repository
    // repserv.UpdateServed(serv);


     var pymtusa =_userRepository.GetUser(serv[0].user.id);
     pymtusa.freeze=true;
    // serv[0].user.freeze=true;
    // Update the user in the repository
    _userRepository.updateUser(serv[0].user);
    int enteredById =int.Parse( serv[0].paymentMain.EnteredBy);
     var user = _userRepository.GetUser(enteredById);

    // Send email asynchronously
    Task.Run(() => SendServedEmailAsync(serv, totalAmount, user));

    // Return the updated serve objects with the calculated total amount
    return new OkObjectResult(new { Served = serv, TotalAmount = totalAmount });
}



    private async Task SendServedEmailAsync(Served[] served, float totalAmount, User pymtuser)
        {
            

            Email em = new Email();
            string logourl = "";//"https://evercaregroup.com/wp-content/uploads/2020/12/EVERCARE_LOGO_03_LEKKI_PRI_FC_RGB.png";
            string applink = "https://cafeteria.evercare.ng";
            EmailSender _emailSender = new EmailSender(this._emailConfig);
            string salutation = "Dear " + pymtuser.firstName + ",";
            string emailcontent = "Meal worth: " + "NGN" + totalAmount.ToString("N") + " has been served by: " + served[0].user.firstName + ". Thanks for visiting Evercare's cafeteria";
            string narration1 = " ";
            string econtent = em.HtmlMail("Served Meal Details", applink, salutation, emailcontent, narration1, logourl);
            var message = new Message(new string[] { pymtuser.userName }, "Cafeteria Application", econtent);
            await _emailSender.SendEmailAsync(message);
        }
    //return NoContent();
    // POST: api/Cities
    // To protect from overposting attacks, see https://go.microsoft.com/
    //fwlink /? linkid = 2123754
    [HttpPost]
    public async Task<ActionResult<User>> PostServ(PaymentMain pymtmm)
    {

        if(pymtmm == null){
            return BadRequest("");
        }
        var pymadet= repserv.GetPymtUnserved(pymtmm.Id);
        if(pymadet.RemainingBalance>0){
            //create new Serve Object and initialize the serve properties to pymmt property
            var serve = new Served
            {
                paymentMainId = pymtmm.Id,
                Dateserved = DateTime.Now, // Set the serve date to current date/time
                ServedBy = 0, // Assuming EnteredBy is the user ID
                isServed = false, // Mark as served
                // User = null, // You can set the user details based on your authentication system
                // TotalAmount = pymtmm.Unit * pymtmm.Amount, // Calculate total amount served
                // Server = "Server Name" // Set the server's name or identifier
            };
            repserv.insertServed(serve);
            return Ok(serve);
        }
       
        return BadRequest("Insufficient remaining balance for serving");

       

    }
    // DELETE: api/Cities/5
    [HttpPost("deleteserved")]
    public async Task<IActionResult> Delete([FromBody] Served serv)
    {

        Served sv = new Served();
        sv.Id = serv.Id;
        sv.Dateserved = serv.Dateserved;
        sv.isServed = serv.isServed;
        // sv.paymentMain=serv.paymentMain;        
        idvalue = repserv.deleteServed(sv);
        return Ok(serv);
    }
    //private bool StateExists(int id)
    // {
    // return _context.States.Any(e => e.Id == id);
    // }

    [HttpGet("ServedReport")]
    public ActionResult<IEnumerable<ServedReportModel>> GetServedReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var servedReport = repserv.GetServedReport(startDate, endDate);
        return Ok(servedReport);
    }
    [HttpGet("UnservedReport")]
    public ActionResult<IEnumerable<UnservedReportModel>> GetUnservedReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var unservedReport = repserv.GetUnservedReport(startDate, endDate);
        return Ok(unservedReport);
    }
    [HttpGet("ServedSummaryReport")]
    public ActionResult<IEnumerable<ServedSummaryReportModel>> GetServedSummaryReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var servedSummaryReport = repserv.GetServedSummaryReport(startDate, endDate);
        return Ok(servedSummaryReport);
    }

    [HttpGet("ServedMealsCount")]
    public ActionResult<int> GetServedMealsCount()
    {
        int servedcount = repserv.GetServedMealsCount();
        return Ok(servedcount);
    }

    [HttpGet("BreakfastCount")]
    public ActionResult<int> GetBreakfastCount()
    {
        int breakfastCount = repserv.GetBreakfastCount();
        return Ok(breakfastCount);
    }

    [HttpGet("LunchCount")]
    public ActionResult<int> GetLunchCount()
    {
        int lunchCount = repserv.GetLunchCount();
        return Ok(lunchCount);
    }

    [HttpGet("DinnerCount")]
    public ActionResult<int> GetDinnerCount()
    {
        int dinnerCount = repserv.GetDinnerCount();
        return Ok(dinnerCount);
    }



}