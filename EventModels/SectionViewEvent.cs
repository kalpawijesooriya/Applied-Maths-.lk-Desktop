using appliedmaths.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace appliedmaths.EventModels
{
    class SectionViewEvent
    {
        public SectionViewEvent(Section item)
        {
            Item = item;
        }

        public Section Item { get; private set; }
    }
}
