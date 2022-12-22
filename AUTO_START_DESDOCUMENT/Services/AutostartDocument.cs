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
        
        public static void StartDoc()
        {
            ISODBContextDataContext db = new ISODBContextDataContext(connectionString);
            List<CustomJsonAdvanceForm.BoxLayout_RefDoc> tempMAdvanceFormItem = new List<CustomJsonAdvanceForm.BoxLayout_RefDoc>();
            List<TRNMemo> listTrnMemo = new List<TRNMemo>();
            var msttemplete = db.MSTTemplates.Where(x => x.DocumentCode == "DAR-N" && x.IsActive == true).ToList();
            var SelectTemp = msttemplete.FirstOrDefault();
            var trnmemo = db.TRNMemos.Where(x => x.TemplateId == SelectTemp.TemplateId).ToList();
            foreach (var ItemMemo in trnmemo)
            {
                if (ItemMemo.MAdvancveForm.Contains("QS-F-IT-0005"))
                {
                    listTrnMemo.Add(ItemMemo);
                }
            }
        }
    }
}
