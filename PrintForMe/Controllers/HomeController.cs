using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.PrintForme;
using CMS.DocumentEngine.Types.PrintForMe;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Web.Mvc;
using PrintForMe.Models;
using PrintForMe.Models.Checkout;
using PrintForMe.Models.Home;
using PrintForMe.Models.Policy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace PrintForMe.Controllers
{
    public class HomeController : Controller
    {
        //private readonly IShoppingService shoppingService;
        private readonly string mCultureName = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
        private readonly string siteName = SiteContext.CurrentSiteName;

        // GET: Home
        public ActionResult Index()
        {
            UserModel.storeLogoPath = Helpers.ContentHelper.GetStoreImage();

            ViewBag.rtl = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.IsRightToLeft;

            // Uncomment and optionally adjust the document query sample when using Page builder on the Home page
            // See ~/App_Start/ApplicationConfig.cs, ~/Views/Shared/_Layout.cshtml and ~/Views/Home/Index.cshtml
            // In the administration UI, create a Page type and set its
            //   URL pattern = '/Home'
            //   Use Page tab = True

            // In the administration UI, create a Page utilizing the new Page type
            Home homeNode = HomeProvider.GetHome("/Home", mCultureName, "PrintForMe")
                                       .Columns("DocumentName", "DocumentID", "LandingImage", "HeadingTitle", "HeadingText")
                                       .CombineWithDefaultCulture();
            //TreeNode page = DocumentHelper.GetDocuments().Path("/Home").OnCurrentSite().TopN(1).FirstOrDefault();
            if (homeNode == null)
            {
                return HttpNotFound();
            }
            var homeModel = new HomeViewModel(homeNode);
            ViewBag.Builder = true;
            HttpContext.Kentico().PageBuilder().Initialize(homeNode.DocumentID);

            return View(homeModel);
        }

        public ActionResult GetTestimonialSection()
        {

            // Loads all testimonial items using the page type's generated provider
            // Uses the testimonial item order from the content tree in the Kentico 'Pages' application
            var testimonials = TestimonialProvider.GetTestimonials()
                .Columns("Name", "Picture", "Content", "Url")
                .Culture(mCultureName)
                .CombineWithDefaultCulture()
                .OrderBy("NodeOrder");

            // Loads the pages selected within the menu items
            // The data only contains values of the NodeGUID identifier column
            var pages = DocumentHelper.GetDocuments()
                .WhereIn("NodeGUID", testimonials.Select(item => item.Name).ToList())
                .Columns("NodeGUID");

            // Creates a collection of view models based on the Testimonial and page data
            var model = testimonials.Select(item => new TestimonialViewModel()
            {
                Name = item.Name,
                PictureUrl = item.Picture,
                Content = item.Content,
                SiteUrl = item.Url
            });
            return PartialView("_testimonials", model);
        }

        [ActionName("terms-and-conditions")]
        public ActionResult Terms()
        {
            // Retrieves the page from the Kentico database
            TreeNode page = DocumentHelper.GetDocuments()
                .Path("/Home/terms-and-conditions")
                .OnCurrentSite()
                .TopN(1)
                .FirstOrDefault();

            // Returns a 404 error when the retrieving is unsuccessful
            if (page == null)
            {
                return HttpNotFound();
            }
            ViewBag.Builder = true;
            // Initializes the page builder with the DocumentID of the page
            HttpContext.Kentico().PageBuilder().Initialize(page.DocumentID);
            return View("Terms");
        }

        [ActionName("terms-and-conditions-for-mobile")]
        public ActionResult TermsForMobile()
        {
            // Retrieves the page from the Kentico database
            TreeNode page = DocumentHelper.GetDocuments()
                .Path("/Home/terms-and-conditions")
                .OnCurrentSite()
                .TopN(1)
                .FirstOrDefault();

            // Returns a 404 error when the retrieving is unsuccessful
            if (page == null)
            {
                return HttpNotFound();
            }
            ViewBag.Builder = true;
            // Initializes the page builder with the DocumentID of the page
            HttpContext.Kentico().PageBuilder().Initialize(page.DocumentID);
            return View("TermsForMobile");
        }

        [ActionName("privacy-policy")]
        public ActionResult PrivacyPolicy()
        {
            // Retrieves the page from the Kentico database
            TreeNode page = DocumentHelper.GetDocuments()
                .Path("/Home/privacy-policy")
                .OnCurrentSite()
                .TopN(1)
                .FirstOrDefault();

            // Returns a 404 error when the retrieving is unsuccessful
            if (page == null)
            {
                return HttpNotFound();
            }
            ViewBag.Builder = true;
            // Initializes the page builder with the DocumentID of the page
            HttpContext.Kentico().PageBuilder().Initialize(page.DocumentID);
            return View("PrivacyPolicy");
        }

        [ActionName("delivery-policy")]
        public ActionResult Questions(String section = "common-questions")
        {
            ViewBag.section = section;

            // Retrieves the page from the Kentico database
            TreeNode page = DocumentHelper.GetDocuments()
                .Path("/Home/Delivery-Policy")
                .OnCurrentSite()
                .TopN(1)
                .FirstOrDefault();

            // Returns a 404 error when the retrieving is unsuccessful
            if (page == null)
            {
                return HttpNotFound();
            }
            ViewBag.Builder = true;

            // Gets all enabled payment methods from Kentico
            var model = new QuestionSectionViewModel();
            IEnumerable<PaymentOptionInfo> enabledPaymentMethods = PaymentOptionInfoProvider.GetPaymentOptions(SiteContext.CurrentSiteID, true).ToList();
            model.PaymentOptions = enabledPaymentMethods.Select(item => new PaymentMethodViewModel()
            {
                DisplayName = item.PaymentOptionDisplayName,
                Description = item.PaymentOptionDescription
            });

            // Gets current Q&As
            // Loads all testimonial items using the page type's generated provider
            // Uses the testimonial item order from the content tree in the Kentico 'Pages' application
            var QAs = QAProvider.GetQAs()
                .Columns("Question", "Answer")
                .Culture(mCultureName)
                .CombineWithDefaultCulture()
                .OrderBy("NodeOrder");
            // Loads the pages selected within the menu items
            // The data only contains values of the NodeGUID identifier column
            var pages = DocumentHelper.GetDocuments()
                .WhereIn("NodeGUID", QAs.Select(item => item.Question).ToList())
                .Columns("NodeGUID");
            // Creates a collection of view models based on the Testimonial and page data
            model.Questions = QAs.Select(item => new QuestionAndAnswerModel()
            {
                Question = item.Question,
                Answer = item.Answer
            });


            // Initializes the page builder with the DocumentID of the page
            HttpContext.Kentico().PageBuilder().Initialize(page.DocumentID);

            return View("Questions", model);
        }

        [ActionName("questions-for-mobile")]
        public ActionResult QuestionsForMobile()
        {            
            // Retrieves the page from the Kentico database
            TreeNode page = DocumentHelper.GetDocuments()
                .Path("/Home/Delivery-Policy")
                .OnCurrentSite()
                .TopN(1)
                .FirstOrDefault();

            // Returns a 404 error when the retrieving is unsuccessful
            if (page == null)
            {
                return HttpNotFound();
            }
            ViewBag.Builder = true;

            // Gets all enabled payment methods from Kentico
            var model = new QuestionSectionViewModel();
            IEnumerable<PaymentOptionInfo> enabledPaymentMethods = PaymentOptionInfoProvider.GetPaymentOptions(SiteContext.CurrentSiteID, true).ToList();
            model.PaymentOptions = enabledPaymentMethods.Select(item => new PaymentMethodViewModel()
            {
                DisplayName = item.PaymentOptionDisplayName,
                Description = item.PaymentOptionDescription
            });

            // Gets current Q&As
            // Loads all testimonial items using the page type's generated provider
            // Uses the testimonial item order from the content tree in the Kentico 'Pages' application
            var QAs = QAProvider.GetQAs()
                .Columns("Question", "Answer")
                .Culture(mCultureName)
                .CombineWithDefaultCulture()
                .OrderBy("NodeOrder");
            // Loads the pages selected within the menu items
            // The data only contains values of the NodeGUID identifier column
            var pages = DocumentHelper.GetDocuments()
                .WhereIn("NodeGUID", QAs.Select(item => item.Question).ToList())
                .Columns("NodeGUID");
            // Creates a collection of view models based on the Testimonial and page data
            model.Questions = QAs.Select(item => new QuestionAndAnswerModel()
            {
                Question = item.Question,
                Answer = item.Answer
            });


            // Initializes the page builder with the DocumentID of the page
            HttpContext.Kentico().PageBuilder().Initialize(page.DocumentID);

            return View("QuestionsForMobile", model);
        }

        public ActionResult Dashboard()
        {
            try
            {
                var orderdetails = OrderInfoProvider.GetOrders().WhereGreaterOrEquals("OrderDate", DateTime.Now.AddDays(-30)).ToList();
                var previousMonthsOrders = OrderInfoProvider.GetOrders().WhereGreaterOrEquals("OrderDate", DateTime.Now.AddDays(-60))
                                                                    .WhereLessOrEquals("OrderDate", DateTime.Now.AddDays(-30)).ToList();

                var orderRatio = ValidationHelper.GetDecimal((((orderdetails.Count() * 100) / previousMonthsOrders.Count()) - 100),0);

                var NoUsers = UserInfoProvider.GetUsers().WhereGreaterOrEquals("UserCreated", DateTime.Now.AddDays(-30)).ToList();

                var monthRevenu = orderdetails.Sum(x => x.OrderGrandTotal);
                var previousmonthRevenu = previousMonthsOrders.Sum(x => x.OrderGrandTotal);
                var totalEarningRatio= ValidationHelper.GetDecimal((((monthRevenu * 100) / previousmonthRevenu) - 100), 0);


                var Inprogress = OrderInfoProvider.GetOrders().ToList().Where(x => x.OrderStatusID == 3).Count();
                var Completed = OrderInfoProvider.GetOrders().ToList().Where(x => x.OrderStatusID == 7).Count();
                var Cancelled = OrderInfoProvider.GetOrders().ToList().Where(x => x.OrderStatusID == 6).Count();
                if (Inprogress < 0)
                {
                    Inprogress = 0;
                }
                if (Completed < 0)
                {
                    Completed = 0;
                }
                if (Cancelled < 0)
                {
                    Cancelled = 0;
                }

                DashBoardViewModel model = new DashBoardViewModel();
                model.NumOfOrder = String.Format("{0:n}", orderdetails.Count());
                model.NumOfOrderPer = ValidationHelper.GetString(Math.Round(orderRatio,2),"");
                model.NewClients = NoUsers.Count();
                model.Totalearning = String.Format("{0:n}", monthRevenu);
                model.TotalearningPer = ValidationHelper.GetString(Math.Round(totalEarningRatio,2),"");
                model.InProgress = Inprogress;
                model.Waiting = Completed;                

                //HitsInfoProvider.GetAllHitsInfo(int siteId, HitsIntervalEnum interval, string codeName, DateTime time)

                //string statisticsCodeName = "visitors";
                //var views = HitsInfoProvider.GetAllHitsInfo(SiteContext.CurrentSiteID, HitsIntervalEnum.Year, statisticsCodeName + ";%", "SUM(HitsCount)",null);

                return View(model);
            }

            catch (Exception ex)
            {
                throw ex;

            }
        }
    }
}