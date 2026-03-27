using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroTemp.Domain.Entities
{
    [Table("job_attachment")]
    public class JobAttachment
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("job_detail_id")]
        public Guid JobDetailId { get; set; }
        public JobDetail JobDetail { get; set; }

        // id của file trên cloudinary
        [Required]
        [Column("cloudinary_public_id")]
        public string CloudinaryPublicId { get; set; }

        // link ảnh
        [Required]
        [Column("file_url")]
        public string FileUrl { get; set; }

        // loại file (jpg, png...)
        [Column("format")]
        public string? Format { get; set; }

        // dung lượng file
        [Column("file_size")]
        public long? FileSize { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
