using MDM.Models.Attributes;
using MDM.Models.DataModels;
using MDM.Models.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SQLite;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

namespace MDM.Helpers
{
    public static partial class DBHelper
    {
        private static string _DBPath = string.Empty;
        public static string ConnectionSting
        {
            get
            {
                string output = "Data Source=";
                output += _DBPath;
                output += ";Version=3;";
                return output;
            }
            set => _DBPath = value;
        }

        private static string TABLE_MATERIAL = "t_Material";
        private static string TABLE_SLIDE = "t_Slide";
        private static string TABLE_SHAPE = "t_Shape";
        //private static string TABLE_TEXT_SHAPE = "t_TextShape";
        //private static string TABLE_IMAGE_SHAPE = "t_ImageShape";
        //private static string TABLE_TABLE_SHAPE = "t_TableShape";
        private static string TABLE_ITEM = "t_Item";

        private static string[] COLUMN_MATERIAL =       { "Uid", "ParentUid", "IsUsed", "CreateDate", "UpdateDate", "DeleteDate",   "Name" };
        private static string[] COLUMN_SLIDE =          { "Uid", "ParentUid", "IsUsed", "CreateDate", "UpdateDate", "DeleteDate",   "MaterialId", "SlideId", "SlideIndex", "Name", "SlideNumber", "Status", "Description" };
        private static string[] COLUMN_SHAPE =          { "Uid", "ParentUid", "IsUsed", "CreateDate", "UpdateDate", "DeleteDate",   "SlideId", "ShapeType", "ShapeId", "AlternativeText", "Title", "Name", "Left", "Top", "Width", "Height", "DistanceFromOrigin", "Text", "DataTable" };
        //private static string[] COLUMN_TEXT_SHAPE =     { "SubIdx", "ShapeEnjtityIdx", "Text"};
        //private static string[] COLUMN_IMAGE_SHAPE =    { "SubIdx", "ShapeEnjtityIdx", "ImageName", "Extension" };
        //private static string[] COLUMN_TABLE_SHAPE =    { "SubIdx", "ShapeEnjtityIdx", "TableString", "Table" };
        private static string[] COLUMN_ITEM =           { "Uid", "ParentUid", "IsUsed", "CreateDate", "UpdateDate", "DeleteDate", "ParentItemUid", "ParentShapeIdx", "ParentItemIdx", "ItemOrder", "ItemType", "Level", "LineText", "Title" };

