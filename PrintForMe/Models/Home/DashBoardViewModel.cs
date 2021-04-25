using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrintForMe.Models.Home
{
    public class DashBoardViewModel
    {
        //Number of views visited to website.
        public string NumOfViews { get; set; }
        public string NumOfViewsPer { get; set; }

        //Total Number of Orders 
        public string NumOfOrder { get; set; }
        public string NumOfOrderPer { get; set; }       
        
        public string Totalearning { get; set; }
        public string TotalearningPer { get; set; }
        
        public int NewClients { get; set; }        
        public int InProgress { get; set; }
        public int Waiting { get; set; }
    }
}