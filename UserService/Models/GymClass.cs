namespace UserService.Models
{
    public class GymClass
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Trainer { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }

        public string Time { get; set; }

        public int Capacity { get; set; }
    }
}