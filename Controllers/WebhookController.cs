// using Microsoft.AspNetCore.Mvc;
// using DiningVsCodeNew;
// using System;
// using System.Text;
// using System.Text.Unicode;
// using System.Security.Cryptography;
// using Newtonsoft.Json.Linq;
// using DiningVsCodeNew.Models;
// using System.Collections.Generic;

// namespace DiningVsCodeNew.Controllers
// {

// [Route("[controller]")]
// [ApiController]
// public class WebhookController : ControllerBase
// {

//     PaymentMainRepository _reppymtMain;
//     UserRepository _userrep;
//     PaymentDetailsRepository _repPayDetails;
//     OnlinePaymentRepository _reponlinePayment;
//     OrderedMealRepository _ordMeal;


//     int idvalue = 0;
//     public WebhookController(PaymentMainRepository reppymtMain, UserRepository userrep, PaymentDetailsRepository repPayDetails, OnlinePaymentRepository reponlinePayment, OrderedMealRepository omeal)
//     {
//         this._reppymtMain = reppymtMain;
//         this._userrep = userrep;
//         this._repPayDetails = repPayDetails;
//         this._reponlinePayment = reponlinePayment;
//         this._ordMeal = omeal;

//     }
//     // GET: api/Cities
//     [HttpPost]
//     public async Task<IActionResult> PostTransaction([FromBody] Transaction payload)
        
//         string key = "sk_live_60bcaef604255e41e2ac842412dc399cbb39880a";
//         Transaction trans = payload;
//         string retvalue = "500";
//         if (trans.@event == "charge.success")
//         {

//             User us = _userrep.GetUserName(trans.data.customer.email);
//             if (us != null && trans.data.metadata.voucherId != 10)
//             {
//                 List<PaymentDetails> pyds = new List<PaymentDetails>();
//                 float totalAmt = 0;
//                 pyds = _repPayDetails.GetPymtDetails(us.id);
//                 if (pyds != null)
//                 {
//                     foreach (PaymentDetails pyd in pyds)
//                     {
//                         PaymentMain pytMain = new PaymentMain();
//                         pytMain.Amount = pyd.amount;
//                         pytMain.CustCode = pyd.custCode;
//                         pytMain.DateEntered = pyd.dateEntered;
//                         pytMain.EnteredBy = pyd.enteredBy;
//                         pytMain.Id = pyd.id;
//                         pytMain.Paymentmodeid = pyd.paymentmodeid;
//                         pytMain.Unit = pyd.unit;
//                         pytMain.VoucherId = pyd.voucherid;
//                         pytMain.ServedBy = "";
//                         pytMain.opaymentid = 1111;
//                         pytMain.Paid = true;
//                         pytMain.timepaid = pyd.dateEntered;
//                         pytMain.PaymentType = 0;
//                         pytMain.CustTypeiD = pyd.custtypeid;
//                         pytMain.VoucherDescription = "";
//                         pytMain.DateServed = DateTime.Now;
//                         totalAmt = pyd.amount;
//                         _reppymtMain.updatePaymentMain(pytMain);
//                     }
//                     OnlinePayment opaymt = new OnlinePayment();
//                     opaymt.AmountPaid = totalAmt;
//                     opaymt.Paidby = int.Parse(pyds[0].enteredBy);
//                     opaymt.TransRefNo = trans.data.reference;
//                     opaymt.TransDate = DateTime.Now;
//                     // int opaymtid = _reponlinePayment.insertOnlinePayment(opaymt);
//                     foreach (PaymentDetails pyd in pyds)
//                     {
//                         // _reppymtMain.updatePaymentMainbyid(opaymtid, pyd.id);
//                     }
//                 }
//                 retvalue = "200";
//             }
//             else if (us != null && trans.data.metadata.voucherId == 10)
//             {
//                 List<PaymentDetails> pyds = new List<PaymentDetails>();
//                 List<OrderedMeal> omeals = new List<OrderedMeal>();
//                 float totalAmt = 0;
//                 pyds = _repPayDetails.GetPymtDetails(us.id);
//                 omeals = _ordMeal.GetOrderedMealsbyCust(us);
//                 if (omeals.Count > 0)
//                 {
//                     float omealsamount = 0;
//                     foreach (OrderedMeal omeal in omeals)
//                     {
//                         omealsamount = omeal.Amount;
//                     }
//                     OnlinePayment opaymt = new OnlinePayment();
//                     opaymt.AmountPaid = totalAmt;
//                     opaymt.Paidby = us.id;
//                     opaymt.TransRefNo = trans.data.reference;
//                     opaymt.TransDate = DateTime.Now;
//                     // int opaymtid = _reponlinePayment.insertOnlinePayment(opaymt);
//                     int opymtid = 0;
//                     PaymentMain pytMain = new PaymentMain();
//                     pytMain.Amount = omealsamount;
//                     pytMain.CustCode = us.custId;
//                     pytMain.DateEntered = DateTime.Now;
//                     pytMain.EnteredBy = us.id.ToString();
//                     pytMain.Paymentmodeid = 3;
//                     pytMain.Unit = 1;
//                     pytMain.VoucherId = 10;
//                     pytMain.ServedBy = "";
//                     // pytMain.opaymentid = opaymtid;
//                     pytMain.Paid = true;
//                     pytMain.timepaid = DateTime.Now;
//                     pytMain.PaymentType = 1;
//                     pytMain.CustTypeiD = us.custTypeId;
//                     // pytMain.opaymentid = opaymtid;
//                     pytMain.DateServed = DateTime.Now;
//                     // opymtid = _reppymtMain.insertPaymentMain(pytMain);
//                     foreach (OrderedMeal omeal in omeals)
//                     {
//                         omeal.Submitted = true;
//                         omeal.paymentMainId = opymtid;
//                     }
//                 }
//                 retvalue = "200";

