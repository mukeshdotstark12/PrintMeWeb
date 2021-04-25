using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomWebApi.Model.Album
{
    public class Album
    {
        public Album()
        {
            albumMaster = new List<Album>();
        }
        public List<Album> albumMaster { get; set; }
        public Guid AlbumRowGUID { get; set; }
        public DateTime AlbumCreatedDate { get; set; }
        public int LoggedinUserID { get; set; }
        public Boolean AlbumStatus { get; set; }
        public string AlbumPageType { get; set; }
        public string AlbumSize { get; set; }
        public string AlbumPageCountCode { get; set; }
        public int AlbumID { get; set; }
        public string ImagesName { get; set; }
        public int Price { get; set; }
        public string ImagesLocation { get; set; }
        public string SKUID { get; set; }
        public DateTime ModifyDate { get; set; }
    }
}
