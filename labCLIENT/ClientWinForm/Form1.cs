using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientWinForm
{
    public partial class Form1 : Form
    {
        int lastrow = 0;
        public Form1()
        {
            InitializeComponent();
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;


            SetUpForm();
        }
        private void SetUpForm()
        {
            SetTable();
            SetClimberCB();
            SetMountainCB();
        }
        public void SetTable()
        {
            dataGridView1.Rows.Clear();
            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri("http://localhost:49343/api/");

                // Таблица
                var responseTask = client.GetAsync("climb");
                responseTask.Wait();

                var GetResult = responseTask.Result;
                if (GetResult.IsSuccessStatusCode)
                {
                    var readTask = GetResult.Content.ReadAsAsync<SearchResultLine[]>();
                    readTask.Wait();

                    var climbs = readTask.Result;

                    foreach (SearchResultLine climb in climbs)
                    {
                        int row = dataGridView1.Rows.Add();
                        dataGridView1.Rows[row].Cells[0].Value = climb.Id;
                        dataGridView1.Rows[row].Cells[1].Value = climb.ClimberSurname;
                        dataGridView1.Rows[row].Cells[2].Value = climb.ClimberName;
                        dataGridView1.Rows[row].Cells[3].Value = climb.Mountain;
                        dataGridView1.Rows[row].Cells[4].Value = "🗑";
                    }
                }
            }
            
        }
        private void SetClimberCB()
        {
            comboBox1.DataSource = null;
            comboBox1.Items.Clear();
            using (var client = new HttpClient())
            {


                HttpResponseMessage response = client.GetAsync("http://localhost:49343/api/climber/get").Result;

                if (response.IsSuccessStatusCode)
                {
                    // Чтение содержимого ответа
                    string responseContent = response.Content.ReadAsStringAsync().Result;

                    // Десериализация JSON-строки в словарь
                    Dictionary<int, string> climbers = JsonConvert.DeserializeObject<Dictionary<int, string>>(responseContent);
                    List<KeyValuePair<int, string>> climberList = new List<KeyValuePair<int, string>>(climbers);
                    comboBox1.DisplayMember = "Value";
                    comboBox1.ValueMember = "Key";
                    comboBox1.DataSource = climberList;
                }
                else
                {
                    Console.WriteLine("Ошибка при получении данных об альпинистах. Код ошибки: " + response.StatusCode);
                }
            }
        }
        private void SetMountainCB()
        {
            comboBox2.DataSource = null;
            comboBox2.Items.Clear();
            using (var client = new HttpClient())
            {


                HttpResponseMessage response = client.GetAsync("http://localhost:49343/api/mountain/get").Result;

                if (response.IsSuccessStatusCode)
                {
                    // Чтение содержимого ответа
                    string responseContent = response.Content.ReadAsStringAsync().Result;

                    // Десериализация JSON-строки в словарь
                    Dictionary<int, string> mountains = JsonConvert.DeserializeObject<Dictionary<int, string>>(responseContent);
                    List<KeyValuePair<int, string>> mountainList = new List<KeyValuePair<int, string>>(mountains);
                    comboBox2.DisplayMember = "Value";
                    comboBox2.ValueMember = "Key";
                    comboBox2.DataSource = mountainList;
                }
                else
                {
                    Console.WriteLine("Ошибка при получении данных о вершинах. Код ошибки: " + response.StatusCode);
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            int climberName = (int)comboBox2.SelectedValue;
            int mountainName = (int)comboBox1.SelectedValue;

            string baseUrl = "http://localhost:49343/api/edit/AddClimb";
            using (var client = new HttpClient())
            {


                HttpResponseMessage response = client.GetAsync($"{baseUrl}?climberId={climberName}&mountainId={mountainName}").Result;

                if (response.IsSuccessStatusCode)
                {
                    SetUpForm();
                }
                else
                {
                    Console.WriteLine("Ошибка при добавлении. Код ошибки: " + response.StatusCode);
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView senderGrid = (DataGridView)sender;
            if (e.RowIndex < 0 || e.ColumnIndex != dataGridView1.Columns["Delete"].Index)
            {
                return;
            }

            int Id = (int)dataGridView1.Rows[e.RowIndex].Cells["Id"].Value;

            string baseUrl = "http://localhost:49343/api/climb/deleteclimb?Id=";
            string url = baseUrl + Id;

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, url)).Result;

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Запись успешно удалена.");
                    SetUpForm();
                }
                else
                {
                    Console.WriteLine("Ошибка при удалении записи. Код ошибки: " + response.StatusCode);
                }
            }
        }
    }
}
