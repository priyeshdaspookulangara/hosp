using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UniCareERP.Domain.Enums;

namespace UniCareERP.Application.DTOs.Finance
{
    public class InvoiceDto
    {
        public Guid Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;

        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty; // To be populated
        public string PatientCode { get; set; } = string.Empty; // To be populated

        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal AmountDue => TotalAmount - AmountPaid;

        public InvoiceStatus Status { get; set; }
        public string StatusText => Status.ToString();

        public List<InvoiceItemDto> InvoiceItems { get; set; } = new List<InvoiceItemDto>();
    }

    public class CreateInvoiceDto
    {
        [Required]
        public Guid PatientId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime InvoiceDate { get; set; } = DateTime.Today;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; } = DateTime.Today.AddDays(30);

        // Optional: Reference to the appointment that generated this invoice
        public Guid? SourceAppointmentId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Invoice must have at least one line item.")]
        public List<CreateInvoiceItemDto> InvoiceItems { get; set; } = new List<CreateInvoiceItemDto>();
    }
}
