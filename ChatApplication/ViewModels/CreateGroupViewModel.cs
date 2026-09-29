using System.ComponentModel.DataAnnotations;

namespace Chat.ViewModels
{
    public class CreateGroupViewModel
    {
        public string GroupName { get; set; }
        public List<int> MemberUserIds { get; set; } = new List<int>();
    }
}