//             }
//         }
//         return Ok(retvalue);
//     }


// // public static void Main()
// // 	{
// // 				var chargeResponse = new VirtualAccount_VM()
// // 				{
// // 					  transaction_reference = "REFE52ARZHTS/1668421222619_1",
// // 					  virtual_account_number = "2129125316",
// // 					  principal_amount = "222.00",
// // 					  settled_amount = "221.78",
// // 					  fee_charged = "0.22",
// // 					  transaction_date = "2022-11-14T10:20:22.619Z",
// // 					  customer_identifier = "SBN1EBZEQ8",
// // 					  transaction_indicator = "C",
// // 					  remarks =  "Transfer FROM sandbox sandbox | [SBN1EBZEQ8] TO sandbox sandbox",
// // 					  currency = "NGN",
// // 					  channel =  "virtual-account",
// // 					  meta =  new MetaBody_VM()
// // 					  {
// // 						freeze_transaction_ref =  null,
// // 						reason_for_frozen_transaction =  null
// // 					  },
// // 					  encrypted_body = "ViASuHLhO+SP3KtmcdAOis+3Obg54d5SgCFPFMcguYfkkYs/i44jeT5Dbx52TcOvHRp9HlnCoFwbATkEihzv2C8UyPoC38sRb90S5Z9Fq7vRwjDQz/hYi/nKbWA0btPr3A+UXhX1Nu5ek+TL0ENUC8W1ZX/FrowX3HQaYiwe3tU/Kfr2XvAGwT7IAx5CQBhpzL34faHP4jbwSVmSgVYmW5rd2ClWQ7WWJjDMakrqYJva8qd0vhkqSpyz2KywOV9t9zSHRx3VpbvlDsBdkNGr+4Axh/7Gspu3xo9mMOIdv73OzjN4VA/qQP+fQMCjU1pbS8oh81HjwkHjzC5SBhzR8IU8bsmvFUyzJMfDoJuUB+fs09SLW7pdfODwK5vB8LtdKPnAuTPlv5dHVAPeMG/ubtl/HOqCZs4axjuO557srw0GpKk86bwaVKt4IQ17nY/QCJFC273HWU1CawP7d3nQasRZf/TU7ra+fOjQBHQ7Gtz2Pnfp3gLljBKenMT4Cabks1X2/6ZQpd/yGFkloYdS7ZW3kEvrorjcyma4WNDmJfhcdR9XGsom6Y/M/n/gMMa0z2KPbHDRoEBeRYbQHcnu5LnGWzBA4Y4RMSTDesD876PDB1bOnMzNPrWYam6ZVRHz"
// // 				};
				
// // 				String SerializedPayload = JsonConvert.SerializeObject(chargeResponse);
// // 				Console.WriteLine(SerializedPayload);
// //                 string result = "";
// //                 var secretKeyBytes = Encoding.UTF8.GetBytes("sandbox_sk_9ac9418e847972dd45f5fe845b5716ef305589808eda");
// //                 var inputBytes = Encoding.UTF8.GetBytes(SerializedPayload);
// //                 var hmac = new HMACSHA512(secretKeyBytes);
// //                 byte[] hashValue = hmac.ComputeHash(inputBytes);
// //                 result = BitConverter.ToString(hashValue).Replace("-", string.Empty);
// // 				Console.WriteLine(result);
		
// // 				Console.WriteLine(result.ToLower() == "18b9eb6ca68f92ca9f058da7bce6545efb12660cf75f960e552cf6098bb5ee8e71f20331dcfe0dfaea07439cc6629f901850291a39f374a1bd076c4eff1026c8");
// // 	}
// // }

// }
// }