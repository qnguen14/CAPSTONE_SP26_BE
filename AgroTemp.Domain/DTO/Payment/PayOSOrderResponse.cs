namespace AgroTemp.Domain.DTO.Payment;

public class PayOSOrderResponse
{
    public Guid Id { get; set; }
    public long OrderCode { get; set; }
    public long TotalAmount { get; set; }
    public string? Description { get; set; }
    public string? PaymentLinkId { get; set; }
    public string? QrCode { get; set; }
    public string? CheckoutUrl { get; set; }
    public string? Status { get; set; }
    public long Amount { get; set; }
    public long AmountPaid { get; set; }
    public long AmountRemaining { get; set; }
    public string? Bin { get; set; }
    public string? AccountNumber { get; set; }
    public string? AccountName { get; set; }
    public string? Currency { get; set; }
    public string? ReturnUrl { get; set; }
    public string? CancelUrl { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? CanceledAt { get; set; }
    public string? CancellationReason { get; set; }
    public DateTimeOffset? LastTransactionUpdate { get; set; }

    public string? BuyerName { get; set; }
    public string? BuyerCompanyName { get; set; }
    public string? BuyerEmail { get; set; }
    public string? BuyerPhone { get; set; }
    public string? BuyerAddress { get; set; }
    public DateTimeOffset? ExpiredAt { get; set; }
    public bool? BuyerNotGetInvoice { get; set; }
    public int? TaxPercentage { get; set; }

    public List<PayOSOrderItemResponse> Items { get; set; } = new();
    public List<PayOSOrderTransactionResponse> Transactions { get; set; } = new();
}

public class PayOSOrderItemResponse
{
    public string? Name { get; set; }
    public int Quantity { get; set; }
    public int Price { get; set; }
    public string? Unit { get; set; }
    public int? TaxPercentage { get; set; }
}

public class PayOSOrderTransactionResponse
{
    public string? Reference { get; set; }
    public long Amount { get; set; }
    public string? AccountNumber { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset TransactionDateTime { get; set; }
    public string? VirtualAccountName { get; set; }
    public string? VirtualAccountNumber { get; set; }
    public string? CounterAccountBankId { get; set; }
    public string? CounterAccountBankName { get; set; }
    public string? CounterAccountName { get; set; }
    public string? CounterAccountNumber { get; set; }
}
