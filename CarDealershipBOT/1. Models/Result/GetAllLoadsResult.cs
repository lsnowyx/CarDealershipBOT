namespace ELD888TGBOT
{
    public class GetAllLoadsResult
    {
        public class GetAllLoadsUserResult
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int Id { get; set; }
        }

        public class Load
        {
            public int Id { get; set; }
            public string PickUpLocation { get; set; }
            public string Destination { get; set; }
            public string Notes { get; set; }
            public string Status { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public GetAllLoadsUserResult Manager { get; set; }
            public GetAllLoadsUserResult Driver { get; set; }
            public List<TaskItem> Tasks { get; set; }
        }
        public class TaskItem
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public int OrderNumber { get; set; }
            public bool IsPhotoRequired { get; set; }
            public int Points { get; set; }
            public string Status { get; set; }
        }
        public class ApiResponse
        {
            public int Offset { get; set; }
            public int Limit { get; set; }
            public int TotalMatchingRecords { get; set; }
            public List<Load> Results { get; set; }
        }
    }
}
