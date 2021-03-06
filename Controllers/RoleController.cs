using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp4.Specification;
using WebApp4.Condition;
using WebApp4.Entities;
using WebApp4.Models;
using PagedList;
using BootstrapMvcSample.Controllers;
using NLog;
using WebApp4.Infrastructure;

namespace WebApp4.Controllers
{
    public class RoleController : BootstrapBaseController
    {
        private readonly string[] updateAttr = new string[] { "RoleName", "Description", "ModifiedOn", "ModifiedBy", "IsDeleted" };
        private readonly IRoleRepository roleRepository;
        private readonly ILogger logger;

        //// If you are using Dependency Injection, you can delete the following constructor
        //public RoleController()
        //    : this(new RoleRepository())
        //{
        //}

        public RoleController(IRoleRepository roleRepository, ILogger logger)
        {
            this.roleRepository = roleRepository;
            this.logger = logger;
        }

        //
        // Search Method

        private List<SearchConditionGroup> BuildCondition()
        {
            List<SearchConditionGroup> _scgList = new List<SearchConditionGroup>();

            //
            List<SearchCondition> _sc1 = new List<SearchCondition>();
            string param = Request.QueryString["param"];
            if (!string.IsNullOrEmpty(param))
            {
                _sc1.Add(new SearchCondition() { PropertyName = "RoleName", Operation = SearchOperationEnum.Contains, PropertyValue = param });
            }
            SearchConditionGroup _scg1 = new SearchConditionGroup() {ConditionList=_sc1,ConstraintType=ConstraintType.Or };

            //
            List<SearchCondition> _sc2 = new List<SearchCondition>();
            if (!string.IsNullOrEmpty(param))
            {
                _sc2.Add(new SearchCondition() { PropertyName = "Description", Operation = SearchOperationEnum.Contains, PropertyValue = param });
            }

            SearchConditionGroup _scg2 = new SearchConditionGroup() { ConditionList = _sc2, ConstraintType = ConstraintType.And };

            _scgList.Add(_scg1);
            _scgList.Add(_scg2);
            return _scgList;
        }

        //
        // GET: /Role/

        public ViewResult Index(int? page)
        {
            logger.Trace("Ioc");

            var pageIndex = (page ?? 1) - 1;
            var pageSize = 5;
            int totalCount;

            Specification<Role> c = SpecificationBuilder.BuildSpecification<Role>(BuildCondition());

            var roles = roleRepository.AllMatching(c, pageIndex, pageSize, "CreatedOn", false, out totalCount);

            var rolesAsIPagedList = new StaticPagedList<Role>(roles, pageIndex + 1, pageSize, totalCount);
            ViewBag.OnePageOfroles = rolesAsIPagedList;

            return View();
        }

        //
        // GET: /Role/Details/5

        public ViewResult Details(System.Guid id)
        {
            return View(roleRepository.Find(id));
        }

        //
        // GET: /Role/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Role/Create

        [HttpPost]
        public ActionResult Create(Role role)
        {
            if (ModelState.IsValid)
            {
                roleRepository.InsertOrUpdate(role);
                roleRepository.Save();
                Success("\u4fdd\u5b58\u6210\u529f\uff01");
                return RedirectToAction("Index");
            }
            else
            {
                Error("\u4fdd\u5b58\u5931\u8d25\uff0c\u8868\u5355\u4e2d\u5b58\u5728\u4e00\u4e9b\u9519\u8bef\uff01");
                return View();
            }
        }

        //
        // GET: /Role/Edit/5

        public ActionResult Edit(System.Guid id)
        {
            return View(roleRepository.Find(id));
        }

        //
        // POST: /Role/Edit/5

        [HttpPost]
        public ActionResult Edit(Role role)
        {
            if (ModelState.IsValid)
            {
                roleRepository.InsertOrUpdate(role, updateAttr);
                roleRepository.Save();
                Success("\u4fee\u6539\u6210\u529f\uff01");
                return RedirectToAction("Index");
            }
            else
            {
                Error("\u4fee\u6539\u5931\u8d25\uff0c\u8868\u5355\u4e2d\u5b58\u5728\u4e00\u4e9b\u9519\u8bef\uff01");
                return View();
            }
        }

        //
        // GET: /Role/Delete/5

        public ActionResult Delete(System.Guid id)
        {
            return View(roleRepository.Find(id));
        }

        //
        // POST: /Role/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(System.Guid id)
        {
            roleRepository.Delete(id);
            roleRepository.Save();
            Success("\u5220\u9664\u6210\u529f\uff01");
            return RedirectToAction("Index");
        }
    }
}

