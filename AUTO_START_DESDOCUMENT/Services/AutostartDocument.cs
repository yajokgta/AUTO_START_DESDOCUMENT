using AUTO_START_DESDOCUMENT.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolfApprove.Model.CustomClass;

namespace AUTO_START_DESDOCUMENT.Services
{
    
    class AutostartDocument
    {
        public static string connectionString = ConfigurationSettings.AppSettings["connectionString"];
        public static string documentcode = ConfigurationSettings.AppSettings["Ducumentcode"];
        public static string EffectiveDate = ConfigurationSettings.AppSettings["EffectiveDate"];
        public static string Storageperiod = ConfigurationSettings.AppSettings["Storageperiod"];

        public static void StartDoc()
        {
            ISODBContextDataContext db = new ISODBContextDataContext(connectionString);
            List<CustomJsonAdvanceForm.BoxLayout_RefDoc> tempMAdvanceFormItem = new List<CustomJsonAdvanceForm.BoxLayout_RefDoc>();
            List<TRNMemo> listTrnMemoDARN = new List<TRNMemo>();
            List<InfomationDARN> ValueInMadvanceList = new List<InfomationDARN>();
            List<TRNMemo> listTrnMemoDest = new List<TRNMemo>();
            var msttemplete = db.MSTTemplates.Where(x => x.DocumentCode == "DAR-N" && x.IsActive == true).ToList();
            var SelectTemp = msttemplete.FirstOrDefault();
            var trnmemo = db.TRNMemos.Where(x => x.TemplateId == SelectTemp.TemplateId && x.StatusName.ToLower() == "completed").ToList();
            foreach (var ItemMemo in trnmemo)
            {
                if (ItemMemo.MAdvancveForm.Contains(documentcode))
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
                        GetValue.EffectiveDate = Convert.ToDateTime(tempItem.Box_Control_Value);
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
            DateTime dt = Convert.ToDateTime(ValueInMadvanceListLast.EffectiveDate);
            var TempleteDest = db.MSTTemplates.Where(x => x.DocumentCode == "QS-F-IT-005" && x.IsActive == true);
            var TempleteDestLast = TempleteDest.Last();
            var TrnmemoDest = db.TRNMemos.Where(x => x.TemplateId == TempleteDestLast.TemplateId && x.StatusName.ToLower() == "completed" && x.ModifiedDate <= dt.AddYears(-StorageperiodInt)).ToList();
            foreach (var objtrnmemodest in TrnmemoDest)
            {
                
            }
        }
    }
}
