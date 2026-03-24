using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AgroTemp.API.Models.Farm;

public class UploadFarmImageRequest
{
    [Required]
    public IFormFile File { get; set; }
}
