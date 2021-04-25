using CMS.Ecommerce;
using CMS.Globalization;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PrintForMe.Models.Checkout
{
    //DocSection:BillingAddressModel
    public class BillingAddressViewModel
    {
        [Required]
        [DisplayName("Address line 1")]
        [MaxLength(100, ErrorMessage = "The maximum length allowed for the field has been exceeded.")]
        public string Line1 { get; set; }

        [DisplayName("Address line 1")]
        [MaxLength(100, ErrorMessage = "The maximum length allowed for the field has been exceeded.")]
        public string AddressName { get; set; }

        [DisplayName("Address line 2")]
        [MaxLength(100, ErrorMessage = "The maximum length allowed for the field has been exceeded.")]
        public string Line2 { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "The maximum length allowed for the field has been exceeded.")]
        public string City { get; set; }

        [MaxLength(100, ErrorMessage = "Please enter your full name.")]
        public string PersonalName { get; set; }

        //[Required]
        [MaxLength(100, ErrorMessage = "Please enter your mobile number.")]
        public string Phone { get; set; }

        [Required]
        [DisplayName("Postal code")]
        [MaxLength(20, ErrorMessage = "The maximum length allowed for the field has been exceeded.")]
        [DataType(DataType.PostalCode, ErrorMessage = "Please enter a valid postal code.")]
        public string PostalCode { get; set; }

        [DisplayName("Country")]
        public int CountryID { get; set; }

        [DisplayName("State")]
        public int StateID { get; set; }

        public int AddressID { get; set; }
        public int UserID { get; set; }

        public SelectList Countries { get; set; }

        public List<AddressInfo> Addresses { get; set; }
        [DisplayName("Main Address?")]
        public bool MainAddress { get; set; }

        /// <summary>
        /// Creates a billing address model.
        /// </summary>
        /// <param name="address">Billing address.</param>
        /// <param name="countryList">List of countries.</param>
        public BillingAddressViewModel(AddressInfo address, SelectList countries, List<AddressInfo> addresses)
        {
            if (address != null)
            {
                Line1 = address.AddressLine1;
                Line2 = address.AddressLine2;
                City = address.AddressCity;
                PostalCode = address.AddressZip;
                CountryID = address.AddressCountryID;
                StateID = address.AddressStateID;
                AddressID = address.AddressID;
                Phone = address.AddressPhone;
                PersonalName = address.AddressPersonalName;
                UserID = address.GetValue("AddressUserID", 0);
                MainAddress = address.GetValue("MainAddress", false);
            }

            Countries = countries;
            Addresses = addresses;
        }
        public BillingAddressViewModel(AddressInfo address, SelectList countries)
        {
            if (address != null)
            {
                Line1 = address.AddressLine1;
                Line2 = address.AddressLine2;
                City = address.AddressCity;
                PostalCode = address.AddressZip;
                CountryID = address.AddressCountryID;
                StateID = address.AddressStateID;
                AddressID = address.AddressID;
                Phone = address.AddressPhone;
                PersonalName = address.AddressPersonalName;
                UserID = address.GetValue("AddressUserID", 0);
                MainAddress = address.GetValue("MainAddress", false);
                AddressName = address.AddressName;
            }

            Countries = countries;
        }


        /// <summary>
        /// Creates an empty BillingAddressModel object. 
        /// Required by the MVC framework for model binding during form submission.
        /// </summary>
        public BillingAddressViewModel()
        {
            Countries = new SelectList(CountryInfoProvider.GetCountries(), "CountryID", "CountryDisplayName");
        }


        /// <summary>
        /// Applies the model to an AddressInfo object.
        /// </summary>
        /// <param name="address">Billing address to which the model is applied.</param>
        public void ApplyTo(AddressInfo address)
        {
            address.AddressLine1 = Line1;
            address.AddressLine2 = Line2;
            address.AddressCity = City;
            address.AddressZip = PostalCode;
            address.AddressCountryID = CountryID;
            address.AddressStateID = StateID;
            address.AddressPersonalName = PersonalName;
            address.AddressPhone = Phone;
            address.SetValue("AddressUserID", UserID);
            address.SetValue("MainAddress", MainAddress);
            address.AddressName = AddressName;
        }
        public BillingAddressViewModel FillAddress(AddressInfo address)
        {
            var model = new BillingAddressViewModel();
            model.Line1 = address.AddressLine1;
            model.Line2 = address.AddressLine2;
            model.City = address.AddressCity;
            model.PostalCode = address.AddressZip;
            model.CountryID = address.AddressCustomerID;
            model.StateID = address.AddressStateID;
            model.PersonalName = address.AddressPersonalName;
            model.Phone = address.AddressPhone;
            model.UserID = address.GetValue("AddressUserID", 0);
            model.MainAddress = address.GetValue("MainAddress", false);
            model.AddressName = address.AddressName;
            model.AddressID = address.AddressID;

            return model;
        }
    }
    //EndDocSection:BillingAddressModel
}