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

        public InvoicesController(AIDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<Invoice> GetInvoices()
        {
            return _context.Invoices.ToList();
        }

        //Get total cost for all items
        [HttpGet("{id}/totalCost")]
        public decimal GetInvoiceTotalCost(int id)
        {
            return _context.Invoices.Find(id).TotalValue;

        }

        [HttpPut]
        public Invoice CreateInvoice(InvoiceInput input)
        {
            var invoice = new Invoice();
            invoice.Description = input.Description;
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
            _context.Add(lineItem);

            //Get totalvalue by adding from ef database
            decimal lineItemCost = lineItem.Cost * lineItem.Quantity;
            _context.Invoices.Find(id).TotalValue += lineItemCost;
            _context.SaveChanges();
            return lineItem;
        }
    }

    public class InvoiceInput
    {
        public string Description { get; set; }
    }

    public class LineItemInput
    {
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal Cost { get; set; }
    }
}