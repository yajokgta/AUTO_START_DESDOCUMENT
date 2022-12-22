using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolfApprove.API2.Controllers.Utils;
using WolfApprove.Model.CustomClass;

namespace AUTO_START_DESDOCUMENT.Services
{
    class Ext
    {
        public class ConvertAdvanceFromToListNew
        {
            public static String GetTextControlType(String sType)
            {
                sType = System.Text.RegularExpressions.Regex.Replace(sType, "[A-Z]", " $0");
                return sType.Trim();
            }

            public class jsonData
            {
                public string value { get; set; }
            }
            public static List<CustomJsonAdvanceForm.BoxLayout_RefDoc> convertAdvanceFormToListNew(string advanceForm)
            {
                List<CustomJsonAdvanceForm.BoxLayout_RefDoc> listBoxLayout = new List<CustomJsonAdvanceForm.BoxLayout_RefDoc>();
                try
                {
                    if (advanceForm != null)
                    {
                        List<JObject> jsonAdvanceFormList = new List<JObject>();
                        JObject jsonAdvanceForm = JsonUtils.createJsonObject(advanceForm);
                        if (jsonAdvanceForm.ContainsKey("items"))
                        {
                            jsonAdvanceFormList.Add(jsonAdvanceForm);
                        }

                        if (jsonAdvanceFormList != null)
                        {
                            int iRunning = 1;

                            foreach (JObject json in jsonAdvanceFormList)
                            {
                                JArray itemsArray = (JArray)json["items"];
                                foreach (JObject jItems in itemsArray)
                                {
                                    JArray jLayoutArray = (JArray)jItems["layout"];

                                    CustomJsonAdvanceForm.BoxLayout_RefDoc iBoxLayout = new CustomJsonAdvanceForm.BoxLayout_RefDoc();
                                    if (jLayoutArray.Count >= 1)
                                    {
                                        iBoxLayout = new CustomJsonAdvanceForm.BoxLayout_RefDoc();
                                        JObject jTemplate = (JObject)jLayoutArray[0]["template"];
                                        if (jTemplate != null)
                                        {
                                            if (jTemplate.Count > 0)
                                            {

                                                iBoxLayout.Box_ID = iRunning.ToString(); iRunning++;
                                                iBoxLayout.Box_Column = "2";
                                                iBoxLayout.Box_ControlType = CustomJsonAdvanceForm.GetControlTypeByJSONKey((String)jTemplate["type"]);
                                                //iBoxLayout.Box_Control_ControlTypeCss = GetCssIconControlType(iBoxLayout.Box_ControlType.ToString());
                                                iBoxLayout.Box_Control_ControlTypeText = GetTextControlType(iBoxLayout.Box_ControlType.ToString());

                                                iBoxLayout.Box_Control_Label = (String)jTemplate["label"];
                                                iBoxLayout.Box_Control_AltLabel = (String)jTemplate["alter"];
                                                iBoxLayout.Box_Control_IsText = (String)jTemplate["istext"];
                                                iBoxLayout.Box_Control_TextValue = (String)jTemplate["textvalue"];
                                                if (jTemplate["description"] != null)
                                                    iBoxLayout.Box_Control_Description = (String)jTemplate["description"];

                                                if (jTemplate["formula"] != null)
                                                    iBoxLayout.Box_Control_Formula = (String)jTemplate["formula"];
                                                bool CheckAttribute = false;
                                                foreach (var valueInJtemp in jTemplate)
                                                {
                                                    if (valueInJtemp.Key.ToString() == "attribute")
                                                    {
                                                        CheckAttribute = true;
                                                    }
                                                }
                                                if (CheckAttribute)
                                                {
                                                    if (!String.IsNullOrEmpty(jTemplate["attribute"].ToString()))
                                                    {
                                                        JObject jAttribute = (JObject)jTemplate["attribute"];
                                                        if (jAttribute != null)
                                                        {
                                                            if (String.IsNullOrEmpty(iBoxLayout.Box_Control_Description) && jAttribute["description"] != null)
                                                                iBoxLayout.Box_Control_Description = (String)jAttribute["description"];
                                                            iBoxLayout.Box_Control_DefaultValue = (String)jAttribute["default"];
                                                            iBoxLayout.Box_Control_MaxLength = (String)jAttribute["length"];
                                                            iBoxLayout.Box_Control_Required = (String)jAttribute["require"];

                                                            iBoxLayout.Box_Control_Min = (String)jAttribute["min"];
                                                            iBoxLayout.Box_Control_Max = (String)jAttribute["max"];
                                                            iBoxLayout.Box_Control_Comma = (String)jAttribute["useComma"];

                                                            iBoxLayout.Box_Control_Inline = (String)jAttribute["multipleLine"];
                                                            iBoxLayout.Box_Control_Summary = (String)jAttribute["summary"];

                                                            iBoxLayout.Box_Control_Decimal = (String)jAttribute["decimal"];
                                                            iBoxLayout.Box_Control_Symbol = (String)jAttribute["symbol"];
                                                            iBoxLayout.Box_Control_SymbolPosition = (String)jAttribute["symbolPosition"];
                                                            iBoxLayout.Box_Control_ValueAlign = (String)jAttribute["align"];

                                                            JObject objDate = (JObject)jAttribute["date"];
                                                            if (objDate != null)
                                                            {
                                                                iBoxLayout.Box_Control_Symbol = (String)objDate["symbol"];
                                                            }

                                                            if (jAttribute["items"] != null)
                                                            {
                                                                if (jAttribute["items"].HasValues)
                                                                {
                                                                    JArray itemsItem = (JArray)jAttribute["items"];
                                                                    if (itemsItem != null)
                                                                    {
                                                                        foreach (JObject kItem in itemsItem)
                                                                        {
                                                                            if (!String.IsNullOrEmpty(iBoxLayout.Box_Control_Item))
                                                                                iBoxLayout.Box_Control_Item += ",";
                                                                            iBoxLayout.Box_Control_Item += $"{kItem["item"]}";
                                                                        }
                                                                    }
                                                                }
                                                            }

                                                            JArray itemsColumn = (JArray)jAttribute["column"];
                                                            if (itemsColumn != null)
                                                            {

                                                                JObject jDataColumn = (JObject)jLayoutArray[0]["data"];

                                                                if ((String)jTemplate["type"].ToString() != "an")
                                                                {
                                                                    #region | Column |

                                                                    if (itemsColumn.Count > 0)
                                                                    {
                                                                        iBoxLayout.Box_Control_Column = new List<CustomJsonAdvanceForm.ColumnTable_RefDoc>();
                                                                        iBoxLayout.Box_Control_Table = new List<CustomJsonAdvanceForm.ColumnTable_RefDoc>();
                                                                        int columnCount = 0;
                                                                        foreach (JObject kItem in itemsColumn)
                                                                        {

                                                                            CustomJsonAdvanceForm.ColumnTable_RefDoc iColumnTable = new CustomJsonAdvanceForm.ColumnTable_RefDoc();

                                                                            iColumnTable.Column_Label = (String)kItem["label"];
                                                                            iColumnTable.Column_AltLabel = (String)kItem["alter"];
                                                                            if (!String.IsNullOrEmpty(kItem["control"]["template"].ToString()))
                                                                            {
                                                                                JObject ktemplate = (JObject)kItem["control"]["template"];
                                                                                if (ktemplate != null)
                                                                                {
                                                                                    iColumnTable.Column_ControlType = CustomJsonAdvanceForm.GetControlTypeByJSONKey((String)ktemplate["type"]);
                                                                                    //iColumnTable.Column_ControlTypeCss = GetCssIconControlType(iColumnTable.Column_ControlType.ToString());
                                                                                    iColumnTable.Column_ControlTypeText = GetTextControlType(iColumnTable.Column_ControlType.ToString());
                                                                                    if (ktemplate["description"] != null)
                                                                                        iColumnTable.Column_Description = (String)ktemplate["description"];

                                                                                    if (!String.IsNullOrEmpty(ktemplate["attribute"].ToString()))
                                                                                    {
                                                                                        JObject kAttribute = (JObject)ktemplate["attribute"];
                                                                                        if (kAttribute != null)
                                                                                        {
                                                                                            if (String.IsNullOrEmpty(iColumnTable.Column_Description))
                                                                                                iColumnTable.Column_Description = (String)kAttribute["description"];
                                                                                            iColumnTable.Column_DefaultValue = (String)kAttribute["default"];

                                                                                            if (kAttribute["items"] != null)
                                                                                            {
                                                                                                if (kAttribute["items"].HasValues)
                                                                                                {
                                                                                                    JArray itemsItemInColumn = (JArray)kAttribute["items"];
                                                                                                    if (itemsItemInColumn != null)
                                                                                                    {
                                                                                                        foreach (JObject lItem in itemsItemInColumn)
                                                                                                        {
                                                                                                            if (!String.IsNullOrEmpty(iColumnTable.Column_Item))
                                                                                                                iColumnTable.Column_Item += ",";
                                                                                                            iColumnTable.Column_Item += $"{lItem["item"]}";
                                                                                                        }

                                                                                                    }
                                                                                                }
                                                                                            }

                                                                                        }
                                                                                    }
                                                                                }
                                                                            }

                                                                            //JValue tableRow = (JValue)jDataColumn["row"];
                                                                            //if (tableRow.Value != null)
                                                                            //{
                                                                            JArray tableRowArr = jDataColumn["row"].Count() > 0 ?
                                                                            (JArray)jDataColumn["row"] : null;
                                                                            if (tableRowArr != null)
                                                                            {
                                                                                iBoxLayout.Box_Control_TableRowCount = tableRowArr.Count;
                                                                                int rowCount = 0;
                                                                                foreach (JArray dataRow in tableRowArr)
                                                                                {
                                                                                    CustomJsonAdvanceForm.ColumnTable_RefDoc iTable = new CustomJsonAdvanceForm.ColumnTable_RefDoc();
                                                                                    iTable.Box_Control_RowIndex = rowCount;
                                                                                    iTable.Table_Name = iBoxLayout.Box_Control_Label;
                                                                                    iTable.Column_Label = (String)kItem["label"];
                                                                                    iTable.Column_AltLabel = (String)kItem["alter"];
                                                                                    iTable.Box_Control_Value = (String)dataRow[columnCount]["value"];
                                                                                    iTable.Column_ControlType = iColumnTable.Column_ControlType;
                                                                                    iTable.Column_ControlTypeCss = iColumnTable.Column_ControlTypeCss;
                                                                                    iTable.Column_ControlTypeText = iColumnTable.Column_ControlTypeText;
                                                                                    JArray DataItemValue = (JArray)dataRow[columnCount]["item"];
                                                                                    JArray DataItem = (JArray)kItem["control"]["template"]["attribute"]["items"];
                                                                                    if (DataItemValue != null)
                                                                                    {
                                                                                        foreach (JValue kRowItem in DataItemValue)
                                                                                        {
                                                                                            if (!String.IsNullOrEmpty(iTable.Box_Control_ItemValue))
                                                                                                iTable.Box_Control_ItemValue += ",";
                                                                                            iTable.Box_Control_ItemValue += $"{kRowItem}";

                                                                                        }
                                                                                    }
                                                                                    if (DataItem != null)
                                                                                    {
                                                                                        string[] selectedValue = (string[])null;
                                                                                        if (!string.IsNullOrEmpty(iTable.Box_Control_ItemValue))
                                                                                        {
                                                                                            selectedValue = iTable.Box_Control_ItemValue.Split(',');
                                                                                        }
                                                                                        int i = 0;
                                                                                        foreach (JObject kRowItem in DataItem)
                                                                                        {
                                                                                            if (!String.IsNullOrEmpty(iTable.Column_Item))
                                                                                                iTable.Column_Item += ",";
                                                                                            iTable.Column_Item += $"{kRowItem["item"]}";

                                                                                            if (selectedValue != null && selectedValue[i].ToUpper() == "Y")
                                                                                            {
                                                                                                if (!String.IsNullOrEmpty(iTable.Box_Control_Value))
                                                                                                    iTable.Box_Control_Value += ",";
                                                                                                iTable.Box_Control_Value += $"{kRowItem["item"]}";
                                                                                            }
                                                                                            i++;
                                                                                        }
                                                                                    }
                                                                                    iBoxLayout.Box_Control_Table.Add(iTable);
                                                                                    rowCount++;
                                                                                }
                                                                            }
                                                                            iColumnTable.Box_ID = iBoxLayout.Box_ID;
                                                                            iBoxLayout.Box_Control_Column.Add(iColumnTable);
                                                                            columnCount++;
                                                                        }
                                                                        //}
                                                                    }

                                                                    #endregion
                                                                }

                                                            }

                                                        }
                                                    }
                                                }

                                            }
                                        }

                                        JObject jData = (JObject)jLayoutArray[0]["data"];
                                        if (jData != null)
                                        {
                                            iBoxLayout.Box_Control_Value = (String)jData["value"];
                                            JArray DataItem = (JArray)jData["item"];
                                            if (DataItem != null)
                                            {
                                                string[] selectedValue = (string[])null;
                                                if (!string.IsNullOrEmpty(iBoxLayout.Box_Control_Item))
                                                {
                                                    selectedValue = iBoxLayout.Box_Control_Item.Split(',');
                                                }
                                                int i = 0;
                                                foreach (JValue kItem in DataItem)
                                                {
                                                    if (!String.IsNullOrEmpty(iBoxLayout.Box_Control_ItemValue))
                                                        iBoxLayout.Box_Control_ItemValue += ",";
                                                    iBoxLayout.Box_Control_ItemValue += $"{kItem}";

                                                    if (selectedValue != null && $"{kItem}".ToUpper() == "Y")
                                                    {
                                                        if (!String.IsNullOrEmpty(iBoxLayout.Box_Control_Value))
                                                            iBoxLayout.Box_Control_Value += ",";
                                                        iBoxLayout.Box_Control_Value += selectedValue[i];
                                                    }
                                                    i++;
                                                }
                                            }
                                        }

                                        listBoxLayout.Add(iBoxLayout);
                                    }

                                    if (jLayoutArray.Count == 2)
                                    {
                                        iBoxLayout = new CustomJsonAdvanceForm.BoxLayout_RefDoc();
                                        iBoxLayout.Box_Column = "1";

                                        JObject jTemplate = (JObject)jLayoutArray[1]["template"];
                                        if (jTemplate != null)
                                        {
                                            iBoxLayout.Box_ID = iRunning.ToString(); iRunning++;
                                            iBoxLayout.Box_Column = "1";
                                            iBoxLayout.Box_ControlType = CustomJsonAdvanceForm.GetControlTypeByJSONKey((String)jTemplate["type"]);
                                            //iBoxLayout.Box_Control_ControlTypeCss = GetCssIconControlType(iBoxLayout.Box_ControlType.ToString());
                                            iBoxLayout.Box_Control_ControlTypeText = GetTextControlType(iBoxLayout.Box_ControlType.ToString());

                                            iBoxLayout.Box_Control_Label = (String)jTemplate["label"];
                                            iBoxLayout.Box_Control_AltLabel = (String)jTemplate["alter"];
                                            iBoxLayout.Box_Control_IsText = (String)jTemplate["istext"];
                                            iBoxLayout.Box_Control_TextValue = (String)jTemplate["textvalue"];
                                            if (jTemplate["description"] != null)
                                                iBoxLayout.Box_Control_Description = (String)jTemplate["description"];

                                            if (jTemplate["formula"] != null)
                                                iBoxLayout.Box_Control_Formula = (String)jTemplate["formula"];

                                            if (!String.IsNullOrEmpty(jTemplate["attribute"].ToString()))
                                            {
                                                JObject jAttribute = (JObject)jTemplate["attribute"];
                                                if (jAttribute != null)
                                                {
                                                    if (String.IsNullOrEmpty(iBoxLayout.Box_Control_Description) && jAttribute["description"] != null)
                                                        iBoxLayout.Box_Control_Description = (String)jAttribute["description"];
                                                    iBoxLayout.Box_Control_DefaultValue = (String)jAttribute["default"];
                                                    iBoxLayout.Box_Control_MaxLength = (String)jAttribute["length"];
                                                    iBoxLayout.Box_Control_Required = (String)jAttribute["require"];

                                                    iBoxLayout.Box_Control_Min = (String)jAttribute["min"];
                                                    iBoxLayout.Box_Control_Max = (String)jAttribute["max"];
                                                    iBoxLayout.Box_Control_Comma = (String)jAttribute["useComma"];

                                                    iBoxLayout.Box_Control_Inline = (String)jAttribute["multipleLine"];
                                                    iBoxLayout.Box_Control_Summary = (String)jAttribute["summary"];

                                                    iBoxLayout.Box_Control_Decimal = (String)jAttribute["decimal"];
                                                    iBoxLayout.Box_Control_Symbol = (String)jAttribute["symbol"];
                                                    iBoxLayout.Box_Control_SymbolPosition = (String)jAttribute["symbolPosition"];
                                                    iBoxLayout.Box_Control_ValueAlign = (String)jAttribute["align"];

                                                    if (jAttribute["items"] != null)
                                                    {
                                                        if (jAttribute["items"].HasValues)
                                                        {
                                                            JArray itemsItem = (JArray)jAttribute["items"];
                                                            if (itemsItem != null)
                                                            {
                                                                foreach (JObject kItem in itemsItem)
                                                                {
                                                                    if (!String.IsNullOrEmpty(iBoxLayout.Box_Control_Item))
                                                                        iBoxLayout.Box_Control_Item += ",";
                                                                    iBoxLayout.Box_Control_Item += $"{kItem["item"]}";
                                                                }
                                                            }
                                                        }
                                                    }

                                                    JArray itemsColumn = (JArray)jAttribute["column"];
                                                    if (itemsColumn != null)
                                                    {
                                                        #region | Column |

                                                        if (itemsColumn.Count > 0)
                                                        {

                                                            JObject jDataColumn = (JObject)jLayoutArray[0]["data"];
                                                            JArray tableRow = (JArray)jDataColumn["row"];
                                                            iBoxLayout.Box_Control_Column = new List<CustomJsonAdvanceForm.ColumnTable_RefDoc>();
                                                            iBoxLayout.Box_Control_Table = new List<CustomJsonAdvanceForm.ColumnTable_RefDoc>();
                                                            int columnCount = 0;
                                                            foreach (JObject kItem in itemsColumn)
                                                            {

                                                                CustomJsonAdvanceForm.ColumnTable_RefDoc iColumnTable = new CustomJsonAdvanceForm.ColumnTable_RefDoc();

                                                                iColumnTable.Column_Label = (String)kItem["label"];
                                                                iColumnTable.Column_AltLabel = (String)kItem["alter"];
                                                                iColumnTable.Column_ControlType = CustomJsonAdvanceForm.GetControlTypeByJSONKey((String)kItem["control"]["template"]["type"]);
                                                                //iColumnTable.Column_ControlTypeCss = GetCssIconControlType(iColumnTable.Column_ControlType.ToString());
                                                                iColumnTable.Column_ControlTypeText = GetTextControlType(iColumnTable.Column_ControlType.ToString());
                                                                if (!String.IsNullOrEmpty(kItem["control"]["template"].ToString()))
                                                                {
                                                                    JObject ktemplate = (JObject)kItem["control"]["template"];
                                                                    if (ktemplate != null)
                                                                    {

                                                                        iColumnTable.Column_ControlType = CustomJsonAdvanceForm.GetControlTypeByJSONKey((String)ktemplate["type"]);
                                                                        //iColumnTable.Column_ControlTypeCss = GetCssIconControlType(iColumnTable.Column_ControlType.ToString());
                                                                        iColumnTable.Column_ControlTypeText = GetTextControlType(iColumnTable.Column_ControlType.ToString());
                                                                        if (ktemplate["attribute"] != null)
                                                                            iColumnTable.Column_Description = (String)ktemplate["attribute"];

                                                                        if (!String.IsNullOrEmpty(ktemplate["attribute"].ToString()))
                                                                        {
                                                                            JObject kAttribute = (JObject)ktemplate["attribute"];
                                                                            if (kAttribute != null)
                                                                            {
                                                                                if (String.IsNullOrEmpty(iColumnTable.Column_Description) && kAttribute["description"] != null)
                                                                                    iColumnTable.Column_Description = (String)kAttribute["description"];
                                                                                iColumnTable.Column_DefaultValue = (String)kAttribute["default"];
                                                                                JObject objkDate = (JObject)kAttribute["date"];

                                                                                if (kAttribute["items"] != null)
                                                                                {
                                                                                    if (kAttribute["items"].HasValues)
                                                                                    {
                                                                                        JArray itemsItemInColumn = (JArray)kAttribute["items"];
                                                                                        if (itemsItemInColumn != null)
                                                                                        {
                                                                                            foreach (JObject lItem in itemsItemInColumn)
                                                                                            {
                                                                                                if (!String.IsNullOrEmpty(iColumnTable.Column_Item))
                                                                                                    iColumnTable.Column_Item += ",";
                                                                                                iColumnTable.Column_Item += $"{lItem["item"]}";
                                                                                            }

                                                                                        }
                                                                                    }
                                                                                }

                                                                            }
                                                                        }

                                                                    }
                                                                }

                                                                if (tableRow != null)
                                                                {
                                                                    iBoxLayout.Box_Control_TableRowCount = tableRow.Count;
                                                                    int rowCount = 0;
                                                                    foreach (JArray dataRow in tableRow)
                                                                    {
                                                                        CustomJsonAdvanceForm.ColumnTable_RefDoc iTable = new CustomJsonAdvanceForm.ColumnTable_RefDoc();
                                                                        iTable.Box_Control_RowIndex = rowCount;
                                                                        iTable.Table_Name = iBoxLayout.Box_Control_Label;
                                                                        iTable.Column_Label = (String)kItem["label"];
                                                                        iTable.Column_AltLabel = (String)kItem["alter"];
                                                                        iTable.Box_Control_Value = (String)dataRow[columnCount]["value"];
                                                                        iTable.Column_ControlType = iColumnTable.Column_ControlType;
                                                                        iTable.Column_ControlTypeCss = iColumnTable.Column_ControlTypeCss;
                                                                        iTable.Column_ControlTypeText = iColumnTable.Column_ControlTypeText;
                                                                        JArray DataItem = (JArray)kItem["control"]["template"]["attribute"]["items"];
                                                                        JArray DataItemValue = (JArray)dataRow[columnCount]["item"];
                                                                        if (DataItemValue != null)
                                                                        {
                                                                            foreach (JValue kRowItem in DataItemValue)
                                                                            {
                                                                                if (!String.IsNullOrEmpty(iTable.Box_Control_ItemValue))
                                                                                    iTable.Box_Control_ItemValue += ",";
                                                                                iTable.Box_Control_ItemValue += $"{kRowItem}";
                                                                            }
                                                                        }
                                                                        if (DataItem != null)
                                                                        {
                                                                            string[] selectedValue = (string[])null;
                                                                            if (!string.IsNullOrEmpty(iTable.Box_Control_ItemValue))
                                                                            {
                                                                                selectedValue = iTable.Box_Control_ItemValue.Split(',');
                                                                            }
                                                                            int i = 0;
                                                                            foreach (JObject kRowItem in DataItem)
                                                                            {
                                                                                if (!String.IsNullOrEmpty(iTable.Column_Item))
                                                                                    iTable.Column_Item += ",";
                                                                                iTable.Column_Item += $"{kRowItem["item"]}";

                                                                                if (selectedValue[i].ToUpper() == "Y")
                                                                                {
                                                                                    if (!String.IsNullOrEmpty(iTable.Box_Control_Value))
                                                                                        iTable.Box_Control_Value += ",";
                                                                                    iTable.Box_Control_Value += $"{kRowItem["item"]}";
                                                                                }
                                                                                i++;
                                                                            }
                                                                        }
                                                                        iBoxLayout.Box_Control_Table.Add(iTable);
                                                                        rowCount++;
                                                                    }
                                                                }
                                                                iColumnTable.Box_ID = iBoxLayout.Box_ID;
                                                                iBoxLayout.Box_Control_Column.Add(iColumnTable);
                                                                columnCount++;
                                                            }
                                                        }

                                                        #endregion

                                                    }
                                                }
                                            }
                                        }

                                        JObject jData = (JObject)jLayoutArray[1]["data"];
                                        if (jData != null)
                                        {
                                            iBoxLayout.Box_Control_Value = (String)jData["value"];
                                            JArray DataItem = (JArray)jData["item"];
                                            if (DataItem != null)
                                            {
                                                string[] selectedValue = (string[])null;
                                                if (!string.IsNullOrEmpty(iBoxLayout.Box_Control_Item))
                                                {
                                                    selectedValue = iBoxLayout.Box_Control_Item.Split(',');
                                                }
                                                int i = 0;
                                                foreach (JValue kItem in DataItem)
                                                {
                                                    if (!String.IsNullOrEmpty(iBoxLayout.Box_Control_ItemValue))
                                                        iBoxLayout.Box_Control_ItemValue += ",";
                                                    iBoxLayout.Box_Control_ItemValue += $"{kItem}";

                                                    if (selectedValue != null && $"{kItem}".ToUpper() == "Y")
                                                    {
                                                        if (!String.IsNullOrEmpty(iBoxLayout.Box_Control_Value))
                                                            iBoxLayout.Box_Control_Value += ",";
                                                        iBoxLayout.Box_Control_Value += selectedValue[i];
                                                    }
                                                    i++;
                                                }
                                            }
                                        }

                                        listBoxLayout.Add(iBoxLayout);
                                    }


                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }
                return listBoxLayout;
            }
        }
    }
}
