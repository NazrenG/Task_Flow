using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Flow.Business.DTOs
{
    public class CompanyDetailDto
    {
        public string CompanyName{ get; set; }
        public string OwnerUsername{ get; set; }
        public string OwnerEmail{ get; set; }
        public string OwnerFullname {  get; set; }
        public string Email {  get; set; }
        public string Address {  get; set; }
        public bool IsPaid { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedDate {  get; set; }
        public int CompanyId { get; set; }

    }
}