        public static bool Create(this mMaterial material)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionSting))
                {
                    connection.Open();

                    string insertQuery = "INSERT INTO ";
                    insertQuery += TABLE_MATERIAL;
                    insertQuery += "(";
                    foreach (string col in COLUMN_MATERIAL)
                    {
                        insertQuery += col;
                        if (col == COLUMN_MATERIAL.Last()) break;
                        insertQuery += ", ";
                    }
                    insertQuery += ") VALUES (";
                    foreach (string col in COLUMN_MATERIAL)
                    {
                        insertQuery += "@" + col;
                        if (col == COLUMN_MATERIAL.Last()) break;
                        insertQuery += ", ";
                    }
                    insertQuery += ")";

                    using (var command = new SQLiteCommand(insertQuery, connection))
                    {
                        foreach (string col in COLUMN_MATERIAL)
                        {
                            command.Parameters.AddWithValue("@"+col, GetValue(material, col));
                        }

                        // 쿼리 실행
                        if(command.ExecuteNonQuery() == 1)
                        {
                            material.Idx = int.Parse(connection.LastInsertRowId.ToString());
                        }
                        return true;
                    }
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
            return false;
        }
        public static bool Create(this mSlide slide)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionSting))
                {
                    connection.Open();

                    string insertQuery = "INSERT INTO ";
                    insertQuery += TABLE_SLIDE;
                    insertQuery += "(";
                    foreach (string col in COLUMN_SLIDE)
                    {
                        insertQuery += col;
                        if (col == COLUMN_SLIDE.Last()) break;
                        insertQuery += ", ";
                    }
                    insertQuery += ") VALUES (";
                    foreach (string col in COLUMN_SLIDE)
                    {
                        insertQuery += "@" + col;
                        if (col == COLUMN_SLIDE.Last()) break;
                        insertQuery += ", ";
                    }
                    insertQuery += ")";

                    using (var command = new SQLiteCommand(insertQuery, connection))
                    {
                        foreach (string col in COLUMN_SLIDE)
                        {
                            command.Parameters.AddWithValue("@" + col, GetValue(slide, col));
                        }

                        // 쿼리 실행
                        if (command.ExecuteNonQuery() == 1)
                        {
                            slide.Idx = int.Parse(connection.LastInsertRowId.ToString());
                        }
                        return true;
                    }
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
            return false;
        }
        public static bool Create(this mShape shape)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionSting))
                {
                    connection.Open();

                    string insertQuery = GenerateInserQuery(shape);
                    using (var command = new SQLiteCommand(insertQuery, connection))
                    {
                        foreach (string col in GetColumnNames(shape))
                        {
                            command.Parameters.AddWithValue("@" + col, GetValue(shape, col));
                        }

                        // 쿼리 실행
                        if (command.ExecuteNonQuery() != 1) return false;
                        shape.Idx = int.Parse(connection.LastInsertRowId.ToString());
                    }
                    return true;
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
            return false;
        }
        public static bool Create(this mItem item)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionSting))
                {
                    connection.Open();

                    string insertQuery = GenerateInserQuery(item);

                    using (var command = new SQLiteCommand(insertQuery, connection))
                    {
                        foreach (string col in GetColumnNames(item))
                        {
                            command.Parameters.AddWithValue("@" + col, GetValue(item, col));
                        }

                        // 쿼리 실행
                        if (command.ExecuteNonQuery() == 1)
                        {
                            item.Idx = int.Parse(connection.LastInsertRowId.ToString());
                        }
                        return true;
                    }
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
            return false;
        }


        public static mMaterial Read()
        {
            mMaterial output = null;
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionSting))
                {
                    string selectQuery = "SELECT * FROM " + TABLE_MATERIAL;
                    selectQuery += " WHERE IsUsed=1";

                    connection.Open();
                    SQLiteCommand cmd = new SQLiteCommand(selectQuery, connection);
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        output = new mMaterial();
                        output.Idx = int.Parse(reader["Idx"].ToString());
                        foreach (string col in COLUMN_MATERIAL)
                        {
                            string value = reader[col].ToString();

                            PropertyInfo[] pInfos = output.GetType().GetProperties();
                            foreach (PropertyInfo p in pInfos)
                            {
                                ColumnHeaderAttribute colHeader = p.GetCustomAttribute(typeof(ColumnHeaderAttribute)) as ColumnHeaderAttribute;
                                if (colHeader == null) continue;
                                if (colHeader.ColumnValue != col) continue;

                                string fullName = p.PropertyType.FullName;
                                if (fullName == typeof(string).FullName)
                                {
                                    p.SetValue(output, value);
                                    break;
                                }
                                if (fullName == typeof(int).FullName)
                                {
                                    p.SetValue(output, int.Parse(value));
                                    break;
                                }
                                if (fullName == typeof(DateTime).FullName)
                                {
                                    p.SetValue(output, DateTime.Parse(value));
                                    break;
                                }
                                if (fullName == typeof(DateTime?).FullName)
                                {
                                    if (!string.IsNullOrEmpty(value)) p.SetValue(output, DateTime.Parse(value));
                                    break;
                                }
                                if (fullName == typeof(bool).FullName)
                                {
                                    p.SetValue(output, int.Parse(value) == 1);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
            return output;
        }
        public static List<mSlide> Read(mMaterial parent)
        {
            List<mSlide> output = new List<mSlide>();
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionSting))
                {
                    string selectQuery = GenerateSelectQuery(parent);
                    selectQuery += "AND MaterialId=";
                    selectQuery += parent.Idx;

                    connection.Open();
                    SQLiteCommand cmd = new SQLiteCommand(selectQuery, connection);
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        mSlide newSlide = new mSlide();
                        newSlide.Idx = int.Parse(reader["Idx"].ToString());
                        foreach (string col in COLUMN_SLIDE)
                        {
                            string value = reader[col].ToString();
                            SetPropertyValue(col, newSlide, value);
                        }

                        output.Add(newSlide);
                    }
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
                return null;
            }
            return output;
        }
        public static List<mShape> Read(mSlide parent)
        {
            List<mShape> output = new List<mShape>();
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionSting))
                {
                    string selectQuery = GenerateSelectQuery(parent);
                    selectQuery += "AND SlideId=";
                    selectQuery += parent.Idx;

                    connection.Open();
                    SQLiteCommand cmd = new SQLiteCommand(selectQuery, connection);
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        mShape mShape = new mShape();
                        mShape.Idx = int.Parse(reader["Idx"].ToString());
                        foreach (string col in COLUMN_SHAPE)
                        {
                            string value = reader[col].ToString();
                            SetPropertyValue(col, mShape, value);
                        }

                        output.Add(mShape);
                    }
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
                return null;
            }
            return output;
        }
        public static List<mItem> Read(mShape parent)
        {
            List<mItem> output = new List<mItem>();
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionSting))
                {
                    string selectQuery = GenerateSelectQuery(parent);
                    selectQuery += "AND ParentShapeIdx=";
                    selectQuery += parent.Idx;

                    connection.Open();
                    SQLiteCommand cmd = new SQLiteCommand(selectQuery, connection);
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        mItem newItem = new mItem();
                        newItem.Idx = int.Parse(reader["Idx"].ToString());
                        foreach (string col in COLUMN_ITEM)
                        {
                            string value = reader[col].ToString();
                            SetPropertyValue(col, newItem, value);
                        }

                        output.Add(newItem);
                    }
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
                return null;
            }
            return output;
        }

        public static bool Update(this mMaterial material)
        {
            try
            {
                string[] COLUMN_UPDATE = { "ParentUid", "Name" };
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionSting))
                {
                    connection.Open();

                    string updateQuery = "UPDATE ";
                    updateQuery += TABLE_MATERIAL;
                    updateQuery += " SET ";
                    updateQuery += "ParentUid = @ParentUid, ";
                    updateQuery += "Name = @Name, ";
                    updateQuery += "UpdateDate = @UpdateDate ";
                    updateQuery += "WHERE Idx =@Idx";

                    using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                    {
                        DateTime now  = DateTime.Now;
                             
                        // 파라미터 값 설정
                        command.Parameters.AddWithValue("@ParentUid", material.ParentUid);
                        command.Parameters.AddWithValue("@Name", material.Name);
                        command.Parameters.AddWithValue("@UpdateDate", now);
                        command.Parameters.AddWithValue("@Idx", material.Idx);  

                        // 쿼리 실행
                        int rowsAffected = command.ExecuteNonQuery();
                        if(rowsAffected ==1) 
                        {
                            material.UpdateDate = now;  
                        }
                        return rowsAffected == 1;
                    }
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
            return false;
        }
        public static bool Update(this mSlide obj)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionSting))
                {
                    connection.Open();

                    string updateQuery = GenerateUpdateQuery(obj);

                    using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Idx", obj.Idx);
                        foreach (string col in GetColumnNames(obj))
                        {
                            command.Parameters.AddWithValue("@" + col, GetValue(obj, col));
                        }

                        // 쿼리 실행
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected == 1;
                    }
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
            return false;
        }
        public static bool Update(this mShape obj)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionSting))
                {
                    connection.Open();

                    string updateQuery = GenerateUpdateQuery(obj);

                    using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Idx", obj.Idx);
                        foreach (string col in GetColumnNames(obj))
                        {
                            command.Parameters.AddWithValue("@" + col, GetValue(obj, col));
                        }

                        // 쿼리 실행
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected == 1;
                    }
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
            return false;
        }
        public static bool Update(this mItem obj)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionSting))
                {
                    connection.Open();

                    string updateQuery = GenerateUpdateQuery(obj);

                    using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Idx", obj.Idx);
                        foreach (string col in GetColumnNames(obj))
                        {
                            command.Parameters.AddWithValue("@" + col, GetValue(obj, col));
                        }

                        // 쿼리 실행
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected == 1;
                    }
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
            return false;
        }

        public static bool Delete(this mMaterial obj) 
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionSting))
                {
                    connection.Open();

                    string updateQuery = "UPDATE ";
                    updateQuery += TABLE_MATERIAL;
                    updateQuery += " SET ";
                    updateQuery += "IsUsed = @IsUsed, ";
                    updateQuery += "DeleteDate = @DeleteDate ";
                    updateQuery += "WHERE Idx =@Idx";

                    using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                    {
                        DateTime now = DateTime.Now;
                        // 파라미터 값 설정
                        command.Parameters.AddWithValue("@IsUsed", 0);
                        command.Parameters.AddWithValue("@DeleteDate", now);
                        command.Parameters.AddWithValue("@Idx", obj.Idx);

                        // 쿼리 실행
                        int rowsAffected = command.ExecuteNonQuery();
                        if(rowsAffected == 1)
                        {
                            obj.IsUsed = false;
                            obj.DeletedDate = now;
                        }
                        return rowsAffected == 1;
                    }
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
            return false;
        }
        public static bool Delete(this mSlide obj)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionSting))
                {
                    connection.Open();

                    string deleteQuery = GenerateDeleteQuery(obj);  

                    using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                    {
                        DateTime now = DateTime.Now;
                        // 파라미터 값 설정
                        command.Parameters.AddWithValue("@IsUsed", 0);
                        command.Parameters.AddWithValue("@DeleteDate", now);
                        command.Parameters.AddWithValue("@Idx", obj.Idx);

                        // 쿼리 실행
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected == 1)
                        {
                            obj.IsUsed = false;
                            obj.DeletedDate = now;
                        }
                        return rowsAffected == 1;
                    }
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
            return false;
        }
        public static bool Delete(this mShape obj)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionSting))
                {
                    connection.Open();

                    string deleteQuery = GenerateDeleteQuery(obj);

                    using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                    {
                        DateTime now = DateTime.Now;
                        // 파라미터 값 설정
                        command.Parameters.AddWithValue("@IsUsed", 0);
                        command.Parameters.AddWithValue("@DeleteDate", now);
                        command.Parameters.AddWithValue("@Idx", obj.Idx);

                        // 쿼리 실행
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected == 1)
                        {
                            obj.IsUsed = false;
                            obj.DeletedDate = now;
                        }
                        return rowsAffected == 1;
                    }
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
            return false;
        }
        public static bool Delete(this mItem obj)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionSting))
                {
                    connection.Open();

                    string deleteQuery = GenerateDeleteQuery(obj);

                    using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                    {
                        DateTime now = DateTime.Now;
                        // 파라미터 값 설정
                        command.Parameters.AddWithValue("@IsUsed", 0);
                        command.Parameters.AddWithValue("@DeleteDate", now);
                        command.Parameters.AddWithValue("@Idx", obj.Idx);

                        // 쿼리 실행
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected == 1)
                        {
                            obj.IsUsed = false;
                            obj.DeletedDate = now;
                        }
                        return rowsAffected == 1;
                    }
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
            return false;
        }

        public static bool CreateTables()
        {
            try
            {
                string materialTable = GenerateTableCreateQuery(typeof(mMaterial));
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
                return false;
            }

            return true;
        }

        private static string GenerateTableCreateQuery(Type type)
        {
            string output = string.Empty;

            output += "CREATE TABLE IF NOT EXISTS ";
            output += GetTableName(type);
            output += " ( ";
            output += "";

            return output;
        }
    }

    public static partial class DBHelper
    {
        
        public static object GetValue(object obj, string col)
        {
            PropertyInfo[] pInfo = obj.GetType().GetProperties();
            foreach (PropertyInfo p in pInfo)
            {

                ColumnHeaderAttribute colHeader = p.GetCustomAttribute(typeof(ColumnHeaderAttribute)) as ColumnHeaderAttribute;
                if (colHeader == null) continue;

                if (colHeader.ColumnValue != col) continue;

                var value = p.GetValue(obj, null);

                string fullName = p.PropertyType.FullName;
                if (fullName == typeof(string).FullName) return value.ToString();
                if (fullName == typeof(int).FullName) return int.Parse(value.ToString());
                if (fullName == typeof(float).FullName) return float.Parse(value.ToString());
                if (fullName == typeof(double).FullName) return double.Parse(value.ToString());
                if (fullName == typeof(DateTime).FullName) return ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss.sss");
                if (fullName == typeof(DateTime?).FullName) return (((DateTime?)value).HasValue ? ((DateTime?)value).Value.ToString("yyyy-MM-dd HH:mm:ss.sss") : null);
                if (fullName == typeof(bool).FullName) return (bool)value == true ? 1 : 0;
            }
            return null;
        }
        private static string GenerateInserQuery(object obj)
        {
            string output = string.Empty;

            string tableName = GetTableName(obj);
            string[] colNames = GetColumnNames(obj);

            output = "INSERT INTO ";
            output += tableName;
            output += "(";
            foreach (string col in colNames)
            {
                output += col;
                if (col == colNames.Last()) break;
                output += ", ";
            }
            output += ") VALUES (";
            foreach (string col in colNames)
            {
                output += "@" + col;
                if (col == colNames.Last()) break;
                output += ", ";
            }
            output += ")";


            return output;
        }
        private static string GenerateSelectQuery(object obj, bool isDetailShape = false)
        {
            string output = string.Empty;

            string tableName = GetChildTableName(obj);

            output = "SELECT * FROM " + tableName;
            output += " WHERE IsUsed=1 ";
            return output;
        }
        private static string GenerateUpdateQuery(object obj)
        {
            string output = string.Empty;

            string tableName = GetTableName(obj);
            string[] colNames = GetColumnNames(obj);

            output = "UPDATE ";
            output += tableName;
            output += " SET ";
            foreach (string col in colNames)
            {
                output += string.Format("{0} = @{0}", col);
                if (col == colNames.Last()) break;
                output += ", ";
            }
            output += " WHERE Idx = @Idx";


            return output;
        }
        private static string GenerateDeleteQuery(object obj)
        {
            string output = string.Empty;

            string tableName = GetTableName(obj);

            output = "UPDATE ";
            output += tableName;
            output += " SET ";
            output += "IsUsed = 0, ";
            output += "DeleteDate = @DeleteDate ";
            output += "WHERE Idx = @Idx";

            return output;
        }
        private static string[] GetColumnNames(object obj)
        {
            string[] output = { };

            if (obj is mMaterial) return COLUMN_MATERIAL;
            if (obj is mSlide) return COLUMN_SLIDE;
            if (obj is mItem) return COLUMN_ITEM;
            if (obj is mShape) return COLUMN_SHAPE;
            
            return output;
        }
        private static string GetTableName(object obj)
        {
            if (obj is mMaterial) return TABLE_MATERIAL;
            if (obj is mSlide) return TABLE_SLIDE;
            if (obj is mItem) return TABLE_ITEM;
            if (obj is mShape) return TABLE_SHAPE;

            return string.Empty;
        }
        private static string GetTableName(Type type)
        {
            string output = string.Empty;
            DescriptionAttribute descriptionAttribute = (DescriptionAttribute)Attribute.GetCustomAttribute(type, typeof(DescriptionAttribute));
            if (descriptionAttribute != null) output = descriptionAttribute.Description;
            
            return output;
        }
        private static string GetChildTableName(object obj)
        {
            if (obj == null) return TABLE_MATERIAL;
            if (obj is mMaterial) return TABLE_SLIDE;
            if (obj is mSlide) return TABLE_SHAPE;
            if (obj is mShape) return TABLE_ITEM;
            return string.Empty;
        }
        public static void SetPropertyValue(string col, object obj, string value)
        {

            PropertyInfo[] pInfos = obj.GetType().GetProperties();
            foreach (PropertyInfo p in pInfos)
            {
                ColumnHeaderAttribute colHeader = p.GetCustomAttribute(typeof(ColumnHeaderAttribute)) as ColumnHeaderAttribute;
                if (colHeader == null) continue;
                if (colHeader.ColumnValue != col) continue;

                string fullName = p.PropertyType.FullName;
                if (fullName == typeof(string).FullName)
                {
                    p.SetValue(obj, value);
                    return;
                }
                if (fullName == typeof(int).FullName)
                {
                    p.SetValue(obj, int.Parse(value));
                    return;
                }
                if (fullName == typeof(DateTime).FullName)
                {
                    p.SetValue(obj, DateTime.Parse(value));
                    return;
                }
                if (fullName == typeof(DateTime?).FullName)
                {
                    if (!string.IsNullOrEmpty(value)) p.SetValue(obj, DateTime.Parse(value));
                    return;
                }
                if (fullName == typeof(bool).FullName)
                {
                    p.SetValue(obj, int.Parse(value) == 1);
                    return;
                }
            }

        }
    }
}
