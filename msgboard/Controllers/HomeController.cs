using Microsoft.Ajax.Utilities;
using msgboard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Services.Protocols;



namespace msgboard.Controllers
{
    public class HomeController : Controller
    {
        Database1Entities1 db = new Database1Entities1();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(Table m)
        {
            var member = db.Table.Where(model => model.account == m.account).FirstOrDefault();

            if (member == null)
            {

                db.Table.Add(m);
                db.SaveChanges();
                return RedirectToAction("Read");
            }
            else
            {
                ViewBag.msg = "帳號重複，請重新輸入";
                return View();
            }

        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Table m)
        {

            var member = db.Table.Where(model => model.account == m.account && model.password == m.password).FirstOrDefault();
            if (member == null)
            {
                ViewBag.msg = "帳號或密碼錯誤請重新輸入";
                return View();
            }
            else
            {
                Session["Welcome"] = $"{member.name}您好";
                Session["Username"] = member.name;
                Session["id"] = member.Id;
                FormsAuthentication.RedirectFromLoginPage(m.account, true);
                return RedirectToAction("messagebox");
            }

        }

        public ActionResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");

        }
        public ActionResult Read()
        {
            var msg = db.Table.OrderByDescending(m => m.Id).ToList();

            return View(msg);
        }
        public ActionResult messagebox(string searchword)
        {
            var viewModel = new MsgDataSearchViewModel();
            if (!string.IsNullOrEmpty(searchword))
            {
                viewModel.SearchKeyword = searchword;
                viewModel.msgdatas = db.msgdata
                            .Where(m => m.fname == searchword)
                            .OrderByDescending(m => m.fdate)
                            .ToList();
            }
            else
            {
                viewModel.msgdatas = db.msgdata.OrderByDescending(m => m.fdate).ToList();
            }


            if (Session["id"] == null)
            {
                return View("../Home/messagebox", "_Layout", viewModel);
            }
            else if ((string)Session["Username"] == "admin")
            {
                return View("../Home/messagebox", "_Layout-admin", viewModel);
            }
            else
                return View("../Home/messagebox", "_Layout-member", viewModel);
        }
        public ActionResult msg()
        {
            if (Session["id"] == null)
            {
                return RedirectToAction("Login");
            }

            return View();

        }
        [HttpPost]
        public ActionResult msg(string content)
        {
            if (Session["id"] == null)
            {
                return RedirectToAction("Login");
            }

            int id = (int)Session["id"];

            var user = db.Table.Find(id);

            if (!string.IsNullOrEmpty(content))
            {
                var message = new msgdata
                {
                    msg = content,
                    fdate = DateTime.Now,
                    fname = user.name
                };

                db.msgdata.Add(message);
                db.SaveChanges();
                return RedirectToAction("messagebox");
            }
            else { return View(); }
        }

        //public ActionResult reply(int id)
        //{
        //    var member_msg = db.msgdata.Where(m => m.Id == id).FirstOrDefault();
        //    return View(member_msg);
        //}
        [HttpPost]
        public ActionResult reply(msgdata replydata)
        {
            var member_msg = db.msgdata.Where(m => m.Id == replydata.Id).FirstOrDefault();
            if (member_msg!=null)
            {
                member_msg.reply = replydata.reply;
                db.SaveChanges();
                return RedirectToAction("messagebox");
            }
            
            return View(replydata);
        }
        public ActionResult delete(msgdata m)
        {
            var member = db.msgdata.Where(model => model.Id == m.Id).FirstOrDefault();
            if (member == null)
            {
                return View();
            }
            else
            {
                db.msgdata.Remove(member);
                db.SaveChanges();
                return RedirectToAction("messagebox");
            }

        }

        public ActionResult identity()
        {
            int id = (int)Session["id"];
            var IDdata = db.Table.Where(model => model.Id == id).FirstOrDefault();

            if (IDdata == null)
            {
                return HttpNotFound();
            }


            return View(IDdata);
        }


        public ActionResult EditIdentity()
        {
            int id = (int)Session["id"];
            var IDdata = db.Table.Where(model => model.Id == id).FirstOrDefault();


            return View(IDdata);
        }
        [HttpPost]
        public ActionResult EditIdentity(Table e)
        {

            int id = (int)Session["id"];
            var IDdata = db.Table.Where(model => model.Id == id).FirstOrDefault();
            if (IDdata != null)
            {
                IDdata.account = e.account;
                IDdata.password = e.password;
                IDdata.name = e.name;
                IDdata.age = e.age;
                IDdata.birthday = e.birthday;
                db.SaveChanges();
                return RedirectToAction("Identity");
            }
            else { return View(e); }
        }

        public ActionResult MemberShipdelete(int id)
        {
            var member = db.Table.Where(model => model.Id == id).FirstOrDefault();
            if (member == null)
            {
                return View();
            }
            else
            {
                db.Table.Remove(member);
                db.SaveChanges();
                return RedirectToAction("read");
            }

        }

        public ActionResult MemberShipEdit(int id)
        {
            var member = db.Table.Where(model => model.Id == id).FirstOrDefault();
            if (member == null)
            {
                return View();
            }
            return View(member);

        }
        [HttpPost]
       
        public ActionResult MemberShipEdit(Table member)
        {
            if (ModelState.IsValid)
            {
                var existingMember = db.Table.FirstOrDefault(model => model.Id == member.Id);
                if (existingMember != null)
                {
                    existingMember.account = member.account;
                    existingMember.password = member.password;
                    existingMember.name = member.name;
                    existingMember.age = member.age;
                    existingMember.birthday = member.birthday;
                    db.SaveChanges();
                    return RedirectToAction("read");
                }
            }
            return View(member);

        }
    }
}