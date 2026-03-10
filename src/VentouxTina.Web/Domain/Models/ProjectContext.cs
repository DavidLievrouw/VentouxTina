namespace VentouxTina.Web.Domain.Models;

public class ProjectContext
{
    public int Id { get; set; }
    public string Locale { get; set; } = "nl-BE";
    public string Headline { get; set; } = string.Empty;
    public string BodyText { get; set; } = string.Empty;
    public string FundraisingGoalText { get; set; } = string.Empty;
}
