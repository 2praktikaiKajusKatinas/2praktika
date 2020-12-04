using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;

namespace Praktika2
{
    public partial class Main : Form
    {
        Mysql mysql = new Mysql();
        public Main()
        {
            InitializeComponent();
        }

        #region "Login panel" ////////////////////
        private void LoginButton_Click(object sender, EventArgs e)
        {
            mysql = new Mysql();
            string accesslevel;
            accesslevel = mysql.Login(UsernameText.Text,PasswordText.Text);
            switch (accesslevel)
            {
                case "1":
                    LoginPanel.Visible = false;
                    AdminPanel.Visible = true;
                    RefreshAll();
                    break;
                case "2":
                    LoginPanel.Visible = false;
                    LecturerPanel.Visible = true;
                    MyIdLabel.Text = mysql.id;
                    MyNameLabel.Text = mysql.myname;
                    RefreshAll();
                    break;
                case "3":
                    LoginPanel.Visible = false;
                    D.Visible = true;
                    StudentIdLabelFill.Text = mysql.id;
                    StudentMyNameLabel.Text = mysql.myname;
                    RefreshAll();
                    break;
                default:
                    MessageBox.Show("Please enter your credentials properly");
                    break;

            }
            UsernameText.Text = null;
            PasswordText.Text = null;
        }
        #endregion "Login panel" ////////////////////



        #region "Admin panel" ////////////////////

        #region "Groups tab" /////////////////////

