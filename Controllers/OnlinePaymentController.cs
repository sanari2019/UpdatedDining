using Microsoft.AspNetCore.Mvc;
using DiningVsCodeNew.Models;
using RepoDb.Enumerations;


namespace DiningVsCodeNew.Controllers;

[ApiController]
[Route("[controller]")]
public class OnlinePaymentController : ControllerBase
{
    private OnlinePaymentRepository reponlinePayment;
    private UserRepository repuser;
    private PaymentMainRepository _pymtmain;
    private OrderedMealRepository _orderedMeal;
    int idvalue = 0;
    private EmailConfiguration _emailConfig;
    public OnlinePaymentController(OnlinePaymentRepository reponlinePymt, EmailConfiguration emailConfig, UserRepository repUser, PaymentMainRepository pymtmain, OrderedMealRepository orderedMeal)
    {
        this.reponlinePayment = reponlinePymt;
        this._emailConfig = emailConfig;
        this.repuser = repUser;
        this._pymtmain = pymtmain;
        _orderedMeal = orderedMeal;
    }
    // GET: api/Cities
    [HttpGet]
    public async Task<ActionResult> GetOnlinePayments()
    {
        return new OkObjectResult(reponlinePayment.GetOnlinePayments());
    }
    // GET: api/Cities/5
    [HttpGet("{id}")]
    public async Task<ActionResult<OnlinePayment>> GetOnlinePayment(int id)
    {
        return new OkObjectResult(reponlinePayment.GetOnlinePayment(id));

    }

    //  [HttpGet("getuser/{userName}")]
    //  public async Task<ActionResult<User>> GetUser(string username)
    // {
    //     return new OkObjectResult(repuser.GetUser(username));  
    // }
    // PUT: api/users/5
    // To protect from overposting attacks, see https://go.microsoft.com/
    // fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutOnlinePayment(int id, OnlinePayment onlinePymt)
    {
        if (id != onlinePymt.Id)
        {
            return BadRequest();
        }
        // _context.Entry(state).State = EntityState.Modified;

        this.reponlinePayment.updateOnlinePayment(onlinePymt);
        return new OkObjectResult(onlinePymt);

        // catch (DbUpdateConcurrencyException)
        // {
        // if (!StateExists(id))
        //  {
        //  return NotFound();
        // }
        // else
        //  {
        // throw;
        // }
    }
    //return NoContent();
    // POST: api/Cities
    // To protect from overposting attacks, see https://go.microsoft.com/
    //fwlink /? linkid = 2123754

    public class OnlinePaymentDto
    {
        public OnlinePayment OnlinePaymentData { get; set; }
        public PaymentMain PaymentMainData { get; set; }
    }
    [HttpPost]
    public ActionResult<OnlinePaymentDto> PostOnlinePayment(OnlinePaymentRequest requestData)
    {

        if (requestData != null)
        {

            // User us = new User();
            var us = repuser.GetUser(requestData.payment.Paidby);
            reponlinePayment.insertOnlinePayment(requestData.payment);
            // PaymentMain py = requestData.pymt;
            // py.Amount = onlinePymt.AmountPaid;
            if (requestData.payment != null)
            {

                if (requestData.orderedMeals.Any())
                {
                    requestData.pymt.Paid = true;
                    requestData.pymt.DateEntered = DateTime.Now;
                    requestData.pymt.timepaid = DateTime.Now;
                    requestData.pymt.DateServed = DateTime.Now;
                    requestData.pymt.opaymentid = requestData.payment.Id;
                    _pymtmain.insertPaymentMain(requestData.pymt);
                    if (requestData.pymt.Id != 0)
                    {
                        foreach (var order in requestData.orderedMeals)
                        {
                            order.Submitted = true;
                            order.paymentMainId = requestData.pymt.Id;
                            _orderedMeal.updateOrderedMeal(order);

                        }
                    }
                }
                else if (requestData.pymnt.Any())
                {
                    foreach (var pyt in requestData.pymnt)
                    {
                        pyt.opaymentid = requestData.payment.Id;
                        pyt.Paid = true;
                        pyt.timepaid = DateTime.Now;
                        pyt.DateServed = DateTime.Now;
                        _pymtmain.updatePaymentMain(pyt);
                    }

                }
                us.freeze = true;
                this.repuser.updateUser(us);

            }

            Task.Run(() =>  SuccessPaymentEmailAsync(us, requestData));
            return Ok(requestData);
        }
        return BadRequest("Invalid onlinepayment data.");

    }

        private async Task SuccessPaymentEmailAsync(User us, OnlinePaymentRequest requestData)
        {
            EmailSender _emailSender = new EmailSender(this._emailConfig);
            Email em = new Email();
            string logourl = "";//"https://evercaregroup.com/wp-content/uploads/2020/12/EVERCARE_LOGO_03_LEKKI_PRI_FC_RGB.png";
            string applink = "https://cafeteria.evercare.ng";
            string salutation = "Dear " + us.firstName + ",";
            string emailcontent = "We have successfully received your payment of: " + "NGN" + requestData.payment.AmountPaid.ToString("N") + " Thanks for visiting Evercare's cafeteria";
            string narration1 = " ";
            string econtent = em.HtmlMail("Payment Confirmation", applink, salutation, emailcontent, narration1, logourl);
            var message = new Message(new string[] { us.userName }, "Cafeteria Application", econtent);
            await _emailSender.SendEmailAsync(message);
           
        }
    // DELETE: api/Cities/5
    [HttpPost("deleteonlinePayment")]
    public async Task<IActionResult> Delete([FromBody] OnlinePayment onlinePymt)
    {

        // if (idvalue != us.id)
        //  {
        //  return NotFound();
        //  }
        idvalue = reponlinePayment.deleteOnlinePayment(onlinePymt);
        return Ok(onlinePymt);
    }
    //private bool StateExists(int id)
    // {
    // return _context.States.Any(e => e.Id == id);
    // }
    [HttpGet("TotalRevenue")]
    public ActionResult<IEnumerable<TotalRevenueModel>> GetTotalRevenue([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var totalRevenue = reponlinePayment.GetTotalRevenue(startDate, endDate);
        return Ok(totalRevenue);
    }

    // GET: api/OnlinePayment/GetOnlinePaymentByRefNo/{transRefNo}
    [HttpGet("GetOnlinePaymentByRefNo/{transRefNo}")]
    public ActionResult<OnlinePayment> GetOnlinePaymentByRefNo(string transRefNo)
    {
        var onlinePayment = reponlinePayment.GetOnlinePaymentByRefNo(transRefNo);
        if (onlinePayment == null)
        {
            return NotFound();
        }
        return Ok(onlinePayment);
    }
//edited
        
        // [HttpGet("validate")]
        // public IActionResult DeleteSampleBiobank(Transactioning transactioning){
        //     // var onlinePayment = reponlinePayment.GetOnlinePaymentByRefNo(transactioning.reference); 
            
        //     // if(onlinePayment != null){
        //     //     var paymentNodes = _pymtmain.GetPaymentbyOpaymentId(onlinePayment.id);
        //     //     if(paymentNodes == null){
        //     //         paymentNodes = _
        //     //         return NotFound("No payment with this ");
        //     //     }

        //     // }else{
        //     //     return OK("Online Payment Not Found")
        //     // }
        // }

}