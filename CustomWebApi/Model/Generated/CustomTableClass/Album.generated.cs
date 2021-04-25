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

[assembly: RegisterCustomTable(AlbumItem.CLASS_NAME, typeof(AlbumItem))]

namespace CMS.CustomTables.Types.Printforme
{
	/// <summary>
	/// Represents a content item of type AlbumItem.
	/// </summary>
	public partial class AlbumItem : CustomTableItem
	{
		#region "Constants and variables"

		/// <summary>
		/// The name of the data class.
		/// </summary>
		public const string CLASS_NAME = "Printforme.Album";


		/// <summary>
		/// The instance of the class that provides extended API for working with AlbumItem fields.
		/// </summary>
		private readonly AlbumItemFields mFields;

		#endregion


		#region "Properties"

		/// <summary>
		/// User ID.
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
		/// Page Count ID.
		/// </summary>
		[DatabaseField]
		public int PageCountID
		{
			get
			{
				return ValidationHelper.GetInteger(GetValue("PageCountID"), 0);
			}
			set
			{
				SetValue("PageCountID", value);
			}
		}


		/// <summary>
		/// Page Size ID.
		/// </summary>
		[DatabaseField]
		public int PageSizeID
		{
			get
			{
				return ValidationHelper.GetInteger(GetValue("PageSizeID"), 0);
			}
			set
			{
				SetValue("PageSizeID", value);
			}
		}


		/// <summary>
		/// State.
		/// </summary>
		[DatabaseField]
		public bool State
		{
			get
			{
				return ValidationHelper.GetBoolean(GetValue("State"), true);
			}
			set
			{
				SetValue("State", value);
			}
		}


		/// <summary>
		/// Gets an object that provides extended API for working with AlbumItem fields.
		/// </summary>
		[RegisterProperty]
		public AlbumItemFields Fields
		{
			get
			{
				return mFields;
			}
		}


		/// <summary>
		/// Provides extended API for working with AlbumItem fields.
		/// </summary>
		[RegisterAllProperties]
		public partial class AlbumItemFields : AbstractHierarchicalObject<AlbumItemFields>
		{
			/// <summary>
			/// The content item of type AlbumItem that is a target of the extended API.
			/// </summary>
			private readonly AlbumItem mInstance;


			/// <summary>
			/// Initializes a new instance of the <see cref="AlbumItemFields" /> class with the specified content item of type AlbumItem.
			/// </summary>
			/// <param name="instance">The content item of type AlbumItem that is a target of the extended API.</param>
			public AlbumItemFields(AlbumItem instance)
			{
				mInstance = instance;
			}


			/// <summary>
			/// User ID.
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
			/// Page Count ID.
			/// </summary>
			public int PageCountID
			{
				get
				{
					return mInstance.PageCountID;
				}
				set
				{
					mInstance.PageCountID = value;
				}
			}


			/// <summary>
			/// Page Size ID.
			/// </summary>
			public int PageSizeID
			{
				get
				{
					return mInstance.PageSizeID;
				}
				set
				{
					mInstance.PageSizeID = value;
				}
			}


			/// <summary>
			/// State.
			/// </summary>
			public bool State
			{
				get
				{
					return mInstance.State;
				}
				set
				{
					mInstance.State = value;
				}
			}
		}

		#endregion


		#region "Constructors"

		/// <summary>
		/// Initializes a new instance of the <see cref="AlbumItem" /> class.
		/// </summary>
		public AlbumItem() : base(CLASS_NAME)
		{
			mFields = new AlbumItemFields(this);
		}

		#endregion
	}
}