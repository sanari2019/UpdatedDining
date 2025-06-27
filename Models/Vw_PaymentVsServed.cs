using System;

namespace DiningVsCodeNew
    {

    public class Vw_PaymentVsServed
    {
         public int Id { get; set; }
    public DateTime DateEntered { get; set; }
    public int EnteredBy { get; set; }
    public int CustTypeId { get; set; }
    public int PaymentModeId { get; set; }
    public string CustCode { get; set; }
    public int VoucherId { get; set; }
    public decimal Unit { get; set; }
    public decimal Amount { get; set; }
    public bool Served { get; set; }
    public DateTime? DateServed { get; set; }
    public string ServedBy { get; set; }
    public bool Paid { get; set; }
    public DateTime? TimePaid { get; set; }
    public int? OPaymentId { get; set; }
    public string PaymentType { get; set; }
    public int TotalServedCount { get; set; }
    public decimal RemainingBalance { get; set; }
    }
    }