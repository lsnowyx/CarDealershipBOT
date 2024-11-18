using System.ComponentModel.DataAnnotations;
namespace ELD888TGBOT._1._Models.Request
{
    public class CreateLoadRequest
    {
        [Required]
        [MaxLength(64)]
        public string PickUpLocation { get; set; }
        [Required]
        [MaxLength(64)]
        public string Destination { get; set; }
        [MaxLength(200)]
        public string Notes { get; set; }
        [Required]
        public string DriverId { get; set; }
        [Required]
        public string StartDate { get; set; }
    }
}