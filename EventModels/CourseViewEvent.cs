using System;
using System.Collections.Generic;
using System.Text;

namespace appliedmaths.EventModels
{
    class CourseViewEvent
    {
        public CourseViewEvent(string id)
        {
            Id = id;
        }

        public string Id { get; private set; }
    }
}
