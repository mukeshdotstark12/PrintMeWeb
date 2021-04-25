using System;
using System.Data;

using CMS.Base;
using CMS.DataEngine;
using CMS.Helpers;

namespace PrintForMe
{
    /// <summary>
    /// Class providing <see cref="PaymentInfo"/> management.
    /// </summary>
    public partial class PaymentInfoProvider : AbstractInfoProvider<PaymentInfo, PaymentInfoProvider>
    {
        /// <summary>
        /// Creates an instance of <see cref="PaymentInfoProvider"/>.
        /// </summary>
        public PaymentInfoProvider()
            : base(PaymentInfo.TYPEINFO)
        {
        }


        /// <summary>
        /// Returns a query for all the <see cref="PaymentInfo"/> objects.
        /// </summary>
        public static ObjectQuery<PaymentInfo> GetPayments()
        {
            return ProviderObject.GetObjectQuery();
        }


        /// <summary>
        /// Returns <see cref="PaymentInfo"/> with specified ID.
        /// </summary>
        /// <param name="id"><see cref="PaymentInfo"/> ID.</param>
        public static PaymentInfo GetPaymentInfo(int id)
        {
            return ProviderObject.GetInfoById(id);
        }


        /// <summary>
        /// Sets (updates or inserts) specified <see cref="PaymentInfo"/>.
        /// </summary>
        /// <param name="infoObj"><see cref="PaymentInfo"/> to be set.</param>
        public static void SetPaymentInfo(PaymentInfo infoObj)
        {
            ProviderObject.SetInfo(infoObj);
        }


        /// <summary>
        /// Deletes specified <see cref="PaymentInfo"/>.
        /// </summary>
        /// <param name="infoObj"><see cref="PaymentInfo"/> to be deleted.</param>
        public static void DeletePaymentInfo(PaymentInfo infoObj)
        {
            ProviderObject.DeleteInfo(infoObj);
        }


        /// <summary>
        /// Deletes <see cref="PaymentInfo"/> with specified ID.
        /// </summary>
        /// <param name="id"><see cref="PaymentInfo"/> ID.</param>
        public static void DeletePaymentInfo(int id)
        {
            PaymentInfo infoObj = GetPaymentInfo(id);
            DeletePaymentInfo(infoObj);
        }
    }
}