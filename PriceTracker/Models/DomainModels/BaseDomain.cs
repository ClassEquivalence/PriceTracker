namespace PriceTracker.Models.DomainModels
{
    /*
     
     */
    public class BaseDomain(int Id) : BaseModel
    {
        public int Id { get; set; } = Id;
    }
}
