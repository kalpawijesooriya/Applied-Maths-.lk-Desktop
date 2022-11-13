using appliedmaths.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace appliedmaths.EventModels
{
   public class NotStartedEvent
    {
        public NotStartedEvent(SectionVideo item)
        {
            Item = item;
        }

        public SectionVideo Item { get; private set; }
    }
}
