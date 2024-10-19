namespace WebSocketsSample.Models
{
    public class BidLog
    {

        public int BidLogId { get; set; }
        public int? BidderId { get; set; }
        public int AuctionLot { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
    }

}