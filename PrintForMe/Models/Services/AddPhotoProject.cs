﻿using PrintForMe.Models.Store;
using System;
using System.Collections.Generic;

namespace PrintForMe.Models.Services
{
    public class AddPhotoProject
    {
        public AddPhotoProject()
        {
            AddPhotoProjectDetails = new List<AddPhotoProjectDetail>();
        }
        public IEnumerable<ServiceSettingModel> Size { get; set; }
        public IEnumerable<object> PaperMaterial { get; set; }
        public int SelectedSize { get; set; }
        public int SelectedPaperMaterial { get; set; }
        public int ProjectID { get; set; }
        public int UserID { get; set; }
        public int PaperMaterialID { get; set; }
        public int SizeID { get; set; }
        public int SKUID { get; set; }
        public bool IsComplete { get; set; }
        public DateTime ProjectDate { get; set; }
        public int ServiceID { get; set; }

        public double TotalPrice { get; set; }
        public List<AddPhotoProjectDetail> AddPhotoProjectDetails { get; set; }
    }
}