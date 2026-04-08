using Uni_Connect.Models;
using System.Collections.Generic;

namespace Uni_Connect.ViewModels
{
    public class SinglePostViewModel
    {
        public User CurrentUser { get; set; }
        public Post Post { get; set; }
        public List<AnswerViewModel> Answers { get; set; }
        
        // Form field for submitting a new answer
        public string NewAnswerContent { get; set; }
    }

    public class AnswerViewModel
    {
        public int AnswerID { get; set; }
        public string Content { get; set; }
        public bool IsAccepted { get; set; }
        public int Upvotes { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public User User { get; set; }
    }
}
