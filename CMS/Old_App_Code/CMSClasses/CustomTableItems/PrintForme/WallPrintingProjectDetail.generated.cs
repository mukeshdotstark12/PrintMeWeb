//--------------------------------------------------------------------------------------------------
// <auto-generated>
//
//     This code was generated by code generator tool.
//
//     To customize the code use your own partial class. For more info about how to use and customize
//     the generated code see the documentation at http://docs.kentico.com.
//
// </auto-generated>
//--------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

using CMS;
using CMS.Base;
using CMS.Helpers;
using CMS.DataEngine;
using CMS.CustomTables.Types.PrintForme;
using CMS.CustomTables;

[assembly: RegisterCustomTable(WallPrintingProjectDetailItem.CLASS_NAME, typeof(WallPrintingProjectDetailItem))]

namespace CMS.CustomTables.Types.PrintForme
{
	/// <summary>
	/// Represents a content item of type WallPrintingProjectDetailItem.
	/// </summary>
	public partial class WallPrintingProjectDetailItem : CustomTableItem
	{
		#region "Constants and variables"

		/// <summary>
		/// The name of the data class.
		/// </summary>
		public const string CLASS_NAME = "PrintForme.WallPrintingProjectDetail";


		/// <summary>
		/// The instance of the class that provides extended API for working with WallPrintingProjectDetailItem fields.
		/// </summary>
		private readonly WallPrintingProjectDetailItemFields mFields;

		#endregion


		#region "Properties"

		/// <summary>
		/// ProjectID.
		/// </summary>
		[DatabaseField]
		public int ProjectID
		{
			get
			{
				return ValidationHelper.GetInteger(GetValue("ProjectID"), 0);
			}
			set
			{
				SetValue("ProjectID", value);
			}
		}


		/// <summary>
		/// ImageUrl.
		/// </summary>
		[DatabaseField]
		public string ImageUrl
		{
			get
			{
				return ValidationHelper.GetString(GetValue("ImageUrl"), @"");
			}
			set
			{
				SetValue("ImageUrl", value);
			}
		}


		/// <summary>
		/// NoOfCopy.
		/// </summary>
		[DatabaseField]
		public int NoOfCopy
		{
			get
			{
				return ValidationHelper.GetInteger(GetValue("NoOfCopy"), 0);
			}
			set
			{
				SetValue("NoOfCopy", value);
			}
		}


		/// <summary>
		/// ImageSizeID.
		/// </summary>
		[DatabaseField]
		public int ImageSizeID
		{
			get
			{
				return ValidationHelper.GetInteger(GetValue("ImageSizeID"), 0);
			}
			set
			{
				SetValue("ImageSizeID", value);
			}
		}


		/// <summary>
		/// Gets an object that provides extended API for working with WallPrintingProjectDetailItem fields.
		/// </summary>
		[RegisterProperty]
		public WallPrintingProjectDetailItemFields Fields
		{
			get
			{
				return mFields;
			}
		}


		/// <summary>
		/// Provides extended API for working with WallPrintingProjectDetailItem fields.
		/// </summary>
		[RegisterAllProperties]
		public partial class WallPrintingProjectDetailItemFields : AbstractHierarchicalObject<WallPrintingProjectDetailItemFields>
		{
			/// <summary>
			/// The content item of type WallPrintingProjectDetailItem that is a target of the extended API.
			/// </summary>
			private readonly WallPrintingProjectDetailItem mInstance;


			/// <summary>
			/// Initializes a new instance of the <see cref="WallPrintingProjectDetailItemFields" /> class with the specified content item of type WallPrintingProjectDetailItem.
			/// </summary>
			/// <param name="instance">The content item of type WallPrintingProjectDetailItem that is a target of the extended API.</param>
			public WallPrintingProjectDetailItemFields(WallPrintingProjectDetailItem instance)
			{
				mInstance = instance;
			}


			/// <summary>
			/// ProjectID.
			/// </summary>
			public int ProjectID
			{
				get
				{
					return mInstance.ProjectID;
				}
				set
				{
					mInstance.ProjectID = value;
				}
			}


			/// <summary>
			/// ImageUrl.
			/// </summary>
			public string ImageUrl
			{
				get
				{
					return mInstance.ImageUrl;
				}
				set
				{
					mInstance.ImageUrl = value;
				}
			}


			/// <summary>
			/// NoOfCopy.
			/// </summary>
			public int NoOfCopy
			{
				get
				{
					return mInstance.NoOfCopy;
				}
				set
				{
					mInstance.NoOfCopy = value;
				}
			}


			/// <summary>
			/// ImageSizeID.
			/// </summary>
			public int ImageSizeID
			{
				get
				{
					return mInstance.ImageSizeID;
				}
				set
				{
					mInstance.ImageSizeID = value;
				}
			}
		}

		#endregion


		#region "Constructors"

		/// <summary>
		/// Initializes a new instance of the <see cref="WallPrintingProjectDetailItem" /> class.
		/// </summary>
		public WallPrintingProjectDetailItem() : base(CLASS_NAME)
		{
			mFields = new WallPrintingProjectDetailItemFields(this);
		}

		#endregion
	}
}