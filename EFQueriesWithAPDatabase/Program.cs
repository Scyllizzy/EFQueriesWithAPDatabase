using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFQueriesWithAPDatabase
{
    class Program
    {
        static void Main(string[] args)
        {
            APContext db = new APContext();

            // Get all Vendors in California
            List<Vendor> caVwndors = (from v in db.Vendors
                                      where v.VendorState == "CA"
                                      orderby v.VendorName
                                      select v).ToList();

            Console.WriteLine("Vendors in CA*****");
            foreach (Vendor currentVendor in caVwndors)
            {
                Console.WriteLine(currentVendor.VendorName);
            }

            // Retrieve a single object
            Vendor singleVendor = (from v in db.Vendors
                                   where v.VendorName == "IBM"
                                   select v).SingleOrDefault();

            if (singleVendor != null)
            {
                Console.WriteLine(singleVendor.VendorName);
            }
            else
            {
                Console.WriteLine("Vendor not found");
            }

            // Get all Vendors and their Invoices (Join)
            List<Vendor> vendorAndInvoices = (from v in db.Vendors
                                              select v).Include(i => i.Invoices).ToList();

            var vendorAndInvoices2 = (from v in db.Vendors
                                               join i in db.Invoices
                                               on v.VendorID equals i.VendorID
                                               select new 
                                               {
                                                    VendorName = v.VendorName,
                                                    Invoices = v.Invoices
                                               }).ToList();

            // Performs an inner join but we don't want a Vendor for each invoice
            foreach (var vendor in vendorAndInvoices2)
            {
                Console.WriteLine(vendor.VendorName);
                foreach (Invoice inv in vendor.Invoices)
                {
                    Console.WriteLine(inv.InvoiceNumber);
                }
            }


            // Get an object but not all of its columns
            // Get Vendors and only location info (came/ city/ state)
            // SELECT VendorName AS Name, VendorCity AS City, etc...
            List<VendorLoc> vendorLocations = (from v in db.Vendors
                                  select new VendorLoc
                                  {
                                      Name = v.VendorName,
                                      City = v.VendorCity,
                                      State = v.VendorState
                                  }).ToList();

            foreach (VendorLoc venLocation in vendorLocations)
            {
                Console.WriteLine(venLocation.Name);
            }

            // Get the sum of all Invoice totals
            double totalInvoiceTotal = Convert.ToDouble((from inv in db.Invoices
                                        select inv.InvoiceTotal).Sum());

            Console.ReadKey();
        }
    }

    class VendorLoc
    {
        public string Name { get; set; }
        public string City { get; set; }
        public string State { get; set; }
    }
}
