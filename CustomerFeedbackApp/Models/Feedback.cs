using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerFeedbackApp.Models
{
    public class Feedback
    {
        public int FeedbackId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string Product { get; set; }
        public string Comment { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.Now;
    }
}
