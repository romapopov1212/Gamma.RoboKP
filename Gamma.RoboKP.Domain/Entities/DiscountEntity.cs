namespace Gamma.RoboKP.Domain.Entities;

public class DiscountEntity
{
    public DiscountEntity() {}
    
    public string Status { get; private set; } = string.Empty;
    public long Percent { get; private set; }

    public double GetMultiplier()
    {
        return 1d - (Percent / 100d);
    }
}