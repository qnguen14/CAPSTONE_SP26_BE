using AgroTemp.Domain.DTO.Job.JobCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroTemp.Service.Interfaces
{
    public interface IJobCategoryService
    {
        Task<List<JobCategoryDTO>> GetAllJobCategories();
        Task<JobCategoryDTO> GetJobCategoryById(string id);
        Task<JobCategoryDTO> CreateJobCategory(JobCategoryDTO jobCategoryDto);
        Task<JobCategoryDTO> UpdateJobCategory(string id, JobCategoryDTO jobCategoryDto);
        Task<bool> DeleteJobCategory(string id);
    }
}
