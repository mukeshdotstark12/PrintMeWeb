using CMS;
using CMS.DataEngine;
using CMS.Helpers;
using PrintForMe;
using System;
using System.Data;
using System.Runtime.Serialization;

[assembly: RegisterObjectType(typeof(PaymentInfo), PaymentInfo.OBJECT_TYPE)]

namespace PrintForMe
{
    /// <summary>
    /// Data container class for <see cref="PaymentInfo"/>.
    /// </summary>
    [Serializable]
    public partial class PaymentInfo : AbstractInfo<PaymentInfo>
    {
        /// <summary>
        /// Object type.
        /// </summary>
        public const string OBJECT_TYPE = "printforme.payment";


        /// <summary>
        /// Type information.
        /// </summary>
        public static readonly ObjectTypeInfo TYPEINFO = new ObjectTypeInfo(typeof(PaymentInfoProvider), OBJECT_TYPE, "PrintForMe.Payment", "PaymentID", "PaymentLastModified", "PaymentGuid", null, null, null, null, null, null)
        {
            ModuleName = "PrintForMe-E-commerce",
            TouchCacheDependencies = true,
        };


        /// <summary>
        /// Payment ID.
        /// </summary>
        [DatabaseField]
        public virtual int PaymentID
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("PaymentID"), 0);
            }
            set
            {
                SetValue("PaymentID", value);
            }
        }


        /// <summary>
        /// Payment datetime.
        /// </summary>
        [DatabaseField]
        public virtual DateTime PaymentDatetime
        {
            get
            {
                return ValidationHelper.GetDateTime(GetValue("PaymentDatetime"), DateTimeHelper.ZERO_TIME);
            }
            set
            {
                SetValue("PaymentDatetime", value, DateTimeHelper.ZERO_TIME);
            }
        }


        /// <summary>
        /// Pay tabs payment ID.
        /// </summary>
        [DatabaseField]
        public virtual string PayTabsPaymentID
        {
            get
            {
                return ValidationHelper.GetString(GetValue("PayTabsPaymentID"), String.Empty);
            }
            set
            {
                SetValue("PayTabsPaymentID", value, String.Empty);
            }
        }


        /// <summary>
        /// Amount.
        /// </summary>
        [DatabaseField]
        public virtual decimal Amount
        {
            get
            {
                return ValidationHelper.GetDecimal(GetValue("Amount"), 0m);
            }
            set
            {
                SetValue("Amount", value, 0m);
            }
        }


        /// <summary>
        /// Fees.
        /// </summary>
        [DatabaseField]
        public virtual decimal Fees
        {
            get
            {
                return ValidationHelper.GetDecimal(GetValue("Fees"), 0m);
            }
            set
            {
                SetValue("Fees", value, 0m);
            }
        }


        /// <summary>
        /// Comment.
        /// </summary>
        [DatabaseField]
        public virtual string Comment
        {
            get
            {
                return ValidationHelper.GetString(GetValue("Comment"), String.Empty);
            }
            set
            {
                SetValue("Comment", value, String.Empty);
            }
        }


        /// <summary>
        /// Status.
        /// </summary>
        [DatabaseField]
        public virtual string Status
        {
            get
            {
                return ValidationHelper.GetString(GetValue("Status"), String.Empty);
            }
            set
            {
                SetValue("Status", value, String.Empty);
            }
        }


        /// <summary>
        /// Type.
        /// </summary>
        [DatabaseField]
        public virtual string Type
        {
            get
            {
                return ValidationHelper.GetString(GetValue("Type"), String.Empty);
            }
            set
            {
                SetValue("Type", value, String.Empty);
            }
        }


        /// <summary>
        /// Order ID.
        /// </summary>
        [DatabaseField]
        public virtual int OrderID
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("OrderID"), 0);
            }
            set
            {
                SetValue("OrderID", value, 0);
            }
        }


        /// <summary>
        /// Sender ID.
        /// </summary>
        [DatabaseField]
        public virtual int SenderID
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("SenderID"), 0);
            }
            set
            {
                SetValue("SenderID", value, 0);
            }
        }


        /// <summary>
        /// Receiver ID.
        /// </summary>
        [DatabaseField]
        public virtual int ReceiverID
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("ReceiverID"), 0);
            }
            set
            {
                SetValue("ReceiverID", value, 0);
            }
        }


        /// <summary>
        /// Payment method ID.
        /// </summary>
        [DatabaseField]
        public virtual int PaymentMethodID
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("PaymentMethodID"), 0);
            }
            set
            {
                SetValue("PaymentMethodID", value, 0);
            }
        }


        /// <summary>
        /// Payment guid.
        /// </summary>
        [DatabaseField]
        public virtual Guid PaymentGuid
        {
            get
            {
                return ValidationHelper.GetGuid(GetValue("PaymentGuid"), Guid.Empty);
            }
            set
            {
                SetValue("PaymentGuid", value);
            }
        }


        /// <summary>
        /// Payment last modified.
        /// </summary>
        [DatabaseField]
        public virtual DateTime PaymentLastModified
        {
            get
            {
                return ValidationHelper.GetDateTime(GetValue("PaymentLastModified"), DateTimeHelper.ZERO_TIME);
            }
            set
            {
                SetValue("PaymentLastModified", value);
            }
        }


        /// <summary>
        /// Deletes the object using appropriate provider.
        /// </summary>
        protected override void DeleteObject()
        {
            PaymentInfoProvider.DeletePaymentInfo(this);
        }


        /// <summary>
        /// Updates the object using appropriate provider.
        /// </summary>
        protected override void SetObject()
        {
            PaymentInfoProvider.SetPaymentInfo(this);
        }


        /// <summary>
        /// Constructor for de-serialization.
        /// </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Streaming context.</param>
        protected PaymentInfo(SerializationInfo info, StreamingContext context)
            : base(info, context, TYPEINFO)
        {
        }


        /// <summary>
        /// Creates an empty instance of the <see cref="PaymentInfo"/> class.
        /// </summary>
        public PaymentInfo()
            : base(TYPEINFO)
        {
        }


        /// <summary>
        /// Creates a new instances of the <see cref="PaymentInfo"/> class from the given <see cref="DataRow"/>.
        /// </summary>
        /// <param name="dr">DataRow with the object data.</param>
        public PaymentInfo(DataRow dr)
            : base(TYPEINFO, dr)
        {
        }
    }
}