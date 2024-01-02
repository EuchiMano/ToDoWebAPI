namespace ToDoWebAPI.Dtos
{
    public class UpdateTodoBadgeDto
    {
        public Guid TodoId { get; set; }
        public string BadgeUrl { get; set; }
    }
}