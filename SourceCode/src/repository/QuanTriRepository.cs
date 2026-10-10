using MySqlConnector;
using System;
using System.Data;
using KindergartenManager.Model;

namespace KindergartenManager.Repository
{
    public class QuanTriRepository
    {
        public bool InsertQuanTriVien(QuanTriVien qt)
        {
            string sql = @"INSERT INTO quan_tri_vien (ma_quan_tri) VALUES (@id)";
            return DbManager.ExecuteNonQuery(sql, new MySqlParameter[]
            {
                new MySqlParameter("@id", qt.MaNguoiDung)
            }) > 0;
        } 
    }
}
