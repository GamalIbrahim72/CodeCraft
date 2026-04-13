using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Application.Interfaces.Repositories;
public interface ILessonRepository:IGenericRepository<Lesson>
{
    Task<int> GetLessonsCountByCourseId(int courseId);
    Task<Lesson?> GetFirstLessonInCourse(int courseId);

}
