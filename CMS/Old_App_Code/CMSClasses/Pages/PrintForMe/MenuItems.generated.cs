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
using CMS.DocumentEngine.Types.PrintForMe;
using CMS.DocumentEngine;

[assembly: RegisterDocumentType(MenuItems.CLASS_NAME, typeof(MenuItems))]

namespace CMS.DocumentEngine.Types.PrintForMe
{
	/// <summary>
	/// Represents a content item of type MenuItems.
	/// </summary>
	public partial class MenuItems : TreeNode
	{
		#region "Constants and variables"

		/// <summary>
		/// The name of the data class.
		/// </summary>
		public const string CLASS_NAME = "PrintForMe.MenuItems";


		/// <summary>
		/// The instance of the class that provides extended API for working with MenuItems fields.
		/// </summary>
		private readonly MenuItemsFields mFields;

		#endregion


		#region "Properties"

		/// <summary>
		/// MenuItemsID.
		/// </summary>
		[DatabaseIDField]
		public int MenuItemsID
		{
			get
			{
				return ValidationHelper.GetInteger(GetValue("MenuItemsID"), 0);
			}
			set
			{
				SetValue("MenuItemsID", value);
			}
		}


		/// <summary>
		/// MenuItems.
		/// </summary>
		[DatabaseField]
		public string MenuItems1
		{
			get
			{
				return ValidationHelper.GetString(GetValue("MenuItems"), @"");
			}
			set
			{
				SetValue("MenuItems", value);
			}
		}


		/// <summary>
		/// Gets an object that provides extended API for working with MenuItems fields.
		/// </summary>
		[RegisterProperty]
		public MenuItemsFields Fields
		{
			get
			{
				return mFields;
			}
		}


		/// <summary>
		/// Provides extended API for working with MenuItems fields.
		/// </summary>
		[RegisterAllProperties]
		public partial class MenuItemsFields : AbstractHierarchicalObject<MenuItemsFields>
		{
			/// <summary>
			/// The content item of type MenuItems that is a target of the extended API.
			/// </summary>
			private readonly MenuItems mInstance;


			/// <summary>
			/// Initializes a new instance of the <see cref="MenuItemsFields" /> class with the specified content item of type MenuItems.
			/// </summary>
			/// <param name="instance">The content item of type MenuItems that is a target of the extended API.</param>
			public MenuItemsFields(MenuItems instance)
			{
				mInstance = instance;
			}


			/// <summary>
			/// MenuItemsID.
			/// </summary>
			public int ID
			{
				get
				{
					return mInstance.MenuItemsID;
				}
				set
				{
					mInstance.MenuItemsID = value;
				}
			}


			/// <summary>
			/// MenuItems.
			/// </summary>
			public string MenuItems
			{
				get
				{
					return mInstance.MenuItems1;
				}
				set
				{
					mInstance.MenuItems1 = value;
				}
			}
		}

		#endregion


		#region "Constructors"

		/// <summary>
		/// Initializes a new instance of the <see cref="MenuItems" /> class.
		/// </summary>
		public MenuItems() : base(CLASS_NAME)
		{
			mFields = new MenuItemsFields(this);
		}

		#endregion
	}
}