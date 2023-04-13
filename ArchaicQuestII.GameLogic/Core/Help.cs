using System;

namespace ArchaicQuestII.GameLogic.Core
{
    public class Help
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Keywords { get; set; }
        public string BriefDescription { get; set; }
        public string Description { get; set; }
        public string RelatedHelpFiles { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public Boolean Deleted { get; set; }
    }
}
