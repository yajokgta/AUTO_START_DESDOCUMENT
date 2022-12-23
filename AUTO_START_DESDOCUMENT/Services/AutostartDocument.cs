using AUTO_START_DESDOCUMENT.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using WolfApprove.Model;
using WolfApprove.Model.CustomClass;
using static WolfApprove.Model.CustomClass.CustomJsonAdvanceForm;

namespace AUTO_START_DESDOCUMENT.Services
{
    
    class AutostartDocument
    {
        public static string connectionString = ConfigurationSettings.AppSettings["connectionString"];
        public static string documentcode = ConfigurationSettings.AppSettings["Ducumentcode"];
        public static string EffectiveDate = ConfigurationSettings.AppSettings["EffectiveDate"];
        public static string Storageperiod = ConfigurationSettings.AppSettings["Storageperiod"];
        public static string EmailCreater = ConfigurationSettings.AppSettings["EmailCreater"];

        public static void StartDoc()
        {
            ISODBContextDataContext db = new ISODBContextDataContext(connectionString);
            if (db.Connection.State == ConnectionState.Open)
            {
                db.Connection.Close();
                db.Connection.Open();
            }
            List<CustomJsonAdvanceForm.BoxLayout_RefDoc> tempMAdvanceFormItem = new List<CustomJsonAdvanceForm.BoxLayout_RefDoc>();
            List<TRNMemo> listTrnMemoDARN = new List<TRNMemo>();
            List<InfomationDARN> ValueInMadvanceList = new List<InfomationDARN>();
            List<TRNMemo> listTrnMemoDest = new List<TRNMemo>();
            var msttemplete = db.MSTTemplates.Where(x => x.DocumentCode == "DAR-N" || x.DocumentCode == "DAR-E" && x.IsActive == true).ToList();
            List<string> SelectTemp = msttemplete.Select(x => x.TemplateId.ToString()).ToList();
            var trnmemo = db.TRNMemos.Where(x => SelectTemp.Contains(x.TemplateId.ToString())).ToList();
            foreach (var ItemMemo in trnmemo)
            {
                if (ItemMemo.MAdvancveForm.Contains("QS-F-IT-0005"))
                {
                    listTrnMemoDARN.Add(ItemMemo);
                }
            }
            foreach (var objlistmemodarn in listTrnMemoDARN)
            {
                InfomationDARN GetValue = new InfomationDARN();
                tempMAdvanceFormItem = Ext.ConvertAdvanceFromToListNew.convertAdvanceFormToListNew(objlistmemodarn.MAdvancveForm);
                foreach (var tempItem in tempMAdvanceFormItem)
                {    
                    if (tempItem.Box_Control_Label == EffectiveDate)
                    {
                        GetValue.EffectiveDate = DateTime.ParseExact(tempItem.Box_Control_Value, "dd/MM/yyyy", null);
                    }
                    if (tempItem.Box_Control_Label == Storageperiod)
                    {
                        GetValue.Storageperiod = tempItem.Box_Control_Value;
                    }
                }
                ValueInMadvanceList.Add(GetValue);
            }
            ValueInMadvanceList = ValueInMadvanceList.OrderBy(x => x.EffectiveDate).ToList();
            var ValueInMadvanceListLast = ValueInMadvanceList.Last();
            int StorageperiodInt = Convert.ToInt32(ValueInMadvanceListLast.Storageperiod);
            /*int StorageperiodInt = 1;*/
            int daysInOneYear = 365;
            int TotalDay = StorageperiodInt * daysInOneYear;
            DateTime dt = DateTime.ParseExact(ValueInMadvanceListLast.EffectiveDate.ToString("dd/MM/yyyy"), "dd/MM/yyyy", null);
            /*DateTime DatimeTotal =  dt.AddYears(-StorageperiodInt);*/
            var TempleteDest = db.MSTTemplates.Where(x => x.DocumentCode == documentcode && x.IsActive == true).ToList();
            var TempleteDestLast = TempleteDest.Last();
            var TrnmemoDest = db.TRNMemos.Where(x => x.TemplateId == TempleteDestLast.TemplateId && x.StatusName.ToLower() == "completed").ToList();
            TrnmemoDest.OrderBy(x=> x.ModifiedDate).ToList();

            List<TRNMemo> ObjTrnmemos = TrnmemoDest.Where(obj =>
            {
                TimeSpan difference = (TimeSpan)(obj.ModifiedDate - dt);
                return difference.TotalDays >= TotalDay;
            }).ToList();

            var emplist = db.ViewEmployees.Where(x => x.Email == EmailCreater).ToList();
            var EmployeeMaster = emplist.FirstOrDefault();
            List<DataListTRNMemo> ListToTable = new List<DataListTRNMemo>();
            var lstCompany = db.MSTCompanies.ToList();
            foreach (var objtrnmemodest in ObjTrnmemos)
            {
                var GetDocumentCodeByTempID = db.MSTTemplates.Where(x => x.TemplateId == objtrnmemodest.TemplateId && x.IsActive == true).ToList().FirstOrDefault();
                DataListTRNMemo GetValueToTable = new DataListTRNMemo();
                GetValueToTable.DocumentCode = GetDocumentCodeByTempID.DocumentCode;
                GetValueToTable.TemplateName = objtrnmemodest.TemplateName;
                GetValueToTable.Department = objtrnmemodest.CDepartmentEn;
                GetValueToTable.DocumentCodeRunning = objtrnmemodest.DocumentNo;
                GetValueToTable.Subject = objtrnmemodest.MemoSubject;
                GetValueToTable.MemoID = objtrnmemodest.MemoId.ToString();
                ListToTable.Add(GetValueToTable);
            }
            var DestinationTemplate = db.MSTTemplates.Where(x => x.DocumentCode == "FM-บันทึกคุณภาพ" && x.IsActive == true).ToList();
            var DestinationTemplateLast = DestinationTemplate.LastOrDefault();
            string strMAdvance = Ext.ReplaceDataProcessCAR(DestinationTemplateLast.AdvanceForm, ListToTable);
            List<ApprovalDetail> LineApprove = GetlineApprove.GetLineapprove(EmployeeMaster);
            TRNMemo objMemo = new TRNMemo();
            objMemo.StatusName = "Wait for Approve";
            objMemo.CreatedDate = DateTime.Now;
            objMemo.CreatedBy = EmployeeMaster.NameEn;
            objMemo.CreatorId = EmployeeMaster.EmployeeId;
            objMemo.RequesterId = EmployeeMaster.EmployeeId;
            objMemo.CNameTh = EmployeeMaster.NameTh;
            objMemo.CNameEn = EmployeeMaster.NameEn;

            objMemo.CPositionId = EmployeeMaster.PositionId;
            objMemo.CPositionTh = EmployeeMaster.PositionNameTh;
            objMemo.CPositionEn = EmployeeMaster.PositionNameEn;
            objMemo.CDepartmentId = EmployeeMaster.DepartmentId;
            objMemo.CDepartmentTh = EmployeeMaster.DepartmentNameTh;
            objMemo.CDepartmentEn = EmployeeMaster.DepartmentNameEn;
            objMemo.RNameTh = EmployeeMaster.NameTh;
            objMemo.RNameEn = EmployeeMaster.NameEn;
            objMemo.RPositionId = EmployeeMaster.PositionId;
            objMemo.RPositionTh = EmployeeMaster.PositionNameTh;
            objMemo.RPositionEn = EmployeeMaster.PositionNameEn;
            objMemo.RDepartmentId = EmployeeMaster.DepartmentId;
            objMemo.RDepartmentTh = EmployeeMaster.DepartmentNameTh;
            objMemo.RDepartmentEn = EmployeeMaster.DepartmentNameEn;

            objMemo.ModifiedDate = DateTime.Now;
            objMemo.ModifiedBy = objMemo.ModifiedBy;
            objMemo.TemplateId = DestinationTemplateLast.TemplateId;
            objMemo.TemplateName = DestinationTemplateLast.TemplateName;
            objMemo.GroupTemplateName = DestinationTemplateLast.GroupTemplateName;
            objMemo.RequestDate = DateTime.Now;
            var CurrentCom = lstCompany.Find(a => a.CompanyId == 1);
            objMemo.CompanyId = 1;
            objMemo.CompanyName = CurrentCom.NameTh;
            objMemo.MemoSubject = DestinationTemplateLast.TemplateName;
            objMemo.MAdvancveForm = strMAdvance;
            objMemo.TAdvanceForm = strMAdvance;
            objMemo.TemplateSubject = DestinationTemplateLast.TemplateSubject;
            objMemo.TemplateDetail = Guid.NewGuid().ToString().Replace("-", "");

            objMemo.ProjectID = 0;
            objMemo.DocumentCode = GenControlRunning.genControlRunning(EmployeeMaster, DestinationTemplateLast.DocumentCode, objMemo, db);
            objMemo.DocumentNo = objMemo.DocumentCode;
            db.TRNMemos.InsertOnSubmit(objMemo);
            db.SubmitChanges();

            foreach (var Approve in LineApprove)
            {
                TRNLineApprove trnLine = new TRNLineApprove();
                trnLine.MemoId = objMemo.MemoId;
                trnLine.Seq = Approve.sequence;
                trnLine.EmployeeId = Approve.approver.EmployeeId;
                trnLine.EmployeeCode = Approve.approver.EmployeeCode;
                trnLine.NameTh = Approve.approver.NameTh;
                trnLine.NameEn = Approve.approver.NameEn;
                trnLine.PositionTH = Approve.approver.PositionNameTh;
                trnLine.PositionEN = Approve.approver.PositionNameEn;
                trnLine.SignatureId = Approve.signature_id;
                trnLine.SignatureTh = Approve.signature_th;
                trnLine.SignatureEn = Approve.signature_en;
                trnLine.IsActive = Approve.approver.IsActive;
                db.TRNLineApproves.InsertOnSubmit(trnLine);
                db.SubmitChanges();
            }
            objMemo.PersonWaitingId = LineApprove.First().approver.EmployeeId;
            objMemo.PersonWaiting = LineApprove.First().approver.NameTh;
            db.SubmitChanges();
            db.Connection.Close();
        }
    }
}
