using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolfApprove.Model.CustomClass;

namespace AUTO_START_DESDOCUMENT.Services
{
    class GetlineApprove
    {
        public static List<ApprovalDetail> GetLineapprove(ViewEmployee Emp)
        {        
            List<ApprovalDetail> lstapprovalDetails = new List<ApprovalDetail>();

            ApprovalDetail Approve = new ApprovalDetail();
            Approve.sequence = 1;
            Approve.approver = new CustomViewEmployee();
            Approve.approver.EmployeeId = Emp.EmployeeId;
            Approve.approver.EmployeeCode = Emp.EmployeeCode;
            Approve.approver.NameTh = Emp.NameTh;
            Approve.approver.NameEn = Emp.NameEn;
            Approve.approver.PositionNameTh = Emp.PositionNameTh;
            Approve.approver.PositionNameEn = Emp.PositionNameEn;
            Approve.signature_id = 1;
            Approve.approver.IsActive = true;
            lstapprovalDetails.Add(Approve);

            return lstapprovalDetails;
        }
    }
}
