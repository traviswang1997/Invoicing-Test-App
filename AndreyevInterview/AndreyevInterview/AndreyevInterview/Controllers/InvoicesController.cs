using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace AndreyevInterview.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InvoicesController : ControllerBase
    {
        private readonly AIDbContext _context;
        List<decimal> total = new List<decimal>() { 0, 0 };

        public InvoicesController(AIDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<Invoice> GetInvoices()
        {
            return _context.Invoices.ToList(); //update endpoint with number of items, total values, total billable cost.
        }

        //return two types of cost in a list
        [HttpGet("{id}/cost")]
        public IEnumerable<decimal> GetInvoiceTotal(int id)
        {
            total[0] = _context.Invoices.Find(id).TotalValue;
            total[1] = _context.Invoices.Find(id).TotalBillable;
            return total;
        }

        //Get total cost for all items
        [HttpGet("{id}/totalCost")]
        public decimal GetInvoiceTotalCost(int id)
        {
            return _context.Invoices.Find(id).TotalValue;

        }

        //get total billable cost
        [HttpGet("{id}/totalBillable")]
        public decimal GetInvoiceTotalBillable(int id)
        {
            var lineItems = this.GetInvoiceLineItems(id);
            decimal totalBillableCost = 0;
            foreach (LineItem lineItem in lineItems)
            {
                if (lineItem.Billable == true)
                {
                    totalBillableCost += lineItem.Quantity * lineItem.Cost;
                }
            }
            return totalBillableCost;
        }

        [HttpPut]
        public Invoice CreateInvoice(InvoiceInput input)
        {
            var invoice = new Invoice();
            invoice.Description = input.Description;
            invoice.TotalValue = input.TotalValue;
            invoice.TotalBillable = input.TotalBillable;
            invoice.NumberOfItem = 0;
            _context.Add(invoice);
            _context.SaveChanges();
            return invoice;
        }

        [HttpGet("{id}")]
        public IEnumerable<LineItem> GetInvoiceLineItems(int id)
        {
            return _context.LineItems.Where(x => x.InvoiceId == id).ToList();
        }

        [HttpPost("{id}")]
        public LineItem AddLineItemToInvoice(int id, LineItemInput input)
        {
            var lineItem = new LineItem();
            lineItem.InvoiceId = id;
            lineItem.Description = input.Description;
            lineItem.Quantity = input.Quantity;
            lineItem.Cost = input.Cost;
            lineItem.Billable = input.Billable;

            _context.Add(lineItem);

            //Get totalvalue by adding from ef database
            decimal lineItemCost = lineItem.Cost * lineItem.Quantity;
            _context.Invoices.Find(id).TotalValue += lineItemCost;
            _context.Invoices.Find(id).TotalBillable += lineItemCost; //update billable item value
            _context.Invoices.Find(id).NumberOfItem++; //update number of items in invoice
            _context.SaveChanges();
            return lineItem;
        }

        //add billable to ef
        [HttpPost("{invoiceId}/{id}")]
        public Boolean ChangeBillable(int invoiceId, int id, LineItemInput input)
        {
            var lineItems = this.GetInvoiceLineItems(invoiceId);
            foreach (LineItem lineItem in lineItems)
            {
                if (lineItem.Id == id)
                {
                    _context.LineItems.Find(id).Billable = input.Billable;
                    decimal cost = lineItem.Cost * lineItem.Quantity;
                    if (input.Billable == false)
                    {
                        _context.Invoices.Find(invoiceId).TotalBillable -= cost;
                    }
                    else
                    {
                        _context.Invoices.Find(invoiceId).TotalBillable += cost;
                    }
                    _context.SaveChanges();
                    return _context.LineItems.Find(id).Billable;
                }
            }

            return false;
        }
    }

    public class InvoiceInput
    {
        public string Description { get; set; }
        public decimal TotalValue { get; set; }
        public decimal TotalBillable { get; set; }
        public decimal NumberOfItem { get; set; }
    }

    public class LineItemInput
    {
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal Cost { get; set; }
        public bool Billable { get; set; }//update billable state
    }
}