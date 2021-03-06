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

[assembly: RegisterDocumentType(ServiceSettings.CLASS_NAME, typeof(ServiceSettings))]

namespace CMS.DocumentEngine.Types.PrintForme
{
	/// <summary>
	/// Represents a content item of type ServiceSettings.
	/// </summary>
	public partial class ServiceSettings : TreeNode
	{
		#region "Constants and variables"

		/// <summary>
		/// The name of the data class.
		/// </summary>
		public const string CLASS_NAME = "PrintForme.ServiceSettings";


		/// <summary>
		/// The instance of the class that provides extended API for working with ServiceSettings fields.
		/// </summary>
		private readonly ServiceSettingsFields mFields;

		#endregion


		#region "Properties"

		/// <summary>
		/// ServiceSettingsID.
		/// </summary>
		[DatabaseIDField]
		public int ServiceSettingsID
		{
			get
			{
				return ValidationHelper.GetInteger(GetValue("ServiceSettingsID"), 0);
			}
			set
			{
				SetValue("ServiceSettingsID", value);
			}
		}


		/// <summary>
		/// Service ID.
		/// </summary>
		[DatabaseField]
		public int ServiceID
		{
			get
			{
				return ValidationHelper.GetInteger(GetValue("ServiceID"), 0);
			}
			set
			{
				SetValue("ServiceID", value);
			}
		}


		/// <summary>
		/// Code.
		/// </summary>
		[DatabaseField]
		public string Code
		{
			get
			{
				return ValidationHelper.GetString(GetValue("Code"), @"");
			}
			set
			{
				SetValue("Code", value);
			}
		}


		/// <summary>
		/// Description.
		/// </summary>
		[DatabaseField]
		public string Description
		{
			get
			{
				return ValidationHelper.GetString(GetValue("Description"), @"");
			}
			set
			{
				SetValue("Description", value);
			}
		}


		/// <summary>
		/// Price.
		/// </summary>
		[DatabaseField]
		public string Price
		{
			get
			{
				return ValidationHelper.GetString(GetValue("Price"), @"");
			}
			set
			{
				SetValue("Price", value);
			}
		}


		/// <summary>
		/// Availability.
		/// </summary>
		[DatabaseField]
		public bool Availability
		{
			get
			{
				return ValidationHelper.GetBoolean(GetValue("Availability"), true);
			}
			set
			{
				SetValue("Availability", value);
			}
		}


		/// <summary>
		/// Gets an object that provides extended API for working with ServiceSettings fields.
		/// </summary>
		[RegisterProperty]
		public ServiceSettingsFields Fields
		{
			get
			{
				return mFields;
			}
		}


		/// <summary>
		/// Provides extended API for working with ServiceSettings fields.
		/// </summary>
		[RegisterAllProperties]
		public partial class ServiceSettingsFields : AbstractHierarchicalObject<ServiceSettingsFields>
		{
			/// <summary>
			/// The content item of type ServiceSettings that is a target of the extended API.
			/// </summary>
			private readonly ServiceSettings mInstance;


			/// <summary>
			/// Initializes a new instance of the <see cref="ServiceSettingsFields" /> class with the specified content item of type ServiceSettings.
			/// </summary>
			/// <param name="instance">The content item of type ServiceSettings that is a target of the extended API.</param>
			public ServiceSettingsFields(ServiceSettings instance)
			{
				mInstance = instance;
			}


			/// <summary>
			/// ServiceSettingsID.
			/// </summary>
			public int ID
			{
				get
				{
					return mInstance.ServiceSettingsID;
				}
				set
				{
					mInstance.ServiceSettingsID = value;
				}
			}


			/// <summary>
			/// Service ID.
			/// </summary>
			public int ServiceID
			{
				get
				{
					return mInstance.ServiceID;
				}
				set
				{
					mInstance.ServiceID = value;
				}
			}


			/// <summary>
			/// Code.
			/// </summary>
			public string Code
			{
				get
				{
					return mInstance.Code;
				}
				set
				{
					mInstance.Code = value;
				}
			}


			/// <summary>
			/// Description.
			/// </summary>
			public string Description
			{
				get
				{
					return mInstance.Description;
				}
				set
				{
					mInstance.Description = value;
				}
			}


			/// <summary>
			/// Price.
			/// </summary>
			public string Price
			{
				get
				{
					return mInstance.Price;
				}
				set
				{
					mInstance.Price = value;
				}
			}


			/// <summary>
			/// Availability.
			/// </summary>
			public bool Availability
			{
				get
				{
					return mInstance.Availability;
				}
				set
				{
					mInstance.Availability = value;
				}
			}
		}

		#endregion


		#region "Constructors"

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceSettings" /> class.
		/// </summary>
		public ServiceSettings() : base(CLASS_NAME)
		{
			mFields = new ServiceSettingsFields(this);
		}

		#endregion
	}
}