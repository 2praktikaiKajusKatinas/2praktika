using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Windows.Forms;


namespace Praktika2
{
    class Mysql
    {
        public MySqlConnection connection = new MySqlConnection("server=localhost;user id=root;password=root;database=2praktika");
        public bool OpenConnection() 
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex) //exceptionai ateiciai
            {
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        MessageBox.Show("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }
        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        string accesslevel;
        public string id;
        public string myname;
        public string Login(string username, string password) //prisijungia prie duombazes
        {
            
            string query = "SELECT accesslevel,usersid,username FROM users WHERE username ='" + username + "' AND password ='" + password + "'";
            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader dataReader = cmd.ExecuteReader();

            while (dataReader.Read())
            {
                accesslevel = dataReader["accesslevel"].ToString();
                id = dataReader["usersid"].ToString();
                myname = dataReader["username"].ToString();
            }
            dataReader.Close();
            CloseConnection();
            return accesslevel;
        }

        #region "Admin lecturers tab" ///////////////////////

        
        public List<string> tempLectureList = new List<string>();
        public List<string> tempLectureList2 = new List<string>();
        public void FillLectures() //uzpido lista, kad rodytu grupes visas pas admin
        {
            string query = "SELECT users.username,lectures.name FROM 2praktika.lecturers JOIN 2praktika.users ON users.usersid=lecturers.lecturerid JOIN 2praktika.lectures ON lectures.lectureid=lecturers.lecturesid ORDER BY relationshipid;";
            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                tempLectureList.Add(dataReader["username"].ToString()); // destytojo vardas
                tempLectureList2.Add(dataReader["name"].ToString()); // paskaitos vardas
            }
            CloseConnection();
        }

        //sukuria sarasa visu destytoju
        public List<string> tempLecturersOnlyList = new List<string>();
        public List<string> tempLecturesOnlyList = new List<string>();
        public List<string> tempLecturersIdOnlyList = new List<string>();
        public List<string> tempLecturesIdOnlyList = new List<string>();
        public void FillLecturersOnly() // TIK destytojai
        {
            string query = "SELECT username FROM 2praktika.users WHERE accesslevel='2';";
            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                tempLecturersOnlyList.Add(dataReader["username"].ToString()); // destytojo vardas
            }
            CloseConnection();
        }
        public void FillLecturesOnly() // TIK destytojai
        {
            string query = "SELECT name FROM 2praktika.lectures;";
            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                tempLecturesOnlyList.Add(dataReader["name"].ToString()); // paskaitos vardas
            }
            CloseConnection();
        }
        public void FillLecturesIdOnly() // paskaitu ids
        {
            string query = "SELECT lectureid FROM 2praktika.lectures;";
            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                tempLecturesIdOnlyList.Add(dataReader["lectureid"].ToString());
            }
            CloseConnection();
        }
        public void FillLecturersIdOnly() // destytoju ids
        {
            string query = "SELECT usersid FROM 2praktika.users WHERE accesslevel='2';";
            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                tempLecturersIdOnlyList.Add(dataReader["usersid"].ToString());
            }
            CloseConnection();
        }
        public void AddLecturerRelation(string lecturerid, string lecturesid) // prideda sarysi
        {
            string query = "INSERT INTO 2praktika.lecturers (lecturerid,lecturesid) VALUE(@lecturerid,@lecturesid);";
            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@lecturerid", lecturerid);
            cmd.Parameters.AddWithValue("@lecturesid", lecturesid);
            cmd.ExecuteNonQuery();
            CloseConnection();
        }

        // dar du listai, kad ids perduoti santykiu
        public List<string> tempLecturerIdRelationship = new List<string>();
        public List<string> tempLecturesIdRelationship = new List<string>();
        public List<string> tempLecturesRelationshipIdsThemselves = new List<string>();
        public void FillLecturersIdRelationship() // destytoju ids
        {
            string query = "SELECT lecturerid FROM 2praktika.lecturers ORDER BY relationshipid;";
            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                tempLecturerIdRelationship.Add(dataReader["lecturerid"].ToString());
            }
            CloseConnection();
        }
        public void FillLecturesIdRelationship() // destytoju ids
        {
            string query = "SELECT lecturesid FROM 2praktika.lecturers ORDER BY relationshipid;";
            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                tempLecturesIdRelationship.Add(dataReader["lecturesid"].ToString());
            }
            CloseConnection();
        }
        public void FillLecturesRelationshipIdsThemselves() // dar vienas listas, nes 17 ju neuzteko
        {
            string query = "SELECT relationshipid FROM 2praktika.lecturers ORDER BY relationshipid;";
            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                tempLecturesRelationshipIdsThemselves.Add(dataReader["relationshipid"].ToString());
            }
            CloseConnection();
        }
        public void DeleteRelationship(string relationshipid) // istrina relationship
        {
            OpenConnection();
            string query = "DELETE FROM 2praktika.lecturers WHERE relationshipid=@relationshipid;";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@relationshipid", relationshipid);
            cmd.ExecuteNonQuery();
            CloseConnection();
        }

        public void DeleteLecture(string name,string lecturesid) // istrina paskaita
        {
            OpenConnection();
            string query = "DELETE FROM 2praktika.lecturers WHERE lecturesid=@lecturesid; DELETE FROM 2praktika.lectures WHERE name=@name;";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@lecturesid", lecturesid);
            cmd.ExecuteNonQuery();
            CloseConnection();
        }
        public void DeleteLecturer(string usersid) // istrina destytoja
        {
            OpenConnection();
            string query = "DELETE FROM 2praktika.lecturers WHERE lecturerid=@usersid; DELETE FROM 2praktika.studentgrades WHERE studentid=@usersid; DELETE FROM 2praktika.users WHERE usersid=@usersid;";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@lecturerid", usersid);
            cmd.Parameters.AddWithValue("@usersid", usersid);
            cmd.ExecuteNonQuery();
            CloseConnection();
        }
        public void AddLecture(string name) // prideda nauja paskaita
        {
            string query = "INSERT INTO 2praktika.lectures (name) VALUE(@name);";
            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.ExecuteNonQuery();
            CloseConnection();
        }
        public void AddLecturer(string username,string password) // prideda nauja destytoja
        {
            string query = "INSERT INTO 2praktika.users (username,password,accesslevel) VALUE(@username,@password,'2');";
            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password);
            cmd.ExecuteNonQuery();
            CloseConnection();
        }

        #endregion "Admin lecturers tab" ///////////////////////


        #region "Admin groups tab" ///////////////////////


        public List<string> tempList = new List<string>();
        public List<string> tempGroupIdList = new List<string>();
        public void FillGroup() //uzpido lista, kad rodytu grupes visas pas admin
        {
            string query = "SELECT groupname FROM 2praktika.groups;";
            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                tempList.Add(dataReader["groupname"].ToString()); //sukuria laikina lista mysql ir tada ji perduoda i main class
            }
            CloseConnection();
        }
        public void FillGroupIds() // grupiu ids, galvojau be ju apsisuksiu, neisejo ir nuvirtau
        {
            string query = "SELECT groupid FROM 2praktika.groups;";
            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                tempGroupIdList.Add(dataReader["groupid"].ToString());
            }
            CloseConnection();
        }
        public void AddGroup(string groupName) // prideda grupe, pas admina matoma
        {
            OpenConnection();
            string query = "INSERT INTO 2praktika.groups (groupname) VALUE(@groupname);";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@groupname", groupName);
            cmd.ExecuteNonQuery();
            CloseConnection();
        }

        public void DeleteGroup(string groupid)
        {
            OpenConnection();
            string query = "DELETE FROM 2praktika.studentgroups WHERE groupid=@groupid; DELETE FROM 2praktika.lecturegroups WHERE groupid=@groupid; DELETE FROM 2praktika.groups WHERE groupid=@groupid;";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@groupid", groupid);
            cmd.ExecuteNonQuery();
            CloseConnection();
        }

        public List<string> tempStudentList = new List<string>();
        public List<string> tempStudentIdList = new List<string>();
        public void FillStudentsList() // uzpildo studentu sarasa
        {
            string query = "SELECT username FROM 2praktika.users WHERE accesslevel='3' ORDER BY usersid;";
            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                tempStudentList.Add(dataReader["username"].ToString());
            }
            CloseConnection();
        }
        public void FillStudentsIdList() // uzpildo studentu ids sarasa
        {
            string query = "SELECT usersid FROM 2praktika.users WHERE accesslevel='3' ORDER BY usersid;";
            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                tempStudentIdList.Add(dataReader["usersid"].ToString());
            }
            CloseConnection();
        }

        public void KillStudent(string usersid) // :^)
        {
            OpenConnection();
            string query = "DELETE FROM 2praktika.studentgroups WHERE studentid=@usersid; DELETE FROM 2praktika.studentgrades WHERE studentid=@usersid; DELETE FROM 2praktika.users WHERE usersid=@usersid;";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@usersid", usersid);
            cmd.ExecuteNonQuery();
            CloseConnection();
        }

        public List<string> tempStudentNameBigList = new List<string>();
        public List<string> tempStudentIdBigList = new List<string>();
        public List<string> tempStudentGroupIdBigList = new List<string>();
        public List<string> tempStudentGroupBigList = new List<string>();
        public List<string> tempStudentRelationshipBigList = new List<string>();
        public void FillFullStudent() // big bad wolf, didzioji
        {
            string query = "SELECT users.username,groups.groupname FROM 2praktika.users JOIN 2praktika.studentgroups ON users.usersid=studentgroups.studentid JOIN 2praktika.groups ON groups.groupid=studentgroups.groupid ORDER BY relationshipid;";
            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                tempStudentNameBigList.Add(dataReader["username"].ToString()); // studento vardas
                tempStudentGroupBigList.Add(dataReader["groupname"].ToString()); // grupes vardas
            }
            CloseConnection();
        }
        public void FillFullStudentGroupIds() // didelis bet su grupemis
        {
            string query = "SELECT * FROM 2praktika.studentgroups;";
            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                tempStudentIdBigList.Add(dataReader["studentid"].ToString()); // studento vardas
                tempStudentGroupIdBigList.Add(dataReader["groupid"].ToString()); // grupes vardas
                tempStudentRelationshipBigList.Add(dataReader["relationshipid"].ToString()); // studento relationship id
            }
            CloseConnection();
        }
        public void AddStudent(string username, string password) // prideda nauja studenta
        {
            string query = "INSERT INTO 2praktika.users (username,password,accesslevel) VALUE(@username,@password,'3');";
            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password);
            cmd.ExecuteNonQuery();
            CloseConnection();
        }

        public void AddStudentGroupRelation(string studentid, string groupid) // prideda sarysi
        {
            string query = "INSERT INTO 2praktika.studentgroups (studentid,groupid) VALUE(@studentid,@groupid);";
            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@studentid", studentid);
            cmd.Parameters.AddWithValue("@groupid", groupid);
            cmd.ExecuteNonQuery();
            CloseConnection();
        }

        public List<string> tempGroupNamesLGList = new List<string>();
        public List<string> tempLecturesNamesLGList = new List<string>();
        public List<string> tempRelationshipIdLGList = new List<string>();
        public List<string> tempGroupIdLGList = new List<string>();
        public List<string> tempLectureIdLGList = new List<string>();
        public void FillFullGroups() // didele grupiu, varda paskaitos ir varda grupes uzpildo i listus
        {
            string query = "SELECT lectures.name,groups.groupname FROM 2praktika.lectures JOIN 2praktika.lecturegroups ON lectures.lectureid=lecturegroups.lectureid JOIN 2praktika.groups ON groups.groupid=lecturegroups.groupid ORDER BY relationshipid;";
            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                tempLecturesNamesLGList.Add(dataReader["name"].ToString()); // studento vardas
                tempGroupNamesLGList.Add(dataReader["groupname"].ToString()); // grupes vardas
            }
            CloseConnection();
        }

        public void FillFullGroupIds() // visus ids uzpildo
        {
            string query = "SELECT * FROM 2praktika.lecturegroups ORDER BY relationshipid;";
            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                tempGroupIdLGList.Add(dataReader["groupid"].ToString()); // groupid
                tempLectureIdLGList.Add(dataReader["lectureid"].ToString()); // lectureid
                tempRelationshipIdLGList.Add(dataReader["relationshipid"].ToString()); // relationshipid
            }
            CloseConnection();
        }
        public void AddLectureGroupRelation(string lectureid, string groupid) // prideda sarysi
        {
            string query = "INSERT INTO 2praktika.lecturegroups (lectureid,groupid) VALUE(@lectureid,@groupid);";
            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@lectureid", lectureid);
            cmd.Parameters.AddWithValue("@groupid", groupid);
            cmd.ExecuteNonQuery();
            CloseConnection();
        }

        public void DeleteLgRelationship(string relationshipid)
        {
            string query = "DELETE FROM 2praktika.lecturegroups WHERE relationshipid=@relationshipid";
            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@relationshipid", relationshipid);
            cmd.ExecuteNonQuery();
            CloseConnection();
        }

        public void DeleteStudentGroupRelationship(string relationshipid)
        {
            string query = "DELETE FROM 2praktika.studentgroups WHERE relationshipid=@relationshipid";
            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@relationshipid", relationshipid);
            cmd.ExecuteNonQuery();
            CloseConnection();
        }
        #endregion "Admin groups tab" ///////////////////////


        #region "Lecturer panel" ///////////////////////

        public List<string> tempGradesRelationshipIdList = new List<string>();
        public List<string> tempGradesStudentsIdList = new List<string>();
        public List<string> tempGradesLectureIdList = new List<string>();
        public List<string> tempGradesList = new List<string>();

        public List<string> tempGradesStudentsNamesList = new List<string>();
        public List<string> tempGradesLecturesNamesList = new List<string>();
        public void FillGradesInfo()
        {
            string query = "SELECT * FROM 2praktika.studentgrades WHERE studentgrades.lectureid=@lectureid ORDER BY relationshipid;";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            foreach (var item in tempMyLecturesIdList)
            {
                OpenConnection();
                cmd.Parameters.AddWithValue("@lectureid", item);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    tempGradesRelationshipIdList.Add(dataReader["relationshipid"].ToString()); // relationship
                    tempGradesStudentsIdList.Add(dataReader["studentid"].ToString()); // student id
                    tempGradesLectureIdList.Add(dataReader["lectureid"].ToString()); // lecture id
                    tempGradesList.Add(dataReader["grade"].ToString()); // pazymys
                }
                cmd.Parameters.Clear();
                CloseConnection();
            }
        }

        public void FillGradesNameInfo() // isrenka vardus bei paskaitu vardus TIK TUOS KURIE TAM DESTYTOJUI PRIKLAUSO
        {
            string query = "SELECT users.username,lectures.name FROM 2praktika.studentgrades JOIN 2praktika.users ON users.usersid=studentgrades.studentid JOIN 2praktika.lectures ON lectures.lectureid=studentgrades.lectureid WHERE studentgrades.lectureid=@lectureid ORDER BY relationshipid;"; // dieve apsaugok mane kad nereiktu tokio vel rasyt
            MySqlCommand cmd = new MySqlCommand(query, connection);
            foreach (var item in tempMyLecturesIdList)
            {
                OpenConnection();
                cmd.Parameters.AddWithValue("@lectureid", item);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    tempGradesStudentsNamesList.Add(dataReader["username"].ToString()); //
                    tempGradesLecturesNamesList.Add(dataReader["name"].ToString()); // 
                }
                cmd.Parameters.Clear();
                CloseConnection();
            }
        }

        public List<string> tempMyLecturesList = new List<string>();
        public List<string> tempMyLecturesIdList = new List<string>();
        public void FillMyLectures(string lecturerid)
        {
            string query = "SELECT lectures.name FROM 2praktika.lecturers JOIN 2praktika.lectures ON lecturers.lecturesid=lectures.lectureid WHERE lecturerid=@lecturerid ORDER BY lecturerid;";
            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@lecturerid", lecturerid);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                tempMyLecturesList.Add(dataReader["name"].ToString()); 
            }
            CloseConnection();
        }

        public void FillMyLecturesId(string lecturerid)
        {
            string query = "SELECT lecturesid FROM 2praktika.lecturers WHERE lecturerid=@lecturerid ORDER BY lecturerid;";
            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@lecturerid", lecturerid);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                tempMyLecturesIdList.Add(dataReader["lecturesid"].ToString());
            }
            CloseConnection();
        }

        public void ChangeGrade(string grade,string relationshipid)
        {
            string query = "UPDATE 2praktika.studentgrades SET grade=@grade WHERE relationshipid=@relationshipid;";
            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@grade", grade);
            cmd.Parameters.AddWithValue("@relationshipid", relationshipid);
            cmd.ExecuteNonQuery();
            CloseConnection();
        }

        public List<string> tempMyStudentsNamesList = new List<string>();
        public List<string> tempMyStudentsIdsList = new List<string>();
        public List<string> tempMyStudentsGroupsNamesList = new List<string>();
        public List<string> tempMySTudentsGroupsIdList = new List<string>();

        /*public void FillMyStudentsList()
        {
            string query = "SELECT * FROM 2praktika.users WHERE usersid IN (SELECT studentid FROM 2praktika.studentgroups WHERE groupid IN (SELECT groupid FROM 2praktika.lecturegroups WHERE lectureid IN (SELECT lecturesid FROM 2praktika.lecturers WHERE lecturerid=@id)));"; // teti gelbek
            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                tempMyStudentsNamesList.Add(dataReader["username"].ToString());
                tempMyStudentsIdsList.Add(dataReader["usersid"].ToString());
            }
            CloseConnection();
        }*/
        /*public void FillMyStudentsGroupsList()
        {
            string query = "SELECT groupid FROM 2praktika.studentgroups WHERE studentid IN (SELECT usersid FROM 2praktika.users WHERE usersid IN (SELECT studentid FROM 2praktika.studentgroups WHERE groupid IN (SELECT groupid FROM 2praktika.lecturegroups WHERE lectureid IN (SELECT lecturesid FROM 2praktika.lecturers WHERE lecturerid=@id))));"; // dievas jau nebeisgelbes
            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                tempMySTudentsGroupsIdList.Add(dataReader["groupid"].ToString());
            }
            CloseConnection();
        }*/
        /*public void FillMyStudentsGroupsNamesList()
        {
            string query = "SELECT groupname FROM 2praktika.groups WHERE groupid IN (SELECT groupid FROM 2praktika.studentgroups WHERE studentid IN (SELECT usersid FROM 2praktika.users WHERE usersid IN (SELECT studentid FROM 2praktika.studentgroups WHERE groupid IN (SELECT groupid FROM 2praktika.lecturegroups WHERE lectureid IN (SELECT lecturesid FROM 2praktika.lecturers WHERE lecturerid=@id)))));"; // dievas jau nebeisgelbes
            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                tempMyStudentsGroupsNamesList.Add(dataReader["groupname"].ToString());
            }
            CloseConnection();
        }*/
        public List<string> tempMyStudentsLecturesIdList = new List<string>();
        /*public void FillMyStudentsLecturesIdList()
        {
            string query = "SELECT lectureid FROM 2praktika.lecturegroups WHERE groupid IN (SELECT groupid FROM 2praktika.studentgroups WHERE studentid IN (SELECT usersid FROM 2praktika.users WHERE usersid IN (SELECT studentid FROM 2praktika.studentgroups WHERE groupid IN (SELECT groupid FROM 2praktika.lecturegroups WHERE lectureid IN (SELECT lecturesid FROM 2praktika.lecturers WHERE lecturerid=@id)))));"; // :-(
            OpenConnection(); // nesugalvojau kaip kitaip, o jau veluoju priduoti, GAL KAZKAIP LENGVIAU RASIU !! TODO !!
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                tempMyStudentsLecturesIdList.Add(dataReader["lectureid"].ToString());
            }
            CloseConnection();
        }*/
        
        public List<string> tempMyStudentsLecturesNamesList = new List<string>();
        public void FillMyStudentsLectures()
        {
            string query = "select users.username,users.usersid,groups.groupid,groups.groupname,lectures.lectureid,lectures.name             from 2praktika.lectures join 2praktika.lecturers on lecturers.lecturesid = lectures.lectureid join 2praktika.lecturegroups on lecturegroups.lectureid = lectures.lectureid join 2praktika.groups on groups.groupid = lecturegroups.groupid join 2praktika.studentgroups on studentgroups.groupid = lecturegroups.groupid join 2praktika.users on studentgroups.studentid = users.usersid where lecturers.lecturerid=@id; "; // :-(
            OpenConnection(); // berods paskutinis is visos sitos litanijos
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                tempMyStudentsIdsList.Add(dataReader["usersid"].ToString());
                tempMyStudentsNamesList.Add(dataReader["username"].ToString());
                tempMyStudentsLecturesIdList.Add(dataReader["lectureid"].ToString());
                tempMyStudentsLecturesNamesList.Add(dataReader["name"].ToString());
                tempMySTudentsGroupsIdList.Add(dataReader["groupid"].ToString());
                tempMyStudentsGroupsNamesList.Add(dataReader["groupname"].ToString());
            }
            CloseConnection();
        }

        public void AddNewGrade(string usersid, string grade, string lectureid)
        {
            string query = "INSERT INTO 2praktika.studentgrades (studentid,grade,lectureid) VALUE(@usersid,@grade,@lectureid);";
            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@usersid", usersid);
            cmd.Parameters.AddWithValue("@grade", grade);
            cmd.Parameters.AddWithValue("@lectureid", lectureid);
            cmd.ExecuteNonQuery();
            CloseConnection();
        }
        public void DeleteGrade(string relationshipid)
        {
            string query = "DELETE FROM 2praktika.studentgrades WHERE relationshipid=@relationshipid";
            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@relationshipid", relationshipid);
            cmd.ExecuteNonQuery();
            CloseConnection();
        }

        #endregion "Lecturer panel" ///////////////////////

        #region "Student panel" ///////////////////////
        public List<string> tempStudentGradesList = new List<string>();
        public void FillStudentGradesList()
        {
            string query = "SELECT grade FROM 2praktika.studentgrades WHERE studentid=@studentid ORDER BY relationshipid;";
            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@studentid", id);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                tempStudentGradesList.Add(dataReader["grade"].ToString());
            }
            CloseConnection();
        }
        public List<string> tempStudentLecturesList = new List<string>();
        public void FillStudentLecturesList()
        {
            string query = "SELECT name FROM 2praktika.lectures WHERE lectureid IN (SELECT lectureid FROM 2praktika.studentgrades WHERE studentid=@studentid ORDER BY relationshipid);";
            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@studentid", id);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                tempStudentLecturesList.Add(dataReader["name"].ToString());
            }
            CloseConnection();
        }

        #endregion "Student panel" ///////////////////////
    }
}