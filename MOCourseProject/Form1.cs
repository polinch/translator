using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace MOCourseProject
{
    public partial class Form1 : Form
    {
        Scaner Analyze = new Scaner();

        public Form1()
        {
            InitializeComponent();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Clear();
                StreamReader sr = File.OpenText(openFileDialog1.FileName);
                //строка для считывания
                string line = null;
                //чтение первой строки
                line = sr.ReadLine();
                //чтение строк из файла и запись в textBox
                while (line != null)
                {
                    textBox1.AppendText(line);
                    textBox1.AppendText("\r");
                    line = sr.ReadLine();
                    if (line != null)
                        textBox1.AppendText("\n");
                }
                sr.Close();
            }
        }

        private void translateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            Analyze = new Scaner();
            dataGridView3.Rows.Clear();
            Analyze.Str = textBox1.Text;
            string v = null;
            Item lex;
            Analyze.kar = 0;
            //Работа с магазинной памятью
            //начальное содержимое магазина - программа и маркер дна: programm = 21, маркер дна = 0 
            //Analyze.MP.Push((int)Token.end);
            Analyze.MP.Push((int)Token.program);
            int temp = Analyze.MP.Top();
            bool flag;
            lex = Analyze.ScanStr(Analyze.Str, ref Analyze.kar);
            flag = Analyze.MPauto(lex, temp, ref Analyze.rez);
            if (!flag)
            {
                MessageBox.Show("Ошибка");
                while (!Analyze.MP.IsEmpty())
                    Analyze.MP.Pop();
                Analyze.MP.Push((int)Token.end);
            }
            else if (flag){
                while (!Analyze.MP.IsEmpty())
                    Analyze.MP.Pop();
                String line = null;
                int i = 0;
                while (i < Analyze.rez.commands.Size()){
                    int elem = Analyze.rez.commands.Mas[i];
                    switch (elem){
                        case (int)Token.assign:
                            line = "Присвоить: " + Analyze.rez.commands.Mas[i + 1] + ", " + Analyze.rez.commands.Mas[i + 2] + "\r\n";
                            dataGridView3.Rows.Add(i, line);
                            line = null;
                            i += 3;
                            elem = Analyze.rez.commands.Mas[i];
                            break;
                        case (int)Token.add:
                            line = "Сложить: " + Analyze.rez.commands.Mas[i + 1] + ", " + Analyze.rez.commands.Mas[i + 2] + ", " + Analyze.rez.commands.Mas[i + 3] + "\r\n";
                            dataGridView3.Rows.Add(i, line);
                            line = null;
                            i += 4;
                            elem = Analyze.rez.commands.Mas[i];
                            break;
                        case (int)Token.mov:
                            line = "Умножить: " + Analyze.rez.commands.Mas[i + 1] + ", " + Analyze.rez.commands.Mas[i + 2] + ", " + Analyze.rez.commands.Mas[i + 3] + "\r\n";
                            dataGridView3.Rows.Add(i, line);
                            line = null;
                            i += 4;
                            elem = Analyze.rez.commands.Mas[i];
                            break;
                        case (int)Token.compare:
                            line = "Равно: " + Analyze.rez.commands.Mas[i + 1] + ", " + Analyze.rez.commands.Mas[i + 2] + ", " + Analyze.rez.commands.Mas[i + 3] + "\r\n";
                            dataGridView3.Rows.Add(i, line);
                            line = null;
                            i += 4;
                            elem = Analyze.rez.commands.Mas[i];
                            break;
                        case (int)Token.compare2:
                            line = "Не равно: " + Analyze.rez.commands.Mas[i + 1] + ", " + Analyze.rez.commands.Mas[i + 2] + ", " + Analyze.rez.commands.Mas[i + 3] + "\r\n";
                            dataGridView3.Rows.Add(i, line);
                            line = null;
                            i += 4;
                            elem = Analyze.rez.commands.Mas[i];
                            break;
                        case (int)Token.label:
                            line = "Метка: " + Analyze.rez.commands.Mas[i + 1] + "\r\n";
                            dataGridView3.Rows.Add(i, line);
                            line = null;
                            i += 2;
                            elem = Analyze.rez.commands.Mas[i];
                            break;
                        case (int)Token.condtras:
                            line = "Переход по сравнению: " + Analyze.rez.commands.Mas[i + 1] + ", " + Analyze.rez.commands.Mas[i + 2] + "\r\n";
                            dataGridView3.Rows.Add(i, line);
                            line = null;
                            i += 3;
                            elem = Analyze.rez.commands.Mas[i];
                            break;
                        case (int)Token.tras0:
                            line = "Условный переход по нулю: " + Analyze.rez.commands.Mas[i + 1] + ", " + Analyze.rez.commands.Mas[i + 2] + "\r\n";
                            dataGridView3.Rows.Add(i, line);
                            line = null;
                            i += 3;
                            elem = Analyze.rez.commands.Mas[i];
                            break;
                        case (int)Token.uncondtras:
                            line = "Безусловный переход: " + Analyze.rez.commands.Mas[i + 1] + "\r\n";
                            dataGridView3.Rows.Add(i, line);
                            line = null;
                            i += 2;
                            elem = Analyze.rez.commands.Mas[i];
                            break;
                        default:
                            //dataGridView3.Rows.Add(i, line);
                            break;
                    }
                }
                int j = 0;
                while (j < Analyze.rez.data.Size()){
                    dataGridView1.Rows.Add(j, Analyze.rez.data.Mas[j].name, Analyze.rez.data.Mas[j].value);
                    j++;
                }

                int k = 0;
                while (k < Analyze.rez.labels.Size()){
                    dataGridView2.Rows.Add(k, Analyze.rez.labels.Mas[k]);
                    k++;
                }

            }
        }

        private void programmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            Analyze.Interpret(ref Analyze.rez);
            int i = 0;
            while (i < Analyze.rez.data.Size()){
                dataGridView1.Rows.Add(i, Analyze.rez.data.Mas[i].name, Analyze.rez.data.Mas[i].value);
                i++;
            }

            int k = 0;
            while (k < Analyze.rez.labels.Size()){
                dataGridView2.Rows.Add(k, Analyze.rez.labels.Mas[k]);
                k++;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
    
