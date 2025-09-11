using Microsoft.AspNetCore.Mvc;

namespace Controllers_ActionResults.Controllers
{
    public class BankController : Controller
    {
        [Route("/")]
        public IActionResult Index()
        {
            return Content("Welcome To Bank");
        }
        [Route("/account-details")]
        public IActionResult Details()
        {
            return Json(new { accountNumber = 1001, accountHolderName = "Example Name", currentBalance = 5000 });
        }
        [Route("/account-statement")]
        public IActionResult Account()
        {
            return File("/BACKEND roadmap.pdf" , "application/pdf");
        }
        [Route("/get-current-balance/{accountNumber:int?}")]
        public IActionResult GetBalance()
        {
            object accountNumberObj;
            if (HttpContext.Request.RouteValues.TryGetValue("accountNumber", out accountNumberObj) && accountNumberObj is string accountNumber)
            {
                if (accountNumberObj is null)
                {
                    return NotFound("account Number should be supplied");
                }
                else if (int.TryParse(accountNumber, out int accountNumberInt))
                {
                    if(accountNumberInt == 1001)
                    {
                        return Content("5000");
                    }
                    else
                    {
                        return BadRequest("account number should be 1001");
                    }
                }
                else
                {
                    return BadRequest("Enter a valid formant for account number");
                }
            }
            else
            {
                return NotFound("account number should be supplied");
            }
        }
    }
}
