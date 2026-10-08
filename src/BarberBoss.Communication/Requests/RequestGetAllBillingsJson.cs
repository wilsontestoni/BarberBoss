using BarberBoss.Communication.Enums;

namespace BarberBoss.Communication.Requests;

public class RequestGetAllBillingsJson
{
    public string? BarberName { get; set; }
    public string? ClientName { get; set; }
    public string? ServiceName { get; set; }
    public Status? Status { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public SortDir SortDir { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
