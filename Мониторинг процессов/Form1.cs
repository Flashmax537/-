using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace Мониторинг_процессов
{
    public partial class Form1 : Form
    {
        public List<Processes> procList = new List<Processes>();

        public Form1()
        {
            InitializeComponent();
            List<Processes> procList = new List<Processes>();
            dataGridView1.AutoGenerateColumns = true;
            bindingSource1.DataSource = this.procList;

            Timer time = new Timer();
            time.Interval = 500;
            time.Tick += new EventHandler(monitor);
            time.Start();
        }

        private void monitor(object sender, EventArgs e)
        {
            try
            {
                this.procList.Clear();
                foreach (var Proc in Process.GetProcesses())
                {
                    string CPU = "";

                    try
                    {
                        CPU = Proc.TotalProcessorTime.ToString();

                    }
                    catch (Exception error)
                    {
                    }

                    this.procList.Add(new Processes(Proc.Id.ToString(), Proc.ProcessName, Proc.PeakVirtualMemorySize64.ToString(), Proc.WorkingSet64.ToString(), CPU));
                }
                dataGridView1.DataSource = this.procList;
                dataGridView1.Refresh();
            }
            catch (DataException error)
            {
                MessageBox.Show(error.Message, "Ошибка обновления данных.");
            }
        }

        public class Processes
        {
            public string PID { get; set; }
            public string Name { get; set; }
            public string Memory { get; set; }
            public string PeakVirtualMemory { get; set; }
            public string CPU { get; set; }

            public Processes(string column1, string column2, string column3, string column4, string column5)
            {
                PID = column1;
                Name = column2;
                Memory = column3;
                PeakVirtualMemory = column4;
                CPU = column5;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            saveFileDialog1.DefaultExt = "txt";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filename = saveFileDialog1.FileName;
                StreamWriter write = new StreamWriter(filename);
                string startTime = "Нет доступа к информации о процессе";
                
                foreach (var Proc in Process.GetProcesses())
                {
                    string currentproc = Proc.ProcessName;
                    try
                    {
                        startTime = Proc.StartTime.ToString();
                    }
                    catch (Exception error)
                    {
                    }
                    write.WriteLine("Имя: " + Proc.ProcessName + ", Время старта: " + startTime);
                }
                write.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string ID = "";
            int row = Convert.ToInt32(dataGridView1.CurrentCell.RowIndex);
            int pid = Convert.ToInt32(dataGridView1[0, row].Value);
            Process process = Process.GetProcessById(pid);
            string processName = process.ProcessName;
            foreach (ProcessThread thread in process.Threads)
            {
                ID += "ID: " + thread.Id + ", Состояние: " + thread.ThreadState + "\n";
            }
            MessageBox.Show(ID, "Информация о процессе " + processName);
        }
    }
}
