using System.Text.Json.Serialization;

namespace BoldDesk.Models;

public class TicketData
{
    [JsonPropertyName("ticketObjects")]
    public List<Ticket> TicketObjects { get; set; } = new();

    [JsonPropertyName("ticketListDetails")]
    public List<TicketListDetail> TicketListDetails { get; set; } = new();
}