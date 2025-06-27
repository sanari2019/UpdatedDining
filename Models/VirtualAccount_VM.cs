
namespace DiningVsCodeNew
{
    public class VirtualAccount_VM
    {
    // public string event { get; set; }
    public string transaction_reference { get; set; }
    public string virtual_account_number { get; set; }
    public string principal_amount { get; set; }
    public string settled_amount { get; set; }
    public string fee_charged { get; set; }
    public string transaction_date { get; set; }
    public string customer_identifier { get; set; }
    public string transaction_indicator { get; set; }
    public string remarks { get; set; }
    public string currency { get; set; }
    public string channel { get; set; }
    public MetaBody_VM meta { get; set; }
    public string encrypted_body { get; set; }
    }
}