using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUTO_START_DESDOCUMENT.Services
{
    internal class GenControlRunning
    {
        public static string genControlRunning(ViewEmployee Emp, string DocumentCode, TRNMemo objTRNMemo, ISODBContextDataContext db)
        {
            string TempCode = DocumentCode;
            String sPrefixDocNo = $"{TempCode}-{DateTime.Now.Year.ToString()}-";
            int iRunning = 1;
            List<TRNMemo> temp = db.TRNMemos.Where(a => a.DocumentNo.ToUpper().Contains(sPrefixDocNo.ToUpper())).ToList();
            if (temp.Count > 0)
            {
                String sLastDocumentNo = temp.OrderBy(a => a.DocumentNo).Last().DocumentNo;
                if (!String.IsNullOrEmpty(sLastDocumentNo))
                {
                    List<String> list_LastDocumentNo = sLastDocumentNo.Split('-').ToList();
                    //Arm Edit 2020-05-18 Bug If Prefix have '-' will no running because list_LastDocumentNo.Count > 3
                    //if (list_LastDocumentNo.Count == 3)
                    //{
                    //    iRunning = Ext.checkDataIntIsNull(list_LastDocumentNo[2]) + 1;
                    //}
                    if (list_LastDocumentNo.Count >= 3)
                    {
                        iRunning = checkDataIntIsNull(list_LastDocumentNo[list_LastDocumentNo.Count - 1]) + 1;
                    }
                }
            }
            String sDocumentNo = $"{sPrefixDocNo}{iRunning.ToString().PadLeft(6, '0')}";

            // new code 2021-03-10
            // New Doc NO Master Data
            try
            {

                var mstMasterDataList = db.MSTMasterDatas.Where(a => a.MasterType == "DocNo").ToList();

                if (mstMasterDataList != null)
                    if (mstMasterDataList.Count() > 0)
                    {
                        var getCompany = db.MSTCompanies.Where(a => a.CompanyId == objTRNMemo.CompanyId).ToList();
                        var getDepartment = db.MSTDepartments.Where(a => a.DepartmentId == Emp.DepartmentId).ToList();
                        var getDivision = db.MSTDivisions.Where(a => a.DivisionId == Emp.DivisionId).ToList();

                        string CompanyCode = "";
                        string DepartmentCode = "";
                        string DivisionCode = "";
                        if (getCompany != null)
                            if (!string.IsNullOrWhiteSpace(getCompany.First().CompanyCode)) CompanyCode = getCompany.First().CompanyCode;
                        if (DepartmentCode != null)
                            if (!string.IsNullOrWhiteSpace(getDepartment.First().DepartmentCode)) DepartmentCode = getDepartment.First().DepartmentCode;
                        if (DivisionCode != null)
                        {
                            if (getDivision.Count > 0)
                                if (!string.IsNullOrWhiteSpace(getDivision.First().DivisionCode)) DivisionCode = getDivision.First().DivisionCode;
                        }
                        foreach (var getMaster in mstMasterDataList)
                        {
                            if (!string.IsNullOrWhiteSpace(getMaster.Value2))
                            {
                                var Tid_array = getMaster.Value2.Split('|');
                                string FixDoc = getMaster.Value1;
                                if (Tid_array.Count() > 0)
                                {
                                    if (Tid_array.Contains(objTRNMemo.TemplateId.ToString()))
                                    {
                                        sDocumentNo = DocNoGenerate(FixDoc, TempCode, CompanyCode, DepartmentCode, DivisionCode, db);
                                    }
                                }
                            }
                            else
                            {
                                string FixDoc = getMaster.Value1;
                                sDocumentNo = DocNoGenerate(FixDoc, TempCode, CompanyCode, DepartmentCode, DivisionCode, db);
                            }
                        }

                    }




            }
            catch (Exception ex) { }



            return sDocumentNo;
        }
        public static int checkDataIntIsNull(object Input)
        {
            int Results = 0;
            if (Input != null)
                int.TryParse(Input.ToString().Replace(",", ""), out Results);

            return Results;
        }
        public static string DocNoGenerate(string FixDoc, string DocCode, string CCode, string DCode, string DSCode, ISODBContextDataContext db)
        {
            string sDocumentNo = "";
            int iRunning;
            if (!string.IsNullOrWhiteSpace(FixDoc))
            {
                string y4 = DateTime.Now.ToString("yyyy");
                string y2 = DateTime.Now.ToString("yy");
                string CompanyCode = CCode;
                string DepartmentCode = DCode;
                string DivisionCode = DSCode;
                string FixCode = FixDoc;
                FixCode = FixCode.Replace("[CompanyCode]", CompanyCode);
                FixCode = FixCode.Replace("[DepartmentCode]", DepartmentCode);
                FixCode = FixCode.Replace("[DocumentCode]", DocCode);
                FixCode = FixCode.Replace("[DivisionCode]", DivisionCode);

                FixCode = FixCode.Replace("[YYYY]", y4);
                FixCode = FixCode.Replace("[YY]", y2);
                sDocumentNo = FixCode;
                List<TRNMemo> tempfixDoc = db.TRNMemos.Where(a => a.DocumentNo.ToUpper().Contains(sDocumentNo.ToUpper())).ToList();


                List<TRNMemo> tempfixDocByYear = db.TRNMemos.ToList();

                tempfixDocByYear = tempfixDocByYear.FindAll(a => a.DocumentNo != ("Auto Generate") & Convert.ToDateTime(a.RequestDate).Year.ToString().Equals(y4)).ToList();
                //if (tempfixDoc.Count > 0)
                //{
                //    String sLastDocumentNofix = tempfixDoc.OrderBy(a => a.DocumentNo).Last().DocumentNo;
                //    if (!String.IsNullOrEmpty(sLastDocumentNofix))
                //    {
                //        List<String> list_LastDocumentNofix = sLastDocumentNofix.Split('-').ToList();
                //        //Arm Edit 2020-05-18 Bug If Prefix have '-' will no running because list_LastDocumentNo.Count > 3

                //        if (list_LastDocumentNofix.Count >= 3)
                //        {
                //            iRunning = Ext.checkDataIntIsNull(list_LastDocumentNofix[list_LastDocumentNofix.Count - 1]) + 1;
                //            sDocumentNo = $"{sDocumentNo}-{iRunning.ToString().PadLeft(6, '0')}";
                //        }
                //    }
                //}
                if (tempfixDocByYear.Count > 0)
                {
                    tempfixDocByYear = tempfixDocByYear.OrderByDescending(a => a.MemoId).ToList();

                    String sLastDocumentNofix = tempfixDocByYear.First().DocumentNo;
                    if (!String.IsNullOrEmpty(sLastDocumentNofix))
                    {
                        List<String> list_LastDocumentNofix = sLastDocumentNofix.Split('-').ToList();
                        //Arm Edit 2020-05-18 Bug If Prefix have '-' will no running because list_LastDocumentNo.Count > 3

                        if (list_LastDocumentNofix.Count >= 3)
                        {
                            iRunning = checkDataIntIsNull(list_LastDocumentNofix[list_LastDocumentNofix.Count - 1]) + 1;
                            sDocumentNo = $"{sDocumentNo}-{iRunning.ToString().PadLeft(6, '0')}";
                        }
                    }
                }
                else
                {
                    sDocumentNo = $"{sDocumentNo}-{1.ToString().PadLeft(6, '0')}";

                }
            }
            return sDocumentNo;

        }
    }
}
