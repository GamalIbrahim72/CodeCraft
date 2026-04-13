using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Domain.Entities;
public class Quiz
{
    public int Id { get; set; }

    public string Question { get; set; }

    public string OptionA { get; set; }

    public string OptionB { get; set; }

    public string OptionC { get; set; }

    public string OptionD { get; set; }

    public string CorrectAnswer { get; set; }

    public int LessonId { get; set; }

    public Lesson Lesson { get; set; }
}
