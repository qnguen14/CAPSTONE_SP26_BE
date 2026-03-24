using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroTemp.Domain.DTO.Rating
{
    public class CreateRatingRequest
    {
        public Guid RaterId { get; set; }
        public Guid RateeId { get; set; }
        public Guid JobPostId { get; set; }
        public int RatingScore { get; set; }
        public string? ReviewText { get; set; }
        public int TypeId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
