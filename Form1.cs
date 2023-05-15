using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static HW_CSharp_5.SoccerPlayer;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Xml.Linq;
using System.Data.Common;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.InteropServices.ComTypes;

// Вариант для класса - 6
// (Предметная область - Игроки спортивной команды) 

// Вариант доп. функц-ти - 2
// Отбор объектов, удовлетворяющих заданному условию для числового поля
// (например, отобразить все страны, численность которых не меньше 50 млн. человек).  

// Вариант для построения диаграммы - 2
// Столбчатая диаграмма отображает среднее значение
// фиксированного поля для каждой группы объектов
// (например, средняя стоимость автомобилей разных производителей).
// Группирующий показатель относится к типу Enum. 

namespace HW_CSharp_5
{
    public partial class Form1 : Form
    {
        private List<SoccerPlayer> lstPlayers = new List<SoccerPlayer>
        {
            new SoccerPlayer(1, "Александр Васютин", 28, Position.Вратарь, "../../img/vasyutin.png"),
            new SoccerPlayer(2, "Дмитрий Чистяков", 29, Position.Защитник, "../../img/chystjakov.png"),
            new SoccerPlayer(4, "Данил Круговой", 24, Position.Защитник, "../../img/krugovoy.png"),
            new SoccerPlayer(3, "Дуглас Сантос", 29, Position.Защитник, "../../img/santos.png"),
            new SoccerPlayer(60, "Кирилл Столбов", 19, Position.Полузащитник, "../../img/stolbov.png"),
            new SoccerPlayer(5, "Вильмар Барриос", 29, Position.Полузащитник, "../../img/barryos.png"),
            new SoccerPlayer(7, "Зелимхан Бакаев", 26, Position.Полузащитник, "../../img/bakaev.png"),
            new SoccerPlayer(33, "Иван Сергеев", 27, Position.Нападающий, "../../img/sergeev.png"),
            new SoccerPlayer(30, "Матео Кассьерра", 26, Position.Нападающий, "../../img/kessyera.png")
        };

        private BindingSource bs = new BindingSource();

        public Form1()
        {
            InitializeComponent();
            bindAll();

            /*
            int V = ('m' + 'a') % 8;
            int F = 'M' % 3;
            int D = 'A' % 3;
            MessageBox.Show("Вариант класса - " + V.ToString() +
                "\nВариант доп. функци-ти - " + F.ToString() +
                "\nВариант для построения диаграммы - " + D.ToString(), "Вариант_Muradyan_Artur");
            */
        }

        private void bindAll()
        {
            bs.DataSource = lstPlayers;

            #region bindingNavigator
            bindingNavigator.BindingSource = bs;
            #endregion

            #region dataGridView
            dataGridView.RowHeadersVisible = false;
            dataGridView.AutoGenerateColumns = false;
            dataGridView.DataSource = bs;
            dataGridView.CellValidating += dataGridView_CellValidating;
            var column1 = new DataGridViewTextBoxColumn
            {
                Width = 55,
                HeaderText = "Номер",
                Name = "Number",
                DataPropertyName = "Number"
            };
            var column2 = new DataGridViewTextBoxColumn
            {
                Width = 140,
                HeaderText = "Игрок",
                Name = "Name",
                DataPropertyName = "Name"
            };
            var column3 = new DataGridViewTextBoxColumn
            {
                Width = 60,
                HeaderText = "Возраст",
                Name = "Age",
                DataPropertyName = "Age"
            };
            var column4 = new DataGridViewComboBoxColumn
            {
                Width = 100,
                HeaderText = "Амплуа",
                Name = "position",
                DataPropertyName = "position",
                DataSource = Enum.GetValues(typeof(Position))
            };
            dataGridView.Columns.Add(column1);
            dataGridView.Columns.Add(column2);
            dataGridView.Columns.Add(column3);
            dataGridView.Columns.Add(column4);
            #endregion

            #region chart
            // В chart выводится средний возраст игроков на разных позициях
            bs.CurrentChanged += (o, e) => chart.DataBind();

            chart.Titles.Add("Средний возраст игроков Зенита в разных амплуа");
            chart.Legends.Clear();
            ChangeChartDataSource(chart, bs);

            chart.Series[0].XValueMember = "Positions";
            chart.Series[0].YValueMembers = "ageAvg";
            #endregion

            #region pictureBox
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.DataBindings.Add("ImageLocation", bs, "photoPath", true);
            pictureBox.DoubleClick += PictureBox_DoubleClick;

            System.Windows.Forms.ToolTip toolTip = new System.Windows.Forms.ToolTip();
            toolTip.SetToolTip(pictureBox, "Заменить изображение по двойному щелчку");
            #endregion

            #region propertyGrid
            propertyGrid.DataBindings.Add("SelectedObject", bs, "");
            #endregion

            #region toolStripTextBox
            // Фильтрация игроков по минимальному возрасту
            toolStripTextBox.Enter += (o,e) => toolStripTextBox.Text = "";
            toolStripTextBox.TextChanged += (o, e) =>
            {
                int minAge;

                if (int.TryParse(toolStripTextBox.Text, out minAge))
                {
                    for (int i = 0; i < dataGridView.Rows.Count-1; i++)
                    {
                        var row = dataGridView.Rows[i];
                        int currentPlayerAge = (int)row.Cells["Age"].Value;

                        if (currentPlayerAge < minAge)
                        {
                            dataGridView.CurrentCell = null;
                            row.Visible = false;
                        }
                        else row.Visible = true;
                    }
                }
            };
            toolStripTextBox.Leave += (o, e) =>
            {
                int minAge;
                if (toolStripTextBox.Text == "")
                    toolStripTextBox.Text = "Фильтрация";
                else if (!(int.TryParse(toolStripTextBox.Text, out minAge)))
                    toolStripTextBox.Text = "Введите мин. возраст!";
                else
                {
                    minAge = int.Parse(toolStripTextBox.Text);

                    toolStripTextBox.Text = "Мин. возраст: " + minAge;
                }
            };
            #endregion
        }

