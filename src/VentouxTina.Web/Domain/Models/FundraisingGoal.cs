namespace VentouxTina.Web.Domain.Models;

public class FundraisingGoal
{
    public int Id { get; set; }
    public string OrganizationName { get; set; } = "Klimmen tegen MS";
    public decimal GoalAmountEur { get; set; } = 500m;
    public bool IsFundraiser { get; set; } = true;
    public string Audience { get; set; } = "backers";
}
