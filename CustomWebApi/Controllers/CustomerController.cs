using CMS.Ecommerce;
using CMS.Globalization;
using CustomWebApi.Models.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CustomWebApi.Controllers
{    
    public class CustomerController : ApiController
    {
        #region GET
       
        [HttpGet]
        public IHttpActionResult GetCustomerByUserId(int customerUserID)
        {
            try
            {
                // Gets the first customer whose last name is 'Smith'
                CustomerInfo customer = CustomerInfoProvider.GetCustomers()
                                                                .WhereEquals("CustomerUserID", customerUserID)
                                                                .TopN(1)
                                                                .FirstOrDefault();

                if (customer != null)
                {
                    return Ok(new CustomerDetail { 
                        CustomerCompany = customer.CustomerCompany,
                        CustomerEmail = customer.CustomerEmail,
                        CustomerCreated = customer.CustomerCreated,
                        CustomerFax = customer.CustomerFax,
                        CustomerFirstName = customer.CustomerFirstName,
                        CustomerGUID = customer.CustomerGUID.ToString(),
                        CustomerID = customer.CustomerID,
                        CustomerInfoName = customer.CustomerInfoName,
                        CustomerLastName = customer.CustomerLastName,
                        CustomerOrganizationID = customer.CustomerOrganizationID,
                        CustomerPhone = customer.CustomerPhone,
                        CustomerTaxRegistrationID = customer.CustomerTaxRegistrationID,
                        CustomerUserID = customer.CustomerUserID
                    });
                }
                else
                {
                    return Ok("Incorrect CustomerUserId..!");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        
        [HttpGet]
        public IHttpActionResult GetCustomerById(int customerID)
        {
            try
            {
                // Gets the first customer whose last name is 'Smith'
                CustomerInfo customer = CustomerInfoProvider.GetCustomers()
                                                                .WhereEquals("CustomerID", customerID)
                                                                .TopN(1)
                                                                .FirstOrDefault();

                if (customer != null)
                {
                    return Ok(new CustomerDetail
                    {
                        CustomerCompany = customer.CustomerCompany,
                        CustomerEmail = customer.CustomerEmail,
                        CustomerCreated = customer.CustomerCreated,
                        CustomerFax = customer.CustomerFax,
                        CustomerFirstName = customer.CustomerFirstName,
                        CustomerGUID = customer.CustomerGUID.ToString(),
                        CustomerID = customer.CustomerID,
                        CustomerInfoName = customer.CustomerInfoName,
                        CustomerLastName = customer.CustomerLastName,
                        CustomerOrganizationID = customer.CustomerOrganizationID,
                        CustomerPhone = customer.CustomerPhone,
                        CustomerTaxRegistrationID = customer.CustomerTaxRegistrationID,
                        CustomerUserID = customer.CustomerUserID
                    });
                }
                else
                {
                    return Ok("Incorrect CustomerId..!");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        
        [HttpGet]
        public IHttpActionResult GetCustomerAddress(int customerID)
        {
            try
            {
                AddressInfo address = AddressInfoProvider.GetAddresses()
                                                                  .WhereEquals("AddressCustomerID", customerID)
                                                                  .TopN(1)
                                                                  .FirstOrDefault();

                if (address != null)
                {
                    return Ok(new CustomerAddressDetail { 
                        AddressCity = address.AddressCity,
                        AddressCountryID = address.AddressCountryID,
                        AddressCustomerID = address.AddressCustomerID,
                        AddressGUID = address.AddressGUID,
                        AddressID = address.AddressID,
                        AddressLastModified = address.AddressLastModified,
                        AddressLine1 = address.AddressLine1,
                        AddressLine2 = address.AddressLine2,
                        AddressName = address.AddressName,
                        AddressPersonalName = address.AddressPersonalName,
                        AddressPhone = address.AddressPhone,
                        AddressStateID = address.AddressStateID,
                        AddressZip = address.AddressZip
                    });
                }
                else
                {
                    return Ok("Incorrect CustomerId..!");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        #endregion

        #region Post        
        [HttpPost]
        public IHttpActionResult UpdateCustomer([FromBody] UpdateCustomer updateCustomer)
        {
            try
            {
                // Gets the first customer whose last name is 'Smith'
                CustomerInfo customer = CustomerInfoProvider.GetCustomers()
                                                                .WhereEquals("CustomerID", updateCustomer.CustomerID)
                                                                .TopN(1)
                                                                .FirstOrDefault();
                if (customer != null)
                {
                    // Updates the customer's properties
                    customer.CustomerFirstName = updateCustomer.CustomerFirstName;
                    customer.CustomerLastName = updateCustomer.CustomerLastName;
                    customer.CustomerEmail = updateCustomer.CustomerEmail;
                    customer.CustomerPhone = updateCustomer.CustomerPhone;

                    // Saves the changes to the database
                    CustomerInfoProvider.SetCustomerInfo(customer);

                    return Ok(new CustomerDetail
                    {
                        CustomerCompany = customer.CustomerCompany,
                        CustomerEmail = customer.CustomerEmail,
                        CustomerCreated = customer.CustomerCreated,
                        CustomerFax = customer.CustomerFax,
                        CustomerFirstName = customer.CustomerFirstName,
                        CustomerGUID = customer.CustomerGUID.ToString(),
                        CustomerID = customer.CustomerID,
                        CustomerInfoName = customer.CustomerInfoName,
                        CustomerLastName = customer.CustomerLastName,
                        CustomerOrganizationID = customer.CustomerOrganizationID,
                        CustomerPhone = customer.CustomerPhone,
                        CustomerTaxRegistrationID = customer.CustomerTaxRegistrationID,
                        CustomerUserID = customer.CustomerUserID
                    });
                }
                else
                {
                    return Ok("Incorrect Customer info..!");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        
        [HttpPost]
        public IHttpActionResult CreateAddress([FromBody] CreateAddressDto createAddress)
        {
            try
            {
                // Gets the first customer whose last name is 'Smith'
                CustomerInfo customer = CustomerInfoProvider.GetCustomers()
                                                                .WhereEquals("CustomerID", createAddress.customerID)
                                                                .TopN(1)
                                                                .FirstOrDefault();

                // Gets a country for the address
                CountryInfo country = CountryInfoProvider.GetCountryInfo(createAddress.countryName);

                // Gets a state for the address
                StateInfo state = StateInfoProvider.GetStateInfo(createAddress.stateName);

                if ((customer != null) && (country != null))
                {
                    // Creates a new address object and sets its properties
                    AddressInfo newAddress = new AddressInfo
                    {
                        AddressName = createAddress.addressName,
                        AddressLine1 = createAddress.addressLine1,
                        AddressLine2 = createAddress.addressLine2,
                        AddressCity = createAddress.addressCity,
                        AddressZip = createAddress.addressZip,
                        AddressPhone = createAddress.addressPhone,
                        AddressPersonalName = customer.CustomerInfoName,
                        AddressCustomerID = customer.CustomerID,
                        AddressCountryID = country.CountryID,
                        AddressStateID = state != null ? state.StateID : 0
                    };

                    // Saves the address to the dataabase
                    AddressInfoProvider.SetAddressInfo(newAddress);

                    return Ok(newAddress);
                }
                else
                {
                    return Ok("Incorrect Customer info..!");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        
        [HttpPost]
        public IHttpActionResult UpdateAddress([FromBody] UpdateAddressDto updateAddress)
        {
            try
            {
                // Gets a country for the address
                CountryInfo country = CountryInfoProvider.GetCountryInfo(updateAddress.countryName);

                // Gets a state for the address
                StateInfo state = StateInfoProvider.GetStateInfo(updateAddress.stateName);

                AddressInfo address = AddressInfoProvider.GetAddresses()
                                                  .WhereEquals("AddressCustomerID", updateAddress.customerID)
                                                  .TopN(1)
                                                  .FirstOrDefault();

                if ((address != null) && (country != null))
                {
                    // Updates the address properties
                    address.AddressName = updateAddress.addressName;
                    address.AddressLine1 = updateAddress.addressLine1;
                    address.AddressLine2 = updateAddress.addressLine2;
                    address.AddressCity = updateAddress.addressCity;
                    address.AddressZip = updateAddress.addressZip;
                    address.AddressPhone = updateAddress.addressPhone;
                    address.AddressCustomerID = updateAddress.customerID;
                    address.AddressCountryID = country.CountryID;
                    address.AddressStateID = state != null ? state.StateID : 0;

                    // Saves the changes to the database
                    AddressInfoProvider.SetAddressInfo(address);

                    return Ok(address);
                }
                else
                {
                    return Ok("Incorrect Customer info..!");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        #endregion

        #region DELETE        
        [HttpDelete]
        public IHttpActionResult DeleteAddress(int addressId)
        {
            try
            {
                // Gets the address
                AddressInfo address = AddressInfoProvider.GetAddresses()
                                                              .WhereStartsWith("AddressID", addressId.ToString())
                                                              .TopN(1)
                                                              .FirstOrDefault();


                if (address != null)
                {
                    // Deletes the address
                    AddressInfoProvider.DeleteAddressInfo(address);

                    return Ok(true);
                }
                else
                {
                    return Ok(false);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        #endregion

    }
}
