using System;
using Currency;

namespace LaboratoryWork3
{
    public sealed class CourseEventArgs : EventArgs
    {
        public Course OldCourse { get; }
        public Course NewCourse { get; }
            
        public CourseEventArgs(Course oldCourse, Course newCourse)
        {
            OldCourse = oldCourse;
            NewCourse = newCourse;
        }
    }
}
