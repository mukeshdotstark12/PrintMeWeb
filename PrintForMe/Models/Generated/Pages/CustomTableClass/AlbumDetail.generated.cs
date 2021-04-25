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
using CMS.CustomTables.Types.Printforme;
using CMS.CustomTables;

[assembly: RegisterCustomTable(AlbumDetailItem.CLASS_NAME, typeof(AlbumDetailItem))]

namespace CMS.CustomTables.Types.Printforme
{
	/// <summary>
	/// Represents a content item of type AlbumDetailItem.
	/// </summary>
	public partial class AlbumDetailItem : CustomTableItem
	{
		#region "Constants and variables"

		/// <summary>
		/// The name of the data class.
		/// </summary>
		public const string CLASS_NAME = "Printforme.AlbumDetail";


		/// <summary>
		/// The instance of the class that provides extended API for working with AlbumDetailItem fields.
		/// </summary>
		private readonly AlbumDetailItemFields mFields;

		#endregion


		#region "Properties"

		/// <summary>
		/// UserID.
		/// </summary>
		[DatabaseField]
		public int UserID
		{
			get
			{
				return ValidationHelper.GetInteger(GetValue("UserID"), 0);
			}
			set
			{
				SetValue("UserID", value);
			}
		}


		/// <summary>
		/// Size.
		/// </summary>
		[DatabaseField]
		public string Size
		{
			get
			{
				return ValidationHelper.GetString(GetValue("Size"), @"");
			}
			set
			{
				SetValue("Size", value);
			}
		}


		/// <summary>
		/// NoofPages.
		/// </summary>
		[DatabaseField]
		public int NoofPages
		{
			get
			{
				return ValidationHelper.GetInteger(GetValue("NoofPages"), 0);
			}
			set
			{
				SetValue("NoofPages", value);
			}
		}


		/// <summary>
		/// AlbumDate.
		/// </summary>
		[DatabaseField]
		public DateTime AlbumDate
		{
			get
			{
				return ValidationHelper.GetDateTime(GetValue("AlbumDate"), DateTimeHelper.ZERO_TIME);
			}
			set
			{
				SetValue("AlbumDate", value);
			}
		}


		/// <summary>
		/// Gets an object that provides extended API for working with AlbumDetailItem fields.
		/// </summary>
		[RegisterProperty]
		public AlbumDetailItemFields Fields
		{
			get
			{
				return mFields;
			}
		}


		/// <summary>
		/// Provides extended API for working with AlbumDetailItem fields.
		/// </summary>
		[RegisterAllProperties]
		public partial class AlbumDetailItemFields : AbstractHierarchicalObject<AlbumDetailItemFields>
		{
			/// <summary>
			/// The content item of type AlbumDetailItem that is a target of the extended API.
			/// </summary>
			private readonly AlbumDetailItem mInstance;


			/// <summary>
			/// Initializes a new instance of the <see cref="AlbumDetailItemFields" /> class with the specified content item of type AlbumDetailItem.
			/// </summary>
			/// <param name="instance">The content item of type AlbumDetailItem that is a target of the extended API.</param>
			public AlbumDetailItemFields(AlbumDetailItem instance)
			{
				mInstance = instance;
			}


			/// <summary>
			/// UserID.
			/// </summary>
			public int UserID
			{
				get
				{
					return mInstance.UserID;
				}
				set
				{
					mInstance.UserID = value;
				}
			}


			/// <summary>
			/// Size.
			/// </summary>
			public string Size
			{
				get
				{
					return mInstance.Size;
				}
				set
				{
					mInstance.Size = value;
				}
			}


			/// <summary>
			/// NoofPages.
			/// </summary>
			public int NoofPages
			{
				get
				{
					return mInstance.NoofPages;
				}
				set
				{
					mInstance.NoofPages = value;
				}
			}


			/// <summary>
			/// AlbumDate.
			/// </summary>
			public DateTime AlbumDate
			{
				get
				{
					return mInstance.AlbumDate;
				}
				set
				{
					mInstance.AlbumDate = value;
				}
			}
		}

		#endregion


		#region "Constructors"

		/// <summary>
		/// Initializes a new instance of the <see cref="AlbumDetailItem" /> class.
		/// </summary>
		public AlbumDetailItem() : base(CLASS_NAME)
		{
			mFields = new AlbumDetailItemFields(this);
		}

		#endregion
	}
}