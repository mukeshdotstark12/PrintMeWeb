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

[assembly: RegisterDocumentType(AdminSection.CLASS_NAME, typeof(AdminSection))]

namespace CMS.DocumentEngine.Types.PrintForMe
{
	/// <summary>
	/// Represents a content item of type AdminSection.
	/// </summary>
	public partial class AdminSection : TreeNode
	{
		#region "Constants and variables"

		/// <summary>
		/// The name of the data class.
		/// </summary>
		public const string CLASS_NAME = "PrintForMe.AdminSection";


		/// <summary>
		/// The instance of the class that provides extended API for working with AdminSection fields.
		/// </summary>
		private readonly AdminSectionFields mFields;

		#endregion


		#region "Properties"

		/// <summary>
		/// Gets an object that provides extended API for working with AdminSection fields.
		/// </summary>
		[RegisterProperty]
		public AdminSectionFields Fields
		{
			get
			{
				return mFields;
			}
		}


		/// <summary>
		/// Provides extended API for working with AdminSection fields.
		/// </summary>
		[RegisterAllProperties]
		public partial class AdminSectionFields : AbstractHierarchicalObject<AdminSectionFields>
		{
			/// <summary>
			/// The content item of type AdminSection that is a target of the extended API.
			/// </summary>
			private readonly AdminSection mInstance;


			/// <summary>
			/// Initializes a new instance of the <see cref="AdminSectionFields" /> class with the specified content item of type AdminSection.
			/// </summary>
			/// <param name="instance">The content item of type AdminSection that is a target of the extended API.</param>
			public AdminSectionFields(AdminSection instance)
			{
				mInstance = instance;
			}
		}

		#endregion


		#region "Constructors"

		/// <summary>
		/// Initializes a new instance of the <see cref="AdminSection" /> class.
		/// </summary>
		public AdminSection() : base(CLASS_NAME)
		{
			mFields = new AdminSectionFields(this);
		}

		#endregion
	}
}