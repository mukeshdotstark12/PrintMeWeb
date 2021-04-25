using CMS.Core;
using CMS.Helpers;
using CMS.Newsletters;
using CMS.SiteProvider;
using Kentico.Membership;
using Microsoft.AspNet.Identity.Owin;
using PrintForMe.Models.Subscription;
using System.Web;
using System.Web.Mvc;

namespace PrintForMe.Controllers
{
    public class SubscriptionController : Controller
    {
        private SubscribeSettings mNewsletterSubscriptionSettings;

        private readonly ISubscriptionService mSubscriptionService;
        private readonly ISubscriptionApprovalService mSubscriptionApprovalService;
        private readonly IUnsubscriptionProvider mUnsubscriptionProvider;
        private readonly IEmailHashValidator mEmailHashValidator;
        private readonly IContactProvider mContactProvider;

        private SubscribeSettings NewsletterSubscriptionSettings
        {
            get
            {
                return mNewsletterSubscriptionSettings ?? (mNewsletterSubscriptionSettings = new SubscribeSettings
                {
                    AllowOptIn = true,
                    RemoveUnsubscriptionFromNewsletter = true,
                    RemoveAlsoUnsubscriptionFromAllNewsletters = true,
                    SendConfirmationEmail = true
                });
            }
        }

        private UserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<UserManager>();
            }
        }


        public SubscriptionController()
        {
            mSubscriptionService = Service.Resolve<ISubscriptionService>();
            mSubscriptionApprovalService = Service.Resolve<ISubscriptionApprovalService>();
            mUnsubscriptionProvider = Service.Resolve<IUnsubscriptionProvider>();
            mEmailHashValidator = Service.Resolve<IEmailHashValidator>(); ;
            mContactProvider = Service.Resolve<IContactProvider>();
        }

        // POST: Subscription/Subscribe
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Subscribe(SubscribeModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_Subscribe", model);
            }

            var newsletter = NewsletterInfoProvider.GetNewsletterInfo("PrintForMeMvcNewsletter", SiteContext.CurrentSiteID);
            var contact = mContactProvider.GetContactForSubscribing(model.Email);

            string resultMessage;
            if (!mSubscriptionService.IsMarketable(contact, newsletter))
            {
                mSubscriptionService.Subscribe(contact, newsletter, NewsletterSubscriptionSettings);

                // The subscription service is configured to use double opt-in, but newsletter must allow for it
                resultMessage = ResHelper.GetString(newsletter.NewsletterEnableOptIn ? "PrintForMe.ConfirmationSent" : "PrintForMe.Subscribed");
            }
            else
            {
                resultMessage = ResHelper.GetString("PrintForMe.AlreadySubscribed");
            }

            return Content(resultMessage);
        }

        // GET: Subscription/Show
        public ActionResult Show()
        {
            //if (User.Identity.IsAuthenticated)
            //{
            //    // Handle authenticated user
            //    return PartialView("_SubscribeAuthenticated");
            //}
            var model = new SubscribeModel();

            return PartialView("_Subscribe", model);
        }
    }
}