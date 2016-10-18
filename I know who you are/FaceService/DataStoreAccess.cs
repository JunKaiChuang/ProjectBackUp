using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using static FaceService.FaceRecognitionService;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace FaceService
{
    /// <summary>
    /// 個人資料model
    /// </summary>
    public class PersonInfo
    {
        public Int64 userId { get; set; }

        [DisplayName("性別")]
        public string gender { get; set; }

        [DisplayName("出生日期")]
        public DateTime birthDate { get; set; }

        [DisplayName("標籤")]
        public string tag { get; set; }

        [DisplayName("名稱")]
        [Required(ErrorMessage = "此欄位必填")]
        public string userName { get; set; }
    }

    /// <summary>
    /// 功能名稱 : 臉部資料存取
    /// </summary>
    class DataStoreAccess
    {
        private SQLiteConnection _sqLiteConnection;

        /// <summary>
        /// 初始化資料庫連結
        /// </summary>
        /// <param name="databasePath"></param>
        public DataStoreAccess(String databasePath)
        {
            _sqLiteConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath));

        }

        /// <summary>
        /// 建立個人資料
        /// </summary>
        /// <param name="personInfo"></param>
        /// <returns></returns>
        public String CreatePersonInfo(PersonInfo personInfo)
        {
            try
            {
                _sqLiteConnection.Open();
                var insertQuery = "INSERT INTO personInfo(userId, gender, birthDate, tag, userName) VALUES(@userId, @gender, @birthDate, @tag, @userName)";
                var cmd = new SQLiteCommand(insertQuery, _sqLiteConnection);
                cmd.Parameters.AddWithValue("userId", personInfo.userId);
                cmd.Parameters.AddWithValue("gender", personInfo.gender == null ? "" : personInfo.gender);
                cmd.Parameters.AddWithValue("birthDate", personInfo.birthDate == DateTime.MinValue ? DateTime.Today : personInfo.birthDate);
                cmd.Parameters.AddWithValue("tag", personInfo.tag == null ? "" : personInfo.tag);
                cmd.Parameters.AddWithValue("userName", personInfo.userName);
                var result = cmd.ExecuteNonQuery();
                return String.Format("{0} Person information create successfully", result);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                _sqLiteConnection.Close();
            }
        }

        /// <summary>
        /// 儲存人臉至資料庫
        /// </summary>
        /// <param name="username">人名</param>
        /// <param name="faceBlob">人臉影像</param>
        /// <returns></returns>
        public String SaveFace(string userName, string gender, Byte[] faceBlob)
        {
            try
            {
                var exisitingUserId = GetUserId(userName);
                if (exisitingUserId == 0) {
                    exisitingUserId = GenerateUserId();
                    PersonInfo tempPerson = new PersonInfo {
                        userName = userName,
                        gender = gender,
                        userId = exisitingUserId
                    };
                    CreatePersonInfo(tempPerson);
                }
                
                _sqLiteConnection.Open();
                var insertQuery = "INSERT INTO faces(faceSample, userId) VALUES(@faceSample,@userId)";
                var cmd = new SQLiteCommand(insertQuery, _sqLiteConnection);
                cmd.Parameters.AddWithValue("userId", exisitingUserId);
                cmd.Parameters.Add("faceSample", DbType.Binary, faceBlob.Length).Value = faceBlob;
                var result = cmd.ExecuteNonQuery();
                return String.Format("{0} face(s) saved successfully", result);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                _sqLiteConnection.Close();
            }

        }

        /// <summary>
        /// 從資料庫取出對應人名的臉部影像
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public List<Face> CallFaces(string username)
        {
            var faces = new List<Face>();
            try
            {
                _sqLiteConnection.Open();
                var query = username.ToLower().Equals("ALL_USERS".ToLower()) 
                    ? @"  SELECT F.id, F.faceSample, P.userId, P.userName
                          FROM faces AS F, personInfo AS P
                          WHERE F.userId = P.userId"
                    : @"  SELECT F.id, F.faceSample, P.userId, P.userName
                          FROM faces AS F, personInfo AS P
                          WHERE P.userName=@userName AND F.userId = P.userId";
                var cmd = new SQLiteCommand(query, _sqLiteConnection);
                if (!username.ToLower().Equals("ALL_USERS".ToLower())) cmd.Parameters.AddWithValue("userName", username);
                var result = cmd.ExecuteReader();
                if (!result.HasRows) return null;

                while (result.Read())
                {
                    var face = new Face
                    {
                        Image = (byte[])result["faceSample"],
                        Id = Convert.ToInt32(result["id"]),
                        Label = (String)result["userName"],
                        UserId = Convert.ToInt32(result["userId"])
                    };
                    faces.Add(face);
                }
                faces = faces.OrderBy(f => f.Id).ToList();



            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                _sqLiteConnection.Close();
            }
            return faces;
        }

        /// <summary>
        /// 取得資料庫中faceSample
        /// </summary>
        /// <param name="id">faces Id</param>
        /// <returns></returns>
        public byte[] GetFaceById(string id)
        {
            var face = new Face();
            try
            {
                _sqLiteConnection.Open();
                var query = @"  SELECT F.id, F.faceSample, P.userId, P.userName
                                FROM faces AS F, personInfo AS P
                                WHERE F.id=@id AND F.userId = P.userId";
                var cmd = new SQLiteCommand(query, _sqLiteConnection);
                cmd.Parameters.AddWithValue("id", id);
                var result = cmd.ExecuteReader();
                while (result.Read())
                {
                    var faceDetail = new Face
                    {
                        Image = (byte[])result["faceSample"],
                        Id = Convert.ToInt32(result["id"]),
                        Label = (String)result["userName"],
                        UserId = Convert.ToInt32(result["userId"])
                    };
                    face = faceDetail;
                }
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                _sqLiteConnection.Close();
            }
            return face.Image;
        }

        /// <summary>
        /// 取得臉部資料清單
        /// </summary>
        /// <param name="userName">人名</param>
        /// <returns></returns>
        public List<string> GetFacesByUserName(string userName)
        {
            var faces = new List<string>();
            try
            {
                _sqLiteConnection.Open();
                var query = @"  SELECT F.id 
                                FROM faces AS F, personInfo AS P
                                WHERE P.username = @userName AND F.userId = P.userId";
                var cmd = new SQLiteCommand(query, _sqLiteConnection);
                cmd.Parameters.AddWithValue("userName", userName);
                var result = cmd.ExecuteReader();
                while (result.Read())
                {
                    faces.Add(result["id"].ToString());
                }
                faces.Sort();
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                _sqLiteConnection.Close();
            }
            return faces;
        }

        /// <summary>
        /// 取得臉部資料清單
        /// </summary>
        /// <param name="userName">人名</param>
        /// <returns></returns>
        public DataTable GetFacesListByUserID(int userID)
        {
            var faces = new DataTable();
            try
            {
                _sqLiteConnection.Open();
                var query = @"  SELECT F.id AS Value,'Face' AS Text
                                FROM faces AS F
                                WHERE F.userId = @userId";
                var cmd = new SQLiteCommand(query, _sqLiteConnection);
                cmd.Parameters.AddWithValue("userId", userID);
                SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
                da.Fill(faces);
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                _sqLiteConnection.Close();
            }
            return faces;
        }

        /// <summary>
        /// 取得人名對應的UserId
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public int GetUserId(string userName)
        {
            var userId = 0;
            try
            {
                _sqLiteConnection.Open();
                var selectQuery = "SELECT userId FROM personInfo WHERE userName=@userName LIMIT 1";
                var cmd = new SQLiteCommand(selectQuery, _sqLiteConnection);
                cmd.Parameters.AddWithValue("userName", userName);
                var result = cmd.ExecuteReader();
                if (!result.HasRows) return 0;
                while (result.Read())
                {
                    userId = Convert.ToInt32(result["userId"]);

                }
            }
            catch
            {
                return userId;
            }
            finally
            {
                _sqLiteConnection.Close();
            }
            return userId; ;
        }

        /// <summary>
        /// 取得UserId對應的人名
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetUsername(int userId)
        {
            var username = "";
            try
            {
                _sqLiteConnection.Open();
                var selectQuery = "SELECT username FROM personInfo WHERE userId=@userId LIMIT 1";
                var cmd = new SQLiteCommand(selectQuery, _sqLiteConnection);
                cmd.Parameters.AddWithValue("userId", userId);
                var result = cmd.ExecuteReader();
                if (!result.HasRows) return username;
                while (result.Read())
                {
                    username = (String)result["userName"];

                }
            }
            catch
            {
                return username;
            }
            finally
            {
                _sqLiteConnection.Close();
            }
            return username; ;
        }

        /// <summary>
        /// 取得所有資料庫中的人名
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllUsernames()
        {
            var usernames = new List<string>();
            try
            {
                _sqLiteConnection.Open();
                var query = "SELECT DISTINCT userName FROM personInfo";
                var cmd = new SQLiteCommand(query, _sqLiteConnection);
                var result = cmd.ExecuteReader();
                while (result.Read())
                {
                    usernames.Add((String)result["userName"]);
                }
                usernames.Sort();
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                _sqLiteConnection.Close();
            }
            return usernames;
        }

        /// <summary>
        /// 取得所有個人資料
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllPersonInfo() {
            DataSet ds = new DataSet();
            var query = @"      SELECT P.userId, P.userName, C.codeData AS gender
                                FROM personInfo AS P, codeTable AS C
                                WHERE C.codeType='Gender' AND C.codeValue = P.gender";
            var cmd = new SQLiteCommand(query, _sqLiteConnection);

            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
            da.Fill(ds);
            return ds.Tables.Count > 0 ? ds.Tables[0] : new DataTable();
        }

        /// <summary>
        /// 取得個人資料
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DataTable GetPersonInfo(int userId)
        {
            DataSet ds = new DataSet();
            var query = @"  SELECT P.userId, P.userName, P.birthDate, P.tag, C.codeData AS gender
                            FROM personInfo AS P, codeTable AS C
                            WHERE P.userId = @userId AND C.codeType = 'Gender' AND C.codeValue = P.gender LIMIT 1";
            var cmd = new SQLiteCommand(query, _sqLiteConnection);
            cmd.Parameters.AddWithValue("userId", userId);
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
            da.Fill(ds);

            //var tempPerson = new PersonInfo
            //{
            //    userId = Convert.ToInt64(ds.Tables[0].Rows[0]["userId"]),
            //    userName = ds.Tables[0].Rows[0]["userName"].ToString(),
            //    gender = ds.Tables[0].Rows[0]["gender"].ToString(),
            //    birthDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["birthDate"].ToString()),
            //    tag = ds.Tables[0].Rows[0]["tag"].ToString()
            //};
            ds.Tables[0].Rows[0]["birthDate"] = Convert.ToDateTime(ds.Tables[0].Rows[0]["birthDate"]).Date.ToString("yyyy/MM/dd");
            return ds.Tables[0];
        }

        /// <summary>
        /// 更新個人資料
        /// </summary>
        /// <param name="personInfo"></param>
        public void UpdatePersonInfo(PersonInfo personInfo)
        {
            try
            {
                _sqLiteConnection.Open();
                var insertQuery = @"    UPDATE personInfo
                                        SET gender = @gender, birthDate = @birthDate, tag = @tag, userName = @userName
                                        WHERE userId = @userId";
                var cmd = new SQLiteCommand(insertQuery, _sqLiteConnection);
                cmd.Parameters.AddWithValue("userId", personInfo.userId);
                cmd.Parameters.AddWithValue("gender", personInfo.gender == null ? "" : personInfo.gender);
                cmd.Parameters.AddWithValue("birthDate", personInfo.birthDate == DateTime.MinValue ? DateTime.Today : personInfo.birthDate);
                cmd.Parameters.AddWithValue("tag", personInfo.tag == null ? "" : personInfo.tag);
                cmd.Parameters.AddWithValue("userName", personInfo.userName);
                var result = cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _sqLiteConnection.Close();
            }
        }

        /// <summary>
        /// 刪除個人資料
        /// </summary>
        /// <param name="userId"></param>
        public void DeletePerson(int userId)
        {
            try
            {
                _sqLiteConnection.Open();
                var insertQuery = @"    PRAGMA foreign_keys = 1;
                                        DELETE
                                        FROM personInfo
                                        WHERE userId = @userId";
                var cmd = new SQLiteCommand(insertQuery, _sqLiteConnection);
                cmd.Parameters.AddWithValue("userId", userId);
                var result = cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _sqLiteConnection.Close();
            }
        }

        /// <summary>
        /// 刪除人名
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool DeleteUser(string username)
        {
            var toReturn = false;
            try
            {
                _sqLiteConnection.Open();
                var query = "DELETE FROM personInfo WHERE userName=@userName";
                var cmd = new SQLiteCommand(query, _sqLiteConnection);
                cmd.Parameters.AddWithValue("userName", username);
                var result = cmd.ExecuteNonQuery();
                if (result > 0) toReturn = true;
            }
            catch (Exception)
            {
                return toReturn;
            }
            finally
            {
                _sqLiteConnection.Close();
            }
            return toReturn;
        }

        /// <summary>
        /// 刪除人臉資料
        /// </summary>
        /// <param name="id">人臉資料ID</param>
        /// <returns></returns>
        public bool DeleteFace(string id)
        {
            var toReturn = false;
            try
            {
                _sqLiteConnection.Open();
                var query = "DELETE FROM faces WHERE id=@id";
                var cmd = new SQLiteCommand(query, _sqLiteConnection);
                cmd.Parameters.AddWithValue("id", id);
                var result = cmd.ExecuteNonQuery();
                if (result > 0) toReturn = true;
            }
            catch (Exception)
            {
                return toReturn;
            }
            finally
            {
                _sqLiteConnection.Close();
            }
            return toReturn;
        }

        /// <summary>
        /// 產生UserId
        /// </summary>
        /// <returns></returns>
        public int GenerateUserId()
        {
            var date = DateTime.Now.ToString("MMddHHmmss");
            return Convert.ToInt32(date);
        }

        
        public bool IsUsernameValid(string username)
        {
            throw new NotImplementedException();
        }

        public string SaveAdmin(string username, string password)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 取得性別代碼
        /// </summary>
        /// <returns></returns>
        public DataTable GetGenderCode()
        {
            DataTable dt = new DataTable();
            var query = @"      SELECT C.codeData AS Text, C.codeValue AS Value
                                FROM codeTable AS C
                                WHERE C.codeType = 'Gender'";
            var cmd = new SQLiteCommand(query, _sqLiteConnection);

            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
            da.Fill(dt);
            return dt;
        }
    }
}
