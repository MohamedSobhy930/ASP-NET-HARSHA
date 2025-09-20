using Microsoft.AspNetCore.Mvc;
using ModelBinding_Validation.Entities;
namespace ModelBinding_Validation.Controllers
{
    public class OrderController : Controller
    {

        [HttpPost("/order")]
        public IActionResult Create([Bind("InvoicePrice,OrderDate,Products")] Order order)
        {
            if(!ModelState.IsValid)
            {
                string messages = string.Join("\n", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(messages);
            }
            Random random = new Random();
            int randomOrderNumber = random.Next(1,999999);
            return Json(new {OrderNo = randomOrderNumber});
        }
    }
}