        private void ChangeChartDataSource(Chart chart, BindingSource bs)
        {
            chart.DataSource = from players in bs.DataSource as List<SoccerPlayer>
                               group players by players.position into positionGroup
                               select new { Positions = positionGroup.Key.ToString(), ageAvg = positionGroup.Average(player => player.age) };
        }

        private void PictureBox_DoubleClick(object sender, EventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog
            {
                InitialDirectory = Environment.CurrentDirectory,
                Filter = "Картинка в формате jpg|*.jpg|Картинка в формате jpeg|*.jpeg"
            };
            if (opf.ShowDialog() == DialogResult.OK)
            {
                (bs.Current as SoccerPlayer).photoPath = opf.FileName;
                bs.ResetBindings(false);
            }
        }
        private void dataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridViewRow row = dataGridView.Rows[e.RowIndex];
                SoccerPlayer player = (SoccerPlayer)row.DataBoundItem ?? new SoccerPlayer();

                switch (dataGridView.Columns[e.ColumnIndex].Name)
                {
                    case "Number":
                        int number;
                        if (int.TryParse(e.FormattedValue.ToString(), out number))
                            player.number = number;
                        else
                        {
                            MessageBox.Show("Недопустимое значение! Пожалуйста, введите число.");
                            e.Cancel = true;
                        }
                        break;
                    case "Name":
                        player.name = e.FormattedValue.ToString();
                        break;
                    case "Age":
                        int age;
                        if (int.TryParse(e.FormattedValue.ToString(), out age))
                            player.age = age;
                        else
                        {
                            MessageBox.Show("Недопустимое значение! Пожалуйста, введите число.");
                            e.Cancel = true;
                        }
                        break;
                    case "Position":
                        Position position;
                        if (Enum.TryParse(e.FormattedValue.ToString(), out position))
                            player.position = position;
                        break;
                }
            }
        }
        
        // Сереализация
        private void сохранитьToolStripButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            
            sfd.InitialDirectory = Environment.CurrentDirectory;
            sfd.Filter = "Файл в bin|*.bin|Файл в xml|*.xml";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                switch (sfd.FilterIndex)
                {
                    case 1:
                        BinarySerialize(sfd.FileName);
                        break;
                    case 2:
                        XmlSerialize(sfd.FileName);
                        break;
                }
            }
        }
        private void XmlSerialize(string file)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<SoccerPlayer>));

            Stream stream = new FileStream(file, FileMode.Create);
            xmlSerializer.Serialize(stream, bs.DataSource);

            stream.Close();
        }
        private void BinarySerialize(string file)
        {
            BinaryFormatter binSerializer = new BinaryFormatter();

            Stream stream = new FileStream(file, FileMode.Create);
            binSerializer.Serialize(stream, bs.DataSource);

            stream.Close();
        }
        
        // Десереализация
        private void открытьToolStripButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog sfd = new OpenFileDialog();

            sfd.InitialDirectory = Environment.CurrentDirectory;
            sfd.Filter = "Файл в bin|*.bin|Файл в xml|*.xml";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                switch (sfd.FilterIndex)
                {
                    case 1:
                        BinaryFileOpen(sfd.FileName);
                        break;
                    case 2:
                        XmlFileOpen(sfd.FileName);
                        break;
                }
            }
        }
        private void XmlFileOpen(string file)
        {
            XmlSerializer xmlDeserializer = new XmlSerializer(typeof(List<SoccerPlayer>));
            Stream stream = new FileStream(file, FileMode.Open);

            bs.DataSource = (List<SoccerPlayer>)xmlDeserializer.Deserialize(stream);
            ChangeChartDataSource(chart, bs);

            stream.Close();
        }
        private void BinaryFileOpen(string file)
        {
            BinaryFormatter binDeserializer = new BinaryFormatter();
            Stream stream = new FileStream(file, FileMode.Open);

            bs.DataSource = (List<SoccerPlayer>)binDeserializer.Deserialize(stream);
            ChangeChartDataSource(chart, bs);

            stream.Close();
        }
    }
}