        private void AddButton_Click(object sender, EventArgs e) // prideda grupe
        {
            if(String.IsNullOrWhiteSpace(NewGroupText.Text))
            {
                MessageBox.Show("Please enter something first.");
            }
            else
            {
                mysql.AddGroup(NewGroupText.Text);
                RefreshAll();
                NewGroupText.Clear();
            }
        }
        private void DeleteGroupButton_Click(object sender, EventArgs e) // istrina grupe
        {
            if (String.IsNullOrWhiteSpace(SelectedGroupLabel.Text))
            {
                MessageBox.Show("Please select something first.");
            }
            else
            {
                foreach (ListViewItem eachItem in AdminGroupList.SelectedItems)
                {
                    AdminGroupList.Items.Remove(eachItem);
                    mysql.DeleteGroup(eachItem.SubItems[1].Text);
                }
                RefreshAll();
                SelectedGroupLabel.Text = string.Empty;
                SelectedGroupIdLabel.Text = string.Empty;
            }
        }
        private void AdminGroupList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AdminGroupList.SelectedItems.Count > 0)
            {
                ListViewItem itema = AdminGroupList.SelectedItems[0];
                SelectedGroupLabel.Text = itema.SubItems[0].Text;
                SelectedGroupIdLabel.Text = itema.SubItems[1].Text; 
            }
            else
            {
                SelectedGroupLabel.Text = string.Empty;
                SelectedGroupIdLabel.Text = string.Empty;
            }
        }

        private void SimpleStudentList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SimpleStudentList.SelectedItems.Count > 0)
            {
                ListViewItem itema = SimpleStudentList.SelectedItems[0];
                SelectedStudentLabel.Text = itema.SubItems[0].Text;
                StudentIdLabel.Text = itema.SubItems[1].Text;
            }
            else
            {
                SelectedStudentLabel.Text = string.Empty;
                StudentIdLabel.Text = string.Empty;
            }
        }

        private void DeleteStudentButton_Click(object sender, EventArgs e)
        {
            if(String.IsNullOrWhiteSpace(SelectedStudentLabel.Text))
            {
                MessageBox.Show("Please enter something first.");
            }
            else
            {
                mysql.KillStudent(StudentIdLabel.Text);
                RefreshAll();
                SelectedStudentLabel.Text = string.Empty;
                StudentIdLabel.Text = string.Empty;                
            }
        }

        private void AddNewStudentButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(NewStudentText.Text))
            {
                MessageBox.Show("Please enter something first.");
            }
            else
            {
                string text = NewStudentText.Text;
                string[] textSplit = text.Split(' ');
                mysql.AddStudent(textSplit[0], textSplit[1]);
                RefreshAll();
                NewStudentText.Clear();
            }

        }

        private void AddGroupStudentRelationshipButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(StudentIdLabel.Text) || String.IsNullOrWhiteSpace(SelectedGroupIdLabel.Text))
            {
                MessageBox.Show("Please select something first.");
            }
            else
            {
                mysql.AddStudentGroupRelation(StudentIdLabel.Text,SelectedGroupIdLabel.Text);
                RefreshAll();
            }
        }



        #endregion "Groups tab" /////////////////////







        #region "Lecturers tab" /////////////////////
        private void LogoutButton_Click(object sender, EventArgs e)
        {
            AdminPanel.Visible = false;
            LoginPanel.Visible = true;
        }


        private void DeleteLGRelationshipButton_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem eachItem in FullLgList.SelectedItems)
            {
                FullLgList.Items.Remove(eachItem);
                mysql.DeleteLgRelationship(eachItem.Text);
            }
            RefreshAll();
        }

        private void DeleteStudentGroupRelationshipButton_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem eachItem in FullStudentList.SelectedItems)
            {
                FullStudentList.Items.Remove(eachItem);
                mysql.DeleteStudentGroupRelationship(eachItem.Text);
            }
            RefreshAll();
        }

        private void LecturerSimpleList_SelectedIndexChanged(object sender, EventArgs e) // kad pasirinktu ir matytu pasirinkima atskirai
        {
            if (LecturerSimpleList.SelectedItems.Count > 0)
            {
                ListViewItem item = LecturerSimpleList.SelectedItems[0];
                LecturerLable.Text = item.SubItems[0].Text;
                LecturersIdLabel.Text = item.SubItems[1].Text;
                SelectedLecturerLabel.Text = item.SubItems[0].Text;
            }
            else
            {
                LecturerLable.Text = string.Empty;
                LecturersIdLabel.Text = string.Empty;
                SelectedLecturerLabel.Text = string.Empty;
            }

        }

        private void LecturesSimpleList_SelectedIndexChanged(object sender, EventArgs e) // tas pats kas virsuje tik su salia esanciu
        {
            if (LecturesSimpleList.SelectedItems.Count > 0)
            {
                ListViewItem itema = LecturesSimpleList.SelectedItems[0];
                LecturesLabel.Text = itema.SubItems[0].Text;    
                LecturesIdLabel.Text = itema.SubItems[1].Text;
                SelectedLectureLabel.Text = itema.SubItems[0].Text;
                SelectedLectureLGLabel.Text = itema.SubItems[0].Text;
                SelectedLectureIDLGLabel.Text = itema.SubItems[1].Text;
            }
            else
            {
                LecturesLabel.Text = string.Empty;
                LecturesIdLabel.Text = string.Empty;
                SelectedLectureLabel.Text = string.Empty;
                SelectedLectureLGLabel.Text = string.Empty;
                SelectedLectureIDLGLabel.Text = string.Empty;
            }
        }
        private void DeleteLectureButton_Click(object sender, EventArgs e) // istrina pacia paskaita
        {
            mysql.DeleteLecture(SelectedLectureLabel.Text, LecturesIdLabel.Text);
            RefreshAll();
            LecturesLabel.Text = string.Empty;
            LecturesIdLabel.Text = string.Empty;
            SelectedLectureLabel.Text = string.Empty;
            SelectedLectureLGLabel.Text = string.Empty;
            SelectedLectureIDLGLabel.Text = string.Empty;
        }
        private void AdminLecturesAssignButton_Click(object sender, EventArgs e) // priskiria destytoja prie paskaitos duombazeje
        {
            if (String.IsNullOrWhiteSpace(LecturerLable.Text) || String.IsNullOrWhiteSpace(LecturesLabel.Text))
            {
                MessageBox.Show("Please select something first.");
            }
            else
            {
                mysql.AddLecturerRelation(LecturersIdLabel.Text,LecturesIdLabel.Text);
                RefreshAll();
                LecturerLable.Text = string.Empty;
                LecturersIdLabel.Text = string.Empty;
                LecturesLabel.Text = string.Empty;
                LecturesIdLabel.Text = string.Empty;
            }
        }

        private void DeleteRelationshipButton_Click(object sender, EventArgs e) // istrina destytojas:paskaita santyki
        {
            foreach (ListViewItem eachItem in AdminLecturesList.SelectedItems)
            {
                AdminLecturesList.Items.Remove(eachItem);
                mysql.DeleteRelationship(eachItem.Text);
            }
            RefreshAll();
        }
        private void AddNewLectureButton_Click(object sender, EventArgs e) // prideda nauja paskaita i sarasa
        {
            if (String.IsNullOrWhiteSpace(NewLectureText.Text))
            {
                MessageBox.Show("Please select something first.");
            }
            else
            {
                mysql.AddLecture(NewLectureText.Text);
                RefreshAll();
                NewLectureText.Clear();
            }
        }
        private void AddNewLecturerButton_Click(object sender, EventArgs e) // prideda nauja destytoja
        {
            if (String.IsNullOrWhiteSpace(NewLecturerText.Text))
            {
                MessageBox.Show("Please select something first.");
            }
            else
            {
                string text = NewLecturerText.Text;
                string[] textSplit = text.Split(' ');
                mysql.AddLecturer(textSplit[0],textSplit[1]);
                RefreshAll();
                NewLecturerText.Clear();
            }
        }
        private void DeleteLecturerButton_Click(object sender, EventArgs e) // istrina destytoja is egzistencijos
        {
            if (String.IsNullOrWhiteSpace(LecturersIdLabel.Text))
            {
                MessageBox.Show("Please select something first.");
            }
            else
            {
                mysql.DeleteLecturer(LecturersIdLabel.Text);
                RefreshAll();
            }
        }

        private void GroupsLGList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GroupsLGList.SelectedItems.Count > 0)
            {
                ListViewItem itema = GroupsLGList.SelectedItems[0];
                SelectedGroupIdLGLabel.Text = itema.SubItems[1].Text;
                SelectedGroupLGLabel.Text = itema.SubItems[0].Text;
            }
            else
            {
                SelectedGroupIdLGLabel.Text = string.Empty;
                SelectedGroupLGLabel.Text = string.Empty;
            }
        }


        private void AssignLectureGroupButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(SelectedLectureLGLabel.Text) || String.IsNullOrWhiteSpace(SelectedGroupLGLabel.Text))
            {
                MessageBox.Show("Please select something first.");
            }
            else
            {
                mysql.AddLectureGroupRelation(SelectedLectureIDLGLabel.Text,SelectedGroupIdLGLabel.Text);
                RefreshAll();
                SelectedGroupIdLGLabel.Text = string.Empty;
                SelectedGroupLGLabel.Text = string.Empty;
                SelectedLectureIDLGLabel.Text = string.Empty;
                SelectedLectureLGLabel.Text = string.Empty;
            }
        }




        #endregion "Lecturers tab" /////////////////////

        #endregion "Admin panel" ////////////////////



        #region "Lecturer panel" ////////////////////
        private void LecturerLogoutButton_Click(object sender, EventArgs e)
        {
            LecturerPanel.Visible = false;
            LoginPanel.Visible = true;
        }





        #endregion "Lecturer panel" ////////////////////



        #region "Student panel" ////////////////////
        private void StudentLogoutButton_Click(object sender, EventArgs e)
        {
            D.Visible = false;
            LoginPanel.Visible = true;
        }


        #endregion "Student panel" ////////////////////



        #region "Misc" ////////////////////

        public void RefreshAll() // istrina viska ir tada vel sugeneruoja, spageciu kodas
        {
            ClearAllLists();
            GenerateGroups();
        }
        public void GenerateGroups() // viska sugeneruoja
        {
            // groups
            mysql.FillGroup();
            mysql.FillGroupIds();
            mysql.FillStudentsIdList();
            mysql.FillStudentsList();
            mysql.FillFullStudent();
            mysql.FillFullStudentGroupIds();
            mysql.FillFullGroups();
            mysql.FillFullGroupIds();



            for (int i = 0; i < mysql.tempList.Count; i++) // uzpildo grupes ir ju ids
            {
                ListViewItem abc = new ListViewItem(mysql.tempList[i]);
                abc.SubItems.Add(mysql.tempGroupIdList[i]);
                AdminGroupList.Items.Add(abc);
            }
            for (int i = 0; i < mysql.tempList.Count; i++) // uzpildo grupes ir ju ids kitame tabe
            {
                ListViewItem abc = new ListViewItem(mysql.tempList[i]);
                abc.SubItems.Add(mysql.tempGroupIdList[i]);
                GroupsLGList.Items.Add(abc);
            }
            for (int i = 0; i < mysql.tempStudentList.Count; i++) // uzpildo studentus ir ju ids
            {
                ListViewItem abc = new ListViewItem(mysql.tempStudentList[i]);
                abc.SubItems.Add(mysql.tempStudentIdList[i]);
                SimpleStudentList.Items.Add(abc);
            }
            for (int i = 0; i < mysql.tempStudentRelationshipBigList.Count; i++) // uzpildo studentus ir ju ids ir grupes bei ju ids ir relationship ids ir ten viska ko tik sirdis geidzia
            {
                ListViewItem abc = new ListViewItem(mysql.tempStudentRelationshipBigList[i]);
                abc.SubItems.Add(mysql.tempStudentNameBigList[i]);
                abc.SubItems.Add(mysql.tempStudentIdBigList[i]);
                abc.SubItems.Add(mysql.tempStudentGroupBigList[i]);
                abc.SubItems.Add(mysql.tempStudentGroupIdBigList[i]);
                FullStudentList.Items.Add(abc);
            }
            for (int i = 0; i < mysql.tempRelationshipIdLGList.Count; i++) // ilgas group relationship dalykas
            {
                ListViewItem abc = new ListViewItem(mysql.tempRelationshipIdLGList[i]);
                abc.SubItems.Add(mysql.tempLecturesNamesLGList[i]);
                abc.SubItems.Add(mysql.tempLectureIdLGList[i]);
                abc.SubItems.Add(mysql.tempGroupNamesLGList[i]);
                abc.SubItems.Add(mysql.tempGroupIdLGList[i]);
                FullLgList.Items.Add(abc);
            }
            

            // lectures
            mysql.FillLectures();
            mysql.FillLecturersOnly();
            mysql.FillLecturesOnly();
            mysql.FillLecturersIdOnly();
            mysql.FillLecturesIdOnly();
            mysql.FillLecturersIdRelationship();
            mysql.FillLecturesIdRelationship();
            mysql.FillLecturesRelationshipIdsThemselves();
            mysql.FillLecturesIdRelationship();
            for (int i = 0; i < mysql.tempLectureList.Count; i++) // uzpildo lecturers, ju id, ir lectures, bei ju id, virsuje
            {
                ListViewItem abc = new ListViewItem(mysql.tempLecturesRelationshipIdsThemselves[i]);
                abc.SubItems.Add(mysql.tempLectureList[i]);
                abc.SubItems.Add(mysql.tempLecturerIdRelationship[i]);
                abc.SubItems.Add(mysql.tempLectureList2[i]);
                abc.SubItems.Add(mysql.tempLecturesIdRelationship[i]);
                AdminLecturesList.Items.Add(abc);
            }
            for (int i = 0; i < mysql.tempLecturersOnlyList.Count; i++) // uzpildo pirmaja lentele su destytojais ir ids
            {
                ListViewItem abc = new ListViewItem(mysql.tempLecturersOnlyList[i]);
                abc.SubItems.Add(mysql.tempLecturersIdOnlyList[i]);
                LecturerSimpleList.Items.Add(abc);
            }
            for (int i = 0; i < mysql.tempLecturesOnlyList.Count; i++) // uzpildo antraja lentele su paskaitomis ir ju ids
            {
                ListViewItem abc = new ListViewItem(mysql.tempLecturesOnlyList[i]);
                abc.SubItems.Add(mysql.tempLecturesIdOnlyList[i]);
                LecturesSimpleList.Items.Add(abc);
            }

            // destytojo view
            mysql.FillMyLectures(mysql.id);
            mysql.FillMyLecturesId(mysql.id);
            mysql.FillGradesInfo();
            mysql.FillGradesNameInfo();
            mysql.FillMyStudentsLectures();
            /*mysql.FillMyStudentsList();
            mysql.FillMyStudentsGroupsList();
            mysql.FillMyStudentsLecturesIdList();
            mysql.FillMyStudentsLecturesNames();
            mysql.FillMyStudentsLecturesIdList();
            mysql.FillMyStudentsGroupsNamesList();*/ // perdariau, nes sitie visi neveike :(
            for (int i = 0; i < mysql.tempGradesRelationshipIdList.Count; i++)
            {
                ListViewItem abc = new ListViewItem(mysql.tempGradesRelationshipIdList[i]);
                abc.SubItems.Add(mysql.tempGradesStudentsNamesList[i]);
                abc.SubItems.Add(mysql.tempGradesStudentsIdList[i]);
                abc.SubItems.Add(mysql.tempGradesLecturesNamesList[i]);
                abc.SubItems.Add(mysql.tempGradesLectureIdList[i]);
                abc.SubItems.Add(mysql.tempGradesList[i]);
                LecturerFullList.Items.Add(abc);
            }
            for (int i = 0; i < mysql.tempMyLecturesList.Count; i++)
            {
                ListViewItem abc = new ListViewItem(mysql.tempMyLecturesList[i]);
                abc.SubItems.Add(mysql.tempMyLecturesIdList[i]);
                MyLecturesList.Items.Add(abc);
            }
            for (int i = 0; i < mysql.tempMyStudentsNamesList.Count; i++)
            {
                ListViewItem abc = new ListViewItem(mysql.tempMyStudentsIdsList[i]);
                abc.SubItems.Add(mysql.tempMyStudentsNamesList[i]);
                abc.SubItems.Add(mysql.tempMyStudentsLecturesIdList[i]);
                abc.SubItems.Add(mysql.tempMyStudentsLecturesNamesList[i]);
                abc.SubItems.Add(mysql.tempMySTudentsGroupsIdList[i]);
                abc.SubItems.Add(mysql.tempMyStudentsGroupsNamesList[i]);
                MyStudentList.Items.Add(abc);
            }

            // studento view
            mysql.FillStudentLecturesList();
            mysql.FillStudentGradesList();
            for (int i = 0; i < mysql.tempStudentLecturesList.Count; i++)
            {
                ListViewItem abc = new ListViewItem(mysql.tempStudentLecturesList[i]);
                abc.SubItems.Add(mysql.tempStudentGradesList[i]);
                StudentGradesList.Items.Add(abc);
            }
        }
        public void ClearAllLists() //refreshina listus, kad nebutu taip, kad du kartus ta pati irasa i viena lista imeta
        {
            // lectures

            AdminLecturesList.Items.Clear();
            LecturerSimpleList.Items.Clear();
            LecturesSimpleList.Items.Clear();
            mysql.tempLectureList.Clear();
            mysql.tempLectureList2.Clear();
            mysql.tempLecturerIdRelationship.Clear();
            mysql.tempLecturersIdOnlyList.Clear();
            mysql.tempLecturersOnlyList.Clear();
            mysql.tempLecturesIdOnlyList.Clear();
            mysql.tempLecturesIdRelationship.Clear();
            mysql.tempLecturesOnlyList.Clear();
            mysql.tempLecturesRelationshipIdsThemselves.Clear();
            LecturesLabel.Text = string.Empty;
            LecturersIdLabel.Text = string.Empty;
            SelectedLectureLabel.Text = string.Empty;


            // grupes
            AdminGroupList.Items.Clear();
            SimpleStudentList.Items.Clear();
            FullStudentList.Items.Clear();
            GroupsLGList.Items.Clear();
            FullLgList.Items.Clear();
            mysql.tempList.Clear();
            mysql.tempStudentIdList.Clear();
            mysql.tempStudentList.Clear();
            mysql.tempStudentGroupBigList.Clear();
            mysql.tempStudentGroupIdBigList.Clear();
            mysql.tempStudentIdBigList.Clear();
            mysql.tempStudentNameBigList.Clear();
            mysql.tempStudentRelationshipBigList.Clear();
            mysql.tempGroupIdList.Clear();

            mysql.tempGroupIdLGList.Clear();
            mysql.tempGroupNamesLGList.Clear();
            mysql.tempLectureIdLGList.Clear();
            mysql.tempLecturesNamesLGList.Clear();
            mysql.tempRelationshipIdLGList.Clear();

            // destytojo view
            LecturerFullList.Items.Clear();
            MyLecturesList.Items.Clear();
            MyStudentList.Items.Clear();
            mysql.tempGradesLectureIdList.Clear();
            mysql.tempGradesLecturesNamesList.Clear();
            mysql.tempGradesList.Clear();
            mysql.tempGradesRelationshipIdList.Clear();
            mysql.tempGradesStudentsIdList.Clear();
            mysql.tempGradesStudentsNamesList.Clear();
            mysql.tempMyLecturesIdList.Clear();
            mysql.tempMyLecturesList.Clear();
            mysql.tempMyStudentsNamesList.Clear();
            mysql.tempMyStudentsIdsList.Clear();
            mysql.tempMySTudentsGroupsIdList.Clear();
            mysql.tempMyStudentsGroupsNamesList.Clear();
            mysql.tempMyStudentsLecturesIdList.Clear();
            mysql.tempMyStudentsLecturesNamesList.Clear();
            
            // studento view
            StudentGradesList.Items.Clear();
            mysql.tempStudentGradesList.Clear();
            mysql.tempStudentLecturesList.Clear();
        }





        #endregion

        private void ChangeGradeButton_Click(object sender, EventArgs e)
        {
            if (Regex.IsMatch(GradeText.Text,"[0-9]"))
            {
                if (String.IsNullOrWhiteSpace(GradeText.Text) || Int32.Parse(GradeText.Text)>=10 || Int32.Parse(GradeText.Text)<=1)
                {
                    MessageBox.Show("Please enter something sensible first.");
                }
                else
                {
                    ListViewItem itema = LecturerFullList.SelectedItems[0];
                    string tempItem = itema.SubItems[0].Text;
                    mysql.ChangeGrade(GradeText.Text,tempItem);
                    GradeText.Text = string.Empty;
                    RefreshAll();
                }
            }
            else
            {
                MessageBox.Show("Please enter numbers only.");
            }
        }

        private void NewGradeButton_Click(object sender, EventArgs e)
        {
            if (Regex.IsMatch(NewGradeText.Text, "[0-9]"))
            {
                if (String.IsNullOrWhiteSpace(NewGradeText.Text) || Int32.Parse(NewGradeText.Text) >= 10 || Int32.Parse(NewGradeText.Text) <= 1)
                {
                    MessageBox.Show("Please enter something sensible first.");
                }
                else
                {
                    ListViewItem itema = MyStudentList.SelectedItems[0];
                    string tempItem = itema.SubItems[0].Text; // usersid
                    string tempItema = itema.SubItems[2].Text; // lectureid
                    mysql.AddNewGrade(tempItem, NewGradeText.Text,tempItema);
                    RefreshAll();
                    NewGradeText.Text = string.Empty;
                }
            }
            else
            {
                MessageBox.Show("Please enter numbers only.");
            }
        }

        private void DeleteSelectedGradeButton_Click(object sender, EventArgs e)
        {
            ListViewItem itema = LecturerFullList.SelectedItems[0];
            string tempItem = itema.SubItems[0].Text; // relationshipid
            if (String.IsNullOrWhiteSpace(tempItem))
            {
                MessageBox.Show("Please select something first.");
            }
            else
            {
                mysql.DeleteGrade(tempItem);
                RefreshAll();
            }

        }
    }
}
