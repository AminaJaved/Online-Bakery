using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class UserController : Controller
    {

        BakerySystemEntities1 db = new BakerySystemEntities1();
        // GET: User
        public ActionResult Index(int ?page)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.Categories.Where(x => x.CategoryStatus == 1).OrderByDescending(x => x.CategoryID).ToList();
            IPagedList<Category> stu = list.ToPagedList(pageindex, pagesize);


            return View(stu);
        }
        public ActionResult SignUp()
        {

            return View();
        }

        [HttpPost]
        public ActionResult SignUp(Customer uvm, HttpPostedFileBase imgfile)
        {
            try
            {
                string path = uploadimgfile(imgfile);
                if (path.Equals("-1"))
                {
                    ViewBag.error = "Image could not be uploaded....";
                }
                else
                {
                    Customer u = new Customer();
                    u.UserName = uvm.UserName;
                    u.EmailAddress = uvm.EmailAddress;
                    u.Password = uvm.Password;
                    u.HomeAdress = uvm.HomeAdress;
                    u.UserImage = path;
                    u.ContactNumber = uvm.ContactNumber;
                    db.Customers.Add(u);
                    db.SaveChanges();
                    return RedirectToAction("login");

                }
            }
            catch
            {

                return View();
            }
            return View();
        } //method......................... end.....................
        [HttpGet]
        public ActionResult login()
        {
            return View();
        }


        [HttpPost]
        public ActionResult login(Customer avm)
        {
            Customer ad = db.Customers.Where(x => x.EmailAddress == avm.EmailAddress && x.Password == avm.Password).SingleOrDefault();
            if (ad != null)
            {

                Session["CustomerID"] = ad.CustomerID.ToString();
                return RedirectToAction("Index");

            }
            else
            {
                ViewBag.error = "Invalid username or password";

            }

            return View();
        }


        [HttpGet]
        public ActionResult CreateAd()
        {
            List<Category> li = db.Categories.ToList();
            ViewBag.categorylist = new SelectList(li, "CategoryID", "CategoryName");

            return View();
        }

        [HttpPost]
        public ActionResult CreateAd(Product pvm, HttpPostedFileBase imgfile)
        {
            List<Product> li = db.Products.ToList();
            ViewBag.categorylist = new SelectList(li, "CategoryID", "CategoryName");
            
            
         string path = uploadimgfile(imgfile);
        if (path.Equals("-1"))
        {
            ViewBag.error = "Image could not be uploaded....";
        }
        else
        {
            Product p = new Product();
            p.ProductName = pvm.ProductName;
            p.ProductPrice = pvm.ProductPrice;
            p.ProductImage = path;
            p.ProductStatus = 1;
            p.ProdctFK_category = pvm.ProdctFK_category;
            p.ProductDescription = pvm.ProductDescription;
            p.ProdctFK_customer = Convert.ToInt32(Session["CustomerID"].ToString());
            db.Products.Add(p);
            db.SaveChanges();
            Response.Redirect("index");

        }
            
            return View();
        }


        public ActionResult Ads(int ?id, int?page)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.Products.Where(x=>x.ProdctFK_category==id).OrderByDescending(x=>x.ProductID).ToList();
            IPagedList<Product> stu = list.ToPagedList(pageindex, pagesize);


            return View(stu);

           
        }
        //For search filter
        [HttpPost]
        public ActionResult Ads(int? id, int? page,string search)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.Products.Where(x => x.ProductName.Contains(search)).OrderByDescending(x => x.ProductID).ToList(); 
            IPagedList<Product> stu = list.ToPagedList(pageindex, pagesize);


            return View(stu);


        }



        public ActionResult AllProducts(int? page)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.Products.Where(x => x.ProductStatus == 1).OrderByDescending(x => x.ProductID).ToList();
            IPagedList<Product> stu = list.ToPagedList(pageindex, pagesize);


            return View(stu);
        }


















        public string uploadimgfile(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();
            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
                {
                    try
                    {

                        path = Path.Combine(Server.MapPath("~/Content/upload"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "~/Content/upload/" + random + Path.GetFileName(file.FileName);

                        //    ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        path = "-1";
                    }
                }
                else
                {
                    Response.Write("<script>alert('Only jpg ,jpeg or png formats are acceptable....'); </script>");
                }
            }

            else
            {
                Response.Write("<script>alert('Please select a file'); </script>");
                path = "-1";
            }



            return path;
        }


        public ActionResult ProductDetails(int? id)
        {
            Adviewmodel ad = new Adviewmodel();
            Product p = db.Products.Where(x => x.ProductID == id).SingleOrDefault();
            ad.ProductID = p.ProductID;
            ad.ProductName = p.ProductName;
            ad.ProductImage = p.ProductImage;
            ad.ProductPrice = p.ProductPrice;
            Category cat = db.Categories.Where(x => x.CategoryID == p.ProdctFK_category).SingleOrDefault();
            ad.CategoryName = cat.CategoryName;
            Customer u = db.Customers.Where(x => x.CustomerID == p.ProdctFK_customer).SingleOrDefault();
            //ad.UserName = u.UserName;
            //ad.UserImage = u.UserImage;
            //ad.ContactNumber = u.ContactNumber;





            return View(ad);
        }


        public ActionResult Signout()
        {
            Session.RemoveAll();
            Session.Abandon();

            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Cart()
        {
            String user = User.Identity.Name;
            return View();          
        }
        [HttpPost]

        public ActionResult Cart(Product pr)
        {
            List<Cart> list = db.Carts.ToList();

            try
            {
                string email;
                email = User.Identity.Name;
                int userid=0;
                BakerySystemEntities1 db1 = new BakerySystemEntities1();
                List<Customer> l1 = db1.Customers.ToList<Customer>();
                foreach (Customer a in l1)
                {
                    if (a.EmailAddress == email)
                    {
                        userid = a.CustomerID;
                        break;
                    }
                }






                Cart p = new Cart();
                p.Name = pr.ProductName;
                p.Price = pr.ProductPrice;
                p.Image = pr.ProductImage;
                p.Description = pr.ProductDescription;

                list.Add(p);
                db.Carts.Add(p);                
                db.SaveChanges();

                return View(list);
            }
            catch
            {
                return View();
            }

            
        }


        public ActionResult order()
        {
            return View();
        }

        public ActionResult OrderNow(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            if (Session["CART"] == null)
            {
                List<MyCart> IsCart = new List<MyCart>
                {
                    new MyCart(db.Products.Find(id),1)
                };
                Session["CART"] = IsCart;
            }
            else
            {
                List<MyCart> IsCart = (List<MyCart>)Session["CART"];
                int Check = IsExist(id);
                if(Check==-1)
                {
                    IsCart.Add(new MyCart(db.Products.Find(id), 1));
                }
                else
                {
                    IsCart[Check].Quantity++;
                }
              //  IsCart.Add(new MyCart(db.Products.Find(id), 1));
                Session["CART"] = IsCart;
            }
            return View("order");
        }

        public int IsExist(int? id)
        {
            List<MyCart> IsCart = (List<MyCart>)Session["CART"];
            for(int i=0; i< IsCart.Count; i++)
            {
                if(IsCart[i].Product.ProductID==id)
                {
                    return i;
                }
            }
            return -1;
        }
        public ActionResult DeleteOrder(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            int Check = IsExist(id);
            List<MyCart> IsCart = (List<MyCart>)Session["CART"];
            IsCart.RemoveAt(Check);
            return View("order");
        }
    }
}