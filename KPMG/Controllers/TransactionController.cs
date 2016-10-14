using KPMG.Models;
using KPMG.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Net;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace KPMG.Controllers
{
    public class TransactionController : Controller
    {
        private KPMGDatabaseEntities context = new KPMGDatabaseEntities();
        private int PageSize = 50;

        public ActionResult Upload()
        {
            return View(new Upload());
        }

        [HttpPost]
        public ActionResult Upload(Upload model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.FileData.ContentLength > 0)
            {
                List<Transaction> listTrans = new List<Transaction>();

                using (StreamReader sr = new StreamReader(model.FileData.InputStream))
                {
                    while (!sr.EndOfStream)
                    {
                        string record = sr.ReadLine();

                        try
                        {
                            string[] values = record.Split(',');
                            Transaction tran = new Transaction();
                            tran.Account = values[0];
                            tran.Description = values[1];
                            tran.CurrencyCode = values[2];
                            decimal conversion;
                            if (!decimal.TryParse(values[3], out conversion))
                            {
                                model.IgnoredLines.Add("Not a valid amount", record);
                                continue;
                            }
                            tran.Amount = conversion;
                            listTrans.Add(tran);
                        }
                        catch (Exception ex)
                        {
                            model.IgnoredLines.Add(ex.Message, record);
                        }
                    }


                    context.Transactions.AddRange(listTrans);
                    // Use of EF to retrieve validation errors
                    var errors = context.GetValidationErrors();
                    foreach (var e in errors)
                    {
                        model.IgnoredLines.Add(e.ValidationErrors.First().ErrorMessage, e.Entry.Entity.ToString());
                        listTrans.Remove((Transaction)e.Entry.Entity);
                    }

                    DataTable dt = Utility.ConvertToDataTable<Transaction>(listTrans);

                    using (SqlConnection connection = new SqlConnection(context.Database.Connection.ConnectionString))
                    {
                        connection.Open();

                        // EntityFramework insert operation runs slow for a bulk insert
                        // better to use SqlBulkCopy class just for the import
                        using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(connection))
                        {
                            foreach (DataColumn dc in dt.Columns)
                                sqlBulkCopy.ColumnMappings.Add(dc.ColumnName, dc.ColumnName);

                            sqlBulkCopy.DestinationTableName = string.Format("[{0}s]", dt.TableName);

                            // Error during this insert should be managed by a customized error page
                            // but error managing was not part of the task
                            sqlBulkCopy.WriteToServer(dt);
                        }
                    }

                    model.ImportedLines = dt.Rows.Count;

                }
            }

            return View(model);           
        }

        public ActionResult Index(int? page)
        {
            int pageNumber = (page ?? 1);
            return View(context.Transactions.OrderByDescending(x => x.ID).ToPagedList(pageNumber, PageSize));
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction tran = context.Transactions.Find(id);
            if (tran == null)
            {
                return HttpNotFound();
            }
            return View(tran);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction tran = context.Transactions.Find(id);
            if (tran == null)
            {
                return HttpNotFound();
            }
            return View(tran);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Transaction tran)
        {
            if (ModelState.IsValid)
            {
                context.Entry(tran).State = EntityState.Modified;
                int a = context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tran);
        }

        // GET: Movies/Delete/5 
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction tran = context.Transactions.Find(id);
            if (tran == null)
            {
                return HttpNotFound();
            }
            return View(tran);
        }

        // POST: Movies/Delete/5 
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction tran = context.Transactions.Find(id);
            context.Transactions.Remove(tran);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                context.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}