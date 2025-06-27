using Microsoft.AspNetCore.Mvc;
using DiningVsCodeNew;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;


namespace DiningVsCodeNew.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{

    UserRepository repuser;
    private EmailConfiguration _emailConfig;
    int idvalue = 0;
    public UserController(UserRepository repUser, EmailConfiguration emailConfig)
    {
        this.repuser = repUser;
        this._emailConfig = emailConfig;
    }
    // GET: api/Cities
    [HttpGet]
    public async Task<ActionResult> GetUsers()
    {
        return new OkObjectResult(repuser.GetUsers());
    }
    // GET: api/Cities/5
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        return new OkObjectResult(repuser.GetUser(id));

    }
    [HttpGet("getfiltuser")]
    public IActionResult GetUserByUsernamePattern([FromQuery] string usernamePattern)

    {


        var filtUser = repuser.GetUserByUsernamePattern(usernamePattern);
        return Ok(filtUser);

    }

    [HttpGet("getuser/{userName}")]
    public async Task<ActionResult<User>> GetUser(string username)
    {
        return new OkObjectResult(repuser.GetUserName(username));
    }


    [HttpGet("getuserbycustid/{custId}")]
    public async Task<ActionResult<User>> GetUserByCustId(string custId)
    {
        return new OkObjectResult(repuser.GetUserByCustId(custId));
    }




    // PUT: api/users/5
    // To protect from overposting attacks, see https://go.microsoft.com/
    // fwlink/?linkid=2123754
    [HttpPost("{id}")]
    public async Task<IActionResult> PutUser(int id, User user)
    {
        if (id != user.id)
        {
            return BadRequest();
        }
        // _context.Entry(state).State = EntityState.Modified;

        repuser.updateUser(user);
        return new OkObjectResult(user);

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
    [HttpPost]
    public async Task<ActionResult<User>> PostUser(User user)
    {
        if (user != null)
        {
            var usercustid = repuser.GetUserByCustId(user.custId);
            var usermail = repuser.GetUserByUsername(user.userName);

            if (usercustid != null)
            {
                // User with the same custId already exists
                return Conflict("User with the same custId already exists.");
            }

            if (usermail != null)
            {
                // User with the same username already exists
                return Conflict("User with the same username already exists.");
            }

            // If both usercustid and usermail are null, it means user doesn't exist, proceed with user creation
            repuser.insertUser(user);

            // Send welcome email
            EmailSender _emailSender = new EmailSender(this._emailConfig);
            Email em = new Email();
            string logourl = ""; //"https://evercaregroup.com/wp-content/uploads/2020/12/EVERCARE_LOGO_03_LEKKI_PRI_FC_RGB.png";
            string applink = "https://cafeteria.evercare.ng";
            string salutation = "Dear " + user.firstName + ",";
            string emailcontent = "Your account has been successfully created on Evercare's Food & Beverage Application. You can now enjoy a seamless dining experience with our app.";
            string narration1 = "Thank you for choosing Evercare's Cafeteria!";
            string econtent = em.HtmlMail("Welcome to Evercare's Cafeteria", applink, salutation, emailcontent, narration1, logourl);
            var message = new Message(new string[] { user.userName }, "Cafeteria Application", econtent);
            await _emailSender.SendEmailAsync(message);

            return Ok(user);
        }

        return BadRequest("Invalid user data."); // Return bad request if user object is null


    }
    // DELETE: api/Cities/5
    [HttpPost("deleteuser")]
    public async Task<IActionResult> Delete([FromBody] User us)
    {

        // if (idvalue != us.id)
        //  {
        //  return NotFound();
        //  }
        idvalue = repuser.deleteUser(us);
        return Ok(us);
    }
    //private bool StateExists(int id)
    // {
    // return _context.States.Any(e => e.Id == id);
    // }

}