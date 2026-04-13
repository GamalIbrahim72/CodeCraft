using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Application.Interfaces.Repositories;
public interface ICourseRepository: IGenericRepository<Course>
{
    Task<IEnumerable<Course>> GetCoursesByTrackId(int trackId);
    Task<int> GetNextOrder(int trackId);
}
