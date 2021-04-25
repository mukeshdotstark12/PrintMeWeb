using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomWebApi.Model.Album
{
	public class AlbumMaster
	{
		public DateTime ProjectDate { get; set; }		
		public int ProjectID { get; set; }
		public int UserId { get; set; }
		public int PaperCount { get; set; }
		public int PaperSize { get; set; }
		public int PaperMaterial { get; set; }
		public string SKUID { get; set; }
		public bool IsComplete { get; set; }
		public int TotalPrice { get; set; }
		public int ServiceId { get; set; }
		public List<AlbumPages> GetAlbumPage {get;set;}
	}
    
}
