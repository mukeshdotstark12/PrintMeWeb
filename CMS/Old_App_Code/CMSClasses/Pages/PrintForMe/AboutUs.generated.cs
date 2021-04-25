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
using CMS.DocumentEngine.Types.PrintForme;
using CMS.DocumentEngine;

[assembly: RegisterDocumentType(AboutUs.CLASS_NAME, typeof(AboutUs))]

namespace CMS.DocumentEngine.Types.PrintForme
{
	/// <summary>
	/// Represents a content item of type AboutUs.
	/// </summary>
	public partial class AboutUs : TreeNode
	{
		#region "Constants and variables"

		/// <summary>
		/// The name of the data class.
		/// </summary>
		public const string CLASS_NAME = "PrintForme.AboutUs";


		/// <summary>
		/// The instance of the class that provides extended API for working with AboutUs fields.
		/// </summary>
		private readonly AboutUsFields mFields;

		#endregion


		#region "Properties"

		/// <summary>
		/// AboutUsID.
		/// </summary>
		[DatabaseIDField]
		public int AboutUsID
		{
			get
			{
				return ValidationHelper.GetInteger(GetValue("AboutUsID"), 0);
			}
			set
			{
				SetValue("AboutUsID", value);
			}
		}


		/// <summary>
		/// Name.
		/// </summary>
		[DatabaseField]
		public string Name
		{
			get
			{
				return ValidationHelper.GetString(GetValue("Name"), @"");
			}
			set
			{
				SetValue("Name", value);
			}
		}


		/// <summary>
		/// Heading.
		/// </summary>
		[DatabaseField]
		public string Heading
		{
			get
			{
				return ValidationHelper.GetString(GetValue("Heading"), @"");
			}
			set
			{
				SetValue("Heading", value);
			}
		}


		/// <summary>
		/// SubHeading.
		/// </summary>
		[DatabaseField]
		public string SubHeading
		{
			get
			{
				return ValidationHelper.GetString(GetValue("SubHeading"), @"");
			}
			set
			{
				SetValue("SubHeading", value);
			}
		}


		/// <summary>
		/// Text.
		/// </summary>
		[DatabaseField]
		public string Text
		{
			get
			{
				return ValidationHelper.GetString(GetValue("Text"), @"");
			}
			set
			{
				SetValue("Text", value);
			}
		}


		/// <summary>
		/// Image.
		/// </summary>
		[DatabaseField]
		public string Image
		{
			get
			{
				return ValidationHelper.GetString(GetValue("Image"), @"");
			}
			set
			{
				SetValue("Image", value);
			}
		}


		/// <summary>
		/// Gets an object that provides extended API for working with AboutUs fields.
		/// </summary>
		[RegisterProperty]
		public AboutUsFields Fields
		{
			get
			{
				return mFields;
			}
		}


		/// <summary>
		/// Provides extended API for working with AboutUs fields.
		/// </summary>
		[RegisterAllProperties]
		public partial class AboutUsFields : AbstractHierarchicalObject<AboutUsFields>
		{
			/// <summary>
			/// The content item of type AboutUs that is a target of the extended API.
			/// </summary>
			private readonly AboutUs mInstance;


			/// <summary>
			/// Initializes a new instance of the <see cref="AboutUsFields" /> class with the specified content item of type AboutUs.
			/// </summary>
			/// <param name="instance">The content item of type AboutUs that is a target of the extended API.</param>
			public AboutUsFields(AboutUs instance)
			{
				mInstance = instance;
			}


			/// <summary>
			/// AboutUsID.
			/// </summary>
			public int ID
			{
				get
				{
					return mInstance.AboutUsID;
				}
				set
				{
					mInstance.AboutUsID = value;
				}
			}


			/// <summary>
			/// Name.
			/// </summary>
			public string Name
			{
				get
				{
					return mInstance.Name;
				}
				set
				{
					mInstance.Name = value;
				}
			}


			/// <summary>
			/// Heading.
			/// </summary>
			public string Heading
			{
				get
				{
					return mInstance.Heading;
				}
				set
				{
					mInstance.Heading = value;
				}
			}


			/// <summary>
			/// SubHeading.
			/// </summary>
			public string SubHeading
			{
				get
				{
					return mInstance.SubHeading;
				}
				set
				{
					mInstance.SubHeading = value;
				}
			}


			/// <summary>
			/// Text.
			/// </summary>
			public string Text
			{
				get
				{
					return mInstance.Text;
				}
				set
				{
					mInstance.Text = value;
				}
			}


			/// <summary>
			/// Image.
			/// </summary>
			public string Image
			{
				get
				{
					return mInstance.Image;
				}
				set
				{
					mInstance.Image = value;
				}
			}
		}

		#endregion


		#region "Constructors"

		/// <summary>
		/// Initializes a new instance of the <see cref="AboutUs" /> class.
		/// </summary>
		public AboutUs() : base(CLASS_NAME)
		{
			mFields = new AboutUsFields(this);
		}

		#endregion
	}
}